using System.Collections.Generic;

namespace Rosdex.Model
{
    public class Snapshot
    {
        public IReadOnlyList<SymbolDefinition> Symbols { get; }
        public IReadOnlyList<Project> Projects { get; }

        public Snapshot(IReadOnlyList<SymbolDefinition> symbols, IReadOnlyList<Project> projects)
        {
            Symbols = symbols;
            Projects = projects;
        }
    }
}
