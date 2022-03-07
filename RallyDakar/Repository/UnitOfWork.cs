using RallyDakar.Data;
using RallyDakar.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RallyDakar.Repository
{
	public class UnitOfWork : IUnitOfWork
	{
		private readonly RallyDakarDbContext _context;
		private IGenericRepository<Vehicle> _vehicles;
		private IGenericRepository<Race> _races;

		public UnitOfWork(RallyDakarDbContext context)
		{
			_context = context;
		}

		public IGenericRepository<Vehicle> Vehicles => _vehicles ??= new GenericRepository<Vehicle>(_context);
		public IGenericRepository<Race> Races => _races ??= new GenericRepository<Race>(_context);

		public void Dispose()
		{
			_context.Dispose();
			GC.SuppressFinalize(this);
		}

		public async Task Save()
		{
			await _context.SaveChangesAsync();
		}
	}
}
