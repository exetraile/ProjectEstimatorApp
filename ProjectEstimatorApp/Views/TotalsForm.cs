using System.Drawing;
using System.Windows.Forms;
using ProjectEstimatorApp.Models;
using ProjectEstimatorApp.Styles;

namespace ProjectEstimatorApp.Views
{
    public partial class TotalsForm : Form
    {
        public TotalsForm(ProjectSummary summary)
        {
            InitializeComponent();
            StyleHelper.Forms.ApplyDialogStyle(this);
            InitializeControls(summary);
        }

        private void InitializeControls(ProjectSummary summary)
        {
            Text = "Project Totals";
            ClientSize = new Size(800, 600);

            var grid = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoGenerateColumns = true,
            };
            StyleHelper.Grids.ApplyDataGridStyle(grid);
            grid.DataSource = summary.FloorSummaries;

            var totalsPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 120,
                BackColor = StyleHelper.Config.ElementBackgroundColor,
                Padding = new Padding(20)
            };

            var lblWorks = StyleHelper.Labels.Body($"Works Total: {summary.TotalWorks:N2}", bold: true);
            lblWorks.ForeColor = StyleHelper.Config.TextColor;
            lblWorks.Location = new Point(20, 20);

            var lblMaterials = StyleHelper.Labels.Body($"Materials Total: {summary.TotalMaterials:N2}", bold: true);
            lblMaterials.ForeColor = StyleHelper.Config.TextColor;
            lblMaterials.Location = new Point(20, 50);

            var lblOverall = StyleHelper.Labels.Header($"Overall Total: {summary.OverallTotal:N2}");
            lblOverall.ForeColor = StyleHelper.Config.AccentColor;
            lblOverall.Location = new Point(20, 80);

            totalsPanel.Controls.Add(lblWorks);
            totalsPanel.Controls.Add(lblMaterials);
            totalsPanel.Controls.Add(lblOverall);

            Controls.Add(grid);
            Controls.Add(totalsPanel);

            BackColor = StyleHelper.Config.BackgroundColor;
            Font = StyleHelper.Config.NormalFont;
        }
    }
}
