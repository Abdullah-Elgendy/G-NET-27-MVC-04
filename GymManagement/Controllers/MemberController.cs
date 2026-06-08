using GymManagement.BLL.Services.Interfaces;
using GymManagement.BLL.ViewModels.MemberViewModels;
using Microsoft.AspNetCore.Mvc;

namespace GymManagement.PL.Controllers
{
    public class MemberController : Controller
    {
        #region Constructor and Services
        private readonly IMemberService _memberService;
        public MemberController(IMemberService memberService)
        {
            _memberService = memberService;
        } 
        #endregion

        //===============================================================================

        #region Get All Members
        // GET baseUrl/Members/Index
        // Index - List of all members
        public async Task<IActionResult> Index(CancellationToken ct)
        {
            var members = await _memberService.GetMembersAsync(ct);
            return View(members);
        } 
        #endregion 

        #region Create Member
        // GET baseUrl/Members/Create
        // Create - Create and show empty form

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // POST baseUrl/Members/Create {Member}
        // Create - Create Member after form submit 
        [HttpPost]
        public async Task<IActionResult> Create(CreateMemberViewModel model, CancellationToken ct)
        {
            if (!ModelState.IsValid) return View(nameof(Create), model);

            var res = await _memberService.CreateMemberAsync(model, ct);

            if (res)
                TempData["SuccessMessage"] = "Member Created Successfully";
            else
                TempData["ErrorMessage"] = "Failed To Create Member";


            return RedirectToAction(nameof(Index), TempData);
        }
        #endregion

        #region Member Details
        // GET baseUrl/Members/MemberDetails/{id}
        // MemberDetails - Show one member's details 
        public async Task<IActionResult> MemberDetails(int id, CancellationToken ct = default)
        {
            var member = await _memberService.GetMemberDetails(id, ct);

            if (member is null)
            {
                TempData["ErrorMessage"] = "Member Not Found!";
                RedirectToAction(nameof(Index), TempData);
            }
            return View(member);
        }
        #endregion

        #region Health Record Details
        // GET baseUrl/Members/HealthRecordDetails/{id}
        // HealthRecordDetails - show one member's details 
        #endregion

        #region Edit Member
        // GET baseUrl/Members/Edit/{id}
        // Edit - Create and show pre-filled form for edit

        // POST baseUrl/Members/Edit {EditedMember}
        // Edit - Update Member after form submit 
        #endregion

        #region Delete Member
        // GET baseUrl/Members/Delete/{id}
        // Delete - Show Confirmation Form

        // POST baseUrl/Members/Delete {Member}
        // DeleteConfirmed - Submit form for delete
        #endregion

    }
}
