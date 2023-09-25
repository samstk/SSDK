﻿using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SSDK.CSC.ScriptComponents;
using SSDK.CSC.ScriptComponents.Expressions;
using SSDK.CSC.ScriptComponents.Statements;
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
        /// Converts the given type parameter constraint clauses into a dictionary.
        /// </summary>
        /// <param name="typeSyntax">the type syntax to convert</param>
        /// <returns>a dictionary containing all type constraints on param keys</returns>
        internal static Dictionary<string, CSharpType[]> ToTypeConstraints(this SyntaxList<TypeParameterConstraintClauseSyntax> typeSyntax)
        {
            Dictionary<string, List<CSharpType>> constraints = new Dictionary<string, List<CSharpType>>();
            
            // Find constraints on each type parameter.

            foreach(TypeParameterConstraintClauseSyntax constraint in typeSyntax)
            {
                string key = constraint.Name.ToString();
                if (!constraints.ContainsKey(key)) constraints.Add(key, new List<CSharpType>());

                constraints[key].AddRange(constraint.Constraints.ToTypeConstraints());
            }

            // Convert to array

            Dictionary<string, CSharpType[]> returnConstraints = new Dictionary<string, CSharpType[]>();

            foreach(string key in constraints.Keys)
            {
                returnConstraints.Add(key, constraints[key].ToArray());
            }

            return returnConstraints;
        }

        /// <summary>
        /// Converts the given type parameter constraints into an array.
        /// </summary>
        /// <param name="typeSyntax">the type syntax to convert</param>
        /// <returns>an array of all type constraints</returns>
        internal static CSharpType[] ToTypeConstraints(this SeparatedSyntaxList<TypeParameterConstraintSyntax> typeSyntax)
        {
            CSharpType[] types = new CSharpType[typeSyntax.Count];

            for(int i = 0; i<types.Length; i++)
            {
                types[i] = new CSharpType(typeSyntax[i] as TypeConstraintSyntax);
            }

            return types;
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
        /// Converts the given type syntax to a c# type
        /// </summary>
        /// <param name="type">the type syntax to convert</param>
        /// <returns>the c# type</returns>
        /// <exception cref="Exception">occurs when an unhandled type syntax happens (to be checked)</exception>
        internal static CSharpType ToType(this TypeConstraintSyntax type)
        {
            return new CSharpType(type);
        }

        /// <summary>
        /// Converts the given list of type parameters to a string of arrays
        /// </summary>
        /// <param name="typeList">the type list syntax to convert</param>
        /// <returns>the c# type</returns>
        internal static string[] ToNames(this TypeParameterListSyntax typeList)
        {
            string[] types = new string[typeList.Parameters.Count];
            for(int i = 0; i<typeList.Parameters.Count; i++)
            {
                types[i] = typeList.Parameters[i].Identifier.ToString();
            }
            return types;
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
        /// Converts the given compatible syntax to a c# statement.
        /// </summary>
        /// <param name="syntax">the compatible syntax</param>
        /// <returns>the c# expression resulting from the syntax.</returns>
        internal static CSharpStatement ToStatement(this StatementSyntax syntax)
        {
            if (syntax is ReturnStatementSyntax)
            {
                return new CSharpReturnStatement(((ReturnStatementSyntax)syntax).Expression);
            }
            else if (syntax is ExpressionStatementSyntax)
            {
                return new CSharpExpressionStatement(((ExpressionStatementSyntax)syntax).Expression.ToExpression());
            }
            throw new Exception("Unhandled case");
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
                return new CSharpLiteralValueExpression((LiteralExpressionSyntax)syntax);
            }
            else if (syntax is AssignmentExpressionSyntax)
            {
                return new CSharpAssignmentExpression((AssignmentExpressionSyntax)syntax);
            }
            else if (syntax is IdentifierNameSyntax)
            {
                return new CSharpIdentifierExpression(((IdentifierNameSyntax)syntax));
            }
            else if (syntax is InvocationExpressionSyntax)
            {
                return new CSharpInvocationExpression(((InvocationExpressionSyntax)syntax));
            }
            else if (syntax is MemberAccessExpressionSyntax)
            {
                return new CSharpMemberAccessExpression(((MemberAccessExpressionSyntax)syntax));
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
        /// Converts the given compatible syntax to an c# expression.
        /// </summary>
        /// <param name="syntax">the compatible syntax</param>
        /// <returns>the c# expression resulting from the syntax</returns>

        internal static CSharpExpression ToExpression(this ArgumentSyntax syntax)
        {
            return syntax.Expression.ToExpression();
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
        /// Converts the given argument list to a list of expressions.
        /// </summary>
        /// <param name="syntax">the argument list syntax to convert</param>
        /// <returns>an array of expressions representing the parameters of a function</returns>

        internal static CSharpExpression[] ToExpressions(this ArgumentListSyntax syntax)
        {
            CSharpExpression[] expressions = new CSharpExpression[syntax.Arguments.Count];
            for (int i = 0; i < syntax.Arguments.Count; i++)
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
        /// Gets the c# variables of a given field declaration.
        /// </summary>
        /// <param name="fieldDeclaration">the field declaration to convert</param>
        /// <returns>the array of c# variables derived from the declaration</returns>
        internal static CSharpVariable[] ToVariables(this FieldDeclarationSyntax fieldDeclaration)
        {
            (CSharpGeneralModifier gModifier, CSharpAccessModifier modifier) = fieldDeclaration.Modifiers.GetConcreteModifier();
            CSharpVariable[] variables = new CSharpVariable[fieldDeclaration.Declaration.Variables.Count];
            CSharpAttribute[] attributes = fieldDeclaration.AttributeLists.ToAttributes();
            CSharpType type = fieldDeclaration.Declaration.Type.ToType();
            for(int i = 0; i < fieldDeclaration.Declaration.Variables.Count; i++)
            {
                variables[i] = new CSharpVariable(fieldDeclaration.Declaration.Variables[i].Identifier.ToString(), 
                    type, attributes, gModifier, modifier);
            }
            
            return variables;
        }


        /// <summary>
        /// Gets the concrete modifier (single/combation) for a given syntax modifier list.
        /// </summary>
        /// <param name="modifierList">the syntax token list that contains the modifiers</param>
        /// <returns>a tuple of whether the static modifier is present, the correct access modifier for a combination of syntax tokens</returns>
        internal static (CSharpGeneralModifier, CSharpAccessModifier) GetConcreteModifier(this SyntaxTokenList modifierList)
        {
            // Detect access modifiers in token list
            bool hasPublic = false;
            bool hasPrivate = false;
            bool hasInternal = false;
            bool hasProtected = false;

            CSharpGeneralModifier generalModifier = CSharpGeneralModifier.None;

            foreach (SyntaxToken token in modifierList)
            {
                if (token.Value != null)
                {
                    if (token.RawKind == (int)SyntaxKind.PublicKeyword)
                    {
                        hasPublic = true;
                    }
                    else if (token.RawKind == (int)SyntaxKind.PrivateKeyword)
                    {
                        hasPrivate = true;
                    }
                    else if (token.RawKind == (int)SyntaxKind.InternalKeyword)
                    {
                        hasInternal = true;
                    }
                    else if (token.RawKind == (int)SyntaxKind.ProtectedKeyword)
                    {
                        hasProtected = true;
                    }
                    else if (token.RawKind == (int)SyntaxKind.AbstractKeyword)
                    {
                        generalModifier |= CSharpGeneralModifier.Abstract;
                    }
                    else if (token.RawKind == (int)SyntaxKind.AsyncKeyword)
                    {
                        generalModifier |= CSharpGeneralModifier.Async;
                    }
                    else if (token.RawKind == (int)SyntaxKind.ConstKeyword)
                    {
                        generalModifier |= CSharpGeneralModifier.Const;
                    }
                    else if (token.RawKind == (int)SyntaxKind.EventKeyword)
                    {
                        generalModifier |= CSharpGeneralModifier.Event;
                    }
                    else if (token.RawKind == (int)SyntaxKind.ExternKeyword)
                    {
                        generalModifier |= CSharpGeneralModifier.Extern;
                    }
                    else if (token.RawKind == (int)SyntaxKind.InKeyword)
                    {
                        generalModifier |= CSharpGeneralModifier.In;
                    }
                    else if (token.RawKind == (int)SyntaxKind.NewKeyword)
                    {
                        generalModifier |= CSharpGeneralModifier.New;
                    }
                    else if (token.RawKind == (int)SyntaxKind.OutKeyword)
                    {
                        generalModifier |= CSharpGeneralModifier.Out;
                    }
                    else if (token.RawKind == (int)SyntaxKind.OverrideKeyword)
                    {
                        generalModifier |= CSharpGeneralModifier.Override;
                    }
                    else if (token.RawKind == (int)SyntaxKind.ReadOnlyKeyword)
                    {
                        generalModifier |= CSharpGeneralModifier.Readonly;
                    }
                    else if (token.RawKind == (int)SyntaxKind.SealedKeyword)
                    {
                        generalModifier |= CSharpGeneralModifier.Sealed;
                    }
                    else if (token.RawKind == (int)SyntaxKind.StaticKeyword)
                    {
                        generalModifier |= CSharpGeneralModifier.Static;
                    }
                    else if (token.RawKind == (int)SyntaxKind.UnsafeKeyword)
                    {
                        generalModifier |= CSharpGeneralModifier.Unsafe;
                    }
                    else if (token.RawKind == (int)SyntaxKind.VirtualKeyword)
                    {
                        generalModifier |= CSharpGeneralModifier.Virtual;
                    }
                    else if (token.RawKind == (int)SyntaxKind.VolatileKeyword)
                    {
                        generalModifier |= CSharpGeneralModifier.Volatile;
                    }
                }
            }

            // Return corresponding modifier
            return (generalModifier,
                hasPublic ? CSharpAccessModifier.Public
                : hasPrivate ? (hasProtected ? CSharpAccessModifier.PrivateProtected : CSharpAccessModifier.Private)
                : hasProtected ? (hasInternal ? CSharpAccessModifier.ProtectedInternal : CSharpAccessModifier.Protected)
                : CSharpAccessModifier.Internal);
        }
    }
}
