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
    public sealed class CSharpForStatement : CSharpStatement
    {
        #region Properties & Fields
        /// <summary>
        /// Gets the initializers when this statement first begins.
        /// </summary>
        public CSharpExpression[] Initializers { get; private set; }

        /// <summary>
        /// Gets the expression that is evaluated to be true or false.
        /// </summary>
        public CSharpExpression Condition { get; private set; }
        
        /// <summary>
        /// Gets the expressions evaluated after every iteration for incrementation
        /// </summary>
        public CSharpExpression[] Incrementors { get; private set; }

        /// <summary>
        /// Gets the execution block of this while statement.
        /// </summary>
        public CSharpStatementBlock Block { get; private set; }
        #endregion

        /// <summary>
        /// Creates the if statement from the given syntax.
        /// </summary>
        /// <param name="syntax">the syntax to create from</param>
        internal CSharpForStatement(ForStatementSyntax syntax)
        {
            Incrementors = syntax.Incrementors.ToExpressions();
            Condition = syntax.Condition.ToExpression();
            Initializers = syntax.Initializers.ToExpressions();
            Block = new CSharpStatementBlock(syntax.Statement);
            Syntax = syntax;
        }

        public override void ProcessMap(CSharpConversionMap map, StringBuilder result)
        {
            map.ProcessForStatement(this, result);
        }

        internal override void CreateMemberSymbols(CSharpProject project, CSharpMemberSymbol parentSymbol)
        {
            Symbol = new CSharpMemberSymbol("for{", parentSymbol, this, false);
            foreach(CSharpExpression expression in Initializers)
            {
                expression.CreateMemberSymbols(project, Symbol);
            }
            Condition?.CreateMemberSymbols(project, Symbol);
            foreach (CSharpExpression expression in Incrementors)
            {
                expression.CreateMemberSymbols(project, Symbol);
            }
        }

        internal override void ResolveMembers(CSharpProject project)
        {
            foreach (CSharpExpression expression in Initializers)
            {
                expression.ResolveMembers(project);
            }
            Condition?.ResolveMembers(project);
            foreach (CSharpExpression expression in Incrementors)
            {
                expression.ResolveMembers(project);
            }
        }

        public override string ToString()
        {
            return $"for ({Initializers.ToReadableString()}; {Condition}; {Incrementors.ToReadableString()})";
        }
    }
}