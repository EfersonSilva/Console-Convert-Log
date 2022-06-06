using EfersonTesting.EfersonSilva;
using EfersonTesting.EfersonSilva.Infrastructure.CustomException;
using EfersonTesting.EfersonSilva.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;

namespace EfersonTesting.EfersonSilva_Tests
{
    public class Worker_Test
    {
        private readonly Mock<ILogger<Worker>> _loggerMock;
        private readonly Mock<IConvertLog> _startMock;
        private readonly Mock<IValidationSettings> _validationMock;
        public Worker_Test()
        {
            _loggerMock = new Mock<ILogger<Worker>>();
            _startMock = new Mock<IConvertLog>();
            _validationMock = new Mock<IValidationSettings>();
        }

        [Fact]
        public void Worker_Constructor_Exception()
        {
            Assert.Equal(new Exception("logger").Message, Assert.Throws<Exception>(() => new Worker(null, _startMock.Object, _validationMock.Object)).Message);
            Assert.Equal(new Exception("start").Message, Assert.Throws<Exception>(() => new Worker(_loggerMock.Object, null, _validationMock.Object)).Message);
            Assert.Equal(new Exception("validationSettings").Message, Assert.Throws<Exception>(() => new Worker(_loggerMock.Object, _startMock.Object, null)).Message);
        }

        [Fact]
        public void Worker_Success_Tests()
        {
            //Act
            var worker = new Worker(_loggerMock.Object, _startMock.Object, _validationMock.Object);
            worker.StartAsync(It.IsAny<CancellationToken>());

            //Assert
            _validationMock.Verify(x => x.ValidationAppSettings(), Times.Once);
            _startMock.Verify(x => x.StartConvertLogAsync(), Times.Once());
            
        }

        [Fact]
        public async Task Worker_Exception_ValidationAppSettings()
        {
            //Arrange
            _validationMock.Setup(x => x.ValidationAppSettings()).Throws(new CustomException(""));


            //Act
            var worker = new Worker(_loggerMock.Object, _startMock.Object, _validationMock.Object);


            //Assert
            await Assert.ThrowsAsync<CustomException>(() => worker.StartAsync(It.IsAny<CancellationToken>()));
        }

        [Fact]
        public async Task Worker_Exception_StartProgramAsync()
        {
            //Arrange
            _startMock.Setup(x => x.StartConvertLogAsync()).Throws(new CustomException(""));

            //Act
            var worker = new Worker(_loggerMock.Object, _startMock.Object, _validationMock.Object);


            //Assert
            await Assert.ThrowsAsync<CustomException>(() => worker.StartAsync(It.IsAny<CancellationToken>()));
        }

    }
}