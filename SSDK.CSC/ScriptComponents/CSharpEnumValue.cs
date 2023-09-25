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
    /// a c# enum value.
    /// </summary>
    public sealed class CSharpEnumValue : CSharpComponent
    {
        #region Properties & Fields
        /// <summary>
        /// Gets the name of the value.
        /// </summary>
        public string Name { get; private set; }
        
        /// <summary>
        /// Gets the expression of the enum value if applicable.
        /// </summary>
        public CSharpExpression Expression { get; private set; }

        /// <summary>
        /// Gets the attributes of the enum value.
        /// </summary>
        public CSharpAttribute[] Attributes { get; private set; }
        
        /// <summary>
        /// Gets the syntax that formed this enum.
        /// </summary>
        public EnumMemberDeclarationSyntax Syntax { get; private set; }
        #endregion

        internal CSharpEnumValue(EnumMemberDeclarationSyntax syntax)
        {
            Name = syntax.Identifier.ToString();

            Expression = syntax.EqualsValue?.Value.ToExpression();

            Attributes = syntax.AttributeLists.ToAttributes();

            Syntax = syntax;
        }
    }
}
