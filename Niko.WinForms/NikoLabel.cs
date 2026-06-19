using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Niko.WinForms
{
    /// <summary>
    /// Label control in Niko UI style. It allows you to customize text, accent color, and font. | Элемент управления Label в стиле Niko UI. Позволяет настраивать текст, цвет акцента и шрифт.
    /// </summary>
    public class NikoLabel : Label
    {
        public NikoLabel()
        {
            this.BackColor = Color.Black;
            this.ForeColor = Color.FromArgb(152, 101, 246);
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
            get => this.ForeColor;
            set => this.ForeColor = value;
        }

        [Category("Niko UI")]
        [Browsable(true)]
        [Description("Text font / Шрифт текста")]
        public Font TextFont
        {
            get => this.Font;
            set => this.Font = value;
        }
    }
}