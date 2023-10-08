using SSDK.CSC.ScriptComponents;
using SSDK.CSC;
using System.Text;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SSDK.CSC.Helpers;

namespace SSDK.CSC.ScriptComponents.Statements
{
    /// <summary>
    /// A c# catch for a try statement.
    /// </summary>
    public sealed class CSharpTryCatch : CSharpComponent
    {
        #region Properties & Fields
        /// <summary>
        /// Gets the symbol that represents this catch.
        /// </summary>
        /// <remarks>
        /// ResolveMembers must be called on the project to be set.
        /// </remarks>
        public CSharpMemberSymbol Symbol { get; internal set; }
        /// <summary>
        /// Gets the execution block of this try statement.
        /// </summary>
        public CSharpStatementBlock Block { get; private set; }

        /// <summary>
        /// Gets the expression responsible for filtering the exceptions.
        /// </summary>
        public CSharpExpression Filter { get; private set; }

        /// <summary>
        /// Gets the variable declaration (e.g. Exception exc)
        /// </summary>
        public CSharpVariable Variable { get; private set; }

        /// <summary>
        /// Gets the syntax that formed this catch
        /// </summary>
        public CatchClauseSyntax Syntax { get; private set; }
        #endregion

        /// <summary>
        /// Creates the catch from the given syntax.
        /// </summary>
        /// <param name="syntax">the syntax to create from</param>
        internal CSharpTryCatch(CatchClauseSyntax syntax)
        {
            if(syntax.Filter != null)
            {
                Filter = syntax.Filter.FilterExpression.ToExpression();
            }
            if(syntax.Declaration != null)
            {
                Variable = new CSharpVariable(
                    syntax.Declaration.Identifier.ToString(),
                    syntax.Declaration.Type.ToType(),
                    new CSharpAttribute[0],
                    CSharpGeneralModifier.None, CSharpAccessModifier.DefaultOrNone,
                    null);
            }
            Block = new CSharpStatementBlock(syntax.Block);
            Syntax = syntax;
        }

        /// <summary>
        /// Creates a member symbol for this component
        /// </summary>
        internal override void CreateMemberSymbols(CSharpProject project, CSharpMemberSymbol parentSymbol)
        {
            Symbol = new CSharpMemberSymbol("catch{", parentSymbol, this, false);
            Variable?.CreateMemberSymbols(project, Symbol);
            Block?.CreateMemberSymbols(project, Symbol);
        }

        internal override void ResolveMembers(CSharpProject project)
        {
            Variable?.ResolveMembers(project);
            Block?.ResolveMembers(project);
        }

        public override string ToString()
        {
            return $"catch ({(Filter != null ? Filter : "")}) ...";
        }
    }
}