using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Rosdex.Model;

namespace Rosdex.Indexing
{
    public class SnapshotBuilder
    {
        public string Name { get; set; }

        public IDictionary<SymbolPath, SymbolDefinitionBuilder> SymbolDefinitions { get; } = new Dictionary<SymbolPath, SymbolDefinitionBuilder>();
        public IList<SymbolReferenceBuilder> SymbolReferences { get; } = new List<SymbolReferenceBuilder>();
        public IList<ProjectBuilder> Projects { get; } = new List<ProjectBuilder>();

        public Snapshot Build()
        {
            return new Snapshot(
                Name,
                SymbolDefinitions.Values.Select(b => b.Build()).ToList(),
                SymbolReferences.Select(b => b.Build()).ToList(),
                Projects.Select(b => b.Build()).ToList());
        }

        public void DefineSymbol(ISymbol symbol, Location location)
        {
            var path = SymbolPath.ForSymbol(symbol ?? throw new ArgumentNullException(nameof(symbol)));
            SymbolDefinitions.Add(path, new SymbolDefinitionBuilder(path, symbol, location ?? throw new ArgumentNullException(nameof(location))));
        }

        public void ReferenceSymbol(ISymbol symbol, Location location)
        {
            var path = SymbolPath.ForSymbol(symbol ?? throw new ArgumentNullException(nameof(symbol)));
            SymbolReferences.Add(new SymbolReferenceBuilder(path, symbol, location ?? throw new ArgumentNullException(nameof(location))));
        }
    }
}
