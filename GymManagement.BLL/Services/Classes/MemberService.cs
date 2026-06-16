using GymManagement.BLL.Services.Interfaces;
using GymManagement.BLL.ViewModels.HealthRecordViewModels;
using GymManagement.BLL.ViewModels.MemberViewModels;
using GymManagement.DAL.Data.Models;
using GymManagement.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace GymManagement.BLL.Services.Classes
{
    public class MemberService : IMemberService
    {
        private readonly IUnitOfWork _unitOfWork;

        public MemberService(IUnitOfWork unitOfWork)
        { 
            _unitOfWork = unitOfWork;
        }
        public async Task<IEnumerable<MemberViewModel>> GetMembersAsync(CancellationToken ct)
        {
            var members = await _unitOfWork.GetRepository<Member>().GetAllAsync(ct: ct);
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
            var emailExists = await _unitOfWork.GetRepository<Member>().AnyAsync(x => x.Email == member.Email, ct);
            //Checking Phone Number Unique
            var phoneExists = await _unitOfWork.GetRepository<Member>().AnyAsync(x => x.Phone == member.Phone, ct);

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
            _unitOfWork.GetRepository<Member>().AddAsync(createdMember);
            var rowsAffected = await _unitOfWork.SaveChangesAsync(ct);
            return rowsAffected > 0;
        }
        public async Task<MemberViewModel?> GetMemberDetailsAsync(int id, CancellationToken ct = default)
        {
            var member = await _unitOfWork.GetRepository<Member>().GetByIdAsync(id, ct);
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

            var activeMembership = await _unitOfWork.GetRepository<MemberPlans>().FirstOrDefaultAsync(x => x.MemberId == member.Id && x.EndDate > DateTime.Now);
           
            if(activeMembership is not null)
            {
                var currentPlan = await _unitOfWork.GetRepository<Plan>().GetByIdAsync(activeMembership.PlanId, ct);

                model.PlanName = currentPlan?.Name;
                model.MemberShipStartDate = activeMembership.CreatedAt.ToString();
                model.MemberShipEndDate = activeMembership.EndDate.ToString();
            }

            return model;
        }
        public async Task<HealthRecordViewModel?> GetMemberHealthRecordAsync(int id, CancellationToken ct = default)
        {
            var healthRecord = await _unitOfWork.GetRepository<HealthRecord>().FirstOrDefaultAsync(x => x.MemberId == id, ct: ct);
            if (healthRecord is null) return null;

            var model = new HealthRecordViewModel()
            {
                Height = healthRecord.Height,
                Weight = healthRecord.Weight,
                BloodType = healthRecord.BloodType,
                Note = healthRecord.Note
            };

            return model;
        }
        public async Task<MemberToUpdateViewModel?> GetMemberToUpdateAsync(int id, CancellationToken ct = default)
        {
            var member = await _unitOfWork.GetRepository<Member>().GetByIdAsync(id, ct);
            if (member is null) return null;

            var model = new MemberToUpdateViewModel()
            {
                Name = member.Name,
                Phone = member.Phone,
                Street = member.Address.Street,
                BuildingNumber = member.Address.BuildingNo,
                City = member.Address.City,
                Email = member.Email,
                Photo = member.Photo
            };

            return model;
        }
        public async Task<bool> UpdateMemberAsync(int id,MemberToUpdateViewModel updatedMember, CancellationToken ct)
        {
            var member = await _unitOfWork.GetRepository<Member>().GetByIdAsync(id, ct);
            if (member is null) return false;

            var emailExists = await _unitOfWork.GetRepository<Member>().AnyAsync(x => x.Email == updatedMember.Email && x.Id != id, ct);
            var phoneExists = await _unitOfWork.GetRepository<Member>().AnyAsync(x => x.Phone == updatedMember.Phone && x.Id != id, ct);

            if (emailExists || phoneExists) return false;

            member.UpdatedAt = DateTime.Now;
            member.Email = updatedMember.Email;
            member.Phone = updatedMember.Phone;
            member.Address.BuildingNo = updatedMember.BuildingNumber;
            member.Address.City = updatedMember.City;
            member.Address.Street = updatedMember.Street;

            _unitOfWork.GetRepository<Member>().UpdateAsync(member);
            var res = await _unitOfWork.SaveChangesAsync(ct);

            return res > 0;
        }
        public async Task<bool> RemoveMemberAsync(int id, CancellationToken ct = default)
        {
            var member = await _unitOfWork.GetRepository<Member>().GetByIdAsync(id, ct);
            if (member == null) return false;

            var hasFutureBooking = await _unitOfWork.GetRepository<MemberSessions>().AnyAsync(x => x.MemberId == id && x.Session.StartDate > DateTime.Now);
            if (hasFutureBooking) return false;

            _unitOfWork.GetRepository<Member>().DeleteAsync(member);
            var res = await _unitOfWork.SaveChangesAsync(ct);

            return res > 0;
        }

    }
}
