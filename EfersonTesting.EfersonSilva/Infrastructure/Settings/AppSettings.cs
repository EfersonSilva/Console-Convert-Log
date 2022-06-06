using EfersonTesting.EfersonSilva.Interfaces;

namespace EfersonTesting.EfersonSilva.Settings
{
    public class AppSettings : IAppSettings
    {
        public const string SectionName = "Settings";
        public string? LinkPath { get; set; }
        public bool EnablePathConsole { get; set; }
        public string? Provider { get; set; }
        public string? FilePath { get; set; }
    }

    public class DefaultAppSettings
    {
        public string LinkPath { get; }
        public bool EnablePathConsole { get; }
        public string Provider { get; }
        public string FilePath { get; }

        public DefaultAppSettings()
        {
            LinkPath = "www.test.com";
            Provider = "TEST";
            EnablePathConsole = false;
            FilePath = "\\test\\test.txt";
        }
    }
}
