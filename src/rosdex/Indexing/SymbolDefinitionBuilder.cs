using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Rosdex.Model;

namespace Rosdex.Indexing
{
    public class SymbolDefinitionBuilder
    {
        public SymbolPath Path { get; set; }
        public ISymbol Symbol { get; set; }
        public Location Location { get; set; }

        public SymbolDefinitionBuilder() { }

        public SymbolDefinitionBuilder(SymbolPath path, ISymbol symbol, Location location)
        {
            Path = path;
            Symbol = symbol;
            Location = location;
        }

        public SymbolDefinition Build()
        {
            return new SymbolDefinition(
                Path,
                Symbol.Name,
                GetFullName(Symbol),
                GetSymbolType(Symbol),
                SourceSpan.FromLocation(Location));
        }

        private SymbolType GetSymbolType(ISymbol symbol)
        {
            switch (symbol)
            {
                case INamedTypeSymbol namedType:
                    return GetSymbolType(namedType.TypeKind);
                default:
                    throw new NotImplementedException($"Unrecognized Symbol Kind: {symbol.Kind}");
            }
        }

        private SymbolType GetSymbolType(TypeKind typeKind)
        {
            switch (typeKind)
            {
                case TypeKind.Class:
                    return SymbolType.Class;
                default:
                    throw new NotImplementedException($"Unrecognized Type Kind: {typeKind}");
            }
        }

        private string GetFullName(ISymbol symbol)
        {
            return $"{GetFullNamespaceName(symbol.ContainingNamespace)}.{symbol.Name}, {symbol.ContainingAssembly.Name}";
        }

        private string GetFullNamespaceName(INamespaceSymbol ns)
        {
            var namespaces = new List<string>();
            var current = ns;
            while(current != null && !current.IsGlobalNamespace)
            {
                namespaces.Insert(0, current.Name);
                current = current.ContainingNamespace;
            }
            return string.Join(".", namespaces);
        }
    }
}
