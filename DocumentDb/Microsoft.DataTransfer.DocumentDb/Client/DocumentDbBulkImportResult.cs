using System;
using System.Collections.Generic;

namespace Microsoft.DataTransfer.DocumentDb.Client
{
    sealed class DocumentDbBulkImportResult
    {
        public Exception Error { get; private set; }
        public List<object> BadDocuments { get; } = new List<object>();

        public void ReportBadObjects(List<object> badObjects)
        {
            if (badObjects != null)
            {
                BadDocuments.AddRange(badObjects);
            }
        }

        public void ReportError(Exception exception)
        {
            Error = exception;
        }
    }
}