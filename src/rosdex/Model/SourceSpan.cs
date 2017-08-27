using System;
using Microsoft.CodeAnalysis;

namespace Rosdex.Model
{
    public struct SourceSpan : IEquatable<SourceSpan>
    {
        public string Path { get; }
        public SourceLocation Start { get; }
        public SourceLocation End { get; }

        public SourceSpan(string path, SourceLocation start, SourceLocation end)
        {
            Path = !string.IsNullOrEmpty(path) ? path : throw new ArgumentNullException(nameof(path));
            Start = start;
            End = end;
        }

        public bool Equals(SourceSpan other) =>
            string.Equals(Path, other.Path, StringComparison.Ordinal) &&
            Equals(Start, other.Start) &&
            Equals(End, other.End);

        public override bool Equals(object obj) => obj is SourceSpan other && Equals(other);

        public override int GetHashCode() => HashCode.New()
            .Add(Path)
            .Add(Start)
            .Add(End);

        public static bool operator ==(SourceSpan left, SourceSpan right) => Equals(left, right);
        public static bool operator !=(SourceSpan left, SourceSpan right) => !Equals(left, right);

        public override string ToString()
        {
            if(End == Start)
            {
                return $"{Path}@{Start}";
            }
            else
            {
                return $"{Path}@{Start}->{End}";
            }
        }

        public static SourceSpan FromLocation(Location location)
        {
            var mappedLineSpan = location.GetMappedLineSpan();
            return new SourceSpan(
                mappedLineSpan.Path,
                SourceLocation.FromLinePosition(mappedLineSpan.StartLinePosition),
                SourceLocation.FromLinePosition(mappedLineSpan.EndLinePosition));
        }
    }
}
