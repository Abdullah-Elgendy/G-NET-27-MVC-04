using GymManagement.BLL.ViewModels.PlanViewModels;
using GymManagement.DAL.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagement.BLL.Services.Interfaces
{
    public interface IPlansService
    {
        public Task<IEnumerable<PlanViewModel>> GetAllPlansAsync(CancellationToken ct = default);
        public Task<PlanDetailsViewModel?> GetPlanDetailsAsync(int id, CancellationToken ct = default);
        public Task<PlanToUpdateViewModel?> GetPlanToUpdateAsync(int id, CancellationToken ct = default);
        public Task<bool> UpdatePlanAsync(int id, PlanToUpdateViewModel updatedPlan, CancellationToken ct = default);
        public Task<bool> ToggleStatusAsync(int id, CancellationToken ct = default);

    }
}
