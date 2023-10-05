using Microsoft.CodeAnalysis.CSharp.Syntax;
using SSDK.CSC.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSDK.CSC.ScriptComponents.Expressions
{
    /// <summary>
    /// A c# switch expression arm.
    /// </summary>
    public sealed class CSharpSwitchArm
    {
        #region Properties & Fields
        /// <summary>
        /// Gets the pattern for activating this arm.
        /// </summary>
        public CSharpPatternExpression Pattern { get; private set; }

        /// <summary>
        /// Gets the resulting expression from this arm
        /// </summary>
        public CSharpExpression Expression { get; private set; }

        /// <summary>
        /// Gets the syntax that formed this arm.
        /// </summary>
        public SwitchExpressionArmSyntax Syntax { get; private set; }
        #endregion


        /// <summary>
        /// Creates the switch statement from the given syntax.
        /// </summary>
        /// <param name="returnSyntax">the syntax to create from</param>
        internal CSharpSwitchArm(SwitchExpressionArmSyntax syntax)
        {
            Syntax = syntax;
            Expression = syntax.Expression.ToExpression();
            Pattern = new CSharpPatternExpression(syntax.Pattern);
        }

        public override string ToString()
        {
            return $"";
        }
    }
}
