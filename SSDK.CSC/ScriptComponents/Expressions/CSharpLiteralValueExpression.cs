using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Operations;
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
    public sealed class CSharpLiteralValueExpression : CSharpExpression
    {
        #region Properties & Fields
        /// <summary>
        /// Gets the actual literal value of this expression
        /// </summary>
        public object Value { get; private set; }

        /// <summary>
        /// Gets the symbol holding the literal value class/struct
        /// </summary>
        public CSharpMemberSymbol LiteralTypeSymbol { get; private set; }
        #endregion
        /// <summary>
        /// Creates the literal value from the given syntax
        /// </summary>
        /// <param name="syntax">the syntax to create from</param>
        internal CSharpLiteralValueExpression(LiteralExpressionSyntax syntax)
        {
            Syntax = syntax;
            Value = syntax.Token.Value;
        }

        /// <summary>
        /// Creates the literal value from the given syntax
        /// </summary>
        /// <param name="syntax">the syntax to create from</param>
        internal CSharpLiteralValueExpression(InterpolatedStringTextSyntax syntax)
        {
            Syntax = syntax.Parent as ExpressionSyntax;
            Value = syntax.TextToken.ValueText;
        }

        public override void ProcessMap(CSharpConversionMap map, StringBuilder result)
        {
            map.ProcessLiteralValueExpression(this, result);
        }

        internal override void CreateMemberSymbols(CSharpProject project, CSharpMemberSymbol parentSymbol)
        {
            Symbol = new CSharpMemberSymbol("literal", parentSymbol, this, false);
        }

        internal override void ResolveMembers(CSharpProject project)
        {
            if (Value is string)
                LiteralTypeSymbol = Symbol.FindBestMatchingSymbol("string", null);
            else if (Value is int)
                LiteralTypeSymbol = Symbol.FindBestMatchingSymbol("int", null);
            else if (Value is uint)
                LiteralTypeSymbol = Symbol.FindBestMatchingSymbol("uint", null);
            else if (Value is double)
                LiteralTypeSymbol = Symbol.FindBestMatchingSymbol("double", null);
            else if (Value is float)
                LiteralTypeSymbol = Symbol.FindBestMatchingSymbol("float", null);
            else if (Value is ulong)
                LiteralTypeSymbol = Symbol.FindBestMatchingSymbol("ulong", null);
            else if (Value is long)
                LiteralTypeSymbol = Symbol.FindBestMatchingSymbol("long", null);
            else if (Value is ushort)
                LiteralTypeSymbol = Symbol.FindBestMatchingSymbol("ushort", null);
            else if (Value is short)
                LiteralTypeSymbol = Symbol.FindBestMatchingSymbol("short", null);
            else if (Value is char)
                LiteralTypeSymbol = Symbol.FindBestMatchingSymbol("char", null);
            else if (Value is bool)
                LiteralTypeSymbol = Symbol.FindBestMatchingSymbol("bool", null);
        }

        public override string ToString()
        {
            return $"{Value} ({Value.GetType().Name.ToLower()})";
        }
    }
}
