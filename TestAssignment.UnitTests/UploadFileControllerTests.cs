using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace TestAssignment.UnitTests
{
    using TestAssignment.ProfanityCheck.Model;
    using TestAssignment.ProfanityCheck.ProfanityService.Interfaces;
    using TestAssignment.WebAPI.Controllers;

    public class UploadFileControllerTests
    {
        private readonly MockRepository _mockRepository;
        private readonly UploadFileController _uploadFileController;

        private readonly Mock<IProfanityServices> _mocKProfanityServices;

        /***
         * The NullLogger<T> implements ILogger<T> and it is a minimalistic logger that does nothing. In other words, it doesn’t log 
         * messages to any place and it swallows exceptions. It is worth mentioning that both the ILogger<T> interface and the 
         * NullLogger<T> class are included in the NuGet package Microsoft.Extensions.Logging.Abstractions.
         * 
         * In a lot of cases, the logging system doesn’t participate in business logic or business rules. Therefore, we don’t need to care 
         * much about what the ILogger<T> does in the system under test (SUT). Then in these scenarios, the NullLogger<T> object is a 
         * perfect replacement of the ILogger<T> object in unit tests. The following code snippet shows an example.
         * 
         * ***/
        private readonly Mock<NullLogger<UploadFileController>> _mocKLogger;

        public UploadFileControllerTests()
        {
            _mockRepository = new MockRepository(MockBehavior.Strict);

            _mocKProfanityServices = _mockRepository.Create<IProfanityServices>();
            _mocKLogger = _mockRepository.Create<NullLogger<UploadFileController>>();

            _uploadFileController = new UploadFileController(_mocKLogger.Object, _mocKProfanityServices.Object);
        }

        [Fact]
        public async Task UploadTextFile_BigFileWithNullFileNameAndNullContentPassed_ReturnsBadRequest()
        {
            //Arrange
            string expectedFileContents = null;
            string expectedFileName = null;
            var file = ArrangeFile(expectedFileName, expectedFileContents);
            file.Setup(x => x.Length).Returns(2197152);
            //Act
            var badResponse = await _uploadFileController.UploadTextFile(file.Object);

            //Assert
            Assert.IsType<BadRequestObjectResult>(badResponse);
            _mockRepository.VerifyAll();

        }

        [Fact]
        public async Task UploadTextFile_NullContentPassed_ReturnsBadRequest()
        {
            //Arrange
            string expectedFileContents = null;
            string expectedFileName = "test.txt";
            var file = ArrangeFile(expectedFileName, expectedFileContents);

            //Act
            var badResponse = await _uploadFileController.UploadTextFile(file.Object);

            //Assert
            Assert.IsType<BadRequestObjectResult>(badResponse);
            _mockRepository.VerifyAll();

        }

        [Fact]
        public async Task UploadTextFile_InvalidFileTypePassed_ReturnsBadRequest()
        {
            //Arrange
            string expectedFileContents = "This is the test text for PDF file.";
            string expectedFileName = "test.pdf";
            var file = ArrangeFile(expectedFileName, expectedFileContents);

            //Act
            var badResponse = await _uploadFileController.UploadTextFile(file.Object);

            //Assert
            Assert.IsType<BadRequestObjectResult>(badResponse);
            _mockRepository.VerifyAll();

        }

        [Fact]
        public async Task UploadTextFile_EmptyFilePassed_ReturnsBadRequest()
        {
            //Arrange
            string expectedFileContents = "";
            string expectedFileName = "test.txt";
            var file = ArrangeFile(expectedFileName, expectedFileContents).Object;

            //Act
            var badResponse = await _uploadFileController.UploadTextFile(file);

            //Assert
            Assert.IsType<BadRequestObjectResult>(badResponse);
            _mockRepository.VerifyAll();
        }

        [Fact]
        public async Task UploadTextFile_ValidFilePassed_ReturnsOkResponse()
        {
            //Arrange
            string expectedFileContents = "This is the expected file contents!";
            string expectedFileName = "test.txt";
            var file = ArrangeFile(expectedFileName, expectedFileContents).Object;

            _mocKProfanityServices.Setup(service => service.CheckProfanity(It.IsAny<string>())).ReturnsAsync(new ProfanityCheckResult());

            //Act            
            var okResponse = await _uploadFileController.UploadTextFile(file);

            //Assert
            Assert.IsType<OkObjectResult>(okResponse);
            _mockRepository.VerifyAll();
        }

        [Fact]
        public async Task UploadTextFile_InvalidFilePassed_ReturnsUnprocessableEntityResponse()
        {
            //Arrange
            string expectedFileContents = "This is a small child with small dick with little shit.What the fuck he is supposed to do?";
            string expectedFileName = "test.txt";
            var file = ArrangeFile(expectedFileName, expectedFileContents).Object;

            var expectedProfanityCheckResult = new ProfanityCheckResult()
            {
                ContainsProfanity = true,
                ProfanityWordCount = 3,
            };

            _mocKProfanityServices.Setup(service => service.CheckProfanity(It.IsAny<string>())).ReturnsAsync(expectedProfanityCheckResult);

            //Act
            var unprocessableResponse = await _uploadFileController.UploadTextFile(file);

            //Assert
            Assert.IsType<UnprocessableEntityObjectResult>(unprocessableResponse);
            _mockRepository.VerifyAll();
        }

        private Mock<IFormFile> ArrangeFile(string fileName, string content)
        {
            var fileMock = new Mock<IFormFile>();
            var ms = new MemoryStream();
            var writer = new StreamWriter(ms);
            writer.Write(content);
            writer.Flush();
            ms.Position = 0;
            fileMock.Setup(_ => _.OpenReadStream()).Returns(ms);
            fileMock.Setup(_ => _.FileName).Returns(fileName);
            fileMock.Setup(_ => _.Length).Returns(ms.Length);
            fileMock.Setup(_ => _.ContentType).Returns("Text");

            return fileMock;
        }
    }
}
