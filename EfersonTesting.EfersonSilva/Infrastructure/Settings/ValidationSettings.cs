using EfersonTesting.EfersonSilva.Interfaces;
using EfersonTesting.EfersonSilva.Settings;
using Microsoft.Extensions.Options;
using System.Reflection;

namespace EfersonTesting.EfersonSilva.Infrastructure.Settings
{
    public class ValidationSettings : IValidationSettings
    {
        private readonly ILogger<ValidationSettings> _logger;
        private AppSettings _appSettings;

        public ValidationSettings(ILogger<ValidationSettings> logger, IOptions<AppSettings> appSettings)
        {
            _logger = logger;
            _appSettings = appSettings.Value;
        }

        public void ValidationAppSettings()
        {
            LoadDefaultSettings(_appSettings);
        }

        private void LoadDefaultSettings(AppSettings appSettings)
        {
            _logger.LogInformation("Loading Default AppSettings.");

            var defaultSettings = new DefaultAppSettings();
            PropertyInfo[] propertiesDefault = defaultSettings.GetType().GetProperties();

            foreach (var p in propertiesDefault)
            {
                var propertyAppSettings = appSettings.GetType().GetProperty(p.Name);

                var property = propertyAppSettings.GetValue(appSettings);

                if (property is null)
                {
                    _logger.LogInformation("Property {p.Name} is null.", p.Name);

                    if (propertyAppSettings.CanWrite)
                        propertyAppSettings.SetValue(appSettings, p.GetValue(defaultSettings), null);
                    else
                        _logger.LogCritical("Property {propertyAppSettings.Name}, Can't be Writen.", propertyAppSettings.Name);
                }
            }

            _logger.LogInformation("Loaded Defaul Settings.");
        }
    }
}
