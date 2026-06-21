using AutoMapper;
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

        private readonly IMapper _mapper;

        public MemberService(IUnitOfWork unitOfWork, IMapper mapper)
        { 
            _unitOfWork = unitOfWork;
            _mapper = mapper;
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

            //var membersViewModel = members.Select(
            //    x => new MemberViewModel()
            //    {
            //        Name = x.Name,
            //        Email = x.Email,
            //        Gender = x.Gender.ToString(),
            //        Phone = x.Phone,
            //        Photo = x.Photo,
            //        id = x.Id
            //    }).ToList();

            //METHOD3: using auto mapper
            var membersViewModel = _mapper.Map<IEnumerable<Member>,IEnumerable<MemberViewModel>>(members);

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

            var createdMember = _mapper.Map<CreateMemberViewModel, Member>(member);

            //Add To Database
            _unitOfWork.GetRepository<Member>().AddAsync(createdMember);
            var rowsAffected = await _unitOfWork.SaveChangesAsync(ct);
            return rowsAffected > 0;
        }
        public async Task<MemberViewModel?> GetMemberDetailsAsync(int id, CancellationToken ct = default)
        {
            var member = await _unitOfWork.GetRepository<Member>().GetByIdAsync(id, ct);
            if (member == null) return null;

            var model = _mapper.Map<Member, MemberViewModel>(member);

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

            var model = _mapper.Map<HealthRecord, HealthRecordViewModel>(healthRecord);

            return model;
        }
        public async Task<MemberToUpdateViewModel?> GetMemberToUpdateAsync(int id, CancellationToken ct = default)
        {
            var member = await _unitOfWork.GetRepository<Member>().GetByIdAsync(id, ct);
            if (member is null) return null;

            var model = _mapper.Map<Member, MemberToUpdateViewModel>(member);

            return model;
        }
        public async Task<bool> UpdateMemberAsync(int id,MemberToUpdateViewModel updatedMember, CancellationToken ct)
        {
            var member = await _unitOfWork.GetRepository<Member>().GetByIdAsync(id, ct);
            if (member is null) return false;

            var emailExists = await _unitOfWork.GetRepository<Member>().AnyAsync(x => x.Email == updatedMember.Email && x.Id != id, ct);
            var phoneExists = await _unitOfWork.GetRepository<Member>().AnyAsync(x => x.Phone == updatedMember.Phone && x.Id != id, ct);

            if (emailExists || phoneExists) return false;

            _mapper.Map(updatedMember, member);
            member.UpdatedAt = DateTime.Now;

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
