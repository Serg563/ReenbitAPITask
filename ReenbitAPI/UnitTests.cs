using Azure;
using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using ReenbitAPI.Controllers;
using ReenbitAPI.Models;
using ReenbitAPI.Services;
using System.Threading;

namespace ReenbitAPI
{
    [TestFixture]
    public class UnitTests
    {
        private Mock<BlobServiceClient> _blobServiceClientMock;
        private BlobService _blobService;

        [SetUp]
        public void Setup()
        {
            _blobServiceClientMock = new Mock<BlobServiceClient>();
            _blobService = new BlobService(_blobServiceClientMock.Object);
        }

        [Test]
        public async Task BlobService_GetBlob_ShouldGetBlob()
        {
            const string blobName = "testBlob";
            const string container = "testContainer";

            var blobContainerClientMock = new Mock<BlobContainerClient>();
            var blobClientMock = new Mock<BlobClient>();
            blobClientMock.Setup(c => c.Uri).Returns(new Uri("https://example.com/blob"));
            blobContainerClientMock.Setup(c => c.GetBlobClient(blobName)).Returns(blobClientMock.Object);
            _blobServiceClientMock.Setup(c => c.GetBlobContainerClient(container)).Returns(blobContainerClientMock.Object);

            // Act
            var blobUri = await _blobService.GetBlob(blobName, container);

            // Assert
            Assert.AreEqual("https://example.com/blob", blobUri);

        }
        [Test]
        public async Task FormFileController_UpdateBlob_ReturnsUpdatedBlob()
        {
   
            var form = new UploadForm
            {
                Email = "serg@gmail.com",
                File = new FormFile(Stream.Null, 0, 0, "test.txt", "test.txt")
            };
            var blobServiceMock = new Mock<IBlobService>();
            blobServiceMock.Setup(x => x.UploadBlob(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<IFormFile>()))
                .ReturnsAsync("https://example.com/test.txt");
            var controller = new FormFileController(blobServiceMock.Object);

            // Act
            var result = await controller.UploadFile(form);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
        }
        [Test]
        public async Task FormFileController_UpdateBlob_ReturnsBadRequest()
        {
            // Arrange
            var form = new UploadForm
            {
                Email = null,
                File = null
            };
            var blobServiceMock = new Mock<IBlobService>();
            var controller = new FormFileController(blobServiceMock.Object);

            // Act
            var result = await controller.UploadFile(form);

            // Assert
            Assert.IsInstanceOf<BadRequestResult>(result);
        }

        [Test]
        public async Task FormFileController_GetEmail_ReturnOkResult()
        {
            // Arrange
            var form = new UploadForm
            {
                Email = "serg@gmail.com",
                File = new FormFile(Stream.Null, 0, 0, "test.txt", "test.txt")
            };
            var blobServiceMock = new Mock<IBlobService>();
            var controller = new FormFileController(blobServiceMock.Object);

            // Act
            var result = controller.GetEmail();

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
        }

        [TestCase("")]
        [TestCase(null)]
        public async Task FormFileController_GetEmail_ReturnBadRequest(string storedEmail)
        {
            // Arrange
            var form = new UploadForm
            {
                Email = "serg@gmail.com",
                File = new FormFile(Stream.Null, 0, 0, "test.txt", "test.txt")
            };
            var blobServiceMock = new Mock<IBlobService>();
            var controller = new FormFileController(blobServiceMock.Object);
            controller.storedEmail = storedEmail;

            // Act
            var result = controller.GetEmail();

            // Assert
            Assert.IsInstanceOf<BadRequestResult>(result);
        }

    }
}
