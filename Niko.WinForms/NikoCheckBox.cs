using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;

namespace Niko.WinForms
{
    /// <summary>
    /// CheckBox control in Niko UI style. It allows you to customize accent color, hover color, pressed color, box background color, box size, border thickness, checked state, and whether to draw a cross instead of a checkmark. | Элемент управления чекбоксом в стиле Niko UI. Позволяет настраивать цвет акцента, цвет при наведении, цвет при зажатии, цвет фона квадратика, размер квадратика, толщину рамки, состояние выбора и возможность рисовать крестик вместо галочки.
    /// </summary>
    [DefaultEvent("CheckedChanged")]
    public class NikoCheckBox : Control
    {
        private bool _checked = false;
        private bool _isHovered = false;
        private bool _isPressed = false;

        private int _boxSize = 13;
        private int _borderThickness = 1;

        #region События

        [Category("Niko UI")]
        [Description("Occurs when the checkbox state changes / Происходит при изменении состояния чекбокса")]
        public event EventHandler CheckedChanged;

        #endregion

        #region Свойства Кастомизации

        [Category("Niko UI")]
        [Description("Indicates whether the checkbox is checked / Указывает, выбран ли данный чекбокс")]
        public bool Checked
        {
            get => _checked;
            set
            {
                if (_checked != value)
                {
                    _checked = value;
                    CheckedChanged?.Invoke(this, EventArgs.Empty);
                    CustomInvalidate();
                }
            }
        }

        [Category("Niko UI")]
        [Description("Checkbox square size (odd values recommended) / Размер квадратика чекбокса (рекомендуются нечетные значения)")]
        [DefaultValue(13)]
        public int BoxSize
        {
            get => _boxSize;
            set
            {
                if (value < 5) value = 5;
                if (_boxSize != value)
                {
                    _boxSize = value;
                    CustomInvalidate();
                }
            }
        }

        [Category("Niko UI")]
        [Description("Border thickness in pixels / Толщина рамки квадратика в пикселях")]
        [DefaultValue(1)]
        public int BorderThickness
        {
            get => _borderThickness;
            set
            {
                if (value < 1) value = 1;
                if (_borderThickness != value)
                {
                    _borderThickness = value;
                    CustomInvalidate();
                }
            }
        }

        [Category("Niko UI")]
        [Description("Main color in normal state / Основной цвет в обычном состоянии")]
        public Color AccentColor { get; set; } = Color.FromArgb(158, 134, 255);

        [Category("Niko UI")]
        [Description("Color when hovering with the mouse / Цвет при наведении мыши")]
        public Color HoverColor { get; set; } = Color.FromArgb(180, 140, 255);

        [Category("Niko UI")]
        [Description("Color when the mouse is pressed / Цвет при зажатии мыши")]
        public Color PressedColor { get; set; } = Color.FromArgb(120, 75, 210);

        [Category("Niko UI")]
        [Description("Inner box background color / Цвет внутренности квадратика")]
        public Color BoxBackColor { get; set; } = Color.FromArgb(12, 7, 20);

        [Category("Niko UI")]
        [Description("Draws a pixel 'X' instead of a checkmark / Если true — вместо галочки будет пиксельный крестик 'X'")]
        public bool DrawAsCross { get; set; } = false;

        [Category("Niko UI")]
        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [Description("The text associated with the checkbox / Текст, отображаемый рядом с чекбоксом")]
        public override string Text
        {
            get => base.Text;
            set { base.Text = value; CustomInvalidate(); }
        }

        [Category("Niko UI")]
        [Browsable(true)]
        [Description("The font used for the checkbox text / Шрифт текста чекбокса")]
        public override Font Font
        {
            get => base.Font;
            set { base.Font = value; CustomInvalidate(); }
        }
        #endregion

        public NikoCheckBox()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.UserPaint |
                     ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.ResizeRedraw |
                     ControlStyles.SupportsTransparentBackColor |
                     ControlStyles.Selectable, true);

            this.BackColor = Color.Transparent;
            this.ForeColor = AccentColor;
            this.Cursor = Cursors.Hand;
            this.Size = new Size(120, 20);
        }

        private void CustomInvalidate()
        {
            if (this.Parent != null && this.BackColor == Color.Transparent)
            {
                this.Parent.Invalidate(this.Bounds, true);
            }
            else
            {
                this.Invalidate();
            }
        }

        #region Логика Мыши и Клавиатуры

        protected override void OnMouseEnter(EventArgs e) { base.OnMouseEnter(e); _isHovered = true; CustomInvalidate(); }
        protected override void OnMouseLeave(EventArgs e) { base.OnMouseLeave(e); _isHovered = false; _isPressed = false; CustomInvalidate(); }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (e.Button == MouseButtons.Left)
            {
                this.Focus();
                _isPressed = true;
                CustomInvalidate();
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            if (_isPressed && e.Button == MouseButtons.Left)
            {
                _isPressed = false;
                if (this.ClientRectangle.Contains(e.Location))
                {
                    Checked = !Checked;
                }
                CustomInvalidate();
            }
        }

        protected override void OnClick(EventArgs e) { }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (e.KeyCode == Keys.Space) { _isPressed = true; CustomInvalidate(); }
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            base.OnKeyUp(e);
            if (e.KeyCode == Keys.Space) { _isPressed = false; Checked = !Checked; CustomInvalidate(); }
        }

        #endregion

        #region Отрисовка

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            g.SmoothingMode = SmoothingMode.None;
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            int boxX = 0;
            int boxY = (this.Height - _boxSize) / 2;
            Rectangle boxRect = new Rectangle(boxX, boxY, _boxSize, _boxSize);

            using (Brush bgBrush = new SolidBrush(BoxBackColor))
            {
                g.FillRectangle(bgBrush, boxRect);
            }

            Color activeColor = AccentColor;
            if (_isPressed) activeColor = PressedColor;
            else if (_isHovered) activeColor = HoverColor;

            using (Pen borderPen = new Pen(activeColor, _borderThickness))
            {
                borderPen.Alignment = PenAlignment.Inset;
                g.DrawRectangle(borderPen, boxX, boxY, _boxSize, _boxSize);
            }

            if (_checked)
            {
                using (Pen checkPen = new Pen(activeColor, 1))
                {
                    int padding = _borderThickness + 2;

                    if (DrawAsCross)
                    {
                        g.DrawLine(checkPen, boxX + padding, boxY + padding, boxX + _boxSize - 1 - padding, boxY + _boxSize - 1 - padding);
                        g.DrawLine(checkPen, boxX + padding, boxY + _boxSize - 1 - padding, boxX + _boxSize - 1 - padding, boxY + padding);
                    }
                    else
                    {
                        int checkLeftX = boxX + _borderThickness + 2;
                        int checkBottomX = boxX + (_boxSize / 2) - 1;
                        int checkBottomY = boxY + _boxSize - _borderThickness - 4;
                        int checkRightX = boxX + _boxSize - _borderThickness - 3;
                        int checkRightY = boxY + _borderThickness + 2;

                        g.DrawLine(checkPen, checkLeftX, checkBottomY - 2, checkBottomX, checkBottomY);
                        g.DrawLine(checkPen, checkLeftX, checkBottomY - 1, checkBottomX, checkBottomY + 1);

                        g.DrawLine(checkPen, checkBottomX, checkBottomY, checkRightX, checkRightY);
                        g.DrawLine(checkPen, checkBottomX, checkBottomY + 1, checkRightX, checkRightY + 1);
                    }
                }
            }

            int textX = _boxSize + 8;
            Rectangle textRect = new Rectangle(textX, 0, this.Width - textX, this.Height);

            TextFormatFlags flags = TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis;
            TextRenderer.DrawText(g, this.Text, this.Font, textRect, this.ForeColor, flags);
        }

        #endregion
    }
}