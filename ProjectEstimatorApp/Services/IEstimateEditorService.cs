using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectEstimatorApp.Models;

namespace ProjectEstimatorApp.Services
{
    public interface IEstimateEditorService
    {
        void SetCurrentItem(object item);
        void AddWorkItem(EstimateItem item);
        void AddMaterialItem(EstimateItem item);
        void RemoveWorkItem(int index);
        void RemoveMaterialItem(int index);
        decimal CalculateWorksTotal();
        decimal CalculateMaterialsTotal();
        decimal CalculateEstimateTotal();
        List<EstimateItem> GetCurrentWorks();
        List<EstimateItem> GetCurrentMaterials();
    }
}
