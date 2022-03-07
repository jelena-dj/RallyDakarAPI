using AutoMapper;
using RallyDakar.Data;
using RallyDakar.Models;

namespace RallyDakar.Configurations
{
	public class MapperInitializer : Profile
	{
		public MapperInitializer()
		{
			CreateMap<Vehicle, VehicleDTO>().ReverseMap();
			CreateMap<Vehicle, VehicleCreateUpdateDTO>().ReverseMap();
			CreateMap<Vehicle, VehicleLBDataDTO>().ReverseMap();
			CreateMap<Vehicle, VehicleStatisticsDTO>().ReverseMap();
			CreateMap<Race, CreateRaceDTO>().ReverseMap();
			CreateMap<Race, RaceDTO>().ReverseMap();
			CreateMap<Race, RaceStatusDTO>().ReverseMap();
		}
	}
}
