using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Niko.WinForms
{
    /// <summary>
    /// Slider button control in Niko UI style. It allows you to customize accent color, hover color, pressed color, arrow color, and direction (left or right). The button body stretches to fit any size while maintaining pixel-perfect arrow rendering. | Элемент управления кнопкой-слайдером в стиле Niko UI. Позволяет настраивать цвет акцента, цвет при наведении, цвет при зажатии, цвет стрелки и направление (влево или вправо). Тело кнопки растягивается под любой размер, сохраняя пиксельную четкость стрелки.
    /// </summary>
    public class NikoSliderButton : Control
    {
        public enum ButtonDirection { Left, Right }

        private ElementState _state = ElementState.Normal;
        private enum ElementState { Normal, Hover, Pressed }

        #region Свойства Кастомизации
        [Category("Niko UI - Style")]
        [Description("Arrow direction (Left or Right) / Направление стрелки кнопки (Влево или Вправо)")]
        public ButtonDirection Direction { get; set; } = ButtonDirection.Left;

        [Category("Niko UI - Colors")]
        [Description("Primary accent color / Основной акцентный цвет")]
        public Color AccentColor { get; set; } = Color.FromArgb(152, 101, 246);

        [Category("Niko UI - Colors")]
        [Description("Color when hovering with the mouse / Цвет при наведении мыши")]
        public Color HoverColor { get; set; } = Color.FromArgb(182, 142, 255);

        [Category("Niko UI - Colors")]
        [Description("Color when the mouse is pressed / Цвет при зажатии мыши")]
        public Color PressedColor { get; set; } = Color.FromArgb(118, 67, 212);

        [Category("Niko UI - Colors")]
        [Description("Arrow color / Цвет стрелки")]
        public Color ArrowColor { get; set; } = Color.FromArgb(12, 7, 20);

        #endregion

        public NikoSliderButton()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint |
                     ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw |
                     ControlStyles.SupportsTransparentBackColor, true);

            this.Size = new Size(14, 14);
            this.BackColor = Color.Transparent;
        }

        private Color GetColorForState(ElementState state)
        {
            switch (state)
            {
                case ElementState.Hover:
                    return HoverColor;
                case ElementState.Pressed:
                    return PressedColor;
                default:
                    return AccentColor;
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.None;

            Color currentBodyColor = GetColorForState(_state);

            using (Brush bodyBrush = new SolidBrush(currentBodyColor))
            using (Brush arrowBrush = new SolidBrush(ArrowColor))
            {
                g.FillRectangle(bodyBrush, 0, 0, Width, Height);

                int paddingX = Math.Max(3, Width / 4);
                int paddingY = Math.Max(3, Height / 4);

                if (Direction == ButtonDirection.Left)
                {
                    Point[] leftArrow = {
                        new Point(paddingX, Height / 2),
                        new Point(Width - paddingX - 1, paddingY),
                        new Point(Width - paddingX - 1, Height - paddingY - 1)
                    };
                    g.FillPolygon(arrowBrush, leftArrow);
                }
                else
                {
                    Point[] rightArrow = {
                        new Point(Width - paddingX - 1, Height / 2),
                        new Point(paddingX, paddingY),
                        new Point(paddingX, Height - paddingY - 1)
                    };
                    g.FillPolygon(arrowBrush, rightArrow);
                }
            }
        }

        #region Управление состояниями мыши

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            _state = ElementState.Hover;
            this.Invalidate();
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            _state = ElementState.Normal;
            this.Invalidate();
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (e.Button == MouseButtons.Left)
            {
                _state = ElementState.Pressed;
                this.Invalidate();
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            _state = this.ClientRectangle.Contains(e.Location) ? ElementState.Hover : ElementState.Normal;
            this.Invalidate();
        }

        #endregion
    }
}