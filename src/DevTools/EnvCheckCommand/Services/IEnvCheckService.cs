namespace DevTools.EnvCheckCommand.Services;

using Models;

public interface IEnvCheckService
{
    Task<IEnumerable<ToolInfo>> CheckEnvironmentAsync(CancellationToken cancellationToken = default);
}
