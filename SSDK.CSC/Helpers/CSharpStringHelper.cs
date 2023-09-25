using SSDK.CSC.ScriptComponents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSDK.CSC.Helpers
{
    /// <summary>
    /// Helper methods for string representations of c# components.
    /// </summary>
    internal static class CSharpStringHelper
    {
        /// <summary>
        /// Gets a readable string representation of a number of variables.
        /// </summary>
        /// <param name="variables">the array of variables</param>
        /// <returns>the string representation of variables</returns>
        public static string ToReadableString(this CSharpVariable[] variables)
        {
            StringBuilder result = new StringBuilder();

            foreach(CSharpVariable variable in variables)
            {
                if (result.Length > 0)
                    result.Append(", ");
                result.Append(variable.ToString());
            }

            return result.ToString();
        }

        /// <summary>
        /// Converts the access modifier to a readable prefix string
        /// </summary>
        /// <param name="modifier">the modifier to convert</param>
        /// <returns>the access modifier as a string</returns>
        public static string ToReadablePrefix(this CSharpAccessModifier modifier)
        {
            switch (modifier)
            {
                case CSharpAccessModifier.Internal: return "internal";
                case CSharpAccessModifier.Public: return "public";
                case CSharpAccessModifier.Protected: return "protected";
                case CSharpAccessModifier.Private: return "private";
                case CSharpAccessModifier.PrivateProtected: return "private protected";
                case CSharpAccessModifier.ProtectedInternal: return "protected internal";
                default: return "<unknown access modifier>";
            }
        }

        /// <summary>
        /// Converts the general modifier to a readable prefix string (including trailing whitespace)
        /// </summary>
        /// <param name="modifier">the general to convert</param>
        /// <returns>the general modifier as a string</returns>
        public static string ToReadablePrefix(this CSharpGeneralModifier modifier)
        {
            string prefix = "";
            if (modifier.HasFlag(CSharpGeneralModifier.Abstract))
                prefix += "abstract ";
            if (modifier.HasFlag(CSharpGeneralModifier.Async))
                prefix += "async ";
            if (modifier.HasFlag(CSharpGeneralModifier.Const))
                prefix += "const ";
            if (modifier.HasFlag(CSharpGeneralModifier.Event))
                prefix += "event ";
            if (modifier.HasFlag(CSharpGeneralModifier.Extern))
                prefix += "extern ";
            if (modifier.HasFlag(CSharpGeneralModifier.In))
                prefix += "in ";
            if (modifier.HasFlag(CSharpGeneralModifier.New))
                prefix += "new ";
            if (modifier.HasFlag(CSharpGeneralModifier.Out))
                prefix += "out ";
            if (modifier.HasFlag(CSharpGeneralModifier.Override))
                prefix += "override ";
            if (modifier.HasFlag(CSharpGeneralModifier.Readonly))
                prefix += "readonly ";
            if (modifier.HasFlag(CSharpGeneralModifier.Sealed))
                prefix += "sealed ";
            if (modifier.HasFlag(CSharpGeneralModifier.Static))
                prefix += "static ";
            if (modifier.HasFlag(CSharpGeneralModifier.Unsafe))
                prefix += "unsafe ";
            if (modifier.HasFlag(CSharpGeneralModifier.Virtual))
                prefix += "virtual ";
            if (modifier.HasFlag(CSharpGeneralModifier.Volatile))
                prefix += "volatile ";

            return prefix;
        }

        /// <summary>
        /// Converts the attributes to a readable prefix (including trailing whitespace)
        /// </summary>
        /// <param name="attributes">the attributes to convert</param>
        /// <returns>the attributes as a string</returns>
        public static string ToReadablePrefix(this CSharpAttribute[] attributes)
        {
            string result = "";

            foreach (CSharpAttribute attribute in attributes)
            {
                if (result.Length > 0)
                    result += ", ";
                result += attribute.ToString();
            }

            return result + (result.Length > 0 ? " " : "");
        }

        /// <summary>
        /// Converts the expressions to a readable string, seperated by commas
        /// </summary>
        /// <param name="expressions">the expressions to convert</param>
        /// <returns>the expressions as a string</returns>
        public static string ToReadableString(this CSharpExpression[] expressions)
        {
            string result = "";
            foreach(CSharpExpression expr in expressions)
            {
                if (result.Length > 0)
                    result += ",";
                if (expr != null)
                    result += expr.ToString();
            }
            return result;
        }
    }
}
