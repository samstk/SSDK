using Microsoft.CodeAnalysis.CSharp;
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
    /// A c# property, which has get and set permissions that may differ each property.
    /// </summary>
    public class CSharpProperty
    {
        #region Properties & Fields
        /// <summary>
        /// Gets the attributes of the variable
        /// </summary>
        public CSharpAttribute[] Attributes { get; private set; }

        /// <summary>
        /// Gets the name of this property
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the type of this property.
        /// </summary>
        public CSharpType Type { get; private set; }
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

        /// <summary>
        /// Gets the general modifier of this property.
        /// </summary>
        public CSharpGeneralModifier GeneralModifier { get; private set; } = CSharpGeneralModifier.None;

        /// <summary>
        /// Gets the access modifier of this property.
        /// </summary>
        public CSharpAccessModifier AccessModifier { get; private set; }

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

        internal CSharpProperty(PropertyDeclarationSyntax syntax)
        {
            (GeneralModifier, AccessModifier) = syntax.Modifiers.GetConcreteModifier();
            
            Name = syntax.Identifier.ToString();
            Type = syntax.Type.ToType();
            Attributes = syntax.AttributeLists.ToAttributes();

            foreach(AccessorDeclarationSyntax accessor in syntax.AccessorList.Accessors)
            {
                if(accessor.Keyword.RawKind == (int)SyntaxKind.GetKeyword)
                {
                    (_, GetAccess) = accessor.Modifiers.GetConcreteModifier();
                    if (accessor.Body != null)
                    {
                        Get = new CSharpStatementBlock(accessor.Body);
                    }
                    else if (accessor.ExpressionBody != null)
                    {
                        Get = CSharpStatementBlock.WithReturn(accessor.ExpressionBody.Expression);
                    }
                }
                else if (accessor.Keyword.RawKind == (int)SyntaxKind.SetKeyword)
                {
                    (_, SetAccess) = accessor.Modifiers.GetConcreteModifier();
                    if(accessor.Body != null)
                    {
                        Set = new CSharpStatementBlock(accessor.Body);
                    }
                    else if (accessor.ExpressionBody != null)
                    {
                        Set = CSharpStatementBlock.WithReturn(accessor.ExpressionBody.Expression);
                    }
                }
            }
        }
    }
}
