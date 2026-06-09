using GymManagement.DAL.Repositories.Classes;
using GymManagement.DAL.Repositories.Interfaces;
using GymManagement.DAL.Data.DbContexts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GymManagement.DAL.Data.Models;
using GymManagement.BLL.Services.Interfaces;
using GymManagement.BLL.ViewModels.PlanViewModels;

namespace GymManagement.Controllers
{
    
    public class PlansController : Controller
    {
        private readonly IPlansService _plansService;
        
        public PlansController(IPlansService plansService)
        {
            _plansService = plansService;
        }


        #region Get All Plans
        //Index
        // GET: BaseUrl/Plans/Index -> Index view -> List of all plans
        public async Task<IActionResult> Index(CancellationToken ct)
        {
            var plans = await _plansService.GetAllPlansAsync();

            return View(plans);
        }
        #endregion

        #region Get Plan Details
        // Details
        // GET: BaseUrl/Plans/Details/{Id}-> Detail view -> Details about plan
        public async Task<IActionResult> Details(int Id, CancellationToken ct)
        {
            var plan = await _plansService.GetPlanDetailsAsync(Id, ct);

            if (plan is null)
            {
                TempData["ErrorMessage"] = "Plan does not exist!";
                return RedirectToAction(nameof(Index), TempData);
            }
                
            else
                return View(plan);
        }
        #endregion

        #region Edit Plan
        //Edit
        //GET: get a view for the editing page
        [HttpGet]
        public async Task<IActionResult> Edit([FromRoute] int id, CancellationToken ct = default)
        {
            var plan = await _plansService.GetPlanToUpdateAsync(id, ct);
            if (plan is null)
            {
                TempData["ErrorMessage"] = "Cannot edit plan!";
                return RedirectToAction(nameof(Index), TempData);
            }

            return View(plan);
        }

        //Edit
        //POST: send updated plan form and save to database
        [HttpPost]
        public async Task<IActionResult> Edit([FromRoute] int id, PlanToUpdateViewModel updatedPlan, CancellationToken ct = default)
        {
            if (!ModelState.IsValid) return RedirectToAction(nameof(Index));

            var res = await _plansService.UpdatePlanAsync(id, updatedPlan, ct);
            if(res)
            {
                TempData["SuccessMessage"] = "Plan updated successfully!";
               
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to update plan!";
            }

            return RedirectToAction(nameof(Index), TempData);
        }
        #endregion

        #region Toggle Status
        //ActivateToggle
        //POST: toggle the isActive flag

        [HttpPost]
        public async Task<IActionResult> ActivateToggle(int id, CancellationToken ct = default)
        {
            var res = await _plansService.ToggleStatusAsync(id, ct);

            if (res)
            {
                TempData["SuccessMessage"] = "Plan status toggled successfully!";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to toggle plan status!";
                
            }


            return RedirectToAction(nameof(Index), TempData);
        }
        #endregion
    }
}
