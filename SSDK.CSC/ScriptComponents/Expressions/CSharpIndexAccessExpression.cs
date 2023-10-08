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
    /// A C# index access expression (e.g. a[3])
    /// </summary>
    public sealed class CSharpIndexAccessExpression : CSharpExpression
    {
        #region Properties & Fields
        /// <summary>
        /// Gets the left-hand side expression of this index access
        /// indicating the target.
        /// </summary>
        public CSharpExpression Member { get; private set; }

        /// <summary>
        /// Gets the arguments to be used in this index access.
        /// </summary>
        public CSharpExpression[] Arguments { get; private set; }
        #endregion

        /// <summary>
        /// Creates the IndexAccess expression from the syntax.
        /// </summary>
        /// <param name="syntax">the syntax to create from</param>
        internal CSharpIndexAccessExpression(ElementAccessExpressionSyntax syntax)
        {
            Syntax = syntax;
            Member = syntax.Expression.ToExpression();
            Arguments = syntax.ArgumentList.ToExpressions();
        }

        public override void ProcessMap(CSharpConversionMap map, StringBuilder result)
        {
            map.ProcessIndexAccessExpression(this, result);
        }

        internal override void CreateMemberSymbols(CSharpProject project, CSharpMemberSymbol parentSymbol)
        {
            Symbol = new CSharpMemberSymbol("[", parentSymbol, this, false);
            Member?.CreateMemberSymbols(project, parentSymbol);
            foreach(CSharpExpression expr in Arguments)
            {
                expr?.CreateMemberSymbols(project, parentSymbol);
            }
        }

        internal override void ResolveMembers(CSharpProject project)
        {
            Member?.ResolveMembers(project);
            foreach (CSharpExpression expr in Arguments)
            {
                expr?.ResolveMembers(project);
            }
        }

        public override string ToString()
        {
            return $"{Member}[{Arguments.ToReadableString()}]";
        }
    }
}
