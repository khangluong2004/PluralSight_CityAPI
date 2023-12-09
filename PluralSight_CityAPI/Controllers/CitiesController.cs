using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PluralSight_CityAPI.Models;
using PluralSight_CityAPI.Services;
using System.Reflection.Metadata.Ecma335;
using System.Text.Json;

namespace PluralSight_CityAPI.Controllers
{
    // Attributes in [] for routing and add Metadata
    // Convention: Return IActionResult coupled with HTTP status code (200 level: success, 400 level: Client's mistake, 500: Server's error) 
    [ApiController]
    [Authorize]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/cities")]
    public class CitiesController: ControllerBase
    {
        //private readonly CitiesDataStore _cityDataStore;
        private readonly ICityInfoRepository _cityInfoRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<CitiesController> _logger;
        const int maxPageSize = 20;

        public CitiesController(ICityInfoRepository cityInfoRepository, IMapper mapper, ILogger<CitiesController> logger)
        {
            _cityInfoRepository = cityInfoRepository;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CityWithoutPointDto>>> GetCities([FromQuery] string? name, 
            [FromQuery] string? searchQuery, int pageNumber = 1, int pageSize = 10)
        {
            if (pageSize > maxPageSize)
            {
                pageSize = maxPageSize;
            }

            var (citiesData, pageDetails) = await _cityInfoRepository.GetCitiesAsync(name, searchQuery, pageNumber, pageSize);

            Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(pageDetails));

            return Ok(_mapper.Map<IEnumerable<CityWithoutPointDto>>(citiesData));
        }
        /// <summary>
        /// Get the city of cityId
        /// </summary>
        /// <param name="cityId"> id of the city </param>
        /// <param name="includePoints"> Whether to include the points or not </param>
        /// <returns> An IActionResult </returns>
        /// <response code="200"> Return requested city </response> 

        [HttpGet("{cityId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]

        // Change to IActionResult since there are 2 possible return types
        
        public async Task<IActionResult> GetCity(int cityId, bool includePoints = false) {
            var cityData = await _cityInfoRepository.GetCityAsync(cityId, includePoints);

            if (cityData == null)
            {
                return NotFound();
            }

            if (includePoints)
            {
                return Ok(_mapper.Map<CityDto>(cityData));
            }

            return Ok(_mapper.Map<CityWithoutPointDto>(cityData));
        }
    }
}
