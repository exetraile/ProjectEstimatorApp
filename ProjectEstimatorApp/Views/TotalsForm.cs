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
            ApplyStyles();
        }

        private void InitializeControls(ProjectSummary summary)
        {
            Text = "Project Totals";
            Width = 800;
            Height = 600;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            StartPosition = FormStartPosition.CenterParent;

            var grid = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoGenerateColumns = true,
                DataSource = summary.FloorSummaries,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None
            };

            var totalsPanel = new Panel { Dock = DockStyle.Bottom, Height = 100, BackColor = Color.FromArgb(240, 240, 240) };
            var lblWorks = new Label { Text = $"Works Total: {summary.TotalWorks:N2}", Left = 20, Top = 20, AutoSize = true };
            var lblMaterials = new Label { Text = $"Materials Total: {summary.TotalMaterials:N2}", Left = 20, Top = 45, AutoSize = true };
            var lblOverall = new Label
            {
                Text = $"Overall Total: {summary.OverallTotal:N2}",
                Left = 20,
                Top = 70,
                AutoSize = true,
                Font = new Font(DefaultFont, FontStyle.Bold)
            };

            totalsPanel.Controls.AddRange(new Control[] { lblWorks, lblMaterials, lblOverall });
            Controls.Add(grid);
            Controls.Add(totalsPanel);
        }

        private void ApplyStyles()
        {
            BackColor = Color.White;
            Font = new Font("Segoe UI", 9);
        }
    }
}