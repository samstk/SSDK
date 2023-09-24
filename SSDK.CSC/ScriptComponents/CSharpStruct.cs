using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSDK.CSC.ScriptComponents
{
    /// <summary>
    /// A C# struct, which may contain methods, properties, fields, and sub-classes
    /// </summary>
    public sealed class CSharpStruct
    {
        #region Properties & Fields
        /// <summary>
        /// Gets the access modifier applied to this struct.
        /// </summary>
        public CSharpAccessModifier AccessModifier { get; private set; } = CSharpAccessModifier.Internal;

        /// <summary>
        /// Gets all attributes applied to this component.
        /// </summary>
        public CSharpAttribute[] Attributes { get; private set; }

        /// <summary>
        /// Gets all sub-classes of this struct.
        /// </summary>
        public CSharpClass[] Subclasses { get; private set; }

        /// <summary>
        /// Gets all sub-structs of this struct.
        /// </summary>
        public CSharpStruct[] Substructs { get; private set; }

        /// <summary>
        /// Gets all delegate declarations in this struct.
        /// </summary>
        public CSharpDelegate[] Delegates { get; private set; }

        /// <summary>
        /// Gets all static methods of this struct.
        /// </summary>
        public CSharpMethod[] StaticMethods { get; private set; }

        /// <summary>
        /// Gets all instance methods of this struct.
        /// </summary>
        public CSharpMethod[] InstanceMethods { get; private set; }

        /// <summary>
        /// Gets the instance constructors of this struct.
        /// </summary>
        public CSharpMethod[] InstanceConstructors { get; private set; }

        /// <summary>
        /// Gets the instance destructor of this struct.
        /// </summary>
        public CSharpMethod InstanceDestructor { get; private set; }

        /// <summary>
        /// Gets the static constructor of this struct.
        /// </summary>
        public CSharpMethod StaticConstructor { get; private set; }

        /// <summary>
        /// Gets the static destructor of this class.
        /// </summary>
        public CSharpMethod StaticDestructor { get; private set; }

        /// <summary>
        /// Gets all static properties of this class
        /// </summary>
        public CSharpProperty[] StaticProperties { get; private set; }

        /// <summary>
        /// Gets all instance properties of this class
        /// </summary>
        public CSharpProperty[] InstanceProperties { get; private set; }

        /// <summary>
        /// Gets all static fields of this class
        /// </summary>
        public CSharpVariable[] StaticFields { get; private set; }

        /// <summary>
        /// Gets all instance fields of this class
        /// </summary>
        public CSharpVariable[] InstanceFields { get; private set; }

        /// <summary>
        /// Gets all operator overloads of this struct.
        /// </summary>
        public CSharpMethod[] Operators { get; private set; }
        #endregion
    }
}
