using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

using TestAssignment.ProfanityCheck.Model;
using TestAssignment.ProfanityCheck.ProfanityService.Interfaces;
using TestAssignment.DAL.Repositories.Interfaces;

namespace TestAssignment.ProfanityCheck.ProfanityService
{

    public class ProfanityServices : IProfanityServices
    {
        private readonly ILogger<ProfanityServices> _logger;
        private readonly IProfanityListRepository _profanityListRepository;

        public ProfanityServices(ILogger<ProfanityServices> logger, IProfanityListRepository profanityListRepository)
        {
            _logger = logger;
            _profanityListRepository = profanityListRepository;
        }

        public async Task<ProfanityCheckResult> CheckProfanity(string text)
        {
            var checker = new ProfanityChecker();
            //Create the new stopwatch
            Stopwatch stopwatch = new Stopwatch();

            //Start timmer
            stopwatch.Start();

            var swearWordsList = checker.DetectAllProfanities(text, await _profanityListRepository.GetProfanityList());

            //Stop timer
            stopwatch.Stop();

            ProfanityCheckResult profanityCheckResult = new ProfanityCheckResult();
            if(swearWordsList.Count() > 0)
            {
                profanityCheckResult.ContainsProfanity = true;
                profanityCheckResult.ProfanityWordCount = swearWordsList.Count();
                profanityCheckResult.ProcessingTime = stopwatch.Elapsed.TotalMilliseconds;

                _logger.LogInformation("Supplied text file contains Profanity text.");
            }
            else
            {
                profanityCheckResult.ContainsProfanity = false;
                profanityCheckResult.ProfanityWordCount = 0;
                profanityCheckResult.ProcessingTime = stopwatch.Elapsed.TotalMilliseconds;

                _logger.LogInformation("Supplied text file is a Clean text.");
            }
            return profanityCheckResult;
        }
    }
}
