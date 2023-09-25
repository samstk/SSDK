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
    /// A c# variable declaration, which is also an statement.
    /// </summary>
    public sealed class CSharpVariable : CSharpStatement
    {
        #region Properties & Fields
        /// <summary>
        /// Gets the type of the variable
        /// </summary>
        public CSharpType Type { get; private set; }

        /// <summary>
        /// Gets the name of the variable
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Returns true if the variable is part of a statement, rather
        /// than an argument/parameter list.
        /// </summary>
        public bool InStatement { get; internal set; } = false;

        /// <summary>
        /// Gets the access modifier of this variable.
        /// Default is internal.
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
        public bool IsSealed{ get { return GeneralModifier.HasFlag(CSharpGeneralModifier.Sealed); } }
        public bool IsStatic { get { return GeneralModifier.HasFlag(CSharpGeneralModifier.Static); } }
        public bool IsUnsafe { get { return GeneralModifier.HasFlag(CSharpGeneralModifier.Unsafe); } }
        public bool IsVirtual { get { return GeneralModifier.HasFlag(CSharpGeneralModifier.Virtual); } }
        public bool IsVolatile { get { return GeneralModifier.HasFlag(CSharpGeneralModifier.Volatile); } }
        #endregion

        /// <summary>
        /// Gets the attributes of the variable
        /// </summary>
        public CSharpAttribute[] Attributes { get; private set; }

        /// <summary>
        /// Gets the expression when this variable/parameter was first declared
        /// (default expression)
        /// </summary>
        public CSharpExpression Expression { get; private set; }
        #endregion
    
        /// <summary>
        /// Creates a c# variable from the given syntax
        /// </summary>
        /// <param name="paramSyntax">the parameter syntax</param>
        internal CSharpVariable(ParameterSyntax paramSyntax)
        {
            Name = paramSyntax.Identifier.ToString();
            Type = paramSyntax.Type.ToType();
            Attributes = paramSyntax.AttributeLists.ToAttributes();
            if (paramSyntax.Default != null)
            {
                Expression = paramSyntax.Default.ToExpression();
            }
        }

        /// <summary>
        /// Creates a c# variable from the given information
        /// </summary>
        /// <param name="name">name of the variable</param>
        /// <param name="generalModifier">the general modifiers of the variable (e.g. volatile)</param>
        /// <param name="accessModifier">the access modifier of the variable</param>
        /// <param name="expression">the initial expression of the variable</param>
        /// <param name="attributes">the attributes of the variable</param>
        /// <param name="type">the type of the variable</param>
        internal CSharpVariable(string name, CSharpType type, CSharpAttribute[] attributes, CSharpGeneralModifier generalModifier, CSharpAccessModifier accessModifier, CSharpExpression expression)
        {
            Name = name;
            Type = type;
            GeneralModifier = generalModifier;
            AccessModifier = accessModifier;
            Attributes = attributes;
            Expression = expression;
        }

        public override void ProcessMap(CSharpConversionMap map, StringBuilder result)
        {
            map.ProcessVariable(this, result);
        }

        public override string ToString()
        {
            return $"{Attributes.ToReadablePrefix()}{AccessModifier.ToReadablePrefix()} {GeneralModifier.ToReadablePrefix()}{Type} {Name}{(Expression != null ? $" = {Expression}" : "")}";
        }
    }
}
