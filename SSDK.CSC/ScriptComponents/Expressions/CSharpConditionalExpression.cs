using Microsoft.CodeAnalysis.CSharp;
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
    /// A c# conditional expression
    /// </summary>
    public sealed class CSharpConditionalExpression : CSharpExpression
    {
        #region Properties & Fields
        /// <summary>
        /// Gets the expression that is to be evaluated.
        /// </summary>
        public CSharpExpression Condition { get; private set; }

        /// <summary>
        /// Gets the expression on evaluated true.
        /// </summary>
        public CSharpExpression ExpressionOnTrue { get; private set; }

        /// <summary>
        /// Gets the expression on evaluated false.
        /// </summary>
        public CSharpExpression ExpressionOnFalse { get; private set; }
        #endregion

        /// <summary>
        /// Creates the condition and values from the given syntax
        /// </summary>
        /// <param name="syntax">the syntax to create from</param>
        internal CSharpConditionalExpression(ConditionalExpressionSyntax syntax)
        {
            Syntax = syntax;
            Condition = syntax.Condition.ToExpression();
            ExpressionOnTrue = syntax.WhenTrue.ToExpression();
            ExpressionOnFalse = syntax.WhenFalse.ToExpression();
        }

        public override void ProcessMap(CSharpConversionMap map, StringBuilder result)
        {
            map.ProcessConditionalExpression(this, result);
        }

        public override string ToString()
        {
            return $"{Condition} ? {ExpressionOnTrue} : {ExpressionOnFalse}";
        }
    }
}
