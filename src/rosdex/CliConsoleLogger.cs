using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.Extensions.Logging;

namespace Rosdex
{
    internal class CliConsoleLoggerProvider : ILoggerProvider
    {
        public ILogger CreateLogger(string categoryName)
        {
            return new CliConsoleLogger(categoryName);
        }

        public void Dispose()
        {
        }
    }

    internal class CliConsoleLogger : ILogger
    {
        private static object _consoleLock = new object();

        private readonly string _categoryName;
        private AsyncLocal<Stack<string>> _scopes = new AsyncLocal<Stack<string>>();

        public CliConsoleLogger(string categoryName)
        {
            _categoryName = categoryName;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            var scopes = _scopes.Value;
            if (scopes == null)
            {
                scopes = new Stack<string>();
                _scopes.Value = scopes;
            }
            scopes.Push(state.ToString());
            return new Disposable(() =>
            {
                _scopes.Value.Pop();
            });
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            // Don't run user code in the lock!
            var message = formatter(state, exception);

            lock (_consoleLock)
            {
                var scopes = _scopes.Value?.ToList();
                var scopeStr = string.Empty;
                if (scopes?.Count > 0)
                {
                    scopes.Reverse();
                    scopeStr = string.Join(" ", scopes) + " ";
                }
                var (prefix, fg, bg) = GetLogLevelInfo(logLevel);

                var oldFg = Console.ForegroundColor;
                var oldBg = Console.BackgroundColor;
                Console.ForegroundColor = fg;
                Console.BackgroundColor = bg;

                Console.Write(prefix);

                Console.ForegroundColor = oldFg;
                Console.BackgroundColor = oldBg;

                Console.WriteLine($": {scopeStr}{message}");
            }
        }

        private (string prefix, ConsoleColor foreground, ConsoleColor background) GetLogLevelInfo(LogLevel logLevel)
        {
            // We must explicitly set the background color if we are setting the foreground color,
            // since just setting one can look bad on the users console.
            switch (logLevel)
            {
                case LogLevel.Critical:
                    return ("crit", ConsoleColor.White, ConsoleColor.Red);
                case LogLevel.Error:
                    return ("fail", ConsoleColor.Red, ConsoleColor.Black);
                case LogLevel.Warning:
                    return ("warn", ConsoleColor.Yellow, ConsoleColor.Black);
                case LogLevel.Information:
                    return ("info", ConsoleColor.DarkGreen, ConsoleColor.Black);
                case LogLevel.Debug:
                    return ("dbug", ConsoleColor.Gray, ConsoleColor.Black);
                case LogLevel.Trace:
                    return ("trce", ConsoleColor.Gray, ConsoleColor.Black);
                default:
                    throw new ArgumentOutOfRangeException(nameof(logLevel));
            }
        }
    }

    internal class Disposable : IDisposable
    {
        public static readonly IDisposable Null = new Disposable();
        private readonly Action _action;

        private Disposable() : this(DoNothing)
        {
        }

        public Disposable(Action action)
        {
            _action = action;
        }

        public void Dispose()
        {
            _action();
        }

        private static void DoNothing()
        {
        }
    }
}
