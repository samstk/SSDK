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
    /// A c# statement for any expression.
    /// </summary>
    public sealed class CSharpExpressionStatement : CSharpStatement
    {
        #region Properties & Fields
        /// <summary>
        /// Gets the assignment expression
        /// </summary>
        public CSharpExpression Expression { get; private set; }
        #endregion

        /// <summary>
        /// Creates a statement for any given c# expression
        /// </summary>
        /// <param name="expression">the expression to embed in this statement</param>
        public CSharpExpressionStatement(CSharpExpression expression)
        {
            Expression = expression;
        }

        public override void ProcessMap(CSharpConversionMap map, StringBuilder result)
        {
            map.ProcessExpressionStatement(this, result);
        }

        public override string ToString()
        {
            return $"{Expression};";
        }
    }
}
