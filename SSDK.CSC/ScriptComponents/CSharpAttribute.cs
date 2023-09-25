using Microsoft.CodeAnalysis.CSharp.Syntax;
using SSDK.CSC.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSDK.CSC.ScriptComponents
{
    /// <summary>
    /// A C# attribute, which may be applied to a component.
    /// </summary>
    public sealed class CSharpAttribute
    {
        #region Properties & Fields
        /// <summary>
        /// Gets the used name of the attribute
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the list of expressions depicts the parameters
        /// of this attribute.
        /// </summary>
        public CSharpExpression[] Parameters { get; private set; }
        #endregion

        /// <summary>
        /// Creates an c# attribute from the given syntax
        /// </summary>
        /// <param name="syntax">the syntax to create from</param>
        internal CSharpAttribute(AttributeSyntax syntax)
        {
            Name = syntax.Name.ToString();

            Parameters = syntax.ArgumentList.ToExpressions();
        }

        public override string ToString()
        {
            return $"[{Name}(...)]";
        }
    }
}
