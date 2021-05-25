using Moq;
using System.Collections.Generic;
using Xunit;

namespace TestAssignment.UnitTests
{
    using TestAssignment.ProfanityCheck;
    public  class ProfanityCheckerTests
    {
        private readonly MockRepository _mockRepository;
        private readonly ProfanityChecker _profanityChecker;

        public ProfanityCheckerTests()
        {
            _mockRepository = new MockRepository(MockBehavior.Strict);
            _profanityChecker = new ProfanityChecker();
        }

        [Fact]
        public void DetectAllProfanities_EmptyTextAndBandedWordsListPassed_ReturnsEmptyList()
        {
            //Arrange

            //Act
            var swearList = _profanityChecker.DetectAllProfanities(string.Empty, new List<string>());

            //Assert
            Assert.Empty(swearList);
            _mockRepository.VerifyAll();

        }

        [Fact]
        public void DetectAllProfanities_NullTextAndEmptyBandedWordsListPassed_ReturnsEmptyList()
        {
            //Arrange

            //Act
            var swearList = _profanityChecker.DetectAllProfanities(null, new List<string>());

            //Assert
            Assert.Empty(swearList);
            _mockRepository.VerifyAll();

        }

        [Fact]
        public void DetectAllProfanities_TextAndBandedWordsListPassed_ReturnsProfanityList()
        {
            //Arrange
            string text = "You are a complete twat and a dick.";
            List<string> bannedWordsList = new List<string>() { "twat", "dick" };

            //Act
            var swearList = _profanityChecker.DetectAllProfanities(text, bannedWordsList);

            //Assert
            Assert.Equal(2, swearList.Count);
            Assert.Equal("twat", swearList[0]);
            Assert.Equal("dick", swearList[1]);
            _mockRepository.VerifyAll();

        }

        [Fact]
        public void DetectAllProfanities_SwearWordsWithSpicialCharactersPassed_ReturnsProfanityList()
        {
            //Arrange
            string text = "You are, a complete twat, and a @dick:";
            List<string> bannedWordsList = new List<string>() { "twat", "dick" };

            //Act
            var swearList = _profanityChecker.DetectAllProfanities(text, bannedWordsList);

            //Assert
            Assert.Equal(2, swearList.Count);
            Assert.Equal("twat", swearList[0]);
            Assert.Equal("dick", swearList[1]);
            _mockRepository.VerifyAll();

        }

        [Fact]
        public void DetectAllProfanities_SwearWordsWithMixedCasePassed_ReturnsProfanityList()
        {
            //Arrange
            string text = "You are, a complete twat, and a @dick:";
            List<string> bannedWordsList = new List<string>() { "twat", "dick" };

            //Act
            var swearList = _profanityChecker.DetectAllProfanities(text, bannedWordsList);

            //Assert
            Assert.Equal(2, swearList.Count);
            Assert.Equal("twat", swearList[0]);
            Assert.Equal("dick", swearList[1]);
            _mockRepository.VerifyAll();

        }

        [Fact]
        public void DetectAllProfanities_SwearPhrasePassed_ReturnsProfanityList()
        {
            //Arrange
            string text = "2 girls 1 cup is my favourite video";
            List<string> bannedWordsList = new List<string>() { "twat", "dick", "2 girls 1 cup" };

            //Act
            var swearList = _profanityChecker.DetectAllProfanities(text, bannedWordsList);

            //Assert
            Assert.Single(swearList);
            Assert.Equal("2 girls 1 cup", swearList[0]);
            _mockRepository.VerifyAll();

        }

        [Fact]
        public void DetectAllProfanities_SwearPhraseAndSwearWordsPassed_ReturnsProfanityList()
        {
            //Arrange
            string text = "2 girls 1 cup is my favourite twatting video";
            List<string> bannedWordsList = new List<string>() { "twatting", "dick", "2 girls 1 cup" };

            //Act
            var swearList = _profanityChecker.DetectAllProfanities(text, bannedWordsList);

            //Assert
            Assert.Equal(2, swearList.Count);
            Assert.Equal("2 girls 1 cup", swearList[0]);
            Assert.Equal("twatting", swearList[1]);
            _mockRepository.VerifyAll();

        }
        [Fact]
        public void DetectAllProfanities_SwearPhraseAndSwearWordsWithMatchPassed_ReturnsProfanityList()
        {
            //Arrange
            string text = "2 girls 1 cup is my favourite twatting video";
            List<string> bannedWordsList = new List<string>() { "twatting", "dick", "2 girls 1 cup" };

            //Act
            var swearList = _profanityChecker.DetectAllProfanities(text, bannedWordsList, true);

            //Assert
            Assert.Equal(2, swearList.Count);
            Assert.Equal("2 girls 1 cup", swearList[0]);
            Assert.Equal("twatting", swearList[1]);
            _mockRepository.VerifyAll();

        }

        [Fact]
        public void DetectAllProfanities_SwearWordsLikeScunthorpeAndPenistonePassed_ReturnsEmptyProfanityList()
        {
            //Arrange
            string text = "Scunthorpe penistone ";
            List<string> bannedWordsList = new List<string>() { "twatting", "dick", "2 girls 1 cup", "penis", "cunt" };

            //Act
            var swearList = _profanityChecker.DetectAllProfanities(text, bannedWordsList);

            //Assert
            Assert.Empty(swearList);
            _mockRepository.VerifyAll();

        }

    }
}
