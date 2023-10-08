using Microsoft.CodeAnalysis.CSharp.Syntax;
using SSDK.CSC.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSDK.CSC.ScriptComponents.Expressions
{
    /// <summary>
    /// A C# instantiation expression (e.g. new Dict(true))
    /// </summary>
    public sealed class CSharpInstantiationExpression : CSharpExpression
    {
        #region Properties & Fields
        /// <summary>
        /// Gets the type of this expression
        /// indicating the class/struct target.
        /// </summary>
        public CSharpType Type { get; private set; }

        /// <summary>
        /// Gets the arguments to be used in this instantiation.
        /// </summary>
        public CSharpExpression[] Arguments { get; private set; }

        /// <summary>
        /// Gets the initializer of the instantiation (in curly braces)
        /// </summary>
        public CSharpExpression[] Initializer { get; private set; }

        /// <summary>
        /// If true, then the Type instantiation is not set, which
        /// means it must be derived from the context.
        /// </summary>
        public bool IsImplicit { get; private set; }

        /// <summary>
        /// If true, then the instantiation is for an implicit array creation
        /// (i.e. no arguments, just array elements {1, 2, 3})
        /// </summary>
        public bool IsArrayInitializer { get; private set; }
        #endregion

        /// <summary>
        /// Creates the instantiation expression from the syntax.
        /// </summary>
        /// <param name="syntax">the syntax to create from</param>
        internal CSharpInstantiationExpression(ObjectCreationExpressionSyntax syntax)
        {
            Syntax = syntax;
            Type = syntax.Type?.ToType();
            Arguments = syntax.ArgumentList?.ToExpressions();
            Initializer = CSharpExpression.Empty;
            if (syntax.Initializer != null)
                Initializer = syntax.Initializer.Expressions.ToExpressions();
            IsImplicit = false;
        }

        /// <summary>
        /// Creates the instantiation expression from the syntax.
        /// </summary>
        /// <param name="syntax">the syntax to create from</param>
        internal CSharpInstantiationExpression(ImplicitObjectCreationExpressionSyntax syntax)
        {
            Syntax = syntax;
            Arguments = syntax.ArgumentList?.ToExpressions();
            Initializer = CSharpExpression.Empty;
            if (syntax.Initializer != null)
                Initializer = syntax.Initializer.Expressions.ToExpressions();
            IsImplicit = true;
        }

        internal CSharpInstantiationExpression(InitializerExpressionSyntax syntax)
        {
            Syntax = Syntax;
            Initializer = syntax.Expressions.ToExpressions();
            IsImplicit = true;
            IsArrayInitializer = true;
        }

        public override void ProcessMap(CSharpConversionMap map, StringBuilder result)
        {
            map.ProcessInstantiationExpression(this, result);
        }

        internal override void CreateMemberSymbols(CSharpProject project, CSharpMemberSymbol parentSymbol)
        {
            Symbol = new CSharpMemberSymbol("new(", parentSymbol, this, false);
            if (Arguments != null)
            {
                foreach (CSharpExpression expression in Arguments)
                {
                    expression?.CreateMemberSymbols(project, Symbol);
                }
            }
            foreach(CSharpExpression expression in Initializer)
            {
                expression?.CreateMemberSymbols(project, Symbol);
            }
        }

        internal override void ResolveMembers(CSharpProject project)
        {
            if (Arguments != null)
            {
                foreach (CSharpExpression expression in Arguments)
                {
                    expression?.ResolveMembers(project);
                }
            }
            foreach (CSharpExpression expression in Initializer)
            {
                expression?.ResolveMembers(project);
            }
        }

        public override string ToString()
        {
            return $"new {Type}({Arguments.ToReadableString()}) "+"{ "+Initializer.ToReadableString()+" }";
        }
    }
}
