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
    /// A c# goto statement for a given label.
    /// </summary>
    public sealed class CSharpGotoStatement : CSharpStatement
    {
        #region Properties & Fields
        /// <summary>
        /// Gets the expression holding the label
        /// </summary>
        public CSharpExpression Expression { get; private set; }
        #endregion


        /// <summary>
        /// Creates the goto statement from the given syntax.
        /// </summary>
        /// <param name="syntax">the syntax to create from</param>
        internal CSharpGotoStatement(GotoStatementSyntax syntax)
        {
            if (syntax.Expression != null)
                Expression = syntax.Expression.ToExpression();
            Syntax = syntax;
        }

        public override void ProcessMap(CSharpConversionMap map, StringBuilder result)
        {
            map.ProcessGotoStatement(this, result);
        }

        public override string ToString()
        {
            return $"goto {Expression};";
        }
    }
}
