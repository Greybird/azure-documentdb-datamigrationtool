using Microsoft.DataTransfer.Basics;
using Microsoft.DataTransfer.DocumentDb.Transformation;
using Microsoft.DataTransfer.Extensibility;
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.DataTransfer.DocumentDb.Client;

namespace Microsoft.DataTransfer.DocumentDb.Sink.EnhancedBulk
{
    sealed class DocumentDbEnhancedBulkSinkAdapterInternalFactory : DocumentDbSinkAdapterFactoryBase<IDocumentDbEnhancedBulkSinkAdapterConfiguration>
    {
        public override string Description
        {
            get { return Resources.EnhancedBulkSinkDescription; }
        }

        protected override async Task<IDataSinkAdapter> CreateAsync(IDataTransferContext context, IDataItemTransformation transformation,
            IDocumentDbEnhancedBulkSinkAdapterConfiguration configuration, CancellationToken cancellation)
        {
            if (String.IsNullOrEmpty(configuration.Collection))
                throw Errors.CollectionNameMissing();

            var instanceConfiguration = GetInstanceConfiguration(configuration);

            var sink = new DocumentDbEnhancedBulkSinkAdapter(
                CreateClient(configuration, context, false, null),
                transformation,
                instanceConfiguration);

            await sink.InitializeAsync(cancellation);

            return sink;
        }

        private static DocumentDbEnhancedBulkSinkAdapterInstanceConfiguration GetInstanceConfiguration(
            IDocumentDbEnhancedBulkSinkAdapterConfiguration configuration)
        {
            Guard.NotNull("configuration", configuration);

            return new DocumentDbEnhancedBulkSinkAdapterInstanceConfiguration
            {
                Collection = configuration.Collection,
                PartitionKey = configuration.PartitionKey,
                CollectionThroughput = GetValueOrDefault(configuration.CollectionThroughput,
                    Defaults.Current.SinkCollectionThroughput, Errors.InvalidCollectionThroughput),
                IndexingPolicy = GetIndexingPolicy(configuration),
                DisableIdGeneration = configuration.DisableIdGeneration,
                UpdateExisting = configuration.UpdateExisting,
                BatchSize = GetValueOrDefault(configuration.BatchSize,
                    Defaults.Current.EnhancedBulkSinkBatchSize, Errors.InvalidBatchSize)
            };
        }
    }
}
