// Views/MainForm.cs
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using ProjectEstimatorApp.Models;
using ProjectEstimatorApp.Services;
using FontAwesome.Sharp;

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
            Width = 1200;
            Height = 800;
            BackColor = Color.White;
            Font = new Font("Segoe UI", 9);

            // Tree view
            _projectTree = new TreeView
            {
                Dock = DockStyle.Left,
                Width = 300,
                BackColor = Color.FromArgb(240, 240, 240),
                BorderStyle = BorderStyle.None,
                Font = new Font("Segoe UI", 10),
                ShowLines = false,
                ShowPlusMinus = false,
                FullRowSelect = true
            };
            _projectTree.AfterSelect += ProjectTree_AfterSelect;

            // Buttons panel
            var buttonsPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                Height = 60,
                BackColor = Color.FromArgb(52, 152, 219),
                Padding = new Padding(10),
                FlowDirection = FlowDirection.LeftToRight
            };

            _btnNewProject = CreateStyledButton("New Project", IconChar.FileAlt);
            _btnSaveProject = CreateStyledButton("Save", IconChar.Save);
            _btnLoadProject = CreateStyledButton("Load", IconChar.FolderOpen);
            _btnAddFloor = CreateStyledButton("Add Floor", IconChar.LayerGroup);
            _btnAddRoom = CreateStyledButton("Add Room", IconChar.DoorOpen);
            _btnAddEstimate = CreateStyledButton("Add Estimate", IconChar.PlusCircle);
            _btnShowTotals = CreateStyledButton("Totals", IconChar.Calculator);

            buttonsPanel.Controls.AddRange(new Control[] {
                _btnNewProject, _btnSaveProject, _btnLoadProject,
                _btnAddFloor, _btnAddRoom, _btnAddEstimate, _btnShowTotals
            });

            // Estimate editor
            _estimateEditorControl = new EstimateEditorControl(_estimateEditor)
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White
            };

            // Event handlers
            _btnNewProject.Click += (s, e) => CreateNewProject();
            _btnSaveProject.Click += (s, e) => SaveProject();
            _btnLoadProject.Click += (s, e) => LoadProject();
            _btnAddFloor.Click += (s, e) => AddFloor();
            _btnAddRoom.Click += (s, e) => AddRoom();
            _btnAddEstimate.Click += (s, e) => AddEstimate();
            _btnShowTotals.Click += (s, e) => ShowTotals();

            // Добавление элементов
            Controls.Add(_estimateEditorControl);
            Controls.Add(buttonsPanel);
            Controls.Add(_projectTree);
        }

        private Button CreateStyledButton(string text, IconChar icon, Color? bgColor = null)
        {
            var btn = new Button
            {
                Text = text,
                Height = 40,
                Width = 120,
                FlatStyle = FlatStyle.Flat,
                BackColor = bgColor ?? Color.FromArgb(52, 152, 219),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                TextImageRelation = TextImageRelation.ImageBeforeText,
                Image = icon.ToBitmap(16, Color.White),
                Cursor = Cursors.Hand
            };

            btn.FlatAppearance.BorderSize = 0;
            btn.FlatAppearance.MouseOverBackColor = Color.FromArgb(41, 128, 185);
            btn.FlatAppearance.MouseDownBackColor = Color.FromArgb(32, 102, 148);

            return btn;
        }
        private void InitializeProjectTree()
        {
            _projectTree.Nodes.Clear();
            if (_projectManager.CurrentProject != null)
            {
                var projectNode = _projectTree.Nodes.Add(_projectManager.CurrentProject.Name);
                projectNode.Tag = _projectManager.CurrentProject;

                foreach (var estimate in _projectManager.CurrentProject.ProjectEstimates)
                {
                    var estimateNode = projectNode.Nodes.Add(estimate.Category);
                    estimateNode.Tag = estimate;
                }

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
                    // Проверяем, что выбрано (проект, этаж или комната)
                    if (_projectTree.SelectedNode?.Tag is Project project)
                    {
                        // Добавляем оценку на уровне проекта
                        _structureService.AddEstimateToProject(dialog.InputText);
                    }
                    else if (_projectTree.SelectedNode?.Tag is Floor floor)
                    {
                        // Добавляем оценку на уровне этажа
                        _structureService.AddEstimateToFloor(floor.Name, dialog.InputText);
                    }
                    else if (_projectTree.SelectedNode?.Tag is Room room)
                    {
                        // Добавляем оценку на уровне комнаты
                        var floorName = (_projectTree.SelectedNode?.Parent?.Tag as Floor)?.Name;
                        if (floorName != null)
                        {
                            _structureService.AddEstimate(floorName, room.Name, dialog.InputText);
                        }
                    }
                    else
                    {
                        // Если ничего не выбрано или выбрана оценка, добавляем на уровень проекта по умолчанию
                        _structureService.AddEstimateToProject(dialog.InputText);
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