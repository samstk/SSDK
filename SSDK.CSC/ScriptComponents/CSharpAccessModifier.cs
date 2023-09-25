using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSDK.CSC.ScriptComponents
{
    /// <summary>
    /// Represents an access modifier applied to a corresponding component.
    /// </summary>
    public enum CSharpAccessModifier
    {
        /// <summary>
        /// The default c# access modifier, which is internal.
        /// </summary>
        DefaultOrNone,
        /// <summary>
        /// An access modifier applied to a corresponding component. <br/>
        /// has the following access: <br/>
        /// - within the same class <br/>
        /// - within a derived class (same assembly) <br/>
        /// - within a non-derived class (same assembly)
        /// </summary>
        Internal,
        /// <summary>
        /// An access modifier applied to a corresponding component. <br/>
        /// has the following access: <br/>
        /// - within the same class <br/>
        /// - within a derived class (same assembly) <br/>
        /// - within a non-derived class (same assembly) <br/>
        /// - within a derived class (different assembly) <br/>
        /// - within a non-derived class (different assembly) <br/>
        /// </summary>
        Public,
        /// <summary>
        /// An access modifier applied to a corresponding component. <br/>
        /// has the following access: <br/>
        /// - within the same class <br/>
        /// - within a derived class (same assembly) <br/>
        /// - within a derived class (different assembly) <br/>
        /// </summary>
        Protected,
        /// <summary>
        /// An access modifier applied to a corresponding component. <br/>
        /// has the following access: <br/>
        /// - within the same class <br/>
        /// </summary>
        Private,
        /// <summary>
        /// An access modifier applied to a corresponding component. <br/>
        /// has the following access: <br/>
        /// - within the same class <br/>
        /// - within a derived class (same assembly) <br/>
        /// </summary>
        PrivateProtected,
        /// <summary>
        /// An access modifier applied to a corresponding component. <br/>
        /// has the following access: <br/>
        /// - within the same class <br/>
        /// - within a derived class (same assembly) <br/>
        /// - within a non-derived class (same assembly) <br/>
        /// - within a derived class (different assembly)
        /// </summary>
        ProtectedInternal
    }
}
