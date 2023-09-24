using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSDK.CSC.ScriptComponents
{
    /// <summary>
    /// A c# using directive.
    /// <br/>
    /// e.g. using static System.Console;
    /// <br/> 
    /// e.g. using WriteLn = System.Console.WriteLine;
    /// </summary>
    public sealed class CSharpUsingDirective : CSharpStatement
    {
        #region Properties & Fields
        /// <summary>
        /// Gets the alias of this directive. If set, then indicates
        /// the target should be accessible via this name,
        /// </summary>
        public string Alias { get; private set; }

        /// <summary>
        /// Gets the target context name in this using directive.
        /// </summary>
        public string Target { get; private set; }

        /// <summary>
        /// If true, then this using directive has the static modifier on it.
        /// </summary>
        public bool IsStatic { get; private set; }
        
        /// <summary>
        /// Gets the syntax that declared this directive.
        /// </summary>
        public new UsingDirectiveSyntax Syntax { get; private set; }
        #endregion

        internal CSharpUsingDirective(UsingDirectiveSyntax syntax)
        {
            Syntax = syntax;
            
            if (syntax.Alias != null)
                Alias = syntax.Alias.Name.ToString();
            
            Target = syntax.Name.ToString();

            IsStatic = syntax.StaticKeyword.Value != null;
        }

        public override string ToString()
        {
            return Alias != null ? $"using {Alias} = {Target}"
                : $"using {(IsStatic ? "static " : "")}{Target}";
        }
    }
}
