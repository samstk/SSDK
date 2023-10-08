using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SSDK.CSC.Helpers;
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
        /// Gets the symbol that represents this component.
        /// </summary>
        /// <remarks>
        /// ResolveMembers must be called on the project before being set.
        /// </remarks>
        public CSharpMemberSymbol Symbol { get; private set; }

        /// <summary>
        /// Gets the name of this namespace.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the attributes of the namespace.
        /// </summary>
        public CSharpAttribute[] Attributes { get; private set; }

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
            Attributes = unitSyntax.AttributeLists.ToAttributes();
            AddUsings(unitSyntax.Usings);
            AddMembers(unitSyntax.Members);
        }

        internal CSharpNamespace(NamespaceDeclarationSyntax namespaceSyntax)
        {
            Attributes = namespaceSyntax.AttributeLists.ToAttributes();
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
                UsingDirectives[i] = new CSharpUsingDirective(usings[i], this);
            }
        }

        internal void AddMembers(SyntaxList<MemberDeclarationSyntax> members){
            // Convert members
            List<CSharpDelegate> delegates = new List<CSharpDelegate>();
            List<CSharpNamespace> namespaces = new List<CSharpNamespace>();
            List<CSharpClass> classes = new List<CSharpClass>();
            List<CSharpStruct> structs = new List<CSharpStruct>();
            List<CSharpEnum> enums = new List<CSharpEnum>();
            foreach (MemberDeclarationSyntax member in members)
            {
                if (member is DelegateDeclarationSyntax)
                {
                    delegates.Add(new CSharpDelegate((DelegateDeclarationSyntax)member));
                }
                else if (member is NamespaceDeclarationSyntax)
                {
                    namespaces.Add(new CSharpNamespace((NamespaceDeclarationSyntax)member) { Parent = this });
                }
                else if (member is ClassDeclarationSyntax)
                {
                    classes.Add(new CSharpClass((ClassDeclarationSyntax)member));
                }
                else if (member is StructDeclarationSyntax)
                {
                    structs.Add(new CSharpStruct((StructDeclarationSyntax)member));
                }
                else if (member is EnumDeclarationSyntax)
                {
                    enums.Add(new CSharpEnum((EnumDeclarationSyntax)member));
                }
            }

            Delegates = delegates.ToArray();
            Namespaces = namespaces.ToArray();
            Classes = classes.ToArray();
            Structs = structs.ToArray();
            Enums = enums.ToArray();
        }

        /// <summary>
        /// Creates new member symbols for the members of the namespace,
        /// and the namespace itself.
        /// </summary>
        internal override void CreateMemberSymbols(CSharpProject project, CSharpMemberSymbol parentSymbol)
        {
            if (Name != null)
                Symbol = new CSharpMemberSymbol(Name, parentSymbol, this);

            foreach (CSharpClass @class in Classes)
            {
                @class.CreateMemberSymbols(project, Symbol);
            }

            foreach (CSharpStruct @struct in Structs)
            {
                @struct.CreateMemberSymbols(project, Symbol);
            }

            foreach (CSharpEnum @enum in Enums)
            {
                @enum.CreateMemberSymbols(project, Symbol);
            }

            foreach (CSharpDelegate @delegate in Delegates)
            {
                @delegate.CreateMemberSymbols(project, Symbol);
            }

            foreach (CSharpNamespace @namespace in Namespaces)
            {
                @namespace.CreateMemberSymbols(project, Symbol);
            }
        }

        internal override void ResolveMembers(CSharpProject project)
        {
            foreach (CSharpClass @class in Classes)
            {
                @class.ResolveMembers(project);
            }

            foreach (CSharpStruct @struct in Structs)
            {
                @struct.ResolveMembers(project);
            }

            foreach (CSharpEnum @enum in Enums)
            {
                @enum.ResolveMembers(project);
            }

            foreach (CSharpDelegate @delegate in Delegates)
            {
                @delegate.ResolveMembers(project);
            }

            foreach (CSharpNamespace @namespace in Namespaces)
            {
                @namespace.ResolveMembers(project);
            }
        }

        public override string ToString()
        {
            if (Name == null)
                return "root namespace";
            return $"namespace {Name}";
        }
    }
}