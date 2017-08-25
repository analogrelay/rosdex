using Microsoft.Extensions.CommandLineUtils;
using Microsoft.Extensions.Logging;

namespace Rosdex
{
    internal class LoggingOptions
    {
        private readonly CommandOption _verboseOption;
        private readonly CommandOption _veryVerboseOption;

        public LoggingOptions(CommandOption verboseOption, CommandOption veryVerboseOption)
        {
            _verboseOption = verboseOption;
            _veryVerboseOption = veryVerboseOption;
        }

        public ILoggerFactory CreateLoggerFactory()
        {
            var filters = new LoggerFilterOptions();

            var level = LogLevel.Information;
            if(_veryVerboseOption.HasValue())
            {
                level = LogLevel.Trace;
            }
            else if(_verboseOption.HasValue())
            {
                level = LogLevel.Debug;
            }

            filters.Rules.Add(new LoggerFilterRule(
                providerName: null,
                categoryName: null,
                logLevel: level,
                filter: (_, __, ___) => true));

            if(!_veryVerboseOption.HasValue())
            {
                filters.Rules.Add(new LoggerFilterRule(
                    providerName: null,
                    categoryName: "Microsoft.EntityFrameworkCore",
                    logLevel: null,
                    filter: (_, __, ___) => false));
            }

            var factory = new LoggerFactory(new[] {
                new CliConsoleLoggerProvider()
            }, filters);
            return factory;
        }

        public static LoggingOptions Register(CommandLineApplication app)
        {
            return new LoggingOptions(
                verboseOption: app.Option("-v|--verbose", "Be verbose", CommandOptionType.NoValue),
                veryVerboseOption: app.Option("-vv|--very-verbose", "Be very verbose", CommandOptionType.NoValue));
        }
    }
}
