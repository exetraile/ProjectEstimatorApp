using System;
using System.Drawing;
using System.Windows.Forms;
using System.Linq;
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
            Text = "Детализация проекта";
            ClientSize = new Size(1000, 700);
            StartPosition = FormStartPosition.CenterParent;

            var mainPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(10),
                BackColor = StyleHelper.Config.BackgroundColor
            };

            var treeView = new TreeView
            {
                Dock = DockStyle.Left,
                Width = 300,
                BackColor = StyleHelper.Config.ElementBackgroundColor,
                ForeColor = StyleHelper.Config.TextColor,
                Font = StyleHelper.Config.NormalFont,
                BorderStyle = BorderStyle.None
            };

            var detailsPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = StyleHelper.Config.BackgroundColor,
                Padding = new Padding(10)
            };

            var totalsPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 150,
                BackColor = StyleHelper.Config.ElementBackgroundColor,
                Padding = new Padding(20)
            };

            PopulateTreeView(treeView, summary);
            treeView.AfterSelect += (s, e) => ShowDetails(e.Node?.Tag, detailsPanel);

            var splitContainer = new SplitContainer
            {
                Dock = DockStyle.Fill,
                Orientation = Orientation.Horizontal,
                SplitterDistance = 520,
                BackColor = StyleHelper.Config.BackgroundColor
            };

            splitContainer.Panel1.Controls.Add(treeView);
            splitContainer.Panel1.Controls.Add(detailsPanel);
            splitContainer.Panel2.Controls.Add(totalsPanel);

            // Totals information
            var lblProjectWorks = StyleHelper.Labels.Body($"Работы по проекту: {summary.ProjectWorksTotal:N2} руб.", bold: true);
            lblProjectWorks.ForeColor = StyleHelper.Config.TextColor;
            lblProjectWorks.Location = new Point(20, 20);

            var lblProjectMaterials = StyleHelper.Labels.Body($"Материалы по проекту: {summary.ProjectMaterialsTotal:N2} руб.", bold: true);
            lblProjectMaterials.ForeColor = StyleHelper.Config.TextColor;
            lblProjectMaterials.Location = new Point(20, 45);

            var lblEstimatesWorks = StyleHelper.Labels.Body($"Работы по сметам: {summary.EstimatesWorksTotal:N2} руб.", bold: true);
            lblEstimatesWorks.ForeColor = StyleHelper.Config.TextColor;
            lblEstimatesWorks.Location = new Point(20, 70);

            var lblEstimatesMaterials = StyleHelper.Labels.Body($"Материалы по сметам: {summary.EstimatesMaterialsTotal:N2} руб.", bold: true);
            lblEstimatesMaterials.ForeColor = StyleHelper.Config.TextColor;
            lblEstimatesMaterials.Location = new Point(20, 95);

            var lblOverall = StyleHelper.Labels.Header($"ИТОГО: {summary.OverallTotal:N2} руб.");
            lblOverall.ForeColor = StyleHelper.Config.AccentColor;
            lblOverall.Location = new Point(20, 120);

            totalsPanel.Controls.Add(lblProjectWorks);
            totalsPanel.Controls.Add(lblProjectMaterials);
            totalsPanel.Controls.Add(lblEstimatesWorks);
            totalsPanel.Controls.Add(lblEstimatesMaterials);
            totalsPanel.Controls.Add(lblOverall);

            mainPanel.Controls.Add(splitContainer);
            Controls.Add(mainPanel);
        }

        private void PopulateTreeView(TreeView treeView, ProjectSummary summary)
        {
            treeView.Nodes.Clear();

            var projectNode = new TreeNode(summary.ProjectName)
            {
                Tag = summary,
                NodeFont = new Font(StyleHelper.Config.NormalFont, FontStyle.Bold)
            };

            // Project estimates
            if (summary.ProjectEstimates.Any())
            {
                var projectEstimatesNode = new TreeNode("Сметы проекта")
                {
                    NodeFont = new Font(StyleHelper.Config.NormalFont, FontStyle.Bold)
                };

                foreach (var estimate in summary.ProjectEstimates)
                {
                    projectEstimatesNode.Nodes.Add(CreateEstimateNode(estimate));
                }
                projectNode.Nodes.Add(projectEstimatesNode);
            }

            // Estimates
            foreach (var estimate in summary.EstimateSummaries)
            {
                projectNode.Nodes.Add(CreateEstimateNode(estimate));
            }

            treeView.Nodes.Add(projectNode);
            projectNode.ExpandAll();
        }

        private TreeNode CreateEstimateNode(EstimateSummary estimate)
        {
            var estimateNode = new TreeNode($"{estimate.EstimateName} ({estimate.Total:N2} руб.)")
            {
                Tag = estimate,
                NodeFont = new Font(StyleHelper.Config.NormalFont, FontStyle.Bold)
            };

            // Estimate details
            foreach (var detail in estimate.EstimateDetailSummaries)
            {
                var detailNode = new TreeNode($"{detail.EstimateDetailName} ({detail.Total:N2} руб.)")
                {
                    Tag = detail
                };
                estimateNode.Nodes.Add(detailNode);
            }

            // Nested estimates
            foreach (var nestedEstimate in estimate.EstimateEstimates)
            {
                estimateNode.Nodes.Add(CreateEstimateNode(nestedEstimate));
            }

            return estimateNode;
        }

        private void ShowDetails(object item, Panel detailsPanel)
        {
            detailsPanel.Controls.Clear();

            if (item == null) return;

            var grid = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoGenerateColumns = false,
                BackgroundColor = StyleHelper.Config.BackgroundColor,
                BorderStyle = BorderStyle.None
            };
            StyleHelper.Grids.ApplyDataGridStyle(grid);

            if (item is ProjectSummary project)
            {
                ShowProjectSummary(project, grid);
            }
            else if (item is EstimateSummary estimate)
            {
                ShowEstimateSummary(estimate, grid);
            }
            else if (item is EstimateDetailSummary detail)
            {
                ShowEstimateDetailSummary(detail, grid);
            }

            detailsPanel.Controls.Add(grid);
        }

        private void ShowProjectSummary(ProjectSummary project, DataGridView grid)
        {
            grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Тип", DataPropertyName = "Type", Width = 200 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Стоимость", DataPropertyName = "Value", Width = 150, DefaultCellStyle = new DataGridViewCellStyle { Format = "N2" } });

            grid.DataSource = new[]
            {
                new { Type = "Работы по проекту", Value = project.ProjectWorksTotal },
                new { Type = "Материалы по проекту", Value = project.ProjectMaterialsTotal },
                new { Type = "Работы по сметам", Value = project.EstimatesWorksTotal },
                new { Type = "Материалы по сметам", Value = project.EstimatesMaterialsTotal },
                new { Type = "Общая стоимость", Value = project.OverallTotal }
            };
        }

        private void ShowEstimateSummary(EstimateSummary estimate, DataGridView grid)
        {
            grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Тип", DataPropertyName = "Type", Width = 200 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Стоимость", DataPropertyName = "Value", Width = 150, DefaultCellStyle = new DataGridViewCellStyle { Format = "N2" } });

            grid.DataSource = new[]
            {
                new { Type = "Работы по смете", Value = estimate.EstimateWorksTotal },
                new { Type = "Материалы по смете", Value = estimate.EstimateMaterialsTotal },
                new { Type = "Работы по деталям", Value = estimate.EstimateDetailsWorksTotal },
                new { Type = "Материалы по деталям", Value = estimate.EstimateDetailsMaterialsTotal },
                new { Type = "Общая стоимость", Value = estimate.Total }
            };
        }

        private void ShowEstimateDetailSummary(EstimateDetailSummary detail, DataGridView grid)
        {
            grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Параметр", DataPropertyName = "Parameter", Width = 200 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Значение", DataPropertyName = "Value", Width = 300 });

            grid.DataSource = new[]
            {
                new { Parameter = "Название", Value = detail.EstimateDetailName },
                new { Parameter = "Площадь", Value = detail.Area.ToString("N2") + " м²" },
                new { Parameter = "Стоимость работ", Value = detail.WorksTotal.ToString("N2") + " руб." },
                new { Parameter = "Стоимость материалов", Value = detail.MaterialsTotal.ToString("N2") + " руб." },
                new { Parameter = "Общая стоимость", Value = detail.Total.ToString("N2") + " руб." }
            };
        }
    }
}