using Microsoft.DataTransfer.Extensibility.Basics.Sink;

namespace Microsoft.DataTransfer.DocumentDb.Sink.EnhancedBulk
{
    /// <summary>
    /// Provides data sink adapters capable of writing data to DocumentDB account in parallel.
    /// </summary>
    public sealed class DocumentDbEnhancedBulkSinkAdapterFactory : DataSinkAdapterFactoryWrapper<IDocumentDbEnhancedBulkSinkAdapterConfiguration>
    {
        /// <summary>
        /// Creates a new instance of <see cref="DocumentDbEnhancedBulkSinkAdapterFactory" />.
        /// </summary>
        public DocumentDbEnhancedBulkSinkAdapterFactory()
            : base(new DocumentDbEnhancedBulkSinkAdapterInternalFactory()) { }
    }
}
