using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SSDK.CSC.ScriptComponents;
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
        /// Gets the list of type param names in the given syntax
        /// </summary>
        /// <param name="parameterList">the parameter syntax</param>
        /// <returns>an array of type param names</returns>
        internal static string[] GetTypeParams(this TypeParameterListSyntax parameterList)
        {
            return parameterList.Parameters.Select((typeparam) => typeparam.Identifier.ToString()).ToArray();
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
