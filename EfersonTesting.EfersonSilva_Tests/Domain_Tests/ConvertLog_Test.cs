using EfersonTesting.EfersonSilva.Domain;
using EfersonTesting.EfersonSilva.Infrastructure.CustomException;
using EfersonTesting.EfersonSilva.Interfaces;
using EfersonTesting.EfersonSilva.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace EfersonTesting.EfersonSilva_Tests.Domain_Tests
{
    public class ConvertLog_Test
    {
        private readonly Mock<ILogger<ConvertLog>> _loggerMock;
        private readonly Mock<IOptions<AppSettings>> _appSettingsMock;
        private readonly Mock<IHttpClientRequestLog> _httpClientRequestLog;

        public ConvertLog_Test()
        {
            _loggerMock = new Mock<ILogger<ConvertLog>>();
            _appSettingsMock = new Mock<IOptions<AppSettings>>();
            _appSettingsMock.Setup(x => x.Value).Returns(new AppSettings());
            _httpClientRequestLog = new Mock<IHttpClientRequestLog>();
        }

        [Fact]
        public void ConvertLog_Constructor_Test()
        {
            Assert.Equal(new Exception("logger").Message, Assert.Throws<Exception>(() => new ConvertLog(null, null, null)).Message);
            Assert.Equal(new Exception("appSettingValue").Message, Assert.Throws<CustomException>(() => new ConvertLog(_loggerMock.Object, new Mock<IOptions<AppSettings>>().Object, null)).Message);
            Assert.Equal(new Exception("httpClientRequestLog").Message, Assert.Throws<CustomException>(() => new ConvertLog(_loggerMock.Object, _appSettingsMock.Object, null)).Message);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task ConvertLog_Test_SuccessAsync(bool enableConsole)
        {
            //Arrange
            string filePath = "\\test\\test.txt";
            var appSetting = GetAppSetting(enableConsole);
            var data = GetData();

            _httpClientRequestLog.Setup(x => x.HttpClientGetAsync(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult(data));

            _appSettingsMock.Setup(x => x.Value).Returns(appSetting);
            Console.SetIn(new StringReader("convert https://teste.com/teste.txt "+ filePath));
            var convertLog = new ConvertLog(_loggerMock.Object, _appSettingsMock.Object, _httpClientRequestLog.Object);

            //Act
            await convertLog.StartConvertLogAsync();

            string logs = File.ReadAllText(filePath);

            //Assert
            _httpClientRequestLog.Verify(x => x.HttpClientGetAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
            Assert.NotNull(logs);

            File.Delete(filePath);
        }

        public string GetData()
        {
            const string aspas = "\"";
            return "312|200|HIT|" + aspas + "GET /robots.txt HTTP/1.1" + aspas + "|100.2 " + Environment.NewLine +
                "101|200|MISS|" + aspas + "POST /myImages HTTP/1.1" + aspas + " |319.4" + Environment.NewLine +
                "199|404|MISS|" + aspas + "GET /not-found HTTP/1.1" + aspas + " |142.9" + Environment.NewLine +
                "312|200|INVALIDATE|" + aspas + "GET /robots.txt HTTP/1.1" + aspas + "|245.1";

        }

        private AppSettings GetAppSetting(bool enableConsole)
        {
            return new AppSettings
            {
                EnablePathConsole = enableConsole,
                FilePath = "\\test\\test.txt",
                LinkPath = "https://teste.com/teste.txt",
                Provider = "TEST"
            };
        }

    }
}
