using System;
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

var app = builder.Build();

app.Urls.Add("http://0.0.0.0:5075");

var firestore = await FirestoreDb.CreateAsync("tempdatabase-23468");
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

app.MapPost("/tickets", async (CreateTicketRequest request) =>
{
    var doc = firestore.Collection("Tickets").Document();
    var now = Timestamp.GetCurrentTimestamp();
    var data = new Dictionary<string, object>
    {
        ["ComputerName"] = request.ComputerName,
        ["CreatedAt"] = now,
        ["CreatedBy"] = request.CreatedBy,
        ["Description"] = request.Description,
        ["Status"] = request.Status,
        ["Title"] = request.Title,
        ["UpdatedAt"] = now
    };

    await doc.SetAsync(data);
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

app.MapPut("/tickets/{id}", async (string id, UpdateTicketRequest request) =>
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

    if (!isOwner)
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

    await doc.UpdateAsync(updates);
    return Results.Ok(new { Id = id, Message = "Ticket updated succesfully" });
});

app.MapDelete("/tickets/{id}", async (string id, string createdBy, string computerName) =>
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

    if (!isOwner)
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
}
public class UpdateTicketRequest
{
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";
    public string Status { get; set; } = "";
    public string CreatedBy { get; set; } = "";
    public string ComputerName { get; set; } = "";
}
