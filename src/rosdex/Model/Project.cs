using System.Collections.Generic;

namespace Rosdex.Model
{
    public class Project
    {
        public string Name { get; }
        public string FilePath { get; }
        public string Language { get; }
        public string AssemblyName { get; }
        public IReadOnlyList<Document> Documents { get; }

        public Project(string name, string filePath, string assemblyName, string language, IReadOnlyList<Document> documents)
        {
            Name = name;
            FilePath = filePath;
            AssemblyName = assemblyName;
            Language = language;
            Documents = documents;
        }
    }
}
