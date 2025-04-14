using System;
using System.Linq;
using ProjectEstimatorApp.Models;

namespace ProjectEstimatorApp.Services
{
    public class CalculationService : ICalculationService
    {
        public ProjectSummary CalculateProjectSummary(Project project)
        {
            var summary = new ProjectSummary { CalculationDate = DateTime.Now };

            summary.FloorSummaries = project.Floors.Select(CalculateFloorSummary).ToList();
            summary.TotalWorks = summary.FloorSummaries.Sum(f => f.WorksTotal);
            summary.TotalMaterials = summary.FloorSummaries.Sum(f => f.MaterialsTotal);
            summary.OverallTotal = summary.TotalWorks + summary.TotalMaterials;

            return summary;
        }

        public FloorSummary CalculateFloorSummary(Floor floor)
        {
            var summary = new FloorSummary { FloorName = floor.Name };

            // Добавляем работы/материалы самого этажа
            summary.WorksTotal = floor.Works.Sum(i => i.Total);
            summary.MaterialsTotal = floor.Materials.Sum(i => i.Total);

            // Добавляем данные из комнат
            summary.RoomSummaries = floor.Rooms.Select(CalculateRoomSummary).ToList();
            summary.WorksTotal += summary.RoomSummaries.Sum(r => r.WorksTotal);
            summary.MaterialsTotal += summary.RoomSummaries.Sum(r => r.MaterialsTotal);
            summary.Total = summary.WorksTotal + summary.MaterialsTotal;

            return summary;
        }

        public RoomSummary CalculateRoomSummary(Room room)
        {
            var summary = new RoomSummary { RoomName = room.Name };

            // Добавляем работы/материалы самой комнаты
            summary.WorksTotal = room.Works.Sum(i => i.Total);
            summary.MaterialsTotal = room.Materials.Sum(i => i.Total);

            // Добавляем данные из смет
            summary.EstimateSummaries = room.Estimates.Select(e => new EstimateSummary
            {
                Category = e.Category,
                WorksTotal = e.Works.Sum(i => i.Total),
                MaterialsTotal = e.Materials.Sum(i => i.Total),
                Total = e.Works.Sum(i => i.Total) + e.Materials.Sum(i => i.Total)
            }).ToList();

            summary.WorksTotal += summary.EstimateSummaries.Sum(e => e.WorksTotal);
            summary.MaterialsTotal += summary.EstimateSummaries.Sum(e => e.MaterialsTotal);
            summary.Total = summary.WorksTotal + summary.MaterialsTotal;

            return summary;
        }
    }
}