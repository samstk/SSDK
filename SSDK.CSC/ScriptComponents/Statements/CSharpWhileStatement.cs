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
    public sealed class CSharpWhileStatement : CSharpStatement
    {
        #region Properties & Fields
        /// <summary>
        /// Gets the expression that is evaluated to be true or false.
        /// </summary>
        public CSharpExpression Condition { get; private set; }

        /// <summary>
        /// Gets the execution block of this while statement.
        /// </summary>
        public CSharpStatementBlock Block { get; private set; }
        #endregion

        /// <summary>
        /// Creates the if statement from the given syntax.
        /// </summary>
        /// <param name="syntax">the syntax to create from</param>
        internal CSharpWhileStatement(WhileStatementSyntax syntax)
        {
            Condition = syntax.Condition.ToExpression();
            Block = new CSharpStatementBlock(syntax.Statement);
            Syntax = syntax;
        }

        public override void ProcessMap(CSharpConversionMap map, StringBuilder result)
        {
            map.ProcessWhileStatement(this, result);
        }

        public override string ToString()
        {
            return $"while ({Condition})";
        }
    }
}