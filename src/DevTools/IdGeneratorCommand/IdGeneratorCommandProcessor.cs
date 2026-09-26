namespace DevTools.IdGeneratorCommand;

using Models;
using Services;

public class IdGeneratorCommandProcessor(IIdGeneratorService idGeneratorService)
{
    public Task<bool> ExecuteUuidV4(bool withoutDashes, CancellationToken cancellationToken) =>
        ExecuteAsync(IdType.UuidV4, withoutDashes, cancellationToken);

    public Task<bool> ExecuteUuidV7(bool withoutDashes, CancellationToken cancellationToken) =>
        ExecuteAsync(IdType.UuidV7, withoutDashes, cancellationToken);

    public Task<bool> ExecuteSnowflake(CancellationToken cancellationToken) =>
        ExecuteAsync(IdType.Snowflake, withoutDashes: false, cancellationToken);

    private async Task<bool> ExecuteAsync(IdType idType, bool withoutDashes, CancellationToken cancellationToken)
    {
        var request = new IdRequest
        {
            Type = idType,
            WithoutDashes = withoutDashes
        };

        var result = await idGeneratorService.GenerateAsync(request, cancellationToken);

        if (result.Success)
        {
            Console.WriteLine(result.Id);
            return true;
        }

        Console.WriteLine($"Error generating id: {result.ErrorMessage}");
        return false;
    }
}
