using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;

namespace tempProj
{
    /// <summary>
    /// An API server the app can use, read from the Firebase "API Hosting" collection.
    /// </summary>
    public sealed class NetworkProfile
    {
        public NetworkProfile(string name, string prefix, string serverHost, int serverPort)
        {
            Name = name;
            Prefix = prefix;
            ServerHost = serverHost;
            ServerPort = serverPort;
        }

        public string Name { get; private set; }

        /// <summary>Start of the IPv4 addresses on the server's network, e.g. "192.168.1."</summary>
        public string Prefix { get; private set; }

        public string ServerHost { get; private set; }
        public int ServerPort { get; private set; }

        public string ServerUrl
        {
            get { return "http://" + ServerHost + ":" + ServerPort; }
        }

        /// <summary>
        /// Builds a profile from a server IP such as "192.168.1.50". The network prefix is the first
        /// three parts of the address ("192.168.1."). Returns false when the IP isn't a valid IPv4 address.
        /// </summary>
        public static bool TryCreate(string name, string ip, int port, out NetworkProfile profile)
        {
            profile = null;
            ip = (ip ?? "").Trim();

            IPAddress address;
            if (!IPAddress.TryParse(ip, out address) ||
                address.AddressFamily != AddressFamily.InterNetwork ||
                ip.Count(c => c == '.') != 3 ||
                port < 1 || port > 65535)
            {
                return false;
            }

            string normalized = address.ToString();
            string prefix = normalized.Substring(0, normalized.LastIndexOf('.') + 1);

            if (string.IsNullOrWhiteSpace(name))
            {
                name = "Network " + prefix + "x";
            }

            profile = new NetworkProfile(name.Trim(), prefix, normalized, port);
            return true;
        }
    }

    public sealed class HostDirectoryResult
    {
        public List<NetworkProfile> Hosts = new List<NetworkProfile>();

        /// <summary>True when Firebase couldn't be read and the last saved copy is being used.</summary>
        public bool FromSavedCopy;

        /// <summary>Why Firebase couldn't be read (null when it worked).</summary>
        public string Error;
    }

    /// <summary>
    /// Reads the API server address(es) from Firebase instead of having them written into the app.
    ///
    /// Firestore collection "API Hosting", one document per server, with the field:
    ///   IP Address   (string)   e.g. "192.168.1.50"           required
    ///   Name         (string)   e.g. "Main office"             optional
    ///   Port         (number)   defaults to 5075               optional
    ///
    /// The list is read over Firestore's REST endpoint, so the Firestore rules must allow reading
    /// this collection without signing in (and should keep writing locked). The last good list is
    /// saved on the PC so the app still starts when Firebase can't be reached.
    /// </summary>
    public static class HostDirectory
    {
        private const string ProjectId = "radyo-pilipino-branch-platform";
        private const string CollectionName = "API Hosting";
        private const int DefaultPort = 5075;

        private static readonly HttpClient Http = new HttpClient { Timeout = TimeSpan.FromSeconds(6) };

        private static string CachePath
        {
            get
            {
                return Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    "RPMG Ticketing", "api-hosts.txt");
            }
        }

        public static async Task<HostDirectoryResult> LoadAsync()
        {
            HostDirectoryResult result = new HostDirectoryResult();

            try
            {
                List<NetworkProfile> hosts = await FetchFromFirebaseAsync();
                if (hosts.Count == 0)
                {
                    throw new InvalidOperationException(
                        "No server with a valid \"IP Address\" was found in \"" + CollectionName + "\".");
                }

                result.Hosts = hosts;
                SaveCopy(hosts);
                return result;
            }
            catch (Exception ex)
            {
                result.Error = ex.Message;
            }

            List<NetworkProfile> saved = LoadCopy();
            if (saved.Count > 0)
            {
                result.Hosts = saved;
                result.FromSavedCopy = true;
            }

            return result;
        }

        private static async Task<List<NetworkProfile>> FetchFromFirebaseAsync()
        {
            ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12;

            string url = "https://firestore.googleapis.com/v1/projects/" + ProjectId +
                         "/databases/(default)/documents/" + Uri.EscapeDataString(CollectionName) + "?pageSize=100";

            using (HttpResponseMessage response = await Http.GetAsync(url))
            {
                string body = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    string detail = null;
                    try { detail = (string)JObject.Parse(body)["error"]["message"]; } catch { }

                    if (response.StatusCode == HttpStatusCode.Forbidden)
                    {
                        throw new InvalidOperationException(
                            "Firebase did not allow reading \"" + CollectionName + "\" (check the Firestore rules).");
                    }

                    throw new InvalidOperationException(
                        "Firebase answered " + (int)response.StatusCode + (detail != null ? ": " + detail : "."));
                }

                List<NetworkProfile> hosts = new List<NetworkProfile>();
                JArray documents = JObject.Parse(body)["documents"] as JArray;
                if (documents == null)
                {
                    return hosts;
                }

                foreach (JToken document in documents)
                {
                    JObject fields = document["fields"] as JObject;
                    if (fields == null)
                    {
                        continue;
                    }

                    string ip = ReadString(fields, "IP Address", "IP", "Host");
                    string name = ReadString(fields, "Name", "Label");
                    int port = ReadInt(fields, "Port") ?? DefaultPort;

                    NetworkProfile profile;
                    if (NetworkProfile.TryCreate(name, ip, port, out profile) &&
                        !hosts.Any(h => h.ServerHost == profile.ServerHost && h.ServerPort == profile.ServerPort))
                    {
                        hosts.Add(profile);
                    }
                }

                return hosts;
            }
        }

        // Firestore REST wraps each value by type, e.g. {"stringValue": "192.168.1.50"}.
        private static JObject FindField(JObject fields, params string[] names)
        {
            foreach (string wanted in names)
            {
                foreach (JProperty property in fields.Properties())
                {
                    if (Simplify(property.Name) == Simplify(wanted))
                    {
                        return property.Value as JObject;
                    }
                }
            }

            return null;
        }

        private static string ReadString(JObject fields, params string[] names)
        {
            JObject field = FindField(fields, names);
            return field == null ? null : (string)field["stringValue"];
        }

        private static int? ReadInt(JObject fields, params string[] names)
        {
            JObject field = FindField(fields, names);
            if (field == null)
            {
                return null;
            }

            int value;
            string text = (string)field["integerValue"] ?? (string)field["stringValue"];
            return int.TryParse(text, out value) ? (int?)value : null;
        }

        private static string Simplify(string text)
        {
            return new string((text ?? "").Where(char.IsLetterOrDigit).ToArray()).ToLowerInvariant();
        }

        private static void SaveCopy(List<NetworkProfile> hosts)
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(CachePath));
                File.WriteAllLines(CachePath, hosts.Select(h =>
                    h.Name.Replace('\t', ' ') + "\t" + h.ServerHost + "\t" + h.ServerPort));
            }
            catch
            {
                // The saved copy is only a convenience; ignore problems writing it.
            }
        }

        private static List<NetworkProfile> LoadCopy()
        {
            List<NetworkProfile> hosts = new List<NetworkProfile>();

            try
            {
                if (!File.Exists(CachePath))
                {
                    return hosts;
                }

                foreach (string line in File.ReadAllLines(CachePath))
                {
                    string[] parts = line.Split('\t');
                    int port;
                    NetworkProfile profile;

                    if (parts.Length == 3 && int.TryParse(parts[2], out port) &&
                        NetworkProfile.TryCreate(parts[0], parts[1], port, out profile))
                    {
                        hosts.Add(profile);
                    }
                }
            }
            catch
            {
                hosts.Clear();
            }

            return hosts;
        }
    }

    /// <summary>
    /// Works out which API server this PC should use, so the user is not asked.
    /// The list of servers comes from Firebase (see <see cref="HostDirectory"/>).
    /// </summary>
    public static class NetworkDetector
    {
        /// <summary>
        /// Returns the server to use, or null when it can't be decided (in which case the user
        /// should be asked). A single server in the list is used straight away.
        /// </summary>
        public static async Task<NetworkProfile> DetectAsync(IList<NetworkProfile> networks)
        {
            if (networks == null || networks.Count == 0)
            {
                return null;
            }

            if (networks.Count == 1)
            {
                return networks[0];
            }

            List<string> ips = new List<string>();
            List<string> gateways = new List<string>();
            CollectAddresses(ips, gateways);

            // 1) The default gateway is the most reliable hint (virtual adapters such as VMware or
            //    Hyper-V have 192.168.x addresses too, but normally no gateway). 2) Then the PC's own IPs.
            List<NetworkProfile> candidates = null;
            foreach (List<string> addresses in new[] { gateways, ips })
            {
                List<NetworkProfile> matches = networks
                    .Where(n => addresses.Any(a => a.StartsWith(n.Prefix, StringComparison.Ordinal)))
                    .ToList();

                if (matches.Count == 1)
                {
                    return matches[0];
                }

                if (matches.Count > 1)
                {
                    candidates = matches;
                    break;
                }
            }

            // Several (or no) matches: see which of the candidate servers actually answers.
            if (candidates == null)
            {
                candidates = networks.ToList();
            }

            bool[] reachable = await Task.WhenAll(candidates.Select(n => IsReachableAsync(n, 1500)));
            List<NetworkProfile> online = candidates.Where((n, i) => reachable[i]).ToList();

            return online.Count == 1 ? online[0] : null;
        }

        /// <summary>Short description of this PC's IPv4 address and gateway, shown in the dialog.</summary>
        public static string DescribeLocalNetwork()
        {
            List<string> ips = new List<string>();
            List<string> gateways = new List<string>();
            CollectAddresses(ips, gateways);

            if (ips.Count == 0)
            {
                return "No active network connection was found on this PC. Check the cable or Wi-Fi.";
            }

            string text = "Detected on this PC:  IPv4 " + string.Join(", ", ips.Distinct());
            if (gateways.Count > 0)
            {
                text += "   |   Gateway " + string.Join(", ", gateways.Distinct());
            }
            return text;
        }

        private static void CollectAddresses(List<string> ips, List<string> gateways)
        {
            try
            {
                foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces())
                {
                    if (nic.OperationalStatus != OperationalStatus.Up ||
                        nic.NetworkInterfaceType == NetworkInterfaceType.Loopback ||
                        nic.NetworkInterfaceType == NetworkInterfaceType.Tunnel)
                    {
                        continue;
                    }

                    IPInterfaceProperties props = nic.GetIPProperties();

                    foreach (UnicastIPAddressInformation ua in props.UnicastAddresses)
                    {
                        if (ua.Address.AddressFamily == AddressFamily.InterNetwork)
                        {
                            ips.Add(ua.Address.ToString());
                        }
                    }

                    foreach (GatewayIPAddressInformation ga in props.GatewayAddresses)
                    {
                        if (ga.Address.AddressFamily == AddressFamily.InterNetwork &&
                            !ga.Address.Equals(IPAddress.Any))
                        {
                            gateways.Add(ga.Address.ToString());
                        }
                    }
                }
            }
            catch
            {
                // If the adapters can't be read we simply fall back to asking the user.
            }
        }

        /// <summary>True when the network's API server accepts a connection within the timeout.</summary>
        public static async Task<bool> IsReachableAsync(NetworkProfile network, int timeoutMs)
        {
            try
            {
                using (TcpClient client = new TcpClient())
                {
                    Task connect = client.ConnectAsync(network.ServerHost, network.ServerPort);
                    Task finished = await Task.WhenAny(connect, Task.Delay(timeoutMs));
                    if (finished != connect)
                    {
                        return false;
                    }

                    await connect;
                    return client.Connected;
                }
            }
            catch
            {
                return false;
            }
        }
    }

    /// <summary>
    /// Fallback dialog shown only when the network can't be detected automatically.
    /// </summary>
    public class NetworkSelectDialog : Form
    {
        private static readonly Color Ink = Color.FromArgb(15, 23, 42);
        private static readonly Color Slate = Color.FromArgb(100, 116, 139);
        private static readonly Color SlateDark = Color.FromArgb(51, 65, 85);
        private static readonly Color LineColor = Color.FromArgb(203, 213, 225);
        private static readonly Color Accent = Color.FromArgb(153, 27, 27);
        private static readonly Color AccentSoft = Color.FromArgb(254, 242, 242);

        private const int Pad = 28;
        private const int ContentWidth = 464;
        private const int CardHeight = 68;
        private const int CardStep = 76;       // card height + gap
        private const int NoteLineHeight = 19;
        private const int MaxListedInNote = 4;  // servers named in the "how to check" box

        private readonly IList<NetworkProfile> networks;
        private readonly List<RoundedPanel> cards = new List<RoundedPanel>();
        private readonly List<Label> dots = new List<Label>();
        private Label lblError;
        private Panel listHost;
        private int selectedIndex = -1;

        /// <summary>The network the user picked (valid when the dialog returns OK).</summary>
        public NetworkProfile SelectedNetwork { get; private set; }

        private readonly string cancelText;

        public NetworkSelectDialog(IList<NetworkProfile> networks, string detectedSummary, string cancelText = "Exit")
        {
            this.networks = networks;
            this.cancelText = cancelText;
            BuildUi(detectedSummary);
        }

        private void BuildUi(string detectedSummary)
        {
            SuspendLayout();

            Font = new Font("Segoe UI", 9F);
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.White;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Select Network";
            try { Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath); } catch { }

            Controls.Add(new Panel { Dock = DockStyle.Top, Height = 5, BackColor = Accent });

            Controls.Add(new Label
            {
                Text = "Select your network",
                Font = new Font("Segoe UI", 15F, FontStyle.Bold),
                ForeColor = Ink,
                AutoSize = false,
                Location = new Point(Pad, 24),
                Size = new Size(ContentWidth, 32)
            });

            Controls.Add(new Label
            {
                Text = "We couldn't detect your network automatically. Choose the network this computer " +
                       "is connected to so the app can reach the ticket server.",
                ForeColor = Slate,
                AutoSize = false,
                Location = new Point(Pad, 60),
                Size = new Size(ContentWidth, 44)
            });

            // Height of the "how to check your network" box: it grows with the number of lines.
            int noteLines = 3 + Math.Min(networks.Count, MaxListedInNote) +
                            (networks.Count > MaxListedInNote ? 1 : 0);
            int noteHeight = 52 + noteLines * NoteLineHeight + 14;

            // The server list scrolls when it would make the dialog taller than the screen, so the
            // buttons are never pushed off the bottom of the display.
            int listTop = 116;
            int below = 16 + noteHeight + 12 + 38 + 26 + 42 + Pad; // note, summary, error, buttons
            int maxList = Math.Max(CardStep * 2 - 8, MaxClientHeight() - listTop - below);
            int fullList = Math.Max(networks.Count, 1) * CardStep - 8;
            int listHeight = Math.Min(fullList, maxList);
            bool scrolls = fullList > listHeight;
            int cardWidth = ContentWidth - (scrolls ? SystemInformation.VerticalScrollBarWidth : 0);

            listHost = new Panel
            {
                Location = new Point(Pad, listTop),
                Size = new Size(ContentWidth, listHeight),
                AutoScroll = scrolls,
                BackColor = Color.White
            };
            Controls.Add(listHost);

            for (int i = 0; i < networks.Count; i++)
            {
                RoundedPanel card = CreateCard(networks[i], i, i * CardStep, cardWidth);
                cards.Add(card);
                listHost.Controls.Add(card);
            }

            int y = listTop + listHeight + 16;
            Controls.Add(CreateNotePanel(y, noteHeight));
            y += noteHeight + 12;

            Controls.Add(new Label
            {
                Text = detectedSummary ?? "",
                Font = new Font("Segoe UI", 8.5F),
                ForeColor = Slate,
                AutoSize = false,
                Location = new Point(Pad, y),
                Size = new Size(ContentWidth, 34)
            });
            y += 38;

            lblError = new Label
            {
                Text = "Please select a network to continue.",
                ForeColor = Accent,
                AutoSize = false,
                Visible = false,
                Location = new Point(Pad, y),
                Size = new Size(ContentWidth, 20)
            };
            Controls.Add(lblError);
            y += 26;

            RoundedButton btnContinue = new RoundedButton
            {
                Text = "Continue",
                Font = new Font("Segoe UI", 9.75F, FontStyle.Bold),
                BackColor = Color.White,
                ForeColor = Accent,
                BorderColor = Accent,
                BorderSize = 1,
                HoverBackColor = Color.FromArgb(254, 242, 242),
                PressedBackColor = Color.FromArgb(254, 226, 226),
                Cursor = Cursors.Hand,
                UseVisualStyleBackColor = false,
                Location = new Point(Pad + ContentWidth - 140, y),
                Size = new Size(140, 42)
            };
            btnContinue.Click += btnContinue_Click;

            RoundedButton btnExit = new RoundedButton
            {
                Text = cancelText,
                Font = new Font("Segoe UI", 9.75F),
                BackColor = Color.White,
                ForeColor = SlateDark,
                BorderColor = LineColor,
                BorderSize = 1,
                HoverBackColor = Color.FromArgb(248, 250, 252),
                PressedBackColor = Color.FromArgb(226, 232, 240),
                Cursor = Cursors.Hand,
                UseVisualStyleBackColor = false,
                DialogResult = DialogResult.Cancel,
                Location = new Point(Pad + ContentWidth - 140 - 12 - 110, y),
                Size = new Size(110, 42)
            };

            Controls.Add(btnContinue);
            Controls.Add(btnExit);
            AcceptButton = btnContinue;
            CancelButton = btnExit;

            ClientSize = new Size(Pad * 2 + ContentWidth, y + 42 + Pad);
            ResumeLayout(false);
        }

        private RoundedPanel CreateCard(NetworkProfile network, int index, int y, int width)
        {
            RoundedPanel card = new RoundedPanel
            {
                Location = new Point(0, y),
                Size = new Size(width, CardHeight),
                CornerRadius = 10,
                BackColor = Color.White,
                BorderColor = LineColor,
                Cursor = Cursors.Hand
            };

            Label dot = new Label
            {
                BackColor = Color.Transparent,
                Cursor = Cursors.Hand,
                Location = new Point(18, 24),
                Size = new Size(20, 20)
            };
            dot.Paint += (s, e) => DrawRadio(e.Graphics, index == selectedIndex);
            dots.Add(dot);

            Label name = new Label
            {
                Text = network.Name,
                Font = new Font("Segoe UI", 10.5F, FontStyle.Bold),
                ForeColor = Ink,
                BackColor = Color.Transparent,
                Cursor = Cursors.Hand,
                AutoSize = false,
                AutoEllipsis = true,
                Location = new Point(52, 13),
                Size = new Size(width - 68, 22)
            };

            Label detail = new Label
            {
                Text = "IP / gateway starts with " + network.Prefix + "*   \u2022   Server " + network.ServerHost,
                ForeColor = Slate,
                BackColor = Color.Transparent,
                Cursor = Cursors.Hand,
                AutoSize = false,
                AutoEllipsis = true,
                Location = new Point(52, 37),
                Size = new Size(width - 68, 20)
            };

            card.Controls.Add(dot);
            card.Controls.Add(name);
            card.Controls.Add(detail);

            EventHandler pick = (s, e) => SelectNetwork(index);
            card.Click += pick;
            dot.Click += pick;
            name.Click += pick;
            detail.Click += pick;

            return card;
        }

        private RoundedPanel CreateNotePanel(int y, int height)
        {
            RoundedPanel panel = new RoundedPanel
            {
                Location = new Point(Pad, y),
                Size = new Size(ContentWidth, height),
                CornerRadius = 10,
                BackColor = Color.FromArgb(248, 250, 252),
                BorderColor = Color.FromArgb(226, 232, 240)
            };

            panel.Controls.Add(new Label
            {
                Text = "How to check your network",
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                ForeColor = Ink,
                BackColor = Color.Transparent,
                AutoSize = false,
                Location = new Point(16, 14),
                Size = new Size(ContentWidth - 32, 22)
            });

            string body =
                "1.  Press Windows + R, type  cmd  and press Enter.\n" +
                "2.  In the Command Prompt, type  ipconfig  and press Enter.\n" +
                "3.  Under your active adapter, look at Default Gateway (or IPv4 Address):\n";
            int listed = 0;
            foreach (NetworkProfile n in networks)
            {
                if (listed == MaxListedInNote)
                {
                    body += "          \u2022  ...and " + (networks.Count - listed) + " more (see the list above)\n";
                    break;
                }

                body += "          \u2022  " + n.Prefix + "x   \u2192   choose " + n.Name + "\n";
                listed++;
            }

            panel.Controls.Add(new Label
            {
                Text = body.TrimEnd('\n'),
                ForeColor = SlateDark,
                BackColor = Color.Transparent,
                AutoSize = false,
                Location = new Point(16, 44),
                Size = new Size(ContentWidth - 32, height - 56)
            });

            return panel;
        }

        private static void DrawRadio(Graphics g, bool selected)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            using (Pen pen = new Pen(selected ? Accent : LineColor, 2f))
            {
                g.DrawEllipse(pen, 1, 1, 17, 17);
            }

            if (selected)
            {
                using (SolidBrush brush = new SolidBrush(Accent))
                {
                    g.FillEllipse(brush, 5, 5, 10, 10);
                }
            }
        }

        private void SelectNetwork(int index)
        {
            selectedIndex = index;
            lblError.Visible = false;

            for (int i = 0; i < cards.Count; i++)
            {
                bool on = i == index;
                cards[i].BorderColor = on ? Accent : LineColor;
                cards[i].BackColor = on ? AccentSoft : Color.White;
                dots[i].Invalidate();
            }

            // Keep the chosen server visible when the list is scrolling.
            if (listHost != null && index >= 0 && index < cards.Count)
            {
                listHost.ScrollControlIntoView(cards[index]);
            }
        }

        // The tallest the dialog's client area may be, so it always fits on the screen
        // (in the same design units the rest of the layout uses).
        private int MaxClientHeight()
        {
            Rectangle area = Screen.FromPoint(Cursor.Position).WorkingArea;
            float scale = 1f;
            try
            {
                using (Graphics g = CreateGraphics())
                {
                    scale = g.DpiY / 96f;
                }
            }
            catch
            {
                // Fall back to 100% scaling.
            }

            // Leave room for the title bar, the window border and some breathing space.
            return (int)((area.Height - 90) / Math.Max(scale, 1f));
        }

        // Mouse wheel scrolls the server list when it has a scroll bar.
        protected override void OnMouseWheel(MouseEventArgs e)
        {
            if (listHost != null && listHost.AutoScroll)
            {
                int top = -listHost.AutoScrollPosition.Y - e.Delta;
                listHost.AutoScrollPosition = new Point(0, Math.Max(0, top));
                return;
            }

            base.OnMouseWheel(e);
        }

        private void btnContinue_Click(object sender, EventArgs e)
        {
            if (selectedIndex < 0)
            {
                lblError.Visible = true;
                return;
            }

            SelectedNetwork = networks[selectedIndex];
            DialogResult = DialogResult.OK;
        }

        // Up / Down arrow keys move the selection.
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if ((keyData == Keys.Down || keyData == Keys.Up) && networks.Count > 0)
            {
                int step = keyData == Keys.Down ? 1 : -1;
                int next = selectedIndex < 0 ? 0 : (selectedIndex + step + networks.Count) % networks.Count;
                SelectNetwork(next);
                return true;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }
    }
}
