using Microsoft.CodeAnalysis.CSharp.Syntax;
using SSDK.CSC.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSDK.CSC.ScriptComponents.Statements
{
    /// <summary>
    /// A c# label statement for a given label.
    /// </summary>
    public sealed class CSharpLabelStatement : CSharpStatement
    {
        #region Properties & Fields
        /// <summary>
        /// Gets the name of the label
        /// </summary>
        public string Name { get; set; }
        #endregion


        /// <summary>
        /// Creates the label statement from the given syntax.
        /// </summary>
        /// <param name="syntax">the syntax to create from</param>
        internal CSharpLabelStatement(LabeledStatementSyntax syntax)
        {
            Name = syntax.Identifier.ToString();
            Syntax = syntax;
        }

        public override void ProcessMap(CSharpConversionMap map, StringBuilder result)
        {
            map.ProcessLabelStatement(this, result);
        }

        public override string ToString()
        {
            return $"{Name}:";
        }
    }
}
