using RallyDakar.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using static RallyDakar.Helpers.Enums;

namespace RallyDakar.Data
{
	public class Vehicle
	{
		public int Id { get; set; }
		public string TeamName { get; set; }
		public string Model { get; set; }
		public DateTime ManufacturingDate { get; set; }
		public VehicleType Type { get; set; }
		public VehicleSubType SubType { get; set; }

		[ForeignKey(nameof(Race))]
		public int RaceId { get; set; }

		public float MaxSpeed { get; set; }
		public float RepairTime { get; set; }
		public float LightMalfunctionProbability { get; set; }
		public float HeavyMalfunctionProbability { get; set; }

		public VehicleStatus Status { get; set; } = VehicleStatus.Pending;
		public float PassedDistance { get; set; } = 0;
		public DateTime FinishTime { get; set; }
		public int LightMalfunctionNumber { get; set; }
		public bool HasEncounteredHeavyMalfunction{ get; set; }
		public float RepairmentHourCounter { get; set; }


		public Vehicle () {
		}

		public Vehicle(int id, string teamName, string model, DateTime manufacturingDate, VehicleType type, VehicleSubType subType, int raceId)
		{
			Id = id;
			TeamName = teamName;
			Model = model;
			ManufacturingDate = manufacturingDate;
			Type = type;
			SubType = subType;
			RaceId = raceId;
			SetStaticData();
		}

		public void SetStaticData()
		{
			switch (Type)
			{
				case VehicleType.Car:
					RepairTime = ConstantsCar.RepairTime;
					MaxSpeed = (SubType == VehicleSubType.SportsCar) ? ConstantsCar.SportsCarMaxSpeed : ConstantsCar.TerrainCarMaxSpeed;
					LightMalfunctionProbability = (SubType == VehicleSubType.SportsCar) ? ConstantsCar.SportCarLightMalfunctionProb : ConstantsCar.TerrainCarLightMalfunctionProb;
					HeavyMalfunctionProbability = (SubType == VehicleSubType.SportsCar) ? ConstantsCar.SportCarHeavyMalfunctionProb : ConstantsCar.TerrainCarHeavyMalfunctionProb;
					break;
				case VehicleType.Motorcycle:
					RepairTime = ConstantsMoto.RepairTime;
					MaxSpeed = (SubType == VehicleSubType.SportMotocycle) ? ConstantsMoto.SportMotoMaxSpeed : ConstantsMoto.CrossMotoMaxSpeed;
					LightMalfunctionProbability = (SubType == VehicleSubType.SportMotocycle) ? ConstantsMoto.SportMotoLightMalfunctionProb : ConstantsMoto.CrossMotoLightMalfunctionProb;
					HeavyMalfunctionProbability = (SubType == VehicleSubType.SportMotocycle) ? ConstantsMoto.SportMotoHeavyMalfunctionProb : ConstantsMoto.CrossMotoHeavyMalfunctionProb;
					break;
				case VehicleType.Truck:
					RepairTime = ConstantsTruck.RepairTime;
					MaxSpeed = ConstantsTruck.MaxSpeed;
					LightMalfunctionProbability = ConstantsTruck.LightMalfunctionProb;
					HeavyMalfunctionProbability = ConstantsTruck.HeavyMalfunctionProb;
					break;
				default:
					RepairTime = 0;
					MaxSpeed = 0;
					LightMalfunctionProbability = 0;
					HeavyMalfunctionProbability = 0;
					break;
			}
		}
	}
}
