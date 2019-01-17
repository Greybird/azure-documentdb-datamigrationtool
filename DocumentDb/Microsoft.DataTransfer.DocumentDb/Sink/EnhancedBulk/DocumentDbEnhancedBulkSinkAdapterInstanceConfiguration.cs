
namespace Microsoft.DataTransfer.DocumentDb.Sink.EnhancedBulk
{
    sealed class DocumentDbEnhancedBulkSinkAdapterInstanceConfiguration : DocumentDbSinkAdapterInstanceConfiguration, IDocumentDbEnhancedBulkSinkAdapterInstanceConfiguration
    {
        public string Collection { get; set; }

        public int BatchSize { get; set; }
    }
}
