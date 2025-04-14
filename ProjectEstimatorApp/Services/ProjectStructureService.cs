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

        public void AddFloor(string name)
        {
            ValidateProjectExists();
            ValidateName(name, "Floor name");

            if (_projectManager.CurrentProject.Floors.Any(f =>
                string.Equals(f.Name, name, StringComparison.OrdinalIgnoreCase)))
                throw new ArgumentException($"Floor '{name}' already exists");

            _projectManager.CurrentProject.Floors.Add(new Floor { Name = name.Trim() });
            UpdateModifiedDate();
        }

        public void AddRoom(string floorName, string roomName, double width, double height)
        {
            ValidateProjectExists();
            ValidateName(floorName, "Floor name");
            ValidateName(roomName, "Room name");
            ValidateDimensions(width, height);

            var floor = GetFloor(floorName);
            if (floor.Rooms.Any(r =>
                string.Equals(r.Name, roomName, StringComparison.OrdinalIgnoreCase)))
                throw new ArgumentException($"Room '{roomName}' already exists on floor '{floorName}'");

            floor.Rooms.Add(new Room
            {
                Name = roomName.Trim(),
                Width = width,
                Height = height
            });
            UpdateModifiedDate();
        }

        public void AddEstimate(string floorName, string roomName, string category)
        {
            ValidateProjectExists();
            ValidateName(floorName, "Floor name");
            ValidateName(roomName, "Room name");
            ValidateName(category, "Estimate category");

            var room = GetRoom(floorName, roomName);
            if (room.Estimates.Any(e =>
                string.Equals(e.Category, category, StringComparison.OrdinalIgnoreCase)))
                throw new ArgumentException($"Estimate '{category}' already exists in room '{roomName}'");

            room.Estimates.Add(new Estimate { Category = category.Trim() });
            UpdateModifiedDate();
        }

        public void RemoveFloor(string floorName)
        {
            ValidateProjectExists();
            var floor = GetFloor(floorName);
            _projectManager.CurrentProject.Floors.Remove(floor);
            UpdateModifiedDate();
        }

        public void RemoveRoom(string floorName, string roomName)
        {
            ValidateProjectExists();
            var room = GetRoom(floorName, roomName);
            var floor = GetFloor(floorName);
            floor.Rooms.Remove(room);
            UpdateModifiedDate();
        }

        public void RemoveEstimate(string floorName, string roomName, string category)
        {
            ValidateProjectExists();
            var estimate = GetEstimate(floorName, roomName, category);
            var room = GetRoom(floorName, roomName);
            room.Estimates.Remove(estimate);
            UpdateModifiedDate();
        }

        public IEnumerable<string> GetFloorNames()
        {
            ValidateProjectExists();
            return _projectManager.CurrentProject.Floors.Select(f => f.Name).ToList();
        }

        public IEnumerable<string> GetRoomNames(string floorName)
        {
            ValidateProjectExists();
            var floor = GetFloor(floorName);
            return floor.Rooms.Select(r => r.Name).ToList();
        }

        #region Private Helpers
        private Floor GetFloor(string name)
        {
            var floor = _projectManager.CurrentProject.Floors
                .FirstOrDefault(f => string.Equals(f.Name, name, StringComparison.OrdinalIgnoreCase));
            return floor ?? throw new ArgumentException($"Floor '{name}' not found");
        }

        private Room GetRoom(string floorName, string roomName)
        {
            var floor = GetFloor(floorName);
            var room = floor.Rooms
                .FirstOrDefault(r => string.Equals(r.Name, roomName, StringComparison.OrdinalIgnoreCase));
            return room ?? throw new ArgumentException($"Room '{roomName}' not found on floor '{floorName}'");
        }

        private Estimate GetEstimate(string floorName, string roomName, string category)
        {
            var room = GetRoom(floorName, roomName);
            var estimate = room.Estimates
                .FirstOrDefault(e => string.Equals(e.Category, category, StringComparison.OrdinalIgnoreCase));
            return estimate ?? throw new ArgumentException($"Estimate '{category}' not found in room '{roomName}'");
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