namespace EfersonTesting.EfersonSilva.Interfaces
{
    public interface IAppSettings
    {
        public string? LinkPath { get; set; }
        public bool EnablePathConsole { get; set; }
        public string? Provider { get; set; }
        public string? FilePath { get; set; }
    }
}
