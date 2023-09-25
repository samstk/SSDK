using Microsoft.CodeAnalysis.CSharp.Syntax;
using SSDK.CSC.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSDK.CSC.ScriptComponents.Expressions
{
    /// <summary>
    /// A C# this reference expression
    /// </summary>
    public sealed class CSharpThisExpression : CSharpExpression
    {
        /// <summary>
        /// Creates the this expression from the syntax.
        /// </summary>
        /// <param name="syntax">the syntax to create from</param>
        internal CSharpThisExpression(ExpressionSyntax syntax)
        {
            Syntax = syntax;
        }

        public override void ProcessMap(CSharpConversionMap map, StringBuilder result)
        {
            map.ProcessThisExpression(this, result);
        }

        public override string ToString()
        {
            return $"this";
        }
    }
}
