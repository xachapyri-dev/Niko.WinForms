using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Niko.WinForms
{
    /// <summary>
    /// HScroll control in Niko UI style. It allows you to customize accent color, hover color, pressed color, track color, arrow color, scroll step, and whether to show a center line or draw the thumb as a ring. It also provides a Value property and ValueChanged event for horizontal scroll position tracking. | Элемент управления горизонтальной прокруткой в стиле Niko UI. Позволяет настраивать цвет акцента, цвет при наведении, цвет при зажатии, цвет трека, цвет стрелок, шаг прокрутки и отображение центральной линии или рисование ползунка в виде кольца. Также предоставляет свойство Value и событие ValueChanged для отслеживания позиции горизонтальной прокрутки.
    /// </summary>
    public class NikoHScroll : Control
    {
        private Panel _targetPanel;
        private bool _isDragging = false;
        private int _dragXOffset = 0;
        private int _lastValue = 0;

        private ElementState _leftBtnState = ElementState.Normal;
        private ElementState _rightBtnState = ElementState.Normal;
        private ElementState _thumbState = ElementState.Normal;

        private enum ElementState { Normal, Hover, Pressed }

        private int _btnSize = 14;
        private int _thumbSize = 12;

        #region Свойства Кастомизации

        [Category("Niko UI")]
        [Description("Primary element color in normal state / Основной цвет элементов в обычном состоянии")]
        public Color AccentColor { get; set; } = Color.FromArgb(152, 101, 246);

        [Category("Niko UI")]
        [Description("Element color on mouse hover / Цвет элементов при наведении курсора")]
        public Color HoverColor { get; set; } = Color.FromArgb(180, 140, 255);

        [Category("Niko UI")]
        [Description("Element color when pressed / Цвет элементов при нажатии")]
        public Color PressedColor { get; set; } = Color.FromArgb(120, 75, 210);

        [Category("Niko UI")]
        [Description("Track background color / Цвет фона дорожки прокрутки")]
        public Color TrackColor { get; set; } = Color.FromArgb(12, 7, 20);

        [Category("Niko UI")]
        [Description("Arrow color / Цвет стрелок")]
        public Color ArrowColor { get; set; } = Color.FromArgb(12, 7, 20);

        [Category("Niko UI")]
        [Description("Distance to scroll per step / Величина шага прокрутки")]
        public int ScrollStep { get; set; } = 30;

        [Category("Niko UI")]
        [Description("Whether to show a center guide line / Показывать ли центральную направляющую линию")]
        public bool ShowCenterLine { get; set; } = true;

        [Category("Niko UI")]
        [Description("Whether to draw the scroll thumb as a ring / Отрисовывать ли ползунок в виде кольца")]
        public bool DrawThumbAsRing { get; set; } = true;

        [Category("Niko UI")]
        [Description("Current horizontal scroll position in pixels / Текущая позиция горизонтальной прокрутки в пикселях")]
        public int Value
        {
            get => _targetPanel != null ? Math.Abs(_targetPanel.AutoScrollPosition.X) : 0;
            set
            {
                if (_targetPanel == null) return;

                int totalWidth = _targetPanel.DisplayRectangle.Width;
                int viewWidth = _targetPanel.ClientRectangle.Width;
                int maxScrollX = Math.Max(0, totalWidth - viewWidth);

                int clampedValue = Math.Max(0, Math.Min(value, maxScrollX));

                _targetPanel.AutoScrollPosition = new Point(clampedValue, Math.Abs(_targetPanel.AutoScrollPosition.Y));

                this.Invalidate();
                NotifyValueChanged();
            }
        }

        [Category("Niko UI")]
        [Description("Occurs when the scroll value changes / Происходит при изменении значения прокрутки")]
        public event EventHandler ValueChanged;

        [Category("Niko UI")]
        [Description("The panel to be controlled by this scrollbar / Панель, которой управляет данный скроллбар")]
        public Panel TargetPanel
        {
            get { return _targetPanel; }
            set
            {
                if (_targetPanel != null)
                {
                    _targetPanel.Scroll -= OnPanelScroll;
                    _targetPanel.MouseWheel -= OnPanelMouseWheel;
                    _targetPanel.Resize -= OnPanelLayoutChanged;
                    _targetPanel.ControlAdded -= OnPanelLayoutChanged;
                    _targetPanel.ControlRemoved -= OnPanelLayoutChanged;
                }
                _targetPanel = value;
                if (_targetPanel != null)
                {
                    _targetPanel.Scroll += OnPanelScroll;
                    _targetPanel.MouseWheel += OnPanelMouseWheel;
                    _targetPanel.Resize += OnPanelLayoutChanged;
                    _targetPanel.ControlAdded += OnPanelLayoutChanged;
                    _targetPanel.ControlRemoved += OnPanelLayoutChanged;
                    _targetPanel.AutoScroll = true;
                }
                this.Invalidate();
                NotifyValueChanged();
            }
        }

        #endregion

        private void NotifyValueChanged()
        {
            int currentVal = Value;
            if (currentVal != _lastValue)
            {
                _lastValue = currentVal;
                ValueChanged?.Invoke(this, EventArgs.Empty);
            }
        }
        public NikoHScroll()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint |
                     ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw |
                     ControlStyles.SupportsTransparentBackColor, true);

            this.Height = 14;
            this.BackColor = Color.Transparent;
        }

        private void OnPanelScroll(object sender, ScrollEventArgs e) { this.Invalidate(); NotifyValueChanged(); }
        private void OnPanelLayoutChanged(object sender, EventArgs e) { this.Invalidate(); NotifyValueChanged(); }
        private void OnPanelMouseWheel(object sender, MouseEventArgs e) { HandleWheel(e.Delta); }
        protected override void OnMouseWheel(MouseEventArgs e) { base.OnMouseWheel(e); HandleWheel(e.Delta); }

        private void HandleWheel(int delta)
        {
            if (_targetPanel == null) return;
            int totalWidth = _targetPanel.DisplayRectangle.Width;
            int viewWidth = _targetPanel.ClientRectangle.Width;
            if (totalWidth <= viewWidth) return;

            int clicks = delta / 120;
            int currentX = Math.Abs(_targetPanel.AutoScrollPosition.X);
            int newX = currentX - (clicks * ScrollStep);

            _targetPanel.AutoScrollPosition = new Point(newX, Math.Abs(_targetPanel.AutoScrollPosition.Y));
            this.Invalidate();
            NotifyValueChanged();
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

        private Rectangle GetThumbRectangle()
        {
            if (_targetPanel == null) return Rectangle.Empty;
            int totalWidth = _targetPanel.DisplayRectangle.Width;
            int viewWidth = _targetPanel.ClientRectangle.Width;
            if (totalWidth <= viewWidth) return Rectangle.Empty;

            int usableTrackWidth = Width - (_btnSize * 2) - _thumbSize;
            int scrollX = Math.Abs(_targetPanel.AutoScrollPosition.X);
            int maxScrollX = totalWidth - viewWidth;

            float percent = Math.Max(0f, Math.Min(1f, (float)scrollX / maxScrollX));
            int thumbX = _btnSize + (int)(percent * usableTrackWidth);
            int thumbY = (Height - _thumbSize) / 2;

            return new Rectangle(thumbX, thumbY, _thumbSize, _thumbSize);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.None;

            if (TrackColor != Color.Transparent)
            {
                using (Brush trackBrush = new SolidBrush(TrackColor))
                {
                    g.FillRectangle(trackBrush, 0, 0, Width, Height);
                }
            }

            using (Brush leftBrush = new SolidBrush(GetColorForState(_leftBtnState)))
            using (Brush arrowBrush = new SolidBrush(ArrowColor))
            {
                g.FillRectangle(leftBrush, 0, 0, _btnSize, _btnSize);
                Point[] leftArrow = {
                    new Point(4, _btnSize / 2),
                    new Point(_btnSize - 5, 3),
                    new Point(_btnSize - 5, _btnSize - 4)
                };
                g.FillPolygon(arrowBrush, leftArrow);
            }

            int rx = Width - _btnSize;
            using (Brush rightBrush = new SolidBrush(GetColorForState(_rightBtnState)))
            using (Brush arrowBrush = new SolidBrush(ArrowColor))
            {
                g.FillRectangle(rightBrush, rx, 0, _btnSize, _btnSize);
                Point[] rightArrow = {
                    new Point(rx + _btnSize - 4, _btnSize / 2),
                    new Point(rx + 4, 3),
                    new Point(rx + 4, _btnSize - 4)
                };
                g.FillPolygon(arrowBrush, rightArrow);
            }

            if (_targetPanel == null) return;
            Rectangle thumbRect = GetThumbRectangle();
            if (thumbRect.IsEmpty) return;

            if (ShowCenterLine)
            {
                using (Pen linePen = new Pen(GetColorForState(_thumbState), 1))
                {
                    g.DrawLine(linePen, _btnSize, Height / 2, Width - _btnSize, Height / 2);
                }
            }

            Color currentThumbColor = GetColorForState(_thumbState);
            using (Brush thumbBrush = new SolidBrush(currentThumbColor))
            using (Pen thumbPen = new Pen(currentThumbColor, 1))
            {
                int thumbX = thumbRect.X;
                int thumbY = thumbRect.Y;

                if (DrawThumbAsRing)
                {
                    g.DrawLine(thumbPen, thumbX + 3, thumbY, thumbX + _thumbSize - 4, thumbY);
                    g.DrawLine(thumbPen, thumbX + 3, thumbY + _thumbSize - 1, thumbX + _thumbSize - 4, thumbY + _thumbSize - 1);
                    g.DrawLine(thumbPen, thumbX, thumbY + 3, thumbX, thumbY + _thumbSize - 4);
                    g.DrawLine(thumbPen, thumbX + _thumbSize - 1, thumbY + 3, thumbX + _thumbSize - 1, thumbY + _thumbSize - 4);

                    g.DrawLine(thumbPen, thumbX + 1, thumbY + 2, thumbX + 2, thumbY + 1);
                    g.DrawLine(thumbPen, thumbX + _thumbSize - 3, thumbY + 1, thumbX + _thumbSize - 2, thumbY + 2);
                    g.DrawLine(thumbPen, thumbX + 1, thumbY + _thumbSize - 3, thumbX + 2, thumbY + _thumbSize - 2);
                    g.DrawLine(thumbPen, thumbX + _thumbSize - 3, thumbY + _thumbSize - 2, thumbX + _thumbSize - 2, thumbY + _thumbSize - 3);

                    g.FillRectangle(thumbBrush, thumbX + _thumbSize / 2, Height / 2, 1, 1);
                }
                else
                {
                    g.FillRectangle(thumbBrush, thumbX + 1, thumbY + 1, _thumbSize - 2, _thumbSize - 2);
                }
            }
        }

        #region Управление мышью (X-ось)

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (_targetPanel == null || e.Button != MouseButtons.Left) return;

            if (e.X < _btnSize)
            {
                _leftBtnState = ElementState.Pressed;
                ScrollBy(-ScrollStep);
                this.Invalidate();
                return;
            }
            if (e.X > Width - _btnSize)
            {
                _rightBtnState = ElementState.Pressed;
                ScrollBy(ScrollStep);
                this.Invalidate();
                return;
            }

            Rectangle thumbRect = GetThumbRectangle();
            if (!thumbRect.IsEmpty && thumbRect.Contains(e.Location))
            {
                _isDragging = true;
                _thumbState = ElementState.Pressed;
                _dragXOffset = e.X - thumbRect.X;
                this.Capture = true;
                this.Invalidate();
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (_isDragging && _targetPanel != null)
            {
                int totalWidth = _targetPanel.DisplayRectangle.Width;
                int viewWidth = _targetPanel.ClientRectangle.Width;
                int maxScrollX = totalWidth - viewWidth;
                int usableTrackWidth = Width - (_btnSize * 2) - _thumbSize;

                int proposedThumbX = e.X - _dragXOffset - _btnSize;
                proposedThumbX = Math.Max(0, Math.Min(usableTrackWidth, proposedThumbX));

                float percent = (float)proposedThumbX / usableTrackWidth;
                int newScrollX = (int)(percent * maxScrollX);

                _targetPanel.AutoScrollPosition = new Point(newScrollX, Math.Abs(_targetPanel.AutoScrollPosition.Y));
                this.Invalidate();
                NotifyValueChanged();
                return;
            }

            ElementState oldLeft = _leftBtnState;
            ElementState oldRight = _rightBtnState;
            ElementState oldThumb = _thumbState;

            _leftBtnState = (e.X < _btnSize) ? ElementState.Hover : ElementState.Normal;
            _rightBtnState = (e.X > Width - _btnSize) ? ElementState.Hover : ElementState.Normal;

            Rectangle thumbRect = GetThumbRectangle();
            _thumbState = (!thumbRect.IsEmpty && thumbRect.Contains(e.Location)) ? ElementState.Hover : ElementState.Normal;

            if (_leftBtnState != oldLeft || _rightBtnState != oldRight || _thumbState != oldThumb)
            {
                this.Invalidate();
            }
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            _leftBtnState = ElementState.Normal;
            _rightBtnState = ElementState.Normal;
            if (!_isDragging) _thumbState = ElementState.Normal;
            this.Invalidate();
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            _leftBtnState = ElementState.Normal;
            _rightBtnState = ElementState.Normal;

            if (_isDragging)
            {
                _isDragging = false;
                this.Capture = false;
                _thumbState = GetThumbRectangle().Contains(e.Location) ? ElementState.Hover : ElementState.Normal;
            }
            this.Invalidate();
            NotifyValueChanged();
        }

        #endregion

        private void ScrollBy(int value)
        {
            if (_targetPanel == null) return;
            int currentX = Math.Abs(_targetPanel.AutoScrollPosition.X);
            _targetPanel.AutoScrollPosition = new Point(currentX + value, Math.Abs(_targetPanel.AutoScrollPosition.Y));
            this.Invalidate();
            NotifyValueChanged();
        }
    }
}