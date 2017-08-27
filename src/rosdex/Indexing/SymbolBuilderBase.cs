using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Rosdex.Model;

namespace Rosdex.Indexing
{
    public abstract class SymbolBuilderBase
    {
        public SymbolPath Path { get; set; }
        public ISymbol Symbol { get; set; }
        public Location Location { get; set; }

        protected SymbolBuilderBase() { }

        protected SymbolBuilderBase(SymbolPath path, ISymbol symbol, Location location)
        {
            Path = path ?? throw new ArgumentNullException(nameof(path));
            Symbol = symbol ?? throw new ArgumentNullException(nameof(symbol));
            Location = location ?? throw new ArgumentNullException(nameof(location));
        }

        protected static SymbolType GetSymbolType(ISymbol symbol)
        {
            switch (symbol)
            {
                case INamedTypeSymbol namedType:
                    return GetSymbolType(namedType.TypeKind);
                default:
                    throw new NotImplementedException($"Unrecognized Symbol Kind: {symbol.Kind}");
            }
        }

        protected static SymbolType GetSymbolType(TypeKind typeKind)
        {
            switch (typeKind)
            {
                case TypeKind.Class:
                    return SymbolType.Class;
                default:
                    throw new NotImplementedException($"Unrecognized Type Kind: {typeKind}");
            }
        }

        protected static string GetFullName(ISymbol symbol)
        {
            if (symbol.ContainingAssembly == null)
            {
                return $"{GetFullNamespaceName(symbol.ContainingNamespace)}.{symbol.Name}";
            }
            else
            {
                return $"{GetFullNamespaceName(symbol.ContainingNamespace)}.{symbol.Name}, {symbol.ContainingAssembly.Name}";
            }
        }

        protected static string GetFullNamespaceName(INamespaceSymbol ns)
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
