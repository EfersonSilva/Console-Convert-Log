using EfersonTesting.EfersonSilva.Infrastructure.CustomException;
using EfersonTesting.EfersonSilva.Infrastructure.Settings;
using EfersonTesting.EfersonSilva.Interfaces;
using EfersonTesting.EfersonSilva.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace EfersonTesting.EfersonSilva_Tests.ValidationSettings_Test
{
    public class ValidationSettings_Test
    {
        private readonly Mock<ILogger<ValidationSettings>> _loggerMock;

        public ValidationSettings_Test()
        {
            _loggerMock = new Mock<ILogger<ValidationSettings>>();
        }

        [Fact]
        public void ValidationSettings_Sucess()
        {
            //Arrange
            var appSettingsMock = new Mock<IOptions<AppSettings>>();
            appSettingsMock.Setup(x => x.Value).Returns(new AppSettings());


            var validationSettings = new ValidationSettings(_loggerMock.Object, appSettingsMock.Object);

            //Act
            validationSettings.ValidationAppSettings();


            Assert.NotNull(appSettingsMock.Object.Value.Provider);
            Assert.False(appSettingsMock.Object.Value.EnablePathConsole);
            Assert.NotNull(appSettingsMock.Object.Value.FilePath);
            Assert.NotNull(appSettingsMock.Object.Value.LinkPath);

            Assert.Equal("TEST", appSettingsMock.Object.Value.Provider);
            Assert.Equal("\\test\\test.txt", appSettingsMock.Object.Value.FilePath);
            Assert.Equal("www.test.com", appSettingsMock.Object.Value.LinkPath);
        }
    }
}
