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
    /// A C# invocation expression (e.g. a.ToString(true))
    /// </summary>
    public sealed class CSharpInvocationExpression : CSharpExpression
    {
        #region Properties & Fields
        /// <summary>
        /// Gets the left-hand side expression of this invocation
        /// indicating the target.
        /// </summary>
        public CSharpExpression Member { get; private set; }

        /// <summary>
        /// Gets the arguments to be used in this invocation.
        /// </summary>
        public CSharpExpression[] Arguments { get; private set; }
        #endregion

        /// <summary>
        /// Creates the invocation expression from the syntax.
        /// </summary>
        /// <param name="syntax">the syntax to create from</param>
        internal CSharpInvocationExpression(InvocationExpressionSyntax syntax)
        {
            Syntax = syntax;
            Member = syntax.Expression.ToExpression();
            Arguments = syntax.ArgumentList.ToExpressions();
        }

        public override void ProcessMap(CSharpConversionMap map, StringBuilder result)
        {
            map.ProcessInvocationExpression(this, result);
        }

        internal override void CreateMemberSymbols(CSharpProject project, CSharpMemberSymbol parentSymbol)
        {
            Symbol = new CSharpMemberSymbol(":(", parentSymbol, this, false);
            Member?.CreateMemberSymbols(project, parentSymbol);
            foreach(CSharpExpression expression in Arguments)
            {
                expression?.CreateMemberSymbols(project, Symbol);
            }
        }

        internal override void ResolveMembers(CSharpProject project)
        {
            Member?.ResolveMembers(project);
            foreach (CSharpExpression expression in Arguments)
            {
                expression?.ResolveMembers(project);
            }
        }

        public override string ToString()
        {
            return $"{Member}({Arguments.ToReadableString()})";
        }
    }
}
