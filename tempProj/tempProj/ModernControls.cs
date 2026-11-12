using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace tempProj
{
    /// <summary>
    /// Shared drawing helpers for the custom controls below.
    /// </summary>
    internal static class GraphicsHelper
    {
        public static GraphicsPath RoundedRect(Rectangle bounds, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            int diameter = Math.Min(radius * 2, Math.Min(bounds.Width, bounds.Height));

            if (diameter <= 0)
            {
                path.AddRectangle(bounds);
                return path;
            }

            Rectangle arc = new Rectangle(bounds.Location, new Size(diameter, diameter));
            path.AddArc(arc, 180, 90);
            arc.X = bounds.Right - diameter;
            path.AddArc(arc, 270, 90);
            arc.Y = bounds.Bottom - diameter;
            path.AddArc(arc, 0, 90);
            arc.X = bounds.Left;
            path.AddArc(arc, 90, 90);
            path.CloseFigure();
            return path;
        }

        /// <summary>
        /// Finds the first opaque background colour up the parent chain,
        /// used to paint the corners outside a rounded shape.
        /// </summary>
        public static Color ParentBack(Control control)
        {
            Control parent = control.Parent;
            while (parent != null && parent.BackColor.A < 255)
            {
                parent = parent.Parent;
            }
            return parent != null ? parent.BackColor : SystemColors.Control;
        }
    }

    /// <summary>
    /// A panel with rounded corners and an optional border (used for "cards").
    /// </summary>
    public class RoundedPanel : Panel
    {
        private static readonly Color DefaultBorder = Color.FromArgb(226, 232, 240);

        private int cornerRadius = 10;
        private int borderThickness = 1;
        private Color borderColor = DefaultBorder;
        private Color focusBorderColor = Color.Empty;
        private bool childFocused;

        public RoundedPanel()
        {
            SetStyle(ControlStyles.UserPaint |
                     ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.ResizeRedraw |
                     ControlStyles.SupportsTransparentBackColor, true);
            BackColor = Color.White;
        }

        [Category("Appearance"), DefaultValue(10)]
        public int CornerRadius
        {
            get { return cornerRadius; }
            set { cornerRadius = Math.Max(0, value); Invalidate(); }
        }

        [Category("Appearance"), DefaultValue(1)]
        public int BorderThickness
        {
            get { return borderThickness; }
            set { borderThickness = Math.Max(0, value); Invalidate(); }
        }

        [Category("Appearance")]
        public Color BorderColor
        {
            get { return borderColor; }
            set { borderColor = value; Invalidate(); }
        }

        /// <summary>
        /// When set (not Empty), the panel shows this border colour plus a soft glow
        /// while any control inside it has keyboard focus.
        /// </summary>
        [Category("Appearance")]
        public Color FocusBorderColor
        {
            get { return focusBorderColor; }
            set { focusBorderColor = value; Invalidate(); }
        }

        private bool ShouldSerializeBorderColor() { return borderColor != DefaultBorder; }
        private void ResetBorderColor() { BorderColor = DefaultBorder; }
        private bool ShouldSerializeFocusBorderColor() { return !focusBorderColor.IsEmpty; }
        private void ResetFocusBorderColor() { FocusBorderColor = Color.Empty; }

        protected override void OnControlAdded(ControlEventArgs e)
        {
            base.OnControlAdded(e);
            e.Control.GotFocus += ChildFocusChanged;
            e.Control.LostFocus += ChildFocusChanged;
        }

        protected override void OnControlRemoved(ControlEventArgs e)
        {
            base.OnControlRemoved(e);
            e.Control.GotFocus -= ChildFocusChanged;
            e.Control.LostFocus -= ChildFocusChanged;
        }

        private void ChildFocusChanged(object sender, EventArgs e)
        {
            bool focused = false;
            foreach (Control c in Controls)
            {
                if (c.Focused)
                {
                    focused = true;
                    break;
                }
            }

            if (focused != childFocused)
            {
                childFocused = focused;
                Invalidate();
            }
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            // Everything is painted in OnPaint.
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.Clear(GraphicsHelper.ParentBack(this));
            g.SmoothingMode = SmoothingMode.AntiAlias;

            Rectangle rect = new Rectangle(0, 0, Width - 1, Height - 1);
            if (rect.Width <= 0 || rect.Height <= 0)
            {
                return;
            }

            using (GraphicsPath path = GraphicsHelper.RoundedRect(rect, cornerRadius))
            using (SolidBrush brush = new SolidBrush(BackColor))
            {
                g.FillPath(brush, path);

                if (childFocused && !focusBorderColor.IsEmpty)
                {
                    // Soft glow along the edge, then the coloured border just inside it.
                    using (Pen glow = new Pen(Color.FromArgb(50, focusBorderColor), 4f))
                    {
                        g.DrawPath(glow, path);
                    }

                    Rectangle inner = Rectangle.Inflate(rect, -2, -2);
                    if (inner.Width > 0 && inner.Height > 0)
                    {
                        using (GraphicsPath innerPath = GraphicsHelper.RoundedRect(inner, Math.Max(0, cornerRadius - 2)))
                        using (Pen pen = new Pen(focusBorderColor, Math.Max(1, borderThickness)))
                        {
                            g.DrawPath(pen, innerPath);
                        }
                    }
                }
                else if (borderThickness > 0)
                {
                    using (Pen pen = new Pen(borderColor, borderThickness))
                    {
                        g.DrawPath(pen, path);
                    }
                }
            }
        }
    }

    /// <summary>
    /// Icons available on a RoundedButton (drawn with the Segoe MDL2 Assets font, Windows 10/11).
    /// </summary>
    public enum ButtonGlyph
    {
        None,
        Add,
        Document,
        Refresh,
        Download
    }

    /// <summary>
    /// A flat button with rounded corners, hover/pressed colours and an optional icon.
    /// </summary>
    public class RoundedButton : Button
    {
        private static readonly Color DefaultBorder = Color.FromArgb(226, 232, 240);

        // Look of a disabled button (flat grey-blue, same as the Update button)
        private static readonly Color DisabledBack = Color.FromArgb(241, 245, 249);
        private static readonly Color DisabledBorder = Color.FromArgb(203, 213, 225);

        private int cornerRadius = 8;
        private int borderSize = 0;
        private Color borderColor = DefaultBorder;
        private Color hoverBackColor = Color.Empty;
        private Color pressedBackColor = Color.Empty;
        private Color iconColor = Color.Empty;
        private ButtonGlyph glyph = ButtonGlyph.None;
        private Font iconFont;
        private bool hovering;
        private bool pressing;

        public RoundedButton()
        {
            SetStyle(ControlStyles.UserPaint |
                     ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.ResizeRedraw, true);
            iconFont = new Font("Segoe MDL2 Assets", 11F, FontStyle.Regular, GraphicsUnit.Point);
        }

        [Category("Appearance"), DefaultValue(8)]
        public int CornerRadius
        {
            get { return cornerRadius; }
            set { cornerRadius = Math.Max(0, value); Invalidate(); }
        }

        [Category("Appearance"), DefaultValue(0)]
        public int BorderSize
        {
            get { return borderSize; }
            set { borderSize = Math.Max(0, value); Invalidate(); }
        }

        [Category("Appearance")]
        public Color BorderColor
        {
            get { return borderColor; }
            set { borderColor = value; Invalidate(); }
        }

        [Category("Appearance")]
        public Color HoverBackColor
        {
            get { return hoverBackColor; }
            set { hoverBackColor = value; Invalidate(); }
        }

        [Category("Appearance")]
        public Color PressedBackColor
        {
            get { return pressedBackColor; }
            set { pressedBackColor = value; Invalidate(); }
        }

        [Category("Appearance")]
        public Color IconColor
        {
            get { return iconColor; }
            set { iconColor = value; Invalidate(); }
        }

        [Category("Appearance"), DefaultValue(ButtonGlyph.None)]
        public ButtonGlyph Glyph
        {
            get { return glyph; }
            set { glyph = value; Invalidate(); }
        }

        private bool ShouldSerializeBorderColor() { return borderColor != DefaultBorder; }
        private void ResetBorderColor() { BorderColor = DefaultBorder; }
        private bool ShouldSerializeHoverBackColor() { return !hoverBackColor.IsEmpty; }
        private void ResetHoverBackColor() { HoverBackColor = Color.Empty; }
        private bool ShouldSerializePressedBackColor() { return !pressedBackColor.IsEmpty; }
        private void ResetPressedBackColor() { PressedBackColor = Color.Empty; }
        private bool ShouldSerializeIconColor() { return !iconColor.IsEmpty; }
        private void ResetIconColor() { IconColor = Color.Empty; }

        private string GlyphText
        {
            get
            {
                switch (glyph)
                {
                    case ButtonGlyph.Add: return "";
                    case ButtonGlyph.Document: return "";
                    case ButtonGlyph.Refresh: return "";
                    case ButtonGlyph.Download: return "";
                    default: return "";
                }
            }
        }

        private static bool IsLeft(ContentAlignment a)
        {
            return a == ContentAlignment.TopLeft ||
                   a == ContentAlignment.MiddleLeft ||
                   a == ContentAlignment.BottomLeft;
        }

        private static bool IsRight(ContentAlignment a)
        {
            return a == ContentAlignment.TopRight ||
                   a == ContentAlignment.MiddleRight ||
                   a == ContentAlignment.BottomRight;
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            hovering = true;
            Invalidate();
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            hovering = false;
            pressing = false;
            Invalidate();
        }

        protected override void OnMouseDown(MouseEventArgs mevent)
        {
            base.OnMouseDown(mevent);
            if (mevent.Button == MouseButtons.Left)
            {
                pressing = true;
                Invalidate();
            }
        }

        protected override void OnMouseUp(MouseEventArgs mevent)
        {
            base.OnMouseUp(mevent);
            pressing = false;
            Invalidate();
        }

        protected override void OnEnabledChanged(EventArgs e)
        {
            base.OnEnabledChanged(e);
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs pevent)
        {
            Graphics g = pevent.Graphics;
            g.Clear(GraphicsHelper.ParentBack(this));
            g.SmoothingMode = SmoothingMode.AntiAlias;

            Color fill = BackColor;
            Color border = borderColor;
            if (!Enabled)
            {
                fill = DisabledBack;
                border = DisabledBorder;
            }
            else
            {
                if (pressing && !pressedBackColor.IsEmpty)
                {
                    fill = pressedBackColor;
                }
                else if (hovering && !hoverBackColor.IsEmpty)
                {
                    fill = hoverBackColor;
                }
            }

            Rectangle rect = new Rectangle(0, 0, Width - 1, Height - 1);
            if (rect.Width > 0 && rect.Height > 0)
            {
                using (GraphicsPath path = GraphicsHelper.RoundedRect(rect, cornerRadius))
                using (SolidBrush brush = new SolidBrush(fill))
                {
                    g.FillPath(brush, path);

                    if (borderSize > 0)
                    {
                        using (Pen pen = new Pen(border, borderSize))
                        {
                            g.DrawPath(pen, path);
                        }
                    }
                }
            }

            DrawContent(g);
        }

        private void DrawContent(Graphics g)
        {
            Color textColor = Enabled ? ForeColor : Color.FromArgb(148, 163, 184);
            Color glyphColor = !Enabled ? textColor : (iconColor.IsEmpty ? ForeColor : iconColor);

            TextFormatFlags flags = TextFormatFlags.NoPadding | TextFormatFlags.NoPrefix;
            Size infinite = new Size(int.MaxValue, int.MaxValue);

            string glyphText = GlyphText;
            bool hasIcon = glyphText.Length > 0;
            bool hasText = !string.IsNullOrEmpty(Text);

            Size iconSize = hasIcon ? TextRenderer.MeasureText(g, glyphText, iconFont, infinite, flags) : Size.Empty;
            Size textSize = hasText ? TextRenderer.MeasureText(g, Text, Font, infinite, flags) : Size.Empty;

            int gap = (hasIcon && hasText) ? 10 : 0;
            int total = iconSize.Width + gap + textSize.Width;

            int x;
            if (IsLeft(TextAlign))
            {
                x = Padding.Left;
            }
            else if (IsRight(TextAlign))
            {
                x = Width - Padding.Right - total;
            }
            else
            {
                x = (Width - total) / 2;
            }

            if (hasIcon)
            {
                int y = (Height - iconSize.Height) / 2;
                TextRenderer.DrawText(g, glyphText, iconFont, new Point(x, y), glyphColor, flags);
                x += iconSize.Width + gap;
            }

            if (hasText)
            {
                int y = (Height - textSize.Height) / 2;
                TextRenderer.DrawText(g, Text, Font, new Point(x, y), textColor, flags);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && iconFont != null)
            {
                iconFont.Dispose();
                iconFont = null;
            }
            base.Dispose(disposing);
        }
    }

    /// <summary>
    /// A small fully-rounded label ("4 Items" badge). Set BackColor, BorderColor and ForeColor.
    /// </summary>
    public class PillLabel : Label
    {
        private static readonly Color DefaultBorder = Color.FromArgb(226, 232, 240);

        private Color borderColor = DefaultBorder;

        public PillLabel()
        {
            SetStyle(ControlStyles.UserPaint |
                     ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.ResizeRedraw, true);
            AutoSize = true;
        }

        [Category("Appearance")]
        public Color BorderColor
        {
            get { return borderColor; }
            set { borderColor = value; Invalidate(); }
        }

        private bool ShouldSerializeBorderColor() { return borderColor != DefaultBorder; }
        private void ResetBorderColor() { BorderColor = DefaultBorder; }

        public override Size GetPreferredSize(Size proposedSize)
        {
            Size text = TextRenderer.MeasureText(Text, Font, new Size(int.MaxValue, int.MaxValue),
                TextFormatFlags.NoPadding | TextFormatFlags.NoPrefix);
            return new Size(text.Width + 22, text.Height + 8);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.Clear(GraphicsHelper.ParentBack(this));
            g.SmoothingMode = SmoothingMode.AntiAlias;

            Rectangle rect = new Rectangle(0, 0, Width - 1, Height - 1);
            if (rect.Width <= 0 || rect.Height <= 0)
            {
                return;
            }

            using (GraphicsPath path = GraphicsHelper.RoundedRect(rect, rect.Height / 2))
            using (SolidBrush brush = new SolidBrush(BackColor))
            using (Pen pen = new Pen(borderColor, 1))
            {
                g.FillPath(brush, path);
                g.DrawPath(pen, path);
            }

            TextRenderer.DrawText(g, Text, Font, ClientRectangle, ForeColor,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter |
                TextFormatFlags.SingleLine | TextFormatFlags.NoPadding | TextFormatFlags.NoPrefix);
        }
    }

    /// <summary>
    /// Draws grey "hint" text inside an empty text box (works on every .NET version).
    /// </summary>
    internal static class HintPainter
    {
        public static readonly Color DefaultColor = Color.FromArgb(148, 163, 184);

        public static void Draw(Control control, string hint, Color color, bool multiline)
        {
            if (string.IsNullOrEmpty(hint) || control.Text.Length > 0)
            {
                return;
            }

            TextFormatFlags flags = TextFormatFlags.Left | TextFormatFlags.Top |
                                    TextFormatFlags.NoPadding | TextFormatFlags.NoPrefix;
            if (multiline)
            {
                flags |= TextFormatFlags.WordBreak;
            }

            using (Graphics g = control.CreateGraphics())
            {
                Rectangle r = control.ClientRectangle;
                r.X += 1;
                r.Width = Math.Max(0, r.Width - 1);
                TextRenderer.DrawText(g, hint, control.Font, r, color, control.BackColor, flags);
            }
        }
    }

    /// <summary>
    /// A TextBox that shows HintText (e.g. "Enter ticket title...") while it is empty.
    /// </summary>
    public class HintTextBox : TextBox
    {
        private const int WM_PAINT = 0x000F;

        private string hintText = "";
        private Color hintColor = HintPainter.DefaultColor;

        [Category("Appearance"), DefaultValue("")]
        public string HintText
        {
            get { return hintText; }
            set { hintText = value ?? ""; Invalidate(); }
        }

        [Category("Appearance")]
        public Color HintColor
        {
            get { return hintColor; }
            set { hintColor = value; Invalidate(); }
        }

        private bool ShouldSerializeHintColor() { return hintColor != HintPainter.DefaultColor; }
        private void ResetHintColor() { HintColor = HintPainter.DefaultColor; }

        protected override void OnTextChanged(EventArgs e)
        {
            base.OnTextChanged(e);
            Invalidate();
        }

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            if (m.Msg == WM_PAINT && IsHandleCreated)
            {
                HintPainter.Draw(this, hintText, hintColor, Multiline);
            }
        }
    }

    /// <summary>
    /// A RichTextBox that shows HintText while it is empty.
    /// </summary>
    public class HintRichTextBox : RichTextBox
    {
        private const int WM_PAINT = 0x000F;

        private string hintText = "";
        private Color hintColor = HintPainter.DefaultColor;

        [Category("Appearance"), DefaultValue("")]
        public string HintText
        {
            get { return hintText; }
            set { hintText = value ?? ""; Invalidate(); }
        }

        [Category("Appearance")]
        public Color HintColor
        {
            get { return hintColor; }
            set { hintColor = value; Invalidate(); }
        }

        private bool ShouldSerializeHintColor() { return hintColor != HintPainter.DefaultColor; }
        private void ResetHintColor() { HintColor = HintPainter.DefaultColor; }

        protected override void OnTextChanged(EventArgs e)
        {
            base.OnTextChanged(e);
            Invalidate();
        }

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            if (m.Msg == WM_PAINT && IsHandleCreated)
            {
                HintPainter.Draw(this, hintText, hintColor, true);
            }
        }
    }

    /// <summary>
    /// A drop-down list whose face is repainted flat (white, chevron arrow) so it matches the card style.
    /// Place it inside a RoundedPanel to get the rounded border.
    /// The native painting is skipped completely, so no native edge/line can show through.
    /// </summary>
    public class ModernComboBox : ComboBox
    {
        private const int WM_ERASEBKGND = 0x0014;
        private const int WM_PAINT = 0x000F;
        private const int WM_NCPAINT = 0x0085;
        private static readonly Color ChevronColor = Color.FromArgb(100, 116, 139);

        public ModernComboBox()
        {
            DropDownStyle = ComboBoxStyle.DropDownList;
            BackColor = Color.White;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct RECT
        {
            public int left, top, right, bottom;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct PAINTSTRUCT
        {
            public IntPtr hdc;
            public bool fErase;
            public RECT rcPaint;
            public bool fRestore;
            public bool fIncUpdate;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
            public byte[] rgbReserved;
        }

        [DllImport("user32.dll")]
        private static extern IntPtr GetWindowDC(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);

        [DllImport("user32.dll")]
        private static extern IntPtr BeginPaint(IntPtr hWnd, out PAINTSTRUCT lpPaint);

        [DllImport("user32.dll")]
        private static extern bool EndPaint(IntPtr hWnd, ref PAINTSTRUCT lpPaint);

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_ERASEBKGND && IsHandleCreated)
            {
                // We paint every pixel ourselves in WM_PAINT.
                m.Result = (IntPtr)1;
                return;
            }

            if (m.Msg == WM_PAINT && IsHandleCreated)
            {
                // Do NOT call base: the native paint is what draws the stray line.
                PAINTSTRUCT ps;
                IntPtr hdc = BeginPaint(Handle, out ps);
                try
                {
                    using (Graphics g = Graphics.FromHdc(hdc))
                    {
                        DrawFace(g, new Rectangle(0, 0, Math.Max(Width, ClientSize.Width), Math.Max(Height, ClientSize.Height)));
                    }
                }
                finally
                {
                    EndPaint(Handle, ref ps);
                }
                m.Result = IntPtr.Zero;
                return;
            }

            base.WndProc(ref m);

            if (m.Msg == WM_NCPAINT && IsHandleCreated)
            {
                // Cover the native non-client edge (if any) as well.
                IntPtr hdc = GetWindowDC(Handle);
                if (hdc == IntPtr.Zero)
                {
                    return;
                }

                try
                {
                    using (Graphics g = Graphics.FromHdc(hdc))
                    {
                        DrawFace(g, new Rectangle(0, 0, Width, Height));
                    }
                }
                finally
                {
                    ReleaseDC(Handle, hdc);
                }
            }
        }

        private void DrawFace(Graphics g, Rectangle r)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;

            using (SolidBrush brush = new SolidBrush(BackColor))
            {
                g.FillRectangle(brush, r);
            }

            string text = SelectedItem != null ? GetItemText(SelectedItem) : "";
            Color textColor = Enabled ? ForeColor : Color.FromArgb(71, 85, 105);
            TextRenderer.DrawText(g, text, Font,
                new Rectangle(r.X + 13, r.Y, Math.Max(0, r.Width - 13 - 40), r.Height), textColor,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter |
                TextFormatFlags.EndEllipsis | TextFormatFlags.NoPrefix);

            int cx = r.Right - 26;
            int cy = r.Height / 2;
            using (Pen pen = new Pen(ChevronColor, 2f))
            {
                pen.StartCap = LineCap.Round;
                pen.EndCap = LineCap.Round;
                g.DrawLines(pen, new Point[]
                {
                    new Point(cx - 5, cy - 2),
                    new Point(cx, cy + 3),
                    new Point(cx + 5, cy - 2)
                });
            }
        }
    }
}