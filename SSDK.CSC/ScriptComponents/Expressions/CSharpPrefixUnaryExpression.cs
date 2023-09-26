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
    /// A C# prefix unary operator expression (e.g. !true)
    /// </summary>
    public sealed class CSharpPrefixUnaryExpression : CSharpExpression
    {
        #region Properties & Fields

        /// <summary>
        /// Gets the expression to apply the operator to.
        /// </summary>
        public CSharpExpression On { get; private set; }

        /// <summary>
        /// Gets the operator of this prefix unary expression
        /// </summary>
        public string Operator { get; private set; }
        #endregion

        /// <summary>
        /// Creates the prefix unary expression from the syntax.
        /// </summary>
        /// <param name="syntax">the syntax to create from</param>
        internal CSharpPrefixUnaryExpression(PrefixUnaryExpressionSyntax syntax)
        {
            Syntax = syntax;
            On = syntax.Operand.ToExpression();
            Operator = syntax.OperatorToken.Text;
        }

        public override void ProcessMap(CSharpConversionMap map, StringBuilder result)
        {
            map.ProcessPrefixUnaryExpression(this, result);
        }

        public override string ToString()
        {
            return $"{Operator}{On}";
        }
    }
}
