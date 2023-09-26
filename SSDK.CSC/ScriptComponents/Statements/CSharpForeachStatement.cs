using SSDK.CSC.ScriptComponents;
using SSDK.CSC;
using System.Text;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SSDK.CSC.Helpers;
using Microsoft.CodeAnalysis.CSharp;

namespace SSDK.CSC.ScriptComponents.Statements
{
    /// <summary>
    /// A c# if statement for a given expression.
    /// </summary>
    public sealed class CSharpForeachStatement : CSharpStatement
    {
        #region Properties & Fields
        /// <summary>
        /// Gets the name of the variable in this for each statement.
        /// </summary>
        public string Name { get; private set; }
        /// <summary>
        /// Gets the execution block of this while statement.
        /// </summary>
        public CSharpStatementBlock Block { get; private set; }

        /// <summary>
        /// Gets the target expression to enumerate
        /// </summary>
        public CSharpExpression Target { get; private set; }

        /// <summary>
        /// Gets the type of the enumeration.
        /// </summary>
        public CSharpType Type { get; private set; }

        /// <summary>
        /// Gets whether the await keyword was applied before this statement.
        /// </summary>
        public bool IsAwaited { get; private set; }
        #endregion

        /// <summary>
        /// Creates the if statement from the given syntax.
        /// </summary>
        /// <param name="syntax">the syntax to create from</param>
        internal CSharpForeachStatement(ForEachStatementSyntax syntax)
        {
            IsAwaited = syntax.RawKind == (int)SyntaxKind.AwaitKeyword;
            Target = syntax.Expression.ToExpression();
            Type = syntax.Type.ToType();
            Block = new CSharpStatementBlock(syntax.Statement);
            Syntax = syntax;
        }

        public override void ProcessMap(CSharpConversionMap map, StringBuilder result)
        {
            map.ProcessForeachStatement(this, result);
        }

        public override string ToString()
        {
            return $"foreach ()";
        }
    }
}