using System;
using System.Collections.Generic;

namespace Rosdex.Model
{
    public class Snapshot
    {
        public string Name { get; }
        public IReadOnlyList<SymbolDefinition> SymbolDefinitions { get; }
        public IReadOnlyList<SymbolReference> SymbolReferences { get; }
        public IReadOnlyList<Project> Projects { get; }

        public Snapshot(string name, IReadOnlyList<SymbolDefinition> symbolDefinitions, IReadOnlyList<SymbolReference> symbolReferences, IReadOnlyList<Project> projects)
        {
            Name = !string.IsNullOrEmpty(name) ? name : throw new ArgumentNullException(nameof(name));
            SymbolDefinitions = symbolDefinitions ?? throw new ArgumentNullException(nameof(symbolDefinitions));
            SymbolReferences = symbolReferences ?? throw new ArgumentNullException(nameof(symbolReferences));
            Projects = projects ?? throw new ArgumentNullException(nameof(projects));
        }
    }
}
