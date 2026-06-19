using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace Niko.WinForms
{
    /// <summary>
    /// Form state button types for the custom title bar. | Типы кнопок управления состоянием окна для кастомной панели заголовка.
    /// </summary>
    public enum NikoButtonType { Minimize, Maximize, Close }

    /// <summary>
    /// Form with a custom title bar in Niko UI style. It allows you to customize the title text, title icon, accent color, title font, and whether to hide the taskbar when maximizing. | Форма с кастомной панелью заголовка в стиле Niko UI. Позволяет настраивать текст заголовка, иконку заголовка, цвет акцента, шрифт заголовка и скрывать ли панель задач при максимизации.
    /// </summary>
    public class NikoForm : Form
    {
        private Color _accent = Color.FromArgb(158, 134, 255);
        private bool _hideTaskbarOnMaximize = false;

        private Panel _titleBar;
        private Label _lblTitle;
        private PictureBox _picIcon;
        private NikoTitleButton _btnClose, _btnMax, _btnMin;

        public Panel ContentPanel { get; private set; }

        public NikoForm()
        {
            this.FormBorderStyle = FormBorderStyle.None;
            this.BackColor = Color.Black;
            this.DoubleBuffered = true;
            this.MinimumSize = new Size(200, 100);

            _titleBar = new Panel { BackColor = Color.Black };
            _titleBar.MouseDown += TitleBar_MouseDown;
            _titleBar.Paint += TitleBar_Paint;

            _lblTitle = new Label { ForeColor = _accent, AutoSize = true, Location = new Point(40, 10) };
            _lblTitle.MouseDown += TitleBar_MouseDown;

            _picIcon = new PictureBox { Size = new Size(24, 24), Location = new Point(10, 8), SizeMode = PictureBoxSizeMode.StretchImage };

            _btnClose = new NikoTitleButton(NikoButtonType.Close, this);
            _btnClose.Click += (s, e) => this.Close();

            _btnMax = new NikoTitleButton(NikoButtonType.Maximize, this);
            _btnMax.Click += (s, e) => MaximizeToggle();

            _btnMin = new NikoTitleButton(NikoButtonType.Minimize, this);
            _btnMin.Click += (s, e) => this.WindowState = FormWindowState.Minimized;

            _titleBar.Controls.AddRange(new Control[] { _picIcon, _lblTitle, _btnClose, _btnMax, _btnMin });
            this.Controls.Add(_titleBar);

            ContentPanel = new Panel { BackColor = Color.Black };
            this.Controls.Add(ContentPanel);

            TitleFont = new Font("Arial", 12, FontStyle.Bold);
        }

        private void MaximizeToggle()
        {
            if (this.WindowState == FormWindowState.Maximized)
            {
                this.WindowState = FormWindowState.Normal;
            }
            else
            {
                if (!_hideTaskbarOnMaximize)
                {
                    Screen currentScreen = Screen.FromControl(this);
                    this.MaximizedBounds = new Rectangle(
                        currentScreen.WorkingArea.X - currentScreen.Bounds.X,
                        currentScreen.WorkingArea.Y - currentScreen.Bounds.Y,
                        currentScreen.WorkingArea.Width,
                        currentScreen.WorkingArea.Height
                    );
                }
                else
                {
                    this.MaximizedBounds = Rectangle.Empty;
                }

                this.WindowState = FormWindowState.Maximized;
            }
            _btnMax?.Invalidate();
        }

        private void TitleBar_Paint(object sender, PaintEventArgs e)
        {
            if (_accent.IsEmpty) return;
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
            using (Brush b = new SolidBrush(_accent))
            {
                e.Graphics.FillRectangle(b, 0, _titleBar.Height - 5, _titleBar.Width, 5);
            }
        }


        [Category("Niko UI")]
        [DisplayName("Hide Taskbar On Maximize")]
        [Description("If true, the window will expand over the taskbar. If false, the taskbar remains visible / Если true, окно развернется поверх панели задач. Если false, трей останется видимым.")]
        public bool HideTaskbarOnMaximize
        {
            get => _hideTaskbarOnMaximize;
            set => _hideTaskbarOnMaximize = value;
        }

        [Category("Niko UI")]
        [Description("Title text displayed in the header / Текст заголовка, отображаемый в шапке")]
        public string TitleText { get => _lblTitle.Text; set => _lblTitle.Text = value; }

        [Category("Niko UI")]
        [Description("Icon displayed in the header / Иконка, отображаемая в шапке")]
        public Image TitleIcon { get => _picIcon.Image; set => _picIcon.Image = value; }

        [Category("Niko UI")]
        [Description("Accent color for the header and buttons / Акцентный цвет для шапки и кнопок")]
        public Color AccentColor
        {
            get => _accent;
            set
            {
                _accent = value;
                _lblTitle.ForeColor = value;
                _btnClose?.Invalidate();
                _btnMax?.Invalidate();
                _btnMin?.Invalidate();
                _titleBar.Invalidate();
                this.Invalidate();
            }
        }

        [Category("Niko UI")]
        [Description("Font used for the title / Шрифт, используемый для заголовка")]
        public Font TitleFont
        {
            get => _lblTitle.Font;
            set
            {
                if (value == null) return;
                _lblTitle.Font = value;
                _lblTitle.Location = new Point(40, (_titleBar.Height - 5 - _lblTitle.Height) / 2);
            }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            if (_titleBar == null || ContentPanel == null) return;

            _titleBar.Bounds = new Rectangle(5, 5, this.Width - 10, 45);
            ContentPanel.Bounds = new Rectangle(5, 50, this.Width - 10, this.Height - 55);

            int btnY = (_titleBar.Height - 5 - _btnClose.Height) / 2;
            int rightX = _titleBar.Width - 5;

            rightX -= 35; _btnClose.Location = new Point(rightX, btnY);
            rightX -= 35; _btnMax.Location = new Point(rightX, btnY);
            rightX -= 35; _btnMin.Location = new Point(rightX, btnY);

            this.Invalidate();
        }

        protected override void WndProc(ref Message m)
        {
            const int WM_NCHITTEST = 0x84;
            const int BORDER_WIDTH = 7;

            if (m.Msg == WM_NCHITTEST)
            {
                Point pos = new Point(m.LParam.ToInt32());
                pos = this.PointToClient(pos);

                if (pos.X <= BORDER_WIDTH && pos.Y <= BORDER_WIDTH) { m.Result = (IntPtr)13; return; }
                if (pos.X >= this.Width - BORDER_WIDTH && pos.Y <= BORDER_WIDTH) { m.Result = (IntPtr)14; return; }
                if (pos.X <= BORDER_WIDTH && pos.Y >= this.Height - BORDER_WIDTH) { m.Result = (IntPtr)16; return; }
                if (pos.X >= this.Width - BORDER_WIDTH && pos.Y >= this.Height - BORDER_WIDTH) { m.Result = (IntPtr)17; return; }

                if (pos.X <= BORDER_WIDTH) { m.Result = (IntPtr)10; return; }
                if (pos.X >= this.Width - BORDER_WIDTH) { m.Result = (IntPtr)11; return; }
                if (pos.Y <= BORDER_WIDTH) { m.Result = (IntPtr)12; return; }
                if (pos.Y >= this.Height - BORDER_WIDTH) { m.Result = (IntPtr)15; return; }
            }
            base.WndProc(ref m);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            if (_accent.IsEmpty) return;

            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
            using (Pen p = new Pen(_accent, 5))
            {
                p.Alignment = System.Drawing.Drawing2D.PenAlignment.Inset;
                e.Graphics.DrawRectangle(p, 0, 0, this.Width, this.Height);
            }
        }

        [DllImport("user32.dll")] public static extern bool ReleaseCapture();
        [DllImport("user32.dll")] public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        private void TitleBar_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (e.Clicks == 2)
                {
                    MaximizeToggle();
                }
                else
                {
                    ReleaseCapture();
                    SendMessage(this.Handle, 0x112, 0xf012, 0);
                }
            }
        }
    }
    public class NikoTitleButton : Control
    {
        private readonly NikoButtonType _type;
        private readonly NikoForm _parent;
        private bool _isHovered = false;
        private bool _isPressed = false;

        public NikoTitleButton(NikoButtonType type, NikoForm parent)
        {
            _type = type;
            _parent = parent;
            this.Size = new Size(30, 26);
            this.Cursor = Cursors.Hand;
            this.DoubleBuffered = true;
        }

        protected override void OnMouseEnter(EventArgs e) { _isHovered = true; Invalidate(); base.OnMouseEnter(e); }
        protected override void OnMouseLeave(EventArgs e) { _isHovered = false; _isPressed = false; Invalidate(); base.OnMouseLeave(e); }
        protected override void OnMouseDown(MouseEventArgs e) { if (e.Button == MouseButtons.Left) { _isPressed = true; Invalidate(); } base.OnMouseDown(e); }
        protected override void OnMouseUp(MouseEventArgs e) { _isPressed = false; Invalidate(); base.OnMouseUp(e); }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;

            Color baseColor = _parent.AccentColor;
            Color bg = baseColor;

            if (_isPressed)
                bg = Color.FromArgb(Math.Max(0, baseColor.R - 35), Math.Max(0, baseColor.G - 45), Math.Max(0, baseColor.B - 25));
            else if (_isHovered)
                bg = Color.FromArgb(Math.Min(255, baseColor.R + 25), Math.Min(255, baseColor.G + 20), Math.Min(255, baseColor.B + 15));

            using (var brushBg = new SolidBrush(bg))
            {
                g.FillRectangle(brushBg, this.ClientRectangle);
            }

            Color symbolColor = Color.FromArgb(12, 7, 20);
            using (var penSym = new Pen(symbolColor, 2))
            using (var brushSym = new SolidBrush(symbolColor))
            {
                int w = Width;
                int h = Height;

                switch (_type)
                {
                    case NikoButtonType.Minimize:
                        g.FillRectangle(brushSym, w / 2 - 6, h / 2 + 3, 12, 3);
                        break;

                    case NikoButtonType.Maximize:
                        if (_parent.WindowState == FormWindowState.Maximized)
                        {
                            g.DrawRectangle(penSym, w / 2 - 3, h / 2 - 6, 7, 7);
                            g.DrawRectangle(penSym, w / 2 - 6, h / 2 - 3, 7, 7);
                        }
                        else
                        {
                            g.DrawRectangle(penSym, w / 2 - 5, h / 2 - 5, 10, 10);
                        }
                        break;

                    case NikoButtonType.Close:
                        g.DrawLine(penSym, w / 2 - 5, h / 2 - 5, w / 2 + 4, h / 2 + 4);
                        g.DrawLine(penSym, w / 2 - 5, h / 2 + 4, w / 2 + 4, h / 2 - 5);
                        break;
                }
            }
        }
    }
}