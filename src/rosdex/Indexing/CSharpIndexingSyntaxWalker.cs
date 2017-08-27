using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Logging;

namespace Rosdex.Indexing
{
    public class CSharpIndexingSyntaxWalker : CSharpSyntaxWalker
    {
        private readonly DocumentBuilder _builder;
        private readonly SemanticModel _semanticModel;
        private readonly ILogger<CSharpIndexingSyntaxWalker> _logger;
        private readonly CancellationToken _cancellationToken;

        public CSharpIndexingSyntaxWalker(DocumentBuilder builder, SemanticModel semanticModel, ILogger<CSharpIndexingSyntaxWalker> logger, CancellationToken cancellationToken) : base(SyntaxWalkerDepth.Trivia)
        {
            _builder = builder;
            _semanticModel = semanticModel;
            _logger = logger;
            _cancellationToken = cancellationToken;
        }

        public override void VisitClassDeclaration(ClassDeclarationSyntax node)
        {
            // Define the symbol
            var symbol = _semanticModel.GetDeclaredSymbol(node);
            if (symbol == null)
            {
                _logger.LogWarning(node.GetLocation(), "Unable to resolve class declaration: {Name}.", node.Identifier);
            }

            _builder.Snapshot.DefineSymbol(symbol, node.GetLocation());
            _logger.LogTrace(node.GetLocation(), "Defined symbol: {Name}.", node.Identifier);

            base.VisitClassDeclaration(node);
        }

        public override void VisitIdentifierName(IdentifierNameSyntax node)
        {
            // See if we're part of a qualified name
            var qname = node.FirstAncestorOrSelf<QualifiedNameSyntax>();
            var syntaxNode = qname == null ? (SyntaxNode)node : qname;

            // Get the symbol
            var symbolInfo = _semanticModel.GetSymbolInfo(syntaxNode, _cancellationToken);
            if (symbolInfo.Symbol == null)
            {
                _logger.LogWarning(node.GetLocation(), "Failed to resolve symbol for: {Syntax}", syntaxNode.ToString());
            }
            else if (symbolInfo.Symbol is INamedTypeSymbol namedTypeSymbol && namedTypeSymbol.TypeKind == TypeKind.Class)
            {
                _logger.LogTrace(node.GetLocation(), "Referenced symbol: {Kind}::{Name}", symbolInfo.Symbol.Kind, symbolInfo.Symbol.Name);
                _builder.Snapshot.ReferenceSymbol(symbolInfo.Symbol, node.GetLocation());
            }
            else
            {
                _logger.LogWarning(node.GetLocation(), "Discarding unsupported symbol: {Kind}::{Name}", symbolInfo.Symbol.Kind, symbolInfo.Symbol.Name);
            }

            base.VisitIdentifierName(node);
        }
    }
}
