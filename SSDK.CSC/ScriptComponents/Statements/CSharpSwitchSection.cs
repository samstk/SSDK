using Microsoft.CodeAnalysis.CSharp.Syntax;
using SSDK.CSC.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSDK.CSC.ScriptComponents.Statements
{
    /// <summary>
    /// A c# switch-case section.
    /// </summary>
    public sealed class CSharpSwitchSection
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

        public override string ToString()
        {
            return $"case {Labels.ToReadableString()}";
        }
    }
}
