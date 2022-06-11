
namespace MusicGreeter.Api;
public class Api
{
    private ConfigValues _configValues;

    public Api(IOptions<ConfigValues> options)
    {
        _configValues = options.Value;
    }

    [FunctionName("negotiate")]
    public SignalRConnectionInfo GetSignalRInfo(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequest req, 
        [SignalRConnectionInfo(HubName = "analyzis")] SignalRConnectionInfo connectionInfo) =>
        connectionInfo;

    [FunctionName("upload")]
    public async Task<IActionResult> Run(
   [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
   ILogger log, ExecutionContext context)
    {
        
        log.LogInformation("C# HTTP trigger function processed a request to save image.");
        
        var fileData = await req.ReadFormAsync();
        var file = req.Form.Files["image"];
        return new OkObjectResult($"{file.FileName} {fileData["spotifyurl"]} {fileData["username"]}");
    }

}