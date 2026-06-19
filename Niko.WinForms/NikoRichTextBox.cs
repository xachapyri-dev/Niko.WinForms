using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Niko.WinForms
{
    /// <summary>
    /// A custom RichTextBox control in Niko UI style. It allows you to customize accent color, border color, background color, text color, and border thickness. | Кастомный элемент управления RichTextBox в стиле Niko UI. Позволяет настраивать цвет акцента, цвет рамки, цвет фона, цвет текста и толщину рамки.
    /// </summary>
    [DefaultProperty("Text")]
    public class NikoRichTextBox : Control
    {
        private int _borderThickness = 1;
        private readonly RichTextBox _internalTextBox;
        private bool _isFocused = false;

        #region Настройки стиля Niko UI

        [Category("Niko UI")]
        [Description("Primary accent color / Основной акцентный цвет")]
        public Color AccentColor { get; set; } = Color.FromArgb(158, 134, 255);

        [Category("Niko UI")]
        [Description("Pixel border color / Цвет пиксельной рамки")]
        public Color BorderColor { get; set; } = Color.FromArgb(158, 134, 255);

        [Category("Niko UI")]
        [Description("Deep background color for boxes / Глубокий цвет фона для блоков")]
        public Color BoxBackColor { get; set; } = Color.FromArgb(12, 7, 20);

        [Category("Niko UI")]
        [Description("Regular text color / Обычный цвет текста")]
        public Color TextColor { get; set; } = Color.FromArgb(220, 215, 230);

        [Category("Niko UI")]
        [Description("The text contained in the control / Текст, отображаемый в элементе управления")]
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public override string Text
        {
            get => _internalTextBox.Text;
            set => _internalTextBox.Text = value;
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
                }
            }
        }

        [Category("Niko UI")]
        [Description("RTF formatted text / Текст в формате RTF")]
        public string Rtf
        {
            get => _internalTextBox.Rtf;
            set => _internalTextBox.Rtf = value;
        }

        [Category("Niko UI")]
        [Description("Indicates whether the control is read-only / Указывает, доступен ли элемент только для чтения")]
        public bool ReadOnly
        {
            get => _internalTextBox.ReadOnly;
            set => _internalTextBox.ReadOnly = value;
        }

        #endregion

        public NikoRichTextBox()
        {
            SetStyle(ControlStyles.DoubleBuffer | ControlStyles.ResizeRedraw, true);

            this.Size = new Size(250, 100);
            this.BackColor = Color.Black;

            _internalTextBox = new RichTextBox
            {
                BorderStyle = BorderStyle.None,
                BackColor = Color.Black,
                ForeColor = this.TextColor,
                Font = this.Font,
                Location = new Point(5, 5), 
                Size = new Size(this.Width - 10, this.Height - 10),
                ScrollBars = RichTextBoxScrollBars.Vertical
            };

            _internalTextBox.Enter += (s, e) => { _isFocused = true; this.Invalidate(); };
            _internalTextBox.Leave += (s, e) => { _isFocused = false; this.Invalidate(); };
            _internalTextBox.TextChanged += (s, e) => OnTextChanged(e);

            this.Controls.Add(_internalTextBox);
        }

        protected override void OnFontChanged(EventArgs e)
        {
            base.OnFontChanged(e);
            if (_internalTextBox != null) _internalTextBox.Font = this.Font;
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            if (_internalTextBox != null)
            {
                _internalTextBox.Size = new Size(this.Width - 10, this.Height - 10);
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            _internalTextBox.Focus();
        }

        #region Отрисовка пиксельной рамки

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.None;

            Rectangle rect = new Rectangle(0, 0, this.Width - 1, this.Height - 1);

            using (Brush bgBrush = new SolidBrush(this.BoxBackColor))
            {
                g.FillRectangle(bgBrush, rect);
            }

            Color currentBorder = _isFocused ? AccentColor : BorderColor;

            using (Pen p = new Pen(currentBorder, _borderThickness))
            {
                g.DrawRectangle(p, rect);
            }
        }

        #endregion
    }
}