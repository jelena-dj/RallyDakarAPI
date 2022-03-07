using AutoMapper;
using Microsoft.Extensions.Logging;
using Moq;
using RallyDakar.Configurations;
using RallyDakar.CustomExceptions;
using RallyDakar.Data;
using RallyDakar.Helpers;
using RallyDakar.IRepository;
using RallyDakar.Models;
using RallyDakar.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Xunit;
using static RallyDakar.Helpers.Enums;

namespace RallyDakarTests
{
	public class VehicleServiceTest 
	{
		private Mock<IUnitOfWork> unitOfWorkMock;
		private Mock<ILogger<VehicleService>> loggerMock;
		private IMapper mapper;
		private Mock<IGenericRepository<Race>> raceRepositoryMock;
		private Mock<IGenericRepository<Vehicle>> vehicleRepositoryMock ;

		public VehicleServiceTest()
		{
			unitOfWorkMock = new Mock<IUnitOfWork>();
			loggerMock = new Mock<ILogger<VehicleService>>();
			raceRepositoryMock = new Mock<IGenericRepository<Race>>();
			vehicleRepositoryMock = new Mock<IGenericRepository<Vehicle>>();

			vehicleRepositoryMock.Setup(m => m.Insert((Vehicle)null));
			vehicleRepositoryMock.Setup(m => m.Update((Vehicle)null));
			vehicleRepositoryMock.Setup(m => m.Delete(It.IsAny<int>()));

			unitOfWorkMock.Setup(m => m.Races).Returns(raceRepositoryMock.Object);
			unitOfWorkMock.Setup(m => m.Vehicles).Returns(vehicleRepositoryMock.Object);
			unitOfWorkMock.Setup(m => m.Save());

			var myProfile = new MapperInitializer();
			var configuration = new MapperConfiguration(cfg => cfg.AddProfile(myProfile));
			mapper = new Mapper(configuration);
		}

		[Fact]
		public async Task AddVehicleToANullRace()
		{
			Race race = null;

			raceRepositoryMock.Setup(m => m.Get(q => q.Id == 1, null)).ReturnsAsync(race).Verifiable();

			VehicleCreateUpdateDTO vDTO = CreateRandomCreateUpdateDTO();
			VehicleService vehicleService = new VehicleService(unitOfWorkMock.Object, loggerMock.Object, mapper);
			await Assert.ThrowsAsync<InvalidStateException>(() => vehicleService.AddVehicle(vDTO));
		}

		[Fact]
		public async Task AddVehicleNotPendingRace()
		{
			Race race = new Race() { RaceStatus = RaceStatus.Started };

			raceRepositoryMock.Setup(m => m.Get(q => q.Id == 1, null)).ReturnsAsync(race).Verifiable();

			VehicleCreateUpdateDTO vDTO = CreateRandomCreateUpdateDTO();
			VehicleService vehicleService = new VehicleService(unitOfWorkMock.Object, loggerMock.Object, mapper);
			await Assert.ThrowsAsync<InvalidStateException>(() => vehicleService.AddVehicle(vDTO));
		}

		[Fact]
		public async Task AddVehicleToPendingRace()
		{
			Race race = new Race() { RaceStatus = RaceStatus.Pending };

			raceRepositoryMock.Setup(m => m.Get(q => q.Id == 1, null)).ReturnsAsync(race).Verifiable();

			VehicleCreateUpdateDTO vDTO = CreateRandomCreateUpdateDTO();


			VehicleService vehicleService = new VehicleService(unitOfWorkMock.Object, loggerMock.Object, mapper);
			var testVDTO = await vehicleService.AddVehicle(vDTO);
			Assert.Equal(vDTO.Model, testVDTO.Model);
			Assert.Equal(ConstantsCar.SportsCarMaxSpeed, testVDTO.MaxSpeed);
			Assert.Equal(ConstantsCar.SportCarHeavyMalfunctionProb, testVDTO.HeavyMalfunctionProbability);
			Assert.Equal(ConstantsCar.SportCarLightMalfunctionProb, testVDTO.LightMalfunctionProbability);
			Assert.Equal(ConstantsCar.RepairTime, testVDTO.RepairTime);

		}

		[Fact]
		public async Task UpdateNonExistingVehicle()
		{
			Vehicle vehicle = null;
			int id = 1;
			vehicleRepositoryMock.Setup(m => m.Get(q => q.Id == 1, null)).ReturnsAsync(vehicle).Verifiable();
			VehicleCreateUpdateDTO vDTO = CreateRandomCreateUpdateDTO();
			VehicleService vehicleService = new VehicleService(unitOfWorkMock.Object, loggerMock.Object, mapper);
			await Assert.ThrowsAsync<EntityNotFoundException>(() => vehicleService.UpdateVehicle(id, vDTO));
		}


		[Fact]
		public async Task UpdateVehicle_SetNonExistingRace()
		{
			Race race = null;
			int id = 1;
			Vehicle vehicle = new Vehicle();
			vehicleRepositoryMock.Setup(m => m.Get(q => q.Id == 1, null)).ReturnsAsync(vehicle).Verifiable();
			raceRepositoryMock.Setup(m => m.Get(q => q.Id == 1, null)).ReturnsAsync(race).Verifiable();

			VehicleCreateUpdateDTO vDTO = CreateRandomCreateUpdateDTO();
			VehicleService vehicleService = new VehicleService(unitOfWorkMock.Object, loggerMock.Object, mapper);
			await Assert.ThrowsAsync<InvalidStateException>(() => vehicleService.UpdateVehicle(id, vDTO));
		}

		[Fact]
		public async Task UpdateVehicleThatBelongsToANonPendingRace()
		{
			Race race = new Race() { RaceStatus = RaceStatus.Started };
			int id = 1;
			Vehicle vehicle = new Vehicle();
			vehicleRepositoryMock.Setup(m => m.Get(q => q.Id == 1, null)).ReturnsAsync(vehicle).Verifiable();
			raceRepositoryMock.Setup(m => m.Get(q => q.Id == 1, null)).ReturnsAsync(race).Verifiable();

			VehicleCreateUpdateDTO vDTO = CreateRandomCreateUpdateDTO();
			VehicleService vehicleService = new VehicleService(unitOfWorkMock.Object, loggerMock.Object, mapper);
			await Assert.ThrowsAsync<InvalidStateException>(() => vehicleService.UpdateVehicle(id, vDTO));
		}

		[Fact]
		public async Task UpdateVehicleThatBelongsToAPendingRace()
		{
			Race race = new Race() { RaceStatus = RaceStatus.Pending };
			int id = 1;
			Vehicle vehicle = new Vehicle() { Id = 1 };

			vehicleRepositoryMock.Setup(m => m.Get(q => q.Id == 1, null)).ReturnsAsync(vehicle).Verifiable();
			raceRepositoryMock.Setup(m => m.Get(q => q.Id == 1, null)).ReturnsAsync(race).Verifiable();

			VehicleCreateUpdateDTO vDTO = CreateRandomCreateUpdateDTO();
			VehicleService vehicleService = new VehicleService(unitOfWorkMock.Object, loggerMock.Object, mapper);
			await vehicleService.UpdateVehicle(id, vDTO);

			unitOfWorkMock.Verify(m => m.Vehicles.Update(vehicle));
		}

		[Fact]
		public async Task DeleteNonExistingVehicle()
		{
			Vehicle vehicle = null;
			int id = 1;
			vehicleRepositoryMock.Setup(m => m.Get(q => q.Id == 1, null)).ReturnsAsync(vehicle).Verifiable();
			VehicleService vehicleService = new VehicleService(unitOfWorkMock.Object, loggerMock.Object, mapper);
			await Assert.ThrowsAsync<EntityNotFoundException>(() => vehicleService.DeleteVehicle(id));
		}

		[Fact]
		public async Task DeleteVehicleFromANonPendingRace()
		{
			Race race = new Race() { Id = 1, RaceStatus = RaceStatus.Started };
			int id = 1;
			Vehicle vehicle = new Vehicle() { RaceId = 1 };
			vehicleRepositoryMock.Setup(m => m.Get(q => q.Id == 1, null)).ReturnsAsync(vehicle).Verifiable();
			raceRepositoryMock.Setup(m => m.Get(q => q.Id == 1, null)).ReturnsAsync(race).Verifiable();

			VehicleService vehicleService = new VehicleService(unitOfWorkMock.Object, loggerMock.Object, mapper);
			await Assert.ThrowsAsync<InvalidStateException>(() => vehicleService.DeleteVehicle(id));
		}

		[Fact]
		public async Task DeleteVehicleFromPendingRace()
		{
			Race race = new Race() { Id = 1, RaceStatus = RaceStatus.Pending };
			int id = 1;
			Vehicle vehicle = new Vehicle() { Id = 1, RaceId = 1 };
			vehicleRepositoryMock.Setup(m => m.Get(q => q.Id == 1, null)).ReturnsAsync(vehicle).Verifiable();
			raceRepositoryMock.Setup(m => m.Get(q => q.Id == 1, null)).ReturnsAsync(race).Verifiable();
			VehicleService vehicleService = new VehicleService(unitOfWorkMock.Object, loggerMock.Object, mapper);
			await vehicleService.DeleteVehicle(id);
			unitOfWorkMock.Verify(m => m.Vehicles.Delete(1));
		}

		[Fact]
		public async Task GetLeaderboardHappyPath()
		{
			Race race = new Race() { Id = 1, RaceStatus = RaceStatus.Finished };
			int id = 1;
			Vehicle vehicle1 = new Vehicle() { Id = 1, RaceId = 1, PassedDistance = 10000, FinishTime = DateTime.Now.AddHours(5), Status = VehicleStatus.Finished };
			Vehicle vehicle2 = new Vehicle() { Id = 2, RaceId = 1, PassedDistance = 500, FinishTime = DateTime.Now, Status = VehicleStatus.HeavyMalfunction };
			Vehicle vehicle3 = new Vehicle() { Id = 3, RaceId = 1, PassedDistance = 10000, FinishTime = DateTime.Now.AddHours(2), Status = VehicleStatus.Finished };
			IList<Vehicle> vehicles = new List<Vehicle>();
			vehicles.Add(vehicle1);
			vehicles.Add(vehicle2);
			vehicles.Add(vehicle3);

			vehicleRepositoryMock.Setup(m => m.GetAll(q => q.RaceId == 1, null, null)).ReturnsAsync(vehicles).Verifiable();
			raceRepositoryMock.Setup(m => m.Get(q => q.Id == 1, null)).ReturnsAsync(race).Verifiable();
			VehicleService vehicleService = new VehicleService(unitOfWorkMock.Object, loggerMock.Object, mapper);
			List<VehicleLBDataDTO> leaderboard = (List<VehicleLBDataDTO>)await vehicleService.GetLeaderboardAll(id);

			Assert.Equal(3, leaderboard[0].Id);
			Assert.Equal(1, leaderboard[1].Id);
			Assert.Equal(2, leaderboard[2].Id);
		}

		[Fact]
		public async Task GetLeaderboardForTypeHappyPath()
		{
			Race race = new Race() { Id = 1, RaceStatus = RaceStatus.Finished };
			int id = 1;
			Vehicle vehicle1 = new Vehicle() { Id = 1, RaceId = 1, PassedDistance = 10000, FinishTime = DateTime.Now.AddHours(5), Status = VehicleStatus.Finished, Type = VehicleType.Car };
			Vehicle vehicle2 = new Vehicle() { Id = 2, RaceId = 1, PassedDistance = 500, FinishTime = DateTime.Now, Status = VehicleStatus.HeavyMalfunction, Type = VehicleType.Car };
			IList<Vehicle> vehicles = new List<Vehicle>();
			vehicles.Add(vehicle1);
			vehicles.Add(vehicle2);

			VehicleType vType = VehicleType.Car;

			vehicleRepositoryMock.Setup(m => m.GetAll(q => q.RaceId == id && q.Type == vType, null, null)).ReturnsAsync(vehicles).Verifiable();
			raceRepositoryMock.Setup(m => m.Get(q => q.Id == 1, null)).ReturnsAsync(race).Verifiable();
			VehicleService vehicleService = new VehicleService(unitOfWorkMock.Object, loggerMock.Object, mapper);
			List<VehicleLBDataDTO> leaderboard = (List<VehicleLBDataDTO>)await vehicleService.GetLeaderboardByType(id, VehicleType.Car);

			Assert.Equal(1, leaderboard[0].Id);
			Assert.Equal(2, leaderboard[1].Id);
		}


		private VehicleCreateUpdateDTO CreateRandomCreateUpdateDTO()
		{
			return new VehicleCreateUpdateDTO()
			{
				TeamName = "TeamName",
				Model = "Model",
				ManufacturingDate = DateTime.Now,
				Type = VehicleType.Car,
				SubType = VehicleSubType.SportsCar,
				RaceId = 1
			};
		}

		private VehicleDTO CreateRandomDTO()
		{
			return new VehicleDTO()
			{
				TeamName = "TeamName",
				Model = "Model",
				ManufacturingDate = DateTime.Now,
				Type = VehicleType.Car,
				SubType = VehicleSubType.SportsCar,
				RaceId = 1
			};
		}

		
	}
}
