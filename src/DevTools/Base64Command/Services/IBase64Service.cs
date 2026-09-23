using DevTools.Base64Command.Models;

namespace DevTools.Base64Command.Services;

public interface IBase64Service
{
    ValueTask<Base64Result> TransformAsync(Base64Request request, CancellationToken cancellationToken);
}