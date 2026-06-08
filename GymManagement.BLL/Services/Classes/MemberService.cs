using GymManagement.BLL.Services.Interfaces;
using GymManagement.BLL.ViewModels.MemberViewModels;
using GymManagement.DAL.Data.Models;
using GymManagement.DAL.Repositories.Interfaces;

namespace GymManagement.BLL.Services.Classes
{
    public class MemberService : IMemberService
    {
        private readonly IGenericRepository<Member> _memberRepo;
        private readonly IGenericRepository<MemberPlans> _memberShipRepo;
        private readonly IGenericRepository<Plan> _planRepo;

        public MemberService(IGenericRepository<Member> memberRepo, IGenericRepository<MemberPlans> memberShipRepo, IGenericRepository<Plan> planRepo)
        {
            _memberRepo = memberRepo;
            _memberShipRepo = memberShipRepo;
            _planRepo = planRepo;
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
                }).ToList();

            return membersViewModel;
        }
        public async Task<bool> CreateMemberAsync(CreateMemberViewModel member, CancellationToken ct = default)
        {
            //Logic  Create Member To Database

            //Check Email Unique
            var emailExists = await _memberRepo.AnyAsync(x => x.Email == member.Email, ct);
            //Checking Phone Number Unique
            var phoneExists = await _memberRepo.AnyAsync(x => x.Phone == member.Phone, ct);

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

        public async Task<MemberViewModel?> GetMemberDetails(int id, CancellationToken ct = default)
        {
            var member = await _memberRepo.GetByIdAsync(id, ct);
            if (member == null) return null;

            //cast to membervciewmodel
            var model = new MemberViewModel()
            {
                id = member.Id,
                Name = member.Name,
                Email = member.Email,
                Phone = member.Phone,
                Gender = member.Gender.ToString(),
                Photo = member.Photo,
                DateOfBirth = member.DateOfBirth.ToString(),
                Address = $"{member.Address.BuildingNo} - {member.Address.Street} - {member.Address.City}",
                
            };

            var activeMembership = await _memberShipRepo.FirstOrDefaultAsync(x => x.MemberId == member.Id && x.EndDate > DateTime.Now);
           
            if(activeMembership is not null)
            {
                var currentPlan = await _planRepo.GetByIdAsync(activeMembership.PlanId, ct);

                model.PlanName = currentPlan?.Name;
                model.MemberShipStartDate = activeMembership.CreatedAt.ToString();
                model.MemberShipEndDate = activeMembership.EndDate.ToString();
            }

            return model;
        }
    }
}
