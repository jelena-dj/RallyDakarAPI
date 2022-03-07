using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using static RallyDakar.Helpers.Enums;

namespace RallyDakar.Models
{
	public class VehicleCreateUpdateDTO
	{
		[Required]
		[StringLength(maximumLength: 50, ErrorMessage = "Team name is too long.")]
		public string TeamName { get; set; }

		[Required]
		[StringLength(maximumLength: 50, ErrorMessage = "Model name is too long")]
		public string Model { get; set; }

		[Required]
		public DateTime ManufacturingDate { get; set; }

		[Required]
		[Range(0,2)]
		public VehicleType Type { get; set; }

		[Required]
		[Range(0,4)]
		public VehicleSubType SubType { get; set; }

		[Required]
		public int RaceId { get; set; }
	}

	public class VehicleDTO : VehicleCreateUpdateDTO
	{
		public int Id { get; set; }
		public float MaxSpeed { get; set; }
		public float RepairTime { get; set; }
		public float LightMalfunctionProbability { get; set; }
		public float HeavyMalfunctionProbability { get; set; }
		public VehicleStatus Status { get; set; }
		public float PassedDistance { get; set; } 
		public DateTime FinishTime { get; set; }
	}

	public class VehicleLBDataDTO
	{
		public int Id { get; set; }
		public string TeamName { get; set; }
		public string Model { get; set; }
		public DateTime FinishTime { get; set; }
		public VehicleStatus Status { get; set; }
		public float PassedDistance { get; set; }
	}

	public class VehicleStatisticsDTO
	{
		public int Id { get; set; }
		public DateTime FinishTime { get; set; }
		public VehicleStatus Status { get; set; }
		public double PassedDistance { get; set; }
		public int LightMalfunctionNumber { get; set; }
		public bool HasEncounteredHeavyMalfunction { get; set; }
	}
}
