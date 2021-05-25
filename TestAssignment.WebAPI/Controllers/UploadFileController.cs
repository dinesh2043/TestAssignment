using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using TestAssignment.WebAPI.Helpers;

using TestAssignment.ProfanityCheck.Model;
using TestAssignment.ProfanityCheck.ProfanityService.Interfaces;

namespace TestAssignment.WebAPI.Controllers
{
    [Produces("application/json")]
    [Route("[controller]")]
    [ApiController]
    public class UploadFileController : ControllerBase
    {
        private readonly ILogger<UploadFileController> _logger;
        private readonly IProfanityServices _profanityServices;
       

        public UploadFileController(ILogger<UploadFileController> logger, IProfanityServices profanityServices)
        {
            _logger = logger;
            _profanityServices = profanityServices;
        }

        /// <summary>
        /// Upload Text File (*.txt). 
        /// </summary>
        /// <remarks>
        /// Asynchronous request to upload text file after profanity check. Only *.txt file less then or equal 2MB is supported.
        /// Endpoint counts only unique profanity words and processing time is calculated in milliseconds.
        /// </remarks>
        // POST UploadTextFile
        [HttpPost]
        //[RequestSizeLimit(2097152)]
        //[RequestFormLimits(BufferBodyLengthLimit = 2097152)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ProfanityCheckResult))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity, Type = typeof(ProfanityCheckResult))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(string))]
        public async Task<IActionResult> UploadTextFile(IFormFile file)
        {
            if (FileHelper.CheckFile(file))
            {
                string text = await FileHelper.ReadFormFileAsync(file);
                var profanityResult = await _profanityServices.CheckProfanity(text);
                if (profanityResult.ContainsProfanity)
                {
                    _logger.LogWarning("Uploaded file is not a valid text file.");
                    return UnprocessableEntity(profanityResult);
                }
                else
                {
                    _logger.LogInformation("Uploaded file is a valid text file.");
                    return Ok(profanityResult);
                }
            }
            else
            {
                return BadRequest(new { message = "Empty content or Invalid File. Only *.txt file less then or equal to 2MB is supported!" });
            }
        }
    }
}

