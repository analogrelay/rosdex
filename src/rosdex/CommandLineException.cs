using System;
using System.Runtime.Serialization;

namespace Rosdex
{
    [Serializable]
    internal class CommandLineException : Exception
    {
        public byte ExitCode { get; }

        public CommandLineException(string message) : this(message, 1) { }

        public CommandLineException(string message, byte exitCode) : base(message)
        {
            ExitCode = exitCode;
        }

        public CommandLineException(string message, Exception innerException) : this(message, 1, innerException) { }

        public CommandLineException(string message, byte exitCode, Exception innerException) : base(message, innerException)
        {
            ExitCode = exitCode;
        }

        protected CommandLineException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            ExitCode = info.GetByte(nameof(ExitCode));
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue(nameof(ExitCode), ExitCode);
        }
    }
}
