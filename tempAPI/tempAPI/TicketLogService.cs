using Google.Cloud.Firestore;

// ---------------------------------------------------------------------------------------------
// Ticket history log.
//
// Every change made to a ticket (created, status changed, reopened, edited, deleted) is written as
// ONE immutable log entry:
//
//   1. PRIMARY  - Firebase project "ticketing-system-9219d", collection "TicketLogs"
//                 (the source of truth, next to the Tickets collection).
//   2. MIRROR   - Firebase project "radyo-pilipino-branch-platform", collection "ticketLogs"
//                 (the admin portal's project, so the portal's "Ticket Logs" tab can read it with
//                 its normal sign-in + Firestore rules, without opening up the ticketing project).
//
// The mirror needs a SECOND service-account key placed next to the API exe as
// "portal-service-account.json" (service account of radyo-pilipino-branch-platform).
// If that file is missing, the API still works and logs to the primary project only.
// The same document ID is used in both projects, so a retry/backfill never creates duplicates.
// ---------------------------------------------------------------------------------------------

public static class TicketLogAction
{
    public const string Created = "created";
    public const string StatusChanged = "status_changed";
    public const string Reopened = "reopened";
    public const string Edited = "edited";
    public const string Deleted = "deleted";
}

public sealed class TicketLogEntry
{
    public string TicketId { get; init; } = "";
    public string TicketTitle { get; init; } = "";
    public string Action { get; init; } = "";
    public string OldStatus { get; init; } = "";
    public string NewStatus { get; init; } = "";
    /// <summary>Windows user who made the change.</summary>
    public string ChangedBy { get; init; } = "";
    /// <summary>Computer the change was made from.</summary>
    public string ComputerName { get; init; } = "";
    public string ClientIp { get; init; } = "";
    public bool ByAdmin { get; init; }
    /// <summary>Windows user who originally created the ticket.</summary>
    public string TicketOwner { get; init; } = "";
    public string Note { get; init; } = "";
    /// <summary>Reason / remarks typed by the user in the Windows app (max 500 characters).</summary>
    public string Remarks { get; init; } = "";
}

public sealed class TicketLogService
{
    private const string PrimaryCollection = "TicketLogs";
    private const string MirrorCollection = "ticketLogs";

    private readonly FirestoreDb _primary;
    private readonly FirestoreDb? _mirror;
    private readonly ILogger _logger;

    public TicketLogService(FirestoreDb primary, FirestoreDb? mirror, ILogger logger)
    {
        _primary = primary;
        _mirror = mirror;
        _logger = logger;
    }

    public bool MirrorEnabled => _mirror != null;

    public static string CleanRemarks(string? remarks)
    {
        string text = (remarks ?? "").Trim();
        return text.Length <= 500 ? text : text[..500];
    }

    /// <summary>Builds the client for the admin portal's Firebase project, or null when its key file is absent.</summary>
    public static FirestoreDb? CreateMirror(string portalProjectId, ILogger logger)
    {
        string path = Path.Combine(AppContext.BaseDirectory, "portal-service-account.json");
        if (!File.Exists(path))
        {
            logger.LogWarning(
                "portal-service-account.json not found next to the API: ticket logs will be saved to the ticketing project only " +
                "(the admin portal's Ticket Logs tab stays empty until it is added).");
            return null;
        }

        try
        {
            return new FirestoreDbBuilder
            {
                ProjectId = portalProjectId,
                CredentialsPath = path
            }.Build();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Could not connect to the admin portal's Firebase project '{Project}'.", portalProjectId);
            return null;
        }
    }

    /// <summary>
    /// Records one history entry. Never throws: a logging problem must not stop a ticket from being
    /// created, updated or deleted.
    /// </summary>
    public async Task RecordAsync(TicketLogEntry entry)
    {
        var data = new Dictionary<string, object>
        {
            ["ticketId"] = entry.TicketId,
            ["ticketTitle"] = entry.TicketTitle,
            ["action"] = entry.Action,
            ["oldStatus"] = entry.OldStatus,
            ["newStatus"] = entry.NewStatus,
            ["changedBy"] = entry.ChangedBy,
            ["computerName"] = entry.ComputerName,
            ["clientIp"] = entry.ClientIp,
            ["byAdmin"] = entry.ByAdmin,
            ["ticketOwner"] = entry.TicketOwner,
            ["note"] = entry.Note,
            ["remarks"] = CleanRemarks(entry.Remarks),
            ["occurredAt"] = Timestamp.GetCurrentTimestamp()
        };

        string logId = _primary.Collection(PrimaryCollection).Document().Id;

        try
        {
            await _primary.Collection(PrimaryCollection).Document(logId).SetAsync(data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Could not save ticket log {LogId} (ticket {TicketId}) to the ticketing project.", logId, entry.TicketId);
        }

        if (_mirror == null) return;

        try
        {
            await _mirror.Collection(MirrorCollection).Document(logId).SetAsync(data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Could not mirror ticket log {LogId} (ticket {TicketId}) to the admin portal project.", logId, entry.TicketId);
        }
    }
}
