using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.Extensions.Logging;

namespace Rosdex.Indexing
{
    public class Indexer
    {
        private readonly ILogger<Indexer> _logger;
        private readonly ILoggerFactory _loggerFactory;

        public Indexer(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<Indexer>();
            _loggerFactory = loggerFactory;
        }

        public async Task<Model.Snapshot> BuildIndexAsync(Workspace workspace, CancellationToken cancellationToken = default)
        {
            var builder = new SnapshotBuilder();

            _logger.LogInformation("Generating Snapshot Index...");
            foreach(var project in workspace.CurrentSolution.Projects)
            {
                var projectBuilder = new ProjectBuilder(builder);
                await IndexProjectAsync(project, projectBuilder, cancellationToken);
                projectBuilder.Build();
            }
            var snapshot = builder.Build();
            _logger.LogInformation("Generated Snapshot Index");

            return snapshot;
        }

        private async Task<bool> IndexProjectAsync(Project project, ProjectBuilder builder, CancellationToken cancellationToken)
        {
            using (_logger.BeginScope("[{ProjectName}]", project.Name))
            {
                _logger.LogInformation("Indexing Project...");

                foreach(var document in project.Documents)
                {
                    var documentBuilder = new DocumentBuilder(builder);
                    await IndexDocumentAsync(document, documentBuilder, cancellationToken);
                    documentBuilder.Build();
                }

                _logger.LogInformation("Indexing complete");
                return true;
            }
        }

        private async Task IndexDocumentAsync(Document document, DocumentBuilder builder, CancellationToken cancellationToken)
        {
            using(_logger.BeginScope("[{DocumentName}]", document.Name))
            {
                _logger.LogDebug("Indexing Document...");

                builder.Name = document.Name;
                builder.FilePath = document.FilePath;
                builder.Folders = document.Folders;

                _logger.LogTrace("Loading document text");
                builder.Text = await document.GetTextAsync(cancellationToken);
                _logger.LogTrace("Loaded document text");

                _logger.LogTrace("Getting semantic model");
                var semanticModel = await document.GetSemanticModelAsync(cancellationToken);
                _logger.LogTrace("Got semantic model");

                _logger.LogTrace("Getting syntax root");
                var syntaxRoot = await document.GetSyntaxRootAsync(cancellationToken);
                _logger.LogTrace("Got syntax root");

                // Visit syntax
                _logger.LogTrace("Walking syntax tree");
                var walker = new CSharpIndexingSyntaxWalker(builder, semanticModel, _loggerFactory.CreateLogger<CSharpIndexingSyntaxWalker>());
                walker.Visit(syntaxRoot);

                _logger.LogDebug("Indexing completed");
            }
        }

    }
}
