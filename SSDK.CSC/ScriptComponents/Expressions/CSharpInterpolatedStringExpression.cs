using Microsoft.CodeAnalysis.CSharp;
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
    /// A c# primitive literal value (such as string)
    /// </summary>
    public sealed class CSharpInterpolatedStringExpression : CSharpExpression
    {
        #region Properties & Fields
        /// <summary>
        /// Gets the contents of this interpolated string.
        /// </summary>
        public CSharpExpression[] Contents { get; private set; }

        #endregion
        /// <summary>
        /// Creates the interpolated string from the given syntax
        /// </summary>
        /// <param name="syntax">the syntax to create from</param>
        internal CSharpInterpolatedStringExpression(InterpolatedStringExpressionSyntax syntax)
        {
            Contents = new CSharpExpression[syntax.Contents.Count];
            for(int i = 0; i < Contents.Length; i++)
            {
                object content = syntax.Contents[i];
                if (content is InterpolatedStringTextSyntax)
                {
                    Contents[i] = new CSharpLiteralValueExpression(((InterpolatedStringTextSyntax)content));
                }
                else if (content is InterpolationSyntax)
                {
                    Contents[i] = ((InterpolationSyntax)content).Expression.ToExpression();
                }
            }
            Syntax = syntax;
        }

        public override void ProcessMap(CSharpConversionMap map, StringBuilder result)
        {
            map.ProcessInterpolatedStringExpression(this, result);
        }

        public override string ToString()
        {
            return $"$\"...\"";
        }
    }
}
