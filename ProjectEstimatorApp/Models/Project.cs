using System.Collections.Generic;
using System;
using System.Linq;

namespace ProjectEstimatorApp.Models
{
    public class Project
    {
        public string Name { get; set; } = "Новый проект";
        public List<Estimate> Estimates { get; set; } = new List<Estimate>();
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime ModifiedDate { get; set; } = DateTime.Now;
        public List<EstimateModel> ProjectEstimates { get; set; } = new List<EstimateModel>();
    }

    public class Estimate
    {
        public string Name { get; set; } = "Новая смета";
        public List<EstimateDetail> EstimateDetails { get; set; } = new List<EstimateDetail>();
        public List<EstimateItem> Works { get; set; } = new List<EstimateItem>();
        public List<EstimateItem> Materials { get; set; } = new List<EstimateItem>();
        public List<EstimateModel> EstimateEstimates { get; set; } = new List<EstimateModel>();
    }

    public class EstimateDetail
    {
        public string Name { get; set; } = "Новая деталь сметы";
        public double Width { get; set; }
        public double Height { get; set; }
        public List<EstimateModel> Estimates { get; set; } = new List<EstimateModel>();
        public List<EstimateItem> Works { get; set; } = new List<EstimateItem>();
        public List<EstimateItem> Materials { get; set; } = new List<EstimateItem>();
    }

    public class EstimateModel
    {
        public string Category { get; set; } = "Общие работы";
        public List<EstimateItem> Works { get; set; } = new List<EstimateItem>();
        public List<EstimateItem> Materials { get; set; } = new List<EstimateItem>();
        public decimal WorksTotal => Works.Sum(w => w.Total);
        public decimal MaterialsTotal => Materials.Sum(m => m.Total);
    }

    public class EstimateItem
    {
        public string Name { get; set; } = "Новая позиция";
        public string Unit { get; set; } = "шт.";
        public decimal Quantity { get; set; } = 1;
        public decimal PricePerUnit { get; set; }
        public string Notes { get; set; }
        public decimal Total => Quantity * PricePerUnit;
    }

    public class ProjectSummary
    {
        public string ProjectName { get; set; }
        public DateTime CalculationDate { get; set; }
        public List<EstimateSummary> ProjectEstimates { get; set; } = new List<EstimateSummary>();
        public decimal ProjectWorksTotal { get; set; }
        public decimal ProjectMaterialsTotal { get; set; }
        public decimal ProjectTotal { get; set; }
        public List<EstimateSummary> EstimateSummaries { get; set; } = new List<EstimateSummary>();
        public decimal EstimatesWorksTotal { get; set; }
        public decimal EstimatesMaterialsTotal { get; set; }
        public decimal EstimatesTotal { get; set; }
        public decimal TotalWorks { get; set; }
        public decimal TotalMaterials { get; set; }
        public decimal OverallTotal { get; set; }
    }

    public class EstimateSummary
    {
        public string EstimateName { get; set; }
        public List<EstimateSummary> EstimateEstimates { get; set; } = new List<EstimateSummary>();
        public decimal EstimateWorksTotal { get; set; }
        public decimal EstimateMaterialsTotal { get; set; }
        public decimal EstimateTotal { get; set; }
        public List<EstimateDetailSummary> EstimateDetailSummaries { get; set; } = new List<EstimateDetailSummary>();
        public decimal EstimateDetailsWorksTotal { get; set; }
        public decimal EstimateDetailsMaterialsTotal { get; set; }
        public decimal EstimateDetailsTotal { get; set; }
        public decimal WorksTotal { get; set; }
        public decimal MaterialsTotal { get; set; }
        public decimal Total { get; set; }
    }

    public class EstimateDetailSummary
    {
        public string EstimateDetailName { get; set; }
        public double Area { get; set; }
        public List<EstimateSummary> Estimates { get; set; } = new List<EstimateSummary>();
        public decimal WorksTotal { get; set; }
        public decimal MaterialsTotal { get; set; }
        public decimal Total { get; set; }
    }

    public class EstimateModelSummary
    {
        public string Category { get; set; }
        public decimal WorksTotal { get; set; }
        public decimal MaterialsTotal { get; set; }
        public decimal Total { get; set; }
    }
}

