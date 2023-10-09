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
        public CSharpExpressionStatement(CSharpExpression expression, StatementSyntax syntax)
        {
            Expression = expression;
            Syntax = syntax;
        }

        public override void ProcessMap(CSharpConversionMap map, StringBuilder result)
        {
            map.ProcessExpressionStatement(this, result);
        }

        internal override void ResolveMembers(CSharpProject project)
        {
            Expression.ResolveMembers(project);
        }

        internal override void CreateMemberSymbols(CSharpProject project, CSharpMemberSymbol parentSymbol)
        {
            Symbol = new CSharpMemberSymbol("(", parentSymbol, this, false);
            Expression?.CreateMemberSymbols(project, Symbol);
        }

        public override string ToString()
        {
            return $"{Expression};";
        }
    }
}
