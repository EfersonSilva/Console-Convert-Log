using EfersonTesting.EfersonSilva.Domain;
using EfersonTesting.EfersonSilva.Infrastructure.HttpClientRequest;
using EfersonTesting.EfersonSilva.Infrastructure.Settings;
using EfersonTesting.EfersonSilva.Interfaces;
using EfersonTesting.EfersonSilva.Settings;

namespace EfersonTesting.EfersonSilva.CrossCutting.Ioc
{
    public class RegisterService
    {
        private readonly IConfiguration Configuration;
        public RegisterService(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        public void RegisterServices(IServiceCollection services)
        {
            services.AddOptions();
            services.AddSingleton<IConvertLog, ConvertLog>();
            services.AddSingleton<IValidationSettings, ValidationSettings>();
            services.AddTransient<IHttpClientRequestLog, HttpClientRequestLog>();   
            services.AddHostedService<Worker>();

            GetConfiguration(services);
        }

        public void GetConfiguration(IServiceCollection services)
        {
            services.Configure<AppSettings>(Configuration.GetSection(AppSettings.SectionName));
        }
    }
}
