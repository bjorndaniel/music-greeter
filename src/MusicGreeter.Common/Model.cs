namespace MusicGreeter.Common;
public class Employee
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public byte[] Image { get; set; } = Array.Empty<byte>();
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string SpotifyUrl { get; set; } = string.Empty;
    public string DisplayName => $"{FirstName} {LastName}";
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
