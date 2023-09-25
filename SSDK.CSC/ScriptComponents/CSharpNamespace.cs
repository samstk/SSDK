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
    /// A C# namespace, which contain numerous classes, structures, and enums for compilation.
    /// </summary>
    public sealed class CSharpNamespace : CSharpComponent
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
        public CSharpStruct[] Structs { get; private set; }

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
        public CompilationUnitSyntax CompilationUnitSyntax { get; private set; }

        /// <summary>
        /// Gets the namespace syntax. If this syntax is root, then it will
        /// not have one.
        /// </summary>
        public NamespaceDeclarationSyntax Syntax { get; private set; }
        #endregion

        /// <summary>
        /// Creates a new root namespace with the given compilation unit
        /// </summary>
        /// <param name="unitSyntax">the compilation unit</param>
        internal CSharpNamespace(CompilationUnitSyntax unitSyntax)
        {
            CompilationUnitSyntax = unitSyntax;
            AddUsings(unitSyntax.Usings);
            AddMembers(unitSyntax.Members);
        }

        internal CSharpNamespace(NamespaceDeclarationSyntax namespaceSyntax)
        {
            Syntax = namespaceSyntax;
            Name = namespaceSyntax.Name.ToString();
            AddUsings(namespaceSyntax.Usings);
            AddMembers(namespaceSyntax.Members);
        }

        /// <summary>
        /// Converts an array syntax
        /// </summary>
        /// <param name="usings"></param>
        /// <returns></returns>
        internal void AddUsings(SyntaxList<UsingDirectiveSyntax> usings)
        {
            UsingDirectives = new CSharpUsingDirective[usings.Count];
            for (int i = 0; i < usings.Count; i++)
            {
                UsingDirectives[i] = new CSharpUsingDirective(usings[i]);
            }
        }

        internal void AddMembers(SyntaxList<MemberDeclarationSyntax> members){
            // Convert members
            List<CSharpDelegate> delegates = new List<CSharpDelegate>();
            List<CSharpNamespace> namespaces = new List<CSharpNamespace>();
            List<CSharpClass> classes = new List<CSharpClass>();
            List<CSharpStruct> structs = new List<CSharpStruct>();
            foreach (MemberDeclarationSyntax member in members)
            {
                if (member is DelegateDeclarationSyntax)
                {
                    delegates.Add(new CSharpDelegate((DelegateDeclarationSyntax)member));
                }
                else if (member is NamespaceDeclarationSyntax)
                {
                    namespaces.Add(new CSharpNamespace((NamespaceDeclarationSyntax)member));
                }
                else if (member is ClassDeclarationSyntax)
                {
                    classes.Add(new CSharpClass((ClassDeclarationSyntax)member));
                }
                else if (member is StructDeclarationSyntax)
                {
                    structs.Add(new CSharpStruct((StructDeclarationSyntax)member));
                }
            }

            Delegates = delegates.ToArray();
            Namespaces = namespaces.ToArray();
            Classes = classes.ToArray();
            Structs = structs.ToArray();
        }

        public override string ToString()
        {
            if (Name == null)
                return "root namespace";
            return $"namespace {Name}";
        }
    }
}