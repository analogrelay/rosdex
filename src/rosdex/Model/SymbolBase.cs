using System;

namespace Rosdex.Model
{
    public abstract class SymbolBase
    {
        public SymbolPath Path { get; }
        public string Name { get; }
        public string FullName { get; }
        public SymbolType Type { get; }
        public SourceSpan Location { get; }

        protected SymbolBase(SymbolPath path, string name, string fullName, SymbolType type, SourceSpan location)
        {
            Path = path ?? throw new ArgumentNullException(nameof(path));
            Name = !string.IsNullOrEmpty(name) ? name : throw new ArgumentNullException(nameof(name));
            FullName = !string.IsNullOrEmpty(fullName) ? fullName : throw new ArgumentNullException(nameof(fullName));
            Type = type;
            Location = location;
        }
    }
}
