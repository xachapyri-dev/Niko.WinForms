using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Niko.WinForms
{
    /// <summary>
    /// Context menu with a custom renderer in Niko UI style. Allows you to customize background, border, accent, and text colors. | Контекстное меню с кастомным рендерером в стиле Niko UI. Позволяет настраивать цвета фона, рамки, акцента и текста.
    /// </summary>
    public class NikoContextMenuStrip : ContextMenuStrip
    {
        private readonly NikoMenuRenderer _customRenderer;

        #region Проброс свойств в панель Свойства (Properties)

        [Category("Niko UI")]
        [Description("Dropdown menu background color / Цвет фона выпадающего меню")]
        public Color BackgroundColor
        {
            get => _customRenderer.BackgroundColor;
            set => _customRenderer.BackgroundColor = value;
        }

        [Category("Niko UI")]
        [Description("Pixel border color / Цвет пиксельной рамки")]
        public Color BorderColor
        {
            get => _customRenderer.BorderColor;
            set => _customRenderer.BorderColor = value;
        }

        [Category("Niko UI")]
        [Description("Pixel border size / Толщина пиксельной рамки")]
        [DefaultValue(1)]
        public int BorderSize
        {
            get => _customRenderer.BorderSize;
            set => _customRenderer.BorderSize = value;
        }

        [Category("Niko UI")]
        [Description("Highlight color on hover / Цвет подсветки активного пункта при наведении")]
        public Color AccentColor
        {
            get => _customRenderer.AccentColor;
            set => _customRenderer.AccentColor = value;
        }

        [Category("Niko UI")]
        [Description("Regular menu item text color / Обычный цвет текста пунктов меню")]
        public Color TextColor
        {
            get => _customRenderer.TextColor;
            set => _customRenderer.TextColor = value;
        }

        [Category("Niko UI")]
        [Description("Selected menu item text color / Цвет текста пункта при наведении")]
        public Color SelectedTextColor
        {
            get => _customRenderer.SelectedTextColor;
            set => _customRenderer.SelectedTextColor = value;
        }

        [Category("Niko UI")]
        [Description("Should the outline be the main color / Делать ли обводку главным цветом")]
        public bool AccentColorToBorderColor { get; set; } = true;

        #endregion

        public NikoContextMenuStrip() : base()
        {
            _customRenderer = new NikoMenuRenderer();
            this.Renderer = _customRenderer;
            this.ImageScalingSize = new Size(16, 16);
        }

        public NikoContextMenuStrip(IContainer container) : base(container)
        {
            _customRenderer = new NikoMenuRenderer();
            this.Renderer = _customRenderer;
            this.ImageScalingSize = new Size(16, 16);
        }
    }

    public class NikoMenuRenderer : ToolStripRenderer
    {
        public Color BackgroundColor { get; set; } = Color.FromArgb(12, 7, 20);
        public Color BorderColor { get; set; } = Color.FromArgb(158, 134, 255);
        public int BorderSize { get; set; } = 1;
        public Color AccentColor { get; set; } = Color.FromArgb(158, 134, 255);
        public Color TextColor { get; set; } = Color.FromArgb(220, 215, 230);
        public Color SelectedTextColor { get; set; } = Color.FromArgb(12, 7, 20);
        public bool AccentColorToBorderColor { get; set; } = true;

        protected override void OnRenderToolStripBackground(ToolStripRenderEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.None;
            using (Brush bgBrush = new SolidBrush(BackgroundColor))
                g.FillRectangle(bgBrush, e.AffectedBounds);
        }

        protected override void OnRenderToolStripBorder(ToolStripRenderEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.None;

            if (BorderSize <= 0) return;

            if (!AccentColorToBorderColor)
            {
                using (Pen p = new Pen(BorderColor, BorderSize))
                {
                    p.Alignment = PenAlignment.Inset;
                    g.DrawRectangle(p, 0, 0, e.ToolStrip.Width - 1, e.ToolStrip.Height - 1);
                }
            }
            else
            {
                using (Pen p = new Pen(AccentColor, BorderSize))
                {
                    p.Alignment = PenAlignment.Inset;

                    g.DrawRectangle(p, 0, 0, e.ToolStrip.Width - 1, e.ToolStrip.Height - 1);
                }
            }

        }

        protected override void OnRenderImageMargin(ToolStripRenderEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.None;
            using (Brush bgBrush = new SolidBrush(BackgroundColor))
                g.FillRectangle(bgBrush, e.AffectedBounds);
        }

        protected override void OnRenderMenuItemBackground(ToolStripItemRenderEventArgs e)
        {
            if (!e.Item.Enabled) return;
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.None;

            if (e.Item.Selected)
            {
                int offset = BorderSize + 1;
                using (Brush hoverBrush = new SolidBrush(AccentColor))
                {
                    g.FillRectangle(hoverBrush, new Rectangle(offset, 1, e.Item.Width - (offset * 2), e.Item.Height - 2));
                }
            }
        }

        protected override void OnRenderItemText(ToolStripItemTextRenderEventArgs e)
        {
            if (!e.Item.Enabled) e.TextColor = Color.FromArgb(70, 60, 90);
            else if (e.Item.Selected) e.TextColor = SelectedTextColor;
            else e.TextColor = TextColor;

            e.TextFormat |= TextFormatFlags.NoPrefix;
            base.OnRenderItemText(e);
        }

        protected override void OnRenderSeparator(ToolStripSeparatorRenderEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.None;
            if (!AccentColorToBorderColor)
            {
                using (Pen p = new Pen(BorderColor, 1))
                {
                    int y = e.Item.Height / 2;
                    g.DrawLine(p, BorderSize + 4, y, e.Item.Width - (BorderSize + 4), y);
                }
            }
            else
            {
                using (Pen p = new Pen(AccentColor, 1))
                {
                    int y = e.Item.Height / 2;
                    g.DrawLine(p, BorderSize + 4, y, e.Item.Width - (BorderSize + 4), y);
                }
            }
            
        }

        protected override void OnRenderArrow(ToolStripArrowRenderEventArgs e)
        {
            e.ArrowColor = e.Item.Selected ? SelectedTextColor : AccentColor;
            base.OnRenderArrow(e);
        }
    }
}