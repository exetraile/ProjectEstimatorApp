// Services/ProjectManagerService.cs
using System;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using ProjectEstimatorApp.Models;

namespace ProjectEstimatorApp.Services
{
    public class ProjectManagerService : IProjectManagerService
    {
        public Project CurrentProject { get; private set; }

        public void CreateNewProject(string name)
        {
            CurrentProject = new Project { Name = name.Trim() };
        }

        public void SaveProject(string filePath)
        {
            if (CurrentProject == null) throw new InvalidOperationException("No project to save");

            CurrentProject.ModifiedDate = DateTime.Now;
            var json = JsonConvert.SerializeObject(CurrentProject, new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });
            File.WriteAllText(filePath, json);
        }

        public void LoadProject(string filePath)
        {
            var json = File.ReadAllText(filePath);
            CurrentProject = JsonConvert.DeserializeObject<Project>(json);
            CurrentProject.ModifiedDate = DateTime.Now;
        }

        public bool ProjectExists() => CurrentProject != null;
    }
}