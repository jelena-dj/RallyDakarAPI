using Microsoft.EntityFrameworkCore;

namespace RallyDakar.Data
{
	public class RallyDakarDbContext : DbContext
	{
		public RallyDakarDbContext(DbContextOptions options) : base(options)
		{ }

		public DbSet<Vehicle> Vehicles { get; set; }

		public DbSet<Race> Races { get; set; }
	}
}
