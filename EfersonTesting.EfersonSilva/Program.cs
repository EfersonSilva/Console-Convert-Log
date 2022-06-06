using EfersonTesting.EfersonSilva.CrossCutting.Ioc;

IConfiguration configuration = default;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        configuration = context.Configuration;
        new RegisterService(configuration).RegisterServices(services);
    })
    .Build();

await host.RunAsync();
