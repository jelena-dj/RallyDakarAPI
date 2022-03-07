using RallyDakar.Helpers;
using RallyDakar.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static RallyDakar.Helpers.Enums;

namespace RallyDakar.Services
{
	public interface IVehicleService
	{		
		public Task<IList<VehicleDTO>> GetVehicles();
		public Task<VehicleDTO> GetVehicleById(int Id);
		public Task<VehicleDTO> AddVehicle(VehicleCreateUpdateDTO vehicleDTO);
		public Task UpdateVehicle(int Id, VehicleCreateUpdateDTO vehicleDTO);
		public Task DeleteVehicle(int Id);
		public Task<IList<VehicleLBDataDTO>> GetLeaderboardAll(int raceId);
		public Task<IList<VehicleLBDataDTO>> GetLeaderboardByType(int raceId, VehicleType vehicleType);
		public Task<VehicleStatisticsDTO> GetVehicleStatistics(int Id);

	}
}
