using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSDK.CSC.Nodes
{
    /// <summary>
    /// A node of a parsed CS code for CSC usage, which
    /// represents a using directive.
    /// </summary>
    public sealed class CSCUsingDirective : CSCNode
    {
        /// <summary>
        /// Gets the target of the using directive, which should 
        /// be a namespace.
        /// </summary>
        public CSCSymbol Target { get; private set; }

        /// <summary>
        /// If true, then the do not throw any exception for say,
        /// using static System.Console;
        /// </summary>
        public bool IsStatic { get; private set; }

        /// <summary>
        /// Creates a new CS using directive with the
        /// </summary>
        /// <param name="target">the identifier symbol pointing to what to use</param>
        public CSCUsingDirective(CSCSymbol target)
        {
            Target = target;
        }

        public override string Map(CSCResult result, CSCMapping mapping)
        {
            if (mapping.UsingDirective != null)
                return mapping.UsingDirective(result, this);
            return "";
        }

        public override string PreprocessMap(CSCResult result, CSCMapping mapping)
        {
            if (mapping.PreprocessUsingDirective != null)
                return mapping.PreprocessUsingDirective(result, this);
            return "";
        }
    }
}
