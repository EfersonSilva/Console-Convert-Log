using EfersonTesting.EfersonSilva.Infrastructure.CustomException;
using EfersonTesting.EfersonSilva.Infrastructure.HttpClientRequest;
using EfersonTesting.EfersonSilva.Interfaces;
using EfersonTesting.EfersonSilva.Models;
using EfersonTesting.EfersonSilva.Settings;
using Microsoft.Extensions.Options;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace EfersonTesting.EfersonSilva.Domain
{
    public class ConvertLog : IConvertLog
    {
        private readonly ILogger<ConvertLog> _logger;
        private readonly IHttpClientRequestLog _httpClientRequestLog;
        private readonly AppSettings _appSettings;

        public ConvertLog(ILogger<ConvertLog> logger, IOptions<AppSettings> appSettings, IHttpClientRequestLog httpClientRequestLog)
        {
            _logger = logger ?? throw new Exception(nameof(logger));
            _appSettings = appSettings.Value ?? throw new CustomException("appSetting" + nameof(appSettings.Value));
            _httpClientRequestLog = httpClientRequestLog ?? throw new CustomException(nameof(httpClientRequestLog));
        }

        /// <summary>
        /// Method to start the application.
        /// </summary>
        public async Task StartConvertLogAsync()
        {
            await ConsumerHttpAsync(_appSettings);
        }

        public async Task ConsumerHttpAsync(AppSettings appSettings)
        {
            try
            {
                string parameter;
                string filePath;
                string path = GetPathAsync(appSettings, out parameter, out filePath);

                string content = await _httpClientRequestLog.HttpClientGetAsync(path, parameter);

                List<CDNLog> listCdn = CleanLogCDN(content);

                _logger.LogInformation("Started to Convert Log.");

                await DoLog(listCdn, filePath);

                _logger.LogInformation("Log conversion finished...");

                await Task.CompletedTask;
            }
            catch (CustomException ex)
            {
                _logger.LogError(" {Class} - {Method} Error: ", typeof(ConvertLog), DoLog, ex.Message);
            }
        }

        public List<CDNLog> CleanLogCDN(string log)
        {
            string[] linesLog = log.Split("\r\n");
            List<CDNLog> listCdn = new List<CDNLog>();
            CDNLog cdnLog;

            foreach (var line in linesLog)
            {
                try
                {
                    if (!string.IsNullOrEmpty(line))
                    {
                        string[] partOfline = line.Split("|");

                        string parts = Regex.Replace(partOfline[3].ToString(), "[@,\\\"'\\\\]", string.Empty);
                        string[] pathLine = parts.Split(" ");

                        var statusCode = int.Parse(partOfline[1].ToString());
                        var cacheStatus = partOfline[2].ToString();
                        var httpMethod = pathLine[0].ToString();
                        var uriPath = pathLine[1].ToString();
                        int timeTaken = int.Parse(partOfline.Last().Substring(0, 3));


                        cdnLog = new CDNLog
                        {
                            Provider = _appSettings.Provider,
                            ResponseSize = int.Parse(partOfline[0].ToString()),
                            StatusCode = int.Parse(partOfline[1].ToString()),
                            CacheStatus = partOfline[2].ToString(),
                            HttpMethod = pathLine[0].ToString(),
                            UriPath = pathLine[1].ToString(),
                            TimeTaken = int.Parse(partOfline.Last().Substring(0, 3))
                        };

                        listCdn.Add(cdnLog);
                    }
                }
                catch (CustomException ex)
                {
                    _logger.LogError(" {Class} - {Method} Error: ", typeof(ConvertLog), CleanLogCDN, ex.Message);
                    return new List<CDNLog>();
                }
            }

            return listCdn;
        }

        public string GetPathAsync(AppSettings appSettings, out string parameter, out string filePath)
        {
            try
            {
                string linkLog;
                if (appSettings.EnablePathConsole)
                {
                    string path;

                    Console.WriteLine();
                    _logger.LogInformation("Enter the link and folder path: ");
                    path = Console.ReadLine();

                    if (string.IsNullOrEmpty(path))
                        _logger.LogError("Path is empty!");

                    (linkLog, parameter, filePath) = FormatPathConsole(appSettings, path);

                    if (string.IsNullOrEmpty(linkLog) || string.IsNullOrEmpty(parameter) || string.IsNullOrEmpty(filePath))
                        _logger.LogInformation("There is an incorrect parameter.");
                }
                else
                {
                    string[] vetPath = appSettings.LinkPath.Split("/");
                    parameter = vetPath.Last();

                    filePath = appSettings.FilePath.Replace("/", "\\");

                    linkLog = appSettings.LinkPath;
                }

                return linkLog ?? string.Empty;
            }
            catch (CustomException ex)
            {
                _logger.LogError(" {Class} - {Method} Error: ", typeof(ConvertLog), GetPathAsync, ex.Message);
                parameter = string.Empty;
                filePath = string.Empty;

                return string.Empty;
            }
        }

        private static (string linkLog, string parameter, string filePath) FormatPathConsole(AppSettings appSettings, string path)
        {
            string[] vetManyPath = path.Split(" ");

            string linkLog = vetManyPath[0];

            string[] vetLink = linkLog.Split("/");

            string parameter = vetLink.Last();

            string filePath = appSettings.FilePath.Replace("/", "\\");

            return (linkLog, parameter, filePath);
        }

        public Task DoLog(List<CDNLog> listCdn, string filePath)
        {
            try
            {
                StringBuilder text;

                var vetPath = filePath.Split("\\");
                vetPath = vetPath.Where((source, index) => index != (vetPath.Count() - 1)).ToArray();
                string folderPath = String.Join("\\", vetPath);

                if (!Directory.Exists(folderPath))
                    Directory.CreateDirectory(folderPath);

                if (!File.Exists(filePath))
                {
                    text = new StringBuilder();
                    text.AppendLine("#Version: 1.0");
                    text.AppendLine("#Date: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss", new CultureInfo("pt-BR")));
                    text.AppendLine("#Fields: provider http-method status-code uri-path time-taken response-size cache-status");

                    File.WriteAllText(filePath, text.ToString(), Encoding.UTF8);
                }

                text = new StringBuilder();
                foreach (var line in listCdn)
                {
                    if (line.CacheStatus == "INVALIDATE")
                        line.CacheStatus = "REFRESH_HIT";

                    text.AppendLine($"\"{line.Provider}\" {line.HttpMethod} {line.StatusCode} {line.UriPath} {line.TimeTaken} {line.ResponseSize} {line.CacheStatus}");
                }

                text.Append(Environment.NewLine);
                File.AppendAllText(filePath, text.ToString(), Encoding.UTF8);

                _logger.LogInformation(text.ToString(), Environment.NewLine);

                _logger.LogInformation("Path: {filePath}", filePath);

                return Task.CompletedTask;
            }
            catch (CustomException ex)
            {
                _logger.LogError(" {Class} - {Method} Error: ", typeof(ConvertLog), DoLog, ex.Message);
                return Task.CompletedTask;
            }
        }

    }
}
