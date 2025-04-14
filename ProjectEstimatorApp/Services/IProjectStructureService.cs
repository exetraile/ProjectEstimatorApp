// Services/IProjectStructureService.cs
using System.Collections.Generic;

namespace ProjectEstimatorApp.Services
{
    public interface IProjectStructureService
    {
        void AddFloor(string name);
        void AddRoom(string floorName, string roomName, double width, double height);
        void AddEstimate(string floorName, string roomName, string category);
        void RemoveFloor(string floorName);
        void RemoveRoom(string floorName, string roomName);
        void RemoveEstimate(string floorName, string roomName, string category);
        IEnumerable<string> GetFloorNames();
        IEnumerable<string> GetRoomNames(string floorName);
    }
}