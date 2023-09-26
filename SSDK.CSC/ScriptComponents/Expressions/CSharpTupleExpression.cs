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
    /// A C# tuple expression (e.g. (x,y))
    /// </summary>
    public sealed class CSharpTupleExpression : CSharpExpression
    {
        #region Properties & Fields
        /// <summary>
        /// Gets the elements to be used in this tuple.
        /// May have null elements where designated.
        /// </summary>
        public CSharpExpression[] TupleElements { get; private set; }

        /// <summary>
        /// Gets all designations of this tuple.
        /// May be null where no designation occured (e.g. x in (x, int y))
        /// </summary>
        public CSharpVariable[] Designations { get; private set; }
        #endregion

        /// <summary>
        /// Creates the tuple expression from the syntax.
        /// </summary>
        /// <param name="syntax">the syntax to create from</param>
        internal CSharpTupleExpression(TupleExpressionSyntax syntax)
        {
            Syntax = syntax;
            TupleElements = new CSharpExpression[syntax.Arguments.Count];
            Designations = new CSharpVariable[syntax.Arguments.Count];

            for(int i = 0; i <syntax.Arguments.Count; i++)
            {
                ArgumentSyntax arg = syntax.Arguments[i];

                if(arg.Expression is DeclarationExpressionSyntax)
                {
                    DeclarationExpressionSyntax decl = arg.Expression as DeclarationExpressionSyntax;
                    CSharpType type = decl.Type.ToType();
                    string name = "unknown";
                    if (decl.Designation is SingleVariableDesignationSyntax)
                    {
                        name = ((decl.Designation as SingleVariableDesignationSyntax).Identifier.ToString());
                    }
                    else throw new Exception("Unhandled case");
                    Designations[i] = new CSharpVariable(name, type, CSharpAttribute.Empty, CSharpGeneralModifier.None, CSharpAccessModifier.DefaultOrNone, null);
                }
                else
                {
                    TupleElements[i] = arg.Expression.ToExpression();
                }
            }
        }

        public override void ProcessMap(CSharpConversionMap map, StringBuilder result)
        {
            map.ProcessTupleExpression(this, result);
        }

        public override string ToString()
        {
            return $"({TupleElements.ToReadableString()})";
        }
    }
}
