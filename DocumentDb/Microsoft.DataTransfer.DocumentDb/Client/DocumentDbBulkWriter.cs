using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.CosmosDB.BulkExecutor;

namespace Microsoft.DataTransfer.DocumentDb.Client
{
    sealed class DocumentDbBulkWriter : IDocumentDbBulkWriter
    {
        private readonly IBulkExecutor bulkExecutor;

        public DocumentDbBulkWriter(IBulkExecutor bulkExecutor)
        {
            this.bulkExecutor = bulkExecutor;
        }

        public async Task<DocumentDbBulkImportResult> Import(IEnumerable<object> documents, bool updateExisting, bool disableAutomaticIdGeneration, CancellationToken cancellation)
        {
            var result = new DocumentDbBulkImportResult();
            try
            {
                var response = await bulkExecutor.BulkImportAsync(documents, updateExisting, disableAutomaticIdGeneration, cancellationToken:cancellation);
                result.ReportBadObjects(response.BadInputDocuments);
            }
            catch (Exception e)
            {
                result.ReportError(e);
            }

            return result;
        }
    }
}