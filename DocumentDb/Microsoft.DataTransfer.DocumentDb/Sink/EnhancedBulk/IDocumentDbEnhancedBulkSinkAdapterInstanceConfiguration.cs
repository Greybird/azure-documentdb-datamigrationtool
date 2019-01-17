
namespace Microsoft.DataTransfer.DocumentDb.Sink.EnhancedBulk
{
    interface IDocumentDbEnhancedBulkSinkAdapterInstanceConfiguration : IDocumentDbSinkAdapterInstanceConfiguration
    {
        string Collection { get; }
        int BatchSize { get; }
    }
}
