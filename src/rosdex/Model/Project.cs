using System.Collections.Generic;

namespace Rosdex.Model
{
    public class Project
    {
        public IReadOnlyList<Document> Documents { get; }

        public Project(IReadOnlyList<Document> documents)
        {
            Documents = documents;
        }
    }
}
