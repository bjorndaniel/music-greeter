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
        [HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequest req, [SignalRConnectionInfo(HubName = "analyzis")] SignalRConnectionInfo connectionInfo) =>
    connectionInfo;

    [FunctionName("upload")]
    public async Task<IActionResult> Upload(
   [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
   ILogger log, ExecutionContext context)
    {

        log.LogInformation("C# HTTP trigger function processed a request to save image.");

        var fileData = await req.ReadFormAsync();
        var firstname = fileData["firstname"];
        var lastname = fileData["lastname"];
        var url = fileData["spotifyurl"];
        var file = req.Form.Files["image"];
        var bytes = new byte[file.Length];
        using var stream = file.OpenReadStream();
        await stream.ReadAsync(bytes);
        var user = new Employee
        {
            FirstName = firstname,
            LastName = lastname,
            SpotifyUrl = url,
            Image = bytes
        };
        await StorageConnector.AddUserToStorageAsync(user, _configValues);
        await FaceConnector.AddUserToFaceApiAsync(user, _configValues);
        await FaceConnector.TrainModelAsync(_configValues);
        return new OkObjectResult(JsonSerializer.Serialize(user));
    }

    [FunctionName("analyze")]
    [return: SignalR(ConnectionStringSetting = "AzureSignalRConnectionString", HubName = "analyzis")]
    public async Task<SignalRMessage> Analyze(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log, ExecutionContext context)
    {

        log.LogInformation("C# HTTP trigger function processed a request to analyze image.");
        var ms = new MemoryStream();
        await req.Body.CopyToAsync(ms);
        var imageBytes = ms.ToArray();
        var identified = await FaceConnector.AnalyzeImageAsync(imageBytes, _configValues);
        identified.ToList().ForEach(_ =>
        {
            log.LogInformation(_.UserData);
        });
        dynamic data = new ExpandoObject();
        var message = new SignalRMessage
        {
            Target = "ImageAnalyzed"
        };
        var content = new AnalyzeResult
        {
            NrFound = identified.Count(),
            Users = identified.Select(_ => _.UserData).ToList()
        };
        data.result = content;
        message.Arguments = new object[] { data };
        return message;
    }

    [FunctionName("searchspotify")]
    public async Task<IEnumerable<SpotifyResult>> Search(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
        ILogger log, ExecutionContext context)
    {
        var query = req.Query["query"];
        return await SpotifyConnector.SearchSong(query, _configValues);
    }

}