namespace DevTools.Base64Command.Models;

public class Base64Result
{
    public string TransformedText { get; set; } = string.Empty;
    
    public Base64OperationType OperationType { get; set; }
    
    public bool Success { get; set; }
    
    public string? ErrorMessage { get; set; }
}
