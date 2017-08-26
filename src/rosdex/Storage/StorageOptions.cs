using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.CommandLineUtils;
using Rosdex.Storage.Elasticsearch;

namespace Rosdex.Storage
{
    public class StorageOptions
    {
        private ElasticsearchOptions _elasticSearchOptions;

        public StorageOptions(ElasticsearchOptions elasticSearchOptions)
        {
            _elasticSearchOptions = elasticSearchOptions;
        }

        public static StorageOptions Register(CommandLineApplication app)
        {
            var elasticSearchOptions = ElasticsearchOptions.Register(app);

            return new StorageOptions(elasticSearchOptions);
        }
    }
}
