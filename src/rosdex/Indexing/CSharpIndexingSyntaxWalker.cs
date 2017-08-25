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

        public CSharpIndexingSyntaxWalker(DocumentBuilder builder, SemanticModel semanticModel, ILogger<CSharpIndexingSyntaxWalker> logger) : base(SyntaxWalkerDepth.Trivia)
        {
            _builder = builder;
            _semanticModel = semanticModel;
            _logger = logger;
        }

        public override void VisitClassDeclaration(ClassDeclarationSyntax node)
        {
            // Define the symbol
            var symbol = _semanticModel.GetDeclaredSymbol(node);
            if(symbol == null)
            {
                _logger.LogWarning(node.GetLocation(), "Unable to resolve class declaration: {Name}", node.Identifier);
            }

            _builder.DefineSymbol(symbol, node);
            _logger.LogDebug(node.GetLocation(), "Defined symbol: {Name}", node.Identifier);

            base.VisitClassDeclaration(node);
        }
    }
}
