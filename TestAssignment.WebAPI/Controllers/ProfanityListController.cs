using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

using TestAssignment.DAL.Repositories.Interfaces;

namespace TestAssignment.WebAPI.Controllers
{
    [Produces("application/json")]
    [Route("[controller]")]
    [ApiController]
    public class ProfanityListController : ControllerBase
    {
        private readonly ILogger<ProfanityListController> _logger;
        private readonly IProfanityListRepository _profanityListRepository;

        public ProfanityListController(ILogger<ProfanityListController> logger, IProfanityListRepository profanityListRepository)
        {
            _logger = logger;
            _profanityListRepository = profanityListRepository;
        }

        /// <summary>
        /// Asynchronous get request
        /// </summary>
        /// <remarks>To get a profanity list.(I have used Profanity Detector nuget profanity list.)
        /// </remarks>
        // GET ProfanityList
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(string))]
        public async Task<IActionResult> GetProfanityList()
        {
            var profanityList = await _profanityListRepository.GetProfanityList();
            if(profanityList!= null)
            {
                _logger.LogInformation("Profanity List is successfully returned.");
                return Ok(profanityList);
            }
            else
            {
                _logger.LogWarning("Profanity List does not exists.");
                return NotFound("Profanity List not found.");
            }
        }

        /// <summary>
        /// Asynchronous post request
        /// </summary>
        /// <remarks>To add a profanity to the list. (Profanity Detector NuGet's Profanity List is used.)
        /// </remarks>
        // POST ProfanityList
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
        public async Task<IActionResult> AddProfanity(string profanity)
        {
            var addProfanityResult = await _profanityListRepository.AddProfanity(profanity.ToLower());
            if (addProfanityResult)
            {
                _logger.LogInformation("Profanity is successfully added to the ProfanityList.");
                return Ok("Profanity is successfully added to the ProfanityList.");
            }
            else
            {
                _logger.LogWarning("Profanity already exists in ProfanityList.");
                return BadRequest("Profanity already exists in ProfanityList.");
            }
        }

        /// <summary>
        /// Asynchronous delete request
        /// </summary>
        /// <remarks>To delete a profanity from the list.(Profanity Detector NuGet's Profanity List is used.)
        /// </remarks>
        // Delete ProfanityList
        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
        public async Task<IActionResult> DeleteProfanity(string profanity)
        {
            var addProfanityResult = await _profanityListRepository.DeleteProfanity(profanity.ToLower());
            if (addProfanityResult)
            {
                _logger.LogInformation("Profanity is successfully deleted from the ProfanityList.");
                return Ok("Profanity is successfully deleted from the ProfanityList.");
            }
            else
            {
                _logger.LogWarning("Profanity does not exists in ProfanityList.");
                return BadRequest("Profanity does not exists in ProfanityList.");
            }
        }

    }
}
