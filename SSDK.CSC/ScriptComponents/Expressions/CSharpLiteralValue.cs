using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSDK.CSC.ScriptComponents.Expressions
{
    /// <summary>
    /// A c# primitive literal value (such as string)
    /// </summary>
    public sealed class CSharpLiteralValue : CSharpExpression
    {
        #region Properties & Fields
        /// <summary>
        /// Gets the actual literal value of this expression
        /// </summary>
        public object Value { get; private set; }
        #endregion
        /// <summary>
        /// Creates the literal value from the given syntax
        /// </summary>
        /// <param name="syntax">the syntax to create from</param>
        internal CSharpLiteralValue(LiteralExpressionSyntax syntax)
        {
            Syntax = syntax;
            Value = syntax.Token.Value;
        }

        public override string ToString()
        {
            return $"{Value} ({Value.GetType().Name.ToLower()})";
        }
    }

    public enum CSharpLiteralType
    {
        String,
    }
}
