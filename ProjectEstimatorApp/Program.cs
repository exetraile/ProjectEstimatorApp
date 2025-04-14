// Program.cs
using System;
using System.Windows.Forms;
using ProjectEstimatorApp.Services;
using ProjectEstimatorApp.Views;

namespace ProjectEstimatorApp
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            var services = InitializeServices();
            Application.Run(new MainForm(
                services.projectManager,
                services.structureService,
                services.estimateEditor,
                services.calculationService));
        }

        private static (IProjectManagerService projectManager,
                      IProjectStructureService structureService,
                      IEstimateEditorService estimateEditor,
                      ICalculationService calculationService) InitializeServices()
        {
            var projectManager = new ProjectManagerService();
            var structureService = new ProjectStructureService(projectManager);
            var estimateEditor = new EstimateEditorService();
            var calculationService = new CalculationService();

            return (projectManager, structureService, estimateEditor, calculationService);
        }
    }
}