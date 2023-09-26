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
    /// A C# instantiation expression (e.g. new Dict(true))
    /// </summary>
    public sealed class CSharpInstantiationExpression : CSharpExpression
    {
        #region Properties & Fields
        /// <summary>
        /// Gets the type of this expression
        /// indicating the class/struct target.
        /// </summary>
        public CSharpType Type { get; private set; }

        /// <summary>
        /// Gets the arguments to be used in this instantiation.
        /// </summary>
        public CSharpExpression[] Arguments { get; private set; }

        public CSharpExpression[] Initializer { get; private set; }
        #endregion

        /// <summary>
        /// Creates the instantiation expression from the syntax.
        /// </summary>
        /// <param name="syntax">the syntax to create from</param>
        internal CSharpInstantiationExpression(ObjectCreationExpressionSyntax syntax)
        {
            Syntax = syntax;
            Type = syntax.Type?.ToType();
            Arguments = syntax.ArgumentList?.ToExpressions();
            Initializer = CSharpExpression.Empty;
            if (syntax.Initializer != null)
                Initializer = syntax.Initializer.Expressions.ToExpressions();
        }

        public override void ProcessMap(CSharpConversionMap map, StringBuilder result)
        {
            map.ProcessInstantiationExpression(this, result);
        }

        public override string ToString()
        {
            return $"new {Type}({Arguments.ToReadableString()}) "+"{ "+Initializer.ToReadableString()+" }";
        }
    }
}
