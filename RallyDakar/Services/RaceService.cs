using AutoMapper;
using Microsoft.Extensions.Logging;
using RallyDakar.CustomExceptions;
using RallyDakar.Data;
using RallyDakar.Helpers;
using RallyDakar.IRepository;
using RallyDakar.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static RallyDakar.Helpers.Enums;

namespace RallyDakar.Services
{
	public class RaceService : IRaceService
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly ILogger<RaceService> _logger;
		private readonly IMapper _mapper;

		public RaceService(IUnitOfWork unitOfWork, ILogger<RaceService> logger, IMapper mapper)
		{
			_unitOfWork = unitOfWork;
			_logger = logger;
			_mapper = mapper;
		}

		public async Task<IList<RaceDTO>> GetRaces()
		{
			var races = await _unitOfWork.Races.GetAll();
			var results = _mapper.Map<IList<RaceDTO>>(races);
			return results;
		}

		public async Task<RaceDTO> GetRaceById(int Id)
		{
			var race = await _unitOfWork.Races.Get(q => q.Id == Id, new List<string> { "Participants" });
			var result = _mapper.Map<RaceDTO>(race);
			return result;
		}

		public async Task<Race> CreateRace(int year)
		{
			CreateRaceDTO newRace = new CreateRaceDTO() { Year = year };
			var race = _mapper.Map<Race>(newRace);
			await _unitOfWork.Races.Insert(race);
			await _unitOfWork.Save();
			return race;
		}	

		public async Task StartRace(int raceID)
		{
			if (raceID < 1)
			{
				InvalidStateException ex = new InvalidStateException($"[{nameof(RaceService)}] Race ID cannot be less than one.");
				_logger.LogError(ex, $"Invalid inputs");
				throw ex;
			}

			var race = await _unitOfWork.Races.Get(q => q.Id == raceID);
			if (race == null)
			{
				EntityNotFoundException ex = new EntityNotFoundException($"[{nameof(RaceService)}] Race with Id:{raceID} does not exist.");
				_logger.LogError(ex, $"Invalid inputs");
				throw ex;
			}

			var vehicles = await _unitOfWork.Vehicles.GetAll(id => id.RaceId == raceID);
			if (vehicles.Count == 0)
			{
				EntityNotFoundException ex = new EntityNotFoundException($"[{nameof(RaceService)}] Race without participants cannot be started.");
				_logger.LogError(ex, $"Invalid inputs");
				throw ex;
			}
				

			foreach (Vehicle v in vehicles)
			{
				v.Status = VehicleStatus.Running;
			}

			DateTime startTime = DateTime.Now;
			race.StartTime = startTime;
			race.Participants = vehicles;

			int numOfParticipants = vehicles.Count;
			int hourCounter = 1;
			Random random = new Random();
			double randomRes = 0;

			while (numOfParticipants > 0)
			{
				foreach (var vehicle in vehicles)
				{
					if (vehicle.Status == VehicleStatus.Running)
					{
						randomRes = random.Next(0, 1000);//bigger than 100 to decrease the chance of heavy malfunction
						if (randomRes <= vehicle.HeavyMalfunctionProbability)
						{
							vehicle.Status = VehicleStatus.HeavyMalfunction;
							vehicle.FinishTime = startTime.AddHours(hourCounter);
							vehicle.HasEncounteredHeavyMalfunction = true;

							_unitOfWork.Vehicles.Update(vehicle);
							await _unitOfWork.Save();

							numOfParticipants -= 1;
						}
						else if (randomRes <= vehicle.LightMalfunctionProbability)
						{
							vehicle.Status = VehicleStatus.LightMalfunction;
							vehicle.RepairmentHourCounter = vehicle.RepairTime;
							vehicle.LightMalfunctionNumber += 1;

							_unitOfWork.Vehicles.Update(vehicle);
							await _unitOfWork.Save();
						}
						else 
						{
							vehicle.PassedDistance += vehicle.MaxSpeed;
							if (vehicle.PassedDistance >= 10000)
							{
								vehicle.Status = VehicleStatus.Finished;
								vehicle.FinishTime = startTime.AddHours(hourCounter);
								vehicle.PassedDistance = 10000;
								_unitOfWork.Vehicles.Update(vehicle);
								await _unitOfWork.Save();
								numOfParticipants -= 1;
							}
						}
					}
					else if(vehicle.Status == VehicleStatus.LightMalfunction)
					{
						vehicle.RepairmentHourCounter -= 1;
						if (vehicle.RepairmentHourCounter == 0)
							vehicle.Status = VehicleStatus.Running;
					}
				}
				hourCounter += 1;
			}

			race.RaceStatus = RaceStatus.Finished;	 //when the race is started, the whole race is simulated at once and finished
			_unitOfWork.Races.Update(race);
			await _unitOfWork.Save();
		}

		public async Task<RaceStatusDTO> GetRaceStatus(int Id)
		{
			var race = await _unitOfWork.Races.Get(q => q.Id == Id);
			if (race == null)
			{
				EntityNotFoundException ex = new EntityNotFoundException($"[{nameof(RaceService)}] Race with Id:{Id} does not exist.");
				_logger.LogError(ex, $"Invalid inputs");
				throw ex;
			}

			var cars = await _unitOfWork.Vehicles.GetAll(q => q.RaceId == Id && q.Type == VehicleType.Car);
			var motos = await _unitOfWork.Vehicles.GetAll(q => q.RaceId == Id && q.Type == VehicleType.Motorcycle);
			var trucks = await _unitOfWork.Vehicles.GetAll(q => q.RaceId == Id && q.Type == VehicleType.Truck);
			var pending = await _unitOfWork.Vehicles.GetAll(q => q.RaceId == Id && q.Status == VehicleStatus.Pending);
			var finished = await _unitOfWork.Vehicles.GetAll(q => q.RaceId == Id && q.Status == VehicleStatus.Finished);
			var outOfRace = await _unitOfWork.Vehicles.GetAll(q => q.RaceId == Id && q.Status == VehicleStatus.HeavyMalfunction);

			var result = _mapper.Map<RaceStatusDTO>(race);
			result.NumberOfCars = cars.Count();
			result.NumberOfMotos = motos.Count();
			result.NumberOfTrucks = trucks.Count();
			result.NumOfPending = pending.Count();
			result.NumOfFinished = finished.Count();
			result.NumOfOutOfRace = outOfRace.Count();

			return result;
		}
	}
}
