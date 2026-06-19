using System.Windows.Forms;
using System.ComponentModel;
using System.Drawing;

namespace Niko.WinForms
{
    /// <summary>
    /// LabelLink control in Niko UI style. It allows you to customize text, accent color, visited accent color, and font. | Элемент управления LabelLink в стиле Niko UI. Позволяет настраивать текст, цвет акцента, цвет акцента после посещения и шрифт.
    /// </summary>
    public class NikoLabelLink : LinkLabel
    {
        public NikoLabelLink()
        {
            this.BackColor = Color.Black;
            this.LinkColor = Color.FromArgb(152, 101, 246);
        }

        [Category("Niko UI")]
        [Browsable(true)]
        [Description("Element text / Текст элемента")]
        public override string Text
        {
            get => base.Text;
            set => base.Text = value;
        }

        [Category("Niko UI")]
        [Browsable(true)]
        [Description("Text color / Цвет текста")]
        public Color AccentColor
        {
            get => this.LinkColor;
            set => this.LinkColor = value;
        }

        [Category("Niko UI")]
        [Browsable(true)]
        [Description("Text font / Шрифт текста")]
        public Font TextFont
        {
            get => this.Font;
            set => this.Font = value;
        }

        [Category("Niko UI")]
        [Browsable(true)]
        [Description("Text color after visiting / Цвет текста после нажатия")]
        public Color VisitedAccentColor
        {
            get => this.VisitedLinkColor;
            set => this.VisitedLinkColor = value;
        }

    }
}
