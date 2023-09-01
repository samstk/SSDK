using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSDK.Core.Structures.Trees
{
    public sealed class TreeNode<T> : IEnumerable<TreeNode<T>>
    {
        #region Properties & Fields

        /// <summary>
        /// Gets all nodes existing in this tree/subtree, including
        /// this one.
        /// </summary>
        public List<TreeNode<T>> AllNodes
        {
            get
            {
                List<TreeNode<T>> list = new List<TreeNode<T>>();

                list.Add(this);
                
                // Get all children
                foreach(TreeNode<T> child in _Children)
                {
                    list.AddRange(child.AllNodes);
                }

                return list;
            }
        }

        /// <summary>
        /// Gets all children existing in this tree/subtree, including
        /// this one.
        /// </summary>
        public List<TreeNode<T>> AllChildren
        {
            get
            {
                List<TreeNode<T>> list = new List<TreeNode<T>>();

                // Get all children
                foreach (TreeNode<T> child in _Children)
                {
                    list.AddRange(child.AllNodes);
                }

                return list;
            }
        }

        /// <summary>
        /// The children of the current node.
        /// </summary>
        internal List<TreeNode<T>> _Children;

        /// <summary>
        /// Gets the children of the current tree node.
        /// Cannot be modified, use add/remove instead.
        /// </summary>
        public ReadOnlyCollection<TreeNode<T>> Children
        {
            get
            {
                return _Children.AsReadOnly();
            }
        }

        /// <summary>
        /// Gets the depth of the node.
        /// (i.e. how many levels above exists)
        /// </summary>
        public int Depth
        {
            get
            {
                if (ParentNode != null) return ParentNode.Depth + 1;
                return 0;
            }
        }

        /// <summary>
        /// Gets the height of the node.
        /// (i.e. how many levels of children exists)
        /// </summary>
        public int Height
        {
            get
            {
                if (Children.Count == 0)
                    return 0;

                int maxHeight = 0;

                // Search through children for max height
                foreach(TreeNode<T> child in _Children)
                {
                    int height = child.Height;
                    if(height > maxHeight)
                    {
                        maxHeight = height;
                    }
                }

                return maxHeight + 1;
            }
        }

        
        
        /// <summary>
        /// True if this node has children.
        /// </summary>
        public bool IsInternalNode { get { return _Children.Count > 0; } }

        /// <summary>
        /// True if this node has no children.
        /// </summary>
        public bool IsLeafNode { get { return _Children.Count == 0; } }

        /// <summary>
        /// True if this node is treated as the root of a tree.
        /// </summary>
        public bool IsRootNode { get { return ParentNode == null; } }

        /// <summary>
        /// Gets the k-ary ness of the tree.
        /// (i.e. the maximum children a node has in this tree/subtree)
        /// </summary>
        public int K
        {
            get
            {
                int k = _Children.Count;

                // Search children for bigger k-ary
                foreach (TreeNode<T> child in _Children)
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
        /// Gets the number of children this node has.
        /// </summary>
        public int NumberOfChildren
        {
            get
            {
                return _Children.Count;
            }
        }
        /// <summary>
        /// Gets the node that is the parent to this one.
        /// Null if this node is root.
        /// </summary>
        public TreeNode<T> ParentNode { get; internal set; }

        /// <summary>
        /// Gets the number of siblings this node has
        /// </summary>
        public int Siblings
        {
            get
            {
                return ParentNode == null ? 0 : ParentNode._Children.Count - 1;
            }
        }

        /// <summary>
        /// Returns the number of nodes in this subtree (including this one)
        /// </summary>
        public int Size
        {
            get
            {
                int size = 1;

                // Search through children to accumulate size
                foreach (TreeNode<T> child in _Children)
                {
                    size += child.Size;
                }

                return size;
            }
        }

        /// <summary>
        /// The value of the current node.
        /// </summary>
        public T Value;

        #endregion
        #region Constructors
        public TreeNode(T value, TreeNode<T> parentNode=null)
        {
            Value = value;
            ParentNode = parentNode;
            _Children = new List<TreeNode<T>>();
        }

        internal TreeNode() { }
        #endregion
        #region Methods
        #region Cloning
        /// <summary>
        /// Clones the current tree node.
        /// The new node has no parent.
        /// </summary>
        /// <param name="deep">
        /// If false, then only the reference to the root node is copied.
        /// and modifications to the tree are then shared.
        /// </param>
        /// <returns>a new node, which is an exact copy of this one</returns>
        public TreeNode<T> Clone(bool deep=false)
        {
            TreeNode<T> newNode = new TreeNode<T>(Value);
            if (deep)
            {
                foreach (TreeNode<T> child in _Children)
                {
                    TreeNode<T> clone = child.Clone(true);
                    clone.ParentNode = this;
                    newNode._Children.Add(clone);
                }
            }
            else newNode._Children.AddRange(_Children);
            return newNode;
        }
        #endregion
        #region Modifications
        /// <summary>
        /// Adds a node under this tree.
        /// </summary>
        /// <param name="node">the node to add</param>
        /// <param name="insertPosition">the position to insert to (-1 if at end)</param>
        public void Add(TreeNode<T> node, int insertPosition=-1)
        {
            if (node == null) throw new ArgumentNullException("node", "node must not be null");
            
            if (insertPosition == -1)
                _Children.Add(node);
            else _Children.Insert(insertPosition, node);
        }
        /// <summary>
        /// Removes the given node under this tree, assuming that it exists
        /// </summary>
        /// <param name="node">the node to remove</param>
        /// <returns>-1 if the node did not exist, or the child index of the node</returns>
        public int Remove(TreeNode<T> node)
        {
            int index = _Children.IndexOf(node);
            
            if (index == -1) return -1;
            _Children.RemoveAt(index);
            return index;
        }
        #endregion
        #region Traversals
        /// <summary>
        /// Performs the given action on every visit to a node, using 
        /// pre-order traversal logic.
        /// </summary>
        /// <param name="traverseAction">the action to apply on every visited node</param>
        public void TraverseInPreOrder(Action<TreeNode<T>> traverseAction)
        {
            traverseAction(this);

            foreach(TreeNode<T> child in _Children)
            {
                child.TraverseInPreOrder(traverseAction);
            }
        }

        /// <summary>
        /// Performs the given action on every visit to a node, using 
        /// in-order traversal logic.
        /// </summary>
        /// <param name="traverseAction">the action to apply on every visited node</param>
        /// <param name="k">the k-ary of the tree (k/2) is where the in-order parent is visited.</param>
        public void TraverseInOrder(Action<TreeNode<T>> traverseAction, int k)
        {
            int middle = k / 2;

            // Traverse all items before current
            for(int i = 0; i< middle; i++)
            {
                _Children[i].TraverseInOrder(traverseAction, k);
            }

            traverseAction(this);

            // Traverse all items after current
            for(int i = middle; i < _Children.Count; i++)
            {
                _Children[i].TraverseInOrder(traverseAction, k);
            }
        }

        /// <summary>
        /// Performs the given action on every visit to a node, using 
        /// post-order traversal logic.
        /// </summary>
        /// <param name="traverseAction">the action to apply on every visited node</param>
        public void TraverseInPostOrder(Action<TreeNode<T>> traverseAction)
        {
            foreach (TreeNode<T> child in _Children)
            {
                child.TraverseInPreOrder(traverseAction);
            }

            traverseAction(this);
        }
        #endregion
        #endregion
        #region Enumerator
        public IEnumerator<TreeNode<T>> GetEnumerator()
        {
            foreach (TreeNode<T> node in _Children)
                yield return node;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        #endregion
    }
}
