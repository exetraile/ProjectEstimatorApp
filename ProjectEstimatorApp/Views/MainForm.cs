// Views/MainForm.cs


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
            var mainPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 2,
            };
            mainPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 70));
            mainPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

            var buttonsPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                BackColor = StyleHelper.Config.ElementBackgroundColor,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                AutoSize = false,
                Padding = new Padding(10, 12, 10, 12),
                Height = 70
            };

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

            _projectTree = new TreeView
            {
                Dock = DockStyle.Left,
                Width = 300,
                BackColor = StyleHelper.Config.ElementBackgroundColor,
                BorderStyle = BorderStyle.None,
                Font = StyleHelper.Config.NormalFont,
                ForeColor = StyleHelper.Config.TextColor,
                ShowLines = false,
                ShowPlusMinus = false,
                ShowRootLines = false,
                FullRowSelect = true,
                HideSelection = false,
                HotTracking = true
            };
            _projectTree.AfterSelect += ProjectTree_AfterSelect;

            _projectTree.DrawMode = TreeViewDrawMode.OwnerDrawAll;
            _projectTree.DrawNode += (s, e) =>
            {
                if (e.Node == null) return;
                bool isSelected = (e.State & TreeNodeStates.Selected) != 0;
                Color fore = isSelected ? Color.White : StyleHelper.Config.TextColor;
                Color back = isSelected ? StyleHelper.Config.AccentColor : StyleHelper.Config.ElementBackgroundColor;

                using (Brush backBrush = new SolidBrush(back))
                    e.Graphics.FillRectangle(backBrush, e.Bounds);

                TextRenderer.DrawText(e.Graphics, e.Node.Text, StyleHelper.Config.NormalFont, e.Bounds, fore, TextFormatFlags.Left | TextFormatFlags.VerticalCenter);
            };

            _estimateEditorControl = new EstimateEditorControl(_estimateEditor)
            {
                Dock = DockStyle.Fill,
                BackColor = StyleHelper.Config.BackgroundColor
            };

            var contentPanel = new Panel { Dock = DockStyle.Fill };
            contentPanel.Controls.Add(_estimateEditorControl);
            contentPanel.Controls.Add(_projectTree);

            mainPanel.Controls.Add(buttonsPanel, 0, 0);
            mainPanel.Controls.Add(contentPanel, 0, 1);
            Controls.Add(mainPanel);

            _btnNewProject.Click += (s, e) => CreateNewProject();
            _btnSaveProject.Click += (s, e) => SaveProject();
            _btnLoadProject.Click += (s, e) => LoadProject();
            _btnAddFloor.Click += (s, e) => AddFloor();
            _btnAddRoom.Click += (s, e) => AddRoom();
            _btnAddEstimate.Click += (s, e) => AddEstimate();
            _btnShowTotals.Click += (s, e) => ShowTotals();
        }

        private void ProjectTree_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node?.Tag != null)
                _estimateEditorControl.SetCurrentItem(e.Node.Tag);
        }

        private void InitializeProjectTree()
        {
            _projectTree.BeginUpdate();
            _projectTree.Nodes.Clear();

            if (_projectManager.CurrentProject == null)
            {
                _projectTree.EndUpdate();
                return;
            }

            var projectNode = new TreeNode(_projectManager.CurrentProject.Name)
            {
                Tag = _projectManager.CurrentProject,
                NodeFont = new Font(StyleHelper.Config.NormalFont, FontStyle.Bold)
            };

            var estimatesNode = projectNode.Nodes.Add("Project Estimates");
            estimatesNode.NodeFont = new Font(StyleHelper.Config.NormalFont, FontStyle.Italic);
            foreach (var estimate in _projectManager.CurrentProject.ProjectEstimates)
                estimatesNode.Nodes.Add(new TreeNode(estimate.Category) { Tag = estimate });

            foreach (var floor in _projectManager.CurrentProject.Floors)
            {
                var floorNode = projectNode.Nodes.Add(floor.Name);
                floorNode.Tag = floor;
                floorNode.NodeFont = new Font(StyleHelper.Config.NormalFont, FontStyle.Regular);
                foreach (var room in floor.Rooms)
                {
                    var roomNode = floorNode.Nodes.Add($"{room.Name} ({room.Width}x{room.Height})");
                    roomNode.Tag = room;
                    var roomEstimates = roomNode.Nodes.Add("Estimates");
                    roomEstimates.NodeFont = new Font(StyleHelper.Config.NormalFont, FontStyle.Italic);
                    foreach (var estimate in room.Estimates)
                        roomEstimates.Nodes.Add(new TreeNode(estimate.Category) { Tag = estimate });
                }
            }

            _projectTree.Nodes.Add(projectNode);
            projectNode.Expand();
            _projectTree.EndUpdate();
        }


        private void CreateNewProject()
        {
            using (var dialog = new InputDialog("New Project", "Enter project name:"))
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    _projectManager.CreateNewProject(dialog.InputText);
                    InitializeProjectTree();
                }
            }
        }

        private void SaveProject()
        {
            if (!_projectManager.ProjectExists()) return;

            using (var saveDialog = new SaveFileDialog())
            {
                saveDialog.Filter = "Project Files (*.json)|*.json";
                if (saveDialog.ShowDialog() == DialogResult.OK)
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
                if (openDialog.ShowDialog() == DialogResult.OK)
                {
                    _projectManager.LoadProject(openDialog.FileName);
                    InitializeProjectTree();
                }
            }
        }

        private void AddFloor()
        {
            if (!_projectManager.ProjectExists()) return;

            using (var dialog = new InputDialog("Add Floor", "Enter floor name:"))
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    _structureService.AddFloor(dialog.InputText);
                    InitializeProjectTree();
                }
            }
        }

        private void AddRoom()
        {
            if (!_projectManager.ProjectExists()) return;

            using (var dialog = new AddRoomForm())
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    var floorName = _projectTree.SelectedNode?.Tag is Floor floor ? floor.Name :
                        _structureService.GetFloorNames().FirstOrDefault();

                    if (floorName != null)
                    {
                        _structureService.AddRoom(floorName, dialog.RoomName, dialog.Width, dialog.Height);
                        InitializeProjectTree();
                    }
                }
            }
        }

        private void AddEstimate()
        {
            if (!_projectManager.ProjectExists()) return;

            using (var dialog = new InputDialog("Add Estimate", "Enter estimate category:"))
            {
                if (dialog.ShowDialog() == DialogResult.OK && !string.IsNullOrWhiteSpace(dialog.InputText))
                {
                    var selected = _projectTree.SelectedNode?.Tag;

                    switch (selected)
                    {
                        case Project project:
                            _structureService.AddEstimateToProject(dialog.InputText);
                            break;
                        case Floor floor:
                            _structureService.AddEstimateToFloor(floor.Name, dialog.InputText);
                            break;
                        case Room room:
                            var floorNode = _projectTree.SelectedNode?.Parent;
                            if (floorNode?.Tag is Floor parentFloor)
                            {
                                _structureService.AddEstimate(parentFloor.Name, room.Name, dialog.InputText);
                            }
                            break;
                        default:
                            MessageBox.Show("Please select a project, floor or room first",
                                "Selection Required",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);
                            break;
                    }
                    InitializeProjectTree();
                }
            }
        }

        private void ShowTotals()
        {
            if (!_projectManager.ProjectExists()) return;

            var summary = _calculationService.CalculateProjectSummary(_projectManager.CurrentProject);
            var totalsForm = new TotalsForm(summary);
            totalsForm.ShowDialog();
        }
    }
    public class InputDialog : Form
    {
        public string InputText => txtInput.Text.Trim();
        private TextBox txtInput;
        public InputDialog(string title, string prompt)
        {
            StyleHelper.Forms.ApplyDialogStyle(this);
            Text = title;
            ClientSize = new Size(320, 140);
            var lblPrompt = new Label
            {
                Text = prompt,
                Font = StyleHelper.Config.NormalFont,
                ForeColor = StyleHelper.Config.TextColor,
                Location = new Point(10, 10),
                Size = new Size(300, 20),
                AutoSize = false
            };
            txtInput = StyleHelper.Inputs.TextBox();
            txtInput.Location = new Point(10, 40);
            txtInput.Width = 300;
            txtInput.Height = 30;
            var btnOk = StyleHelper.Buttons.Primary("OK", 100);
            btnOk.DialogResult = DialogResult.OK;
            btnOk.Location = new Point(110, 85);
            var btnCancel = StyleHelper.Buttons.Secondary("Cancel", 100);
            btnCancel.DialogResult = DialogResult.Cancel;
            btnCancel.Location = new Point(220, 85);
            Controls.AddRange(new Control[] { lblPrompt, txtInput, btnOk, btnCancel });
            AcceptButton = btnOk;
            CancelButton = btnCancel;
        }
    }
}