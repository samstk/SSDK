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
    /// A C# assignment expression (e.g. x = 3)
    /// </summary>
    public sealed class CSharpAssignmentExpression : CSharpExpression
    {
        #region Properties & Fields
        /// <summary>
        /// Gets the left-hand side expression of this expression.
        /// </summary>
        public CSharpExpression Left { get; private set; }

        /// <summary>
        /// Gets the right-hand side expression of this expression.
        /// </summary>
        public CSharpExpression Right { get; private set; }

        /// <summary>
        /// Gets the operator of this expression
        /// </summary>
        public string Operator { get; private set; }

        /// <summary>
        /// Gets the referenced symbol for the operator method.
        /// </summary>
        public CSharpMemberSymbol ReferencedOperatorSymbol { get; private set; }
        #endregion

        /// <summary>
        /// Creates the binary expression from the syntax.
        /// </summary>
        /// <param name="syntax">the syntax to create from</param>
        internal CSharpAssignmentExpression(AssignmentExpressionSyntax syntax)
        {
            Syntax = syntax;
            Left = syntax.Left.ToExpression();
            Operator = syntax.OperatorToken.Text;
            Right = syntax.Right.ToExpression();
        }

        public override void ProcessMap(CSharpConversionMap map, StringBuilder result)
        {
            map.ProcessAssignmentExpression(this, result);
        }

        internal override void CreateMemberSymbols(CSharpProject project, CSharpMemberSymbol parentSymbol)
        {
            Symbol = new CSharpMemberSymbol("assign", parentSymbol, this, false);
            Left?.CreateMemberSymbols(project, Symbol);
            Right?.CreateMemberSymbols(project, Symbol);
        }

        public override string ToString()
        {
            return $"{Left} {Operator} {Right}";
        }

        internal override void ResolveMembers(CSharpProject project)
        {
            Left?.ResolveMembers(project);
            Right?.ResolveMembers(project);
        }
    }
}
