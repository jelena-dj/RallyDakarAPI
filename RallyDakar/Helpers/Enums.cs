

namespace RallyDakar.Helpers
{
	public static class Enums
	{
		public enum VehicleType
		{
			Car = 0,
			Motorcycle = 1,
			Truck = 2
		}

		public enum VehicleSubType
		{
			NoSubType = 0,
			SportsCar = 1,
			TerrainCar = 2,
			CrossMotocycle = 3,
			SportMotocycle = 4,
		}

		public enum VehicleStatus
		{
			Pending = 0,
			HeavyMalfunction = 1,
			LightMalfunction = 2,
			Running = 3,
			Finished = 4
		}

		public enum RaceStatus
		{
			Pending = 0,
			Started = 1,
			Finished = 2
		}

		public enum MalfunctionType
		{
			LightMalfunction = 0,
			HeavyMalfunction = 1
		}
	}
}
