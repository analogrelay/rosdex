namespace Rosdex.Model
{
    public class SymbolDefinition
    {
        public SymbolPath Path { get; }
        public string Name { get; }
        public string FullName { get; }
        public SymbolType Type { get; }
        public SourceSpan Location { get; }

        public SymbolDefinition(SymbolPath path, string name, string fullName, SymbolType type, SourceSpan location)
        {
            Path = path;
            Name = name;
            FullName = fullName;
            Type = type;
            Location = location;
        }
    }
}
