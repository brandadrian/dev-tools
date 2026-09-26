using DevTools.IdGeneratorCommand.Models;

namespace DevTools.IdGeneratorCommand.Services;

public interface IIdGeneratorService
{
    ValueTask<IdResult> GenerateAsync(IdRequest request, CancellationToken cancellationToken);
}
