using Microsoft.Extensions.Configuration;
using SpeededERP.Modules.Core.Documents.Application.Ports;

namespace SpeededERP.Modules.Core.Documents.Infrastructure.Storage;

/// <summary>
/// Default <see cref="IBlobStorage"/> adapter — plain local filesystem, the On-Premise
/// default per architecture section 21 ("système de fichiers local en On-Premise"). SaaS
/// deployments rebind <see cref="IBlobStorage"/> to an S3/Azure Blob adapter instead; the
/// root directory is configurable via <c>Documents:LocalStoragePath</c>.
/// </summary>
public sealed class LocalFileSystemBlobStorage : IBlobStorage
{
    private readonly string _rootPath;

    public LocalFileSystemBlobStorage(IConfiguration configuration)
    {
        _rootPath = configuration["Documents:LocalStoragePath"] ?? "./data/documents";
        Directory.CreateDirectory(_rootPath);
    }

    public async Task<string> SaveAsync(string fileName, Stream content, CancellationToken cancellationToken = default)
    {
        var storageKey = $"{Guid.NewGuid():N}-{fileName}";
        var fullPath = Path.Combine(_rootPath, storageKey);

        await using var fileStream = File.Create(fullPath);
        await content.CopyToAsync(fileStream, cancellationToken).ConfigureAwait(false);

        return storageKey;
    }

    public Task<Stream> OpenReadAsync(string storageKey, CancellationToken cancellationToken = default)
    {
        var fullPath = Path.Combine(_rootPath, storageKey);
        Stream stream = File.OpenRead(fullPath);
        return Task.FromResult(stream);
    }
}
