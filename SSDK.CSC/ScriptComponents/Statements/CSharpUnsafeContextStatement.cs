using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SSDK.CSC.Helpers;
using SSDK.CSC.ScriptComponents.Statements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSDK.CSC.ScriptComponents
{
    /// <summary>
    /// A c# block for an unsafe context with pointers.
    /// </summary>
    public sealed class CSharpUnsafeContextStatement : CSharpStatement
    {
        #region Properties & Fields
        /// <summary>
        /// Gets the block context.
        /// </summary>
        public CSharpStatementBlock Block { get; private set; }

        #endregion

        /// <summary>
        /// Creates a new unsafe block from the given syntax
        /// </summary>
        /// <param name="syntax">the syntax</param>
        internal CSharpUnsafeContextStatement(UnsafeStatementSyntax syntax)
        {
            Syntax = syntax;
            Block = new CSharpStatementBlock(syntax.Block);
        }


        public override void ProcessMap(CSharpConversionMap map, StringBuilder result)
        {
            map.ProcessUnsafeContextStatement(this, result);
        }

        public override string ToString()
        {
            return "unsafe ...";
        }
    }
}
