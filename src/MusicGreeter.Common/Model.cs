namespace MusicGreeter.Common;
public class Employee
{
    public Guid Id { get; set; } = Guid.NewGuid();
    [JsonPropertyName("image")]
    public byte[] Image { get; set; } = Array.Empty<byte>();
    [JsonPropertyName("firstname")]
    public string FirstName { get; set; } = string.Empty;
    [JsonPropertyName("lastname")]
    public string LastName { get; set; } = string.Empty;
    [JsonPropertyName("spotifyurl")]
    public string SpotifyUrl { get; set; } = string.Empty;
    public string DisplayName => $"{FirstName} {LastName}";
}

public class AnalyzeMessage
{
    public AnalyzeResult? Result { get; set; }
}

public class AnalyzeResult
{
    public int NrFound { get; set; }
    public List<string> Users { get; set; } = new();
}

public class SpotifyResult
{
    public string Artist { get; set; } = string.Empty;
    public string Album { get; set; } = string.Empty;
    public string Track { get; set; } = string.Empty;
    public string Uri { get; set; } = string.Empty;
}
