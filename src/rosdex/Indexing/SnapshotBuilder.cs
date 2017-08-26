using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Rosdex.Model;

namespace Rosdex.Indexing
{
    public class SnapshotBuilder
    {
        public string Name { get; set; }

        public IDictionary<SymbolPath, SymbolDefinitionBuilder> Symbols { get; } = new Dictionary<SymbolPath, SymbolDefinitionBuilder>();
        public IList<ProjectBuilder> Projects { get; } = new List<ProjectBuilder>();

        public void DefineSymbol(ISymbol symbol, Location location)
        {
            var path = SymbolPath.ForSymbol(symbol);
            Symbols.Add(path, new SymbolDefinitionBuilder(path, symbol, location));
        }

        public Snapshot Build()
        {
            return new Snapshot(
                Name,
                Symbols.Values.Select(b => b.Build()).ToList(),
                Projects.Select(b => b.Build()).ToList());
        }
    }
}
