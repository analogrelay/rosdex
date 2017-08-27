using Microsoft.CodeAnalysis;
using Rosdex.Model;

namespace Rosdex.Indexing
{

    public class SymbolDefinitionBuilder : SymbolBuilderBase
    {
        public SymbolDefinitionBuilder() : base() { }

        public SymbolDefinitionBuilder(SymbolPath path, ISymbol symbol, Location location) : base(path, symbol, location)
        {
        }

        public SymbolDefinition Build()
        {
            return new SymbolDefinition(
                Path,
                Symbol.Name,
                GetFullName(Symbol),
                GetSymbolType(Symbol),
                SourceSpan.FromLocation(Location));
        }
    }
}
