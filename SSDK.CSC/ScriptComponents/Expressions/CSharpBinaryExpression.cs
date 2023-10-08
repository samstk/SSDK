using Microsoft.CodeAnalysis.CSharp.Syntax;
using SSDK.CSC.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization.Metadata;
using System.Threading.Tasks;

namespace SSDK.CSC.ScriptComponents.Expressions
{
    /// <summary>
    /// A C# binary operator expression (e.g. 2 + 3)
    /// </summary>
    public sealed class CSharpBinaryExpression : CSharpExpression
    {
        #region Properties & Fields
        /// <summary>
        /// Gets the left-hand side expression of this binary expression.
        /// </summary>
        public CSharpExpression Left { get; private set; }

        /// <summary>
        /// Gets the right-hand side expression of this binary expression.
        /// </summary>
        public CSharpExpression Right { get; private set; }

        /// <summary>
        /// Gets the operator of this binary expression
        /// </summary>
        public string Operator { get; private set; }
        #endregion

        /// <summary>
        /// Creates the binary expression from the syntax.
        /// </summary>
        /// <param name="syntax">the syntax to create from</param>
        internal CSharpBinaryExpression(BinaryExpressionSyntax syntax) {
            Syntax = syntax;
            Left = syntax.Left.ToExpression();
            Operator = syntax.OperatorToken.Text;
            Right = syntax.Right.ToExpression();
        }

        public override void ProcessMap(CSharpConversionMap map, StringBuilder result)
        {
            map.ProcessBinaryExpression(this, result);
        }

        internal override void CreateMemberSymbols(CSharpProject project, CSharpMemberSymbol parentSymbol)
        {
            Symbol = new CSharpMemberSymbol("expr<a?b>(", parentSymbol, this, false);
            Left?.CreateMemberSymbols(project, Symbol);
            Right?.CreateMemberSymbols(project, Symbol);
        }

        internal override void ResolveMembers(CSharpProject project)
        {
            Left?.ResolveMembers(project);
            Right?.ResolveMembers(project);
        }

        public override string ToString()
        {
            return $"{Left} {Operator} {Right}";
        }
    }
}
