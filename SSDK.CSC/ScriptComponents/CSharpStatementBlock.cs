using Microsoft.CodeAnalysis;
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
    /// A c# statement block, which contains a number of statements.
    /// </summary>
    public sealed class CSharpStatementBlock : CSharpStatement
    {
        #region Properties & Fields
        /// <summary>
        /// Gets the statements made in the block in sequence.
        /// </summary>
        public CSharpStatement[] Statements { get; private set; }
        #endregion

        /// <summary>
        /// Creates a new statement block from the given block syntax
        /// </summary>
        /// <param name="syntax">the block syntax</param>
        internal CSharpStatementBlock(BlockSyntax syntax)
        {
            Syntax = syntax;
            Statements = new CSharpStatement[syntax.Statements.Count];

            for(int i = 0; i<syntax.Statements.Count; i++)
            {
                Statements[i] = syntax.Statements[i].ToStatement();
                if (Statements[i] is CSharpVariable)
                {
                    ((CSharpVariable)Statements[i]).InStatement = true;
                }
            }
        }

        /// <summary>
        /// Creates a new statement block from the given block syntax
        /// </summary>
        /// <param name="syntax">the block syntax</param>
        internal CSharpStatementBlock(StatementSyntax singleStatementSyntax)
        {
            Syntax = singleStatementSyntax;

            if (singleStatementSyntax is BlockSyntax)
            {
                BlockSyntax syntax = singleStatementSyntax as BlockSyntax;
                Statements = new CSharpStatement[syntax.Statements.Count];

                for (int i = 0; i < syntax.Statements.Count; i++)
                {
                    Statements[i] = syntax.Statements[i].ToStatement();
                    if (Statements[i] is CSharpVariable)
                    {
                        ((CSharpVariable)Statements[i]).InStatement = true;
                    }
                }
            }
            else Statements = new CSharpStatement[] { singleStatementSyntax.ToStatement() };
        }

        internal CSharpStatementBlock(SyntaxList<StatementSyntax> syntaxList, StatementSyntax parent)
        {
            Syntax = parent;
            Statements = new CSharpStatement[syntaxList.Count];

            for (int i = 0; i < syntaxList.Count; i++)
            {
                Statements[i] = syntaxList[i].ToStatement();
                if (Statements[i] is CSharpVariable)
                {
                    ((CSharpVariable)Statements[i]).InStatement = true;
                }
            }
        }

        internal CSharpStatementBlock() { }

        /// <summary>
        /// Creates a statement block with a single return statement for the given expression.
        /// </summary>
        /// <param name="syntax">the syntax containing the expression</param>
        /// <returns>the statement block with the given expression</returns>
        internal static CSharpStatementBlock WithReturn(ExpressionSyntax syntax)
        {
            return new CSharpStatementBlock()
            {
                Statements = new CSharpStatement[] { new CSharpReturnStatement(syntax, null) }
            };
        }

        public override void ProcessMap(CSharpConversionMap map, StringBuilder result)
        {
            map.ProcessStatementBlock(this, result);
        }
    }
}
