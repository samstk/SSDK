using SSDK.CSC.Nodes;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace SSDK.CSC
{
    /// <summary>
    /// A mapping method for converted a comment between code.
    /// </summary>
    /// <param name="comment">the comment to convert</param>
    /// <returns>the string that is the conversion result of that comment</returns>
    public delegate string CSCommentMapping(CSCComment comment);

    /// <summary>
    /// A crucial class for C# conversion using SSDK.CSC.
    /// Either use one of the pre-made mappings or create your own, but keep in mind the order of executions: <br/>
    /// 1. All namespaces are pre-processsed
    /// </summary>
    public sealed class CSCMapping
    {
        #region Pre-defined Mappings
        /// <summary>
        /// Creates a new instance of a CS to JS mapping with standard rules.
        /// </summary>
        /// <returns></returns>
        public static CSCMapping JS()
        {
            return new CSCMapping()
            {

            };
        }
        #endregion

        #region Mapping Properties
        /// <summary>
        /// Gets or sets the mapping for comments
        /// </summary>
        public CSCommentMapping Comment { get; set; }
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
            Dictionary<string, string> conversions = new Dictionary<string, string>();
            Dictionary<string, CSharpSyntaxTree> syntaxTrees = new Dictionary<string, CSharpSyntaxTree>();
            Dictionary<string, int> orderings = new Dictionary<string, int>();

            // For now, for testing, just generate first-in-first-out orderings
            int index = 0;
            foreach(string key in conversions.Keys)
            {
                orderings.Add(key, index++);
            }

            // Generate syntax trees using roselyn API.
            foreach(string key in scripts.Keys)
            {
                syntaxTrees.Add(key, CSharpSyntaxTree.ParseText(scripts[key]) as CSharpSyntaxTree);
            }

            return new CSCResult(this, conversions, orderings);
        }
    }
}