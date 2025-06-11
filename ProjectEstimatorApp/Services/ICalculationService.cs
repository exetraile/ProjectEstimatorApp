// ICalculationService.cs
using ProjectEstimatorApp.Models;

namespace ProjectEstimatorApp.Services
{
    public interface ICalculationService
    {
        ProjectSummary CalculateProjectSummary(Project project);
        EstimateSummary CalculateEstimateSummary(Estimate estimate);
        EstimateDetailSummary CalculateEstimateDetailSummary(EstimateDetail estimateDetail);
    }
}