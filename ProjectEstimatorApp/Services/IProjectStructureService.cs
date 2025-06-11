// Services/IProjectStructureService.cs
using System.Collections.Generic;
namespace ProjectEstimatorApp.Services
{
    public interface IProjectStructureService
    {
        void AddEstimate(string name);
        void AddEstimateDetail(string estimateName, string estimateDetailName, double width, double height);
        void AddEstimateModel(string estimateName, string estimateDetailName, string category);
        void RemoveEstimate(string estimateName);
        void RemoveEstimateDetail(string estimateName, string estimateDetailName);
        void RemoveEstimateModel(string estimateName, string estimateDetailName, string category);
        IEnumerable<string> GetEstimateNames();
        IEnumerable<string> GetEstimateDetailNames(string estimateName);
        void AddEstimateToProject(string category);
        void AddEstimateToEstimate(string estimateName, string category);
    }
}