using SSDK.CSC.ScriptComponents;
using SSDK.CSC;
using System.Text;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SSDK.CSC.Helpers;

namespace SSDK.CSC.ScriptComponents.Statements
{
    /// <summary>
    /// A c# try statement for a given block.
    /// </summary>
    public sealed class CSharpTryStatement : CSharpStatement
    {
        #region Properties & Fields

        /// <summary>
        /// Gets the execution block of this try statement.
        /// </summary>
        public CSharpStatementBlock Block { get; private set; }

        /// <summary>
        /// Gets all the catch cases of this try statement
        /// </summary>
        public CSharpTryCatch[] Catches { get; private set; }

        /// <summary>
        /// Gets the finally block of this try statement.
        /// </summary>
        public CSharpStatementBlock FinalBlock { get; private set; }
        #endregion

        /// <summary>
        /// Creates the try statement from the given syntax.
        /// </summary>
        /// <param name="syntax">the syntax to create from</param>
        internal CSharpTryStatement(TryStatementSyntax syntax)
        {
            Block = new CSharpStatementBlock(syntax.Block);
            Catches = new CSharpTryCatch[syntax.Catches.Count];
            for(int i = 0; i<Catches.Length; i++)
            {
                Catches[i] = new CSharpTryCatch(syntax.Catches[i]);
            }
            if(syntax.Finally != null)
            {
                FinalBlock = new CSharpStatementBlock(syntax.Finally.Block);
            }
            Syntax = syntax;
        }

        public override void ProcessMap(CSharpConversionMap map, StringBuilder result)
        {
            map.ProcessTryStatement(this, result);
        }

        public override string ToString()
        {
            return $"try ... catch ...";
        }
    }
}