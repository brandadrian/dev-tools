namespace DevTools.EnvCheckCommand.Services;

using System.Diagnostics;
using Models;

public class EnvCheckService : IEnvCheckService
{
    private readonly Dictionary<string, (string command, string versionArg)> _tools = new()
    {
        // Version Control
        { "Git", ("git", "--version") },

        // Core Development
        { ".NET", ("dotnet", "--version") },
        { "Java", ("java", "-version") },
        { "Python", ("python", "--version") },
        { "Node.js", ("node", "--version") },
        { "npm", ("npm", "--version") },

        // Frontend Development
        { "TypeScript", ("tsc", "--version") },
        { "Yarn", ("yarn", "--version") },
        { "React CLI", ("npx", "-p create-react-app --version") },
        { "Angular CLI", ("ng", "--version") },
        { "Vue CLI", ("vue", "--version") },

        // Backend Development
        { "Go", ("go", "version") },
        { "Rust", ("rustc", "--version") },
        { "Ruby", ("ruby", "--version") },
        { "PHP", ("php", "--version") },
        { "Gradle", ("gradle", "--version") },
        { "Maven", ("mvn", "--version") },

        // DevOps & Infrastructure
        { "Docker", ("docker", "--version") },
        { "Podman", ("podman", "--version") },
        { "Docker Compose", ("docker-compose", "--version") },

        // Kubernetes & Orchestration
        { "Kubernetes (kubectl)", ("kubectl", "version --client") },
        { "Kubectx", ("kubectx", "--version") },
        { "Helm", ("helm", "version --short") },
        { "Kustomize", ("kustomize", "version") },
        { "Skaffold", ("skaffold", "version") },
        { "Kind", ("kind", "--version") },
        { "Minikube", ("minikube", "version") },

        // Cloud & Infrastructure
        { "Terraform", ("terraform", "--version") },
        { "AWS CLI", ("aws", "--version") },
        { "Azure CLI", ("az", "--version") },
        { "GCloud CLI", ("gcloud", "--version") },

        // Database
        { "PostgreSQL", ("psql", "--version") },
        { "MySQL", ("mysql", "--version") },
        { "MongoDB", ("mongo", "--version") },
        { "Redis", ("redis-cli", "--version") },
        { "SQLite", ("sqlite3", "--version") },
    };

    public async Task<IEnumerable<ToolInfo>> CheckEnvironmentAsync(CancellationToken cancellationToken = default)
    {
        var results = new List<ToolInfo>();

        foreach (var (name, (command, versionArg)) in _tools)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var toolInfo = await CheckToolAsync(name, command, versionArg, cancellationToken);
            results.Add(toolInfo);
        }

        return results.OrderByDescending(t => t.IsInstalled).ThenBy(t => t.Name);
    }

    private async Task<ToolInfo> CheckToolAsync(string name, string command, string versionArg, CancellationToken cancellationToken)
    {
        try
        {
            var psi = new ProcessStartInfo
            {
                FileName = command,
                Arguments = versionArg,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            };

            using var process = Process.Start(psi);

            if (process is null)
            {
                return new ToolInfo(name, command, null, false, "Failed to start process");
            }

            var timeout = Task.Delay(5000, cancellationToken);
            var completed = process.WaitForExitAsync(cancellationToken);

            var task = await Task.WhenAny(completed, timeout);
            if (task == timeout)
            {
                process.Kill();
                return new ToolInfo(name, command, null, false, "Timeout checking version");
            }

            if (process.ExitCode != 0)
            {
                return new ToolInfo(name, command, null, false, $"Exit code: {process.ExitCode}");
            }

            var output = (await process.StandardOutput.ReadToEndAsync(cancellationToken)).Trim();
            var error = (await process.StandardError.ReadToEndAsync(cancellationToken)).Trim();

            var version = string.IsNullOrEmpty(output) ? error : output;

            return new ToolInfo(name, command, version, true);
        }
        catch (Exception ex)
        {
            return new ToolInfo(name, command, null, false, ex.Message);
        }
    }
}
