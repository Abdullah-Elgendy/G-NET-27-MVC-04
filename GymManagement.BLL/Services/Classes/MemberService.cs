using GymManagement.BLL.Services.Interfaces;
using GymManagement.BLL.ViewModels.MemberViewModels;
using GymManagement.DAL.Data.Models;
using GymManagement.DAL.Repositories.Interfaces;

namespace GymManagement.BLL.Services.Classes
{
    public class MemberService : IMemberService
    {
        private readonly IGenericRepository<Member> _memberRepo;
        public MemberService(IGenericRepository<Member> memberRepo)
        {
            _memberRepo = memberRepo;
        }
        public async Task<IEnumerable<MemberViewModel>> GetMembersAsync(CancellationToken ct)
        {
            var members = await _memberRepo.GetAllAsync(ct: ct);
            if (!members.Any()) return []; //if no members found return empty list.

            //METHOD1: using foreach, creating new MemberViewModel() object 
            //         for each member in members.

            //var membersViewModel = new List<MemberViewModel>();
            //foreach (var member in members)
            //{
            //    var memberViewModel = new MemberViewModel()
            //    {
            //        Name = member.Name,
            //        Email = member.Email,
            //        Gender = member.Gender.ToString(),
            //        Phone = member.Phone,
            //        Photo = member.Photo,
            //        id = member.Id
            //    };
            //    membersViewModel.Add(memberViewModel);
            //}


            //METHOD2: using LINQ Select to project each member into MemberViewModel object
            //         more readable and cleaner.

            var membersViewModel = members.Select(
                x => new MemberViewModel()
            {
                Name = x.Name,
                Email = x.Email,
                Gender = x.Gender.ToString(),
                Phone = x.Phone,
                Photo = x.Photo,
                id = x.Id
            });

            return membersViewModel;
        }

        public async Task<bool> CreateMemberAsync(CreateMemberViewModel model, CancellationToken ct = default)
        {
            #region Logic  Create Member To Database
            //Check Email Unique


            //Checking Phone Number Unique
            #endregion



            //Casting/Mapping from CreateMemberViewModel To Member

            //Add To Database
            return true;
        }
    }
}
