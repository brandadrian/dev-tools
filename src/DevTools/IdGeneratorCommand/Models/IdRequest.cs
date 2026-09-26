namespace DevTools.IdGeneratorCommand.Models;

public class IdRequest
{
    public IdType Type { get; set; }

    public bool WithoutDashes { get; set; }
}
