using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RallyDakar.Services;
using Swashbuckle.AspNetCore.Annotations;
using System.Threading.Tasks;

namespace RallyDakar.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class RaceController : ControllerBase
	{
		private readonly ILogger<RaceController> _logger;
		private readonly IRaceService _raceService;


		public RaceController(ILogger<RaceController> logger, IRaceService raceService)
		{
			_logger = logger;
			_raceService = raceService;
		}

		[HttpGet("{Id:int}", Name = "GetRace")]
		[SwaggerOperation(Summary = "Gets race by Id")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> GetRace(int Id)
		{
			var result = await _raceService.GetRaceById(Id);
			return Ok(result);
		}

		[HttpGet]
		[SwaggerOperation(Summary = "Gets all races")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> GetRaces()
		{
			var results = await _raceService.GetRaces();
			return Ok(results);
		}

		[HttpPost]
		[SwaggerOperation(Summary = "Creates Race for the specified year")]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status201Created)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> CreateRace(int year)
		{
			var race = await _raceService.CreateRace(year);
			int id = race.Id;
			return CreatedAtRoute(nameof(GetRace), new { id = race.Id }, race);
 		}

		[HttpPut("{id:int}")]
		[SwaggerOperation(Summary = "Starts the race with specified Id")]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult> StartRace(int id)
		{
			await _raceService.StartRace(id);
			return NoContent();
		}

		[HttpGet("Id={Id}/Status")]
		[SwaggerOperation(Summary = "Gets Race status")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> GetRaceStatus(int Id)
		{
			var result = await _raceService.GetRaceStatus(Id);
			return Ok(result);
		}
	}
}
