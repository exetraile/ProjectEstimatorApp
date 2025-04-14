// Services/IProjectManagerService.cs
using ProjectEstimatorApp.Models;

namespace ProjectEstimatorApp.Services
{
    public interface IProjectManagerService
    {
        Project CurrentProject { get; }
        void CreateNewProject(string name);
        void SaveProject(string filePath);
        void LoadProject(string filePath);
        bool ProjectExists();
    }
}