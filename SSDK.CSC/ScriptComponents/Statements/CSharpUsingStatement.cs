using SSDK.CSC.ScriptComponents;
using SSDK.CSC;
using System.Text;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SSDK.CSC.Helpers;

namespace SSDK.CSC.ScriptComponents.Statements
{
    /// <summary>
    /// A c# using statement for a given expression.
    /// </summary>
    public sealed class CSharpUsingStatement : CSharpStatement
    {
        #region Properties & Fields
        /// <summary>
        /// Gets the expression that is evaluated.
        /// If not set, then this using statement has a 
        /// declaration instead.
        /// </summary>
        public CSharpExpression Expression { get; private set; }

        /// <summary>
        /// Gets the execution block of this while statement.
        /// </summary>
        public CSharpStatementBlock Block { get; private set; }

        /// <summary>
        /// Gets the variables declared for this using context.
        /// </summary>
        public CSharpVariable[] Variables { get; private set; }
        #endregion

        /// <summary>
        /// Creates the using statement from the given syntax.
        /// </summary>
        /// <param name="syntax">the syntax to create from</param>
        internal CSharpUsingStatement(UsingStatementSyntax syntax)
        {
            Expression = syntax.Expression?.ToExpression();
            Variables = syntax.Declaration?.ToVariables();
            Block = new CSharpStatementBlock(syntax.Statement);
            Syntax = syntax;
        }

        public override void ProcessMap(CSharpConversionMap map, StringBuilder result)
        {
            map.ProcessUsingStatement(this, result);
        }

        internal override void CreateMemberSymbols(CSharpProject project, CSharpMemberSymbol parentSymbol)
        {
            Symbol = new CSharpMemberSymbol("using{", parentSymbol, this, false);
            foreach(CSharpVariable variable in Variables)
            {
                variable.CreateMemberSymbols(project, Symbol);
            }
            Expression?.CreateMemberSymbols(project, Symbol);
            Block?.CreateMemberSymbols(project, Symbol);
        }

        internal override void ResolveMembers(CSharpProject project)
        {
            foreach (CSharpVariable variable in Variables)
            {
                variable.ResolveMembers(project);
            }
            Expression?.ResolveMembers(project);
            Block?.ResolveMembers(project);
        }

        public override string ToString()
        {
            return $"using ({Variables.ToReadableString()}) ...";
        }
    }
}