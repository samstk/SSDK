using Microsoft.CodeAnalysis;
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
    /// A C# class, which may contain methods, properties, fields, and sub-classes.
    /// </summary>
    public sealed class CSharpClass : CSharpComponent
    {
        
        #region Properties & Fields
        /// <summary>
        /// Gets the name of the class
        /// </summary>
        public string Name { get; private set; }
        /// <summary>
        /// Gets the access modifier applied to this class.
        /// </summary>
        public CSharpAccessModifier AccessModifier { get; private set; } = CSharpAccessModifier.Internal;

        /// <summary>
        /// Gets the general modifier of this variable
        /// </summary>
        public CSharpGeneralModifier GeneralModifier { get; private set; } = CSharpGeneralModifier.None;

        #region General Modifier Properties
        public bool IsAbstract { get { return GeneralModifier.HasFlag(CSharpGeneralModifier.Abstract); } }
        public bool IsAsync { get { return GeneralModifier.HasFlag(CSharpGeneralModifier.Async); } }
        public bool IsConst { get { return GeneralModifier.HasFlag(CSharpGeneralModifier.Const); } }
        public bool IsEvent { get { return GeneralModifier.HasFlag(CSharpGeneralModifier.Event); } }
        public bool IsExtern { get { return GeneralModifier.HasFlag(CSharpGeneralModifier.Extern); } }
        public bool IsIn { get { return GeneralModifier.HasFlag(CSharpGeneralModifier.In); } }
        public bool IsNew { get { return GeneralModifier.HasFlag(CSharpGeneralModifier.New); } }
        public bool IsOut { get { return GeneralModifier.HasFlag(CSharpGeneralModifier.Out); } }
        public bool IsOverride { get { return GeneralModifier.HasFlag(CSharpGeneralModifier.Override); } }
        public bool IsReadonly { get { return GeneralModifier.HasFlag(CSharpGeneralModifier.Readonly); } }
        public bool IsSealed { get { return GeneralModifier.HasFlag(CSharpGeneralModifier.Sealed); } }
        public bool IsStatic { get { return GeneralModifier.HasFlag(CSharpGeneralModifier.Static); } }
        public bool IsUnsafe { get { return GeneralModifier.HasFlag(CSharpGeneralModifier.Unsafe); } }
        public bool IsVirtual { get { return GeneralModifier.HasFlag(CSharpGeneralModifier.Virtual); } }
        public bool IsVolatile { get { return GeneralModifier.HasFlag(CSharpGeneralModifier.Volatile); } }
        #endregion

        /// <summary>
        /// Gets all attributes applied to this component.
        /// </summary>
        public CSharpAttribute[] Attributes { get; private set; }

        /// <summary>
        /// Gets all types inherited by this class.
        /// </summary>
        public CSharpType[] Inherits { get; private set; }

        /// <summary>
        /// Gets all sub-classes of this class.
        /// </summary>
        public CSharpClass[] Subclasses { get; private set; }

        /// <summary>
        /// Gets all sub-structs of this class.
        /// </summary>
        public CSharpStruct[] Substructs { get; private set; }

        /// <summary>
        /// Gets all delegate declarations in this class.
        /// </summary>
        public CSharpDelegate[] Delegates { get; private set; }

        /// <summary>
        /// Gets all static methods of this class.
        /// </summary>
        public CSharpMethod[] StaticMethods { get; private set; }

        /// <summary>
        /// Gets all instance methods of this class.
        /// </summary>
        public CSharpMethod[] InstanceMethods { get; private set; }

        /// <summary>
        /// Gets the instance constructors of this class.
        /// </summary>
        public CSharpMethod[] InstanceConstructors { get; private set; }

        /// <summary>
        /// Gets the instance destructor of this class.
        /// </summary>
        public CSharpMethod InstanceDestructor { get; private set; }

        /// <summary>
        /// Gets the static constructor of this class.
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

        /// <summary>
        /// Gets the type parameters of this class.
        /// </summary>
        public string[] TypeParameters { get; private set; }

        /// <summary>
        /// Gets the type constraints on the parameters.
        /// </summary>
        public Dictionary<string, CSharpType[]> TypeConstraints { get; private set; }
        #endregion

        internal static string[] EmptyTypeParameters = new string[0];

        /// <summary>
        /// Creastes the class from the syntax
        /// </summary>
        /// <param name="syntax">the syntax to create from</param>
        internal CSharpClass(ClassDeclarationSyntax syntax)
        {
            (GeneralModifier, AccessModifier) = syntax.Modifiers.GetConcreteModifier();
            Name = syntax.Identifier.ValueText;
            Attributes = syntax.AttributeLists.ToAttributes();
            if (syntax.TypeParameterList != null)
            {
                TypeParameters = syntax.TypeParameterList.ToNames();
                TypeConstraints = syntax.ConstraintClauses.ToTypeConstraints();
            }
            else
            {
                TypeParameters = EmptyTypeParameters;
            }


            AddMembers(syntax.Members);
        }

        internal void AddMembers(SyntaxList<MemberDeclarationSyntax> members)
        {
            List<CSharpClass> classes = new List<CSharpClass>();

            foreach(MemberDeclarationSyntax member in members)
            {
                if(member is ClassDeclarationSyntax)
                {
                    classes.Add(new CSharpClass((ClassDeclarationSyntax)member));
                }
            }
        }

        public override string ToString()
        {
            return $"{Attributes.ToReadablePrefix()}{AccessModifier.ToReadablePrefix()} {(IsStatic ? "static ":"")}class {Name}";
        }
    }
}
