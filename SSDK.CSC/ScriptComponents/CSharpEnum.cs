using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSDK.CSC.ScriptComponents
{
    /// <summary>
    /// A C# class, which may contain methods, properties, fields, and sub-classes
    /// </summary>
    public sealed class CSharpEnum : CSharpComponent
    {
        #region Properties & Fields
        /// <summary>
        /// Gets the access modifier applied to this enum.
        /// </summary>
        public CSharpAccessModifier AccessModifier { get; private set; } = CSharpAccessModifier.Internal;

        /// <summary>
        /// Gets all attributes applied to this component.
        /// </summary>
        public CSharpAttribute[] Attributes { get; private set; }
        
        /// <summary>
        /// Gets the possible values of this enum.
        /// </summary>
        public CSharpEnumValue[] Values { get; private set; }
        #endregion
    }
}
