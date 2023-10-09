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
    /// A C# member access expression (e.g. x.ToString)
    /// </summary>
    public sealed class CSharpMemberAccessExpression : CSharpExpression
    {
        #region Properties & Fields
        /// <summary>
        /// Gets the target expression (parent of target member)
        /// </summary>
        public CSharpExpression Target { get; private set; }

        /// <summary>
        /// Gets the member to be accessed.
        /// </summary>
        public string Member { get; private set; }

        /// <summary>
        /// Gets the referenced member symbol
        /// </summary>
        public CSharpMemberSymbol ReferencedMemberSymbol { get; private set; }
        #endregion

        /// <summary>
        /// Creates the member access expression from the syntax.
        /// </summary>
        /// <param name="syntax">the syntax to create from</param>
        internal CSharpMemberAccessExpression(MemberAccessExpressionSyntax syntax)
        {
            Syntax = syntax;
            Member = syntax.Name.Identifier.ValueText;
            Target = syntax.Expression.ToExpression();
        }

        public override void ProcessMap(CSharpConversionMap map, StringBuilder result)
        {
            map.ProcessMemberAccessExpression(this, result);
        }
        internal override void CreateMemberSymbols(CSharpProject project, CSharpMemberSymbol parentSymbol)
        {
            Symbol = new CSharpMemberSymbol("(access)", parentSymbol, this, false);
            Target?.CreateMemberSymbols(project, Symbol);
        }

        internal override void ResolveMembers(CSharpProject project)
        {
            Target?.ResolveMembers(project);
            CSharpType type = Target?.Symbol.GetComponentType(project);
            ReferencedMemberSymbol = type?.ReferencedSymbol.FindBestMatchingSymbol(
                new string[] {Member}, 0, 2
                );
        }

        public override string ToString()
        {
            return $"{Target}.{Member}";
        }

        internal override CSharpType GetComponentType(CSharpProject project)
        {
            return ReferencedMemberSymbol.GetComponentType(project);
        }
    }
}
