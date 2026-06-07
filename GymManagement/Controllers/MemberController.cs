using GymManagement.BLL.Services.Interfaces;
using GymManagement.BLL.ViewModels.MemberViewModels;
using Microsoft.AspNetCore.Mvc;

namespace GymManagement.PL.Controllers
{
    public class MemberController : Controller
    {
        private readonly IMemberService _memberService;
        public MemberController(IMemberService memberService)
        {
            _memberService = memberService;
        }

        // GET baseUrl/Members/Index
        // Index - List of all members
        public async Task<IActionResult> Index(CancellationToken ct)
        {
            var members = await _memberService.GetMembersAsync(ct);
            return View(members);
        }

        #region Member Details
        // GET baseUrl/Members/MemberDetails/{id}
        // MemberDetails - Show one member's details 
        #endregion

        #region Health Record Details
        // GET baseUrl/Members/HealthRecordDetails/{id}
        // HealthRecordDetails - show one member's details 
        #endregion

        #region Create Member
        // GET baseUrl/Members/Create
        // Create - Create and show empty form

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(CreateMemberViewModel model, CancellationToken ct)
        {
            //Add member to database
            return View();
        }

        // POST baseUrl/Members/Create {Member}
        // Create - Create Member after form submit 
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
