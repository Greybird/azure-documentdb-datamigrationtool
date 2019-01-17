using Microsoft.DataTransfer.WpfHost.Extensibility;
using Microsoft.DataTransfer.WpfHost.Extensibility.Basics;

namespace Microsoft.DataTransfer.DocumentDb.Wpf.Sink.EnhancedBulk
{
    /// <summary>
    /// Provides configuration for Enhanced Bulk DocumentDB data sink.
    /// </summary>
    public sealed class DocumentDbEnhancedBulkSinkAdapterConfigurationProvider : DataAdapterConfigurationProviderWrapper
    {
        /// <summary>
        /// Creates a new instance of <see cref="DocumentDbEnhancedBulkSinkAdapterConfigurationProvider" />.
        /// </summary>
        /// <param name="sharedStorage">Storage to share some configuration values for current import.</param>
        public DocumentDbEnhancedBulkSinkAdapterConfigurationProvider(IImportSharedStorage sharedStorage)
            : base(new DocumentDbEnhancedBulkSinkAdapterInternalConfigurationProvider(sharedStorage)) { }
    }
}
