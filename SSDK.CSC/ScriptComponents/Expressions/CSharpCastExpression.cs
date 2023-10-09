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
    /// A c# cast expression
    /// </summary>
    public sealed class CSharpCastExpression : CSharpExpression
    {
        #region Properties & Fields
        /// <summary>
        /// Gets the expression to be casted to the type..
        /// </summary>
        public CSharpExpression Expression { get; private set; }

        /// <summary>
        /// Gets the cast type
        /// </summary>
        public CSharpType Type { get; private set; }
        #endregion

        /// <summary>
        /// Creates the cast from the given syntax
        /// </summary>
        /// <param name="syntax">the syntax to create from</param>
        internal CSharpCastExpression(CastExpressionSyntax syntax)
        {
            Syntax = syntax;
            Expression = syntax.Expression.ToExpression();
            Type = syntax.Type.ToType();
        }

        public override void ProcessMap(CSharpConversionMap map, StringBuilder result)
        {
            map.ProcessCastExpression(this, result);
        }

        internal override void CreateMemberSymbols(CSharpProject project, CSharpMemberSymbol parentSymbol)
        {
            Symbol = new CSharpMemberSymbol("cast(", parentSymbol, this, false);
            Type?.CreateMemberSymbols(project, Symbol);
            Expression?.CreateMemberSymbols(project, Symbol);
        }

        internal override void ResolveMembers(CSharpProject project)
        {
            Type?.ResolveMembers(project);
            Expression?.ResolveMembers(project);
        }

        public override string ToString()
        {
            return $"({Type}){Expression}";
        }
    }
}
