﻿using Microsoft.CodeAnalysis.CSharp.Syntax;
using SSDK.CSC.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSDK.CSC.ScriptComponents.Expressions
{
    /// <summary>
    /// A C# identifier expression (e.g. x)
    /// </summary>
    public sealed class CSharpIdentifierExpression : CSharpExpression
    {
        #region Properties & Fields
        /// <summary>
        /// Gets the name to be identified.
        /// </summary>
        public string Name { get; private set; }
        #endregion

        /// <summary>
        /// Creates the identifier expression from the syntax.
        /// </summary>
        /// <param name="syntax">the syntax to create from</param>
        internal CSharpIdentifierExpression(IdentifierNameSyntax syntax)
        {
            Syntax = syntax;
            Name = syntax.Identifier.ValueText;
        }

        public override void ProcessMap(CSharpConversionMap map, StringBuilder result)
        {
            map.ProcessIdentifierExpression(this, result);
        }

        internal override void CreateMemberSymbols(CSharpProject project, CSharpMemberSymbol parentSymbol)
        {
            Symbol = new CSharpMemberSymbol("idref", parentSymbol, this, false);
        }

        internal override void ResolveMembers(CSharpProject project)
        {
            Symbol = Symbol.FindBestMatchingSymbol(Name, null);
        }

        internal override CSharpType GetComponentType(CSharpProject project)
        {
            return Symbol.GetComponentType(project);
        }
        public override string ToString()
        {
            return $"{Name}";
        }
    }
}
