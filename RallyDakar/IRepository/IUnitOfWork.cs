using RallyDakar.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RallyDakar.IRepository
{
	public interface IUnitOfWork  : IDisposable
	{
		IGenericRepository<Vehicle> Vehicles { get; }

		IGenericRepository<Race> Races { get; }

		Task Save();
	}
}
