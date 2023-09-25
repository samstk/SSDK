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
    /// A C# method, which may define the format for an anonymous function.
    /// </summary>
    public sealed class CSharpMethod : CSharpComponent
    {
        #region Properties & Fields
        /// <summary>
        /// Gets all attributes applied to this component.
        /// </summary>
        public CSharpAttribute[] Attributes { get; private set; }

        /// <summary>
        /// Gets the access modifier applied to this method.
        /// </summary>
        public CSharpAccessModifier AccessModifier { get; private set; } = CSharpAccessModifier.DefaultOrNone;

        /// <summary>
        /// Gets the syntax that constructed this method.
        /// </summary>
        public MethodDeclarationSyntax Syntax { get; private set; }

        /// <summary>
        /// True if this method is a constructor
        /// </summary>
        public bool IsConstructor { get; private set; } = false;

        /// <summary>
        /// True if this method is a destructor
        /// </summary>
        public bool IsDestructor { get; private set; } = false;

        /// <summary>
        /// If this method is a constructor method, then get the syntax
        /// that constructed this method.
        /// </summary>
        public ConstructorDeclarationSyntax ConstructorSyntax { get; private set; }
       
        /// <summary>
        /// If this method is a destructor method, then get the syntax
        /// that constructed this method.
        /// </summary>
        public DestructorDeclarationSyntax DestructorSyntax { get; private set; }

        /// <summary>
        /// Gets the name of the method
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the return type of the method
        /// </summary>
        public CSharpType ReturnType { get; private set; }

        /// <summary>
        /// Gets the parameters of the method
        /// </summary>
        public CSharpVariable[] Parameters { get; private set; }

        /// <summary>
        /// Gets the type parameters of the method
        /// </summary>
        public string[] TypeParameters { get; private set; }

        /// <summary>
        /// Gets the type constraints on the parameters.
        /// </summary>
        public Dictionary<string, CSharpType[]> TypeConstraints { get; private set; }

        /// <summary>
        /// Gets the execution block of this method.
        /// </summary>
        public CSharpStatementBlock Block { get; private set; }

        /// <summary>
        /// Gets the general modifier of this method
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
        #endregion

        internal CSharpMethod(MethodDeclarationSyntax syntax)
        {
            Syntax = syntax;

            (GeneralModifier, AccessModifier) = syntax.Modifiers.GetConcreteModifier();

            Name = syntax.Identifier.ToString();

            Attributes = syntax.AttributeLists.ToAttributes();

            ReturnType = syntax.ReturnType.ToType();

            Parameters = syntax.ParameterList.ToParameters();

            TypeParameters = syntax.TypeParameterList.ToNames();

            TypeConstraints = syntax.ConstraintClauses.ToTypeConstraints();

            if(syntax.Body != null)
            {
                Block = new CSharpStatementBlock(syntax.Body);
            }
            else if (syntax.ExpressionBody != null)
            {
                Block = CSharpStatementBlock.WithReturn(syntax.ExpressionBody.Expression);
            }
        }

        internal CSharpMethod(ConstructorDeclarationSyntax syntax)
        {
            IsConstructor = true;

            ConstructorSyntax = syntax;

            (GeneralModifier, AccessModifier) = syntax.Modifiers.GetConcreteModifier();

            Name = syntax.Identifier.ToString();

            Attributes = syntax.AttributeLists.ToAttributes();

            ReturnType = CSharpType.Special("this");

            Parameters = syntax.ParameterList.ToParameters();

            TypeParameters = new string[0];

            TypeConstraints = new Dictionary<string, CSharpType[]>();

            if (syntax.Body != null)
            {
                Block = new CSharpStatementBlock(syntax.Body);
            }
            else if (syntax.ExpressionBody != null)
            {
                Block = CSharpStatementBlock.WithReturn(syntax.ExpressionBody.Expression);
            }
        }

        internal CSharpMethod(DestructorDeclarationSyntax syntax)
        {
            IsDestructor = true;

            DestructorSyntax = syntax;

            (GeneralModifier, AccessModifier) = syntax.Modifiers.GetConcreteModifier();

            Name = syntax.Identifier.ToString();

            Attributes = syntax.AttributeLists.ToAttributes();

            ReturnType = CSharpType.Special("~");

            Parameters = syntax.ParameterList.ToParameters();

            TypeParameters = new string[0];

            TypeConstraints = new Dictionary<string, CSharpType[]>();

            if (syntax.Body != null)
            {
                Block = new CSharpStatementBlock(syntax.Body);
            }
            else if (syntax.ExpressionBody != null)
            {
                Block = CSharpStatementBlock.WithReturn(syntax.ExpressionBody.Expression);
            }
        }

        public override string ToString()
        {
            return $"{(AccessModifier.ToReadablePrefix())} {ReturnType} {Name}({Parameters.ToReadableString()}) ...";
        }
    }
}
