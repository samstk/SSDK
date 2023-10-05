using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSDK.CSC.ScriptComponents
{
    /// <summary>
    /// Depicts a general modifier flags onto a class, variable, etc..
    /// </summary>
    [Flags]
    public enum CSharpGeneralModifier
    {
        None = 0,
        /// <summary>
        /// Indicates that the corresponding component has a missing or incomplete implementation 
        /// and cannot be used until it is overriden in an inherited class.
        /// </summary>
        Abstract = 1,
        /// <summary>
        /// Indicates that a component is asynchronous.
        /// </summary>
        Async = 2,
        /// <summary>
        /// Indicates the the component must not be modified.
        /// </summary>
        Const = 4,
        /// <summary>
        /// Indicates that the component indicates an event that can be
        /// invoked.
        /// </summary>
        Event = 8,
        /// <summary>
        /// Indicates that the component is implemented externally.
        /// </summary>
        Extern = 16,
        /// <summary>
        /// Indicates that the component is contravariant.
        /// See 'in' modifier on microsoft's website
        /// </summary>
        In = 32,
        /// <summary>
        /// Indicates that the component explicity hides a member that is inherited.
        /// </summary>
        New = 64,
        /// <summary>
        /// Indicates that the compeonent is covariant.
        /// See 'out' modifier on microsoft's website.
        /// </summary>
        Out = 128,
        /// <summary>
        /// Indicates that the component overrides an inherited member.
        /// </summary>
        Override = 256,
        /// <summary>
        /// Indicates that the component is readonly (can only be modified during declaration
        /// and constructor)
        /// </summary>
        Readonly = 512,
        /// <summary>
        /// Indicates that the component cannot be inherited.
        /// </summary>
        Sealed = 1024,
        /// <summary>
        /// Indicates that the component belongs to the type itself, rather
        /// than an instance of an object.
        /// </summary>
        Static = 2048,
        /// <summary>
        /// Indicates that the component is part of an unsafe context, which
        /// is required for any operation involving pointers.
        /// </summary>
        Unsafe = 4096,
        /// <summary>
        /// Indicates that the component can be overriden in a derived class.
        /// </summary>
        Virtual = 8192,
        /// <summary>
        /// Indicates that a field might be modified by multiple threads that are executing
        /// at the same time.
        /// </summary>
        Volatile = 16384,
        /// <summary>
        /// Indicates that the variable is a reference only
        /// </summary>
        Ref = 32768,
        /// <summary>
        /// Indicates that the variable (inside method parameters) represents all
        /// parameters after it in an array.
        /// </summary>
        Params = 65536
    }
}
