using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.DataTransfer.DocumentDb.Client
{
    interface IDocumentDbBulkWriter
    {
        Task<DocumentDbBulkImportResult> Import(IEnumerable<object> documents, bool updateExisting, bool disableAutomaticIdGeneration, CancellationToken cancellation);
    }
}
