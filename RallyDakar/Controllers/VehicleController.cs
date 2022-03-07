using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RallyDakar.Models;
using System.Threading.Tasks;
using RallyDakar.Services;
using static RallyDakar.Helpers.Enums;
using Swashbuckle.AspNetCore.Annotations;

namespace RallyDakar.Controllers
{
	[Route("api/[controller]")]
	[ApiController]

	public class VehicleController : ControllerBase
	{
		private readonly IVehicleService _vehicleService;

		public VehicleController(IVehicleService vehicleService)
		{
			_vehicleService = vehicleService;
		}

		[HttpGet("{id:int}", Name = "GetVehicle")]
		[SwaggerOperation(Summary = "Gets vehicle by Id")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> GetVehicle(int id)
		{
			var result = await _vehicleService.GetVehicleById(id);
			return Ok(result);
		}

		[HttpGet]
		[SwaggerOperation(Summary = "Gets all vehicles")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> GetVehicles()
		{
			var results = await _vehicleService.GetVehicles();
			return Ok(results);
		}

		[HttpPost]
		[SwaggerOperation(Summary = "Adds a new vehicle")]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status201Created)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult> AddVehicle([FromBody] VehicleCreateUpdateDTO vehicleDTO)
		{
			var vehicle = await _vehicleService.AddVehicle(vehicleDTO);
			int id = vehicle.Id;
			return CreatedAtRoute(nameof(GetVehicle), new { id = vehicle.Id }, vehicle);
		}

		[HttpPut("{id:int}")]
		[SwaggerOperation(Summary = "Updates a vehicle that has the specified Id")]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult> UpdateVehicle(int id, [FromBody] VehicleCreateUpdateDTO updateVehicleDTO)
		{
			await _vehicleService.UpdateVehicle(id, updateVehicleDTO);
			return NoContent();
		}

		[HttpDelete("{id:int}")]
		[SwaggerOperation(Summary = "Deletes a vehicle that has the specified Id")]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> DeleteVehicle(int id)
		{
			await _vehicleService.DeleteVehicle(id);
			return NoContent();
		}

		[HttpGet("raceId={raceId}/Leaderboard")]
		[SwaggerOperation(Summary = "Gets the leaderboard of all participants for the specified Id of the race")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> GetLeaderboardAll(int raceId)
		{
			var results = await _vehicleService.GetLeaderboardAll(raceId);
			return Ok(results);
		}

		[HttpGet("raceId={raceId}/Leaderboard/Type")]
		[SwaggerOperation(Summary = "Gets the leaderboard by the type of participants	for the specified Id of the race")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> GetLeaderboardByType(int raceId, VehicleType vehicleType)
		{
			var results = await _vehicleService.GetLeaderboardByType(raceId, vehicleType);
			return Ok(results);
		}

		[HttpGet("Id={Id}/Statistics")]
		[SwaggerOperation(Summary = "Gets the statistics for the vehicle with specified Id")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> GetVehicleStatistics(int Id)
		{
			var results = await _vehicleService.GetVehicleStatistics(Id);
			return Ok(results);
		}
	}
}
