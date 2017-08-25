using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace Rosdex.Model
{
    public struct SourceLocation : IEquatable<SourceLocation>
    {
        public int Line { get; }
        public int Character { get; }

        public SourceLocation(int line, int character)
        {
            Line = line;
            Character = character;
        }

        public bool Equals(SourceLocation other) =>
            other.Line == Line &&
            other.Character == Character;

        public override bool Equals(object obj) => obj is SourceLocation other && Equals(other);

        public override int GetHashCode() => HashCode.New()
            .Add(Line)
            .Add(Character);

        public override string ToString() => $"{Line},{Character}";

        public static bool operator ==(SourceLocation left, SourceLocation right) => Equals(left, right);
        public static bool operator !=(SourceLocation left, SourceLocation right) => !Equals(left, right);

        public static SourceLocation FromLinePosition(LinePosition linePosition)
        {
            return new SourceLocation(linePosition.Line, linePosition.Character);
        }
    }
}
