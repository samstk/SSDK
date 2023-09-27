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
    /// A c# continue statement.
    /// </summary>
    public sealed class CSharpContinueStatement : CSharpStatement
    {
        /// <summary>
        /// Creates the continue statement from the given syntax.
        /// </summary>
        /// <param name="syntax">the syntax to create from</param>
        internal CSharpContinueStatement(ContinueStatementSyntax syntax)
        {
            Syntax = syntax;
        }

        public override void ProcessMap(CSharpConversionMap map, StringBuilder result)
        {
            map.ProcessContinueStatement(this, result);
        }

        public override string ToString()
        {
            return $"continue;";
        }
    }
}
