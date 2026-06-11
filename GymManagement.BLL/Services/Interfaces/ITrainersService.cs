using GymManagement.BLL.ViewModels.TrainerViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagement.BLL.Services.Interfaces
{
    public interface ITrainersService
    {
        public Task<IEnumerable<TrainerViewModel>> GetAllTrainersAsync(CancellationToken ct = default);
        public Task<TrainerDetailsViewModel?> GetTrainerDetailsAsync(int id, CancellationToken ct = default);
        public Task<bool> CreateTrainerAsync(CreateTrainerViewModel trainer, CancellationToken ct = default);
        public Task<TrainerToUpdateViewModel?> GetTrainerToUpdateAsync(int id, CancellationToken ct = default);
        public Task<bool> UpdateTrainerAsync(int id, TrainerToUpdateViewModel model, CancellationToken ct = default);
        public Task<bool> RemoveTrainerAsync(int id, CancellationToken ct = default);

    }
}
