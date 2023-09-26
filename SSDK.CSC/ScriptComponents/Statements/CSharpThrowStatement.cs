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
    /// A c# throw statement for a given expression.
    /// </summary>
    public sealed class CSharpThrowStatement : CSharpStatement
    {
        #region Properties & Fields
        /// <summary>
        /// Gets the expression thrown
        /// </summary>
        public CSharpExpression Expression { get; private set; }
        #endregion


        /// <summary>
        /// Creates the throw statement from the given syntax.
        /// </summary>
        /// <param name="throwSyntax">the syntax to create from</param>
        internal CSharpThrowStatement(ThrowStatementSyntax throwSyntax)
        {
            if (throwSyntax.Expression != null)
                Expression = throwSyntax.Expression.ToExpression();
            Syntax = throwSyntax;
        }

        public override void ProcessMap(CSharpConversionMap map, StringBuilder result)
        {
            map.ProcessThrowStatement(this, result);
        }

        public override string ToString()
        {
            return $"throw {Expression};";
        }
    }
}
