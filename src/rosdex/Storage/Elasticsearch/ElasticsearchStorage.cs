using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Nest;
using Rosdex.Model;
using Snapshot = Rosdex.Model.Snapshot;

namespace Rosdex.Storage.Elasticsearch
{
    public class ElasticsearchStorage
    {
        private readonly ElasticClient _client;
        private readonly string _indexPrefix;
        private readonly ILogger<ElasticsearchStorage> _logger;

        public ElasticsearchStorage(Uri uri, string userName, string password, string indexPrefix, ILogger<ElasticsearchStorage> logger)
        {
            var config = new ConnectionSettings(uri)
                .BasicAuthentication(userName, password);
            BuildMappings(config);
            _client = new ElasticClient(config);
            _indexPrefix = indexPrefix;
            _logger = logger;
        }

        public async Task<bool> StoreSnapshotAsync(Snapshot snapshot, CancellationToken cancellationToken = default)
        {
            var indexName = _indexPrefix + snapshot.Name;

            // Start building a bulk request
            var request = new BulkRequest(indexName);

            // Store Symbols First
            foreach (var symbol in snapshot.Symbols)
            {
                _logger.LogTrace("Queuing index of Symbol: {SymbolPath}", symbol.Path);
                request.Operations.Add(new BulkIndexOperation<SymbolDefinition>(symbol));
            }

            // Store Projects (and their documents)
            foreach (var project in snapshot.Projects)
            {
                _logger.LogTrace("Queuing index of Project: {Name}", project.Name);
                request.Operations.Add(new BulkIndexOperation<Project>(project));

                foreach(var document in project.Documents)
                {
                    _logger.LogTrace("Queuing index of Document: {Name}", document.Name);
                    request.Operations.Add(new BulkIndexOperation<Document>(document));
                }
            }

            // Store the snapshot metadata itself
            _logger.LogTrace("Queuing index of Snapshot: {Name}", snapshot.Name);
            request.Operations.Add(new BulkIndexOperation<Snapshot>(snapshot));

            // Send the request
            _logger.LogInformation("Writing {DocumentCount} documents to Elasticsearch", request.Operations.Count);
            var response = await _client.BulkAsync(request, cancellationToken);

            if(response.IsValid)
            {
                _logger.LogInformation("Successfully wrote {DocumentCount} documents to Elasticsearch", request.Operations.Count);
                return true;
            }
            else
            {
                foreach(var errorItem in response.ItemsWithErrors)
                {
                    _logger.LogError("Document '{Id}' failed to write: {Error}", errorItem.Id, errorItem.Error);
                }
                return false;
            }
        }
    }
}
