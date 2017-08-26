using System.Collections.Generic;

namespace Rosdex.Model
{
    public class Snapshot
    {
        public string Name { get; }
        public IReadOnlyList<SymbolDefinition> Symbols { get; }
        public IReadOnlyList<Project> Projects { get; }

        public Snapshot(string name, IReadOnlyList<SymbolDefinition> symbols, IReadOnlyList<Project> projects)
        {
            Name = name;
            Symbols = symbols;
            Projects = projects;
        }
    }
}
