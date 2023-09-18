using SSDK.CSC.Nodes;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.Text;

namespace SSDK.CSC
{
    /// <summary>
    /// A mapping method for converting a comment.
    /// NOTE that all nodes in the CSharp Syntax Tree are traversed in pre-order.
    /// </summary>
    /// <param name="comment">the comment to convert</param>
    /// <param name="result">the current result for the conversion</param>
    /// <returns>the string that is the conversion result of that comment</returns>
    public delegate string CSCCommentMapping(CSCResult result, CSCComment comment);

    /// <summary>
    /// A mapping method for converting a using directive.
    /// NOTE that all nodes in the CSharp Syntax Tree are traversed in pre-order.
    /// </summary>
    /// <param name="usingDirective">the directive to convert</param>
    /// <param name="result">the current result for the conversion</param>
    /// <returns>the string that is the conversion result of that directive</returns>
    public delegate string CSCUsingDirectiveMapping(CSCResult result, CSCUsingDirective usingDirective);

    /// <summary>
    /// A mapping method for converting a namespace.
    /// NOTE that all nodes in the CSharp Syntax Tree are traversed in pre-order.
    /// </summary>
    /// <param name="namespace">the namespace to convert</param>
    /// <param name="result">the current result for the conversion</param>
    /// <returns>the string that is the conversion result of that directive</returns>
    public delegate string CSCNamespaceMapping(CSCResult result, CSCNamespace @namespace);

    /// <summary>
    /// A crucial class for C# conversion using SSDK.CSC.
    /// Either use one of the pre-made mappings or create your own, but keep in mind the order of executions: <br/>
    /// 1. All namespaces are pre-processsed
    /// </summary>
    public sealed class CSCMapping
    {
        #region Pre-defined Mappings

        #region Conversions
        /// <summary>
        /// Creates a new instance of a CS to JS mapping with standard rules.
        /// </summary>
        /// <returns></returns>
        public static CSCMapping JS()
        {
            return new CSCMapping()
            {
                Comment = (CSCResult result, CSCComment comment) => $"/* {comment.Text.Replace("/*", "*").Replace("*/", "*")} */",
                UsingDirective = StandardUsingDirectiveProcess
            };
        }

        #endregion
        #region Standard Mappings
        /// <summary>
        /// Gets the standard using directive map process, which automatically 
        /// appends the search directive to the result for future references.
        /// </summary>
        public static CSCUsingDirectiveMapping StandardUsingDirectiveProcess { get; private set; }

        /// <summary>
        /// Gets the standard namespace map preprocess, which automatically
        /// creates a symbol for that namespace.
        /// </summary>
        public static CSCNamespaceMapping StandardNamespacePreprocess { get; private set; }

        static CSCMapping()
        {
            StandardUsingDirectiveProcess = (CSCResult result, CSCUsingDirective directive) =>
            {
                directive.Root.Scope.AddSymbol(directive.Target);
                return "";
            };

            StandardNamespacePreprocess = (CSCResult result, CSCNamespace @namespace) =>
            {
                return "";
            };
        }
        #endregion
        #endregion

        #region Mapping Properties
        /// <summary>
        /// Gets or sets the mapping for this type of node.
        /// </summary>
        public CSCCommentMapping Comment { get; set; }

        /// <summary>
        /// Gets or sets the preprocessing for this type of node.
        /// Note that any output is directly appended to script
        /// in the order it was pre-processed.
        /// </summary>
        public CSCCommentMapping PreprocessComment { get; set; }

        /// <summary>
        /// Gets or sets the mapping for this type of node.
        /// </summary>
        public CSCUsingDirectiveMapping UsingDirective { get; set; }
       
        /// <summary>
        /// Gets or sets the preprocessing for this type of node.
        /// Note that any output is directly appended to script
        /// in the order it was pre-processed.
        /// </summary>
        public CSCUsingDirectiveMapping PreprocessUsingDirective { get; set; }

        /// <summary>
        /// Gets or sets the mapping for this type of node.
        /// </summary>
        public CSCNamespaceMapping Namespace { get; set; }

        /// <summary>
        /// Gets or sets the preprocessing for this type of node.
        /// Note that any output is directly appended to script
        /// in the order it was pre-processed.
        /// </summary>
        public CSCNamespaceMapping PreprocessNamespace { get; set; }
        #endregion

        /// <summary>
        /// Create a conversion mapping without any specified details
        /// </summary>
        public CSCMapping() { }

        /// <summary>
        /// Processes the given files, and converts them to the specified code.
        /// </summary>
        /// <param name="files">the list of files to be processed</param>
        /// <returns>a dictionary that
        /// uses the file name as key, and points to the converted script
        /// of the value.</returns>
        public CSCResult Process(string[] files)
        {
            Dictionary<string, string> scripts = new Dictionary<string, string>();
            foreach(string file in files)
            {
                string source = File.ReadAllText(file);
                scripts.Add(file, source);
            }
            return Process(scripts);
        }

        /// <summary>
        /// Processes the given scripts 
        /// </summary>
        /// <param name="scripts">a dictionary of scripts which are identifiable</param>
        /// <returns>a dictionary of scripts after conversion to the given code</returns>
        public CSCResult Process(Dictionary<string, string> scripts)
        {
            Dictionary<string, StringBuilder> conversions = new Dictionary<string, StringBuilder>();
            Dictionary<string, CSCNode> syntaxTrees = new Dictionary<string, CSCNode>();
            Dictionary<string, int> orderings = new Dictionary<string, int>();

            // For now, for testing, just generate first-in-first-out orderings
            int index = 0;
            foreach(string key in scripts.Keys)
            {
                orderings.Add(key, index++);
            }

            foreach(string key in scripts.Keys)
            {
                // Generate syntax trees using roselyn API.
                CSharpSyntaxTree tree = CSharpSyntaxTree.ParseText(scripts[key]) as CSharpSyntaxTree;

                // Convert syntax tree to conversion nodes for ease-of-use.
                // Does not use the generic SSDK.Core tree, as to improve performance and reduce complexity.
                syntaxTrees.Add(key, CSCNode.FromSyntaxTree(tree));
            }

            StringBuilder[] results = new StringBuilder[syntaxTrees.Count];
            CSCResult result = new CSCResult(this);

            index = 0;

            // Pre-process with all mappings.
            foreach(string key in syntaxTrees.Keys)
            {
                StringBuilder builder = new StringBuilder();
                results[index++] = builder;
                syntaxTrees[key].Preprocess(result, this, builder);
            }

            index = 0;

            // Process with all mappings
            foreach (string key in syntaxTrees.Keys)
            {
                StringBuilder builder = results[index++];
                syntaxTrees[key].Process(result, this, builder);
                conversions.Add(key, builder);
            }

            result.Finalise(conversions, orderings);
            return result;
        }
    }
}