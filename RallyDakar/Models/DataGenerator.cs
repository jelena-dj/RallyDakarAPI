using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RallyDakar.Data;
using System;
using System.Linq;
using static RallyDakar.Helpers.Enums;

namespace RallyDakar.Models
{
	public class DataGenerator
	{
		public static void Initialize(IServiceProvider serviceProvider)
		{
			using (var context = new RallyDakarDbContext(
			serviceProvider.GetRequiredService<DbContextOptions<RallyDakarDbContext>>()))
			{
				if (!context.Races.Any())
				{
					context.Races.AddRange(
						new Race() { Id = 1, Year = 2020 }
						);
				}

				if (!context.Vehicles.Any())
				{
					context.Vehicles.AddRange(
						new Vehicle() { Id = 1, RaceId = 1, Model = "TruckModel", TeamName = "TruckTeam", ManufacturingDate = new DateTime(2020, 1, 1), Type = VehicleType.Truck, SubType = VehicleSubType.NoSubType, MaxSpeed = 80, RepairTime = 7, LightMalfunctionProbability = 6, HeavyMalfunctionProbability = 4 },
						new Vehicle() { Id = 2, RaceId = 1, Model = "CarModel", TeamName = "CarTeam", ManufacturingDate = new DateTime(2020, 1, 1), Type = VehicleType.Car, SubType = VehicleSubType.SportsCar, MaxSpeed = 140, RepairTime = 5, LightMalfunctionProbability = 12, HeavyMalfunctionProbability = 2 },
						new Vehicle() { Id = 3, RaceId = 1, Model = "MotoModel", TeamName = "MotoTeam", ManufacturingDate = new DateTime(2020, 1, 1), Type = VehicleType.Motorcycle, SubType = VehicleSubType.CrossMotocycle, MaxSpeed = 85, RepairTime = 3, LightMalfunctionProbability = 3, HeavyMalfunctionProbability = 2 },
						new Vehicle() { Id = 4, RaceId = 1, Model = "TruckModel1", TeamName = "TruckTeam", ManufacturingDate = new DateTime(2020, 1, 1), Type = VehicleType.Truck, SubType = VehicleSubType.NoSubType, MaxSpeed = 80, RepairTime = 7, LightMalfunctionProbability = 6, HeavyMalfunctionProbability = 4 },
						new Vehicle() { Id = 5, RaceId = 1, Model = "CarModel1", TeamName = "CarTeam", ManufacturingDate = new DateTime(2020, 1, 1), Type = VehicleType.Car, SubType = VehicleSubType.TerrainCar, MaxSpeed = 100, RepairTime = 5, LightMalfunctionProbability = 3, HeavyMalfunctionProbability = 1 },
						new Vehicle() { Id = 6, RaceId = 1, Model = "MotoModel1", TeamName = "MotoTeam", ManufacturingDate = new DateTime(2020, 1, 1), Type = VehicleType.Motorcycle, SubType = VehicleSubType.SportMotocycle, MaxSpeed = 130, RepairTime = 3, LightMalfunctionProbability = 18, HeavyMalfunctionProbability = 10 }
					); ; 

					context.SaveChanges();
				}

			}
		}
	}
}
