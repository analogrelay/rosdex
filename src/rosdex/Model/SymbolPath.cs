using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace Rosdex.Model
{
    public class SymbolPath : IEquatable<SymbolPath>
    {
        public static readonly SymbolPath Empty = new SymbolPath();

        public IReadOnlyList<SymbolPathSegment> Segments { get; }

        private SymbolPath()
        {
            Segments = new SymbolPathSegment[0];
        }

        public SymbolPath(IEnumerable<SymbolPathSegment> segments)
        {
            Segments = segments.ToList();
        }

        public bool Equals(SymbolPath other) => Enumerable.SequenceEqual(Segments, other.Segments);
        public override bool Equals(object obj) => obj is SymbolPath other && Equals(other);
        public override int GetHashCode() => HashCode.New().Add(Segments);
        public override string ToString() => string.Join("/", Segments);
        public static bool operator ==(SymbolPath left, SymbolPath right) => Equals(left, right);
        public static bool operator !=(SymbolPath left, SymbolPath right) => !Equals(left, right);

        public static SymbolPath ForSymbol(ISymbol symbol)
        {
            var segments = new List<SymbolPathSegment>();
            var current = symbol;
            while(current != null)
            {
                segments.Insert(0, SymbolPathSegment.ForSymbol(current));
                current = current.ContainingSymbol;
            }
            return new SymbolPath(segments);
        }
    }
}
