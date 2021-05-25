using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace TestAssignment.UnitTests
{
    using TestAssignment.DAL.Repositories.Interfaces;
    using TestAssignment.ProfanityCheck;
    using TestAssignment.ProfanityCheck.Model;
    using TestAssignment.ProfanityCheck.ProfanityService;
    using TestAssignment.ProfanityCheck.ProfanityService.Interfaces;

    public class ProfanityServiceTests
    {
        private readonly MockRepository _mockRepository;
        private readonly IProfanityServices _profanityServices;

        private readonly Mock<NullLogger<ProfanityServices>> _mockProfanityServicesLogger;
        private readonly Mock<IProfanityListRepository> _mocKProfanityListRepository;

        public ProfanityServiceTests()
        {
            _mockRepository = new MockRepository(MockBehavior.Strict);
            _mockProfanityServicesLogger = _mockRepository.Create<NullLogger<ProfanityServices>>();
            _mocKProfanityListRepository = _mockRepository.Create<IProfanityListRepository>();
            _profanityServices = new ProfanityServices(_mockProfanityServicesLogger.Object, _mocKProfanityListRepository.Object);
        }

        [Fact]
        public async Task CheckProfanity_CleanTextPassed_ReturnsContainsProfanityFalse()
        {
            //Arrange
            string text = "Clean text is passed.";
            List<string> bandedWordsList = new List<string> { "damn", "shit", "ass" };

            var profanityCheckResult = new ProfanityCheckResult
            {
                ContainsProfanity = false
            };

            _mocKProfanityListRepository.Setup(repo => repo.GetProfanityList()).ReturnsAsync(bandedWordsList);
            var mockProfanityChecker = _mockRepository.Create<ProfanityChecker>();
            mockProfanityChecker.Object.DetectAllProfanities(text, bandedWordsList);

            //Act
            var result = await _profanityServices.CheckProfanity(text);

            //Assert
            Assert.Equal(profanityCheckResult.ContainsProfanity, result.ContainsProfanity);
            _mockRepository.VerifyAll();
        }

        [Fact]
        public async Task CheckProfanity_ProfanityTextPassed_ReturnsContainsProfanityTrueWithProfanityCount()
        {
            //Arrange
            string text = "You are damn right? Is it a shit?";
            List<string> bandedWordsList = new List<string> { "damn", "shit", "ass" };
            var profanityCheckResult = new ProfanityCheckResult
            {
                ContainsProfanity = true,
                ProfanityWordCount = 2
            };

            _mocKProfanityListRepository.Setup(repo => repo.GetProfanityList()).ReturnsAsync(bandedWordsList);
            var mockProfanityChecker = _mockRepository.Create<ProfanityChecker>();
            mockProfanityChecker.Object.DetectAllProfanities(text, bandedWordsList);

            //Act
            var result = await _profanityServices.CheckProfanity(text);

            //Assert
            Assert.Equal(profanityCheckResult.ContainsProfanity, result.ContainsProfanity);
            Assert.Equal(profanityCheckResult.ProfanityWordCount, result.ProfanityWordCount);
            _mockRepository.VerifyAll();
        }
    }
}
