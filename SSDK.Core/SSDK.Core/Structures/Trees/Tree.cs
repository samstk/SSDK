using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSDK.Core.Structures.Trees
{
    /// <summary>
    /// A tree is an abstract model of a heirarchical structure, consisting
    /// of parent-child relations between nodes. A node can only have one parent,
    /// whereas a parent can have multiple children.
    /// </summary>
    public abstract class Tree<T> : IEnumerable<TreeNode<T>>
    {
        #region Properties & Fields

        /// <summary>
        /// Gets all nodes existing in this tree
        /// </summary>
        public List<TreeNode<T>> AllNodes
        {
            get
            {
                List<TreeNode<T>> list = new List<TreeNode<T>>();
                if (RootNode == null) return list;

                list.Add(RootNode);

                // Get all nodes of root node
                list.AddRange(RootNode.AllChildren);

                return list;
            }
        }

        /// <summary>
        /// Gets the height of the tree
        /// (i.e. the height of the root node)
        /// </summary>
        public int Height
        {
            get
            {
                return RootNode == null ? 0 : RootNode.Height;
            }
        }

        /// <summary>
        /// True if there exists no nodes in this tree.
        /// (i.e. the root node doesn't exist)
        /// </summary>
        public bool IsEmpty
        {
            get
            {
                return RootNode == null;
            }
        }
        /// <summary>
        /// Gets the k-ary ness of the tree.
        /// (i.e. the maximum children a node has in this tree)
        /// </summary>
        public virtual int K
        {
            get
            {
                if (RootNode == null) return 0;

                int k = RootNode._Children.Count;

                // Search children for bigger k-ary
                foreach (TreeNode<T> child in RootNode._Children)
                {
                    int ck = child.K;
                    if (ck > k)
                    {
                        k = ck;
                    }
                }
                return k;
            }
        }

        /// <summary>
        /// Gets the maximum internal nodes of this tree/subtree, assuming
        /// the k-ary ness is the same.
        /// (i.e. 2^h - 1)
        /// </summary>
        public int MaximumInternalNodes
        {
            get
            {
                return (int)Math.Pow(K, Height);
            }
        }

        /// <summary>
        /// Gets the root node of this tree.
        /// </summary>
        public TreeNode<T> RootNode { get; private set; }

        

        /// <summary>
        /// Returns the number of nodes in this tree
        /// </summary>
        public int Size
        {
            get
            {
                return RootNode == null ? 0 : RootNode.Size;
            }
        }


        #endregion
        #region Constructor
        public Tree()
        {

        }
        #endregion
        #region Methods
        #region Cloning
        /// <summary>
        /// Clones the current tree.
        /// </summary>
        /// <param name="deep">
        /// If false, then only the reference to the root node is copied.
        /// and modifications to the tree are then shared.
        /// </param>
        /// <returns>a new tree, which is an exact copy of this tree.</returns>
        public abstract Tree<T> Clone(bool deep = false);
        #endregion
        #region Modification
        
        /// <summary>
        /// Adds an element to the tree.
        /// </summary>
        /// <param name="element">the element to add</param>
        public abstract void Add(T element);

        /// <summary>
        /// Removes an element from the tree.
        /// </summary>
        /// <param name="element">the element to remove</param>
        /// <returns>the node that was removed, with its references still existing.</returns>
        public abstract TreeNode<T> Remove(T element);

        /// <summary>
        /// Sets the root of the tree.
        /// </summary>
        /// <param name="element">the element contained within the root of the tree</param>
        public void SetRoot(T element)
        {
            RootNode = new TreeNode<T>(element);
        }

        /// <summary>
        /// Sets the root of the tree.
        /// </summary>
        /// <param name="root">the new root of the tree.</param>
        public void SetRoot(TreeNode<T> root)
        {
            RootNode = root;
            if (root != null)
            {
                root.ParentNode = null;
            }
        }
        #endregion
        #region Searching
        #region Traversals
        /// <summary>
        /// Performs the given action on every visit to a node, using 
        /// pre-order traversal logic.
        /// </summary>
        /// <param name="traverseAction">the action to apply on every visited node</param>
        public void TraverseInPreOrder(Action<TreeNode<T>> traverseAction)
        {
            RootNode?.TraverseInPreOrder(traverseAction);
        }

        /// <summary>
        /// Performs the given action on every visit to a node, using 
        /// in-order traversal logic.
        /// </summary>
        /// <param name="traverseAction">the action to apply on every visited node</param>
        /// <param name="k">the k-ary of the tree (k/2) is where the in-order parent is visited.</param>
        public void TraverseInOrder(Action<TreeNode<T>> traverseAction, int k=-1)
        {
            if (k == -1) k = K;
            RootNode?.TraverseInOrder(traverseAction, k);
        }

        /// <summary>
        /// Performs the given action on every visit to a node, using 
        /// post-order traversal logic.
        /// </summary>
        /// <param name="traverseAction">the action to apply on every visited node</param>
        public void TraverseInPostOrder(Action<TreeNode<T>> traverseAction)
        {
            RootNode?.TraverseInPostOrder(traverseAction);
        }
        #endregion
        /// <summary>
        /// Gets the tree node that contains the element to search for.
        /// The current method to search uses an inefficient method.
        /// </summary>
        /// <param name="searchFor">the element to search for</param>
        /// <returns>null if not found, or the element if found.</returns>
        public virtual TreeNode<T> Search(T searchFor)
        {
            if (RootNode == null) return null;

            // Create expanding stack
            Stack<TreeNode<T>> expanding = new Stack<TreeNode<T>>();
            expanding.Push(RootNode);

            // Expand until exhausted
            while(expanding.Count > 0)
            {
                TreeNode<T> expand = expanding.Pop();

                // Check if current node matches.
                if (expand.Value == null && searchFor == null || expand.Value.Equals(searchFor))
                {
                    return expand;
                }

                // Append children to expanding stack.
                foreach (TreeNode<T> child in expand)
                {
                    expanding.Push(child);
                }
            }
            return null;
        }
        #endregion
        #endregion
        #region Enumerator
        public IEnumerator<TreeNode<T>> GetEnumerator()
        {
            if (RootNode == null) yield break;

            // Create expanding stack.
            Stack<TreeNode<T>> expanding = new Stack<TreeNode<T>>();
            expanding.Push(RootNode);

            // Continue to expand stack and return children.
            while(expanding.Count > 0)
            {
                TreeNode<T> expand = expanding.Pop();
                yield return expand;
                foreach(TreeNode<T> child in expand)
                {
                    expanding.Push(child);
                }
            } 
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        #endregion
    }
}
