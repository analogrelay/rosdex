using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.CommandLineUtils;

namespace Rosdex
{
    public class Program
    {
        public static readonly Assembly Assembly = typeof(Program).Assembly;

        public static readonly string Name = Assembly.GetName().Name;
        public static readonly string Version = Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion;

        private static int Main(string[] args)
        {
#if DEBUG
            if (args.Any(a => a.Equals("--debug", StringComparison.OrdinalIgnoreCase)))
            {
                args = args.Where(a => !a.Equals("--debug", StringComparison.OrdinalIgnoreCase)).ToArray();
                Console.WriteLine($"Waiting for debugger to attach. Process ID: {System.Diagnostics.Process.GetCurrentProcess().Id}");
                Console.WriteLine("Press ENTER to continue...");
                Console.ReadLine();
            }
#endif

            // Create cancellation token for Ctrl-C
            var cts = new CancellationTokenSource();
            Console.CancelKeyPress += (s, a) =>
            {
                if(cts.Token.IsCancellationRequested)
                {
                    // Already tried cancelling once, abort now
                    return;
                }
                a.Cancel = true;
                Console.WriteLine("Cancelling, press Ctrl-C again to abort...");
                cts.Cancel();
            };

            var app = new CommandLineApplication();
            app.Name = Name;
            app.FullName = "C# Source Code Indexer";
            app.Description = "Tool to index C# Source Code and write the results to various data stores";
            app.HelpOption("-h|-?|--help");
            app.VersionOption("-v|--version", Version);

            IndexCommand.Register(app, cts.Token);

            app.Command("help", cmd =>
            {
                cmd.Description = "Get help on a specific command, or display this help message";
                var commandArgument = cmd.Argument("<COMMAND>", "The command to get help for (optional)");

                cmd.OnExecute(() =>
                {
                    app.ShowHelp(commandArgument.Value);
                    return 0;
                });
            });

            app.OnExecute(() =>
            {
                app.ShowHelp();
                return 0;
            });

            try
            {
                return app.Execute(args);
            }
            catch(Exception ex)
            {
                return HandleException(ex) ?? 1;
            }
            // Any other exception is unexpected and should crash the process.
        }

        private static int? HandleException(Exception exception)
        {
            switch(exception)
            {
                case TaskCanceledException _:
                case OperationCanceledException _:
                    Console.Error.WriteLine("User cancelled operation");
                    return 2;
                case CommandLineException clex:
                    Console.Error.WriteLine($"error: {clex.Message}");
                    return clex.ExitCode;
                case AggregateException aggex:
                    int? exitCode = null;
                    foreach(var ex in aggex.InnerExceptions)
                    {
                        var code = HandleException(ex);
                        if(code != null)
                        {
                            exitCode = code;
                        }
                    }
                    return exitCode;
                case ReflectionTypeLoadException rtlex:
                    Console.Error.WriteLine(rtlex.ToString());
                    foreach(var ex in rtlex.LoaderExceptions)
                    {
                        HandleException(ex);
                    }
                    return 1;
                default:
                    Console.Error.WriteLine(exception.ToString());
                    return 1;
            }
        }
    }
}
