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
    /// A c# variable declaration, which is also an expression.
    /// </summary>
    public sealed class CSharpVariable : CSharpStatement
    {
        #region Properties & Fields
        /// <summary>
        /// Gets the type of the variable
        /// </summary>
        public CSharpType Type { get; private set; }

        /// <summary>
        /// Gets the name of the variable
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the attributes of the variable
        /// </summary>
        public CSharpAttribute[] Attributes { get; private set; }

        /// <summary>
        /// Gets the expression when this variable/parameter was first declared
        /// (default expression)
        /// </summary>
        public CSharpExpression Expression { get; private set; }
        #endregion
    
        internal CSharpVariable(ParameterSyntax paramSyntax)
        {
            Name = paramSyntax.Identifier.ToString();
            Type = paramSyntax.Type.ToType();
            Attributes = paramSyntax.AttributeLists.ToAttributes();
            if (paramSyntax.Default != null)
            {
                Expression = paramSyntax.Default.ToExpression();
            }
        }

        public override string ToString()
        {
            return $"{Attributes.ToReadablePrefix()}{Type} {Name}{(Expression != null ? $" = {Expression}" : "")}";
        }
    }
}
