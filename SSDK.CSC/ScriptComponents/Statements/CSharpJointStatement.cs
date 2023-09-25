﻿using Microsoft.CodeAnalysis.CSharp.Syntax;
using SSDK.CSC.Helpers;
using SSDK.CSC.ScriptComponents.Statements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSDK.CSC.ScriptComponents
{
    /// <summary>
    /// A c# joint statement, which contains a number of statements in order (e.g. multiple variable declarations)
    /// </summary>
    public sealed class CSharpJointStatement : CSharpStatement
    {
        #region Properties & Fields
        /// <summary>
        /// Gets the statements made in the block in sequence.
        /// </summary>
        public CSharpStatement[] Statements { get; private set; }
        #endregion

        /// <summary>
        /// Creates a new statement block from the given block syntax
        /// </summary>
        /// <param name="syntax">the block syntax</param>
        internal CSharpJointStatement(CSharpStatement[] statements)
        {
            Statements = statements;
            for(int i = 0; i< Statements.Length; i++)
            {
                if (Statements[i] is CSharpVariable)
                {
                    ((CSharpVariable)Statements[i]).InStatement = true;
                }
            }
        }

        internal CSharpJointStatement() { }

        public override void ProcessMap(CSharpConversionMap map, StringBuilder result)
        {
            map.ProcessJointStatement(this, result);
        }

        public override string ToString()
        {
            string result = "";

            foreach(CSharpStatement statement in Statements)
            {
                result += statement.ToString() + " ";
            }

            return result;
        }
    }
}
