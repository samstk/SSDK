using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSDK.CSC.ScriptComponents
{
    /// <summary>
    /// A C# expression, which may depict activation of a particular function.
    /// </summary>
    public abstract class CSharpExpression
    {
        #region Properties & Fields
        /// <summary>
        /// Gets the expression syntax that formed this c# expression
        /// </summary>
        public ExpressionSyntax Syntax { get; protected set; }
        #endregion
    }
}
