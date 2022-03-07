using AutoMapper;
using Microsoft.Extensions.Logging;
using RallyDakar.CustomExceptions;
using RallyDakar.Data;
using RallyDakar.IRepository;
using RallyDakar.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using static RallyDakar.Helpers.Enums;

namespace RallyDakar.Services
{
	public class VehicleService : IVehicleService
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly ILogger<VehicleService> _logger;
		private readonly IMapper _mapper;

		public VehicleService(IUnitOfWork unitOfWork, ILogger<VehicleService> logger, IMapper mapper)
		{
			_unitOfWork = unitOfWork;
			_logger = logger;
			_mapper = mapper;
		}

		public async Task<VehicleDTO> GetVehicleById(int Id)
		{
			var vehicle = await _unitOfWork.Vehicles.Get(v => v.Id == Id);
			var result = _mapper.Map<VehicleDTO>(vehicle);
			return result;
		}

		public async Task<IList<VehicleDTO>> GetVehicles()
		{
			var vehicles = await _unitOfWork.Vehicles.GetAll();
			var results = _mapper.Map<IList<VehicleDTO>>(vehicles);
			return results;
		}

		public async Task<VehicleDTO> AddVehicle(VehicleCreateUpdateDTO vehicleDTO)
		{
			int raceId = vehicleDTO.RaceId;

			await ValidateAction(raceId);

			var vehicle = _mapper.Map<Vehicle>(vehicleDTO);
			vehicle?.SetStaticData();
			await _unitOfWork.Vehicles.Insert(vehicle);
			await _unitOfWork.Save();
			var vDTO = _mapper.Map<VehicleDTO>(vehicle);
			return vDTO;
		}

		public async Task ValidateAction(int raceId)
		{
			var race = await _unitOfWork.Races.Get(q => q.Id == raceId);
			if (race == null)
			{
				InvalidStateException ex = new InvalidStateException($"[{nameof(VehicleService)}] Race with id: {raceId} does not exist.");
				_logger.LogError(ex, $"Invalid inputs");
				throw ex;
			}
			else if (race.RaceStatus != RaceStatus.Pending)
			{
				InvalidStateException ex = new InvalidStateException($"[{nameof(VehicleService)}] Race with id: {raceId} has to be in Pending state for this action.");
				_logger.LogError(ex, $"Invalid inputs");
				throw ex;
			}

		}

		public async Task UpdateVehicle(int Id, VehicleCreateUpdateDTO vehicleDTO)
		{

			var vehicle = await _unitOfWork.Vehicles.Get(q => q.Id == Id);
			if (vehicle == null)
			{
				EntityNotFoundException ex = new EntityNotFoundException($"[{nameof(VehicleService)}] Vehicle with Id:{Id} does not exist.");
				_logger.LogError(ex, $"Invalid inputs");
				throw ex;
			}

			int raceId = vehicleDTO.RaceId;
			await ValidateAction(raceId);

			_mapper.Map(vehicleDTO, vehicle);
			vehicle?.SetStaticData();
			_unitOfWork.Vehicles.Update(vehicle);
			await _unitOfWork.Save();
		}

		public async Task DeleteVehicle(int Id)
		{
			var vehicle = await _unitOfWork.Vehicles.Get(q => q.Id == Id);
			if (vehicle == null)
			{
				EntityNotFoundException ex = new EntityNotFoundException($"[{nameof(VehicleService)}] Vehicle with Id:{Id} does not exist.");
				_logger.LogError(ex, $"Invalid inputs");
				throw ex;
			}

			int raceId = vehicle.RaceId;
			var race = await _unitOfWork.Races.Get(q => q.Id == raceId);
			if(race.RaceStatus != RaceStatus.Pending)
			{
				InvalidStateException ex = new InvalidStateException($"[{nameof(VehicleService)}] Race with id: {raceId} has to be in Pending state for this action.");
				_logger.LogError(ex, $"Invalid inputs");
				throw ex;
			}

			await _unitOfWork.Vehicles.Delete(Id);
			await _unitOfWork.Save();
		}

		public async Task<IList<VehicleLBDataDTO>> GetLeaderboardAll(int raceId)
		{
			var vehicles = await _unitOfWork.Vehicles.GetAll(q => q.RaceId == raceId);
			var vehicleDTOs = _mapper.Map<IList<VehicleLBDataDTO>>(vehicles);
			var results = vehicleDTOs.OrderByDescending(v => v.PassedDistance).ThenBy(v => v.FinishTime).ToList();
			return results;
		}

		public async Task<IList<VehicleLBDataDTO>> GetLeaderboardByType(int raceId, VehicleType vehicleType)
		{
			var vehicles = await _unitOfWork.Vehicles.GetAll(q => q.RaceId == raceId && q.Type == vehicleType);
			var vehicleDTOs = _mapper.Map<IList<VehicleLBDataDTO>>(vehicles);
			var results = vehicleDTOs.OrderByDescending(v => v.PassedDistance).ThenBy(v => v.FinishTime).ToList();
			return results;
		}

		public async Task<VehicleStatisticsDTO> GetVehicleStatistics(int Id)
		{
			if (Id < 1)
			{
				InvalidStateException ex = new InvalidStateException($"[{nameof(VehicleService)}] Vehicle ID cannot be less than one.");
				_logger.LogError(ex, $"Invalid inputs");
				throw ex;
			}

			var vehicle = await _unitOfWork.Vehicles.Get(q => q.Id == Id);
			if (vehicle == null)
			{
				EntityNotFoundException ex = new EntityNotFoundException($"[{nameof(VehicleService)}] Vehicle with Id:{Id} does not exist.");
				_logger.LogError(ex, $"Invalid inputs");
				throw ex;
			}
			else if (vehicle.Status == VehicleStatus.Running || vehicle.Status == VehicleStatus.Pending)
			{
				InvalidStateException ex = new InvalidStateException($"[{nameof(VehicleService)}] Vehicle has not finished the race yet.");
				_logger.LogError(ex, $"Invalid inputs");
				throw ex;
			}

			var result = _mapper.Map<VehicleStatisticsDTO>(vehicle);
			return result;
		}

	}
}
