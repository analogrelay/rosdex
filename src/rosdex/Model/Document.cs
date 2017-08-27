using System;
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
            Name = !string.IsNullOrEmpty(name) ? name : throw new ArgumentNullException(nameof(name));
            FilePath = !string.IsNullOrEmpty(filePath) ? filePath : throw new ArgumentNullException(nameof(filePath));
            Folders = folders ?? throw new ArgumentNullException(nameof(folders));
        }
    }
}
