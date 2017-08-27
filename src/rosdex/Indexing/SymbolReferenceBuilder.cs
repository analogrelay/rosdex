using Microsoft.CodeAnalysis;
using Rosdex.Model;

namespace Rosdex.Indexing
{
    public class SymbolReferenceBuilder : SymbolBuilderBase
    {
        public SymbolReferenceBuilder() : base() { }

        public SymbolReferenceBuilder(SymbolPath path, ISymbol symbol, Location location) : base(path, symbol, location)
        {
        }

        public SymbolReference Build()
        {
            return new SymbolReference(
                Path,
                Symbol.Name,
                GetFullName(Symbol),
                GetSymbolType(Symbol),
                SourceSpan.FromLocation(Location));
        }
    }
}
