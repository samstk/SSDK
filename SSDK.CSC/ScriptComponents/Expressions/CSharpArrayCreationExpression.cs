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
    /// A C# ArrayCreation expression (e.g. new Dict(true))
    /// </summary>
    public sealed class CSharpArrayCreationExpression : CSharpExpression
    {
        #region Properties & Fields
        /// <summary>
        /// Gets the type of this expression
        /// indicating the class/struct target.
        /// </summary>
        public CSharpType Type { get; private set; }

        /// <summary>
        /// Gets the arguments to be used in this array, i.e. the rank specifiers.
        /// Some rank sizes may be left as null (e.g. new int[] still has one rank, one size)
        /// </summary>
        public CSharpExpression[][] Ranks { get; private set; }

        /// <summary>
        /// Gets the initial elements of the array.
        /// </summary>
        public CSharpExpression[] Initializer { get; private set; }
        #endregion

        /// <summary>
        /// Creates the ArrayCreation expression from the syntax.
        /// </summary>
        /// <param name="syntax">the syntax to create from</param>
        internal CSharpArrayCreationExpression(ArrayCreationExpressionSyntax syntax)
        {
            Syntax = syntax;
            Type = syntax.Type.ToType();
            Ranks = new CSharpExpression[syntax.Type.RankSpecifiers.Count][];
            for(int i = 0; i < syntax.Type.RankSpecifiers.Count; i++)
            {
                ArrayRankSpecifierSyntax rankSyntax = syntax.Type.RankSpecifiers[i];
                CSharpExpression[] sizes = new CSharpExpression[rankSyntax.Sizes.Count];
                for(int x = 0; x < sizes.Length; x++)
                {
                    if (rankSyntax.Sizes[x] is OmittedArraySizeExpressionSyntax)
                    {
                        // Do nothing (leave as null)
                    }
                    else sizes[x] = rankSyntax.Sizes[x].ToExpression();
                }
                Ranks[i] = sizes;
            }
            Initializer = CSharpExpression.Empty;
            if (syntax.Initializer != null)
                Initializer = syntax.Initializer.Expressions.ToExpressions();
        }

        public override void ProcessMap(CSharpConversionMap map, StringBuilder result)
        {
            map.ProcessArrayCreationExpression(this, result);
        }

        public override string ToString()
        {
            return $"new {Type}[] " + "{ " + Initializer.ToReadableString() + " }";
        }
    }
}
