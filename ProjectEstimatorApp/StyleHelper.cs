using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace ProjectEstimatorApp.Styles
{
    public static class StyleHelper
    {
        public static class Config
        {
            // Цветовая схема
            public static Color BackgroundColor = Color.FromArgb(18, 18, 18); 
            public static Color ElementBackgroundColor = Color.FromArgb(30, 30, 30); 
            public static Color AccentColor = Color.FromArgb(138, 43, 226); 
            public static Color TextColor = Color.FromArgb(255, 255, 255); 
            public static Color SecondaryTextColor = Color.FromArgb(176, 176, 176); 
            public static Color SuccessColor = Color.FromArgb(76, 175, 80); 
            public static Color ErrorColor = Color.FromArgb(244, 67, 54); 
            public static Color WarningColor = Color.FromArgb(255, 165, 0); 
            public static Color BorderColor = Color.FromArgb(45, 45, 45); 

            public static Font HeaderFont = new Font("Segoe UI", 16, FontStyle.Bold);
            public static Font NormalFont = new Font("Segoe UI", 12);
            public static Font SmallFont = new Font("Segoe UI", 11);

            public static int CornerRadius = 8; 
            public static Padding DefaultPadding = new Padding(10);

            public static Color PlaceholderForeColor = Color.FromArgb(176, 176, 176); 
        }

        public static class Forms
        {
            public static void ApplyMainFormStyle(Form form)
            {
                form.BackColor = Config.BackgroundColor;
                form.Font = Config.NormalFont;
                form.ForeColor = Config.TextColor;
                form.StartPosition = FormStartPosition.CenterScreen;
                form.MinimumSize = new Size(800, 600);
                form.Padding = Config.DefaultPadding;
            }

            public static void ApplyDialogStyle(Form form)
            {
                form.FormBorderStyle = FormBorderStyle.FixedDialog;
                form.BackColor = Config.BackgroundColor;
                form.Font = Config.NormalFont;
                form.ForeColor = Config.TextColor;
                form.StartPosition = FormStartPosition.CenterParent;
                form.Padding = Config.DefaultPadding;
                form.MinimizeBox = false;
                form.MaximizeBox = false;
                form.MinimumSize = new Size(400, 300);
            }
        }

        public static class Buttons
        {
            public static Button Primary(string text, int? width = null)
            {
                var btn = new Button
                {
                    Text = text,
                    FlatStyle = FlatStyle.Flat,
                    BackColor = Config.AccentColor,
                    ForeColor = Color.White,
                    Font = new Font(Config.NormalFont.FontFamily, Config.NormalFont.Size, FontStyle.Bold),
                    Height = 40,
                    MinimumSize = new Size(width ?? 140, 40),
                    Margin = new Padding(8),
                    Cursor = Cursors.Hand,
                };

                btn.FlatAppearance.BorderSize = 0;

                btn.MouseEnter += (s, e) =>
                {
                    btn.BackColor = ControlPaint.Light(Config.AccentColor, 0.1f);
                };
                btn.MouseLeave += (s, e) =>
                {
                    btn.BackColor = Config.AccentColor;
                };
                btn.MouseDown += (s, e) =>
                {
                    btn.BackColor = ControlPaint.Dark(Config.AccentColor, 0.1f);
                };
                btn.MouseUp += (s, e) =>
                {
                    btn.BackColor = Config.AccentColor;
                };

                return btn;
            }

            public static Button Secondary(string text, int? width = null)
            {
                var btn = new Button
                {
                    Text = text,
                    FlatStyle = FlatStyle.Flat,
                    BackColor = Color.Transparent,
                    ForeColor = Config.AccentColor,
                    Font = Config.NormalFont,
                    Height = 40,
                    MinimumSize = new Size(width ?? 140, 40),
                    Margin = new Padding(8),
                    Cursor = Cursors.Hand,
                };

                btn.FlatAppearance.BorderColor = Config.AccentColor;
                btn.FlatAppearance.BorderSize = 1;

                btn.MouseEnter += (s, e) =>
                {
                    btn.BackColor = Color.FromArgb(40, Config.AccentColor.R, Config.AccentColor.G, Config.AccentColor.B); 
                };
                btn.MouseLeave += (s, e) =>
                {
                    btn.BackColor = Color.Transparent;
                };
                btn.MouseDown += (s, e) =>
                {
                    btn.BackColor = Color.FromArgb(30, Config.AccentColor.R, Config.AccentColor.G, Config.AccentColor.B); 
                };
                btn.MouseUp += (s, e) =>
                {
                    btn.BackColor = Color.Transparent;
                };

                return btn;
            }
        }

        public static class Panels
        {
            public static Panel CardPanel()
            {
                var panel = new Panel
                {
                    BackColor = Config.ElementBackgroundColor,
                    Padding = new Padding(16),
                    Margin = new Padding(0, 0, 0, 16),
                };

                panel.Paint += (s, e) =>
                {
                    var rect = panel.ClientRectangle;
                    rect.Width -= 1;
                    rect.Height -= 1;

                    using (var path = CreateRoundedRectanglePath(rect, Config.CornerRadius))
                    {
                        using (var brush = new SolidBrush(Config.ElementBackgroundColor))
                        {
                            e.Graphics.FillPath(brush, path);
                        }

                        using (var pen = new Pen(Config.BorderColor, 1))
                        {
                            e.Graphics.DrawPath(pen, path);
                        }
                    }
                };

                return panel;
            }

            private static GraphicsPath CreateRoundedRectanglePath(Rectangle rect, int radius)
            {
                int diameter = radius * 2;
                GraphicsPath path = new GraphicsPath();

                path.AddArc(rect.X, rect.Y, diameter, diameter, 180, 90);
                path.AddArc(rect.Right - diameter, rect.Y, diameter, diameter, 270, 90);
                path.AddArc(rect.Right - diameter, rect.Bottom - diameter, diameter, diameter, 0, 90);
                path.AddArc(rect.X, rect.Bottom - diameter, diameter, diameter, 90, 90);
                path.CloseFigure();

                return path;
            }
        }

        public static class Labels
        {
            public static Label Header(string text)
            {
                return new Label
                {
                    Text = text,
                    Font = Config.HeaderFont,
                    ForeColor = Config.TextColor,
                    AutoSize = true,
                    Dock = DockStyle.None,
                    Margin = new Padding(0, 0, 0, 8)
                };
            }

            public static Label Body(string text, bool bold = false)
            {
                return new Label
                {
                    Text = text,
                    Font = bold ? new Font(Config.NormalFont, FontStyle.Bold) : Config.NormalFont,
                    ForeColor = Config.SecondaryTextColor,
                    AutoSize = true,
                    Dock = DockStyle.None,
                    Margin = new Padding(0, 0, 0, 4)
                };
            }
        }

        public static class Inputs
        {
            public static TextBox TextBox(string placeholder = "", int height = 40)
            {
                var txt = new TextBox
                {
                    BorderStyle = BorderStyle.FixedSingle,
                    BackColor = Config.ElementBackgroundColor,
                    ForeColor = Config.TextColor,
                    Font = Config.NormalFont,
                    Height = height,
                    Margin = new Padding(0, 0, 0, 8),
                };

                if (!string.IsNullOrEmpty(placeholder))
                {
                    txt.Text = placeholder;
                    txt.ForeColor = Config.PlaceholderForeColor;
                    txt.GotFocus += (s, e) =>
                    {
                        if (txt.Text == placeholder)
                        {
                            txt.Text = "";
                            txt.ForeColor = Config.TextColor;
                        }
                    };
                    txt.LostFocus += (s, e) =>
                    {
                        if (string.IsNullOrWhiteSpace(txt.Text))
                        {
                            txt.Text = placeholder;
                            txt.ForeColor = Config.PlaceholderForeColor;
                        }
                    };
                }
                return txt;
            }

            public static NumericUpDown NumericBox(decimal value = 0, int decimalPlaces = 2)
            {
                return new NumericUpDown
                {
                    BorderStyle = BorderStyle.FixedSingle,
                    BackColor = Config.ElementBackgroundColor,
                    ForeColor = Config.TextColor,
                    Font = Config.NormalFont,
                    Height = 40,
                    Margin = new Padding(0, 0, 0, 8),
                    DecimalPlaces = decimalPlaces,
                    Value = value,
                    Minimum = 0,
                    Maximum = 9999999,
                    ThousandsSeparator = true
                };
            }

            public static ComboBox ComboBox()
            {
                var combo = new ComboBox
                {
                    FlatStyle = FlatStyle.Flat,
                    BackColor = Config.ElementBackgroundColor,
                    ForeColor = Config.TextColor,
                    Font = Config.NormalFont,
                    Margin = new Padding(0, 0, 0, 8),
                    Height = 40,
                    DropDownStyle = ComboBoxStyle.DropDownList,
                    DrawMode = DrawMode.OwnerDrawFixed
                };
                combo.DrawItem += (sender, e) =>
                {
                    e.DrawBackground();
                    if (e.Index >= 0)
                    {
                        using (var brush = new SolidBrush(e.ForeColor))
                        {
                            e.Graphics.DrawString(combo.Items[e.Index].ToString(), e.Font, brush, e.Bounds);
                        }
                    }
                };
                return combo;
            }
        }

        public static class Grids
        {
            public static void ApplyDataGridStyle(DataGridView grid)
            {
                grid.BackgroundColor = Config.BackgroundColor;
                grid.BorderStyle = BorderStyle.None;
                grid.EnableHeadersVisualStyles = false;
                grid.AllowUserToResizeRows = false;

                grid.DefaultCellStyle.BackColor = Config.ElementBackgroundColor;
                grid.DefaultCellStyle.ForeColor = Config.TextColor;
                grid.DefaultCellStyle.SelectionBackColor = Color.FromArgb(58, 43, 226);
                grid.DefaultCellStyle.SelectionForeColor = Config.TextColor;
                grid.DefaultCellStyle.Font = Config.NormalFont;
                grid.DefaultCellStyle.Padding = new Padding(8);
                grid.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

                grid.ColumnHeadersDefaultCellStyle.BackColor = Config.ElementBackgroundColor;
                grid.ColumnHeadersDefaultCellStyle.ForeColor = Config.TextColor;
                grid.ColumnHeadersDefaultCellStyle.Font = new Font(Config.NormalFont.FontFamily, Config.NormalFont.Size, FontStyle.Bold);
                grid.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
                grid.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
                grid.ColumnHeadersHeight = 40;

                grid.RowHeadersVisible = false;
                grid.RowTemplate.Height = 40;
                grid.RowTemplate.Resizable = DataGridViewTriState.False;

                grid.GridColor = Config.BorderColor;
                grid.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
                grid.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(45, 45, 45);

                grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                grid.ScrollBars = ScrollBars.Vertical;
            }
        }
    }
}

