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
    /// A single C# script, which may be a part of a CSharpProject to produce some sort of output.
    /// </summary>
    public sealed class CSharpScript
    {
        #region Properties & Fields
        /// <summary>
        /// Gets the full file path that this script was loaded into.
        /// May be null if the script in the constructor did not indicate a file path.
        /// </summary>
        public string File { get; private set; }

        /// <summary>
        /// Gets the name of the script (i.e. its file name - e.g. Program.cs)
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the syntax tree which was compiled from this code.
        /// </summary>
        public SyntaxTree Syntax { get; private set; }

        /// <summary>
        /// Gets the root namespace of this script.
        /// </summary>
        public CSharpNamespace RootNamespace { get; private set; }
        #endregion

        /// <summary>
        /// Creates a new C# script, by converting the input to an Abstract Syntax Tree (roselyn's SyntaxTree),
        /// and then to local components.
        /// </summary>
        /// <param name="script">the script (or file)</param>
        /// <param name="isFile">if true, then the script parameter is read as the file to read</param>
        /// <param name="immediateCompilation">if true, then the script will attempt to compile references immediately</param>
        /// <remarks>
        /// when using multiple scripts (in majority of cases), using CSharpProject.
        /// </remarks>
        public CSharpScript(string script, bool isFile=false, bool immediateCompilation=true)
        {
            // Get script contents if file and update local references
            string filePath = "<runtime>";
            if (isFile)
            {
                FileInfo fileInfo = new FileInfo(script);
                File = fileInfo.FullName;
                Name = fileInfo.Name;
                filePath = script;
                script = System.IO.File.ReadAllText(script);
            }

            // Create syntax tree
            Syntax = SyntaxFactory.ParseSyntaxTree(script).WithFilePath(filePath);
            RootNamespace = new CSharpNamespace((CompilationUnitSyntax)Syntax.GetRoot());
        }

        /// <summary>
        /// Correlates all identifiers / members of this script's
        /// expressions to a created member.
        /// </summary>
        /// <remarks>
        /// Assumes CreateMemberSymbols is called first on every script.
        /// </remarks>
        internal void ResolveMembers(CSharpProject project)
        {
            RootNamespace?.ResolveMembers(project);
        }

        /// <summary>
        /// Creates new member symbols for the scripts.
        /// </summary>
        internal void CreateMemberSymbols(CSharpProject project)
        {
            RootNamespace?.CreateMemberSymbols(project, null);
        }

        /// <summary>
        /// Merges all member symbols with the same full name (e.g. System.Text),
        /// if it is a mergable component (i.e. a namespace, or a struct/class/enum with partial on it)
        /// </summary>
        /// <param name="mappings">a mapping of full names to member symbols</param>
        internal void MergeMemberSymbols(Dictionary<string, CSharpMemberSymbol> mappings)
        {
            RootNamespace?.Symbol?.MergeMemberSymbols(mappings);
        }

        /// <summary>
        /// Inherit object members on all classes
        /// </summary>
        /// <param name="objectSymbol">the symbol to inherit all children of</param>
        internal void InheritObjectSymbol(CSharpMemberSymbol objectSymbol)
        {
            RootNamespace?.Symbol?.InheritObjectMembers(objectSymbol);
        }

        /// <summary>
        /// Loads all base classes to inheriting classes.
        /// </summary>
        /// <param name="project">the project to report to</param>
        internal void LoadInheritance(CSharpProject project)
        {
            RootNamespace?.LoadInheritance(project);
        }
    }
}
