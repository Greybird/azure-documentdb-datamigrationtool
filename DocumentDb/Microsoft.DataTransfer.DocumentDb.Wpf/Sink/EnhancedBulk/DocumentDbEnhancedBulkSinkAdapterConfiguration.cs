using Microsoft.DataTransfer.Basics.Extensions;
using Microsoft.DataTransfer.DocumentDb.Sink.EnhancedBulk;

namespace Microsoft.DataTransfer.DocumentDb.Wpf.Sink.EnhancedBulk
{
    sealed class DocumentDbEnhancedBulkSinkAdapterConfiguration : DocumentDbSinkAdapterConfiguration, IDocumentDbEnhancedBulkSinkAdapterConfiguration
    {
        public static readonly string CollectionPropertyName =
            ObjectExtensions.MemberName<IDocumentDbEnhancedBulkSinkAdapterConfiguration>(c => c.Collection);

        public static readonly string PartitionKeyPropertyName =
            ObjectExtensions.MemberName<IDocumentDbEnhancedBulkSinkAdapterConfiguration>(c => c.PartitionKey);

        public static readonly string BatchSizePropertyName =
            ObjectExtensions.MemberName<IDocumentDbEnhancedBulkSinkAdapterConfiguration>(c => c.BatchSize);
        

        private string collection;
        private string partitionKey;
        private int? batchSize;

        public string Collection
        {
            get { return collection; }
            set { SetProperty(ref collection, value, ValidateNonEmptyString); }
        }

        public string PartitionKey
        {
            get { return partitionKey; }
            set { SetProperty(ref partitionKey, value); }
        }

        public int? BatchSize
        {
            get { return batchSize; }
            set { SetProperty(ref batchSize, value, ValidatePositiveInteger); }
        }


        public DocumentDbEnhancedBulkSinkAdapterConfiguration(ISharedDocumentDbSinkAdapterConfiguration sharedConfiguration)
            : base(sharedConfiguration)
        {
            BatchSize = Defaults.Current.EnhancedBulkSinkBatchSize;
        }
    }
}
