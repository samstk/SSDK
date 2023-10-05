using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SSDK.CSC.Exceptions;
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
    /// A c# block for fixed variables (does not allow garbage collector to relocate a moveable variable).
    /// </summary>
    public sealed class CSharpFixedContextStatement : CSharpStatement
    {
        #region Properties & Fields
        /// <summary>
        /// Gets the statement that is executed in this fixed context.
        /// </summary>
        public CSharpStatementBlock Block { get; private set; }

        /// <summary>
        /// Gets the variables declared for this fixed context.
        /// </summary>
        public CSharpVariable[] Variables { get; private set; }
        #endregion

        /// <summary>
        /// Creates a fixed block from the given syntax
        /// </summary>
        /// <param name="syntax">the syntax</param>
        internal CSharpFixedContextStatement(FixedStatementSyntax syntax)
        {
            Syntax = syntax;
            Block = new CSharpStatementBlock(syntax.Statement);
            Variables = syntax.Declaration.ToVariables();
            if(Variables.Length != 1 && Variables.Any((v) => v.Type != Variables[0].Type))
            {
                throw new SyntaxOrSemanticException(Syntax);
            }
        }


        public override void ProcessMap(CSharpConversionMap map, StringBuilder result)
        {
            map.ProcessFixedContextStatement(this, result);
        }

        public override string ToString()
        {
            return $"fixed ({Variables.ToReadableString()}) ...";
        }
    }
}
