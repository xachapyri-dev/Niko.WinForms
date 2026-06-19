using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;

namespace Niko.WinForms
{
    /// <summary>
    /// Progress bar control in Niko UI style. It allows you to customize minimum, maximum, current value, border thickness, accent color, progress color, border color, track color, and whether to show percentage text. | Элемент управления полосой прогресса в стиле Niko UI. Позволяет настраивать минимальное, максимальное и текущее значение, толщину рамки, акцентный цвет, цвет заполнения прогресса, цвет рамки, цвет фона шкалы и отображение процентного текста.
    /// </summary>
    public class NikoProgressBar : Control
    {
        private int _minimum = 0;
        private int _maximum = 100;
        private int _value = 0;
        private int _borderThickness = 1;
        private bool _showPercentage = false;

        #region Свойства Кастомизации

        [Category("Niko UI")]
        [Description("Minimum scale value / Минимальное значение шкалы")]
        [DefaultValue(0)]
        public int Minimum
        {
            get => _minimum;
            set
            {
                if (value < 0) value = 0;
                if (value > _maximum) _maximum = value;

                _minimum = value;
                if (_value < _minimum) _value = _minimum;

                CustomInvalidate();
            }
        }

        [Category("Niko UI")]
        [Description("Maximum scale value / Максимальное значение шкалы")]
        [DefaultValue(100)]
        public int Maximum
        {
            get => _maximum;
            set
            {
                if (value < _minimum) value = _minimum;

                _maximum = value;
                if (_value > _maximum) _value = _maximum;

                CustomInvalidate();
            }
        }

        [Category("Niko UI")]
        [Description("Current progress value / Текущее значение шкалы прогресса")]
        [DefaultValue(0)]
        public int Value
        {
            get => _value;
            set
            {
                int clampedValue = value;
                if (clampedValue < _minimum) clampedValue = _minimum;
                if (clampedValue > _maximum) clampedValue = _maximum;

                if (_value != clampedValue)
                {
                    _value = clampedValue;
                    CustomInvalidate();
                }
            }
        }

        [Category("Niko UI")]
        [Description("Pixel border thickness / Толщина пиксельной рамки")]
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
        [Description("Show percentage text in the center / Показывать ли процент загрузки текстом по центру")]
        [DefaultValue(false)]
        public bool ShowPercentage
        {
            get => _showPercentage;
            set { _showPercentage = value; CustomInvalidate(); }
        }

        [Category("Niko UI")]
        [Description("Accent color for border and progress / Акцентный цвет (для BorderColor и ProgressColor)")]
        public Color AccentColor
        {
            get => this.BorderColor;
            set
            {
                this.BorderColor = value;
                this.ProgressColor = value;
            }
        }

        [Category("Niko UI")]
        [Description("Progress bar fill color / Цвет заполненной шкалы прогресса")]
        public Color ProgressColor { get; set; } = Color.FromArgb(158, 134, 255);

        [Category("Niko UI")]
        [Description("Outer border color / Цвет внешней рамки")]
        public Color BorderColor { get; set; } = Color.FromArgb(158, 134, 255);

        [Category("Niko UI")]
        [Description("Empty track background color / Цвет незаполненного фона шкалы")]
        public Color TrackColor { get; set; } = Color.FromArgb(12, 7, 20);

        #endregion

        public NikoProgressBar()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.UserPaint |
                     ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.ResizeRedraw |
                     ControlStyles.SupportsTransparentBackColor, true);

            this.BackColor = Color.Black;
            this.ForeColor = Color.White;
            this.Size = new Size(200, 20);
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

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            g.SmoothingMode = SmoothingMode.None;
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            using (Brush trackBrush = new SolidBrush(TrackColor))
            {
                g.FillRectangle(trackBrush, 0, 0, Width, Height);
            }

            int range = _maximum - _minimum;
            float percent = range > 0 ? (float)(_value - _minimum) / range : 0f;

            int innerPadding = _borderThickness;
            int maxProgressWidth = Width - (innerPadding * 2);
            int progressWidth = (int)Math.Round(maxProgressWidth * percent);
            int progressHeight = Height - (innerPadding * 2);

            if (progressWidth > 0)
            {
                using (Brush progressBrush = new SolidBrush(ProgressColor))
                {
                    g.FillRectangle(progressBrush, innerPadding, innerPadding, progressWidth, progressHeight);
                }
            }

            using (Pen borderPen = new Pen(BorderColor, _borderThickness))
            {
                borderPen.Alignment = PenAlignment.Inset;
                g.DrawRectangle(borderPen, 0, 0, Width, Height);
            }

            if (_showPercentage)
            {
                int currentPercent = range > 0 ? (int)((float)(_value - _minimum) / range * 100) : 0;
                string text = $"{currentPercent}%";

                Rectangle textRect = new Rectangle(innerPadding, innerPadding, maxProgressWidth, progressHeight);

                TextFormatFlags flags = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis;
                TextRenderer.DrawText(g, text, this.Font, textRect, this.ForeColor, flags);
            }
        }
    }
}