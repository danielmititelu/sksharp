using WorkerService;
using SkSharp;

var configuration = new ConfigurationBuilder()
      .AddJsonFile("appsettings.json")
      .AddJsonFile("appsettings.Secret.json", optional: true)
      .Build();

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration(c =>
    {
        c.AddConfiguration(configuration);
    })
    .ConfigureServices(services =>
    {
        var username = configuration.GetValue<string>("username");
        var password = configuration.GetValue<string>("password");
        var cacheFilePath = configuration.GetValue<string>("cacheFilePath");
        if (username == null || password == null || cacheFilePath == null)
        {
            throw new ArgumentException("Username, password or cacheFilePath are missing");
        }

        services.AddSkSharpServices(username, password, cacheFilePath);
        services.AddHostedService<Worker>();
    })
    .Build();

host.Run();
