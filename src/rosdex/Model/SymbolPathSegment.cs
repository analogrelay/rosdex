using System;
using Microsoft.CodeAnalysis;

namespace Rosdex.Model
{
    public struct SymbolPathSegment : IEquatable<SymbolPathSegment>
    {
        public SymbolKind Kind { get; }
        public string Name { get; }

        public SymbolPathSegment(SymbolKind kind, string name)
        {
            Kind = kind;
            Name = name ?? string.Empty;
        }

        public bool Equals(SymbolPathSegment other) =>
            other.Kind == Kind &&
            string.Equals(other.Name, Name, StringComparison.Ordinal);

        public override bool Equals(object obj) =>
            obj is SymbolPathSegment other && Equals(other);

        public override int GetHashCode() => HashCode.New()
            .Add(Name)
            .Add(Kind);

        public static bool operator ==(SymbolPathSegment left, SymbolPathSegment right) => Equals(left, right);
        public static bool operator !=(SymbolPathSegment left, SymbolPathSegment right) => !Equals(left, right);

        public static SymbolPathSegment ForSymbol(ISymbol symbol) =>
            new SymbolPathSegment(symbol.Kind, symbol.Name);

        public override string ToString() => $"{Kind}::{Name}";
    }
}
