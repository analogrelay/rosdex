using System;

namespace Rosdex.Indexing
{
    public class ProjectBuilder
    {
        public SnapshotBuilder Snapshot { get; }

        public ProjectBuilder(SnapshotBuilder snapshot)
        {
            Snapshot = snapshot;
        }

        public void Build()
        {
        }
    }
}
