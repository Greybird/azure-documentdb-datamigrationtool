using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Castle.Core;
using Microsoft.Azure.CosmosDB.BulkExecutor;
using Microsoft.Azure.CosmosDB.BulkExecutor.BulkImport;
using Microsoft.DataTransfer.DocumentDb.Client;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;

namespace Microsoft.DataTransfer.DocumentDb.UnitTests.Client
{
    [TestClass]
    public class DocumentDbBulkWriterTest
    {
        [TestMethod]
        public async Task TechnicalException_Reported()
        {
            var bulkExecutorMock = new Mock<IBulkExecutor>();
            bulkExecutorMock
                .Setup(be => be.BulkImportAsync(
                    It.IsAny<IEnumerable<object>>(),
                    It.IsAny<bool>(),
                    It.IsAny<bool>(),
                    It.IsAny<int?>(),
                    It.IsAny<int?>(),
                    It.IsAny<CancellationToken>()))
                .Throws<Exception>();
            var bulkWriter = new DocumentDbBulkWriter(bulkExecutorMock.Object);

            var response = await bulkWriter.Import(new List<object>(), false, false, CancellationToken.None);

            Assert.IsNotNull(response.Error);
        }

        [TestMethod]
        public async Task IncorrectDocuments_Reported()
        {
            var badObjects = new List<object> { new object(), new object(), new object() };

            var bulkImportResponse = new BulkImportResponse();
            PrivateObject po = new PrivateObject(bulkImportResponse);
            po.SetProperty(nameof(bulkImportResponse.BadInputDocuments), badObjects);

            bulkImportResponse.BadInputDocuments.AddRange(badObjects);

            var bulkExecutorMock = new Mock<IBulkExecutor>();
            bulkExecutorMock
                .Setup(be => be.BulkImportAsync(
                    It.IsAny<IEnumerable<object>>(),
                    It.IsAny<bool>(),
                    It.IsAny<bool>(),
                    It.IsAny<int?>(),
                    It.IsAny<int?>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(bulkImportResponse);
            var bulkWriter = new DocumentDbBulkWriter(bulkExecutorMock.Object);

            var response = await bulkWriter.Import(new List<object>(), false, false, CancellationToken.None);

            Assert.IsNotNull(response.BadDocuments);
            CollectionAssert.AreEquivalent(badObjects, response.BadDocuments);
        }

        [TestMethod]
    public async Task NullIncorrectDocumentsCollection_ReportedAsEmptyCollection()
    {
        
        var bulkExecutorMock = new Mock<IBulkExecutor>();
        bulkExecutorMock
            .Setup(be => be.BulkImportAsync(
                It.IsAny<IEnumerable<object>>(),
                It.IsAny<bool>(),
                It.IsAny<bool>(),
                It.IsAny<int?>(),
                It.IsAny<int?>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new BulkImportResponse());
        var bulkWriter = new DocumentDbBulkWriter(bulkExecutorMock.Object);

        var response = await bulkWriter.Import(new List<object>(), false, false, CancellationToken.None);

        Assert.IsNotNull(response.BadDocuments);
        CollectionAssert.AreEquivalent(new object[0], response.BadDocuments);
        }
    }
}
