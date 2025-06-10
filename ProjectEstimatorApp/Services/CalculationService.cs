// Services/CalculationService.cs
using System;
using System.Linq;
using ProjectEstimatorApp.Models;

namespace ProjectEstimatorApp.Services
{
    public class CalculationService : ICalculationService
    {
        public ProjectSummary CalculateProjectSummary(Project project)
        {
            var summary = new ProjectSummary
            {
                ProjectName = project.Name,
                CalculationDate = DateTime.Now
            };

            summary.ProjectEstimates = project.ProjectEstimates
                .Select(CalculateEstimateSummary)
                .ToList();

            summary.ProjectWorksTotal = summary.ProjectEstimates.Sum(e => e.WorksTotal);
            summary.ProjectMaterialsTotal = summary.ProjectEstimates.Sum(e => e.MaterialsTotal);
            summary.ProjectTotal = summary.ProjectWorksTotal + summary.ProjectMaterialsTotal;
            summary.FloorSummaries = project.Floors.Select(CalculateFloorSummary).ToList();
            summary.FloorsWorksTotal = summary.FloorSummaries.Sum(f => f.WorksTotal);
            summary.FloorsMaterialsTotal = summary.FloorSummaries.Sum(f => f.MaterialsTotal);
            summary.FloorsTotal = summary.FloorsWorksTotal + summary.FloorsMaterialsTotal;
            summary.TotalWorks = summary.ProjectWorksTotal + summary.FloorsWorksTotal;
            summary.TotalMaterials = summary.ProjectMaterialsTotal + summary.FloorsMaterialsTotal;
            summary.OverallTotal = summary.TotalWorks + summary.TotalMaterials;

            return summary;
        }

        public FloorSummary CalculateFloorSummary(Floor floor)
        {
            var summary = new FloorSummary
            {
                FloorName = floor.Name
            };

            summary.FloorEstimates = floor.FloorEstimates
                .Select(CalculateEstimateSummary)
                .ToList();

            summary.FloorWorksTotal = summary.FloorEstimates.Sum(e => e.WorksTotal);
            summary.FloorMaterialsTotal = summary.FloorEstimates.Sum(e => e.MaterialsTotal);
            summary.FloorTotal = summary.FloorWorksTotal + summary.FloorMaterialsTotal;
            summary.RoomSummaries = floor.Rooms.Select(CalculateRoomSummary).ToList();
            summary.RoomsWorksTotal = summary.RoomSummaries.Sum(r => r.WorksTotal);
            summary.RoomsMaterialsTotal = summary.RoomSummaries.Sum(r => r.MaterialsTotal);
            summary.RoomsTotal = summary.RoomsWorksTotal + summary.RoomsMaterialsTotal;
            summary.WorksTotal = summary.FloorWorksTotal + summary.RoomsWorksTotal;
            summary.MaterialsTotal = summary.FloorMaterialsTotal + summary.RoomsMaterialsTotal;
            summary.Total = summary.WorksTotal + summary.MaterialsTotal;

            return summary;
        }

        public RoomSummary CalculateRoomSummary(Room room)
        {
            var summary = new RoomSummary
            {
                RoomName = room.Name,
                Area = room.Width * room.Height
            };

            summary.Estimates = room.Estimates
                .Select(CalculateEstimateSummary)
                .ToList();
            summary.WorksTotal = summary.Estimates.Sum(e => e.WorksTotal);
            summary.MaterialsTotal = summary.Estimates.Sum(e => e.MaterialsTotal);
            summary.Total = summary.WorksTotal + summary.MaterialsTotal;

            return summary;
        }

        public EstimateSummary CalculateEstimateSummary(Estimate estimate)
        {
            return new EstimateSummary
            {
                Category = estimate.Category,
                WorksTotal = estimate.Works.Sum(i => i.Total),
                MaterialsTotal = estimate.Materials.Sum(i => i.Total),
                Total = estimate.Works.Sum(i => i.Total) + estimate.Materials.Sum(i => i.Total)
            };
        }
    }
}