using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using static RallyDakar.Helpers.Enums;

namespace RallyDakar.Models
{
	public class CreateRaceDTO
	{
		[Required]
		public int Year { get; set; }

	}

	public class RaceDTO : CreateRaceDTO
	{
		public int Id { get; set; }
		public RaceStatus RaceStatus { get; set; }
		public DateTime? StartTime { get; set; }
		public List<VehicleDTO> Participants { get; set; }
	}

	public class RaceStatusDTO
	{
		public RaceStatus RaceStatus { get; set; }
		public int NumberOfCars { get; set; }
		public int NumberOfTrucks { get; set; }
		public int NumberOfMotos { get; set; }
		public int NumOfPending { get; set; }
		public int NumOfFinished { get; set; }
		public int NumOfOutOfRace { get; set; }
	}
}
