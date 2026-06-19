using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Niko.WinForms
{
    public class NikoButton : Button
    {
        private Color _accentColor = Color.FromArgb(152, 101, 246);
        private int _borderThickness = 5;

        public NikoButton()
        {
            this.BackColor = Color.Black;
            this.ForeColor = _accentColor;
            this.FlatStyle = FlatStyle.Popup;
        }

        [Category("Niko UI")]
        [Browsable(true)]
        [Description("Panel border color / Цвет обводки панели")]
        public Color AccentColor
        {
            get => _accentColor;
            set
            {
                _accentColor = value;
                this.ForeColor = value;
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

        [Category("Niko UI")][Browsable(true)] public Font TextFont { get => this.Font; set { this.Font = value; } }
        [Category("Niko UI")][Browsable(true)] public Image ButtonBackgroundImage { get => this.BackgroundImage; set => this.BackgroundImage = value; }
        [Category("Niko UI")][Browsable(true)] public string TextValue { get => this.Text; set => this.Text = value; }

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
    }
}