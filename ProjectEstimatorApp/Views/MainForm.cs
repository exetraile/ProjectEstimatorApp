using System;
using System.Drawing;
using System.Windows.Forms;
using ProjectEstimatorApp.Helper;
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

        private TreeView _projectTreeView;
        private Panel _contentPanel;
        private EstimateEditorControl _estimateEditorControl;

        public MainForm(
            IProjectManagerService projectManager,
            IProjectStructureService structureService,
            IEstimateEditorService estimateEditor,
            ICalculationService calculationService)
        {
            _projectManager = projectManager ?? throw new ArgumentNullException(nameof(projectManager));
            _structureService = structureService ?? throw new ArgumentNullException(nameof(structureService));
            _estimateEditor = estimateEditor ?? throw new ArgumentNullException(nameof(estimateEditor));
            _calculationService = calculationService ?? throw new ArgumentNullException(nameof(calculationService));

            InitializeComponent();
            StyleHelper.Forms.ApplyMainFormStyle(this);
            InitializeApplication();
        }

        private void InitializeApplication()
        {
            InitializeControls();
            SetupLayout();
            SetupMenu();
            AttachEventHandlers();
            UpdateProjectTree();
        }

        private void InitializeControls()
        {
            _projectTreeView = new TreeView
            {
                Dock = DockStyle.Fill,
                BorderStyle = BorderStyle.None,
                BackColor = StyleHelper.Config.ElementBackgroundColor,
                ForeColor = StyleHelper.Config.TextColor,
                Font = StyleHelper.Config.NormalFont
            };

            _contentPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = StyleHelper.Config.BackgroundColor
            };

            _estimateEditorControl = new EstimateEditorControl(_estimateEditor)
            {
                Dock = DockStyle.Fill
            };
        }

        private void SetupLayout()
        {
            var mainSplitContainer = new SplitContainer
            {
                Dock = DockStyle.Fill,
                Orientation = Orientation.Horizontal,
                SplitterDistance = 250,
                BackColor = StyleHelper.Config.BackgroundColor
            };

            mainSplitContainer.Panel1.Controls.Add(CreateProjectPanel());
            mainSplitContainer.Panel2.Controls.Add(_contentPanel);
            _contentPanel.Controls.Add(_estimateEditorControl);
            Controls.Add(mainSplitContainer);
        }

        private Panel CreateProjectPanel()
        {
            var panel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = StyleHelper.Config.BackgroundColor
            };

            var header = StyleHelper.Panels.CardPanel();
            header.Dock = DockStyle.Top;
            header.Height = 50;
            header.Margin = new Padding(0, 0, 0, 8);

            var label = StyleHelper.Labels.Header("ПРОЕКТ");
            label.Dock = DockStyle.Fill;
            label.TextAlign = ContentAlignment.MiddleCenter;
            label.ForeColor = StyleHelper.Config.AccentColor;

            header.Controls.Add(label);
            panel.Controls.Add(_projectTreeView);
            panel.Controls.Add(header);
            return panel;
        }

        private void SetupMenu()
        {
            var menuStrip = new MenuStrip();
            menuStrip.BackColor = StyleHelper.Config.ElementBackgroundColor;
            menuStrip.ForeColor = Color.White;
            menuStrip.Renderer = new ToolStripProfessionalRenderer(new MenuColorTable());

            var fileMenu = new ToolStripMenuItem("Файл");
            fileMenu.ForeColor = Color.White;
            fileMenu.DropDownItems.AddRange(new ToolStripItem[] {
                new ToolStripMenuItem("Новый проект", null, NewProjectMenuItem_Click) { ForeColor = Color.White },
                new ToolStripMenuItem("Открыть проект", null, OpenProjectMenuItem_Click) { ForeColor = Color.White },
                new ToolStripMenuItem("Сохранить проект", null, SaveProjectMenuItem_Click) { ForeColor = Color.White },
                new ToolStripSeparator(),
                new ToolStripMenuItem("Экспорт в PDF", null, ExportToPdfMenuItem_Click) { ForeColor = Color.White },
                new ToolStripSeparator(),
                new ToolStripMenuItem("Выход", null, ExitMenuItem_Click) { ForeColor = Color.White }
            });

            var editMenu = new ToolStripMenuItem("Правка");
            editMenu.ForeColor = Color.White;
            editMenu.DropDownItems.AddRange(new ToolStripItem[] {
                new ToolStripMenuItem("Добавить смету", null, AddEstimateMenuItem_Click) { ForeColor = Color.White },
                new ToolStripMenuItem("Добавить деталь сметы", null, AddEstimateDetailMenuItem_Click) { ForeColor = Color.White },
                new ToolStripMenuItem("Удалить смету", null, DeleteEstimateMenuItem_Click) { ForeColor = Color.White },
                new ToolStripMenuItem("Удалить деталь сметы", null, DeleteEstimateDetailMenuItem_Click) { ForeColor = Color.White },
                new ToolStripSeparator(),
                new ToolStripMenuItem("Показать итоги", null, ShowTotalsMenuItem_Click) { ForeColor = Color.White }
            });

            menuStrip.Items.AddRange(new ToolStripItem[] { fileMenu, editMenu });
            Controls.Add(menuStrip);
            MainMenuStrip = menuStrip;
        }

        private void UpdateProjectTree()
        {
            _projectTreeView.Nodes.Clear();
            if (!_projectManager.ProjectExists()) return;

            var projectNode = new TreeNode(_projectManager.CurrentProject.Name) { Tag = _projectManager.CurrentProject };

            foreach (var estimate in _projectManager.CurrentProject.Estimates)
            {
                var estimateNode = new TreeNode(estimate.Name) { Tag = estimate };

                foreach (var detail in estimate.EstimateDetails)
                    estimateNode.Nodes.Add(new TreeNode(detail.Name) { Tag = detail });

                projectNode.Nodes.Add(estimateNode);
            }

            _projectTreeView.Nodes.Add(projectNode);
            projectNode.Expand();
        }

        private void AttachEventHandlers()
        {
            _projectTreeView.AfterSelect += (s, e) => _estimateEditorControl.SetCurrentItem(e.Node?.Tag);
            _projectTreeView.KeyDown += ProjectTreeView_KeyDown;
        }

        private void ProjectTreeView_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                DeleteSelectedNode();
            }
        }

        private void DeleteSelectedNode()
        {
            if (_projectTreeView.SelectedNode == null) return;

            var selectedItem = _projectTreeView.SelectedNode.Tag;

            if (selectedItem is EstimateDetail detail)
            {
                DeleteEstimateDetail(detail);
            }
            else if (selectedItem is Estimate estimate)
            {
                DeleteEstimate(estimate);
            }
        }

        private void NewProjectMenuItem_Click(object sender, EventArgs e)
        {
            var projectName = DialogHelper.ShowInputDialog("Новый проект", "Введите название проекта:");
            if (string.IsNullOrWhiteSpace(projectName)) return;

            _projectManager.CreateNewProject(projectName);
            UpdateProjectTree();
        }

        private void OpenProjectMenuItem_Click(object sender, EventArgs e)
        {
            using (var dialog = new OpenFileDialog { Filter = "Файлы проекта (*.json)|*.json" })
            {
                if (dialog.ShowDialog() != DialogResult.OK) return;

                try
                {
                    _projectManager.LoadProject(dialog.FileName);
                    UpdateProjectTree();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void SaveProjectMenuItem_Click(object sender, EventArgs e)
        {
            using (var dialog = new SaveFileDialog
            {
                Filter = "Файлы проекта (*.json)|*.json",
                FileName = _projectManager.ProjectExists() ? $"{_projectManager.CurrentProject.Name}.json" : "НовыйПроект.json"
            })
            {
                if (dialog.ShowDialog() != DialogResult.OK) return;

                try
                {
                    _projectManager.SaveProject(dialog.FileName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void ExportToPdfMenuItem_Click(object sender, EventArgs e)
        {
            if (!_projectManager.ProjectExists())
            {
                MessageBox.Show("Не загружен ни один проект для экспорта.", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (var dialog = new SaveFileDialog { Filter = "PDF файлы (*.pdf)|*.pdf", FileName = $"{_projectManager.CurrentProject.Name}.pdf" })
            {
                if (dialog.ShowDialog() != DialogResult.OK) return;

                try
                {
                    var summary = _calculationService.CalculateProjectSummary(_projectManager.CurrentProject);
                    ExportHelper.ExportToPdf(summary, dialog.FileName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void ExitMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void AddEstimateMenuItem_Click(object sender, EventArgs e)
        {
            var estimateName = DialogHelper.ShowInputDialog("Добавить смету", "Введите название сметы:");
            if (string.IsNullOrWhiteSpace(estimateName)) return;

            try
            {
                _structureService.AddEstimate(estimateName);
                UpdateProjectTree();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void AddEstimateDetailMenuItem_Click(object sender, EventArgs e)
        {
            if (!(_projectTreeView.SelectedNode?.Tag is Estimate selectedEstimate))
            {
                MessageBox.Show("Пожалуйста, выберите смету для добавления детали.", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var result = DialogHelper.ShowAddEstimateDetailDialog(this);
            if (result.EstimateDetailName == null) return;

            try
            {
                _structureService.AddEstimateDetail(selectedEstimate.Name, result.EstimateDetailName, result.Width, result.Height);
                UpdateProjectTree();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DeleteEstimateMenuItem_Click(object sender, EventArgs e)
        {
            if (!(_projectTreeView.SelectedNode?.Tag is Estimate selectedEstimate))
            {
                MessageBox.Show("Пожалуйста, выберите смету для удаления.", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DeleteEstimate(selectedEstimate);
        }

        private void DeleteEstimateDetailMenuItem_Click(object sender, EventArgs e)
        {
            if (!(_projectTreeView.SelectedNode?.Tag is EstimateDetail selectedDetail))
            {
                MessageBox.Show("Пожалуйста, выберите деталь сметы для удаления.", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DeleteEstimateDetail(selectedDetail);
        }

        private void DeleteEstimate(Estimate estimate)
        {
            if (MessageBox.Show($"Вы уверены, что хотите удалить смету '{estimate.Name}'?", "Подтверждение удаления",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
            {
                return;
            }

            try
            {
                _structureService.RemoveEstimate(estimate.Name);
                UpdateProjectTree();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DeleteEstimateDetail(EstimateDetail detail)
        {
            if (MessageBox.Show($"Вы уверены, что хотите удалить деталь '{detail.Name}'?", "Подтверждение удаления",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
            {
                return;
            }

            try
            {
                var estimateNode = _projectTreeView.SelectedNode.Parent;
                if (estimateNode?.Tag is Estimate parentEstimate)
                {
                    _structureService.RemoveEstimateDetail(parentEstimate.Name, detail.Name);
                    UpdateProjectTree();
                }
                else
                {
                    MessageBox.Show("Не удалось определить родительскую смету для удаляемой детали.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ShowTotalsMenuItem_Click(object sender, EventArgs e)
        {
            if (!_projectManager.ProjectExists())
            {
                MessageBox.Show("Не загружен ни один проект.", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var summary = _calculationService.CalculateProjectSummary(_projectManager.CurrentProject);
            DialogHelper.ShowTotalsDialog(summary, this);
        }
    }

    public class MenuColorTable : ProfessionalColorTable
    {
        public override Color MenuItemSelected => StyleHelper.Config.AccentColor;
        public override Color MenuItemSelectedGradientBegin => StyleHelper.Config.AccentColor;
        public override Color MenuItemSelectedGradientEnd => StyleHelper.Config.AccentColor;
        public override Color MenuItemBorder => StyleHelper.Config.AccentColor;
        public override Color MenuItemPressedGradientBegin => StyleHelper.Config.ElementBackgroundColor;
        public override Color MenuItemPressedGradientEnd => StyleHelper.Config.ElementBackgroundColor;
        public override Color MenuBorder => StyleHelper.Config.BorderColor;
        public override Color MenuStripGradientBegin => StyleHelper.Config.ElementBackgroundColor;
        public override Color MenuStripGradientEnd => StyleHelper.Config.ElementBackgroundColor;
        public override Color ToolStripDropDownBackground => StyleHelper.Config.ElementBackgroundColor;
        public override Color ImageMarginGradientBegin => StyleHelper.Config.ElementBackgroundColor;
        public override Color ImageMarginGradientEnd => StyleHelper.Config.ElementBackgroundColor;
        public override Color ImageMarginGradientMiddle => StyleHelper.Config.ElementBackgroundColor;
    }
}