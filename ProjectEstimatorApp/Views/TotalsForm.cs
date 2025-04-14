// Views/TotalsForm.cs
using System;
using System.Drawing;
using System.Windows.Forms;
using ProjectEstimatorApp.Models;

namespace ProjectEstimatorApp.Views
{
    public partial class TotalsForm : Form
    {
        public TotalsForm(ProjectSummary summary)
        {
            InitializeComponent();
            InitializeControls(summary);
        }

        private void InitializeControls(ProjectSummary summary)
        {
            Text = "Project Totals";
            Width = 600;
            Height = 500;

            var grid = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoGenerateColumns = true,
                DataSource = summary.FloorSummaries
            };

            var totalsPanel = new Panel { Dock = DockStyle.Bottom, Height = 80 };
            var lblWorks = new Label { Text = $"Works Total: {summary.TotalWorks:N2}", Left = 20, Top = 10 };
            var lblMaterials = new Label { Text = $"Materials Total: {summary.TotalMaterials:N2}", Left = 20, Top = 30 };
            var lblOverall = new Label
            {
                Text = $"Overall Total: {summary.OverallTotal:N2}",
                Left = 20,
                Top = 50,
                Font = new Font(DefaultFont, FontStyle.Bold)
            };

            totalsPanel.Controls.AddRange(new Control[] { lblWorks, lblMaterials, lblOverall });
            Controls.Add(grid);
            Controls.Add(totalsPanel);
        }
    }
}