using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SSDK.CSC.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SSDK.CSC.ScriptComponents
{
    /// <summary>
    /// A C# class, which may contain methods, properties, fields, and sub-classes.
    /// </summary>
    public sealed class CSharpClass : CSharpComponent
    {

        #region Properties & Fields
        /// <summary>
        /// Gets the symbol that represents this component.
        /// </summary>
        /// <remarks>
        /// ResolveMembers must be called on the project before being set.
        /// </remarks>
        public CSharpMemberSymbol Symbol { get; private set; }

        /// <summary>
        /// Gets the name of the class
        /// </summary>
        public string Name { get; private set; }
        /// <summary>
        /// Gets the access modifier applied to this class.
        /// </summary>
        public CSharpAccessModifier AccessModifier { get; private set; } = CSharpAccessModifier.DefaultOrNone;

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
        /// Gets all sub-enums of this class.
        /// </summary>
        public CSharpEnum[] Subenums { get; private set; }

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
        /// Gets all indexers of this class.
        /// </summary>
        public CSharpIndexer[] Indexers { get; private set; }   

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
        /// Gets the type parameters of this class as resolved
        /// symbols.
        /// </summary>
        /// <remarks>
        /// The ResolveMembers method must be called on the project.
        /// </remarks>
        public CSharpMemberSymbol[] TypeParameterSymbols { get; private set; }

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
                TypeConstraints = new Dictionary<string, CSharpType[]>();
            }

            if (syntax.BaseList == null || syntax.BaseList.Types.Count == 0)
                Inherits = CSharpType.Empty;
            else Inherits = syntax.BaseList.ToTypes();

            AddMembers(syntax.Members);
        }

        internal void AddMembers(SyntaxList<MemberDeclarationSyntax> members)
        {
            List<CSharpClass> classes = new List<CSharpClass>();
            List<CSharpStruct> structs = new List<CSharpStruct>();
            List<CSharpEnum> enums = new List<CSharpEnum>();
            List<CSharpDelegate> delegates = new List<CSharpDelegate>();
            List<CSharpMethod> staticMethods = new List<CSharpMethod>();
            List<CSharpMethod> instanceMethods = new List<CSharpMethod>();
            List<CSharpMethod> instanceConstructors = new List<CSharpMethod>();
            List<CSharpProperty> staticProperties = new List<CSharpProperty>();
            List<CSharpProperty> instanceProperties = new List<CSharpProperty>();
            List<CSharpIndexer> indexers = new List<CSharpIndexer>(); 
            List<CSharpVariable> staticFields = new List<CSharpVariable>();
            List<CSharpVariable> instanceFields = new List<CSharpVariable>();
            List<CSharpMethod> operators = new List<CSharpMethod>();
            foreach (MemberDeclarationSyntax member in members)
            {
                if (member is ClassDeclarationSyntax)
                {
                    classes.Add(new CSharpClass((ClassDeclarationSyntax)member));
                }
                else if (member is StructDeclarationSyntax)
                {
                    structs.Add(new CSharpStruct((StructDeclarationSyntax)member));
                }
                else if (member is DelegateDeclarationSyntax)
                {
                    delegates.Add(new CSharpDelegate((DelegateDeclarationSyntax)member));
                }
                else if (member is FieldDeclarationSyntax)
                {
                    CSharpVariable[] variables = ((FieldDeclarationSyntax)member).ToVariables();
                    foreach (CSharpVariable variable in variables)
                    {
                        if (variable.IsStatic || variable.IsConst)
                            staticFields.Add(variable);
                        else instanceFields.Add(variable);
                    }
                }
                else if (member is PropertyDeclarationSyntax)
                {
                    CSharpProperty property = new CSharpProperty((PropertyDeclarationSyntax)member);

                    if (property.IsStatic || property.IsConst)
                        staticProperties.Add(property);
                    else instanceProperties.Add(property);
                }
                else if (member is EventFieldDeclarationSyntax)
                {
                    CSharpVariable[] variables = ((EventFieldDeclarationSyntax)member).ToVariables();
                    foreach (CSharpVariable variable in variables)
                    {
                        if (variable.IsStatic || variable.IsConst)
                            staticFields.Add(variable);
                        else instanceFields.Add(variable);
                    }
                }
                else if (member is EventDeclarationSyntax)
                {
                    CSharpVariable variable = ((EventDeclarationSyntax)member).ToVariable();
                    if (variable.IsStatic || variable.IsConst)
                        staticFields.Add(variable);
                    else instanceFields.Add(variable);
                }
                else if (member is MethodDeclarationSyntax)
                {
                    CSharpMethod method = new CSharpMethod((MethodDeclarationSyntax)member);
                    if (method.IsStatic || method.IsConst)
                        staticMethods.Add(method);
                    else instanceMethods.Add(method);
                }
                else if (member is ConstructorDeclarationSyntax)
                {
                    CSharpMethod method = new CSharpMethod((ConstructorDeclarationSyntax)member);
                    if (method.IsStatic || method.IsConst)
                        StaticConstructor = method;
                    else instanceConstructors.Add(method);
                }
                else if (member is DestructorDeclarationSyntax)
                {
                    CSharpMethod method = new CSharpMethod((DestructorDeclarationSyntax)member);
                    if (method.IsStatic || method.IsConst)
                        StaticDestructor = method;
                    else InstanceDestructor = method;
                }
                else if (member is EnumDeclarationSyntax)
                {
                    enums.Add(new CSharpEnum((EnumDeclarationSyntax)member));
                }
                else if (member is IndexerDeclarationSyntax)
                {
                    indexers.Add(new CSharpIndexer((IndexerDeclarationSyntax)member));
                }
                else if (member is OperatorDeclarationSyntax)
                {
                    operators.Add(new CSharpMethod((OperatorDeclarationSyntax)member));
                }
                else if (member is ConversionOperatorDeclarationSyntax)
                {
                    operators.Add(new CSharpMethod((ConversionOperatorDeclarationSyntax)member));
                }
                else throw new Exception();
            }

            Subclasses = classes.ToArray();
            Substructs = structs.ToArray();
            Subenums = enums.ToArray();
            Delegates = delegates.ToArray();
            StaticMethods = staticMethods.ToArray();
            InstanceMethods = instanceMethods.ToArray();
            InstanceConstructors = instanceConstructors.ToArray();
            StaticProperties = staticProperties.ToArray();
            InstanceProperties = instanceProperties.ToArray();
            StaticFields = staticFields.ToArray();
            InstanceFields = instanceFields.ToArray();
            Indexers = indexers.ToArray();
            Operators = operators.ToArray();
        }

        /// <summary>
        /// Creates a member symbol for this component
        /// </summary>
        internal override void CreateMemberSymbols(CSharpProject project, CSharpMemberSymbol parentSymbol)
        {
            Symbol = new CSharpMemberSymbol(Name, parentSymbol, this);

            foreach (CSharpClass @class in Subclasses)
            {
                @class.CreateMemberSymbols(project, Symbol);
            }

            foreach (CSharpStruct @struct in Substructs)
            {
                @struct.CreateMemberSymbols(project, Symbol);
            }

            foreach (CSharpEnum @enum in Subenums)
            {
                @enum.CreateMemberSymbols(project, Symbol);
            }

            foreach (CSharpDelegate @delegate in Delegates)
            {
                @delegate.CreateMemberSymbols(project, Symbol);
            }

            foreach (CSharpMethod method in StaticMethods)
            {
                method.CreateMemberSymbols(project, Symbol);
            }

            foreach (CSharpMethod method in InstanceMethods)
            {
                method.CreateMemberSymbols(project, Symbol);
            }

            foreach (CSharpMethod method in InstanceConstructors)
            {
                method.CreateMemberSymbols(project, Symbol);
            }

            foreach (CSharpMethod method in Operators)
            {
                method.CreateMemberSymbols(project, Symbol);
            }

            foreach (CSharpIndexer indexer in Indexers)
            {
                indexer.CreateMemberSymbols(project, Symbol);
            }

            InstanceDestructor?.CreateMemberSymbols(project, Symbol);
            StaticConstructor?.CreateMemberSymbols(project, Symbol);
            StaticDestructor?.CreateMemberSymbols(project, Symbol);

            foreach (CSharpProperty property in InstanceProperties)
            {
                property.CreateMemberSymbols(project, Symbol);
            }

            foreach (CSharpProperty property in StaticProperties)
            {
                property.CreateMemberSymbols(project, Symbol);
            }

            foreach (CSharpVariable field in InstanceFields)
            {
                field.CreateMemberSymbols(project, Symbol);
            }

            foreach (CSharpVariable field in StaticFields)
            {
                field.CreateMemberSymbols(project, Symbol);
            }
        }

        public override string ToString()
        {
            return $"{Attributes.ToReadablePrefix()}{AccessModifier.ToReadablePrefix()} {(IsStatic ? "static ":"")}class {Name}";
        }

        internal override void ResolveMembers(CSharpProject project)
        {
            foreach (CSharpClass @class in Subclasses)
            {
                @class.ResolveMembers(project);
            }

            foreach (CSharpStruct @struct in Substructs)
            {
                @struct.ResolveMembers(project);
            }

            foreach (CSharpEnum @enum in Subenums)
            {
                @enum.ResolveMembers(project);
            }

            foreach (CSharpDelegate @delegate in Delegates)
            {
                @delegate.ResolveMembers(project);
            }

            foreach (CSharpMethod method in StaticMethods)
            {
                method.ResolveMembers(project);
            }

            foreach (CSharpMethod method in InstanceMethods)
            {
                method.ResolveMembers(project);
            }

            foreach (CSharpMethod method in InstanceConstructors)
            {
                method.ResolveMembers(project);
            }

            foreach (CSharpMethod method in Operators)
            {
                method.ResolveMembers(project);
            }

            foreach (CSharpIndexer indexer in Indexers)
            {
                indexer.ResolveMembers(project);
            }

            InstanceDestructor?.ResolveMembers(project);
            StaticConstructor?.ResolveMembers(project);
            StaticDestructor?.ResolveMembers(project);

            foreach (CSharpProperty property in InstanceProperties)
            {
                property.ResolveMembers(project);
            }

            foreach (CSharpProperty property in StaticProperties)
            {
                property.ResolveMembers(project);
            }

            foreach (CSharpVariable field in InstanceFields)
            {
                field.ResolveMembers(project);
            }

            foreach (CSharpVariable field in StaticFields)
            {
                field.ResolveMembers(project);
            }
        }
    }
}
