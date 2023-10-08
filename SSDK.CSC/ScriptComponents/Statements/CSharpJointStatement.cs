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
    /// A c# joint statement, which contains a number of statements in order (e.g. multiple variable declarations)
    /// No syntax is stored for this type of statement.
    /// </summary>
    public sealed class CSharpJointStatement : CSharpStatement
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
        internal CSharpJointStatement(CSharpStatement[] statements, StatementSyntax syntax)
        {
            Syntax = syntax;
            Statements = statements;
            for(int i = 0; i< Statements.Length; i++)
            {
                if (Statements[i] is CSharpVariable)
                {
                    ((CSharpVariable)Statements[i]).InStatement = true;
                }
            }
        }

        internal CSharpJointStatement() { }

        public override void ProcessMap(CSharpConversionMap map, StringBuilder result)
        {
            map.ProcessJointStatement(this, result);
        }

        internal override void CreateMemberSymbols(CSharpProject project, CSharpMemberSymbol parentSymbol)
        {
            Symbol = new CSharpMemberSymbol("(<st>,<st>,..)", parentSymbol, this, false);
            foreach(CSharpStatement statement in Statements)
            {
                statement.CreateMemberSymbols(project, Symbol);
            }
        }

        internal override void ResolveMembers(CSharpProject project)
        {
            foreach (CSharpStatement statement in Statements)
            {
                statement.ResolveMembers(project);
            }
        }

        public override string ToString()
        {
            string result = "";

            foreach(CSharpStatement statement in Statements)
            {
                result += statement.ToString() + " ";
            }

            return result;
        }
    }
}
