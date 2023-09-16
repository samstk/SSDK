using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSDK.CSC.Nodes
{
    /// <summary>
    /// A node of a parsed CS code for CSC usage, which
    /// represents a comment before a statement. Do not
    /// confuse comment with DocString.
    /// <br/>
    /// Even though /* and // both indicate comments,
    /// they are both included as the same thing.
    /// </summary>
    public sealed class CSCComment : CSCNode
    {
        /// <summary>
        /// Gets the text used in the comment
        /// </summary>
        public string Text { get; private set; }

        /// <summary>
        /// Creates a new CS comment with the
        /// </summary>
        /// <param name="text"></param>
        public CSCComment(string text)
        {
            Text = text;
        }
    }
}
