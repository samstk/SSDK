using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSDK.CSC.ScriptComponents
{
    /// <summary>
    /// A c# property, which has get and set permissions that may differ each property.
    /// </summary>
    public class CSharpProperty
    {
        #region Properties & Fields
        
        /// <summary>
        /// Gets the get access of this property.
        /// </summary>
        public CSharpAccessModifier GetAccess { get; private set; } = CSharpAccessModifier.Internal;

        /// <summary>
        /// Gets the statement block for getting this property.
        /// </summary>
        public CSharpStatementBlock Get { get; private set; }

        /// <summary>
        /// Gets the statement block for setting this property.
        /// </summary>
        public CSharpStatementBlock Set { get; private set; }
        
        /// <summary>
        /// Gets the set access of this property.
        /// </summary>
        public CSharpAccessModifier SetAccess { get; private set; } = CSharpAccessModifier.Internal;
        #endregion
    }
}
