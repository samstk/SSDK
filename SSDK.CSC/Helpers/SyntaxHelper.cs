using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SSDK.CSC.ScriptComponents;
using SSDK.CSC.ScriptComponents.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSDK.CSC.Helpers
{
    /// <summary>
    /// Contains syntax helper methods for generating CSC syntax conversions.
    /// </summary>
    internal static class SyntaxHelper
    {
        /// <summary>
        /// Gets the c# types of a given type parameter list syntax list.
        /// </summary>
        /// <param name="parameterList">the parameter list to find types of</param>
        /// <returns>an array of c# types for the parameter list</returns>
        internal static CSharpType[] GetTypes(this TypeParameterListSyntax parameterList)
        {
            return parameterList.Parameters.Select((typeparam) => typeparam.ToType()).ToArray();
        }

        /// <summary>
        /// Converts the given type syntax to a c# type
        /// </summary>
        /// <param name="type">the type syntax to convert</param>
        /// <returns>the c# type</returns>
        /// <exception cref="Exception">occurs when an unhandled type syntax happens (to be checked)</exception>
        internal static CSharpType ToType(this TypeSyntax type)
        {
            return new CSharpType(type);
        }

        /// <summary>
        /// Converts the given type syntax to a c# type
        /// </summary>
        /// <param name="type">the type syntax to convert</param>
        /// <returns>the c# type</returns>
        /// <exception cref="Exception">occurs when an unhandled type syntax happens (to be checked)</exception>
        internal static CSharpType ToType(this TypeParameterSyntax type)
        {
            return new CSharpType(type);
        }

        /// <summary>
        /// Gets the c# types of a given type argument list syntax list.
        /// </summary>
        /// <param name="argumentList">the argument list to find types of</param>
        /// <returns>an array of c# types for the argument list</returns>
        internal static CSharpType[] ToTypes(this TypeArgumentListSyntax argumentList)
        {
            return argumentList.Arguments.Select((typearg) => typearg.ToType()).ToArray();
        }

        /// <summary>
        /// Converts the given compatible syntax to an c# expression.
        /// </summary>
        /// <param name="syntax">the compatible syntax</param>
        /// <returns>the c# expression resulting from the syntax</returns>
        internal static CSharpExpression ToExpression(this AttributeArgumentSyntax syntax)
        {
            return syntax.Expression.ToExpression();
        }

        /// <summary>
        /// Converts the given compatible syntax to an c# expression.
        /// </summary>
        /// <param name="syntax">the compatible syntax</param>
        /// <returns>the c# expression resulting from the syntax</returns>
        internal static CSharpExpression ToExpression(this ExpressionSyntax syntax)
        {
            if (syntax is BinaryExpressionSyntax)
            {
                return new CSharpBinaryExpression((BinaryExpressionSyntax)syntax);
            }
            else if (syntax is LiteralExpressionSyntax)
            {
                return new CSharpLiteralValue((LiteralExpressionSyntax)syntax);
            }
            throw new Exception("Unhandled case"); 
        }

        /// <summary>
        /// Converts the given compatible syntax to an c# expression.
        /// In this case, it extracts the value expression from the equals clause.
        /// <br/>
        /// e.g. (=512) will return 512.
        /// </summary>
        /// <param name="syntax">the compatible syntax</param>
        /// <returns>the c# expression resulting from the syntax</returns>

        internal static CSharpExpression ToExpression(this EqualsValueClauseSyntax syntax)
        {
            return syntax.Value.ToExpression();
        }

        /// <summary>
        /// Converts the given attribute argument list to a list of expressions.
        /// </summary>
        /// <param name="syntax">the attribute argument list syntax to convert</param>
        /// <returns>an array of expressions representing the parameters of the attribute</returns>

        internal static CSharpExpression[] ToExpressions(this AttributeArgumentListSyntax syntax)
        {
            CSharpExpression[] expressions = new CSharpExpression[syntax.Arguments.Count];
            for(int i = 0; i<syntax.Arguments.Count; i++)
            {
                expressions[i] = syntax.Arguments[i].ToExpression();
            }
            return expressions;
        }

        /// <summary>
        /// Gets the c# attributes of a given attribute syntax list
        /// </summary>
        /// <param name="attributes">the attribute syntax list to get attribute froms</param>
        /// <returns>an array of c# attributes for the attribute list</returns>
        internal static CSharpAttribute[] ToAttributes(this SyntaxList<AttributeListSyntax> attributes)
        {
            List<CSharpAttribute> result = new List<CSharpAttribute>();
            foreach(AttributeListSyntax attrList in attributes)
            {
                foreach(AttributeSyntax attr in attrList.Attributes)
                {
                    result.Add(new CSharpAttribute(attr));
                }
            }
            return result.ToArray();
        }

        /// <summary>
        /// Gets the c# variables of a given parameter list.
        /// </summary>
        /// <param name="parameterList">the parameter list to search</param>
        /// <returns>an array of c# variables which is a effective copy of the parameter list</returns>
        internal static CSharpVariable[] ToParameters(this ParameterListSyntax parameterList)
        {
            CSharpVariable[] variables = new CSharpVariable[parameterList.Parameters.Count];
            
            for(int i = 0; i<parameterList.Parameters.Count; i++)
            {
                variables[i] = new CSharpVariable(parameterList.Parameters[i]);
            }
            
            return variables;
        }

        /// <summary>
        /// Gets the concrete modifier (single/combation) for a given syntax modifier list.
        /// </summary>
        /// <param name="modifierList">the syntax token list that contains the modifiers</param>
        /// <returns>a tuple of whether the static modifier is present, the correct access modifier for a combination of syntax tokens</returns>
        internal static (bool, CSharpAccessModifier) GetConcreteModifier(this SyntaxTokenList modifierList)
        {
            // Detect access modifiers in token list
            bool hasPublic = false;
            bool hasPrivate = false;
            bool hasInternal = false;
            bool hasProtected = false;
            bool isStatic = false;

            foreach (SyntaxToken token in modifierList)
            {
                if (token.Value != null)
                {
                    if (token.ValueText == "public")
                    {
                        hasPublic = true;
                    }
                    else if (token.ValueText == "private")
                    {
                        hasPrivate = true;
                    }
                    else if (token.ValueText == "internal")
                    {
                        hasInternal = true;
                    }
                    else if (token.ValueText == "protected")
                    {
                        hasProtected = true;
                    }
                    else if (token.ValueText == "static")
                    {
                        isStatic = true;
                    }
                }
            }

            // Return corresponding modifier
            return (isStatic,
                hasPublic ? CSharpAccessModifier.Public
                : hasPrivate ? (hasProtected ? CSharpAccessModifier.PrivateProtected : CSharpAccessModifier.Private)
                : hasProtected ? (hasInternal ? CSharpAccessModifier.ProtectedInternal : CSharpAccessModifier.Protected)
                : CSharpAccessModifier.Internal);
        }
    }
}
