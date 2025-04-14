// Services/ICalculationService.cs
using ProjectEstimatorApp.Models;

namespace ProjectEstimatorApp.Services
{
    public interface ICalculationService
    {
        ProjectSummary CalculateProjectSummary(Project project);
        FloorSummary CalculateFloorSummary(Floor floor);
        RoomSummary CalculateRoomSummary(Room room);
    }
}