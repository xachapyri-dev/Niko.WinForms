using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Niko.WinForms
{
    /// <summary>
    /// TextBox control in Niko UI style. It allows you to customize accent color, hover color, focused color, box background color, border thickness, and other standard TextBox properties. | Элемент управления текстовым полем в стиле Niko UI. Позволяет настраивать цвет акцента, цвет при наведении, цвет при фокусе, цвет фона текстового поля, толщину рамки и другие стандартные свойства TextBox.
    /// </summary>
    [DefaultEvent("TextChanged")]
    public class NikoTextBox : Control
    {
        private readonly TextBox _innerTextBox;
        private bool _isHovered = false;
        private int _borderThickness = 1;

        #region Свойства Кастомизации

        [Category("Niko UI")]
        [Description("Primary border color in normal state / Основной цвет рамки в обычном состоянии")]
        public Color AccentColor { get; set; } = Color.FromArgb(158, 134, 255);

        [Category("Niko UI")]
        [Description("Border color on mouse hover / Цвет рамки при наведении мыши")]
        public Color HoverColor { get; set; } = Color.FromArgb(180, 140, 255);

        [Category("Niko UI")]
        [Description("Border color when the field is in focus / Цвет рамки, когда поле находится в фокусе")]
        public Color FocusedColor { get; set; } = Color.FromArgb(120, 75, 210);

        [Category("Niko UI")]
        [Description("Text field background color / Цвет фона текстового поля")]
        public Color BoxBackColor { get; set; } = Color.FromArgb(12, 7, 20);

        [Category("Niko UI")]
        [Description("Thickness of the pixel border / Толщина пиксельной рамки")]
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
                    UpdateTextBoxLayout();
                    this.Invalidate();
                }
            }
        }

        [Category("Niko UI")]
        [Description("The text content of the field / Текстовое содержимое поля")]
        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        public override string Text
        {
            get => _innerTextBox != null ? _innerTextBox.Text : base.Text;
            set
            {
                base.Text = value;
                if (_innerTextBox != null) _innerTextBox.Text = value;
            }
        }

        [Category("Niko UI")]
        [Description("Indicates whether the text box can contain multiple lines / Указывает, может ли поле содержать несколько строк")]
        public bool Multiline
        {
            get => _innerTextBox.Multiline;
            set { _innerTextBox.Multiline = value; UpdateTextBoxLayout(); }
        }

        [Category("Niko UI")]
        [Description("Character used to mask passwords / Символ, используемый для маскировки пароля")]
        public char PasswordChar
        {
            get => _innerTextBox.PasswordChar;
            set => _innerTextBox.PasswordChar = value;
        }

        [Category("Niko UI")]
        [Description("Indicates whether to use the system password character / Указывает, использовать ли системный символ пароля")]
        public bool UseSystemPasswordChar
        {
            get => _innerTextBox.UseSystemPasswordChar;
            set => _innerTextBox.UseSystemPasswordChar = value;
        }

        [Category("Niko UI")]
        [Description("Maximum number of characters allowed / Максимальное количество разрешенных символов")]
        public int MaxLength
        {
            get => _innerTextBox.MaxLength;
            set => _innerTextBox.MaxLength = value;
        }

        [Category("Niko UI")]
        [Description("Indicates whether the text is read-only / Указывает, является ли текст доступным только для чтения")]
        public bool ReadOnly
        {
            get => _innerTextBox.ReadOnly;
            set
            {
                _innerTextBox.ReadOnly = value;
                this.Invalidate();
            }
        }

        [Category("Niko UI")]
        [Description("Font used for the text / Шрифт, используемый для текста")]
        public override Font Font
        {
            get => base.Font;
            set { base.Font = value; _innerTextBox.Font = value; UpdateTextBoxLayout(); }
        }

        [Category("Niko UI")]
        [Description("Text color / Цвет текста")]
        public override Color ForeColor
        {
            get => base.ForeColor;
            set { base.ForeColor = value; _innerTextBox.ForeColor = value; }
        }

        #endregion

        public NikoTextBox()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.UserPaint |
                     ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.ResizeRedraw |
                     ControlStyles.SupportsTransparentBackColor, true);

            this.BackColor = Color.Black;
            this.Size = new Size(150, 26);
            this.BoxBackColor = Color.Black;

            _innerTextBox = new TextBox
            {
                BorderStyle = BorderStyle.None,
                BackColor = this.BoxBackColor,
                ForeColor = AccentColor,
                Font = this.Font,
                Location = new Point(5, 5)
            };

            _innerTextBox.TextChanged += (s, e) => { base.Text = _innerTextBox.Text; OnTextChanged(e); };
            _innerTextBox.Enter += (s, e) => this.Invalidate();
            _innerTextBox.Leave += (s, e) => this.Invalidate();

            _innerTextBox.MouseEnter += (s, e) => { _isHovered = true; this.Invalidate(); };
            _innerTextBox.MouseLeave += (s, e) => { _isHovered = false; this.Invalidate(); };

            this.Controls.Add(_innerTextBox);

            UpdateTextBoxLayout();
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            _innerTextBox.Focus();
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            UpdateTextBoxLayout();
        }

        private void UpdateTextBoxLayout()
        {
            if (_innerTextBox == null) return;

            int sidePadding = _borderThickness + 4;

            _innerTextBox.Left = sidePadding;
            _innerTextBox.Width = this.Width - (sidePadding * 2);

            if (_innerTextBox.Multiline)
            {
                _innerTextBox.Top = sidePadding;
                _innerTextBox.Height = this.Height - (sidePadding * 2);
            }
            else
            {
                _innerTextBox.Top = (this.Height - _innerTextBox.Height) / 2;
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.None;

            if (_innerTextBox.BackColor != BoxBackColor)
            {
                _innerTextBox.BackColor = BoxBackColor;
            }

            using (Brush bgBrush = new SolidBrush(BoxBackColor))
            {
                g.FillRectangle(bgBrush, 0, 0, Width, Height);
            }

            Color currentBorderColor = AccentColor;
            if (_innerTextBox.Focused)
            {
                currentBorderColor = FocusedColor;
            }
            else if (_isHovered)
            {
                currentBorderColor = HoverColor;
            }

            using (Pen borderPen = new Pen(currentBorderColor, _borderThickness))
            {
                borderPen.Alignment = PenAlignment.Inset;
                g.DrawRectangle(borderPen, 0, 0, Width, Height);
            }
        }
    }
}