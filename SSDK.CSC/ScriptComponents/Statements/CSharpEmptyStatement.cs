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
    /// A c# empty statement (;)
    /// </summary>
    public sealed class CSharpEmptyStatement : CSharpStatement
    {
        /// <summary>
        /// Creates the empty statement from the given syntax.
        /// </summary>
        /// <param name="syntax">the syntax to create from</param>
        internal CSharpEmptyStatement(StatementSyntax syntax)
        {
            Syntax = syntax;
        }

        public override void ProcessMap(CSharpConversionMap map, StringBuilder result)
        {
            map.ProcessEmptyStatement(this, result);
        }

        public override string ToString()
        {
            return $";";
        }
    }
}
