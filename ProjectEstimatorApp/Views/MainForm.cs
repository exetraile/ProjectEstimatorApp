// Views/MainForm.cs
using System;
using System.Linq;
using System.Windows.Forms;
using ProjectEstimatorApp.Models;
using ProjectEstimatorApp.Services;

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
            InitializeControls();
            InitializeProjectTree();
        }

        private void InitializeControls()
        {
            // Main form settings
            Text = "Project Estimator";
            Width = 1000;
            Height = 700;

            // Tree view
            _projectTree = new TreeView { Dock = DockStyle.Left, Width = 250 };
            _projectTree.AfterSelect += ProjectTree_AfterSelect;

            // Buttons panel
            var buttonsPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                FlowDirection = FlowDirection.LeftToRight,
                Height = 40
            };

            _btnNewProject = new Button { Text = "New Project" };
            _btnSaveProject = new Button { Text = "Save Project" };
            _btnLoadProject = new Button { Text = "Load Project" };
            _btnAddFloor = new Button { Text = "Add Floor" };
            _btnAddRoom = new Button { Text = "Add Room" };
            _btnAddEstimate = new Button { Text = "Add Estimate" };
            _btnShowTotals = new Button { Text = "Show Totals" };

            buttonsPanel.Controls.AddRange(new Control[] {
                _btnNewProject, _btnSaveProject, _btnLoadProject,
                _btnAddFloor, _btnAddRoom, _btnAddEstimate, _btnShowTotals
            });

            // Estimate editor
            _estimateEditorControl = new EstimateEditorControl(_estimateEditor)
            {
                Dock = DockStyle.Fill
            };

            // Event handlers
            _btnNewProject.Click += (s, e) => CreateNewProject();
            _btnSaveProject.Click += (s, e) => SaveProject();
            _btnLoadProject.Click += (s, e) => LoadProject();
            _btnAddFloor.Click += (s, e) => AddFloor();
            _btnAddRoom.Click += (s, e) => AddRoom();
            _btnAddEstimate.Click += (s, e) => AddEstimate();
            _btnShowTotals.Click += (s, e) => ShowTotals();

            // Add controls to form
            Controls.Add(_estimateEditorControl);
            Controls.Add(buttonsPanel);
            Controls.Add(_projectTree);
        }

        private void InitializeProjectTree()
        {
            _projectTree.Nodes.Clear();
            if (_projectManager.CurrentProject != null)
            {
                var projectNode = _projectTree.Nodes.Add(_projectManager.CurrentProject.Name);
                projectNode.Tag = _projectManager.CurrentProject;

                foreach (var floor in _projectManager.CurrentProject.Floors)
                {
                    var floorNode = projectNode.Nodes.Add(floor.Name);
                    floorNode.Tag = floor;

                    foreach (var room in floor.Rooms)
                    {
                        var roomNode = floorNode.Nodes.Add($"{room.Name} ({room.Width}x{room.Height})");
                        roomNode.Tag = room;

                        foreach (var estimate in room.Estimates)
                        {
                            var estimateNode = roomNode.Nodes.Add(estimate.Category);
                            estimateNode.Tag = estimate;
                        }
                    }
                }
                projectNode.Expand();
            }
        }

        private void ProjectTree_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node?.Tag != null)
            {
                _estimateEditorControl.SetCurrentItem(e.Node.Tag);
            }
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
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    var roomNode = _projectTree.SelectedNode?.Tag is Room room ? room :
                        _projectTree.SelectedNode?.Parent?.Tag as Room;

                    if (roomNode != null)
                    {
                        var floorName = (_projectTree.SelectedNode?.Parent?.Tag as Floor)?.Name ??
                            (_projectTree.SelectedNode?.Parent?.Parent?.Tag as Floor)?.Name;

                        if (floorName != null)
                        {
                            _structureService.AddEstimate(floorName, roomNode.Name, dialog.InputText);
                            InitializeProjectTree();
                        }
                    }
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
        public string InputText => txtInput.Text;
        private TextBox txtInput;

        public InputDialog(string title, string prompt)
        {
            Text = title;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            StartPosition = FormStartPosition.CenterParent;
            ClientSize = new System.Drawing.Size(300, 100);

            var lblPrompt = new Label { Text = prompt, Left = 10, Top = 10, Width = 280 };
            txtInput = new TextBox { Left = 10, Top = 30, Width = 280 };
            var btnOk = new Button { Text = "OK", Left = 120, Top = 60, Width = 80, DialogResult = DialogResult.OK };
            var btnCancel = new Button { Text = "Cancel", Left = 210, Top = 60, Width = 80, DialogResult = DialogResult.Cancel };

            Controls.AddRange(new Control[] { lblPrompt, txtInput, btnOk, btnCancel });
            AcceptButton = btnOk;
            CancelButton = btnCancel;
        }
    }
}