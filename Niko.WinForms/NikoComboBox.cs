using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;

namespace Niko.WinForms
{
    /// <summary>
    /// ComboBox control in Niko UI style. It allows you to customize background color, border color, accent color, text color, selected text color, and border thickness. | Элемент управления ComboBox в стиле Niko UI. Позволяет настраивать цвет фона, цвет рамки, акцентный цвет, цвет текста, цвет текста при выборе и толщину рамки.
    /// </summary>
    public class NikoComboBox : ComboBox
    {
        [Category("Niko UI")]
        [Description("Background color of the container / Цвет фона контейнера")]
        public Color BoxBackColor { get; set; } = Color.Black;

        [Category("Niko UI")]
        [Description("Border color / Цвет рамки")]
        public Color BorderColor { get; set; } = Color.FromArgb(152, 101, 246);

        [Category("Niko UI")]
        [Description("Primary accent color / Основной акцентный цвет")]
        public Color AccentColor { get; set; } = Color.FromArgb(152, 101, 246);

        [Category("Niko UI")]
        [Description("Text color / Цвет текста")]
        public Color TextColor { get; set; } = Color.FromArgb(220, 215, 230);

        [Category("Niko UI")]
        [Description("Text color for selected items / Цвет текста для выбранных элементов")]
        public Color SelectedTextColor { get; set; } = Color.FromArgb(12, 7, 20);

        [Category("Niko UI")]
        [DefaultValue(1)]
        [Description("Border thickness in pixels / Толщина рамки в пикселях")]
        public int BorderSize { get; set; } = 1;

        [Category("Niko UI")]
        [Description("Should the outline be the main color / Делать ли обводку главным цветом")]
        public bool AccentColorToBorderColor { get; set; } = true;

        public NikoComboBox()
        {
            this.SetStyle(ControlStyles.UserPaint |
                          ControlStyles.AllPaintingInWmPaint |
                          ControlStyles.OptimizedDoubleBuffer, true);

            this.DrawMode = DrawMode.OwnerDrawFixed;
            this.DropDownStyle = ComboBoxStyle.DropDownList;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.None;
            g.TextRenderingHint = TextRenderingHint.SingleBitPerPixelGridFit;

            using (Brush bgBrush = new SolidBrush(BoxBackColor))
            {
                g.FillRectangle(bgBrush, this.ClientRectangle);
            }

            if (BorderSize > 0)
            {
                if (!AccentColorToBorderColor)
                {
                    using (Pen p = new Pen(BorderColor, BorderSize))
                    {
                        p.Alignment = PenAlignment.Inset;
                        g.DrawRectangle(p, 0, 0, this.Width - 1, this.Height - 1);
                    }
                }
                else
                {
                    using (Pen p = new Pen(AccentColor, BorderSize))
                    {
                        p.Alignment = PenAlignment.Inset;
                        g.DrawRectangle(p, 0, 0, this.Width - 1, this.Height - 1);
                    }

                }
                
            }

            int arrowSize = 6;
            int arrowX = this.Width - 18;
            int arrowY = (this.Height - arrowSize) / 2 + 1;

            using (Brush arrowBrush = new SolidBrush(AccentColor))
            {
                Point[] points = new Point[]
                {
                    new Point(arrowX, arrowY),
                    new Point(arrowX + 8, arrowY),
                    new Point(arrowX + 4, arrowY + 4)
                };
                g.FillPolygon(arrowBrush, points);
            }

            string text = this.SelectedIndex >= 0 ? this.SelectedItem.ToString() : this.Text;
            int textOffset = BorderSize + 5;

            using (Brush textBrush = new SolidBrush(TextColor))
            {
                SizeF textSize = g.MeasureString(text, this.Font);
                float textY = (this.Height - textSize.Height) / 2;

                g.DrawString(text, this.Font, textBrush, new PointF(textOffset, textY));
            }
        }

        protected override void OnDrawItem(DrawItemEventArgs e)
        {
            if (e.Index < 0) return;

            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.None;
            g.TextRenderingHint = TextRenderingHint.SingleBitPerPixelGridFit;

            string itemText = this.Items[e.Index].ToString();
            bool isSelected = (e.State & DrawItemState.Selected) == DrawItemState.Selected;

            Color itemBg = isSelected ? AccentColor : BoxBackColor;
            Color itemTextCol = isSelected ? SelectedTextColor : TextColor;

            using (Brush bgBrush = new SolidBrush(itemBg))
            {
                g.FillRectangle(bgBrush, e.Bounds);
            }

            using (Brush textBrush = new SolidBrush(itemTextCol))
            {
                Rectangle textBounds = new Rectangle(e.Bounds.X + 4, e.Bounds.Y, e.Bounds.Width - 4, e.Bounds.Height);

                StringFormat sf = new StringFormat
                {
                    LineAlignment = StringAlignment.Center,
                    Alignment = StringAlignment.Near
                };

                g.DrawString(itemText, e.Font, textBrush, textBounds, sf);
            }
        }
    }
}