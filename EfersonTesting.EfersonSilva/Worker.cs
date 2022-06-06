using EfersonTesting.EfersonSilva.Infrastructure.CustomException;
using EfersonTesting.EfersonSilva.Interfaces;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace EfersonTesting.EfersonSilva
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IConvertLog _start;
        private readonly IValidationSettings _validationSettings;

        public Worker(ILogger<Worker> logger, IConvertLog start, IValidationSettings validationSettings)
        {
            _logger = logger ?? throw new Exception(nameof(logger));
            _start = start ?? throw new Exception(nameof(start));
            _validationSettings = validationSettings ?? throw new Exception(nameof(validationSettings));
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                _logger.LogInformation("Starting Service Analysis Logs...");

                _validationSettings.ValidationAppSettings();

                await _start.StartConvertLogAsync();

                _logger.LogInformation("Worker Finish: {time}", DateTimeOffset.Now);
            }
            catch (CustomException ex)
            {
                _logger.LogError("{Class}, {Method} - Error Exception, terminated unexpectedly! ", typeof(Worker), ExecuteAsync, ex.Message);
                throw new CustomException(ex.Message);
            }
        }
    }
}