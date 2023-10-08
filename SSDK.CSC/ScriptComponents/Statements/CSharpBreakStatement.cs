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
    /// A c# break statement.
    /// </summary>
    public sealed class CSharpBreakStatement : CSharpStatement
    {
        /// <summary>
        /// Creates the break statement from the given syntax.
        /// </summary>
        /// <param name="syntax">the syntax to create from</param>
        internal CSharpBreakStatement(BreakStatementSyntax syntax)
        {
            Syntax = syntax;
        }

        public override void ProcessMap(CSharpConversionMap map, StringBuilder result)
        {
            map.ProcessBreakStatement(this, result);
        }

        public override string ToString()
        {
            return $"break;";
        }

        internal override void CreateMemberSymbols(CSharpProject project, CSharpMemberSymbol parentSymbol)
        {
            Symbol = new CSharpMemberSymbol("break", parentSymbol, this, false);
        }

        internal override void ResolveMembers(CSharpProject project)
        {

        }
    }
}
