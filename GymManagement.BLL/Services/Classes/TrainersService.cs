using GymManagement.BLL.Services.Interfaces;
using GymManagement.BLL.ViewModels.TrainerViewModels;
using GymManagement.DAL.Data.Models;
using GymManagement.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace GymManagement.BLL.Services.Classes
{
    public class TrainersService : ITrainersService
    {
        private readonly IGenericRepository<Trainer> _trainerRepo;
        private readonly IGenericRepository<Session> _sessionRepo;

        public TrainersService(IGenericRepository<Trainer> trainerRepo, IGenericRepository<Session> sessionRepo)
        {
            _trainerRepo = trainerRepo;
            _sessionRepo = sessionRepo;
        }

        public async Task<IEnumerable<TrainerViewModel>> GetAllTrainersAsync(CancellationToken ct = default)
        {
            var trainers = await _trainerRepo.GetAllAsync();
            if (trainers is null) return [];

            return trainers.Select(x => new TrainerViewModel()
            {
                Id = x.Id,
                Name = x.Name,
                Email = x.Email,
                Phone = x.Phone,
                Specialization = x.Speciality.ToString()
            });

        }

        public async Task<TrainerDetailsViewModel?> GetTrainerDetailsAsync(int id, CancellationToken ct = default)
        {
            var trainer = await _trainerRepo.GetByIdAsync(id, ct);
            if (trainer is null) return null;

            return new TrainerDetailsViewModel()
            {
                Name = trainer.Name,
                Phone = trainer.Phone,
                Email = trainer.Email,
                Specialization = trainer.Speciality.ToString(),
                DateOfBirth = trainer.DateOfBirth.ToString(),
                Address = $"{trainer.Address.BuildingNo} - {trainer.Address.Street} - {trainer.Address.City}"
            };
        }

        public async Task<bool> CreateTrainerAsync(CreateTrainerViewModel trainer, CancellationToken ct = default)
        {
            var emailExists = await _trainerRepo.AnyAsync(x => x.Email == trainer.Email);
            var phoneExists = await _trainerRepo.AnyAsync(x => x.Phone == trainer.Phone);

            if (emailExists || phoneExists) return false;

            var createdTrainer = new Trainer()
            {
                Name = trainer.Name,
                Email = trainer.Email,
                Phone = trainer.Phone,
                DateOfBirth = trainer.DateOfBirth,
                Gender = trainer.Gender,
                Address = new Address() { BuildingNo = trainer.BuildingNumber, Street = trainer.Street, City = trainer.City },
                Speciality = trainer.Speciality
            };

            var res = await _trainerRepo.AddAsync(createdTrainer, ct);

            return res > 0;
        }

        public async Task<TrainerToUpdateViewModel?> GetTrainerToUpdateAsync(int id, CancellationToken ct = default)
        {
            var trainer = await _trainerRepo.GetByIdAsync(id, ct);
            if (trainer is null) return null;

            return new TrainerToUpdateViewModel()
            {
                Name = trainer.Name,
                Email = trainer.Email,
                Phone = trainer.Phone,
                DateOfBirth = trainer.DateOfBirth,
                Speciality = trainer.Speciality,
                BuildingNumber = trainer.Address.BuildingNo,
                Street = trainer.Address.Street,
                City = trainer.Address.City,
                Gender = trainer.Gender
            };
        }

        public async Task<bool> UpdateTrainerAsync(int id,TrainerToUpdateViewModel model, CancellationToken ct = default)
        {
            var trainer = await _trainerRepo.GetByIdAsync(id, ct);
            if (trainer is null) return false;

            var emailExists = await _trainerRepo.AnyAsync(x => x.Email == model.Email && x.Id != id);
            var phoneExists = await _trainerRepo.AnyAsync(x => x.Phone == model.Phone && x.Id != id);

            if (emailExists || phoneExists) return false;

            trainer.Email = model.Email;
            trainer.Phone = model.Phone;
            trainer.Address.BuildingNo = model.BuildingNumber;
            trainer.Address.Street = model.Street;
            trainer.Address.City = model.City;
            trainer.Speciality = model.Speciality;

            var res = await _trainerRepo.UpdateAsync(trainer, ct);

            return res > 0;
        }

        public async Task<bool> RemoveTrainerAsync(int id, CancellationToken ct = default)
        {
            var trainer = await _trainerRepo.GetByIdAsync(id, ct);
            var assigned = await _sessionRepo.AnyAsync(x => x.TrainerId == id);
            if (trainer is null || assigned) return false;

            var res = await _trainerRepo.DeleteAsync(trainer);

            return res > 0;
        }
    }
}
