using AutoMapper;
using GymManagement.BLL.Services.Interfaces;
using GymManagement.BLL.ViewModels.TrainerViewModels;
using GymManagement.DAL.Data.Models;
using GymManagement.DAL.Repositories.Interfaces;

namespace GymManagement.BLL.Services.Classes
{
    public class TrainersService : ITrainersService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public TrainersService(IUnitOfWork unitOfWork, IMapper mapper)
        {

            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<TrainerViewModel>> GetAllTrainersAsync(CancellationToken ct = default)
        {
            var trainers = await _unitOfWork.GetRepository<Trainer>().GetAllAsync();
            if (trainers is null) return [];

            return _mapper.Map<IEnumerable<Trainer>,IEnumerable<TrainerViewModel>>(trainers);

        }

        public async Task<TrainerDetailsViewModel?> GetTrainerDetailsAsync(int id, CancellationToken ct = default)
        {
            var trainer = await _unitOfWork.GetRepository<Trainer>().GetByIdAsync(id, ct);
            if (trainer is null) return null;

            return _mapper.Map<Trainer, TrainerDetailsViewModel>(trainer);
        }

        public async Task<bool> CreateTrainerAsync(CreateTrainerViewModel trainer, CancellationToken ct = default)
        {
            var emailExists = await _unitOfWork.GetRepository<Trainer>().AnyAsync(x => x.Email == trainer.Email);
            var phoneExists = await _unitOfWork.GetRepository<Trainer>().AnyAsync(x => x.Phone == trainer.Phone);

            if (emailExists || phoneExists) return false;

            var createdTrainer = _mapper.Map<CreateTrainerViewModel, Trainer>(trainer);

            _unitOfWork.GetRepository<Trainer>().AddAsync(createdTrainer);
            var res = await _unitOfWork.SaveChangesAsync(ct);
            return res > 0;
        }

        public async Task<TrainerToUpdateViewModel?> GetTrainerToUpdateAsync(int id, CancellationToken ct = default)
        {
            var trainer = await _unitOfWork.GetRepository<Trainer>().GetByIdAsync(id, ct);
            if (trainer is null) return null;

            return _mapper.Map<Trainer, TrainerToUpdateViewModel>(trainer);
        }

        public async Task<bool> UpdateTrainerAsync(int id, TrainerToUpdateViewModel model, CancellationToken ct = default)
        {
            var trainer = await _unitOfWork.GetRepository<Trainer>().GetByIdAsync(id, ct);
            if (trainer is null) return false;

            var emailExists = await _unitOfWork.GetRepository<Trainer>().AnyAsync(x => x.Email == model.Email && x.Id != id);
            var phoneExists = await _unitOfWork.GetRepository<Trainer>().AnyAsync(x => x.Phone == model.Phone && x.Id != id);

            if (emailExists || phoneExists) return false;

            _mapper.Map(model, trainer);

            _unitOfWork.GetRepository<Trainer>().UpdateAsync(trainer);
            var res = await _unitOfWork.SaveChangesAsync(ct);

            return res > 0;
        }

        public async Task<bool> RemoveTrainerAsync(int id, CancellationToken ct = default)
        {
            var trainer = await _unitOfWork.GetRepository<Trainer>().GetByIdAsync(id, ct);
            var assigned = await _unitOfWork.GetRepository<Session>().AnyAsync(x => x.TrainerId == id);
            if (trainer is null || assigned) return false;

            _unitOfWork.GetRepository<Trainer>().DeleteAsync(trainer);
            var res = await _unitOfWork.SaveChangesAsync(ct);
            return res > 0;
        }
    }
}
