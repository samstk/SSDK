using SSDK.CSC.ScriptComponents;
using SSDK.CSC;
using System.Text;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SSDK.CSC.Helpers;

namespace SSDK.CSC.ScriptComponents.Expressions
{
    /// <summary>
    /// A c# switch expression for a given expression.
    /// </summary>
    public sealed class CSharpSwitchExpression : CSharpExpression
    {
        #region Properties & Fields
        /// <summary>
        /// Gets the expression that is evaluated.
        /// </summary>
        public CSharpExpression Expression { get; private set; }

        /// <summary>
        /// Gets the arms in this switch expression.
        /// </summary>
        public CSharpSwitchArm[] Arms { get; private set; }
        #endregion


        /// <summary>
        /// Creates the switch expression from the given syntax.
        /// </summary>
        /// <param name="syntax">the syntax to create from</param>
        internal CSharpSwitchExpression(SwitchExpressionSyntax syntax)
        {
            Expression = syntax.GoverningExpression.ToExpression();
            Arms = new CSharpSwitchArm[syntax.Arms.Count];
            for (int i = 0; i < Arms.Length; i++)
            {
                Arms[i] = new CSharpSwitchArm(syntax.Arms[i]);
            }
            Syntax = syntax;
        }

        public override void ProcessMap(CSharpConversionMap map, StringBuilder result)
        {
            map.ProcessSwitchExpression(this, result);
        }

        public override string ToString()
        {
            return $"{Expression} switch ...";
        }
    }
}