using DevTools.IdGeneratorCommand.Models;

namespace DevTools.IdGeneratorCommand.Services;

public class IdGeneratorService(SnowflakeIdGenerator snowflakeIdGenerator) : IIdGeneratorService
{
    private const string FormatWithoutDashes = "N";
    private const string FormatWithDashes = "D";

    public ValueTask<IdResult> GenerateAsync(IdRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var id = request.Type switch
            {
                IdType.UuidV4 => FormatGuid(Guid.NewGuid(), request.WithoutDashes),
                IdType.UuidV7 => FormatGuid(Guid.CreateVersion7(), request.WithoutDashes),
                IdType.Snowflake => snowflakeIdGenerator.NextId().ToString(),
                _ => throw new InvalidOperationException($"Unknown id type: {request.Type}")
            };

            return new ValueTask<IdResult>(new IdResult
            {
                Id = id,
                Success = true
            });
        }
        catch (Exception ex)
        {
            return new ValueTask<IdResult>(new IdResult
            {
                Id = string.Empty,
                Success = false,
                ErrorMessage = ex.Message
            });
        }
    }
    
    private static string FormatGuid(Guid guid, bool withoutDashes) =>
        guid.ToString(withoutDashes ? FormatWithoutDashes : FormatWithDashes);
}
