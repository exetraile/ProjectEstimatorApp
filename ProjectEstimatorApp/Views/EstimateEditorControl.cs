using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using ProjectEstimatorApp.Models;
using ProjectEstimatorApp.Services;
using ProjectEstimatorApp.Styles;

namespace ProjectEstimatorApp.Views
{
    public partial class EstimateEditorControl : UserControl
    {
        private readonly IEstimateEditorService _estimateEditor;
        private DataGridView _worksGrid;
        private DataGridView _materialsGrid;
        private Button _btnAddWork;
        private Button _btnAddMaterial;
        private Button _btnRemoveWork;
        private Button _btnRemoveMaterial;
        private Label _lblWorksTotal;
        private Label _lblMaterialsTotal;
        private Label _lblEstimateTotal;

        public EstimateEditorControl(IEstimateEditorService estimateEditor)
        {
            _estimateEditor = estimateEditor ?? throw new ArgumentNullException(nameof(estimateEditor));
            InitializeControls();
            SetupLayout();
            ApplyStyles();
        }

        private void InitializeControls()
        {
            _worksGrid = new DataGridView { Dock = DockStyle.Fill, AutoGenerateColumns = true };
            StyleHelper.Grids.ApplyDataGridStyle(_worksGrid);

            _materialsGrid = new DataGridView { Dock = DockStyle.Fill, AutoGenerateColumns = true };
            StyleHelper.Grids.ApplyDataGridStyle(_materialsGrid);

            _btnAddWork = StyleHelper.Buttons.Primary("Add Work", 120);
            _btnAddMaterial = StyleHelper.Buttons.Primary("Add Material", 120);
            _btnRemoveWork = StyleHelper.Buttons.Secondary("Remove Work", 120);
            _btnRemoveMaterial = StyleHelper.Buttons.Secondary("Remove Material", 120);

            _lblWorksTotal = StyleHelper.Labels.Body("Works Total: 0.00", bold: true);
            _lblMaterialsTotal = StyleHelper.Labels.Body("Materials Total: 0.00", bold: true);
            _lblEstimateTotal = StyleHelper.Labels.Header("Estimate Total: 0.00");

            _btnAddWork.Click += (s, e) => AddWorkItem();
            _btnAddMaterial.Click += (s, e) => AddMaterialItem();
            _btnRemoveWork.Click += (s, e) => RemoveWorkItem();
            _btnRemoveMaterial.Click += (s, e) => RemoveMaterialItem();
        }

        private void SetupLayout()
        {
            var splitContainer = new SplitContainer
            {
                Dock = DockStyle.Fill,
                Orientation = Orientation.Horizontal,
                SplitterDistance = 280
            };

            var worksPanel = new Panel { Dock = DockStyle.Fill };
            var worksHeader = new Panel
            {
                Dock = DockStyle.Top,
                Height = 35,
                BackColor = StyleHelper.Config.AccentColor
            };
            var worksLabel = new Label
            {
                Text = "WORKS",
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                ForeColor = Color.White,
                Font = new Font(StyleHelper.Config.NormalFont, FontStyle.Bold)
            };
            worksHeader.Controls.Add(worksLabel);
            worksPanel.Controls.Add(_worksGrid);
            worksPanel.Controls.Add(worksHeader);

            var materialsPanel = new Panel { Dock = DockStyle.Fill };
            var materialsHeader = new Panel
            {
                Dock = DockStyle.Top,
                Height = 35,
                BackColor = StyleHelper.Config.AccentColor
            };
            var materialsLabel = new Label
            {
                Text = "MATERIALS",
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                ForeColor = Color.White,
                Font = new Font(StyleHelper.Config.NormalFont, FontStyle.Bold)
            };
            materialsHeader.Controls.Add(materialsLabel);
            materialsPanel.Controls.Add(_materialsGrid);
            materialsPanel.Controls.Add(materialsHeader);

            splitContainer.Panel1.Controls.Add(worksPanel);
            splitContainer.Panel2.Controls.Add(materialsPanel);

            var buttonsPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                FlowDirection = FlowDirection.LeftToRight,
                BackColor = StyleHelper.Config.ElementBackgroundColor,
                Height = 50,
                Padding = new Padding(10),
                AutoSize = true,
            };
            buttonsPanel.Controls.AddRange(new Control[] { _btnAddWork, _btnRemoveWork, _btnAddMaterial, _btnRemoveMaterial });

            var totalsPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Bottom,
                FlowDirection = FlowDirection.RightToLeft,
                BackColor = StyleHelper.Config.ElementBackgroundColor,
                Height = 50,
                Padding = new Padding(10, 0, 20, 0),
                AutoSize = true
            };
            totalsPanel.Controls.AddRange(new Control[] { _lblEstimateTotal, _lblMaterialsTotal, _lblWorksTotal });

            Controls.Add(splitContainer);
            Controls.Add(buttonsPanel);
            Controls.Add(totalsPanel);
        }

        private void ApplyStyles()
        {
            BackColor = StyleHelper.Config.BackgroundColor;
        }

        public void SetCurrentItem(object item)
        {
            _estimateEditor.SetCurrentItem(item);
            UpdateData();
        }

        private void UpdateData()
        {
            _worksGrid.DataSource = null;
            _materialsGrid.DataSource = null;

            if (_estimateEditor.GetCurrentWorks() is List<EstimateItem> works)
                _worksGrid.DataSource = new BindingList<EstimateItem>(works);

            if (_estimateEditor.GetCurrentMaterials() is List<EstimateItem> materials)
                _materialsGrid.DataSource = new BindingList<EstimateItem>(materials);

            UpdateTotals();
        }

        private void UpdateTotals()
        {
            decimal worksTotal = _estimateEditor.CalculateWorksTotal();
            decimal materialsTotal = _estimateEditor.CalculateMaterialsTotal();
            decimal estimateTotal = _estimateEditor.CalculateEstimateTotal();

            _lblWorksTotal.Text = $"Works Total: {worksTotal:N2}";
            _lblMaterialsTotal.Text = $"Materials Total: {materialsTotal:N2}";
            _lblEstimateTotal.Text = $"Estimate Total: {estimateTotal:N2}";
        }

        private void AddWorkItem()
        {
            var item = new EstimateItem { Name = "New Work", Unit = "pcs", Quantity = 1, PricePerUnit = 0 };
            _estimateEditor.AddWorkItem(item);
            UpdateData();
        }

        private void AddMaterialItem()
        {
            var item = new EstimateItem { Name = "New Material", Unit = "pcs", Quantity = 1, PricePerUnit = 0 };
            _estimateEditor.AddMaterialItem(item);
            UpdateData();
        }

        private void RemoveWorkItem()
        {
            if (_worksGrid.CurrentRow?.Index >= 0)
            {
                _estimateEditor.RemoveWorkItem(_worksGrid.CurrentRow.Index);
                UpdateData();
            }
        }

        private void RemoveMaterialItem()
        {
            if (_materialsGrid.CurrentRow?.Index >= 0)
            {
                _estimateEditor.RemoveMaterialItem(_materialsGrid.CurrentRow.Index);
                UpdateData();
            }
        }
    }
}