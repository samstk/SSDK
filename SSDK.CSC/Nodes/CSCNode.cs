using Microsoft.CodeAnalysis;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSDK.CSC.Nodes
{
    /// <summary>
    /// A class that simply represents a CSharp language node for conversion.
    /// </summary>
    public abstract class CSCNode : IEnumerable<CSCNode>
    {
        /// <summary>
        /// The root node containing the script that this node is contained in.
        /// </summary>
        public CSCNode Root { get; internal set; }

        /// <summary>
        /// The parent node containing this node.
        /// </summary>
        public CSCNode Parent { get; internal set; }
        
        /// <summary>
        /// The sibling node placed before this node
        /// </summary>
        public CSCNode SiblingBefore { get; internal set; }
        
        /// <summary>
        /// The sibling node placed after this node
        /// </summary>
        public CSCNode SiblingAfter { get; internal set; }

        /// <summary>
        /// Gets the scope that is used when processing this node.
        /// </summary>
        public CSCScope Scope { get; internal set; }

        /// <summary>
        /// Creates a root syntax node from a given syntax tree.
        /// </summary>
        /// <returns>the root node of the syntax tree</returns>
        public static CSCNode FromSyntaxTree(SyntaxTree tree)
        {
            return new CSCComment("Unsupported");
        }



        /// <summary>
        /// Invokes the mapping (preprocessing) function for this node on the given mapping.
        /// </summary>
        /// <param name="mapping">the mapping to invoke this node on</param>
        /// <param name="result">the current result of the conversion</param>
        /// <returns>a string, which is the result of the mapping function</returns>
        public abstract string PreprocessMap(CSCResult result, CSCMapping mapping);

        /// <summary>
        /// Invokes the mapping function for this node on the given mapping.
        /// </summary>
        /// <param name="mapping">the mapping to invoke this node on</param>
        /// <param name="result">the current result of the conversion</param>
        /// <returns>a string, which is the result of the mapping function</returns>
        public abstract string Map(CSCResult result, CSCMapping mapping);

        public IEnumerator<CSCNode> GetEnumerator()
        {
            yield break; // No children node.
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Pre-processes all nodes in pre-order traversal.
        /// </summary>
        /// <param name="mapping">the mapping, containing methods on how to process this</param>
        /// <param name="conversionBuilder">the string builder to append the final output to</param>
        /// <param name="result">the current result of the conversion</param>
        public void Preprocess(CSCResult result, CSCMapping mapping, StringBuilder conversionBuilder)
        {
            // Pre-order traversal
            conversionBuilder.Append(this.PreprocessMap(result, mapping));
            foreach (CSCNode node in this)
            {
                node.Preprocess(result, mapping, conversionBuilder);
            }
        }
        /// <summary>
        /// Processes all nodes in pre-order traversal.
        /// </summary>
        /// <param name="mapping">the mapping, containing methods on how to process this</param>
        /// <param name="conversionBuilder">the string builder to append the final output to</param>
        /// <param name="result">the current result of the conversion</param>
        public void Process(CSCResult result, CSCMapping mapping, StringBuilder conversionBuilder)
        {
            // Pre-order traversal
            conversionBuilder.Append(this.Map(result, mapping));
            foreach (CSCNode node in this)
            {
                node.Preprocess(result, mapping, conversionBuilder);
            }
        }
    }
}
