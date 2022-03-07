using RallyDakar.Data;
using RallyDakar.IRepository;
using RallyDakar.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RallyDakar.Services
{
	public interface IRaceService
	{
		public Task<IList<RaceDTO>> GetRaces();
		public Task<RaceDTO> GetRaceById(int Id);
		public Task<Race> CreateRace(int year);
		public Task StartRace(int raceID);
		public Task<RaceStatusDTO> GetRaceStatus(int Id);
	}
}
