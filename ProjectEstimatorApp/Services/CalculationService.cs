using System;
using System.Linq;
using ProjectEstimatorApp.Models;

namespace ProjectEstimatorApp.Services
{
    public class CalculationService : ICalculationService
    {
        public ProjectSummary CalculateProjectSummary(Project project)
        {
            if (project == null) return new ProjectSummary();

            var summary = new ProjectSummary
            {
                ProjectName = project.Name,
                CalculationDate = DateTime.Now,
                ProjectEstimates = project.ProjectEstimates?
                    .Select(e => new EstimateSummary
                    {
                        EstimateName = e.Category,
                        WorksTotal = e.Works.Sum(w => w.Total),
                        MaterialsTotal = e.Materials.Sum(m => m.Total),
                        Total = e.WorksTotal + e.MaterialsTotal
                    })
                    .ToList(),
                EstimateSummaries = project.Estimates?
                    .Select(CalculateEstimateSummary)
                    .ToList()
            };

            summary.ProjectWorksTotal = summary.ProjectEstimates?.Sum(e => e.WorksTotal) ?? 0;
            summary.ProjectMaterialsTotal = summary.ProjectEstimates?.Sum(e => e.MaterialsTotal) ?? 0;
            summary.ProjectTotal = summary.ProjectEstimates?.Sum(e => e.Total) ?? 0;

            summary.EstimatesWorksTotal = summary.EstimateSummaries?.Sum(e => e.WorksTotal) ?? 0;
            summary.EstimatesMaterialsTotal = summary.EstimateSummaries?.Sum(e => e.MaterialsTotal) ?? 0;
            summary.EstimatesTotal = summary.EstimateSummaries?.Sum(e => e.Total) ?? 0;

            summary.TotalWorks = summary.ProjectWorksTotal + summary.EstimatesWorksTotal;
            summary.TotalMaterials = summary.ProjectMaterialsTotal + summary.EstimatesMaterialsTotal;
            summary.OverallTotal = summary.TotalWorks + summary.TotalMaterials;

            return summary;
        }

        public EstimateSummary CalculateEstimateSummary(Estimate estimate)
        {
            if (estimate == null) return new EstimateSummary();

            var summary = new EstimateSummary
            {
                EstimateName = estimate.Name,
                WorksTotal = estimate.Works.Sum(w => w.Total),
                MaterialsTotal = estimate.Materials.Sum(m => m.Total),
                EstimateDetailSummaries = estimate.EstimateDetails?
                    .Select(CalculateEstimateDetailSummary)
                    .ToList()
            };

            summary.Total = summary.WorksTotal + summary.MaterialsTotal +
                          (summary.EstimateDetailSummaries?.Sum(d => d.Total) ?? 0);

            return summary;
        }

        public EstimateDetailSummary CalculateEstimateDetailSummary(EstimateDetail detail)
        {
            if (detail == null) return new EstimateDetailSummary();

            return new EstimateDetailSummary
            {
                EstimateDetailName = detail.Name,
                Area = detail.Width * detail.Height,
                WorksTotal = detail.Works.Sum(w => w.Total),
                MaterialsTotal = detail.Materials.Sum(m => m.Total),
                Total = detail.Works.Sum(w => w.Total) + detail.Materials.Sum(m => m.Total)
            };
        }
    }
}