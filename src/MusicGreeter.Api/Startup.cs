[assembly: FunctionsStartup(typeof(MusicGreeter.Api.Startup))]
namespace MusicGreeter.Api;
public class Startup : FunctionsStartup
{
    public override void Configure(IFunctionsHostBuilder builder)
    {
        builder.Services.AddOptions<ConfigValues>().Configure<IConfiguration>((cv, c) =>
        {
            cv.StorageConnectionString = Environment.GetEnvironmentVariable("ConfigValues:StorageConnectionString");
            cv.ContainerName = Environment.GetEnvironmentVariable("ConfigValues:ContainerName");
            cv.FaceApiKey = Environment.GetEnvironmentVariable("ConfigValues:FaceApiKey");
            cv.StorageAccountUrl = Environment.GetEnvironmentVariable("ConfigValues:StorageAccountUrl");
        });
    }
}
