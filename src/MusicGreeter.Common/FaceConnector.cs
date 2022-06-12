namespace MusicGreeter.Common;
public static class FaceConnector
{
    private static readonly int _callLimitPerSecond = 10;
    private static readonly Queue<DateTime> _timeStampQueue = new(_callLimitPerSecond);
    private static readonly string _personGroupId = "kompentesfredag";
    private static readonly string _groupName = "faces";
    private static readonly string _faceUrl = "https://westeurope.api.cognitive.microsoft.com";
    public static async Task<bool> AddUserToFaceApiAsync(Employee user, ConfigValues values)
    {
        if (user.Image == null)
        {
            return false;
        }
        var faceClient = new FaceClient(new ApiKeyServiceClientCredentials(values.FaceApiKey), Array.Empty<DelegatingHandler>())
        {
            Endpoint = _faceUrl
        };
        await WaitCallLimitPerSecondAsync();
        using (var stream = new MemoryStream(user.Image))
        {
            try
            {
                var existingList = await faceClient.PersonGroup.GetAsync(_personGroupId);
            }
            catch (APIErrorException)
            {
                try
                {
                    await faceClient.PersonGroup.CreateAsync(_personGroupId, _groupName, "Faces from graph", "recognition_02");
                }
                catch (Exception) { }
            }
            try
            {
                var person = await faceClient.PersonGroupPerson.CreateAsync(_personGroupId, user.Id.ToString(), user.DisplayName);
                await faceClient.PersonGroupPerson.AddFaceFromStreamAsync(_personGroupId, person.PersonId, stream, user.Id.ToString());
            }
            catch (APIErrorException)
            {
                return false;
            }
        }
        return true;
    }

    public static async Task<bool> TrainModelAsync(ConfigValues values)
    {
        try
        {
            var faceClient = new FaceClient(new ApiKeyServiceClientCredentials(values.FaceApiKey), Array.Empty<DelegatingHandler>())
            {
                Endpoint = _faceUrl
            };
            await faceClient.PersonGroup.TrainAsync(_personGroupId);
            return true;
        }
        catch (APIErrorException e)
        {
            Console.WriteLine(e.ToString());
            return false;
        }
    }

    public static async Task<IEnumerable<FacePerson>> AnalyzeImageAsync(byte[] image, ConfigValues values)
    {
        try
        {
            var faceClient = new FaceClient(new ApiKeyServiceClientCredentials(values.FaceApiKey), Array.Empty<DelegatingHandler>())
            {
                Endpoint = _faceUrl
            };
            var idResult = new List<Guid>();
            var retVal = new List<FacePerson>();
            using var stream = new MemoryStream(image);
            var result = await faceClient.Face.DetectWithStreamAsync(stream, true, false, null, "recognition_02", false);
            var foundIds = result.Select(_ => _.FaceId.Value);

            foreach (var chunk in foundIds.Chunk(10))
            {
                try
                {
                    var identified = await faceClient.Face.IdentifyAsync(chunk.ToList(), _personGroupId);
                    idResult.AddRange(identified.Where(i => i.Candidates.Any()).Select(_ => _.Candidates.First().PersonId));
                }
                catch (APIErrorException e)
                {
                    Console.WriteLine(e.Body.Error.Message);
                    Console.WriteLine(e.ToString());
                }
            }
            foreach (var id in idResult)
            {
                var person = await faceClient.PersonGroupPerson.GetAsync(_personGroupId, id);
                if (person != null)
                {
                    retVal.Add(new FacePerson { Name = person.Name, UserData = person.UserData });
                }
            }
            return retVal;
        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
        }
        return new List<FacePerson>();
    }

    private static IEnumerable<IEnumerable<T>> Chunk<T>(this IEnumerable<T> source, int chunksize)
    {
        while (source.Any())
        {
            yield return source.Take(chunksize);
            source = source.Skip(chunksize);
        }
    }

    private static async Task WaitCallLimitPerSecondAsync()
    {
        Monitor.Enter(_timeStampQueue);
        try
        {
            if (_timeStampQueue.Count >= _callLimitPerSecond)
            {
                var timeInterval = DateTime.UtcNow - _timeStampQueue.Peek();
                if (timeInterval < TimeSpan.FromSeconds(1))
                {
                    await Task.Delay(TimeSpan.FromSeconds(1) - timeInterval);
                }
                _timeStampQueue.Dequeue();
            }
            _timeStampQueue.Enqueue(DateTime.UtcNow);
        }
        finally
        {
            Monitor.Exit(_timeStampQueue);
        }
    }
}
