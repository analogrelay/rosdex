using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Rosdex.Model;

using Project = Rosdex.Model.Project;

namespace Rosdex.Indexing
{
    public class SnapshotBuilder
    {
        public IDictionary<SymbolPath, SymbolDefinitionBuilder> Symbols { get; } = new Dictionary<SymbolPath, SymbolDefinitionBuilder>();

        public void DefineSymbol(ISymbol symbol, DocumentBuilder document, Location location)
        {
            var path = SymbolPath.ForSymbol(symbol);
            Symbols.Add(path, new SymbolDefinitionBuilder(path, symbol, document, location));
        }

        public Snapshot Build()
        {
            return new Snapshot(Symbols.Values.Select(b => b.Build()).ToList(), new Project[0]);
        }
    }
}
