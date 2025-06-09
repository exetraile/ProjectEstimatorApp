using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using ProjectEstimatorApp.Models;
using ProjectEstimatorApp.Services;
using ProjectEstimatorApp.Styles;

namespace ProjectEstimatorApp.Views
{
    public partial class MainForm : Form
    {
        private readonly IProjectManagerService _projectManager;
        private readonly IProjectStructureService _structureService;
        private readonly IEstimateEditorService _estimateEditor;
        private readonly ICalculationService _calculationService;

        private TreeView _projectTree;
        private Button _btnNewProject;
        private Button _btnSaveProject;
        private Button _btnLoadProject;
        private Button _btnAddFloor;
        private Button _btnAddRoom;
        private Button _btnAddEstimate;
        private Button _btnShowTotals;
        private EstimateEditorControl _estimateEditorControl;

        public MainForm(
            IProjectManagerService projectManager,
            IProjectStructureService structureService,
            IEstimateEditorService estimateEditor,
            ICalculationService calculationService)
        {
            _projectManager = projectManager;
            _structureService = structureService;
            _estimateEditor = estimateEditor;
            _calculationService = calculationService;

            InitializeComponent();
            StyleHelper.Forms.ApplyMainFormStyle(this);
            InitializeControls();
            InitializeProjectTree();
        }

        private void InitializeControls()
        {
            // Main panel layout
            var mainPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 2
            };
            mainPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 70));
            mainPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

            // Buttons panel
            var buttonsPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                BackColor = StyleHelper.Config.ElementBackgroundColor,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                Padding = new Padding(10, 12, 10, 12),
                Height = 70
            };

            // Initialize buttons
            _btnNewProject = StyleHelper.Buttons.Primary("New Project", 120);
            _btnSaveProject = StyleHelper.Buttons.Primary("Save Project", 120);
            _btnLoadProject = StyleHelper.Buttons.Primary("Load Project", 120);
            _btnAddFloor = StyleHelper.Buttons.Primary("Add Floor", 120);
            _btnAddRoom = StyleHelper.Buttons.Primary("Add Room", 120);
            _btnAddEstimate = StyleHelper.Buttons.Primary("Add Estimate", 120);
            _btnShowTotals = StyleHelper.Buttons.Primary("Show Totals", 120);

            buttonsPanel.Controls.AddRange(new Control[] {
                _btnNewProject, _btnSaveProject, _btnLoadProject,
                _btnAddFloor, _btnAddRoom, _btnAddEstimate, _btnShowTotals
            });

            // TreeView setup
            _projectTree = new TreeView
            {
                Dock = DockStyle.Left,
                Width = 300,
                BackColor = StyleHelper.Config.ElementBackgroundColor,
                BorderStyle = BorderStyle.None,
                Font = StyleHelper.Config.NormalFont,
                ForeColor = StyleHelper.Config.TextColor,
                ShowLines = true,
                ShowPlusMinus = true,
                FullRowSelect = true,
                HideSelection = false
            };

            // Estimate Editor
            _estimateEditorControl = new EstimateEditorControl(_estimateEditor)
            {
                Dock = DockStyle.Fill,
                BackColor = StyleHelper.Config.BackgroundColor
            };

            // Content panel
            var contentPanel = new Panel { Dock = DockStyle.Fill };
            contentPanel.Controls.Add(_estimateEditorControl);
            contentPanel.Controls.Add(_projectTree);

            // Assemble main form
            mainPanel.Controls.Add(buttonsPanel, 0, 0);
            mainPanel.Controls.Add(contentPanel, 0, 1);
            Controls.Add(mainPanel);

            // Event handlers
            _btnNewProject.Click += (s, e) => CreateNewProject();
            _btnSaveProject.Click += (s, e) => SaveProject();
            _btnLoadProject.Click += (s, e) => LoadProject();
            _btnAddFloor.Click += (s, e) => AddFloor();
            _btnAddRoom.Click += (s, e) => AddRoom();
            _btnAddEstimate.Click += (s, e) => AddEstimate();
            _btnShowTotals.Click += (s, e) => ShowTotals();

            _projectTree.AfterSelect += (s, e) =>
            {
                if (e.Node?.Tag != null)
                    _estimateEditorControl.SetCurrentItem(e.Node.Tag);
            };

            _projectTree.BeforeExpand += (s, e) =>
            {
                if (e.Node?.Tag is Floor floor &&
                    e.Node.Nodes.Count == 1 &&
                    e.Node.Nodes[0].Text == "loading...")
                {
                    e.Node.Nodes.Clear();
                    e.Node.Text = $"▼ {floor.Name}";

                    if (floor.Rooms != null)
                    {
                        foreach (var room in floor.Rooms)
                        {
                            var roomNode = new TreeNode($"  {room.Name} ({room.Width}x{room.Height})")
                            {
                                Tag = room,
                                NodeFont = StyleHelper.Config.NormalFont,
                                ForeColor = StyleHelper.Config.TextColor
                            };
                            e.Node.Nodes.Add(roomNode);
                        }
                    }
                }
            };

            _projectTree.BeforeCollapse += (s, e) =>
            {
                if (e.Node?.Tag is Floor floor)
                {
                    e.Node.Text = $"► {floor.Name}";
                }
            };
        }

        private void InitializeProjectTree()
        {
            _projectTree.BeginUpdate();
            try
            {
                _projectTree.Nodes.Clear();

                if (_projectManager.CurrentProject == null)
                    return;

                // Project node
                var projectNode = new TreeNode($"▼ {_projectManager.CurrentProject.Name}")
                {
                    Tag = _projectManager.CurrentProject,
                    NodeFont = new Font(StyleHelper.Config.NormalFont, FontStyle.Bold),
                    ForeColor = StyleHelper.Config.TextColor
                };

                // Floors
                if (_projectManager.CurrentProject.Floors != null)
                {
                    foreach (var floor in _projectManager.CurrentProject.Floors)
                    {
                        var hasRooms = floor.Rooms != null && floor.Rooms.Count > 0;
                        var floorNode = new TreeNode($"► {floor.Name}")
                        {
                            Tag = floor,
                            NodeFont = new Font(StyleHelper.Config.NormalFont,
                                hasRooms ? FontStyle.Bold : FontStyle.Regular),
                            ForeColor = hasRooms ?
                                StyleHelper.Config.AccentColor :
                                StyleHelper.Config.SecondaryTextColor
                        };

                        // Add dummy node if has rooms
                        if (hasRooms)
                        {
                            floorNode.Nodes.Add("loading...");
                        }

                        projectNode.Nodes.Add(floorNode);
                    }
                }

                _projectTree.Nodes.Add(projectNode);
                projectNode.Expand();
            }
            finally
            {
                _projectTree.EndUpdate();
            }
        }

        private void CreateNewProject()
        {
            string projectName = ShowInputDialog("New Project", "Enter project name:");
            if (!string.IsNullOrEmpty(projectName))
            {
                _projectManager.CreateNewProject(projectName);
                InitializeProjectTree();
            }
        }

        private void SaveProject()
        {
            if (!_projectManager.ProjectExists()) return;

            using (var saveDialog = new SaveFileDialog())
            {
                saveDialog.Filter = "Project Files (*.json)|*.json";
                if (saveDialog.ShowDialog(this) == DialogResult.OK)
                {
                    _projectManager.SaveProject(saveDialog.FileName);
                }
            }
        }

        private void LoadProject()
        {
            using (var openDialog = new OpenFileDialog())
            {
                openDialog.Filter = "Project Files (*.json)|*.json";
                if (openDialog.ShowDialog(this) == DialogResult.OK)
                {
                    _projectManager.LoadProject(openDialog.FileName);
                    InitializeProjectTree();
                }
            }
        }

        private void AddFloor()
        {
            if (!_projectManager.ProjectExists()) return;

            string floorName = ShowInputDialog("Add Floor", "Enter floor name:");
            if (!string.IsNullOrEmpty(floorName))
            {
                _structureService.AddFloor(floorName);
                InitializeProjectTree();
            }
        }

        private void AddRoom()
        {
            if (!_projectManager.ProjectExists()) return;

            using (var dialog = new Form())
            {
                dialog.Text = "Add Room";
                dialog.Width = 300;
                dialog.Height = 200;
                dialog.FormBorderStyle = FormBorderStyle.FixedDialog;
                dialog.StartPosition = FormStartPosition.CenterParent;

                var lblName = new Label { Text = "Room name:", Top = 20, Left = 20, Width = 100 };
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

                if (dialog.ShowDialog(this) == DialogResult.OK)
                {
                    var floorName = _projectTree.SelectedNode?.Tag is Floor floor
                        ? floor.Name
                        : _structureService.GetFloorNames().FirstOrDefault();

                    if (floorName != null)
                    {
                        _structureService.AddRoom(
                            floorName,
                            txtName.Text,
                            (double)txtWidth.Value,
                            (double)txtHeight.Value);
                        InitializeProjectTree();
                    }
                }
            }
        }

        private void AddEstimate()
        {
            if (!_projectManager.ProjectExists()) return;

            string category = ShowInputDialog("Add Estimate", "Enter estimate category:");
            if (!string.IsNullOrEmpty(category))
            {
                var selected = _projectTree.SelectedNode?.Tag;

                if (selected is Project project)
                {
                    _structureService.AddEstimateToProject(category);
                }
                else if (selected is Floor floor)
                {
                    _structureService.AddEstimateToFloor(floor.Name, category);
                }
                else if (selected is Room room)
                {
                    var floorNode = _projectTree.SelectedNode?.Parent;
                    if (floorNode?.Tag is Floor parentFloor)
                    {
                        _structureService.AddEstimate(parentFloor.Name, room.Name, category);
                    }
                }
                else
                {
                    MessageBox.Show(this,
                        "Please select a project, floor or room first",
                        "Selection Required",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                }
                InitializeProjectTree();
            }
        }

        private void ShowTotals()
        {
            if (!_projectManager.ProjectExists()) return;

            var summary = _calculationService.CalculateProjectSummary(_projectManager.CurrentProject);

            using (var totalsForm = new Form())
            {
                totalsForm.Text = "Project Totals";
                totalsForm.Width = 600;
                totalsForm.Height = 400;
                totalsForm.StartPosition = FormStartPosition.CenterParent;

                var grid = new DataGridView
                {
                    Dock = DockStyle.Fill,
                    DataSource = summary.FloorSummaries
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
                totalsForm.ShowDialog(this);
            }
        }

        private string ShowInputDialog(string title, string prompt)
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

                return form.ShowDialog(this) == DialogResult.OK ? textBox.Text : string.Empty;
            }
        }
    }
}