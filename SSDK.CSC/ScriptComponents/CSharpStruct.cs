﻿using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SSDK.CSC.Helpers;

namespace SSDK.CSC.ScriptComponents
{
    /// <summary>
    /// A C# struct, which may contain methods, properties, fields, and sub-classes
    /// </summary>
    public sealed class CSharpStruct : CSharpComponent
    {
        #region Properties & Fields
        /// <summary>
        /// Gets the access modifier applied to this struct.
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
        /// Gets the static destructor of this struct.
        /// </summary>
        public CSharpMethod StaticDestructor { get; private set; }

        /// <summary>
        /// Gets all static properties of this struct
        /// </summary>
        public CSharpProperty[] StaticProperties { get; private set; }

        /// <summary>
        /// Gets all instance properties of this struct
        /// </summary>
        public CSharpProperty[] InstanceProperties { get; private set; }

        /// <summary>
        /// Gets all static fields of this struct
        /// </summary>
        public CSharpVariable[] StaticFields { get; private set; }

        /// <summary>
        /// Gets all instance fields of this struct
        /// </summary>
        public CSharpVariable[] InstanceFields { get; private set; }

        /// <summary>
        /// Gets all operator overloads of this struct.
        /// </summary>
        public CSharpMethod[] Operators { get; private set; }

        /// <summary>
        /// Gets the type parameters of this struct.
        /// </summary>
        public string[] TypeParameters { get; private set; }
        #endregion

        /// <summary>
        /// Creastes the struct from the syntax
        /// </summary>
        /// <param name="syntax">the syntax to create from</param>
        internal CSharpStruct(StructDeclarationSyntax syntax)
        {
            (_, AccessModifier) = syntax.Modifiers.GetConcreteModifier();
            Attributes = syntax.AttributeLists.ToAttributes();
            if(syntax.TypeParameterList!=null)
                TypeParameters = syntax.TypeParameterList.ToNames();

            AddMembers(syntax.Members);
        }

        internal void AddMembers(SyntaxList<MemberDeclarationSyntax> members)
        {
            List<CSharpClass> classes = new List<CSharpClass>();
            List<CSharpStruct> structs = new List<CSharpStruct>();
            List<CSharpDelegate> delegates = new List<CSharpDelegate>();
            List<CSharpMethod> staticMethods = new List<CSharpMethod>();
            List<CSharpMethod> instanceMethods = new List<CSharpMethod>();
            List<CSharpMethod> instanceConstructors = new List<CSharpMethod>();
            List<CSharpProperty> staticProperties = new List<CSharpProperty>();
            List<CSharpProperty> instanceProperties = new List<CSharpProperty>();
            List<CSharpVariable> staticFields = new List<CSharpVariable>();
            List<CSharpVariable> instanceFields = new List<CSharpVariable>();
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
                    foreach(CSharpVariable variable in variables)
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
                else if (member is EventDeclarationSyntax)
                {
                    
                }
            }

            Subclasses = classes.ToArray();
            Substructs = structs.ToArray();
            Delegates = delegates.ToArray();
            StaticMethods = staticMethods.ToArray();
            InstanceMethods = instanceMethods.ToArray();
            InstanceConstructors = instanceConstructors.ToArray();
            StaticProperties = staticProperties.ToArray();
            InstanceProperties = instanceProperties.ToArray();
            StaticFields = staticFields.ToArray();
            InstanceFields = instanceFields.ToArray();
        }
    }
}
