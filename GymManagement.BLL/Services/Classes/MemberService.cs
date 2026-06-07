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
        public async Task<bool> CreateMemberAsync(CreateMemberViewModel member, CancellationToken ct = default)
        {
            //Logic  Create Member To Database

            //Check Email Unique
            bool emailExists = await _memberRepo.AnyAsync(x => x.Email == member.Email);
            //Checking Phone Number Unique
            bool phoneExists = await _memberRepo.AnyAsync(x => x.Phone == member.Phone);

            if (emailExists || phoneExists) return false;


            //Casting/Mapping from CreateMemberViewModel To Member
            var createdMember = new Member
            {
                Name = member.Name,
                Email = member.Email,
                Phone = member.Phone,
                Gender = member.Gender,
                DateOfBirth = member.DateOfBirth,
                Address = new Address
                {
                    City = member.City,
                    Street = member.Street,
                    BuildingNo = member.BuildingNumber
                },
                HealthRecord = new HealthRecord
                {
                    BloodType = member.HealthRecordViewModel.BloodType,
                    Weight = member.HealthRecordViewModel.Weight,
                    Height = member.HealthRecordViewModel.Height,
                    Note = member.HealthRecordViewModel.Note,
                }
            };

            //Add To Database
            var rowsAffected = await _memberRepo.AddAsync(createdMember);

            return rowsAffected > 0;
        }

    }
}
