using System.Net;
using System.Text.Json;

// ---------------------------------------------------------------------------------------------
// Admin list: the client IP addresses that are allowed to act as admin (reopen / edit / delete
// any ticket). It is stored in admins.json next to the API's exe so it survives restarts, and it
// can be viewed and changed from the console each time the API starts (see AdminConsole).
// ---------------------------------------------------------------------------------------------

public sealed class AdminEntry
{
    public string Ip { get; set; } = "";
    public string Name { get; set; } = "";
}

internal sealed class AdminFile
{
    public List<AdminEntry> Admins { get; set; } = new();
}

public sealed class AdminList
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        PropertyNameCaseInsensitive = true
    };

    private readonly List<AdminEntry> _entries;

    private AdminList(string filePath, List<AdminEntry> entries)
    {
        FilePath = filePath;
        _entries = entries;
    }

    public string FilePath { get; }
    public IReadOnlyList<AdminEntry> Entries => _entries;

    /// <summary>Loads admins.json, creating it (with the previous built-in admin) on first run.</summary>
    public static AdminList LoadOrCreate()
    {
        string path = Path.Combine(AppContext.BaseDirectory, "admins.json");

        if (!File.Exists(path))
        {
            var seeded = new AdminList(path, new List<AdminEntry>
            {
                new AdminEntry { Ip = "192.168.0.184", Name = "Default admin" }
            });
            seeded.TrySave();
            return seeded;
        }

        try
        {
            AdminFile? file = JsonSerializer.Deserialize<AdminFile>(File.ReadAllText(path), JsonOptions);
            var entries = new List<AdminEntry>();

            foreach (AdminEntry e in file?.Admins ?? new List<AdminEntry>())
            {
                if (TryNormalize(e.Ip, out string ip) && !entries.Any(x => x.Ip == ip))
                {
                    entries.Add(new AdminEntry { Ip = ip, Name = e.Name ?? "" });
                }
            }

            return new AdminList(path, entries);
        }
        catch (Exception ex)
        {
            // Never overwrite a file we couldn't read: keep a copy before starting empty.
            string backup = path + ".bak";
            try { File.Copy(path, backup, overwrite: true); } catch { }
            Console.WriteLine($"Could not read {path}: {ex.Message}");
            Console.WriteLine($"A copy was saved as {backup}. Starting with an empty admin list.");
            return new AdminList(path, new List<AdminEntry>());
        }
    }

    /// <summary>True when this client address is on the admin list.</summary>
    public bool Contains(IPAddress? ip)
    {
        if (ip == null)
            return false;
        if (ip.IsIPv4MappedToIPv6)
            ip = ip.MapToIPv4();

        string text = ip.ToString();
        return _entries.Any(e => string.Equals(e.Ip, text, StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>Returns null on success, otherwise a message to show the person.</summary>
    public string? Add(string ipText, string name)
    {
        if (!TryNormalize(ipText, out string ip))
            return $"'{ipText.Trim()}' is not a valid IP address (example: 192.168.0.184).";
        if (_entries.Any(e => e.Ip == ip))
            return $"{ip} is already in the list.";

        _entries.Add(new AdminEntry { Ip = ip, Name = name.Trim() });
        return TrySave();
    }

    /// <summary>Changes an entry. A blank IP or name keeps the current value.</summary>
    public string? Update(int index, string? ipText, string? name)
    {
        AdminEntry entry = _entries[index];
        string newIp = entry.Ip;

        if (!string.IsNullOrWhiteSpace(ipText))
        {
            if (!TryNormalize(ipText, out newIp))
                return $"'{ipText.Trim()}' is not a valid IP address (example: 192.168.0.184).";
            if (_entries.Where((e, i) => i != index).Any(e => e.Ip == newIp))
                return $"{newIp} is already in the list.";
        }

        entry.Ip = newIp;
        if (!string.IsNullOrWhiteSpace(name))
            entry.Name = name.Trim();

        return TrySave();
    }

    public string? RemoveAt(int index)
    {
        _entries.RemoveAt(index);
        return TrySave();
    }

    public static bool TryNormalize(string? text, out string normalized)
    {
        normalized = "";
        text = text?.Trim();

        if (string.IsNullOrEmpty(text) || !IPAddress.TryParse(text, out IPAddress? ip))
            return false;

        // IPAddress.TryParse also accepts short forms such as "1" or "192.168.1"; require all 4 parts.
        if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork && text.Count(c => c == '.') != 3)
            return false;

        if (ip.IsIPv4MappedToIPv6)
            ip = ip.MapToIPv4();

        normalized = ip.ToString();
        return true;
    }

    private string? TrySave()
    {
        try
        {
            File.WriteAllText(FilePath, JsonSerializer.Serialize(new AdminFile { Admins = _entries }, JsonOptions));
            return null;
        }
        catch (Exception ex)
        {
            return $"Changes could not be saved to {FilePath}: {ex.Message}";
        }
    }
}

public static class AdminConsole
{
    /// <summary>
    /// The menu only makes sense when someone is at the console. It is skipped when the API runs
    /// as a service / redirected input, or when started with --no-menu.
    /// </summary>
    public static bool CanPrompt(string[] args) =>
        !args.Contains("--no-menu", StringComparer.OrdinalIgnoreCase) &&
        Environment.UserInteractive &&
        !Console.IsInputRedirected;

    public static void Run(AdminList admins)
    {
        string? notice = null;

        while (true)
        {
            Draw(admins, notice);
            notice = null;

            Console.Write("  Choose an option: ");
            string? choice = Console.ReadLine();
            if (choice == null)
                return; // input closed: just carry on and start

            switch (choice.Trim().ToUpperInvariant())
            {
                case "":
                case "S":
                    return;

                case "A":
                    notice = AddAdmin(admins);
                    break;

                case "E":
                    notice = EditAdmin(admins);
                    break;

                case "R":
                    notice = RemoveAdmin(admins);
                    break;

                case "Q":
                    Environment.Exit(0);
                    return;

                default:
                    notice = "Unknown option. Type A, E, R, S or Q.";
                    break;
            }
        }
    }

    public static void PrintList(AdminList admins)
    {
        Console.WriteLine($"Admins ({admins.Entries.Count}): " +
            (admins.Entries.Count == 0 ? "none" : string.Join(", ", admins.Entries.Select(e => e.Ip))));
    }

    private static void Draw(AdminList admins, string? notice)
    {
        try { Console.Clear(); } catch { /* no real console window: just keep printing */ }

        Console.WriteLine("==============================================================");
        Console.WriteLine("  Ticketing API  -  admin list");
        Console.WriteLine("==============================================================");
        Console.WriteLine($"  Stored in: {admins.FilePath}");
        Console.WriteLine();

        if (admins.Entries.Count == 0)
        {
            Console.WriteLine("  (no admins yet)");
            Console.WriteLine("  Nobody will be able to reopen or manage other people's tickets.");
        }
        else
        {
            Console.WriteLine($"  {"#",-4}{"IP address",-26}Name / note");
            Console.WriteLine($"  {"--",-4}{"-----------------------",-26}-----------");
            for (int i = 0; i < admins.Entries.Count; i++)
            {
                AdminEntry e = admins.Entries[i];
                Console.WriteLine($"  {i + 1,-4}{e.Ip,-26}{e.Name}");
            }
        }

        Console.WriteLine();
        Console.WriteLine("  [A] Add admin     [E] Edit admin     [R] Remove admin");
        Console.WriteLine("  [S] or Enter: done, start the API     [Q] Quit");
        Console.WriteLine();

        if (notice != null)
        {
            Console.WriteLine($"  >> {notice}");
            Console.WriteLine();
        }
    }

    private static string? AddAdmin(AdminList admins)
    {
        Console.Write("  IP address to add (Enter to cancel): ");
        string ip = Console.ReadLine() ?? "";
        if (string.IsNullOrWhiteSpace(ip))
            return null;

        Console.Write("  Name or note (optional): ");
        string name = Console.ReadLine() ?? "";

        string? error = admins.Add(ip, name);
        if (error != null)
            return error;

        AdminList.TryNormalize(ip, out string added);
        return $"Added {added}.";
    }

    private static string? EditAdmin(AdminList admins)
    {
        int? index = PickNumber(admins, "edit");
        if (index == null)
            return null;

        AdminEntry current = admins.Entries[index.Value];
        Console.Write($"  New IP address [{current.Ip}] (Enter to keep): ");
        string ip = Console.ReadLine() ?? "";
        Console.Write($"  New name [{current.Name}] (Enter to keep): ");
        string name = Console.ReadLine() ?? "";

        return admins.Update(index.Value, ip, name) ?? "Admin updated.";
    }

    private static string? RemoveAdmin(AdminList admins)
    {
        int? index = PickNumber(admins, "remove");
        if (index == null)
            return null;

        AdminEntry current = admins.Entries[index.Value];
        Console.Write($"  Remove {current.Ip} {(current.Name.Length > 0 ? $"({current.Name}) " : "")}? [y/N]: ");
        string answer = (Console.ReadLine() ?? "").Trim();
        if (!answer.Equals("y", StringComparison.OrdinalIgnoreCase))
            return "Nothing removed.";

        return admins.RemoveAt(index.Value) ?? $"Removed {current.Ip}.";
    }

    private static int? PickNumber(AdminList admins, string verb)
    {
        if (admins.Entries.Count == 0)
            return null;

        Console.Write($"  Number of the admin to {verb} (Enter to cancel): ");
        string text = (Console.ReadLine() ?? "").Trim();
        if (text.Length == 0)
            return null;

        if (int.TryParse(text, out int number) && number >= 1 && number <= admins.Entries.Count)
            return number - 1;

        Console.WriteLine("  That number isn't in the list.");
        Console.Write("  Press Enter to continue...");
        Console.ReadLine();
        return null;
    }
}
