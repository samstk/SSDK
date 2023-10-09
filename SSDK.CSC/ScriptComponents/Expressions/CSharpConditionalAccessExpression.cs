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
    /// A C# conditional access expression (e.g. x?.ToString)
    /// </summary>
    public sealed class CSharpConditionalAccessExpression : CSharpExpression
    {
        #region Properties & Fields
        /// <summary>
        /// Gets the target expression, that is to be conditionally accessed.
        /// </summary>
        public CSharpExpression Target { get; private set; }
        
        #endregion

        /// <summary>
        /// Creates the Conditional access expression from the syntax.
        /// </summary>
        /// <param name="syntax">the syntax to create from</param>
        internal CSharpConditionalAccessExpression(ConditionalAccessExpressionSyntax syntax)
        {
            Syntax = syntax;
            Target = syntax.Expression.ToExpression();
        }

        public override void ProcessMap(CSharpConversionMap map, StringBuilder result)
        {
            map.ProcessConditionalAccessExpression(this, result);
        }

        internal override void CreateMemberSymbols(CSharpProject project, CSharpMemberSymbol parentSymbol)
        {
            Symbol = new CSharpMemberSymbol("?", parentSymbol, this, false);
            Target?.CreateMemberSymbols(project, Symbol);
        }

        public override string ToString()
        {
            return $"{Target}?";
        }

        internal override void ResolveMembers(CSharpProject project)
        {
            Target?.ResolveMembers(project);
        }

    }
}
