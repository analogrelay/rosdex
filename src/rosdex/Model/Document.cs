using System.Collections.Generic;

namespace Rosdex.Model
{
    public class Document
    {
        public string Name { get; }
        public string FilePath { get; }
        public IReadOnlyList<string> Folders { get; }

        public Document(string name, string filePath, IReadOnlyList<string> folders)
        {
            Name = name;
            FilePath = filePath;
            Folders = folders;
        }
    }
}
