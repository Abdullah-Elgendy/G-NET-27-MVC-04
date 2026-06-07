using GymManagement.DAL.Repositories.Classes;
using GymManagement.DAL.Repositories.Interfaces;
using GymManagement.DAL.Data.DbContexts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GymManagement.DAL.Data.Models;

namespace GymManagement.Controllers
{
    
    public class PlansController : Controller
    {
        private readonly IGenericRepository<Plan> _planRepo;
        
        public PlansController(IGenericRepository<Plan> planRepo)
        {
            _planRepo = planRepo;
        }

        //Index
        // GET: BaseUrl/Plans/Index -> Index view -> List of all plans
        public async Task<IActionResult> Index(CancellationToken ct)
        {
            var plans = await _planRepo.GetAllAsync(false, ct);

            return View(plans);
        }


        // Details
        // GET: BaseUrl/Plans/Details/{Id}-> Detail view -> Details about plan
        public async Task<IActionResult> Details(int Id, CancellationToken ct)
        {
            var plan = await _planRepo.GetByIdAsync(Id, ct);

            if (plan is null)
            return RedirectToAction(nameof(Index));
            else
            return View(plan);
        }
    }
}
