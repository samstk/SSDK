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
        /// Gets the base class defining this attribute.
        /// </summary>
        public CSharpClass BaseClass { get; private set; }

        /// <summary>
        /// Gets the list of expressions depicts the parameters
        /// of this attribute.
        /// </summary>
        public CSharpExpression[] Parameters { get; private set; }
        #endregion
    }
}
