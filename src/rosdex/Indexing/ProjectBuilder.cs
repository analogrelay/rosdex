using System;
using System.Collections.Generic;
using System.Linq;
using Rosdex.Model;

namespace Rosdex.Indexing
{
    public class ProjectBuilder
    {
        public string Name { get; set; }
        public string FilePath { get; set; }
        public string Language { get; set; }
        public string AssemblyName { get; set; }
        
        public SnapshotBuilder Snapshot { get; }
        public IList<DocumentBuilder> Documents { get; } = new List<DocumentBuilder>();

        public ProjectBuilder(SnapshotBuilder snapshot)
        {
            Snapshot = snapshot;
        }

        public Project Build()
        {
            return new Project(
                Name,
                FilePath,
                AssemblyName,
                Language,
                Documents.Select(d => d.Build()).ToList());
        }
    }
}
