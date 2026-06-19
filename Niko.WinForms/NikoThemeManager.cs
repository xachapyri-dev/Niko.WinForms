using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Reflection;

namespace Niko.WinForms
{
    /// <summary>
    /// Color / Цвет
    /// </summary>
    public enum NikoTheme
    {
        Purple, Blue, Teal, Yellow, White, Red, Pink, Orange, Green, Rainbow, Custom
    }
    /// <summary>
    /// A component for convenient management of accent colors \ Компонент для удобного управления цветам акцента
    /// </summary>
    [Description("A component for convenient management of accent colors / Компонент для удобного управления цветам акцента")]
    public class NikoThemeManager : Component
    {
        private NikoForm _targetForm;
        private NikoTheme _currentTheme = NikoTheme.Purple;
        private Timer _rainbowTimer;
        private float _rainbowHue = 0f;
        private Color _accentColor = Color.FromArgb(152, 101, 246);

        [Category("Niko UI")]
        public NikoForm TargetForm
        {
            get => _targetForm;
            set
            {
                if (_targetForm != null) _targetForm.Load -= TargetForm_Load;
                _targetForm = value;
                if (_targetForm != null) _targetForm.Load += TargetForm_Load;

                if (_targetForm != null) UpdateEverything();
            }
        }

        [Category("Niko UI")]
        [DefaultValue(NikoTheme.Purple)]
        public NikoTheme Theme
        {
            get => _currentTheme;
            set
            {
                _currentTheme = value;
                ApplyTemplateColors();
                if (_targetForm != null) UpdateEverything();
            }
        }

        [Category("Niko UI")]
        public Color AccentColor
        {
            get => _accentColor;
            set
            {
                _accentColor = value;
                if (_currentTheme != NikoTheme.Custom) _currentTheme = NikoTheme.Custom;
                if (_targetForm != null) UpdateEverything();
            }
        }

        public NikoThemeManager() { InitTimer(); ApplyTemplateColors(); }
        public NikoThemeManager(IContainer container) { container.Add(this); InitTimer(); ApplyTemplateColors(); }

        private void InitTimer()
        {
            _rainbowTimer = new Timer { Interval = 40 };
            _rainbowTimer.Tick += RainbowTimer_Tick;
        }

        private void TargetForm_Load(object sender, EventArgs e)
        {
            UpdateEverything();
        }

        private void RainbowTimer_Tick(object sender, EventArgs e)
        {
            _rainbowHue += 2f;
            if (_rainbowHue >= 360f) _rainbowHue = 0f;

            _accentColor = ColorFromHSV(_rainbowHue, 0.8, 0.95);

            if (_targetForm != null) UpdateEverything();
        }
        /// <summary>
        /// Update color / обновить цвет
        /// </summary>
        public void UpdateEverything()
        {
            if (_targetForm == null) return;

            if (_targetForm.BackColor != Color.Black) _targetForm.BackColor = Color.Black;
            _targetForm.AccentColor = _accentColor;

            // В режиме дизайнера не используем рефлексию по компонентам, 
            // чтобы не крашить Visual Studio и не вызывать циклических блокировок (на себе тестировал)
            if (!DesignMode)
            {
                ProcessFormComponents();
            }

            foreach (Control control in _targetForm.Controls)
            {
                ProcessControlRecursive(control);
            }
        }

        private void ProcessControlRecursive(Control control)
        {
            if (control == null) return;

            if (control.ContextMenuStrip is NikoContextMenuStrip controlMenu)
            {
                UpdateContextMenu(controlMenu);
            }

            switch (control)
            {
                case NikoButton btn: btn.AccentColor = _accentColor; break;
                case NikoLabel label: label.ForeColor = _accentColor; break;
                case NikoComboBox combobox:
                    combobox.AccentColor = _accentColor;
                    combobox.BorderColor = _accentColor;
                    combobox.TextColor = _accentColor;
                    combobox.Invalidate();
                    break;
                case NikoContextMenuStrip contextmenu: UpdateContextMenu(contextmenu); break;
                case NikoRichTextBox rtb:
                    rtb.AccentColor = _accentColor;
                    rtb.BorderColor = _accentColor;
                    rtb.TextColor = _accentColor;
                    rtb.Invalidate();
                    break;
                case NikoProgressBar pb:
                    pb.AccentColor = _accentColor;
                    pb.BorderColor = _accentColor;
                    pb.Invalidate();
                    break;
                case NikoRadioButton rb:
                    rb.AccentColor = _accentColor; rb.ForeColor = _accentColor;
                    rb.HoverColor = _accentColor; rb.PressedColor = _accentColor;
                    rb.Invalidate();
                    break;
                case NikoTextBox tb:
                    tb.AccentColor = _accentColor; tb.ForeColor = _accentColor;
                    tb.HoverColor = _accentColor; tb.FocusedColor = _accentColor;
                    tb.Invalidate();
                    break;
                case NikoCheckBox cb:
                    cb.AccentColor = _accentColor; cb.ForeColor = _accentColor;
                    cb.HoverColor = _accentColor; cb.PressedColor = _accentColor;
                    cb.Invalidate();
                    break;
                case NikoSliderButton sb:
                    sb.AccentColor = _accentColor; sb.PressedColor = _accentColor;
                    sb.HoverColor = _accentColor; sb.Invalidate();
                    break;
                case NikoVScroll vs:
                    vs.AccentColor = _accentColor; vs.HoverColor = _accentColor;
                    vs.PressedColor = _accentColor; vs.Invalidate();
                    break;
                case NikoHScroll hs:
                    hs.AccentColor = _accentColor; hs.PressedColor = _accentColor;
                    hs.HoverColor = _accentColor; hs.Invalidate();
                    break;
                case NikoLabelLink ll: ll.AccentColor = _accentColor; ll.Invalidate(); break;
                case NikoSlider sl:
                    sl.AccentColor = _accentColor; sl.HoverColor = _accentColor;
                    sl.PressedColor = _accentColor; sl.Invalidate();
                    break;
                case NikoPanel panel: panel.AccentColor = _accentColor; panel.Invalidate(); break;
            }

            if (control.Controls != null && control.Controls.Count > 0)
            {
                foreach (Control child in control.Controls)
                {
                    ProcessControlRecursive(child);
                }
            }
        }

        private void ProcessFormComponents()
        {
            var fields = _targetForm.GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            foreach (var field in fields)
            {
                try
                {
                    var fieldValue = field.GetValue(_targetForm);
                    if (fieldValue == null) continue;

                    if (fieldValue is NikoContextMenuStrip menu)
                    {
                        UpdateContextMenu(menu);
                    }
                    else if (fieldValue is NotifyIcon notifyIcon && notifyIcon.ContextMenuStrip is NikoContextMenuStrip trayMenu)
                    {
                        UpdateContextMenu(trayMenu);
                    }
                }
                catch { }
            }
        }

        private void UpdateContextMenu(NikoContextMenuStrip menu)
        {
            menu.AccentColor = _accentColor;
            menu.BorderColor = _accentColor;
            menu.TextColor = _accentColor;
            menu.Invalidate();
        }

        private void ApplyTemplateColors()
        {
            _rainbowTimer.Stop();

            switch (_currentTheme)
            {
                case NikoTheme.Purple: _accentColor = Color.FromArgb(150, 101, 255); break;
                case NikoTheme.Blue: _accentColor = Color.FromArgb(116, 108, 230); break;
                case NikoTheme.Teal: _accentColor = Color.FromArgb(53, 205, 210); break;
                case NikoTheme.Yellow: _accentColor = Color.FromArgb(255, 211, 106); break;
                case NikoTheme.White: _accentColor = Color.FromArgb(255, 255, 255); break;
                case NikoTheme.Red: _accentColor = Color.FromArgb(213, 33, 106); break;
                case NikoTheme.Pink: _accentColor = Color.FromArgb(255, 85, 191); break;
                case NikoTheme.Orange: _accentColor = Color.FromArgb(252, 143, 76); break;
                case NikoTheme.Green: _accentColor = Color.FromArgb(62, 213, 100); break;
                case NikoTheme.Rainbow:
                    if (DesignMode)
                    {
                        _accentColor = Color.FromArgb(150, 101, 255);
                    }
                    else
                    {
                        _rainbowTimer.Start();
                    }
                    break;
            }
        }

        private Color ColorFromHSV(double hue, double saturation, double value)
        {
            int hi = Convert.ToInt32(Math.Floor(hue / 60)) % 6;
            double f = hue / 60 - Math.Floor(hue / 60);
            value = value * 255;
            int v = Convert.ToInt32(value);
            int p = Convert.ToInt32(value * (1 - saturation));
            int q = Convert.ToInt32(value * (1 - f * saturation));
            int t = Convert.ToInt32(value * (1 - (1 - f) * saturation));

            if (hi == 0) return Color.FromArgb(255, v, t, p);
            if (hi == 1) return Color.FromArgb(255, q, v, p);
            if (hi == 2) return Color.FromArgb(255, p, v, t);
            if (hi == 3) return Color.FromArgb(255, p, q, v);
            if (hi == 4) return Color.FromArgb(255, t, p, v);
            return Color.FromArgb(255, v, p, q);
        }
    }
}