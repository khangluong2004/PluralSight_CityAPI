using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using PluralSight_CityAPI.Models;
using PluralSight_CityAPI.Services;

namespace PluralSight_CityAPI.Controllers
{
    [ApiController]
    [Authorize]
    [ApiVersion("1.0")]
    [ApiVersion("2.0")]
    [Route("api/v{version:apiVersion}/cities/{cityId}/pointsofinterest")]
    public class PointOfInterestController : ControllerBase
    {
        private readonly ILogger<PointOfInterestController> _logger;
        private readonly IMailService _mail;
        private readonly IMapper _mapper;
        private readonly ICityInfoRepository _cityInfoRepository;

        public PointOfInterestController(ILogger<PointOfInterestController> logger, IMailService mail, ICityInfoRepository cityInfoRepository,IMapper mapper)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mail = mail ?? throw new ArgumentNullException(nameof(mail));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _cityInfoRepository = cityInfoRepository ?? throw new ArgumentNullException(nameof(cityInfoRepository));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PointOfInterestDto>>> GetPoints(int cityId)
        {
            try
            {
                var cityName = User.Claims.FirstOrDefault(c => c.Type == "city")?.Value;

                if (!await _cityInfoRepository.CityNameMatchId(cityName, cityId))
                {
                    return Forbid();
                }

                var cityFound = await _cityInfoRepository.GetCityAsync(cityId, false);
                if (cityFound == null)
                {
                    _logger.LogInformation($"Can't find city with {cityId}");
                    return NotFound();
                }

                var pointsFound = await _cityInfoRepository.GetPointsOfInterestsAsync(cityId);

                return Ok(_mapper.Map<IEnumerable<PointOfInterestDto>>(pointsFound));

            } catch(Exception ex)
            {
                _logger.LogCritical($"Exception with points of city {cityId}", ex);
                return StatusCode(500, "Internal Server Error");
            }
            
        }

        [HttpGet("{pointId}", Name = "GetPoint")]
        public async Task<ActionResult<PointOfInterestDto>> GetPoint(int cityId, int pointId) {
            var cityFound = await _cityInfoRepository.GetCityAsync(cityId, false);
            if (cityFound == null)
            {
                _logger.LogInformation($"Can't find city with {cityId}");
                return NotFound();
            }

            var pointFound = await _cityInfoRepository.GetPointOfInterestAsync(cityId, pointId);
            if (pointFound == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<PointOfInterestDto>(pointFound));
        }

        [HttpPost]
        public async Task<ActionResult<PointOfInterestDto>> CreatePoint(int cityId, [FromBody] PointOfInterestCreationDto pointInfo)
        {
            var cityFound = await _cityInfoRepository.GetCityAsync(cityId, false);
            if (cityFound == null)
            {
                return NotFound();
            }

            var finalPoint = _mapper.Map<Entities.PointOfInterest>(pointInfo);

            await _cityInfoRepository.AddPointOfInterestAsync(cityId, finalPoint);

            await _cityInfoRepository.SaveChangesAsync();

            var returnPoint = _mapper.Map<PointOfInterestDto>(finalPoint);

            return CreatedAtRoute("GetPoint",
                new
                {
                    cityId = cityId,
                    pointId = returnPoint.Id
                },
                returnPoint);
        }

        [HttpPut("{pointId}")]
        public async Task<ActionResult> UpdatePoint(int cityId, int pointId, PointOfInterestUpdateDto pointInfo)
        {
            var cityFound = await _cityInfoRepository.GetCityAsync(cityId, true);
            if (cityFound == null)
            {
                return NotFound();
            }

            // Optimized version, no need for re-search for the city
            var pointFound = cityFound.PointsOfInterest.FirstOrDefault(x => x.Id == pointId);
            if (pointFound == null)
            {
                return NotFound();
            }

            _mapper.Map(pointInfo, pointFound);

            await _cityInfoRepository.SaveChangesAsync();

            return NoContent();

        }

        [HttpPatch("{pointId}")]
        // Using JsonPatch library to parse the patch document
        public async Task<ActionResult> PartialUpdatePoint(int cityId, int pointId, JsonPatchDocument<PointOfInterestUpdateDto> patchDoc)
        {
            var cityFound = await _cityInfoRepository.GetCityAsync(cityId, true);
            if (cityFound == null)
            {
                return NotFound();
            }

            var pointFound = cityFound.PointsOfInterest.FirstOrDefault(x => x.Id == pointId);
            if (pointFound == null)
            {
                return NotFound();
            }

            // Convert to UpdateDto
            var pointFoundType = _mapper.Map<PointOfInterestUpdateDto>(pointFound);

            // Apply patch changes
            patchDoc.ApplyTo(pointFoundType, ModelState);

            // Check if patchDoc is Valid
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Check if the operations resulted in an valid document
            if (!TryValidateModel(pointFoundType))
            {
                return BadRequest(ModelState);
            }

            // Copy changes to actual object
            _mapper.Map(pointFoundType, pointFound);
            await _cityInfoRepository.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{pointId}")]
        public async Task<ActionResult> DeletePoint(int cityId, int pointId)
        {
            // Implement inefficient alternative approach

            var cityFound = await _cityInfoRepository.GetCityAsync(cityId, false);
            if (cityFound == null)
            {
                return NotFound();
            }

            var pointFound = await _cityInfoRepository.GetPointOfInterestAsync(cityId, pointId);
            if (pointFound == null)
            {
                return NotFound();
            }

            _cityInfoRepository.DeletePointOfInterestAsync(pointFound);
            await _cityInfoRepository.SaveChangesAsync();

            _mail.SendMail($"Point {pointId}", "Deleted");

            return NoContent();

        }
    }
}
