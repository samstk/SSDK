using Microsoft.CodeAnalysis.CSharp.Syntax;
using SSDK.CSC.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Collections.Specialized.BitVector32;

namespace SSDK.CSC.ScriptComponents.Statements
{
    /// <summary>
    /// A c# switch-case section.
    /// </summary>
    public sealed class CSharpSwitchSection : CSharpComponent
    {
        #region Properties & Fields
        /// <summary>
        /// Gets the labels that can switch to this section
        /// </summary>
        public CSharpExpression[] Labels { get; private set; }

        /// <summary>
        /// Gets the syntax that formed this case.
        /// </summary>
        public SwitchSectionSyntax Syntax { get; private set; }

        /// <summary>
        /// Gets the execution block of this case.
        /// </summary>
        public CSharpStatementBlock Block { get; private set; }
        #endregion


        /// <summary>
        /// Creates the switch statement from the given syntax.
        /// </summary>
        /// <param name="returnSyntax">the syntax to create from</param>
        internal CSharpSwitchSection(SwitchSectionSyntax syntax)
        {
            Syntax = syntax;
            Labels = syntax.Labels.ToExpressions();
            Block = new CSharpStatementBlock(syntax.Statements, syntax.Parent as StatementSyntax);
        }

        internal override void CreateMemberSymbols(CSharpProject project, CSharpMemberSymbol parentSymbol)
        {
            foreach (CSharpExpression expr in Labels)
            {
                expr.CreateMemberSymbols(project, Symbol);
            }

            Block.CreateMemberSymbols(project, Symbol);
        }

        internal override void ResolveMembers(CSharpProject project)
        {
            foreach (CSharpExpression expr in Labels)
            {
                expr.ResolveMembers(project);
            }

            Block.ResolveMembers(project);
        }

        public override string ToString()
        {
            return $"case {Labels.ToReadableString()}";
        }
    }
}
