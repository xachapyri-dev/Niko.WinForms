using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Niko.WinForms
{
    /// <summary>
    /// Panel control in Niko UI style. It allows you to customize the accent color and border thickness. | Элемент управления Panel в стиле Niko UI. Позволяет настраивать цвет акцента и толщину рамки.
    /// </summary>
    public class NikoPanel : Panel
    {
        private Color _accentColor = Color.FromArgb(152, 101, 246);
        private int _borderThickness = 5;

        [Category("Niko UI")]
        [Browsable(true)]
        [Description("Panel border color / Цвет обводки панели")]
        public Color AccentColor
        {
            get => _accentColor;
            set
            {
                _accentColor = value;
                this.Invalidate();
            }
        }

        [Category("Niko UI")]
        [Browsable(true)]
        [Description("Border thickness in pixels / Толщина обводки (в пикселях)")]
        public int BorderThickness
        {
            get => _borderThickness;
            set
            {
                _borderThickness = value;
                this.Padding = new Padding(value);
                this.Invalidate();
            }
        }

        public NikoPanel()
        {
            this.BackColor = Color.Black;
            this.Padding = new Padding(_borderThickness);
            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            e.Graphics.SmoothingMode = SmoothingMode.None;

            if (_borderThickness > 0)
            {
                using (Pen p = new Pen(_accentColor, _borderThickness))
                {
                    p.Alignment = PenAlignment.Inset;
                    e.Graphics.DrawRectangle(p, 0, 0, this.Width - 1, this.Height - 1);
                }
            }
        }

        protected override void OnScroll(ScrollEventArgs se)
        {
            base.OnScroll(se);
            this.Invalidate();
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);
            this.Invalidate();
        }
        protected override void OnResize(EventArgs eventargs)
        {
            base.OnResize(eventargs);
            this.Invalidate();
        }
        [DllImport("user32.dll")]
        private static extern bool ShowScrollBar(IntPtr hWnd, int wBar, bool bShow);
        private const int SB_BOTH = 3;
        
        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            if (this.AutoScroll)
            {
                ShowScrollBar(this.Handle, SB_BOTH, false);
            }
        }
    }
}