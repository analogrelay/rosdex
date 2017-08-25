using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Rosdex.Indexing
{
    public class DocumentBuilder
    {
        public string Name { get; set; }
        public string FilePath { get; set; }
        public IReadOnlyList<string> Folders { get; set; }
        public SourceText Text { get; set; }

        public ProjectBuilder Project { get; }
        public SnapshotBuilder Snapshot => Project.Snapshot;

        public DocumentBuilder(ProjectBuilder project)
        {
            Project = project;
        }

        public void Build()
        {
        }

        public void DefineSymbol(ISymbol symbol, SyntaxNode node)
        {
            // Record the symbol in the snapshot, referring to this document and position
            Snapshot.DefineSymbol(symbol, this, node.GetLocation());
        }
    }
}
