using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Niko.WinForms
{
    /// <summary>
    /// Slider control in Niko UI style. It allows you to customize minimum, maximum, current value, accent color, hover color, pressed color, arrow color, and button step. | Элемент управления ползунком (Slider) в стиле Niko UI. Позволяет настраивать минимальное, максимальное и текущее значение, цвет акцента, цвет при наведении, цвет при нажатии, цвет стрелок и шаг изменения значения при клике на кнопки.
    /// </summary>
    [DefaultEvent("ValueChanged")]
    public class NikoSlider : Control
    {
        private int _min = 0;
        private int _max = 100;
        private int _value = 50;

        private bool _isDragging = false;

        private ElementState _leftBtnState = ElementState.Normal;
        private ElementState _rightBtnState = ElementState.Normal;
        private ElementState _thumbState = ElementState.Normal;

        private enum ElementState { Normal, Hover, Pressed }

        private Rectangle _rectLeftArrow;
        private Rectangle _rectRightArrow;
        private Rectangle _rectThumb;
        private int _btnSize = 24;

        public event EventHandler ValueChanged;

        #region Свойства Дизайн-Системы (Палитра 152, 101, 246)

        [Category("Niko UI")]
        [Description("Minimum value of the range / Минимальное значение диапазона")]
        public int Minimum { get => _min; set { _min = value; Invalidate(); } }

        [Category("Niko UI")]
        [Description("Maximum value of the range / Максимальное значение диапазона")]
        public int Maximum { get => _max; set { _max = value; Invalidate(); } }

        [Category("Niko UI")]
        [Description("Current value / Текущее значение")]
        public int Value
        {
            get => _value;
            set { _value = Math.Max(_min, Math.Min(_max, value)); Invalidate(); ValueChanged?.Invoke(this, EventArgs.Empty); }
        }

        [Category("Niko UI")]
        [Description("Primary element color / Основной цвет элементов")]
        public Color AccentColor { get; set; } = Color.FromArgb(152, 101, 246);

        [Category("Niko UI")]
        [Description("Element color on hover / Цвет элементов при наведении")]
        public Color HoverColor { get; set; } = Color.FromArgb(182, 142, 255);

        [Category("Niko UI")]
        [Description("Element color when pressed / Цвет элементов при нажатии")]
        public Color PressedColor { get; set; } = Color.FromArgb(118, 67, 212);

        [Category("Niko UI")]
        [Description("Color of the arrows inside buttons / Цвет стрелочек внутри кнопок")]
        public Color ArrowColor { get; set; } = Color.FromArgb(12, 7, 20);

        [Category("Niko UI")]
        [Description("Value change step when clicking buttons / Шаг изменения значения при клике на кнопки")]
        public int ButtonStep { get; set; } = 5;

        #endregion

        public NikoSlider()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint |
                     ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw |
                     ControlStyles.SupportsTransparentBackColor, true);

            this.BackColor = Color.Black;
            this.Size = new Size(220, 30);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            _rectLeftArrow = new Rectangle(0, (Height - _btnSize) / 2, _btnSize, _btnSize);
            _rectRightArrow = new Rectangle(Width - _btnSize, (Height - _btnSize) / 2, _btnSize, _btnSize);
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
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.None;

            int trackLeft = _rectLeftArrow.Right + 10;
            int trackRight = _rectRightArrow.X - 10;
            int trackY = Height / 2;

            float percent = (float)(_value - _min) / (_max - _min);
            int thumbX = trackLeft + (int)(percent * (trackRight - trackLeft));
            _rectThumb = new Rectangle(thumbX - 6, trackY - 6, 12, 12);

            Color leftColor = GetColorForState(_leftBtnState);
            Color rightColor = GetColorForState(_rightBtnState);
            Color thumbColor = (_isDragging) ? GetColorForState(ElementState.Pressed) : GetColorForState(_thumbState);

            using (Pen pAccent = new Pen(AccentColor, 2))
            using (Brush bLeft = new SolidBrush(leftColor))
            using (Brush bRight = new SolidBrush(rightColor))
            using (Brush bThumb = new SolidBrush(thumbColor))
            using (Brush bArrow = new SolidBrush(ArrowColor))
            {
                int padX = _btnSize / 4;
                int padY = _btnSize / 4;

                g.FillRectangle(bLeft, _rectLeftArrow);
                Point[] leftArrow = {
                    new Point(_rectLeftArrow.X + padX, _rectLeftArrow.Y + _btnSize / 2),
                    new Point(_rectLeftArrow.Right - padX - 1, _rectLeftArrow.Y + padY),
                    new Point(_rectLeftArrow.Right - padX - 1, _rectLeftArrow.Bottom - padY - 1)
                };
                g.FillPolygon(bArrow, leftArrow);

                g.FillRectangle(bRight, _rectRightArrow);
                Point[] rightArrow = {
                    new Point(_rectRightArrow.Right - padX - 1, _rectRightArrow.Y + _btnSize / 2),
                    new Point(_rectRightArrow.X + padX, _rectRightArrow.Y + padY),
                    new Point(_rectRightArrow.X + padX, _rectRightArrow.Bottom - padY - 1)
                };
                g.FillPolygon(bArrow, rightArrow);

                g.DrawLine(pAccent, trackLeft, trackY - 2, thumbX, trackY - 2);
                g.DrawLine(pAccent, trackLeft, trackY + 2, thumbX, trackY + 2);
                g.DrawLine(pAccent, thumbX, trackY, trackRight, trackY);

                Point[] diamond = new Point[] {
                    new Point(thumbX, trackY - 7),
                    new Point(thumbX + 7, trackY),
                    new Point(thumbX, trackY + 7),
                    new Point(thumbX - 7, trackY)
                };

                g.FillPolygon(bThumb, diamond);

                if (_thumbState == ElementState.Hover || _isDragging)
                {
                    using (Pen pWhite = new Pen(Color.White, 1))
                    {
                        g.DrawPolygon(pWhite, diamond);
                    }
                }
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (_isDragging)
            {
                UpdateValueFromMouse(e.X);
                return;
            }

            ElementState oldLeft = _leftBtnState;
            ElementState oldRight = _rightBtnState;
            ElementState oldThumb = _thumbState;

            _leftBtnState = _rectLeftArrow.Contains(e.Location)
                ? (_leftBtnState == ElementState.Pressed ? ElementState.Pressed : ElementState.Hover)
                : ElementState.Normal;

            _rightBtnState = _rectRightArrow.Contains(e.Location)
                ? (_rightBtnState == ElementState.Pressed ? ElementState.Pressed : ElementState.Hover)
                : ElementState.Normal;

            _thumbState = _rectThumb.Contains(e.Location) ? ElementState.Hover : ElementState.Normal;

            if (oldLeft != _leftBtnState || oldRight != _rightBtnState || oldThumb != _thumbState)
                Invalidate();
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            _leftBtnState = _rightBtnState = _thumbState = ElementState.Normal;
            Invalidate();
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (e.Button != MouseButtons.Left) return;

            if (_rectLeftArrow.Contains(e.Location))
            {
                _leftBtnState = ElementState.Pressed;
                Value -= ButtonStep;
                Invalidate();
                return;
            }
            if (_rectRightArrow.Contains(e.Location))
            {
                _rightBtnState = ElementState.Pressed;
                Value += ButtonStep;
                Invalidate();
                return;
            }

            if (_rectThumb.Contains(e.Location) || (e.X > _rectLeftArrow.Right && e.X < _rectRightArrow.X))
            {
                _isDragging = true;
                UpdateValueFromMouse(e.X);
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            _isDragging = false;

            _leftBtnState = _rectLeftArrow.Contains(e.Location) ? ElementState.Hover : ElementState.Normal;
            _rightBtnState = _rectRightArrow.Contains(e.Location) ? ElementState.Hover : ElementState.Normal;
            _thumbState = _rectThumb.Contains(e.Location) ? ElementState.Hover : ElementState.Normal;

            Invalidate();
        }

        private void UpdateValueFromMouse(int mouseX)
        {
            int trackLeft = _rectLeftArrow.Right + 10;
            int trackRight = _rectRightArrow.X - 10;
            int trackWidth = trackRight - trackLeft;

            if (trackWidth <= 0) return;

            float percent = (float)(mouseX - trackLeft) / trackWidth;
            percent = Math.Max(0f, Math.Min(1f, percent));
            Value = _min + (int)(percent * (_max - _min));
        }
    }
}