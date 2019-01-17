using Microsoft.DataTransfer.DocumentDb.Client;
using Microsoft.DataTransfer.DocumentDb.Shared;
using Microsoft.DataTransfer.DocumentDb.Transformation;
using Microsoft.DataTransfer.Extensibility;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.DataTransfer.DocumentDb.Sink.EnhancedBulk
{
    sealed class DocumentDbEnhancedBulkSinkAdapter : DocumentDbAdapterBase<IDocumentDbWriteClient, IDocumentDbEnhancedBulkSinkAdapterInstanceConfiguration>, IDataSinkAdapter
    {
        private BlockingCollection<DocumentDbImportTask> items;

        private string collectionLink;
        private Task bulkExecutorTask;

        public int MaxDegreeOfParallelism => Configuration.BatchSize;

        public DocumentDbEnhancedBulkSinkAdapter(IDocumentDbWriteClient client, IDataItemTransformation transformation, IDocumentDbEnhancedBulkSinkAdapterInstanceConfiguration configuration)
            : base(client, transformation, configuration) { }

        public async Task InitializeAsync(CancellationToken cancellation)
        {
            collectionLink = await Client.GetOrCreateCollectionAsync(
                Configuration.Collection, Configuration.PartitionKey,
                Configuration.CollectionThroughput, Configuration.IndexingPolicy,
                cancellation);
            items = new BlockingCollection<DocumentDbImportTask>(Configuration.BatchSize);
            bulkExecutorTask = Task.Run(async () => await Import(cancellation), cancellation);
        }

        public Task WriteAsync(IDataItem dataItem, CancellationToken cancellation)
        {
            if (string.IsNullOrEmpty(collectionLink))
            {
                throw Errors.SinkIsNotInitialized();
            }

            dataItem = Transformation.Transform(dataItem);

            var dataItemSurrogate = new DataItemSurrogate(dataItem);

            var dataImportTask = new DocumentDbImportTask(dataItemSurrogate);
            items.Add(dataImportTask, cancellation);
            return dataImportTask.CompletionSource.Task;
        }

        public async Task CompleteAsync(CancellationToken cancellation)
        {
            items.CompleteAdding();
            await bulkExecutorTask;
        }
        
        private async Task Import(CancellationToken cancellation)
        {
            foreach (var batch in Split(items, Configuration.BatchSize))
            {
                if (cancellation.IsCancellationRequested)
                {
                    break;
                }

                try
                {
                    var bulkExecutor = await Client.CreateBulkWriter(Configuration.Collection, cancellation);
                    var response = await bulkExecutor.Import(
                        batch.Keys,
                        Configuration.UpdateExisting,
                        Configuration.DisableIdGeneration,
                        cancellation);
                    ReportTasksStatus(batch, response);
                }
                catch (OperationCanceledException)
                {
                    ReportTasksStatus(batch, new DocumentDbBulkImportResult());
                    break;
                }
            }
        }

        private IEnumerable<ConcurrentDictionary<object, DocumentDbImportTask>> Split(BlockingCollection<DocumentDbImportTask> items, int batchSize)
        {
            ConcurrentDictionary<object, DocumentDbImportTask> batch = new ConcurrentDictionary<object, DocumentDbImportTask>();
            int count = 0; // caches the dictionary count to prevent multiple evaluation in the loop
            foreach (var item in items.GetConsumingEnumerable())
            {
                if (!batch.TryAdd(item.Document, item))
                {
                    throw Errors.BufferSlotIsOccupied();
                }

                count++;
                if (count == batchSize)
                {
                    yield return batch;
                    batch = new ConcurrentDictionary<object, DocumentDbImportTask>();
                    count = 0;
                }
            }

            if (batch.Any())
            {
                yield return batch;
            }
        }

        private void ReportTasksStatus(ConcurrentDictionary<object, DocumentDbImportTask> batch, DocumentDbBulkImportResult response)
        {
            if (response.BadDocuments != null)
            {
                foreach (var badDocument in response.BadDocuments)
                {
                    if (!batch.TryRemove(badDocument, out var documentTask))
                    {
                        continue;
                    }

                    documentTask.CompletionSource.SetException(Errors.BulkImportInvalidDocument());
                }
            }

            foreach (var batchObject in batch)
            {
                if (!batch.TryRemove(batchObject.Key, out var documentTask))
                {
                    continue;
                }
                documentTask.CompletionSource.SetResult(null);
            }
        }

        private void ReportTaskFailure(ConcurrentDictionary<object, DocumentDbImportTask> batch, Exception exception)
        {
            if (batch == null)
            {
                return;
            }
            foreach (var index in batch.Keys)
            {
                if (!batch.TryRemove(index, out var documentTask))
                {
                    continue;
                }
                documentTask.CompletionSource.SetException(Errors.UnexpectedAsyncFlushError(exception));
            }
        }

        private sealed class DocumentDbImportTask
        {
            public TaskCompletionSource<object> CompletionSource { get; }
            public object Document { get; }

            public DocumentDbImportTask(object document)
            {
                Document = document;
                CompletionSource = new TaskCompletionSource<object>();
            }
        }
    }
}
