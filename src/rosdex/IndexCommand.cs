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

namespace Rosdex
{
    internal class IndexCommand
    {
        public static readonly string Name = "index";

        private readonly IReadOnlyList<string> _projects;
        private readonly ILoggerFactory _loggerFactory;
        private readonly ILogger _logger;

        public IndexCommand(List<string> projects, ILoggerFactory loggerFactory, ILogger logger)
        {
            _logger = logger;
            _projects = projects;
            _loggerFactory = loggerFactory;
        }

        internal static void Register(CommandLineApplication app, CancellationToken shutdownToken)
        {
            app.Command(Name, cmd =>
            {
                cmd.Description = "Index source code";

                // Set up logging options, because ALWAYS
                var loggingOptions = LoggingOptions.Register(cmd);

                var projectsArgument = cmd.Argument("<PROJECTS...>", "Specify paths to projects to index, OR a single Solution File to index all projects in the solution.", multipleValues: true);

                cmd.OnExecute(() =>
                {
                    var loggerFactory = loggingOptions.CreateLoggerFactory();
                    var logger = loggerFactory.CreateLogger<IndexCommand>();

                    if (projectsArgument.Values.Count == 0)
                    {
                        logger.LogError("At least one project must be specified");
                        return Task.FromResult(1);
                    }

                    return new IndexCommand(
                        projects: projectsArgument.Values,
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

            _logger.LogDebug("Prepared workspace");

            // Create an indexer
            var indexer = new Indexer(_loggerFactory);

            // Build a snapshot index
            var snapshot = await indexer.BuildIndexAsync(workspace, cancellationToken);

            // Temp: Dump symbols
            _logger.LogInformation("Defined Symbols:");
            foreach (var symbol in snapshot.Symbols)
            {
                _logger.LogInformation(" {Type} {Name} : {Location}", symbol.Type, symbol.Name, symbol.Location);
            }

            // Save the snapshot index according to the storage options.

            return 0;
        }

        private async Task<bool> LoadProjectsAsync(MSBuildWorkspace workspace, CancellationToken cancellationToken)
        {
            if (_projects.Count == 1 && _projects[0].EndsWith(".sln"))
            {
                _logger.LogInformation("Loading solution: {SolutionPath}", _projects[0]);
                await workspace.OpenSolutionAsync(_projects[0], cancellationToken);
                cancellationToken.ThrowIfCancellationRequested();
                _logger.LogDebug("Loaded solution: {SolutionPath}", _projects[0]);
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
                    _logger.LogInformation("Loading project: {ProjectPath}", project);
                    await workspace.OpenProjectAsync(project, cancellationToken);
                    cancellationToken.ThrowIfCancellationRequested();
                    _logger.LogDebug("Loaded project: {ProjectPath}", project);
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
