using System;
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
            Name = !string.IsNullOrEmpty(name) ? name : throw new ArgumentNullException(nameof(name));
            FilePath = !string.IsNullOrEmpty(filePath) ? filePath : throw new ArgumentNullException(nameof(filePath));
            AssemblyName = !string.IsNullOrEmpty(assemblyName) ? assemblyName : throw new ArgumentNullException(nameof(assemblyName));
            Language = !string.IsNullOrEmpty(language) ? language : throw new ArgumentNullException(nameof(language));
            Documents = documents ?? throw new ArgumentNullException(nameof(documents));
        }
    }
}
