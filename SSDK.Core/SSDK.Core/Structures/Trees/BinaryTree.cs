using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSDK.Core.Structures.Trees
{
    /// <summary>
    /// A binary tree is a tree with at most two children (left, and right).
    /// </summary>
    public abstract class BinaryTree<T> : Tree<T>
    {


        #region Properties & Fields
        /// <summary>
        /// Gets the k-ary ness of the tree. In this case, since the tree
        /// is a binary tree, the k-ary ness is always 2.
        /// </summary>
        public override int K => 2;

        #endregion
    }
}
