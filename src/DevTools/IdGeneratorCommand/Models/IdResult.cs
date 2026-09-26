namespace DevTools.IdGeneratorCommand.Models;

public class IdResult
{
    public string Id { get; set; } = string.Empty;

    public bool Success { get; set; }

    public string? ErrorMessage { get; set; }
}
