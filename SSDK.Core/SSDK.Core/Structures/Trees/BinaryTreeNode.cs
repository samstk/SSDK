using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSDK.Core.Structures.Trees
{
    public sealed class BinaryTreeNode<T> : IEnumerable<BinaryTreeNode<T>>
    {
        #region Properties & Fields
        #region Binary Properties
        /// <summary>
        /// True if this node is the left descendent of the parent.
        /// </summary>
        public bool IsLeft
        {
            get
            {
                return ParentNode != null && ParentNode.Left == this;
            }
        }

        /// <summary>
        /// True if this node is the right descendent of the parent.
        /// </summary>
        public bool IsRight
        {
            get
            {
                return ParentNode != null && ParentNode.Right == this;
            }
        }

        private BinaryTreeNode<T> _Left = null;
        /// <summary>
        /// Gets or sets the left-side child assuming k=2
        /// </summary>
        public BinaryTreeNode<T> Left
        {
            get
            {
                return _Left;
            }
            set
            {
                if (_Left != null) _Left.ParentNode = null;
                _Left = value;
                if (_Left != null) _Left.ParentNode = this;
            }
        }
        /// <summary>
        /// Gets the left-most node (i.e. travelling left as much as possible)
        /// </summary>
        public BinaryTreeNode<T> LeftMostNode
        {
            get
            {
                BinaryTreeNode<T> current = this;
                while(current.Left != null)
                {
                    current = current.Left;
                }
                return current;
            }
        }

        /// <summary>
        /// Gets the in-order predecessor of this node, simply
        /// returning null if it does not exist.
        /// </summary>
        public BinaryTreeNode<T> InOrderPredecessor
        {
            get
            {
                if (Left == null) return null;

                BinaryTreeNode<T> searchNode = Left;
                while(searchNode != null)
                {
                    if (searchNode.Right != null)
                    {
                        // Go furthest right as possible
                        searchNode = searchNode.Right;
                    }
                    else break;
                }
                return searchNode;
            }
        }

        /// <summary>
        /// Gets the in-order successor of this node, simply
        /// returning null if it does not exist.
        /// </summary>
        public BinaryTreeNode<T> InOrderSuccessor
        {
            get
            {
                if (Right == null) return null;

                BinaryTreeNode<T> searchNode = Right;
                while (searchNode != null)
                {
                    if (searchNode.Left != null)
                    {
                        // Go furthest left as possible
                        searchNode = searchNode.Left;
                    }
                    else break;
                }
                return searchNode;
            }
        }

        private BinaryTreeNode<T> _Right;
        /// <summary>
        /// Gets or sets the right-side child assuming k=2
        /// </summary>
        public BinaryTreeNode<T> Right {
            get
            {
                return _Right;
            }
            set
            {
                if (_Right != null) _Right.ParentNode = null;
                _Right = value;
                if (_Right != null) _Right.ParentNode = this;
            }
        }

        /// <summary>
        /// Gets the right-most node (i.e. travelling right as much as possible)
        /// </summary>
        public BinaryTreeNode<T> RightMostNode
        {
            get
            {
                BinaryTreeNode<T> current = this;
                while (current.Right != null)
                {
                    current = current.Right;
                }
                return current;
            }
        }

        /// <summary>
        /// Gets the opposing sibling in a binary tree.
        /// </summary>
        public BinaryTreeNode<T> Sibling
        {
            get
            {
                if (ParentNode == null) return null;
                return ParentNode.Left == this ? ParentNode.Right : ParentNode.Left;
            }
        }
        #endregion
        /// <summary>
        /// Gets all nodes existing in this tree/subtree, including
        /// this one.
        /// </summary>
        public List<BinaryTreeNode<T>> AllNodes
        {
            get
            {
                List<BinaryTreeNode<T>> list = new List<BinaryTreeNode<T>>();

                list.Add(this);

                // Get all children
                if (Left != null)
                    list.AddRange(Left.AllNodes);

                if (Right != null)
                    list.AddRange(Right.AllNodes);

                return list;
            }
        }

        /// <summary>
        /// Gets all children existing in this tree/subtree, including
        /// this one.
        /// </summary>
        public List<BinaryTreeNode<T>> AllChildren
        {
            get
            {
                List<BinaryTreeNode<T>> list = new List<BinaryTreeNode<T>>();

                // Get all children
                if (Left != null)
                    list.AddRange(Left.AllNodes);

                if (Right != null)
                    list.AddRange(Right.AllNodes);

                return list;
            }
        }

        /// <summary>
        /// Gets the children of the current tree node.
        /// Cannot be modified, use add/remove instead.
        /// </summary>
        public ReadOnlyCollection<BinaryTreeNode<T>> Children
        {
            get
            {
                List<BinaryTreeNode<T>> list = new List<BinaryTreeNode<T>>();
                if (Left != null) list.Add(Left);
                if (Right != null) list.Add(Right);
                return list.AsReadOnly();
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

                // Search through children for max height
                int maxHeight = Left != null ? Left.Height : 0;
                if (Right != null)
                {
                    int rightHeight = Right.Height;
                    if (rightHeight > maxHeight)
                    {
                        maxHeight = Right.Height;
                    }
                }

                return maxHeight + 1;
            }
        }

        
        
        /// <summary>
        /// True if this node has children.
        /// </summary>
        public bool IsInternalNode { get { return Left != null || Right != null;  } }

        /// <summary>
        /// True if this node has no children.
        /// </summary>
        public bool IsLeafNode { get { return Left == null && Right == null; } }

        /// <summary>
        /// True if this node is treated as the root of a tree.
        /// </summary>
        public bool IsRootNode { get { return ParentNode == null; } }

        /// <summary>
        /// Gets the k-ary ness of the tree.
        /// (i.e. the maximum children a node has in this tree/subtree)
        /// As a binary tree, it can only be 2-ary.
        /// </summary>
        public int K
        {
            get
            {
                return 2;
            }
        }

        /// <summary>
        /// Gets the number of children this node has (max 2).
        /// </summary>
        public int NumberOfChildren
        {
            get
            {
                return (Left != null ? 1 : 0)
                    + (Right != null ? 1 : 0);
            }
        }
        /// <summary>
        /// Gets the node that is the parent to this one.
        /// Null if this node is root.
        /// </summary>
        public BinaryTreeNode<T> ParentNode { get; internal set; }

        /// <summary>
        /// Gets the number of siblings this node has (either 1 or 0).
        /// </summary>
        public int Siblings
        {
            get
            {
                return ParentNode == null ? 0 : ParentNode.NumberOfChildren - 1;
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
                if (Left != null)
                    size += Left.Size;
                if (Right != null)
                    size += Right.Size;

                return size;
            }
        }

        /// <summary>
        /// The value of the current node.
        /// </summary>
        public T Value;

        #endregion
        #region Constructors
        public BinaryTreeNode(T value, BinaryTreeNode<T> parentNode=null)
        {
            Value = value;
            ParentNode = parentNode;
        }

        internal BinaryTreeNode() { }
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
        public BinaryTreeNode<T> Clone(bool deep=true)
        {
            BinaryTreeNode<T> newNode = new BinaryTreeNode<T>(Value);
            if (deep)
            {
                newNode.Left = (Left != null ? Left.Clone(true) : null);
                newNode.Right = (Right != null ? Right.Clone(true) : null);
            }
            else
            {
                newNode.Left = Left;
                newNode.Right = Right;
            }
            return newNode;
        }
        #endregion
        #region Modifications
        /// <summary>
        /// Removes the given node under this tree, assuming that it exists
        /// </summary>
        /// <param name="node">the node to remove</param>
        /// <returns>-1 if the node did not exist, or the child index (0=left,1=right) of the node</returns>
        public int Remove(BinaryTreeNode<T> node)
        {
            if (Left == node) {
                Left = null;
                return 0;
            }
            else if (Right == node)
            {
                Right = null;
                return 1;
            }
            return -1;
        }
        
        /// <summary>
        /// Removes this node/subtree from the parent node. Use Tree's removal method to avoid
        /// conflicts.
        /// </summary>
        public void Remove()
        {
            if(ParentNode != null)
            {
                ParentNode.Remove(this);
            }
        }
        /// <summary>
        /// Swaps the values of two nodes.
        /// </summary>
        /// <param name="node">the other node to swap with</param>
        public void SwapValue(BinaryTreeNode<T> node)
        {
            T val = Value;
            Value = node.Value;
            node.Value = val;
        }
        #endregion
        #region Traversals
        /// <summary>
        /// Performs the given action on every visit to a node, using 
        /// pre-order traversal logic.
        /// </summary>
        /// <param name="traverseAction">the action to apply on every visited node</param>
        /// <param name="cutoffSelector">a function which returns a boolean indicating the traversal should stop</param>
        public void TraverseInPreOrder(Action<BinaryTreeNode<T>> traverseAction, Func<bool> cutoffSelector=null)
        {
            if (cutoffSelector != null && cutoffSelector()) return;

            traverseAction(this);

            Left?.TraverseInPreOrder(traverseAction, cutoffSelector);
            Right?.TraverseInPreOrder(traverseAction, cutoffSelector);
        }

        /// <summary>
        /// Performs the given action on every visit to a node, using 
        /// in-order traversal logic.
        /// </summary>
        /// <param name="traverseAction">the action to apply on every visited node</param>
        /// <param name="cutoffSelector">a function which returns a boolean indicating the traversal should stop</param>
        /// <param name="k">the k-ary of the tree (k/2) is where the in-order parent is visited.</param>
        public void TraverseInOrder(Action<BinaryTreeNode<T>> traverseAction, int k, Func<bool> cutoffSelector=null)
        {
            if (cutoffSelector != null && cutoffSelector()) return;

            int middle = k / 2;

            // Traverse left before current
            Left?.TraverseInPreOrder(traverseAction, cutoffSelector);

            traverseAction(this);

            // Traverse right after current
            Right?.TraverseInPreOrder(traverseAction, cutoffSelector);
        }

        /// <summary>
        /// Performs the given action on every visit to a node, using 
        /// post-order traversal logic.
        /// </summary>
        /// <param name="cutoffSelector">a function which returns a boolean indicating the traversal should stop</param>
        /// <param name="traverseAction">the action to apply on every visited node</param>
        public void TraverseInPostOrder(Action<BinaryTreeNode<T>> traverseAction, Func<bool> cutoffSelector=null)
        {
            if (cutoffSelector != null && cutoffSelector()) return;

            // Traverse both before current.
            Left?.TraverseInPreOrder(traverseAction, cutoffSelector);
            Right?.TraverseInPreOrder(traverseAction, cutoffSelector);

            traverseAction(this);
        }

        /// <summary>
        /// Traverses the nodes in pre-order and visits the node if the level selector
        /// checks of (e.g. () => 2, visits all nodes in level 2).
        /// Uses BFS algorithm.
        /// </summary>
        /// <param name="traverseAction">the action to apply on every visited node  (w/ level)</param>
        /// <param name="levelSelector">the selector which determines which nodes to visit based on level</param>
        public void TraverseInLevel(Action<BinaryTreeNode<T>, int> traverseAction, Func<int, bool> levelSelector, Func<int, bool> cutoff=null)
        {
            Queue<(BinaryTreeNode<T>, int)> visitQueue = new Queue<(BinaryTreeNode<T>,int)>();
            visitQueue.Enqueue((this, 0));

            while (visitQueue.Count > 0)
            {
                (BinaryTreeNode<T> node, int nodeLevel) = visitQueue.Dequeue();

                // Check if it should visit this node
                if (levelSelector(nodeLevel)) traverseAction(node, nodeLevel);

                // Queue all child nodes if below cutoff.
                if (cutoff != null && cutoff(nodeLevel)) continue;

                if (Left != null) visitQueue.Enqueue((Left, nodeLevel + 1));
                if (Right != null) visitQueue.Enqueue((Right, nodeLevel + 1));
            }
        }
        #endregion
        #endregion
        #region Enumerator
        public IEnumerator<BinaryTreeNode<T>> GetEnumerator()
        {
            if (Left != null) yield return Left;
            if (Right != null) yield return Right;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        #endregion

        public override string ToString()
        {
            return $"BinaryTreeNode({(Value == null ? "null" : Value)})";
        }
    }
}
