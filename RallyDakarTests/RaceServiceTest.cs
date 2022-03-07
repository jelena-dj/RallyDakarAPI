using AutoMapper;
using Microsoft.Extensions.Logging;
using Moq;
using RallyDakar.Configurations;
using RallyDakar.CustomExceptions;
using RallyDakar.Data;
using RallyDakar.IRepository;
using RallyDakar.Models;
using RallyDakar.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using static RallyDakar.Helpers.Enums;

namespace RallyDakarTests
{
	public class RaceServiceTest
	{
		private Mock<IUnitOfWork> unitOfWorkMock;
		private Mock<ILogger<RaceService>> loggerMock;
		private IMapper mapper;
		private Mock<IGenericRepository<Race>> raceRepositoryMock;
		private Mock<IGenericRepository<Vehicle>> vehicleRepositoryMock;

		public RaceServiceTest()
		{
			unitOfWorkMock = new Mock<IUnitOfWork>();
			loggerMock = new Mock<ILogger<RaceService>>();
			raceRepositoryMock = new Mock<IGenericRepository<Race>>();
			vehicleRepositoryMock = new Mock<IGenericRepository<Vehicle>>();

			unitOfWorkMock.Setup(m => m.Vehicles).Returns(vehicleRepositoryMock.Object);
			unitOfWorkMock.Setup(m => m.Races).Returns(raceRepositoryMock.Object);
			unitOfWorkMock.Setup(m => m.Save());

			vehicleRepositoryMock.Setup(m => m.Update((Vehicle)null));
			raceRepositoryMock.Setup(m => m.Update((Race)null));
			raceRepositoryMock.Setup(m => m.Insert((Race)null));

			var myProfile = new MapperInitializer();
			var configuration = new MapperConfiguration(cfg => cfg.AddProfile(myProfile));
			mapper = new Mapper(configuration);
		}

		[Fact]
		public async Task CreateRace()
		{
			int year = 2021;

			CreateRaceDTO raceDTO = new CreateRaceDTO() { Year = year };
			Race race = new Race() { Year = year };

			RaceService raceService = new RaceService(unitOfWorkMock.Object, loggerMock.Object, mapper);
			var newRace = await raceService.CreateRace(year);

			Assert.Equal(race.Year, newRace.Year);
			Assert.Equal(race.Id, newRace.Id);
		}


		[Fact]
		public async Task InvalidIdentifierForStartRace()
		{
			int id = -1;
			RaceService raceService = new RaceService(unitOfWorkMock.Object, loggerMock.Object, mapper);
			await Assert.ThrowsAsync<InvalidStateException>(() => raceService.StartRace(id));
		}

		[Fact]
		public async Task StartingNonexistingRace()
		{
			int id = 1;
			Race race = null;
			raceRepositoryMock.Setup(m => m.Get(q => q.Id == 1, null)).ReturnsAsync(race).Verifiable();
			RaceService raceService = new RaceService(unitOfWorkMock.Object, loggerMock.Object, mapper);
			await Assert.ThrowsAsync<EntityNotFoundException>(() => raceService.StartRace(id));
		}

		[Fact]
		public async Task StartingRaceWithoutVehicles()
		{
			int id = 1;
			Race race = new Race() { Id = id};
			raceRepositoryMock.Setup(m => m.Get(q => q.Id == 1, null)).ReturnsAsync(race).Verifiable();

			IList<Vehicle> vehicles = new List<Vehicle>();
			vehicleRepositoryMock.Setup(m => m.GetAll(q => q.RaceId == id, null, null)).ReturnsAsync(vehicles).Verifiable();

			RaceService raceService = new RaceService(unitOfWorkMock.Object, loggerMock.Object, mapper);
			await Assert.ThrowsAsync<EntityNotFoundException>(() => raceService.StartRace(id));
		}

		[Fact]
		public async Task StartingRaceHappyPath()
		{
			int id = 1;
			Race race = new Race() { Id = id };
			raceRepositoryMock.Setup(m => m.Get(q => q.Id == 1, null)).ReturnsAsync(race).Verifiable();


			Vehicle vehicle1 = new Vehicle() { Id = 1, RaceId = 1, MaxSpeed = 100, HeavyMalfunctionProbability = 1, LightMalfunctionProbability = 3, RepairTime = 5};
			Vehicle vehicle2 = new Vehicle() { Id = 2, RaceId = 1, MaxSpeed = 140, HeavyMalfunctionProbability = 2, LightMalfunctionProbability = 12, RepairTime = 5 };
			Vehicle vehicle3 = new Vehicle() { Id = 3, RaceId = 1, MaxSpeed = 80, HeavyMalfunctionProbability = 4, LightMalfunctionProbability = 6, RepairTime = 7 };
			IList<Vehicle> vehicles = new List<Vehicle>();
			vehicles.Add(vehicle1);
			vehicles.Add(vehicle2);
			vehicles.Add(vehicle3);
			vehicleRepositoryMock.Setup(m => m.GetAll(q => q.RaceId == id, null, null)).ReturnsAsync(vehicles).Verifiable();

			RaceService raceService = new RaceService(unitOfWorkMock.Object, loggerMock.Object, mapper);
			await raceService.StartRace(id);

			unitOfWorkMock.Verify(m => m.Races.Update(race));
		}

	}
}
