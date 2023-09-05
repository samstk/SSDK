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
        /// True if all levels of the tree are full, or all levels but
        /// the last level is full and all leaves are furthest to the left.
        /// </summary>
        public bool IsComplete
        {
            get
            {
                int k = K;
                int[] levels = new int[Height+1];

                // Traverse all levels and accumulate totals for each level.
                TraverseInLevel((el, level) =>
                {
                    levels[level]++;
                }, (el) => true);

                // Check all levels are as expected.
                for(int l = 0; l<levels.Length-1; l++)
                {
                    int expected = (int)Math.Pow(k, l);
                    if(expected != levels[l])
                    {
                        return false;
                    }
                }

                // Check to make sure last level is far-left as possible.
                bool valid = true;
                TraverseInLevel((el, level) =>
                {
                    if (el.Left == null && el.Right != null)
                    {
                        valid = false;
                    }
                }, (l) => l == levels.Length - 2, (l) => valid);

                return valid;
            }
        }

        /// <summary>
        /// True if all nodes in this tree except leaf nodes has k children.
        /// </summary>
        public bool IsProper
        {
            get
            {
                int k = K;
                bool valid = true;

                // Check to make sure all child nodes furthest left.
                TraverseInLevel((el, level) =>
                {
                    int got = 0;
                    foreach(TreeNode<T> child in el.Children)
                    {
                        if (child != null)
                            got++;
                        else got = 0;
                    }
                    if (got != k)
                        valid = false; // Either full or empty.
                }, (l) => true, (l) => valid);
                return valid;
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
        public TreeNode<T> RootNode { get; protected set; }

        

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
        public abstract Tree<T> Clone(bool deep = true);
        /// <summary>
        /// Clones the current tree.
        /// </summary>
        /// <param name="treeType">the tree type constructor</param>
        /// <param name="deep">
        /// If false, then only the reference to the root node is copied.
        /// and modifications to the tree are then shared.
        /// </param>
        /// <returns>a new tree, which is an exact copy of this tree.</returns>
        internal Tree<T> Clone(Func<Tree<T>> treeType, bool deep = true)
        {
            Tree<T> tree = treeType();
            if(deep && RootNode!=null)
            {
                tree.RootNode = RootNode.Clone(true);
            }
            else
            {
                tree.RootNode = RootNode;
            }
            return tree;
        }
        #endregion
        #region Modification
        
        /// <summary>
        /// Adds an element to the tree.
        /// </summary>
        /// <param name="element">the element to add</param>
        public abstract void Add(T element);

        /// <summary>
        /// Removes an element's node from the tree.
        /// </summary>
        /// <param name="elementNode">the element node to remove</param>
        public abstract void Remove(TreeNode<T> elementNode);
        
        /// <summary>
        /// Removes an element from the tree.
        /// </summary>
        /// <param name="element">the element to remove</param>
        /// <returns>the node that was removed, with its references still existing.</returns>
        public TreeNode<T> Remove(T element)
        {
            TreeNode<T> result = Search(element);
            if (result == null) return null;
            Remove(result);
            return result;
        }

        /// <summary>
        /// Sets the root of the tree.
        /// </summary>
        /// <param name="element">the element contained within the root of the tree</param>
        public virtual void SetRoot(T element)
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
        public void TraverseInPreOrder(Action<TreeNode<T>> traverseAction, Func<bool> cutoffSelector = null)
        {
            RootNode?.TraverseInPreOrder(traverseAction, cutoffSelector);
        }

        /// <summary>
        /// Performs the given action on every visit to a node, using 
        /// in-order traversal logic.
        /// </summary>
        /// <param name="traverseAction">the action to apply on every visited node</param>
        /// <param name="k">the k-ary of the tree (k/2) is where the in-order parent is visited.</param>
        public void TraverseInOrder(Action<TreeNode<T>> traverseAction, int k=-1, Func<bool> cutoffSelector = null)
        {
            if (k == -1) k = K;
            RootNode?.TraverseInOrder(traverseAction, k, cutoffSelector);
        }

        /// <summary>
        /// Performs the given action on every visit to a node, using 
        /// post-order traversal logic.
        /// </summary>
        /// <param name="traverseAction">the action to apply on every visited node</param>
        public void TraverseInPostOrder(Action<TreeNode<T>> traverseAction, Func<bool> cutoffSelector=null)
        {
            RootNode?.TraverseInPostOrder(traverseAction, cutoffSelector);
        }

        /// <summary>
        /// Traverses the nodes in pre-order and visits the node if the level selector
        /// checks of (e.g. () => 2, visits all nodes in level 2).
        /// </summary>
        /// <param name="traverseAction">the action to apply on every visited node (w/ level)</param>
        /// <param name="levelSelector">the selector which determines which nodes to visit based on level</param>
        public void TraverseInLevel(Action<TreeNode<T>, int> traverseAction, Func<int, bool> levelSelector, Func<int,bool> cutoffSelector=null)
        {
            RootNode?.TraverseInLevel(traverseAction, levelSelector, cutoffSelector);
        }

        #endregion

        /// <summary>
        /// Returns true if the given level at depth l is full according to k-ary ness.
        /// </summary>
        /// <param name="level">the level to check</param>
        /// <returns>true if the given level is full</returns>
        public bool IsLevelFull(int level)
        {
            int k = K, got = 0;
            int expected = (int)Math.Pow(k, level);

            // Traverse in order to check level is full.
            TraverseInLevel((el, level) => got++, (l) => l == level, (l) => l == level);

            return got == expected;
        }

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
        #region Conversion
        /// <summary>
        /// Converts this tree to a binary tree, assuming k=2
        /// </summary>
        /// <param name="treeType">a constructor for the tree type</param>
        /// <returns>null if invalid tree, else the resulting tree</returns>
        public Tree<T> ToBinaryTree(Func<Tree<T>> treeType, bool deepCloning=true)
        {
            Tree<T> tree = treeType();
            if(RootNode != null)
            {
                if(RootNode.NumberOfChildren > 2)
                {
                    return null;
                }
                else
                {
                    tree.RootNode = RootNode.Clone(false);
                } 
            }
            return tree;
        }

        /// <summary>
        /// Adds a given node to the binary tree node, returning false if
        /// the number of children exceeds two.
        /// </summary>
        /// <param name="to">the node to add itself to</param>
        /// <param name="node"></param>
        /// <returns></returns>
        private static bool AddBinaryNode(TreeNode<T> to, TreeNode<T> left, TreeNode<T> right, bool deepCloning)
        {
            // Check if nodes exceed 2-ary
            if (left != null && left.NumberOfChildren > 2 || right != null && right.NumberOfChildren > 2) return false;

            // Clone left and right (shallow copy)
            if (deepCloning)
            {
                if (left != null)
                    left = left.Clone(false);
                if (right != null)
                    right = right.Clone(false);
            }

            // Re-assign parent nodes
            if (left != null)
            {
                left.ParentNode = to;
            }
            if (right != null)
            {
                right.ParentNode = to;
            }

            // Add children
            bool leftSuccess = AddBinaryNode(left, left.Left, left.Right, deepCloning);
            if (!leftSuccess) return false;

            bool rightSuccess = AddBinaryNode(right, right.Left, right.Right, deepCloning);
            return rightSuccess;
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

        public override string ToString()
        {
            return $"Tree({Size})";
        }
    }
}
