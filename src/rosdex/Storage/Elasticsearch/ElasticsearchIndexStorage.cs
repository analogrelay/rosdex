using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Nest;
using Rosdex.Model;
using Snapshot = Rosdex.Model.Snapshot;

namespace Rosdex.Storage.Elasticsearch
{
    public class ElasticsearchIndexStorage : IndexStorage
    {
        public static readonly string DefaultIndexPrefix = "snapshot-";

        private readonly ElasticClient _client;
        private readonly string _indexPrefix;
        private readonly ILogger<ElasticsearchIndexStorage> _logger;

        public ElasticsearchIndexStorage(Uri uri, string userName, string password, string indexPrefix, ILogger<ElasticsearchIndexStorage> logger)
        {
            var config = new ConnectionSettings(uri ?? throw new ArgumentNullException(nameof(uri)));
            if (!string.IsNullOrEmpty(userName))
            {
                config.BasicAuthentication(userName, password);
            }
            _client = new ElasticClient(config);
            _indexPrefix = indexPrefix;
            _logger = logger;
        }

        public override async Task<bool> StoreSnapshotAsync(Snapshot snapshot, CancellationToken cancellationToken = default)
        {
            _ = snapshot ?? throw new ArgumentNullException(nameof(snapshot));

            var indexName = _indexPrefix + snapshot.Name;

            // Start building a bulk request
            var operations = new List<IBulkOperation>();

            // Store Symbols First
            foreach (var symbol in snapshot.SymbolDefinitions)
            {
                _logger.LogTrace("Queuing index of SymbolDefinition: {SymbolPath}.", symbol.Path);
                operations.Add(new BulkIndexOperation<SymbolDefinitionSurrogate>(new SymbolDefinitionSurrogate(symbol)));
            }

            foreach (var symbol in snapshot.SymbolReferences)
            {
                _logger.LogTrace("Queuing index of SymbolReference: {SymbolPath}.", symbol.Path);
                operations.Add(new BulkIndexOperation<SymbolReferenceSurrogate>(new SymbolReferenceSurrogate(symbol)));
            }

            // Store Projects (and their documents)
            foreach (var project in snapshot.Projects)
            {
                _logger.LogTrace("Queuing index of Project: {Name}.", project.Name);
                operations.Add(new BulkIndexOperation<ProjectSurrogate>(new ProjectSurrogate(project)));

                foreach(var document in project.Documents)
                {
                    _logger.LogTrace("Queuing index of Document: {Name}.", document.Name);
                    operations.Add(new BulkIndexOperation<DocumentSurrogate>(new DocumentSurrogate(document)));
                }
            }

            // Store the snapshot metadata itself
            _logger.LogTrace("Queuing index of Snapshot: {Name}.", snapshot.Name);
            operations.Add(new BulkIndexOperation<SnapshotSurrogate>(new SnapshotSurrogate(snapshot)));

            // Send the request
            var request = new BulkRequest(indexName)
            {
                Operations = operations
            };
            _logger.LogInformation("Writing {DocumentCount} documents to Elasticsearch.", request.Operations.Count);
            var response = await _client.BulkAsync(request, cancellationToken);

            if(response.IsValid)
            {
                _logger.LogInformation("Successfully wrote {DocumentCount} documents to Elasticsearch.", request.Operations.Count);
                return true;
            }
            else
            {
                foreach(var errorItem in response.ItemsWithErrors)
                {
                    _logger.LogError("Document '{Id}' failed to write: {Error}.", errorItem.Id, errorItem.Error);
                }
                return false;
            }
        }

        [ElasticsearchType(Name = nameof(Snapshot))]
        private class SnapshotSurrogate
        {
            public string Name { get; set; }

            public SnapshotSurrogate(Snapshot snapshot)
            {
                Name = snapshot.Name;
            }
        }

        [ElasticsearchType(Name = nameof(Document))]
        private class DocumentSurrogate
        {
            public string Name { get; set; }
            public string FilePath { get; set; }
            public IReadOnlyList<string> Folders { get; set; }

            public DocumentSurrogate(Document document)
            {
                Name = document.Name;
                FilePath = document.FilePath;
                Folders = document.Folders;
            }
        }

        [ElasticsearchType(Name = nameof(Project))]
        private class ProjectSurrogate
        {
            public string Name { get; set; }
            public string FilePath { get; set; }
            public string AssemblyName { get; set; }
            public string Language { get; set; }

            public ProjectSurrogate(Project project)
            {
                Name= project.Name;
                FilePath=project.FilePath;
                AssemblyName=project.AssemblyName;
                Language=project.Language;
            }
        }

        private abstract class SymbolBaseSurrogate
        {
            public string Path { get; set; }
            public string Name { get; set; }
            public string FullName { get; set; }
            public SymbolType Type { get; set; }
            public SourceSpan Location { get; set; }

            protected SymbolBaseSurrogate(SymbolBase symbol)
            {
                Path = symbol.Path.ToString();
                Name = symbol.Name;
                FullName = symbol.FullName;
                Type = symbol.Type;
                Location = symbol.Location;
            }
        }

        [ElasticsearchType(Name = nameof(SymbolDefinition))]
        private class SymbolDefinitionSurrogate : SymbolBaseSurrogate
        {
            public SymbolDefinitionSurrogate(SymbolDefinition symbol) : base(symbol) { }
        }

        [ElasticsearchType(Name = nameof(SymbolReference))]
        private class SymbolReferenceSurrogate : SymbolBaseSurrogate
        {
            public SymbolReferenceSurrogate(SymbolReference symbol) : base(symbol) { }
        }
    }
}
