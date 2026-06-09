using GymManagement.BLL.Services.Interfaces;
using GymManagement.BLL.ViewModels.PlanViewModels;
using GymManagement.DAL.Data.Models;
using GymManagement.DAL.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;


namespace GymManagement.BLL.Services.Classes
{
    public class PlansService : IPlansService
    {
        private readonly IGenericRepository<Plan> _planRepo;
        private readonly IGenericRepository<MemberPlans> _membershipRepo;

        public PlansService(IGenericRepository<Plan> planRepo, IGenericRepository<MemberPlans> membershipRepo)
        {
            _planRepo = planRepo;
            _membershipRepo = membershipRepo;
        }

        public async Task<IEnumerable<PlanViewModel>> GetAllPlansAsync(CancellationToken ct = default)
        {
            var plans = await _planRepo.GetAllAsync(false, ct);
            if (plans is null) return [];

            var plansModel = plans.Select(x => new PlanViewModel()
            {
                id = x.Id,
                name = x.Name,
                price = x.Price,
                duration = x.DurationDays,
                description = x.Description,
                isActive = x.IsActive
            }).ToList();

            return plansModel;
        }

        public async Task<PlanDetailsViewModel?> GetPlanDetailsAsync(int id, CancellationToken ct = default)
        {
            var plan = await _planRepo.GetByIdAsync(id, ct);
            if (plan is null) return null;

            var model = new PlanDetailsViewModel()
            {
                name = plan.Name,
                price = plan.Price,
                duration = plan.DurationDays,
                description = plan.Description,
                isActive = plan.IsActive
            };

            return model;
        }

        public async Task<PlanToUpdateViewModel?> GetPlanToUpdateAsync(int id, CancellationToken ct = default)
        {
            var plan = await _planRepo.GetByIdAsync(id, ct);
            if (plan is null || !plan.IsActive) return null;
            if (await HasActiveMembershipsAsync(id, ct)) return null;

            var model = new PlanToUpdateViewModel()
            {
                name = plan.Name,
                price = plan.Price,
                duration = plan.DurationDays,
                description = plan.Description
            };

            return model;
        }

        public async Task<bool> UpdatePlanAsync(int id, PlanToUpdateViewModel updatedPlan, CancellationToken ct = default)
        {
            var plan = await _planRepo.GetByIdAsync(id, ct);
            if (plan is null || !plan.IsActive) return false;
            if (await HasActiveMembershipsAsync(id, ct)) return false;

            plan.Price = updatedPlan.price;
            plan.DurationDays = updatedPlan.duration;
            plan.Description = updatedPlan.description;
            plan.UpdatedAt = DateTime.Now;

            var res = await _planRepo.UpdateAsync(plan, ct);

            return res > 0;
        }

        public async Task<bool> ToggleStatusAsync(int id, CancellationToken ct = default)
        {
            var plan = await _planRepo.GetByIdAsync(id, ct);
            if (plan is null) return false;
            if (plan.IsActive && await HasActiveMembershipsAsync(id, ct)) return false;

            plan.IsActive = !plan.IsActive;
            plan.UpdatedAt = DateTime.Now;

            var res = await _planRepo.UpdateAsync(plan, ct);

            return res > 0;
        }

        public async Task<bool> HasActiveMembershipsAsync(int planId, CancellationToken ct = default)
        {
            return await _membershipRepo.AnyAsync(x => x.PlanId == planId && x.EndDate > DateTime.Now, ct);
        }
    }
}
