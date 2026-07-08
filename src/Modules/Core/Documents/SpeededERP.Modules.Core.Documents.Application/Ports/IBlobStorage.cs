namespace SpeededERP.Modules.Core.Documents.Application.Ports;

/// <summary>
/// Outbound port for the physical bytes behind a <c>Document</c> aggregate (architecture
/// section 21) — S3/Azure Blob Storage adapters in SaaS deployments,
/// <c>LocalFileSystemBlobStorage</c> (local filesystem) by default in On-Premise.
/// </summary>
public interface IBlobStorage
{
    Task<string> SaveAsync(string fileName, Stream content, CancellationToken cancellationToken = default);

    Task<Stream> OpenReadAsync(string storageKey, CancellationToken cancellationToken = default);
}
