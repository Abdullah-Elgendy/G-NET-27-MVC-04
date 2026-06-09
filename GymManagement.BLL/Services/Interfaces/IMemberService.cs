using GymManagement.BLL.ViewModels.HealthRecordViewModels;
using GymManagement.BLL.ViewModels.MemberViewModels;
using GymManagement.DAL.Data.Models;

namespace GymManagement.BLL.Services.Interfaces
{
    public interface IMemberService
    {
        public Task<IEnumerable<MemberViewModel>> GetMembersAsync(CancellationToken ct = default);
        public Task<bool> CreateMemberAsync(CreateMemberViewModel member, CancellationToken ct = default);
        public Task<MemberViewModel?> GetMemberDetailsAsync(int id, CancellationToken ct = default);
        public Task<HealthRecordViewModel?> GetMemberHealthRecordAsync(int id, CancellationToken ct = default);
        public Task<MemberToUpdateViewModel?> GetMemberToUpdateAsync(int id, CancellationToken ct = default);
        public Task<bool> UpdateMemberAsync(int id, MemberToUpdateViewModel updatedMember, CancellationToken ct);
        public Task<bool> RemoveMemberAsync(int id, CancellationToken ct = default);
    }
}
