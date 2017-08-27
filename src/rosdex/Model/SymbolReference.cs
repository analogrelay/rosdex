namespace Rosdex.Model
{
    public class SymbolReference : SymbolBase
    {
        public SymbolReference(SymbolPath path, string name, string fullName, SymbolType type, SourceSpan location) :
            base(path, name, fullName, type, location)
        {
        }
    }
}
