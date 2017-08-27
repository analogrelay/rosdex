using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.MSBuild;
using Microsoft.Extensions.CommandLineUtils;
using Microsoft.Extensions.Logging;
using Rosdex.Indexing;
using Rosdex.Storage;

namespace Rosdex
{
    internal class IndexCommand
    {
        public static readonly string Name = "index";
        private readonly string _snapshotName;
        private readonly IReadOnlyList<string> _projects;
        private readonly ILoggerFactory _loggerFactory;
        private readonly ILogger _logger;
        private readonly IndexStorage _storage;

        public IndexCommand(string snapshotName, List<string> projects, ILoggerFactory loggerFactory, ILogger logger, IndexStorage storage)
        {
            _logger = logger;
            _storage = storage;
            _snapshotName = snapshotName;
            _projects = projects;
            _loggerFactory = loggerFactory;
        }

        internal static void Register(CommandLineApplication app, CancellationToken shutdownToken)
        {
            app.Command(Name, cmd =>
            {
                cmd.HelpOption("-h|-?|--help");
                cmd.Description = "Index source code.";

                // Set up logging options, because ALWAYS
                var loggingOptions = LoggingOptions.Register(cmd);

                // Set up storage options
                var storageOptions = StorageOptions.Register(cmd);

                var nameOption = cmd.Option("-n|--name <NAME>", "Specifies the name of the snapshot.", CommandOptionType.SingleValue);
                var projectsArgument = cmd.Argument("<PROJECTS...>", "Specify paths to projects to index, OR a single Solution File to index all projects in the solution.", multipleValues: true);

                cmd.OnExecute(() =>
                {
                    var loggerFactory = loggingOptions.CreateLoggerFactory();
                    var logger = loggerFactory.CreateLogger<IndexCommand>();

                    if (!nameOption.HasValue())
                    {
                        logger.LogError("The '--name' option is required.");
                        return Task.FromResult(1);
                    }

                    if (projectsArgument.Values.Count == 0)
                    {
                        logger.LogError("At least one project must be specified.");
                        return Task.FromResult(1);
                    }

                    var storage = storageOptions.CreateIndexStorage(loggerFactory);
                    // TODO: Don't proceed if no storage configured
                    //if (storage == null)
                    //{
                    //    logger.LogError("At least one storage option must be specified.");
                    //    return Task.FromResult(1);
                    //}

                    return new IndexCommand(
                        snapshotName: nameOption.Value(),
                        projects: projectsArgument.Values,
                        storage: storage,
                        loggerFactory: loggerFactory,
                        logger: logger).ExecuteAsync(shutdownToken);
                });
            });
        }

        private async Task<int> ExecuteAsync(CancellationToken cancellationToken)
        {
            // Create a Workspace
            var workspace = MSBuildWorkspace.Create();

            // Load the solution/projects
            if (!await LoadProjectsAsync(workspace, cancellationToken))
            {
                return 1;
            }

            _logger.LogDebug("Prepared workspace.");

            // Create an indexer
            var indexer = new Indexer(_loggerFactory);

            // Build a snapshot index
            var snapshot = await indexer.BuildIndexAsync(_snapshotName, workspace, cancellationToken);

            // Save the snapshot index according to the storage options.
            if (_storage == null)
            {
                _logger.LogWarning("Skipping storage as no storage options were specified.");
            }
            else
            {
                _logger.LogInformation("Saving index data...");
                await _storage.StoreSnapshotAsync(snapshot, cancellationToken);
                _logger.LogInformation("Saved index data.");
            }

            return 0;
        }

        private async Task<bool> LoadProjectsAsync(MSBuildWorkspace workspace, CancellationToken cancellationToken)
        {
            if (_projects.Count == 1 && _projects[0].EndsWith(".sln"))
            {
                _logger.LogInformation("Loading solution: {SolutionPath}.", _projects[0]);
                await workspace.OpenSolutionAsync(_projects[0], cancellationToken);
                cancellationToken.ThrowIfCancellationRequested();
                _logger.LogDebug("Loaded solution: {SolutionPath}.", _projects[0]);
            }
            else
            {
                foreach (var project in _projects)
                {
                    if (project.EndsWith(".sln"))
                    {
                        _logger.LogError("If a solution is provided, it must be the only project specified.");
                        return false;
                    }
                    _logger.LogInformation("Loading project: {ProjectPath}.", project);
                    await workspace.OpenProjectAsync(project, cancellationToken);
                    cancellationToken.ThrowIfCancellationRequested();
                    _logger.LogDebug("Loaded project: {ProjectPath}.", project);
                }
            }

            cancellationToken.ThrowIfCancellationRequested();

            // Check for errors
            var success = true;
            foreach (var diagnostic in workspace.Diagnostics)
            {
                if (diagnostic.Kind == WorkspaceDiagnosticKind.Failure)
                {
                    success = false;
                    _logger.LogError(diagnostic.Message);
                }
                else
                {
                    _logger.LogWarning(diagnostic.Message);
                }
            }

            return success;
        }
    }
}
