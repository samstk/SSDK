using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SSDK.CSC.Helpers;
using SSDK.CSC.ScriptComponents.Statements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSDK.CSC.ScriptComponents
{
    /// <summary>
    /// A c# block for checked/unchecked for intergral-type arithmetic arithmetic operations.
    /// </summary>
    public sealed class CSharpCheckedContextStatement : CSharpStatement
    {
        #region Properties & Fields
        /// <summary>
        /// Gets the block context.
        /// </summary>
        public CSharpStatementBlock Block { get; private set; }

        /// <summary>
        /// Gets whether the block context is checked/unchecked.
        /// It is unchecked if false.
        /// </summary>
        public bool Checked { get; set; } = false;
        #endregion

        /// <summary>
        /// Creates a new checked/unchecked block from the given syntax
        /// </summary>
        /// <param name="syntax">the syntax</param>
        internal CSharpCheckedContextStatement(CheckedStatementSyntax syntax)
        {
            Syntax = syntax;
            Block = new CSharpStatementBlock(syntax.Block);
            Checked = syntax.Keyword.RawKind == (int)SyntaxKind.CheckedKeyword;
        }


        public override void ProcessMap(CSharpConversionMap map, StringBuilder result)
        {
            map.ProcessCheckedContextStatement(this, result);
        }

        internal override void CreateMemberSymbols(CSharpProject project, CSharpMemberSymbol parentSymbol)
        {
            Symbol = new CSharpMemberSymbol(Checked ? "checked{" : "unchecked{", parentSymbol, this, false);
            Block?.CreateMemberSymbols(project, Symbol);
        }

        internal override void ResolveMembers(CSharpProject project)
        {
            Block?.ResolveMembers(project);
        }

        public override string ToString()
        {
            return Checked ? "checked ..." : "unchecked ...";
        }
    }
}
