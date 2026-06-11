using GymManagement.BLL.Services.Interfaces;
using GymManagement.BLL.ViewModels.TrainerViewModels;
using Microsoft.AspNetCore.Mvc;

namespace GymManagement.PL.Controllers
{
    public class TrainersController : Controller
    {

        private readonly ITrainersService _trainersService;
        public TrainersController(ITrainersService trainersService)
        {
            _trainersService = trainersService;
        }


        #region Get all trainers
        //GET: get all trainers 
        public async Task<IActionResult> Index(CancellationToken ct = default)
        {
            var trainers = await _trainersService.GetAllTrainersAsync();

            return View(trainers);
        }
        #endregion

        #region Create Trainer
        //GET: get form
        [HttpGet]
        public IActionResult Create(CancellationToken ct = default)
        {
            return View();
        }

        //POST: send trainer form
        [HttpPost]
        public async Task<IActionResult> Create(CreateTrainerViewModel trainer, CancellationToken ct = default)
        {
            if (!ModelState.IsValid) return View(trainer);

            var res = await _trainersService.CreateTrainerAsync(trainer, ct);

            if (res)
                TempData["SuccessMessage"] = "Trainer created successfully!";
            else
                TempData["ErrorMessage"] = "Failed to create trainer!";

            return RedirectToAction(nameof(Index), TempData);
        }

        #endregion

        #region Get trainer details
        //GET: get trainer details
        public async Task<IActionResult> Details([FromRoute] int id, CancellationToken ct = default)
        {
            var trainer = await _trainersService.GetTrainerDetailsAsync(id, ct);

            if (trainer is null)
            {
                TempData["ErrorMessage"] = "Trainer not found!";
                return RedirectToAction(nameof(Index), TempData);
            }
            else
            {
                return View(trainer);
            }
        }
        #endregion

        #region Edit trainer

        [HttpGet]
        public async Task<IActionResult> Edit([FromRoute] int id, CancellationToken ct = default)
        {
            var trainer = await _trainersService.GetTrainerToUpdateAsync(id, ct);

            if (trainer is null)
            {
                TempData["ErrorMessage"] = "Trainer not found!";
                return RedirectToAction(nameof(Index), TempData);
            }

            return View(trainer);
        }

        [HttpPost]
        public async Task<IActionResult> Edit([FromRoute] int id, TrainerToUpdateViewModel model, CancellationToken ct = default)
        {
            if (!ModelState.IsValid) return View(model);

            var res = await _trainersService.UpdateTrainerAsync(id, model, ct);

            if (res)
                TempData["SuccessMessage"] = "Trainer updated successfully";
            else
                TempData["ErrorMessage"] = "Trainer failed to update";

            return RedirectToAction(nameof(Index), TempData);
        }

        #endregion

        #region Delete trainer
        [HttpGet]
        public async Task<IActionResult> Delete([FromRoute] int id, CancellationToken ct = default)
        {
            var trainer = await _trainersService.GetTrainerDetailsAsync(id, ct);
            if (trainer is null)
            {
                TempData["ErrorMessage"] = "Trainer doesn't exist!";
                return RedirectToAction(nameof(Index), TempData);
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed([FromRoute] int id, CancellationToken ct = default)
        {
            var res = await _trainersService.RemoveTrainerAsync(id, ct);

            if (res)
                TempData["SuccessMessage"] = "Trainer removed successfully";
            else
                TempData["ErrorMessage"] = "Failed to remove trainer!";

            return RedirectToAction(nameof(Index), TempData);
        }
        #endregion
    }
}
