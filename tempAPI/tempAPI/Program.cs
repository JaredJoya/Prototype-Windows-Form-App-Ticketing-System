using System;
using System.IO;
using System.Net;
using System.Linq;
using System.Collections.Generic;
using Google.Cloud.Firestore;
using System.Diagnostics.Contracts;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("Email"));
builder.Services.AddSingleton<EmailService>();

var app = builder.Build();
var emailService = app.Services.GetRequiredService<EmailService>();

app.Urls.Add("http://0.0.0.0:5075");

var credentialsPath = Path.Combine(
    AppContext.BaseDirectory,
    "firestore-service-account.json"
);

if (!File.Exists(credentialsPath))
{
    throw new FileNotFoundException(
        "Firestore credentials file was not found.",
        credentialsPath
    );
}

Environment.SetEnvironmentVariable(
    "GOOGLE_APPLICATION_CREDENTIALS",
    credentialsPath,
    EnvironmentVariableTarget.Process
);

var firestore = await FirestoreDb.CreateAsync("ticketing-system-9219d");
var philippinesTimeZone = TimeZoneInfo.FindSystemTimeZoneById(OperatingSystem.IsWindows() ? "Singapore Standard Time" : "Asia/Manila");

app.MapGet("/firestore-test", async () =>
{
    try
    {
        var collections = firestore.ListRootCollectionsAsync();
        var names = new List<string>();

        await foreach (var collection in collections)
        {
            names.Add(collection.Id);
        }

        return Results.Ok(new
        {
            connected = true,
            collections = names
        });
    }
    catch (Exception ex)
    {
        return Results.Problem(ex.Message);
    }
});
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

//app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

var adminIpAddresses = new HashSet<String>(StringComparer.OrdinalIgnoreCase)
{
    "192.168.0.184"
};

bool isAdminClient(HttpContext context)
{
    IPAddress? remoteIp = context.Connection.RemoteIpAddress;

    if (remoteIp == null)
        return false;
    if (remoteIp.IsIPv4MappedToIPv6)
        remoteIp = remoteIp.MapToIPv4();
    return adminIpAddresses.Contains(remoteIp.ToString());
}

app.MapGet("/permissions", (HttpContext context) =>
{
    return Results.Ok(new
    {
        IsAdmin = isAdminClient(context)
    });
});

app.MapGet("/health", () =>
{
    return Results.Ok(new { status = "Ticketing API is running" });
});

app.MapGet("/tickets", async () =>
{
    var collection = firestore.Collection("Tickets");
    var snapshot = await collection.GetSnapshotAsync();

    var tickets = snapshot.Documents.Select(doc =>
    {
        var data = doc.ToDictionary();
        string GetString(string field)
        {
            return data.TryGetValue(field, out var value) ? value?.ToString() ?? "" : "";
        }
        string? GetDate(string field)
        {
            if (!data.TryGetValue(field, out var value) || value == null)
            {
                return null;
            }
            if (value is Timestamp timestamp)
            {
                DateTime philippinesTime = TimeZoneInfo.ConvertTimeFromUtc(timestamp.ToDateTime(), philippinesTimeZone);
                return philippinesTime.ToString("yyyy-MM-dd hh:mm tt", CultureInfo.InvariantCulture);
            }
            if (value is DateTime dateTime)
            {
                DateTime utcTime = dateTime.Kind switch
                {
                    DateTimeKind.Utc => dateTime,
                    DateTimeKind.Local => dateTime.ToUniversalTime(),
                    _=> DateTime.SpecifyKind(dateTime, DateTimeKind.Utc)
                };

                DateTime philippinesTime = TimeZoneInfo.ConvertTimeFromUtc(utcTime, philippinesTimeZone);
                return philippinesTime.ToString("yyyy-MM-dd hh:mm tt", CultureInfo.InvariantCulture);
            }

            return value.ToString();
        }

        return new
        {
            Id = doc.Id,
            ComputerName = GetString("ComputerName"),
            CreatedAt = GetDate("CreatedAt"),
            CreatedBy = GetString("CreatedBy"),
            Description = GetString("Description"),
            Status = GetString("Status"),
            Title = GetString("Title"),
            UpdatedAt = GetDate("UpdatedAt"),
        };
    }).ToList();

    return Results.Ok(tickets);
});

string FormatManila(DateTime utc) =>
    TimeZoneInfo.ConvertTimeFromUtc(utc, philippinesTimeZone).ToString("yyyy-MM-dd hh:mm tt", CultureInfo.InvariantCulture);

app.MapPost("/tickets", async (CreateTicketRequest request) =>
{
    // 1) Validate the requester's email before doing anything else.
    string? emailError = await EmailService.ValidateRecipientAsync(request.ReceiptEmail);
    if (emailError != null)
        return Results.BadRequest(new { message = emailError });

    // The ID can be generated without writing anything to Firestore yet.
    var doc = firestore.Collection("Tickets").Document();
    var now = Timestamp.GetCurrentTimestamp();
    string when = FormatManila(now.ToDateTime());

    // 2) The admin must be notified first. If that email fails, the ticket is NOT created.
    bool adminNotified = await emailService.SendNewTicketToAdminAsync(
        doc.Id, request.ReceiptEmail, request.Title, request.Description,
        request.Status, request.ComputerName, request.CreatedBy, when);

    if (!adminNotified)
    {
        return Results.Json(new
        {
            message = "The admin could not be notified by email, so the ticket was not created. Please try again or contact IT."
        }, statusCode: 502);
    }

    // 3) Save the ticket.
    var data = new Dictionary<string, object>
    {
        ["ComputerName"] = request.ComputerName,
        ["CreatedAt"] = now,
        ["CreatedBy"] = request.CreatedBy,
        ["Description"] = request.Description,
        ["Status"] = request.Status,
        ["Title"] = request.Title,
        ["ReceiptEmail"] = request.ReceiptEmail,
        ["UpdatedAt"] = now
    };
    await doc.SetAsync(data);

    // 4) Receipt to the requester (background; a failure here is only logged).
    _ = Task.Run(() => emailService.SendReceiptToRequesterAsync(
        doc.Id, request.ReceiptEmail, request.Title, request.Description,
        request.Status, request.ComputerName, request.CreatedBy, when));

    return Results.Ok(new { Id = doc.Id });
});

app.MapGet("/tickets/{id}", async (string id) =>
{
    var doc = firestore.Collection("Tickets").Document(id);
    var snapshot = await doc.GetSnapshotAsync();

    if (!snapshot.Exists)
    {
        return Results.NotFound(new
        {
            message = "Ticket not found."
        });
    }

    var data = snapshot.ToDictionary();
    string GetString(string field)
    {
        return data.TryGetValue(field, out var value) ? value?.ToString() ?? "" : "";
    }
    string GetDate(string field)
    {
        if (!data.TryGetValue(field, out var value) || value == null)
            return "";
        if (value is Timestamp timestamp)
            return timestamp.ToDateTime().ToString("yyyy-MM-dd hh:mm tt");
        if (value is DateTime dateTime)
            return dateTime.ToString("yyyy-MM-dd hh:mm tt");
        return value.ToString() ?? "";
    }

    return Results.Ok(new
    {
        Id = snapshot.Id,
        ComputerName = GetString("ComputerName"),
        CreatedAt = GetDate("CreatedAt"),
        CreatedBy = GetString("CreatedBy"),
        Description = GetString("Description"),
        Status = GetString("Status"),
        Title = GetString("Title"),
        UpdatedAt = GetDate("UpdatedAt")
    });
});

app.MapPut("/tickets/{id}", async (string id, UpdateTicketRequest request, HttpContext context) =>
{
    var doc = firestore.Collection("Tickets").Document(id);
    var snapshot = await doc.GetSnapshotAsync();

    if (!snapshot.Exists)
    {
        return Results.NotFound(new
        {
            message = "Ticket not found"
        });
    }

    var existing = snapshot.ToDictionary();
    string owner = existing.TryGetValue("CreatedBy", out var user)
    ? user?.ToString() ?? "" : "";
    string ownerComputer = existing.TryGetValue("ComputerName", out var computer) 
    ? computer?.ToString() ?? "" : "";

    bool isOwner = string.Equals(owner, request.CreatedBy, StringComparison.OrdinalIgnoreCase) &&
    string.Equals(ownerComputer, request.ComputerName, StringComparison.OrdinalIgnoreCase);

    bool isAdmin = isAdminClient(context);

    if (!isOwner && !isAdmin)
    {
        return Results.Json(new
        {
            message = "You are not authorized to update this ticket."
        }, statusCode: 403);
    }

    var updates = new Dictionary<string, object>
    {
        ["Title"] = request.Title,
        ["Description"] = request.Description,
        ["Status"] = request.Status,
        ["UpdatedAt"] = Timestamp.GetCurrentTimestamp()
    };

    string oldStatus = existing.TryGetValue("Status", out var st) ? st?.ToString() ?? "" : "";
    string ticketTitle = request.Title;
    string requesterEmail = existing.TryGetValue("ReceiptEmail", out var em) ? em?.ToString() ?? "" : "";

    await doc.UpdateAsync(updates);

    if (!string.Equals(oldStatus, request.Status, StringComparison.OrdinalIgnoreCase))
    {
        string when = FormatManila(DateTime.UtcNow);
        _ = Task.Run(() => emailService.SendStatusChangedAsync(
            id, requesterEmail, ticketTitle, oldStatus, request.Status, request.CreatedBy, request.ComputerName, when));
    }

    return Results.Ok(new { Id = id, Message = "Ticket updated succesfully" });
});

app.MapDelete("/tickets/{id}", async (string id, string createdBy, string computerName, HttpContext context) =>
{
    var doc = firestore.Collection("Tickets").Document(id);
    var snapshot = await doc.GetSnapshotAsync();

    if (!snapshot.Exists)
    {
        return Results.NotFound(new
        {
            message = "Ticket not found."
        });
    }

    var existing = snapshot.ToDictionary();
    string owner = existing.TryGetValue("CreatedBy", out var user)
    ? user?.ToString() ?? "" : "";
    string ownerComputer = existing.TryGetValue("ComputerName", out var computer)
    ? computer?.ToString() ?? "" : "";

    bool isOwner = string.Equals(owner, createdBy, StringComparison.OrdinalIgnoreCase) &&
    string.Equals(ownerComputer, computerName, StringComparison.OrdinalIgnoreCase);

    bool isAdmin = isAdminClient(context);

    if (!isOwner && !isAdmin)
    {
        return Results.Json(new
        {
            message = "You are not authorized to delete this ticket."
        }, statusCode: 403);
    }

    await doc.DeleteAsync();
    return Results.Ok(new 
    { 
        Id = id,
        message = "Ticket deleted successfully."
    });
});

app.Run();

public class CreateTicketRequest
{
    public string ComputerName { get; set; } = "";
    public string CreatedBy { get; set; } = "";
    public string Description { get; set; } = "";
    public string Status { get; set; } = "";
    public string Title { get; set; } = "";
    public string ReceiptEmail { get; set; } = "";
}
public class UpdateTicketRequest
{
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";
    public string Status { get; set; } = "";
    public string CreatedBy { get; set; } = "";
    public string ComputerName { get; set; } = "";
}


public class EmailSettings
{
    public string SmtpHost { get; set; } = "smtp.gmail.com";
    public int SmtpPort { get; set; } = 587;
    public string SenderEmail { get; set; } = "";
    public string SenderName { get; set; } = "Ticketing System";
    public string AppPassword { get; set; } = "";   // Gmail App Password (not the normal password)
    public string AdminEmail { get; set; } = "";
}

public class EmailService
{
    private readonly EmailSettings _settings;
    private readonly ILogger<EmailService> _logger;

    public EmailService(Microsoft.Extensions.Options.IOptions<EmailSettings> options, ILogger<EmailService> logger)
    {
        _settings = options.Value;
        _logger = logger;
    }

    // Email format + the domain must exist. (A mailbox can't be verified without sending to it.)
    public static async Task<string?> ValidateRecipientAsync(string? email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return "An email address is required for the ticket receipt.";
        email = email.Trim();

        System.Net.Mail.MailAddress parsed;
        try { parsed = new System.Net.Mail.MailAddress(email); }
        catch (FormatException) { return "The email address is not valid."; }
        if (!string.Equals(parsed.Address, email, StringComparison.OrdinalIgnoreCase) || !parsed.Host.Contains('.'))
            return "The email address is not valid.";

        try
        {
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(4));
            await System.Net.Dns.GetHostEntryAsync(parsed.Host, cts.Token);
        }
        catch (System.Net.Sockets.SocketException ex) when (
            ex.SocketErrorCode == System.Net.Sockets.SocketError.HostNotFound ||
            ex.SocketErrorCode == System.Net.Sockets.SocketError.NoData)
        {
            return $"The email domain '{parsed.Host}' does not exist.";
        }
        catch { /* DNS temporarily unavailable: don't block the ticket for that */ }

        return null;
    }

    // Notify the admin about the new ticket. Returns true only if the mail was accepted for delivery.
    public Task<bool> SendNewTicketToAdminAsync(string id, string requesterEmail, string title, string description,
        string status, string computer, string user, string when)
    {
        string adminText =
            $"A new ticket has been submitted.\n\n" +
            $"Ticket ID : {id}\nTitle     : {title}\nStatus    : {status}\n" +
            $"Submitted by: {user} ({computer})\nRequester email: {requesterEmail}\nCreated   : {when}\n\n" +
            $"Description:\n{description}\n";
        string adminHtml = EmailTemplate.Render(
            headline: "New Ticket Submitted",
            intro: "A new request has just been submitted and is waiting for your attention.",
            id: id, title: title,
            statusBlock: EmailTemplate.Badge(status),
            rows: EmailTemplate.Row("Submitted by", user) + EmailTemplate.Row("Computer", computer) +
                  EmailTemplate.Row("Requester email", requesterEmail) + EmailTemplate.Row("Created", when),
            description: description,
            footer: "This is an automated notification from the Ticketing System.");

        return SendAsync(new List<string> { _settings.AdminEmail },
            $"[New Ticket] {title} (#{id})", adminText, adminHtml);
    }

    // Receipt for the requester, sent after the admin was notified.
    public Task<bool> SendReceiptToRequesterAsync(string id, string requesterEmail, string title, string description,
        string status, string computer, string user, string when)
    {
        string userText =
            $"Your request has been sent to the admin.\n\n" +
            $"Ticket ID : {id}\nTitle     : {title}\nStatus    : {status}\n" +
            $"Computer  : {computer}\nUser      : {user}\nCreated   : {when}\n\n" +
            $"Description:\n{description}\n";
        string userHtml = EmailTemplate.Render(
            headline: "Request Sent to Admin",
            intro: "Your ticket was delivered to the admin successfully. We'll email you whenever its status changes.",
            id: id, title: title,
            statusBlock: EmailTemplate.Badge(status),
            rows: EmailTemplate.Row("Computer", computer) + EmailTemplate.Row("Submitted by", user) + EmailTemplate.Row("Created", when),
            description: description,
            footer: "Keep this email as your receipt. Quote the ticket ID if you need to follow up.");

        return SendAsync(new List<string> { requesterEmail },
            $"[Request Sent to Admin] {title} (#{id})", userText, userHtml);
    }

    public Task SendStatusChangedAsync(string id, string requesterEmail, string title,
        string oldStatus, string newStatus, string changedBy, string computer, string when)
    {
        string text =
            $"The status of a ticket has changed.\n\n" +
            $"Ticket ID : {id}\nTitle     : {title}\n" +
            $"Status    : {oldStatus}  ->  {newStatus}\n" +
            $"Changed by: {changedBy} ({computer})\nWhen      : {when}\n";
        string html = EmailTemplate.Render(
            headline: "Status Updated",
            intro: "The status of this ticket has just changed.",
            id: id, title: title,
            statusBlock: EmailTemplate.Badge(oldStatus) + " <span style=\"color:#94a3b8;font-size:18px;padding:0 6px;\">&rarr;</span> " + EmailTemplate.Badge(newStatus),
            rows: EmailTemplate.Row("Changed by", changedBy) + EmailTemplate.Row("Computer", computer) + EmailTemplate.Row("When", when),
            description: null,
            footer: "This is an automated notification from the Ticketing System.");
        var recipients = new List<string> { _settings.AdminEmail, requesterEmail };
        return SendAsync(recipients, $"[Ticket Status: {newStatus}] {title} (#{id})", text, html);
    }

    private async Task<bool> SendAsync(IEnumerable<string> recipients, string subject, string textBody, string htmlBody)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(_settings.SenderEmail) || string.IsNullOrWhiteSpace(_settings.AppPassword))
            {
                _logger.LogWarning("Email not configured (Email:SenderEmail / Email:AppPassword); skipping '{Subject}'.", subject);
                return false;
            }

            var to = recipients
                .Where(r => !string.IsNullOrWhiteSpace(r))
                .Select(r => r.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();
            if (to.Count == 0) return false;

            using var message = new System.Net.Mail.MailMessage
            {
                From = new System.Net.Mail.MailAddress(_settings.SenderEmail, _settings.SenderName),
                Subject = subject,
                Body = textBody,
                IsBodyHtml = false
            };
            // HTML version for modern clients; plain text above is the fallback.
            message.AlternateViews.Add(System.Net.Mail.AlternateView.CreateAlternateViewFromString(
                htmlBody, System.Text.Encoding.UTF8, "text/html"));
            foreach (var addr in to)
            {
                try { message.To.Add(addr); }
                catch (FormatException) { _logger.LogWarning("Skipping invalid email address '{Addr}'.", addr); }
            }
            if (message.To.Count == 0) return false;

            using var client = new System.Net.Mail.SmtpClient(_settings.SmtpHost, _settings.SmtpPort)
            {
                EnableSsl = true,
                Credentials = new System.Net.NetworkCredential(_settings.SenderEmail, _settings.AppPassword)
            };
            await client.SendMailAsync(message);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email '{Subject}'.", subject);
            return false;
        }
    }
}

public static class EmailTemplate
{
    private static string E(string? v) => System.Net.WebUtility.HtmlEncode(v ?? "");

    private static (string Bg, string Fg) StatusColors(string status) => status.Trim().ToLowerInvariant() switch
    {
        "requesting"    => ("#dbeafe", "#1d4ed8"),
        "in processing" => ("#fef3c7", "#b45309"),
        "outsourcing"   => ("#ede9fe", "#6d28d9"),
        "complete"      => ("#dcfce7", "#15803d"),
        _               => ("#e2e8f0", "#334155")
    };

    public static string Badge(string status)
    {
        var (bg, fg) = StatusColors(status);
        return $"<span style=\"display:inline-block;background:{bg};color:{fg};font-size:13px;font-weight:700;padding:6px 14px;border-radius:999px;\">{E(status)}</span>";
    }

    public static string Row(string label, string value) =>
        $"<tr><td style=\"padding:9px 0;color:#64748b;font-size:13px;width:120px;border-bottom:1px solid #eef2f7;\">{E(label)}</td>" +
        $"<td style=\"padding:9px 0;color:#0f172a;font-size:14px;font-weight:600;border-bottom:1px solid #eef2f7;\">{E(value)}</td></tr>";

    public static string Render(string headline, string intro, string id, string title,
        string statusBlock, string rows, string? description, string footer)
    {
        string descBlock = string.IsNullOrWhiteSpace(description) ? "" :
            "<div style=\"margin-top:22px;\"><div style=\"font-size:12px;font-weight:700;letter-spacing:.08em;color:#64748b;text-transform:uppercase;margin-bottom:8px;\">Description</div>" +
            "<div style=\"background:#f8fafc;border:1px solid #e2e8f0;border-radius:8px;padding:14px 16px;color:#1e293b;font-size:14px;line-height:1.6;\">" +
            E(description).Replace("\r\n", "\n").Replace("\n", "<br>") + "</div></div>";

        return Html
            .Replace("{{HEADLINE}}", E(headline))
            .Replace("{{INTRO}}", E(intro))
            .Replace("{{TICKET_ID}}", E(id))
            .Replace("{{TITLE}}", E(title))
            .Replace("{{STATUS_BLOCK}}", statusBlock)
            .Replace("{{ROWS}}", rows)
            .Replace("{{DESCRIPTION_BLOCK}}", descBlock)
            .Replace("{{FOOTER}}", E(footer));
    }

    private const string Html = """
<!DOCTYPE html>
<html><head><meta charset="utf-8"><meta name="viewport" content="width=device-width,initial-scale=1"></head>
<body style="margin:0;padding:0;background:#eef1f6;font-family:'Segoe UI',Arial,Helvetica,sans-serif;">
<table role="presentation" width="100%" cellpadding="0" cellspacing="0" style="background:#eef1f6;padding:28px 12px;"><tr><td align="center">
  <table role="presentation" width="600" cellpadding="0" cellspacing="0" style="width:100%;max-width:600px;background:#ffffff;border-radius:14px;overflow:hidden;box-shadow:0 2px 10px rgba(15,23,42,.08);">
    <tr><td style="background:#1e2a47;padding:22px 32px;">
      <table role="presentation" width="100%" cellpadding="0" cellspacing="0"><tr>
        <td style="color:#ffffff;font-size:17px;font-weight:700;letter-spacing:.02em;">Ticketing System</td>
        <td align="right" style="color:#94a3b8;font-size:12px;">Automated receipt</td>
      </tr></table>
    </td></tr>
    <tr><td style="padding:32px 32px 8px 32px;">
      <div style="font-size:26px;font-weight:800;color:#0f172a;line-height:1.2;">{{HEADLINE}}</div>
      <div style="font-size:14px;color:#475569;line-height:1.6;margin-top:8px;">{{INTRO}}</div>
    </td></tr>
    <tr><td style="padding:16px 32px 8px 32px;">
      <table role="presentation" width="100%" cellpadding="0" cellspacing="0" style="border:1px solid #e2e8f0;border-radius:10px;"><tr><td style="padding:18px 20px;">
        <div style="font-size:12px;font-weight:700;letter-spacing:.08em;color:#64748b;text-transform:uppercase;">Ticket #{{TICKET_ID}}</div>
        <div style="font-size:19px;font-weight:700;color:#0f172a;margin:6px 0 14px 0;">{{TITLE}}</div>
        <div style="margin-bottom:12px;">{{STATUS_BLOCK}}</div>
        <table role="presentation" width="100%" cellpadding="0" cellspacing="0">{{ROWS}}</table>
      </td></tr></table>
      {{DESCRIPTION_BLOCK}}
    </td></tr>
    <tr><td style="padding:22px 32px 30px 32px;">
      <div style="border-top:1px solid #e2e8f0;padding-top:16px;color:#94a3b8;font-size:12px;line-height:1.6;">{{FOOTER}}</div>
    </td></tr>
  </table>
</td></tr></table>
</body></html>
""";
}
