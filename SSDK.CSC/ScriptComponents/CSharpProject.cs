using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using SSDK.CSC.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSDK.CSC.ScriptComponents
{
    /// <summary>
    /// A C# project, which contains information and methods relating to an entire c# project
    /// (i.e. a number of c# script within a directory, which are to be joined together during compilation) 
    /// </summary>
    public sealed class CSharpProject
    {
        #region Properties & Fields
        /// <summary>
        /// Gets all scripts that this project is made of.
        /// </summary>
        public CSharpScript[] Scripts { get; private set; }

        /// <summary>
        /// Gets all root symbols that this project is made up of.
        /// (e.g. namespaces)
        /// </summary>
        public CSharpMemberSymbol[] RootSymbols { get; private set; }

        /// <summary>
        /// Gets all global using directives used in the root-level of each script, which
        /// should be applied to all scripts.
        /// </summary>
        public CSharpUsingDirective[] GlobalUsingDirectives { get; private set; }

        /// <summary>
        /// Gets all errors that resulted from either a conversion map, or
        /// resolving all member accesses.
        /// </summary>
        public List<string> Errors { get; private set; } = new List<string>();
        #endregion

        /// <summary>
        /// Creates a new C# project, by converting the inputs to an Abstract Syntax Tree (roselyn's SyntaxTree),
        /// and then to local components.
        /// </summary>
        /// <param name="script">the scripts (or files)</param>
        /// <param name="scriptsAreFiles">if true, then the script parameter is read as the file to read</param>
        public CSharpProject(string[] scripts, bool scriptsAreFiles = false)
        {
            // Create scripts and references
            Scripts = new CSharpScript[scripts.Length];

            List<CSharpUsingDirective> usingDirectives = new List<CSharpUsingDirective>();
            // Anonymous functions for populating lists.
            for (int i = 0; i < scripts.Length; i++)
            {
                CSharpScript script = new CSharpScript(scripts[i], scriptsAreFiles, false);
                foreach(CSharpUsingDirective @using in script.RootNamespace.UsingDirectives)
                {
                    if(@using.IsGlobal)
                    {
                        usingDirectives.Add(@using);
                    }
                }
                Scripts[i] = script;
            }

            GlobalUsingDirectives = usingDirectives.ToArray();

            ResolveMembers();
        }

        /// <summary>
        /// Resolves all member accesses of this project, and
        /// correlates all simple identifiers with symbols.
        /// </summary>
        /// <remarks>
        /// Adds to the errors list for every detected error.
        /// </remarks>
        internal void ResolveMembers()
        {
            Dictionary<string, CSharpMemberSymbol> symbolMappings = new Dictionary<string, CSharpMemberSymbol>();

            foreach(CSharpUsingDirective @using in GlobalUsingDirectives)
            {
                @using.CreateMemberSymbols(this, null);
            }

            // Step one: Create symbols for every component
            foreach(CSharpScript script in Scripts)
            {
                script.CreateMemberSymbols(this);
            }

            foreach (CSharpUsingDirective @using in GlobalUsingDirectives)
            {
                CSharpNamespace oldNamespace = @using.ParentNamespace;
                foreach (CSharpScript script in Scripts)
                {
                    @using.ParentNamespace = script.RootNamespace;
                    @using.ResolveMembers(this);
                }
                @using.ParentNamespace = oldNamespace;
            }
            
            // Step two: Merge symbols that are the same
            // (e.g. same identifier namespaces - System & System)
            foreach (CSharpScript script in Scripts)
            {
                script.MergeMemberSymbols(symbolMappings);
            }

            CSharpMemberSymbol objectSymbol = FindSymbol("object");
            if(objectSymbol != null)
            {
                foreach(CSharpScript script in Scripts)
                {
                    script.InheritObjectSymbol(objectSymbol);
                    script.LoadInheritance(this);
                }
            }

            // Step three: Correlate symbols for every usage.
            foreach (CSharpScript script in Scripts)
            {
                script.ResolveMembers(this);
            }

            // Step four: update project for other development use.
            HashSet<CSharpMemberSymbol> rootSymbols = new HashSet<CSharpMemberSymbol>();
            foreach (CSharpScript script in Scripts)
            {
                foreach(CSharpMemberSymbol sym in script.RootNamespace.Symbol.ChildSymbols)
                {
                    rootSymbols.Add(sym);
                }
            }
            RootSymbols = rootSymbols.ToArray();
        }

        /// <summary>
        /// Finds the given symbol using the root context(s) - fully qualified name
        /// (e.g. System.Console)
        /// </summary>
        /// <param name="target">the fully qualified name of the target</param>
        /// <returns>the symbol representing the target</returns>
        /// <remarks>
        /// Stops at the first matching symbol.
        /// </remarks>
        internal CSharpMemberSymbol FindSymbol(string target)
        {
            foreach (CSharpScript script in Scripts)
            {
                CSharpMemberSymbol result = script.RootNamespace.Symbol.FindBestMatchingSymbol(target.GetTargetSections(), 0, 2);
                if (result != null)
                    return result;
            }
            return null;
        }
    }
}
