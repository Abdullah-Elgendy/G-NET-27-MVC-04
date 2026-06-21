using AutoMapper;
using GymManagement.BLL.Services.Interfaces;
using GymManagement.BLL.ViewModels.PlanViewModels;
using GymManagement.DAL.Data.Models;
using GymManagement.DAL.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;


namespace GymManagement.BLL.Services.Classes
{
    public class PlansService : IPlansService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public PlansService(IUnitOfWork unitOfWork, IMapper mapper)
        {

            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<PlanViewModel>> GetAllPlansAsync(CancellationToken ct = default)
        {
            var plans = await _unitOfWork.GetRepository<Plan>().GetAllAsync(false, ct);
            if (plans is null) return [];

            var plansModel = _mapper.Map<IEnumerable<Plan>, IEnumerable<PlanViewModel>>(plans);

            return plansModel;
        }

        public async Task<PlanDetailsViewModel?> GetPlanDetailsAsync(int id, CancellationToken ct = default)
        {
            var plan = await _unitOfWork.GetRepository<Plan>().GetByIdAsync(id, ct);
            if (plan is null) return null;

            var model = _mapper.Map<Plan, PlanDetailsViewModel>(plan);

            return model;
        }

        public async Task<PlanToUpdateViewModel?> GetPlanToUpdateAsync(int id, CancellationToken ct = default)
        {
            var plan = await _unitOfWork.GetRepository<Plan>().GetByIdAsync(id, ct);
            if (plan is null || !plan.IsActive) return null;
            if (await HasActiveMembershipsAsync(id, ct)) return null;

            var model = _mapper.Map<Plan, PlanToUpdateViewModel>(plan);

            return model;
        }

        public async Task<bool> UpdatePlanAsync(int id, PlanToUpdateViewModel updatedPlan, CancellationToken ct = default)
        {
            var plan = await _unitOfWork.GetRepository<Plan>().GetByIdAsync(id, ct);
            if (plan is null || !plan.IsActive) return false;
            if (await HasActiveMembershipsAsync(id, ct)) return false;

            _mapper.Map<PlanToUpdateViewModel,Plan>(updatedPlan, plan);
            plan.UpdatedAt = DateTime.Now;

            _unitOfWork.GetRepository<Plan>().UpdateAsync(plan);
            var res = await _unitOfWork.SaveChangesAsync(ct);

            return res > 0;
        }

        public async Task<bool> ToggleStatusAsync(int id, CancellationToken ct = default)
        {
            var plan = await _unitOfWork.GetRepository<Plan>().GetByIdAsync(id, ct);
            if (plan is null) return false;
            if (plan.IsActive && await HasActiveMembershipsAsync(id, ct)) return false;

            plan.IsActive = !plan.IsActive;
            plan.UpdatedAt = DateTime.Now;

            _unitOfWork.GetRepository<Plan>().UpdateAsync(plan);
            var res = await _unitOfWork.SaveChangesAsync(ct);

            return res > 0;
        }

        public async Task<bool> HasActiveMembershipsAsync(int planId, CancellationToken ct = default)
        {
            return await _unitOfWork.GetRepository<MemberPlans>().AnyAsync(x => x.PlanId == planId && x.EndDate > DateTime.Now, ct);
        }
    }
}
