using System;
using System.Collections.Generic;
using Microsoft.Extensions.CommandLineUtils;
using Microsoft.Extensions.Logging;
using Rosdex.Storage.Elasticsearch;

namespace Rosdex.Storage
{
    public class StorageOptions
    {
        private ElasticsearchOptions _elasticsearchOptions;

        public StorageOptions(ElasticsearchOptions elasticSearchOptions)
        {
            _elasticsearchOptions = elasticSearchOptions ?? throw new ArgumentNullException(nameof(elasticSearchOptions));
        }

        public static StorageOptions Register(CommandLineApplication app)
        {
            var elasticSearchOptions = ElasticsearchOptions.Register(app);

            return new StorageOptions(elasticSearchOptions);
        }

        public IndexStorage CreateIndexStorage(ILoggerFactory loggerFactory)
        {
            var storages = new List<IndexStorage>();

            if(_elasticsearchOptions.TryCreateStorage(loggerFactory, out var elasticsearchStorage))
            {
                storages.Add(elasticsearchStorage);
            }

            if(storages.Count == 0)
            {
                return null;
            }
            else if(storages.Count == 1)
            {
                return storages[0];
            }
            else
            {
                throw new CommandLineException("Multiple storage options cannot be provided at this time.");
            }
        }
    }
}
