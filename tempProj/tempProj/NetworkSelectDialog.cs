using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace tempProj
{
    /// <summary>
    /// One of the networks the app can run on, and the API server that belongs to it.
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

        /// <summary>Start of the IPv4 addresses on this network, e.g. "192.168.1."</summary>
        public string Prefix { get; private set; }

        public string ServerHost { get; private set; }
        public int ServerPort { get; private set; }

        public string ServerUrl
        {
            get { return "http://" + ServerHost + ":" + ServerPort; }
        }
    }

    /// <summary>
    /// Works out which network this PC is connected to, so the user is not asked.
    /// To add or change a network, edit the <see cref="Networks"/> list.
    /// </summary>
    public static class NetworkDetector
    {
        public static readonly NetworkProfile[] Networks = new[]
        {
            new NetworkProfile("Network 1", "192.168.1.", "192.168.1.101", 5075),
            new NetworkProfile("Network 2", "192.168.0.", "192.168.0.184", 5075),
        };

        /// <summary>
        /// Returns the network this PC is on, or null when it can't be decided
        /// (in which case the user should be asked).
        /// </summary>
        public static async Task<NetworkProfile> DetectAsync()
        {
            List<string> ips = new List<string>();
            List<string> gateways = new List<string>();
            CollectAddresses(ips, gateways);

            // 1) The default gateway is the most reliable hint (virtual adapters such as VMware or
            //    Hyper-V have 192.168.x addresses too, but normally no gateway). 2) Then the PC's own IPs.
            List<NetworkProfile> candidates = null;
            foreach (List<string> addresses in new[] { gateways, ips })
            {
                List<NetworkProfile> matches = Networks
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
                candidates = Networks.ToList();
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

        private static async Task<bool> IsReachableAsync(NetworkProfile network, int timeoutMs)
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

        private readonly IList<NetworkProfile> networks;
        private readonly List<RoundedPanel> cards = new List<RoundedPanel>();
        private readonly List<Label> dots = new List<Label>();
        private Label lblError;
        private int selectedIndex = -1;

        /// <summary>The network the user picked (valid when the dialog returns OK).</summary>
        public NetworkProfile SelectedNetwork { get; private set; }

        public NetworkSelectDialog(IList<NetworkProfile> networks, string detectedSummary)
        {
            this.networks = networks;
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

            int y = 116;
            for (int i = 0; i < networks.Count; i++)
            {
                RoundedPanel card = CreateCard(networks[i], i, y);
                cards.Add(card);
                Controls.Add(card);
                y += 76;
            }

            y += 8;
            Controls.Add(CreateNotePanel(y));
            y += 176 + 12;

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
                Text = "Exit",
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

        private RoundedPanel CreateCard(NetworkProfile network, int index, int y)
        {
            RoundedPanel card = new RoundedPanel
            {
                Location = new Point(Pad, y),
                Size = new Size(ContentWidth, 68),
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
                Location = new Point(52, 13),
                Size = new Size(396, 22)
            };

            Label detail = new Label
            {
                Text = "IP / gateway starts with " + network.Prefix + "*   \u2022   Server " + network.ServerHost,
                ForeColor = Slate,
                BackColor = Color.Transparent,
                Cursor = Cursors.Hand,
                AutoSize = false,
                Location = new Point(52, 37),
                Size = new Size(396, 20)
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

        private RoundedPanel CreateNotePanel(int y)
        {
            RoundedPanel panel = new RoundedPanel
            {
                Location = new Point(Pad, y),
                Size = new Size(ContentWidth, 176),
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
            foreach (NetworkProfile n in networks)
            {
                body += "          \u2022  " + n.Prefix + "x   \u2192   choose " + n.Name + "\n";
            }

            panel.Controls.Add(new Label
            {
                Text = body.TrimEnd('\n'),
                ForeColor = SlateDark,
                BackColor = Color.Transparent,
                AutoSize = false,
                Location = new Point(16, 44),
                Size = new Size(ContentWidth - 32, 120)
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
