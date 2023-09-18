using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSDK.CSC.Nodes
{
    /// <summary>
    /// A node of a parsed CS code for CSC usage, which
    /// represents a namespace.
    /// </summary>
    public sealed class CSCNamespace : CSCNode
    {
        /// <summary>
        /// Gets the target of the namespace, which should contain everything in the namespace,
        /// and be referenced in respect to the namespace's parent.
        /// </summary>
        public CSCSymbol Target { get; private set; }


        /// <summary>
        /// Creates a new CS namespace with the
        /// </summary>
        /// <param name="target">the identifier symbol that contains everything in the namespace</param>
        public CSCNamespace(CSCSymbol target)
        {
            Target = target;
        }

        public override string Map(CSCResult result, CSCMapping mapping)
        {
            if (mapping.Namespace != null)
                return mapping.Namespace(result, this);
            return "";
        }

        public override string PreprocessMap(CSCResult result, CSCMapping mapping)
        {
            if (mapping.PreprocessNamespace != null)
                return mapping.PreprocessNamespace(result, this);
            return "";
        }
    }
}
