using Microsoft.CodeAnalysis.CSharp;
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
        /// Contains all root-level namespaces that may contain code.
        /// </summary>
        public CSharpNamespace[] Namespaces { get; private set; }   

        /// <summary>
        /// Gets the root-level classes that may be referenced within the project without using directives.
        /// </summary>
        public CSharpClass[] Classes { get; private set; }

        /// <summary>
        /// Gets the root-level structs that may be referenced within the project without using directives.
        /// </summary>
        public CSharpStruct[] Structures { get; private set; }

        /// <summary>
        /// Gets the root-level enums that may be referenced within the project without using directives.
        /// </summary>
        public CSharpEnum[] Enums { get; private set; }

        /// <summary>
        /// Gets all scripts that this project is made of.
        /// </summary>
        public CSharpScript[] Scripts { get; private set; }

        /// <summary>
        /// Gets the root-level delegates that may be referenced within the project without using directives.
        /// </summary>
        public CSharpDelegate[] Delegates { get; private set; }
        #endregion

        /// <summary>
        /// Creates a new C# project, by converting the inputs to an Abstract Syntax Tree (roselyn's SyntaxTree),
        /// and then to local components.
        /// </summary>
        /// <param name="script">the scripts (or files)</param>
        /// <param name="scriptsAreFiles">if true, then the script parameter is read as the file to read</param>
        public CSharpProject(string[] scripts, bool scriptsAreFiles=false)
        {
            // Create scripts and references
            Scripts = new CSharpScript[scripts.Length];

            // & Populate lists from all scripts (root-level components)
            List<CSharpNamespace> namespaces = new List<CSharpNamespace>();
            List<CSharpClass> classes = new List<CSharpClass>();
            List<CSharpStruct> structs = new List<CSharpStruct>();
            List<CSharpEnum> enums = new List<CSharpEnum>();
            List<CSharpDelegate> delegates = new List<CSharpDelegate>();

            // Anonymous functions for populating lists.
            for (int i = 0; i < scripts.Length; i++)
            {
                CSharpScript script = new CSharpScript(scripts[i], scriptsAreFiles, false);
                
                Scripts[i] = script;
            }
        }
    }
}
