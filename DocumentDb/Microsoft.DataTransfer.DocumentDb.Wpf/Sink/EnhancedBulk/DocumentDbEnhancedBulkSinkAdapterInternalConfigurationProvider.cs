using Microsoft.DataTransfer.Basics;
using Microsoft.DataTransfer.WpfHost.Extensibility;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Controls;

namespace Microsoft.DataTransfer.DocumentDb.Wpf.Sink.EnhancedBulk
{
    sealed class DocumentDbEnhancedBulkSinkAdapterInternalConfigurationProvider : DocumentDbSinkAdapterConfigurationProvider<DocumentDbEnhancedBulkSinkAdapterConfiguration>
    {
        public DocumentDbEnhancedBulkSinkAdapterInternalConfigurationProvider(IImportSharedStorage sharedStorage)
            : base(sharedStorage) { }

        protected override UserControl CreatePresenter(DocumentDbEnhancedBulkSinkAdapterConfiguration configuration)
        {
            return new DocumentDbEnhancedBulkSinkAdapterConfigurationPage { DataContext = configuration };
        }

        protected override UserControl CreateSummaryPresenter(DocumentDbEnhancedBulkSinkAdapterConfiguration configuration)
        {
            return new DocumentDbEnhancedBulkSinkAdapterConfigurationSummaryPage { DataContext = configuration };
        }

        protected override DocumentDbEnhancedBulkSinkAdapterConfiguration CreateValidatableConfiguration()
        {
            return new DocumentDbEnhancedBulkSinkAdapterConfiguration(GetSharedConfiguration());
        }

        protected override void PopulateCommandLineArguments(DocumentDbEnhancedBulkSinkAdapterConfiguration configuration, IDictionary<string, string> arguments)
        {
            base.PopulateCommandLineArguments(configuration, arguments);

            Guard.NotNull("configuration", configuration);
            Guard.NotNull("arguments", arguments);

            arguments.Add(DocumentDbEnhancedBulkSinkAdapterConfiguration.CollectionPropertyName, configuration.Collection);

            if (!String.IsNullOrEmpty(configuration.PartitionKey))
                arguments.Add(DocumentDbEnhancedBulkSinkAdapterConfiguration.PartitionKeyPropertyName, configuration.PartitionKey);

            if (configuration.BatchSize.HasValue && configuration.BatchSize.Value != Defaults.Current.BulkSinkBatchSize)
                arguments.Add(
                    DocumentDbEnhancedBulkSinkAdapterConfiguration.BatchSizePropertyName,
                    configuration.BatchSize.Value.ToString(CultureInfo.InvariantCulture));
        }
    }
}
