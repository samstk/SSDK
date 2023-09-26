using SSDK.CSC.ScriptComponents;
using SSDK.CSC;
using System.Text;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SSDK.CSC.Helpers;

namespace SSDK.CSC.ScriptComponents.Statements {
    /// <summary>
    /// A c# switch statement for a given expression.
    /// </summary>
    public sealed class CSharpSwitchStatement : CSharpStatement
    {
        #region Properties & Fields
        /// <summary>
        /// Gets the expression that is evaluated.
        /// </summary>
        public CSharpExpression Expression { get; private set; }

        /// <summary>
        /// Gets the sections in this switch statement.
        /// </summary>
        public CSharpSwitchSection[] Sections { get; private set; }
        #endregion


        /// <summary>
        /// Creates the switch statement from the given syntax.
        /// </summary>
        /// <param name="syntax">the syntax to create from</param>
    internal CSharpSwitchStatement(SwitchStatementSyntax syntax)
        {
            Expression = syntax.Expression.ToExpression();
            Sections = new CSharpSwitchSection[syntax.Sections.Count];
            for(int i = 0; i < Sections.Length; i++)
            {
                Sections[i] = new CSharpSwitchSection(syntax.Sections[i]);
            }
            Syntax = syntax;
        }

        public override void ProcessMap(CSharpConversionMap map, StringBuilder result)
        {
            map.ProcessSwitchStatement(this, result);
        }

        public override string ToString()
        {
            return $"switch ({Expression.ToString()})";
        }
    }
}