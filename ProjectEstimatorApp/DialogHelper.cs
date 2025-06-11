// DialogHelper.cs
using System.Drawing;
using System.Windows.Forms;
using ProjectEstimatorApp.Models;
using ProjectEstimatorApp.Styles;

namespace ProjectEstimatorApp.Helper
{
    public static class DialogHelper
    {
        public static string ShowInputDialog(string title, string prompt, Form owner = null)
        {
            using (var form = new Form())
            {
                form.Text = title;
                form.Width = 300;
                form.Height = 150;
                form.FormBorderStyle = FormBorderStyle.FixedDialog;
                form.StartPosition = FormStartPosition.CenterParent;

                var label = new Label { Text = prompt, Left = 10, Top = 20, Width = 280 };
                var textBox = new TextBox { Left = 10, Top = 50, Width = 280 };
                var btnOk = new Button { Text = "OK", DialogResult = DialogResult.OK, Left = 110, Top = 80, Width = 75 };

                form.Controls.Add(label);
                form.Controls.Add(textBox);
                form.Controls.Add(btnOk);
                form.AcceptButton = btnOk;

                return form.ShowDialog(owner) == DialogResult.OK ? textBox.Text : string.Empty;
            }
        }

        public static (string EstimateDetailName, double Width, double Height) ShowAddEstimateDetailDialog(Form owner)
        {
            using (var dialog = new Form())
            {
                dialog.Text = "Add EstimateDetail";
                dialog.Width = 300;
                dialog.Height = 200;
                dialog.FormBorderStyle = FormBorderStyle.FixedDialog;
                dialog.StartPosition = FormStartPosition.CenterParent;

                var lblName = new Label { Text = "EstimateDetail name:", Top = 20, Left = 20, Width = 100 };
                var txtName = new TextBox { Top = 20, Left = 120, Width = 150 };

                var lblWidth = new Label { Text = "Width:", Top = 50, Left = 20, Width = 100 };
                var txtWidth = new NumericUpDown { Top = 50, Left = 120, Width = 150, Minimum = 0.1m, Maximum = 100, DecimalPlaces = 2 };

                var lblHeight = new Label { Text = "Height:", Top = 80, Left = 20, Width = 100 };
                var txtHeight = new NumericUpDown { Top = 80, Left = 120, Width = 150, Minimum = 0.1m, Maximum = 100, DecimalPlaces = 2 };

                var btnOk = new Button { Text = "OK", DialogResult = DialogResult.OK, Top = 120, Left = 120, Width = 75 };
                var btnCancel = new Button { Text = "Cancel", DialogResult = DialogResult.Cancel, Top = 120, Left = 200, Width = 75 };

                dialog.Controls.AddRange(new Control[] { lblName, txtName, lblWidth, txtWidth, lblHeight, txtHeight, btnOk, btnCancel });
                dialog.AcceptButton = btnOk;
                dialog.CancelButton = btnCancel;

                if (dialog.ShowDialog(owner) == DialogResult.OK)
                {
                    return (txtName.Text, (double)txtWidth.Value, (double)txtHeight.Value);
                }

                return (null, 0, 0);
            }
        }

        public static void ShowTotalsDialog(ProjectSummary summary, Form owner)
        {
            using (var totalsForm = new Form())
            {
                totalsForm.Text = "Project Totals";
                totalsForm.Width = 600;
                totalsForm.Height = 400;
                totalsForm.StartPosition = FormStartPosition.CenterParent;

                var grid = new DataGridView
                {
                    Dock = DockStyle.Fill,
                    DataSource = summary.EstimateSummaries
                };
                StyleHelper.Grids.ApplyDataGridStyle(grid);

                var lblTotal = new Label
                {
                    Text = $"Overall Total: {summary.OverallTotal:N2}",
                    Dock = DockStyle.Bottom,
                    TextAlign = ContentAlignment.MiddleRight,
                    Font = new Font(StyleHelper.Config.NormalFont, FontStyle.Bold),
                    Height = 40
                };

                totalsForm.Controls.Add(grid);
                totalsForm.Controls.Add(lblTotal);
                totalsForm.ShowDialog(owner);
            }
        }
    }
}

