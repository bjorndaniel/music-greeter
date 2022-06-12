namespace MusicGreeter.Common;
public static class SpotifyConnector
{
    public static async Task<IEnumerable<SpotifyResult>> SearchSong(string query, ConfigValues configValues)
    {
        var config = SpotifyClientConfig.CreateDefault();
        var request = new ClientCredentialsRequest(configValues.SpotifyClientId, configValues.SpotifyClientSecret);
        var response = await new OAuthClient(config).RequestToken(request);

        var spotify = new SpotifyClient(config.WithToken(response.AccessToken));
        var searchRequest = new SearchRequest(SearchRequest.Types.Track, query);

        var result = await spotify.Search.Item(searchRequest);
        return result?.Tracks?.Items?.Take(5).Select(i => new SpotifyResult
        {
            Artist = i.Artists.First().Name,
            Album = i.Album.Name,
            Track = i.Name,
            Uri = i.ExternalUrls.FirstOrDefault().Value ?? string.Empty
        }) ?? new List<SpotifyResult>();
    }
}
