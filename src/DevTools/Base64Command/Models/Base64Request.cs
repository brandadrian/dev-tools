namespace DevTools.Base64Command.Models;

public class Base64Request
{
    public string Text { get; set; } = string.Empty;

    public Base64OperationType OperationType { get; set; }
}