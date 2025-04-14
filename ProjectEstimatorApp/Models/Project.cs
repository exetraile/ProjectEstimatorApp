using System.Collections.Generic;
using System;

namespace ProjectEstimatorApp.Models
{
    public class Project
    {
        public string Name { get; set; } = "Новый проект";
        public List<Floor> Floors { get; set; } = new List<Floor>();
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime ModifiedDate { get; set; } = DateTime.Now;
    }

    public class Floor
    {
        public string Name { get; set; } = "Новый этаж";
        public List<Room> Rooms { get; set; } = new List<Room>();
        public List<EstimateItem> Works { get; set; } = new List<EstimateItem>();
        public List<EstimateItem> Materials { get; set; } = new List<EstimateItem>();
    }

    public class Room
    {
        public string Name { get; set; } = "Новая комната";
        public double Width { get; set; }
        public double Height { get; set; }
        public List<Estimate> Estimates { get; set; } = new List<Estimate>();
        public List<EstimateItem> Works { get; set; } = new List<EstimateItem>();
        public List<EstimateItem> Materials { get; set; } = new List<EstimateItem>();
    }

    public class Estimate
    {
        public string Category { get; set; } = "Общие работы";
        public List<EstimateItem> Works { get; set; } = new List<EstimateItem>();
        public List<EstimateItem> Materials { get; set; } = new List<EstimateItem>();
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
        public DateTime CalculationDate { get; set; }
        public decimal TotalWorks { get; set; }
        public decimal TotalMaterials { get; set; }
        public decimal OverallTotal { get; set; }
        public List<FloorSummary> FloorSummaries { get; set; } = new List<FloorSummary>();
    }

    public class FloorSummary
    {
        public string FloorName { get; set; }
        public decimal WorksTotal { get; set; }
        public decimal MaterialsTotal { get; set; }
        public decimal Total { get; set; }
        public List<RoomSummary> RoomSummaries { get; set; } = new List<RoomSummary>();
    }

    public class RoomSummary
    {
        public string RoomName { get; set; }
        public decimal WorksTotal { get; set; }
        public decimal MaterialsTotal { get; set; }
        public decimal Total { get; set; }
        public List<EstimateSummary> EstimateSummaries { get; set; } = new List<EstimateSummary>();
    }

    public class EstimateSummary
    {
        public string Category { get; set; }
        public decimal WorksTotal { get; set; }
        public decimal MaterialsTotal { get; set; }
        public decimal Total { get; set; }
    }
}