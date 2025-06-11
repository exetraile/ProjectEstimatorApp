using ProjectEstimatorApp.Models;
using System.Collections.Generic;
using System.Linq;

namespace ProjectEstimatorApp.Services
{
    public class EstimateEditorService : IEstimateEditorService
    {
        private object _currentItem;
        private List<EstimateItem> _currentWorks;
        private List<EstimateItem> _currentMaterials;
        public List<EstimateItem> GetCurrentWorks() => _currentWorks;
        public List<EstimateItem> GetCurrentMaterials() => _currentMaterials;

        public void SetCurrentItem(object item)
        {
            _currentItem = item;

            if (item is Estimate estimate)
            {
                _currentWorks = estimate.Works;
                _currentMaterials = estimate.Materials;
            }
            else if (item is EstimateDetail room)
            {
                _currentWorks = room.Works;
                _currentMaterials = room.Materials;
            }
            else if (item is Estimate floor)
            {
                _currentWorks = floor.Works;
                _currentMaterials = floor.Materials;
            }
            else
            {
                _currentWorks = new List<EstimateItem>();
                _currentMaterials = new List<EstimateItem>();
            }
        }

        public void AddWorkItem(EstimateItem item) => _currentWorks?.Add(item);
        public void AddMaterialItem(EstimateItem item) => _currentMaterials?.Add(item);

        public void RemoveWorkItem(int index)
        {
            if (_currentWorks != null && index >= 0 && index < _currentWorks.Count)
                _currentWorks.RemoveAt(index);
        }

        public void RemoveMaterialItem(int index)
        {
            if (_currentMaterials != null && index >= 0 && index < _currentMaterials.Count)
                _currentMaterials.RemoveAt(index);
        }

        public decimal CalculateWorksTotal() => _currentWorks?.Sum(item => item.Total) ?? 0;
        public decimal CalculateMaterialsTotal() => _currentMaterials?.Sum(item => item.Total) ?? 0;
        public decimal CalculateEstimateTotal() => CalculateWorksTotal() + CalculateMaterialsTotal();
    }
}