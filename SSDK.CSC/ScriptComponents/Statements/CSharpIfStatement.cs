using SSDK.CSC.ScriptComponents;
using SSDK.CSC;
using System.Text;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SSDK.CSC.Helpers;

namespace SSDK.CSC.ScriptComponents.Statements
{
    /// <summary>
    /// A c# if statement for a given expression.
    /// </summary>
    public sealed class CSharpIfStatement : CSharpStatement
    {
        #region Properties & Fields
        /// <summary>
        /// Gets the expression that is evaluated to be true or false.
        /// </summary>
        public CSharpExpression Condition { get; private set; }

        /// <summary>
        /// Gets the execution block of this case.
        /// </summary>
        public CSharpStatementBlock Block { get; private set; } 

        /// <summary>
        /// Gets the else if statement
        /// </summary>
        public CSharpIfStatement ElseIf { get; private set; }

        /// <summary>
        /// Gets the else statement (if no else if statements occurred)
        /// </summary>
        public CSharpStatementBlock Else { get; private set; }
        #endregion

        /// <summary>
        /// Creates the if statement from the given syntax.
        /// </summary>
        /// <param name="syntax">the syntax to create from</param>
        internal CSharpIfStatement(IfStatementSyntax syntax)
        {
            Condition = syntax.Condition.ToExpression();
            Block = new CSharpStatementBlock(syntax.Statement);
            Syntax = syntax;

            if(syntax.Else != null)
            {
                if (syntax.Else.Statement is IfStatementSyntax)
                {
                    ElseIf = new CSharpIfStatement(syntax.Else.Statement as IfStatementSyntax);
                }
                else Else = new CSharpStatementBlock(syntax.Else.Statement);
            }
        }

        public override void ProcessMap(CSharpConversionMap map, StringBuilder result)
        {
            map.ProcessIfStatement(this, result);
        }

        internal override void CreateMemberSymbols(CSharpProject project, CSharpMemberSymbol parentSymbol)
        {
            Symbol = new CSharpMemberSymbol("if{", parentSymbol, this, false);
            Block?.CreateMemberSymbols(project, Symbol);
        }

        internal override void ResolveMembers(CSharpProject project)
        {
            Block?.ResolveMembers(project);
        }

        public override string ToString()
        {
            return $"if ({Condition.ToString()})";
        }
    }
}