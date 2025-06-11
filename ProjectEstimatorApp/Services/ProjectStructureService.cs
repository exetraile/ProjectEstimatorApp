// Services/ProjectStructureService.cs
using System;
using System.Collections.Generic;
using System.Linq;
using ProjectEstimatorApp.Models;

namespace ProjectEstimatorApp.Services
{
    public class ProjectStructureService : IProjectStructureService
    {
        private readonly IProjectManagerService _projectManager;

        public ProjectStructureService(IProjectManagerService projectManager)
        {
            _projectManager = projectManager ?? throw new ArgumentNullException(nameof(projectManager));
        }

        public void AddEstimate(string name)
        {
            ValidateProjectExists();
            ValidateName(name, "Estimate name");

            if (_projectManager.CurrentProject.Estimates.Any(e =>
                string.Equals(e.Name, name, StringComparison.OrdinalIgnoreCase)))
                throw new ArgumentException($"Estimate '{name}' already exists");

            _projectManager.CurrentProject.Estimates.Add(new Estimate { Name = name.Trim() });
            UpdateModifiedDate();
        }

        public void AddEstimateDetail(string estimateName, string estimateDetailName, double width, double height)
        {
            ValidateProjectExists();
            ValidateName(estimateName, "Estimate name");
            ValidateName(estimateDetailName, "EstimateDetail name");

            if (width <= 0.1 || height <= 0.1 || width > 50 || height > 50)
                throw new ArgumentException("Размеры EstimateDetail должны быть от 0.1 до 50 метров");

            if (estimateDetailName.Length > 50)
                throw new ArgumentException("Название EstimateDetail не должно превышать 50 символов");

            var estimate = GetEstimate(estimateName);

            if (estimate.EstimateDetails.Count >= 100)
                throw new InvalidOperationException("Превышен лимит EstimateDetails на Estimate (максимум 100)");

            if (estimate.EstimateDetails.Any(r => string.Equals(r.Name, estimateDetailName, StringComparison.OrdinalIgnoreCase)))
                throw new ArgumentException($"EstimateDetail '{estimateDetailName}' уже существует на Estimate '{estimateName}'");

            estimate.EstimateDetails.Add(new EstimateDetail
            {
                Name = estimateDetailName.Trim(),
                Width = Math.Round(width, 2),
                Height = Math.Round(height, 2)
            });

            UpdateModifiedDate();
        }

        public void RemoveEstimate(string estimateName)
        {
            ValidateProjectExists();
            var estimate = GetEstimate(estimateName);

            if (estimate.EstimateDetails.Any())
                throw new InvalidOperationException("Нельзя удалить Estimate с EstimateDetails. Сначала удалите все EstimateDetails.");

            _projectManager.CurrentProject.Estimates.Remove(estimate);
            UpdateModifiedDate();
        }

        public void AddEstimateToProject(string category)
        {
            ValidateProjectExists();
            ValidateName(category, "Estimate category");

            if (_projectManager.CurrentProject.ProjectEstimates.Any(e =>
                string.Equals(e.Category, category, StringComparison.OrdinalIgnoreCase)))
                throw new ArgumentException($"Project estimate '{category}' already exists");

            _projectManager.CurrentProject.ProjectEstimates.Add(new EstimateModel
            {
                Category = category.Trim()
            });
            UpdateModifiedDate();
        }


        public void AddEstimateToEstimate(string estimateName, string category)
        {
            ValidateProjectExists();
            ValidateName(estimateName, "Estimate name");
            ValidateName(category, "Estimate category");

            var estimate = GetEstimate(estimateName);

            if (estimate.EstimateEstimates.Any(e =>
                string.Equals(e.Category, category, StringComparison.OrdinalIgnoreCase)))
                throw new ArgumentException($"Estimate estimate '{category}' already exists on estimate '{estimateName}'");

            estimate.EstimateEstimates.Add(new EstimateModel
            {
                Category = category.Trim()
            });
            UpdateModifiedDate();
        }

        public void AddEstimateModel(string estimateName, string estimateDetailName, string category)
        {
            ValidateProjectExists();
            ValidateName(estimateName, "Estimate name");
            ValidateName(estimateDetailName, "EstimateDetail name");
            ValidateName(category, "Estimate category");

            var estimateDetail = GetEstimateDetail(estimateName, estimateDetailName);
            if (estimateDetail.Estimates.Any(e =>
                string.Equals(e.Category, category, StringComparison.OrdinalIgnoreCase)))
                throw new ArgumentException($"Estimate '{category}' already exists in estimateDetail '{estimateDetailName}'");

            estimateDetail.Estimates.Add(new EstimateModel { Category = category.Trim() });
            UpdateModifiedDate();
        }

        public void RemoveEstimateDetail(string estimateName, string estimateDetailName)
        {
            ValidateProjectExists();
            var estimateDetail = GetEstimateDetail(estimateName, estimateDetailName);
            var estimate = GetEstimate(estimateName);
            estimate.EstimateDetails.Remove(estimateDetail);
            UpdateModifiedDate();
        }

        public void RemoveEstimateModel(string estimateName, string estimateDetailName, string category)
        {
            ValidateProjectExists();
            var estimateModel = GetEstimateModel(estimateName, estimateDetailName, category);
            var estimateDetail = GetEstimateDetail(estimateName, estimateDetailName);
            estimateDetail.Estimates.Remove(estimateModel);
            UpdateModifiedDate();
        }

        public IEnumerable<string> GetEstimateNames()
        {
            ValidateProjectExists();
            return _projectManager.CurrentProject.Estimates.Select(f => f.Name).ToList();
        }

        public IEnumerable<string> GetEstimateDetailNames(string estimateName)
        {
            ValidateProjectExists();
            var estimate = GetEstimate(estimateName);
            return estimate.EstimateDetails.Select(r => r.Name).ToList();
        }

        #region Private Helpers
        private Estimate GetEstimate(string name)
        {
            var estimate = _projectManager.CurrentProject.Estimates
                .FirstOrDefault(f => string.Equals(f.Name, name, StringComparison.OrdinalIgnoreCase));
            return estimate ?? throw new ArgumentException($"Estimate '{name}' not found");
        }

        private EstimateDetail GetEstimateDetail(string estimateName, string estimateDetailName)
        {
            var estimate = GetEstimate(estimateName);
            var estimateDetail = estimate.EstimateDetails
                .FirstOrDefault(r => string.Equals(r.Name, estimateDetailName, StringComparison.OrdinalIgnoreCase));
            return estimateDetail ?? throw new ArgumentException($"EstimateDetail '{estimateDetailName}' not found on estimate '{estimateName}'");
        }

        private EstimateModel GetEstimateModel(string estimateName, string estimateDetailName, string category)
        {
            var estimateDetail = GetEstimateDetail(estimateName, estimateDetailName);
            var estimateModel = estimateDetail.Estimates
                .FirstOrDefault(e => string.Equals(e.Category, category, StringComparison.OrdinalIgnoreCase));
            return estimateModel ?? throw new ArgumentException($"Estimate '{category}' not found in estimateDetail '{estimateDetailName}'");
        }

        private void ValidateProjectExists()
        {
            if (!_projectManager.ProjectExists())
                throw new InvalidOperationException("Project not loaded");
        }

        private void ValidateName(string name, string fieldName)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException($"{fieldName} cannot be empty");
        }

        private void ValidateDimensions(double width, double height)
        {
            if (width <= 0 || height <= 0)
                throw new ArgumentException("Width and height must be positive numbers");
        }

        private void UpdateModifiedDate()
        {
            _projectManager.CurrentProject.ModifiedDate = DateTime.Now;
        }
        #endregion
    }
}

