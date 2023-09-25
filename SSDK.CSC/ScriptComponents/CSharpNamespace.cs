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
    /// A C# namespace, which contain numerous classes, structures, and enums for compilation.
    /// </summary>
    public sealed class CSharpNamespace
    {
        #region Properties & Fields
        /// <summary>
        /// Gets the name of this namespace.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the using directives of this namespace.
        /// </summary>
        public CSharpUsingDirective[] UsingDirectives { get; private set; }

        /// <summary>
        /// Gets the parent of this namespace.
        /// </summary>
        public CSharpNamespace Parent { get; private set; }

        /// <summary>
        /// Contains all child namespaces that may contain code.
        /// </summary>
        public CSharpNamespace[] Namespaces { get; private set; }

        /// <summary>
        /// Gets the child classes that may be referenced within the project under this namespace.
        /// </summary>
        public CSharpClass[] Classes { get; private set; }

        /// <summary>
        /// Gets the child structs that may be referenced within the project under this namespace.
        /// </summary>
        public CSharpStruct[] Structures { get; private set; }

        /// <summary>
        /// Gets the child enums that may be referenced within the project under this namespace.
        /// </summary>
        public CSharpEnum[] Enums { get; private set; }

        /// <summary>
        /// Gets all delegate declarations in this class.
        /// </summary>
        public CSharpDelegate[] Delegates { get; private set; }

        /// <summary>
        /// Gets the compilation of this namespace. If this namespace is not root, then
        /// it will not have one.
        /// </summary>
        public CompilationUnitSyntax CompilationUnit { get; private set; }  
        #endregion

        /// <summary>
        /// Creates a new root namespace with the given compilation unit
        /// </summary>
        /// <param name="unitSyntax">the compilation unit</param>
        internal CSharpNamespace(CompilationUnitSyntax unitSyntax)
        {
            // Convert using directives of 
            List<CSharpUsingDirective> usingDirectives = new List<CSharpUsingDirective>();
            foreach(UsingDirectiveSyntax usingDirective in unitSyntax.Usings)
            {
                usingDirectives.Add(new CSharpUsingDirective(usingDirective));
            }
            UsingDirectives = usingDirectives.ToArray();

            // Convert members
            List<CSharpDelegate> delegates = new List<CSharpDelegate>();
            
            foreach(MemberDeclarationSyntax member in unitSyntax.Members)
            {
                if (member is DelegateDeclarationSyntax)
                {
                    delegates.Add(new CSharpDelegate((DelegateDeclarationSyntax)member));
                }
            }

            Delegates = delegates.ToArray();
        }
    }
}