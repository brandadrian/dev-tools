namespace DevTools.EnvCheckCommand.Models;

public record ToolInfo(
    string Name,
    string Command,
    string? Version,
    bool IsInstalled,
    string? Error = null
);
