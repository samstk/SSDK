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
    /// A C# pattern expression (e.g. > 3).
    /// </summary>
    /// <remarks>
    /// No syntax is stored for this expression
    /// </remarks>
    public sealed class CSharpPatternExpression : CSharpExpression
    {
        #region Properties & Fields
        /// <summary>
        /// Gets the pattern syntax that formed this pattern expression
        /// </summary>
        public PatternSyntax PatternSyntax { get; private set; }

        /// <summary>
        /// Gets the type of this pattern
        /// </summary>
        public CSharpPatternType PatternType { get; private set; }
        
        /// <summary>
        /// Gets the left-hand side of a binary pattern
        /// </summary>
        public CSharpExpression BinaryLeftPattern { get; private set; }

        /// <summary>
        /// Gets the right-hand side of a binary pattern
        /// </summary>
        public CSharpExpression BinaryRightPattern { get; private set; }

        /// <summary>
        /// Gets the expression of a unary (single argument) pattern
        /// </summary>
        public CSharpExpression UnaryExpression { get; private set; }
        #endregion

        internal CSharpPatternExpression()
        {

        }

        /// <summary>
        /// Creates the pattern expression from the syntax.
        /// </summary>
        /// <param name="syntax">the syntax to create from</param>
        internal CSharpPatternExpression(PatternSyntax syntax)
        {
            PatternSyntax = syntax;
            if (syntax is BinaryPatternSyntax)
            {
                BinaryPatternSyntax biPattern = syntax as BinaryPatternSyntax;
                if (syntax.RawKind == (int)SyntaxKind.OrPattern)
                {
                    PatternType = CSharpPatternType.BinaryOr;
                }
                else if (syntax.RawKind == (int)SyntaxKind.AndPattern)
                {
                    PatternType = CSharpPatternType.BinaryAnd;
                }
                BinaryLeftPattern = new CSharpPatternExpression(biPattern.Left);
                BinaryRightPattern = new CSharpPatternExpression(biPattern.Right);
            }
            else if (syntax is ConstantPatternSyntax)
            {
                ConstantPatternSyntax constant = syntax as ConstantPatternSyntax;
                PatternType = CSharpPatternType.Constant;
                UnaryExpression = constant.Expression.ToExpression();
            }
            else if (syntax is DiscardPatternSyntax)
            {
                DiscardPatternSyntax discard = syntax as DiscardPatternSyntax;
                PatternType = CSharpPatternType.Discard;
            }
            else throw new Exception();
        }

        public override void ProcessMap(CSharpConversionMap map, StringBuilder result)
        {
            map.ProcessPatternExpression(this, result);
        }

        internal override void CreateMemberSymbols(CSharpProject project, CSharpMemberSymbol parentSymbol)
        {
            Symbol = new CSharpMemberSymbol("pattern", parentSymbol, this, false);
            BinaryLeftPattern?.CreateMemberSymbols(project, Symbol);
            BinaryRightPattern?.CreateMemberSymbols(project, Symbol);
            UnaryExpression?.CreateMemberSymbols(project, Symbol);
        }

        internal override void ResolveMembers(CSharpProject project)
        {
            BinaryLeftPattern?.ResolveMembers(project);
            BinaryRightPattern?.ResolveMembers(project);
            UnaryExpression?.ResolveMembers(project);
        }

        public override string ToString()
        {
            return $"<pattern>";
        }
    }

    /// <summary>
    /// Represents a type of pattern.
    /// </summary>
    public enum CSharpPatternType
    {
        /// <summary>
        /// Indicates an invalid pattern type
        /// </summary>
        Unknown,
        /// <summary>
        /// Indicates a constant which uses the unary expression
        /// </summary>
        Constant,
        /// <summary>
        /// Indicates that the pattern is in form of 
        /// x or y, which uses binary left and binary right expressions.
        /// </summary>
        BinaryOr,
        /// <summary>
        /// Indicates that the pattern is in form of 
        /// x and y, which uses binary left and binary right expressions.
        /// </summary>
        BinaryAnd,
        /// <summary>
        /// Indicates that the pattern is simply _,
        /// </summary>
        Discard
    }
}
