using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace tempProj
{
    public enum ServerDialogChoice
    {
        Exit,
        Retry,
        ChooseNetwork
    }

    /// <summary>
    /// Shown when the API server for the selected network doesn't answer.
    /// Lets the user retry, pick another network, or exit.
    /// </summary>
    public class ServerUnreachableDialog : Form
    {
        private static readonly Color Ink = Color.FromArgb(15, 23, 42);
        private static readonly Color Slate = Color.FromArgb(100, 116, 139);
        private static readonly Color SlateDark = Color.FromArgb(51, 65, 85);
        private static readonly Color LineColor = Color.FromArgb(203, 213, 225);
        private static readonly Color Accent = Color.FromArgb(153, 27, 27);
        private static readonly Color AccentSoft = Color.FromArgb(254, 242, 242);

        private const int Pad = 28;
        private const int ContentWidth = 464;

        /// <summary>What the user picked. Closing the window any other way counts as Exit.</summary>
        public ServerDialogChoice Choice { get; private set; }

        public ServerUnreachableDialog(NetworkProfile network, string detectedSummary)
        {
            Choice = ServerDialogChoice.Exit;
            BuildUi(network, detectedSummary);
        }

        private void BuildUi(NetworkProfile network, string detectedSummary)
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
            Text = "Connection Problem";
            try { Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath); } catch { }

            Controls.Add(new Panel { Dock = DockStyle.Top, Height = 5, BackColor = Accent });

            Label icon = new Label
            {
                BackColor = Color.Transparent,
                Location = new Point(Pad, 22),
                Size = new Size(40, 40)
            };
            icon.Paint += DrawWarningIcon;
            Controls.Add(icon);

            Controls.Add(new Label
            {
                Text = "Can't reach the ticket server",
                Font = new Font("Segoe UI", 15F, FontStyle.Bold),
                ForeColor = Ink,
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleLeft,
                Location = new Point(Pad + 52, 27),
                Size = new Size(ContentWidth - 52, 32)
            });

            Controls.Add(new Label
            {
                Text = "The app couldn't connect to the server for this network. This usually means " +
                       "the API isn't running yet, or this PC isn't on the network.",
                ForeColor = Slate,
                AutoSize = false,
                Location = new Point(Pad, 76),
                Size = new Size(ContentWidth, 44)
            });

            int y = 128;
            Controls.Add(CreateServerCard(network, y));
            y += 56 + 12;

            Controls.Add(CreateNotePanel(y));
            y += 140 + 10;

            Controls.Add(new Label
            {
                Text = detectedSummary ?? "",
                Font = new Font("Segoe UI", 8.5F),
                ForeColor = Slate,
                AutoSize = false,
                Location = new Point(Pad, y),
                Size = new Size(ContentWidth, 34)
            });
            y += 44;

            RoundedButton btnRetry = CreateButton("Retry", 120, true, ServerDialogChoice.Retry);
            RoundedButton btnChoose = CreateButton("Choose network", 150, false, ServerDialogChoice.ChooseNetwork);
            RoundedButton btnExit = CreateButton("Exit", 100, false, ServerDialogChoice.Exit);

            btnRetry.Location = new Point(Pad + ContentWidth - 120, y);
            btnChoose.Location = new Point(btnRetry.Left - 12 - 150, y);
            btnExit.Location = new Point(btnChoose.Left - 12 - 100, y);

            Controls.Add(btnRetry);
            Controls.Add(btnChoose);
            Controls.Add(btnExit);
            AcceptButton = btnRetry;
            CancelButton = btnExit;

            ClientSize = new Size(Pad * 2 + ContentWidth, y + 42 + Pad);
            ResumeLayout(false);
        }

        private RoundedButton CreateButton(string text, int width, bool primary, ServerDialogChoice choice)
        {
            RoundedButton button = new RoundedButton
            {
                Text = text,
                Font = new Font("Segoe UI", 9.75F, primary ? FontStyle.Bold : FontStyle.Regular),
                BackColor = Color.White,
                ForeColor = primary ? Accent : SlateDark,
                BorderColor = primary ? Accent : LineColor,
                BorderSize = 1,
                HoverBackColor = primary ? Color.FromArgb(254, 242, 242) : Color.FromArgb(248, 250, 252),
                PressedBackColor = primary ? Color.FromArgb(254, 226, 226) : Color.FromArgb(226, 232, 240),
                Cursor = Cursors.Hand,
                UseVisualStyleBackColor = false,
                Size = new Size(width, 42)
            };

            button.Click += (s, e) =>
            {
                Choice = choice;
                DialogResult = DialogResult.OK;
            };

            return button;
        }

        private RoundedPanel CreateServerCard(NetworkProfile network, int y)
        {
            RoundedPanel card = new RoundedPanel
            {
                Location = new Point(Pad, y),
                Size = new Size(ContentWidth, 56),
                CornerRadius = 10,
                BackColor = AccentSoft,
                BorderColor = Accent
            };

            card.Controls.Add(new Label
            {
                Text = "Trying to reach",
                Font = new Font("Segoe UI", 8.5F),
                ForeColor = Slate,
                BackColor = Color.Transparent,
                AutoSize = false,
                Location = new Point(16, 8),
                Size = new Size(ContentWidth - 32, 16)
            });

            card.Controls.Add(new Label
            {
                Text = network.Name + "   \u2022   " + network.ServerHost + ":" + network.ServerPort,
                Font = new Font("Segoe UI", 10.5F, FontStyle.Bold),
                ForeColor = Ink,
                BackColor = Color.Transparent,
                AutoSize = false,
                Location = new Point(16, 26),
                Size = new Size(ContentWidth - 32, 22)
            });

            return card;
        }

        private RoundedPanel CreateNotePanel(int y)
        {
            RoundedPanel panel = new RoundedPanel
            {
                Location = new Point(Pad, y),
                Size = new Size(ContentWidth, 140),
                CornerRadius = 10,
                BackColor = Color.FromArgb(248, 250, 252),
                BorderColor = Color.FromArgb(226, 232, 240)
            };

            panel.Controls.Add(new Label
            {
                Text = "What to check",
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                ForeColor = Ink,
                BackColor = Color.Transparent,
                AutoSize = false,
                Location = new Point(16, 14),
                Size = new Size(ContentWidth - 32, 22)
            });

            panel.Controls.Add(new Label
            {
                Text = "1.  Make sure the API is running on the server PC.\n" +
                       "2.  Check that this PC is connected to the network (cable or Wi-Fi).\n" +
                       "3.  Picked the wrong network? Click Choose network to switch.",
                ForeColor = SlateDark,
                BackColor = Color.Transparent,
                AutoSize = false,
                Location = new Point(16, 44),
                Size = new Size(ContentWidth - 32, 84)
            });

            return panel;
        }

        private static void DrawWarningIcon(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            using (SolidBrush fill = new SolidBrush(AccentSoft))
            {
                g.FillEllipse(fill, 0, 0, 38, 38);
            }

            using (Pen pen = new Pen(Accent, 2f))
            {
                g.DrawEllipse(pen, 1, 1, 36, 36);
            }

            using (Font font = new Font("Segoe UI", 16F, FontStyle.Bold))
            {
                TextRenderer.DrawText(g, "!", font, new Rectangle(0, 0, 38, 38), Accent,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPadding);
            }
        }
    }
}
