using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
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
        /// If true, then this using directive should be used in all scripts.
        /// </summary>
        public bool IsGlobal { get; private set; }
        
        /// <summary>
        /// Gets the syntax that declared this directive.
        /// </summary>
        public new UsingDirectiveSyntax Syntax { get; private set; }

        /// <summary>
        /// Gets the parent namespace that requires this using directive.
        /// </summary>
        public CSharpNamespace ParentNamespace { get; internal set; }
        #endregion

        internal CSharpUsingDirective(UsingDirectiveSyntax syntax, CSharpNamespace parentNamespace)
        {
            Syntax = syntax;
            if (syntax.Alias != null)
                Alias = syntax.Alias.Name.ToString();

            IsGlobal = syntax.GlobalKeyword.RawKind == (int)SyntaxKind.GlobalKeyword;

            Target = syntax.Name.ToString();

            ParentNamespace = parentNamespace;

            IsStatic = syntax.StaticKeyword.Value != null;
        }

        public override void ProcessMap(CSharpConversionMap map, StringBuilder result)
        {
            map.ProcessUsingDirective(this, result);
        }

        internal override void CreateMemberSymbols(CSharpProject project, CSharpMemberSymbol parentSymbol)
        {
            if (Alias != null)
            {
                if (Alias.StartsWith("@"))
                    Alias = Alias.Remove(0, 1);
                Symbol = new CSharpMemberSymbol(Alias, parentSymbol, this);
            } 
            else
            {
                Symbol = new CSharpMemberSymbol("using", parentSymbol, this, false);
            }
        }

        internal override void ResolveMembers(CSharpProject project)
        {
            if (Alias == null)
            {
                CSharpMemberSymbol member = Symbol.FindBestMatchingSymbol(Target, null);
                if (member != null)
                {
                    member.Usages.Add(this);
                    ParentNamespace.Symbol?.LoadedSymbols.AddRange(member.ChildSymbols);
                }
                else
                {
                    project.Errors.Add($"The target of the using directive did not exist in the current context at: {Syntax.GetLocation()}");
                }
                Symbol = member;
            }
            else
            {
                Symbol.AliasTo = Symbol.FindBestMatchingSymbol(Target, null);
                ParentNamespace.Symbol?.LoadedSymbols.Add(Symbol);
            }
        }

        public override string ToString()
        {
            return Alias != null ? $"using {Alias} = {Target}"
                : $"using {(IsStatic ? "static " : "")}{Target}";
        }
    }
}
