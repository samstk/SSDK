﻿using Microsoft.CodeAnalysis.CSharp.Syntax;
using SSDK.CSC.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSDK.CSC.ScriptComponents.Statements
{
    /// <summary>
    /// A c# yield statement.
    /// </summary>
    public sealed class CSharpYieldStatement : CSharpStatement
    {
        #region Properties & Fields
        /// <summary>
        /// Gets the expression holding the label
        /// </summary>
        public CSharpExpression Expression { get; private set; }

        /// <summary>
        /// Gets whether the yield statement is simply a yield break
        /// (i.e. no expression)
        /// </summary>
        public bool IsBreak
        {
            get
            {
                return Expression == null;
            }
        }

        /// <summary>
        /// Gets whether the yield statement is simply a yield return
        /// (i.e. with expression)
        /// </summary>
        public bool IsReturn
        {
            get
            {
                return Expression != null;
            }
        }
        #endregion


        /// <summary>
        /// Creates the yield statement from the given syntax.
        /// </summary>
        /// <param name="syntax">the syntax to create from</param>
        internal CSharpYieldStatement(YieldStatementSyntax syntax)
        {
            if (syntax.Expression != null)
                Expression = syntax.Expression.ToExpression();
            Syntax = syntax;
        }

        public override void ProcessMap(CSharpConversionMap map, StringBuilder result)
        {
            map.ProcessYieldStatement(this, result);
        }

        public override string ToString()
        {
            return $"yield {(IsBreak ? "break": "return")} {Expression};";
        }

        internal override void CreateMemberSymbols(CSharpProject project, CSharpMemberSymbol parentSymbol)
        {
            Symbol = new CSharpMemberSymbol("yield(", parentSymbol, this, false);
            Expression?.CreateMemberSymbols(project, Symbol);
        }

        internal override void ResolveMembers(CSharpProject project)
        {
            Expression?.ResolveMembers(project);
        }
    }
}