using System;
using System.Collections.Generic;
using static RallyDakar.Helpers.Enums;

namespace RallyDakar.Data
{
	public class Race
	{
		public int Id { get; set; }
		public int Year { get; set; }
		public DateTime? StartTime { get; set; }
		public RaceStatus RaceStatus { get; set; }
		public virtual IList<Vehicle> Participants { get; set; }
	}
}
