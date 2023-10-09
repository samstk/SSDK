using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SSDK.CSC.Helpers;
using SSDK.CSC.ScriptComponents.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SSDK.CSC.ScriptComponents
{
    /// <summary>
    /// A c# indexer, which has get and set permissions on a index (e.g. this[x])
    /// </summary>
    public class CSharpIndexer : CSharpComponent
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
        /// Gets the attributes of the indexer
        /// </summary>
        public CSharpAttribute[] Attributes { get; private set; }

        /// <summary>
        /// Gets the type of this property.
        /// </summary>
        public CSharpType Type { get; private set; }
        /// <summary>
        /// Gets the get access of this property.
        /// </summary>
        public CSharpAccessModifier GetAccess { get; private set; } = CSharpAccessModifier.DefaultOrNone;

        /// <summary>
        /// Gets the statement block for getting this property.
        /// </summary>
        public CSharpStatementBlock Get { get; private set; }

        /// <summary>
        /// If true, this property has get access
        /// </summary>
        public bool HasGetAccess { get; private set; }

        /// <summary>
        /// Gets the statement block for setting this property.
        /// </summary>
        public CSharpStatementBlock Set { get; private set; }

        /// <summary>
        /// Gets the set access of this property.
        /// </summary>
        public CSharpAccessModifier SetAccess { get; private set; } = CSharpAccessModifier.DefaultOrNone;

        /// <summary>
        /// If true, this property has set access
        /// </summary>
        public bool HasSetAccess { get; private set; }

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

        /// <summary>
        /// Gets all parameters of this indexer
        /// </summary>
        public CSharpVariable[] Parameters { get; private set; }
        #endregion

        internal CSharpIndexer(IndexerDeclarationSyntax syntax)
        {
            (GeneralModifier, AccessModifier) = syntax.Modifiers.GetConcreteModifier();

            Type = syntax.Type.ToType();
            Attributes = syntax.AttributeLists.ToAttributes();

            foreach (AccessorDeclarationSyntax accessor in syntax.AccessorList.Accessors)
            {
                if (accessor.Keyword.RawKind == (int)SyntaxKind.GetKeyword)
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
                    HasGetAccess = true;
                }
                else if (accessor.Keyword.RawKind == (int)SyntaxKind.SetKeyword)
                {
                    (_, SetAccess) = accessor.Modifiers.GetConcreteModifier();
                    if (accessor.Body != null)
                    {
                        Set = new CSharpStatementBlock(accessor.Body);
                    }
                    else if (accessor.ExpressionBody != null)
                    {
                        Set = CSharpStatementBlock.WithReturn(accessor.ExpressionBody.Expression);
                    }
                    HasSetAccess = true;
                }
            }

            Parameters = syntax.ParameterList.ToVariables();
        }

        /// <summary>
        /// Creates a member symbol for this component
        /// </summary>
        internal override void CreateMemberSymbols(CSharpProject project, CSharpMemberSymbol parentSymbol)
        {
            Symbol = new CSharpMemberSymbol("[]", parentSymbol, this);

            Type?.CreateMemberSymbols(project, Symbol);
            Get?.CreateMemberSymbols(project, Symbol);
            Set?.CreateMemberSymbols(project, Symbol);
        }

        internal override void ResolveMembers(CSharpProject project)
        {
            Type?.CreateMemberSymbols(project, Symbol);

            Get?.ResolveMembers(project);
            Set?.ResolveMembers(project);
        }
    }
}
