using ProjectEstimatorApp.Models;
using ProjectEstimatorApp.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

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
        }

        private void InitializeControls()
        {
            _worksGrid = new DataGridView
            {
                Dock = DockStyle.Top,
                Height = 200,
                AutoGenerateColumns = true,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                Font = new Font("Segoe UI", 9)
            };

            _materialsGrid = new DataGridView
            {
                Dock = DockStyle.Top,
                Height = 200,
                AutoGenerateColumns = true,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                Font = new Font("Segoe UI", 9)
            };

            _btnAddWork = CreateStyledButton("Add Work");
            _btnAddMaterial = CreateStyledButton("Add Material");
            _btnRemoveWork = CreateStyledButton("Remove Work");
            _btnRemoveMaterial = CreateStyledButton("Remove Material");

            _lblWorksTotal = new Label { Text = "Works Total: 0.00", AutoSize = true };
            _lblMaterialsTotal = new Label { Text = "Materials Total: 0.00", AutoSize = true };
            _lblEstimateTotal = new Label
            {
                Text = "Estimate Total: 0.00",
                AutoSize = true,
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };

            _btnAddWork.Click += (s, e) => AddWorkItem();
            _btnAddMaterial.Click += (s, e) => AddMaterialItem();
            _btnRemoveWork.Click += (s, e) => RemoveWorkItem();
            _btnRemoveMaterial.Click += (s, e) => RemoveMaterialItem();
        }
        public void SetCurrentItem(object item)
        {
            _estimateEditor.SetCurrentItem(item);
            UpdateData();
        }

        private void UpdateData()
        {
            if (_estimateEditor == null) return;

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
            _lblWorksTotal.Text = $"Works Total: {_estimateEditor.CalculateWorksTotal():N2}";
            _lblMaterialsTotal.Text = $"Materials Total: {_estimateEditor.CalculateMaterialsTotal():N2}";
            _lblEstimateTotal.Text = $"Estimate Total: {_estimateEditor.CalculateEstimateTotal():N2}";
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

        private Button CreateStyledButton(string text)
        {
            return new Button
            {
                Text = text,
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.White,
                Font = new Font("Segoe UI", 9),
                Height = 30,
                Margin = new Padding(3)
            };
        }

        private void SetupLayout()
        {
            var buttonsPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                FlowDirection = FlowDirection.LeftToRight,
                BackColor = Color.FromArgb(240, 240, 240),
                Height = 40,
                Padding = new Padding(5)
            };

            var totalsPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Bottom,
                FlowDirection = FlowDirection.LeftToRight,
                BackColor = Color.FromArgb(240, 240, 240),
                Height = 40,
                Padding = new Padding(5)
            };

            buttonsPanel.Controls.AddRange(new Control[] { _btnAddWork, _btnRemoveWork, _btnAddMaterial, _btnRemoveMaterial });
            totalsPanel.Controls.AddRange(new Control[] { _lblWorksTotal, _lblMaterialsTotal, _lblEstimateTotal });

            Controls.Add(_materialsGrid);
            Controls.Add(buttonsPanel);
            Controls.Add(_worksGrid);
            Controls.Add(totalsPanel);

            BackColor = Color.White;


            Controls.Add(_materialsGrid);
            Controls.Add(buttonsPanel);
            Controls.Add(_worksGrid);
            Controls.Add(totalsPanel);
        }
    }
}