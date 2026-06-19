using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Niko.WinForms
{
    /// <summary>
    /// Radio button control in Niko UI style. It allows you to customize accent color, hover color, pressed color, box background color, radio size, border thickness, and checked state. | Элемент управления радио-кнопкой в стиле Niko UI. Позволяет настраивать цвет акцента, цвет при наведении, цвет при зажатии, цвет фона кружка, размер кружка, толщину рамки и состояние выбора.
    /// </summary>
    public class NikoRadioButton : RadioButton
    {
        private Color _accentColor = Color.FromArgb(152, 101, 246);
        private Color _hoverColor = Color.FromArgb(152, 101, 246);
        private Color _pressedColor = Color.FromArgb(152, 101, 246);
        private Color _boxBackColor = Color.Black;
        private int _radioSize = 16;
        private int _borderThickness = 2;

        private bool _isHovered = false;
        private bool _isPressed = false;

        public NikoRadioButton()
        {
            this.BackColor = Color.Black;
            this.ForeColor = _accentColor;

            this.SetStyle(ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);
        }

        protected override void OnCheckedChanged(EventArgs e)
        {
            base.OnCheckedChanged(e);
            this.Invalidate();
        }

        #region Свойства из XML-комментария и для NikoThemeManager

        [Category("Niko UI")]
        [Description("Accent color / Цвет акцента")]
        public Color AccentColor
        {
            get => _accentColor;
            set { _accentColor = value; this.Invalidate(); }
        }

        [Category("Niko UI")]
        [Description("Hover color / Цвет при наведении")]
        public Color HoverColor
        {
            get => _hoverColor;
            set { _hoverColor = value; this.Invalidate(); }
        }

        [Category("Niko UI")]
        [Description("Pressed color / Цвет при зажатии")]
        public Color PressedColor
        {
            get => _pressedColor;
            set { _pressedColor = value; this.Invalidate(); }
        }

        [Category("Niko UI")]
        [Description("Box background color / Цвет фона кружка")]
        public Color BoxBackColor
        {
            get => _boxBackColor;
            set { _boxBackColor = value; this.Invalidate(); }
        }

        [Category("Niko UI")]
        [Description("Radio size / Размер кружка")]
        public int RadioSize
        {
            get => _radioSize;
            set { _radioSize = value; this.Invalidate(); }
        }

        [Category("Niko UI")]
        [Description("Border thickness / Толщина рамки")]
        public int BorderThickness
        {
            get => _borderThickness;
            set { _borderThickness = value; this.Invalidate(); }
        }

        #endregion

        #region Управление состояниями мыши

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            _isHovered = true;
            this.Invalidate();
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            _isHovered = false;
            _isPressed = false;
            this.Invalidate();
        }

        protected override void OnMouseDown(MouseEventArgs mevent)
        {
            base.OnMouseDown(mevent);
            if (mevent.Button == MouseButtons.Left)
            {
                _isPressed = true;
                this.Invalidate();
            }
        }

        protected override void OnMouseUp(MouseEventArgs mevent)
        {
            base.OnMouseUp(mevent);
            _isPressed = false;
            this.Invalidate();
        }

        #endregion

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.None;

            using (Brush bgBrush = new SolidBrush(this.BackColor))
            {
                g.FillRectangle(bgBrush, this.ClientRectangle);
            }

            Color currentRenderColor = _accentColor;
            if (_isPressed) currentRenderColor = _pressedColor;
            else if (_isHovered) currentRenderColor = _hoverColor;

            int radioY = (this.Height - _radioSize) / 2;
            Rectangle radioRect = new Rectangle(0, radioY, _radioSize, _radioSize);

            using (Brush boxBg = new SolidBrush(_boxBackColor))
            {
                g.FillEllipse(boxBg, radioRect);
            }

            if (_borderThickness > 0)
            {
                using (Pen p = new Pen(currentRenderColor, _borderThickness))
                {
                    p.Alignment = PenAlignment.Inset;
                    g.DrawEllipse(p, radioRect);
                }
            }

            if (this.Checked)
            {
                int dotSize = _radioSize / 2;
                if (dotSize < 2) dotSize = 2;

                int dotX = radioRect.X + (radioRect.Width - dotSize) / 2;
                int dotY = radioRect.Y + (radioRect.Height - dotSize) / 2;
                Rectangle dotRect = new Rectangle(dotX, dotY, dotSize, dotSize);

                using (Brush dotBrush = new SolidBrush(currentRenderColor))
                {
                    g.FillEllipse(dotBrush, dotRect);
                }
            }

            int textX = _radioSize + 8;
            Rectangle textRect = new Rectangle(textX, 0, this.Width - textX, this.Height);

            using (Brush textBrush = new SolidBrush(this.ForeColor))
            {
                StringFormat sf = new StringFormat
                {
                    Alignment = StringAlignment.Near,
                    LineAlignment = StringAlignment.Center
                };
                g.DrawString(this.Text, this.Font, textBrush, textRect, sf);
            }
        }
    }
}