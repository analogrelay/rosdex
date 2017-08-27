namespace Rosdex.Model
{
    public class SymbolDefinition : SymbolBase
    {
        public SymbolDefinition(SymbolPath path, string name, string fullName, SymbolType type, SourceSpan location) :
            base(path, name, fullName, type, location)
        {
        }
    }
}
