using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis.Text;
using Rosdex.Model;

namespace Rosdex.Indexing
{
    public class DocumentBuilder
    {
        public string Name { get; set; }
        public string FilePath { get; set; }
        public IReadOnlyList<string> Folders { get; set; }

        public ProjectBuilder Project { get; }
        public SnapshotBuilder Snapshot => Project.Snapshot;

        public DocumentBuilder(ProjectBuilder project)
        {
            Project = project;
        }

        public Document Build()
        {
            return new Document(
                Name,
                FilePath,
                Folders);
        }
    }
}
