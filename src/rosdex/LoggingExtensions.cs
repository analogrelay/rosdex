using Microsoft.CodeAnalysis;

namespace Microsoft.Extensions.Logging
{
    internal static class LoggingExtensions
    {
        public static void LogCritical(this ILogger self, Location location, string message, params object[] args) => self.LogCritical(ApplyLocationToMessage(message), ApplyLocationToArgs(location, args));
        public static void LogError(this ILogger self, Location location, string message, params object[] args) => self.LogError(ApplyLocationToMessage(message), ApplyLocationToArgs(location, args));
        public static void LogWarning(this ILogger self, Location location, string message, params object[] args) => self.LogWarning(ApplyLocationToMessage(message), ApplyLocationToArgs(location, args));
        public static void LogInformation(this ILogger self, Location location, string message, params object[] args) => self.LogInformation(ApplyLocationToMessage(message), ApplyLocationToArgs(location, args));
        public static void LogDebug(this ILogger self, Location location, string message, params object[] args) => self.LogDebug(ApplyLocationToMessage(message), ApplyLocationToArgs(location, args));
        public static void LogTrace(this ILogger self, Location location, string message, params object[] args) => self.LogTrace(ApplyLocationToMessage(message), ApplyLocationToArgs(location, args));

        private static string ApplyLocationToMessage(string message) => "{FilePath}@{LineNumber},{ColumnNumber} " + message;

        private static object[] ApplyLocationToArgs(Location location, object[] args)
        {
            // TODO: Make faster.
            var newArgs = new object[args.Length + 3];
            args.CopyTo(newArgs, 3);

            var lineSpan = location.GetMappedLineSpan();
            newArgs[0] = lineSpan.Path;
            newArgs[1] = lineSpan.StartLinePosition.Line;
            newArgs[2] = lineSpan.StartLinePosition.Character;
            return newArgs;
        }
    }
}
