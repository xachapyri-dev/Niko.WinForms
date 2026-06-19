using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Niko.WinForms
{
    /// <summary>
    /// VScrollBar control in Niko UI style. It allows you to customize accent color, hover color, pressed color, track color, arrow color, scroll step, and whether to show a center line and draw the thumb as a ring. | Элемент управления вертикальной полосой прокрутки в стиле Niko UI. Позволяет настраивать цвет акцента, цвет при наведении, цвет при зажатии, цвет фона трека, цвет стрелок, шаг прокрутки и отображение центральной линии и рисование ползунка в виде кольца.
    /// </summary>
    public class NikoVScroll : Control
    {
        private Panel _targetPanel;
        private bool _isDragging = false;
        private int _dragYOffset = 0;
        private int _lastValue = 0;

        private ElementState _upBtnState = ElementState.Normal;
        private ElementState _downBtnState = ElementState.Normal;
        private ElementState _thumbState = ElementState.Normal;

        private enum ElementState { Normal, Hover, Pressed }

        private int _btnSize = 14;
        private int _thumbSize = 12;

        #region Свойства Кастомизации

        [Category("Niko UI")]
        [Description("Primary element color in normal state / Основной цвет элементов в обычном состоянии")]
        public Color AccentColor { get; set; } = Color.FromArgb(158, 134, 255);

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
        [Description("Current vertical scroll position in pixels / Текущая позиция вертикальной прокрутки в пикселях")]
        public int Value
        {
            get => _targetPanel != null ? Math.Abs(_targetPanel.AutoScrollPosition.Y) : 0;
            set
            {
                if (_targetPanel == null) return;

                int totalHeight = _targetPanel.DisplayRectangle.Height;
                int viewHeight = _targetPanel.ClientRectangle.Height;
                int maxScrollY = Math.Max(0, totalHeight - viewHeight);

                int clampedValue = Math.Max(0, Math.Min(value, maxScrollY));

                _targetPanel.AutoScrollPosition = new Point(Math.Abs(_targetPanel.AutoScrollPosition.X), clampedValue);

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

        public NikoVScroll()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint |
                     ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw |
                     ControlStyles.SupportsTransparentBackColor, true);

            this.Width = 14;
            this.BackColor = Color.Transparent;
        }

        private void OnPanelScroll(object sender, ScrollEventArgs e) { this.Invalidate(); NotifyValueChanged(); }
        private void OnPanelLayoutChanged(object sender, EventArgs e) { this.Invalidate(); NotifyValueChanged(); }
        private void OnPanelMouseWheel(object sender, MouseEventArgs e) { HandleWheel(e.Delta); }
        protected override void OnMouseWheel(MouseEventArgs e) { base.OnMouseWheel(e); HandleWheel(e.Delta); }

        private void HandleWheel(int delta)
        {
            if (_targetPanel == null) return;
            int totalHeight = _targetPanel.DisplayRectangle.Height;
            int viewHeight = _targetPanel.ClientRectangle.Height;
            if (totalHeight <= viewHeight) return;

            int clicks = delta / 120;
            int currentY = Math.Abs(_targetPanel.AutoScrollPosition.Y);
            int newY = currentY - (clicks * ScrollStep);

            _targetPanel.AutoScrollPosition = new Point(Math.Abs(_targetPanel.AutoScrollPosition.X), newY);
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
            int totalHeight = _targetPanel.DisplayRectangle.Height;
            int viewHeight = _targetPanel.ClientRectangle.Height;
            if (totalHeight <= viewHeight) return Rectangle.Empty;

            int usableTrackHeight = Height - (_btnSize * 2) - _thumbSize;
            int scrollY = Math.Abs(_targetPanel.AutoScrollPosition.Y);
            int maxScrollY = totalHeight - viewHeight;

            float percent = Math.Max(0f, Math.Min(1f, (float)scrollY / maxScrollY));
            int thumbY = _btnSize + (int)(percent * usableTrackHeight);
            int thumbX = (Width - _thumbSize) / 2;

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

            using (Brush upBrush = new SolidBrush(GetColorForState(_upBtnState)))
            using (Brush arrowBrush = new SolidBrush(ArrowColor))
            {
                g.FillRectangle(upBrush, 0, 0, _btnSize, _btnSize);
                Point[] upArrow = {
                    new Point(_btnSize / 2, 4),
                    new Point(3, _btnSize - 5),
                    new Point(_btnSize - 4, _btnSize - 5)
                };
                g.FillPolygon(arrowBrush, upArrow);
            }

            using (Brush downBrush = new SolidBrush(GetColorForState(_downBtnState)))
            using (Brush arrowBrush = new SolidBrush(ArrowColor))
            {
                g.FillRectangle(downBrush, 0, Height - _btnSize, _btnSize, _btnSize);
                Point[] downArrow = {
                    new Point(3, Height - _btnSize + 4),
                    new Point(_btnSize - 4, Height - _btnSize + 4),
                    new Point(_btnSize / 2, Height - 5)
                };
                g.FillPolygon(arrowBrush, downArrow);
            }

            if (_targetPanel == null) return;
            Rectangle thumbRect = GetThumbRectangle();
            if (thumbRect.IsEmpty) return;

            if (ShowCenterLine)
            {
                using (Pen linePen = new Pen(GetColorForState(_thumbState), 1))
                {
                    g.DrawLine(linePen, Width / 2, _btnSize, Width / 2, Height - _btnSize);
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

                    g.FillRectangle(thumbBrush, Width / 2, thumbY + _thumbSize / 2, 1, 1);
                }
                else
                {
                    g.FillRectangle(thumbBrush, thumbX + 1, thumbY + 1, _thumbSize - 2, _thumbSize - 2);
                }
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (_targetPanel == null || e.Button != MouseButtons.Left) return;

            if (e.Y < _btnSize)
            {
                _upBtnState = ElementState.Pressed;
                ScrollBy(-ScrollStep);
                this.Invalidate();
                return;
            }
            if (e.Y > Height - _btnSize)
            {
                _downBtnState = ElementState.Pressed;
                ScrollBy(ScrollStep);
                this.Invalidate();
                return;
            }

            Rectangle thumbRect = GetThumbRectangle();
            if (!thumbRect.IsEmpty && thumbRect.Contains(e.Location))
            {
                _isDragging = true;
                _thumbState = ElementState.Pressed;
                _dragYOffset = e.Y - thumbRect.Y;
                this.Capture = true;
                this.Invalidate();
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (_isDragging && _targetPanel != null)
            {
                int totalHeight = _targetPanel.DisplayRectangle.Height;
                int viewHeight = _targetPanel.ClientRectangle.Height;
                int maxScrollY = totalHeight - viewHeight;
                int usableTrackHeight = Height - (_btnSize * 2) - _thumbSize;

                int proposedThumbY = e.Y - _dragYOffset - _btnSize;
                proposedThumbY = Math.Max(0, Math.Min(usableTrackHeight, proposedThumbY));

                float percent = (float)proposedThumbY / usableTrackHeight;
                int newScrollY = (int)(percent * maxScrollY);

                _targetPanel.AutoScrollPosition = new Point(Math.Abs(_targetPanel.AutoScrollPosition.X), newScrollY);
                this.Invalidate();
                NotifyValueChanged();
                return;
            }

            ElementState oldUp = _upBtnState;
            ElementState oldDown = _downBtnState;
            ElementState oldThumb = _thumbState;

            _upBtnState = (e.Y < _btnSize) ? ElementState.Hover : ElementState.Normal;
            _downBtnState = (e.Y > Height - _btnSize) ? ElementState.Hover : ElementState.Normal;

            Rectangle thumbRect = GetThumbRectangle();
            _thumbState = (!thumbRect.IsEmpty && thumbRect.Contains(e.Location)) ? ElementState.Hover : ElementState.Normal;

            if (_upBtnState != oldUp || _downBtnState != oldDown || _thumbState != oldThumb)
            {
                this.Invalidate();
            }
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            _upBtnState = ElementState.Normal;
            _downBtnState = ElementState.Normal;
            if (!_isDragging) _thumbState = ElementState.Normal;
            this.Invalidate();
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            _upBtnState = ElementState.Normal;
            _downBtnState = ElementState.Normal;

            if (_isDragging)
            {
                _isDragging = false;
                this.Capture = false;
                _thumbState = GetThumbRectangle().Contains(e.Location) ? ElementState.Hover : ElementState.Normal;
            }
            this.Invalidate();
            NotifyValueChanged();
        }

        private void ScrollBy(int value)
        {
            if (_targetPanel == null) return;
            int currentY = Math.Abs(_targetPanel.AutoScrollPosition.Y);
            _targetPanel.AutoScrollPosition = new Point(Math.Abs(_targetPanel.AutoScrollPosition.X), currentY + value);
            this.Invalidate();
            NotifyValueChanged();
        }
    }
}
// Очень трудна, хочу сдохнуть (шутка)
// Кастомить WinAPI элемент трудно, и поэтому обычно создают Control элемент и делают скрол с нуля