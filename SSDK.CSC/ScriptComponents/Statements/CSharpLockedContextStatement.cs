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
    /// A c# block for locked variables (only allows one thread to execute the block if it has acquired
    /// the locked of a variable).
    /// </summary>
    public sealed class CSharpLockedContextStatement : CSharpStatement
    {
        #region Properties & Fields
        /// <summary>
        /// Gets the statement block that is executed in this locked context.
        /// </summary>
        public CSharpStatementBlock Block { get; private set; }

        /// <summary>
        /// Gets the target of the lock.
        /// </summary>
        public CSharpExpression LockTarget { get; private set; }
        #endregion

        /// <summary>
        /// Creates a fixed block from the given syntax
        /// </summary>
        /// <param name="syntax">the syntax</param>
        internal CSharpLockedContextStatement(LockStatementSyntax syntax)
        {
            Syntax = syntax;
            Block = new CSharpStatementBlock(syntax.Statement);
            LockTarget = syntax.Expression.ToExpression();
        }


        public override void ProcessMap(CSharpConversionMap map, StringBuilder result)
        {
            map.ProcessLockedContextStatement(this, result);
        }

        public override string ToString()
        {
            return $"locked ({LockTarget}) ...";
        }
    }
}
