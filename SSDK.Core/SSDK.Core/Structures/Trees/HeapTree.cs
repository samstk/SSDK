using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SSDK.Core.Structures.Trees
{
    /// <summary>
    /// A heap tree that maintains the root node is the smallest
    /// element. Tree nodes in a heap tree must not be modified directly.
    /// </summary>
    public class HeapTree<T> : BinaryTree<T>
        where T : IComparable
    {
        #region Properties & Fields
        /// <summary>
        /// The node which contains the parent that the next
        /// node must be inserted to.
        /// </summary>
        private BinaryTreeNode<T> HeapInsertionNode;

        /// <summary>
        /// The node which contains the parent that the next node
        /// </summary>

        private BinaryTreeNode<T> HeapRemovalNode;
        #endregion

        #region Methods
        #region Cloning
        public override HeapTree<T> Clone(bool deep = true)
        {
            return (HeapTree<T>)base.Clone(() => new HeapTree<T>()
            {
                HeapInsertionNode = HeapInsertionNode,
                HeapRemovalNode = HeapRemovalNode
            }, deep);
        }
        #endregion
        #region Modification
        /// <summary>
        /// Invalid operation.
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        public override void SetRoot(T element)
        {
            throw new InvalidOperationException("Cannot set the root of a heap tree, the tree must be individually formed.");
        }
        public override BinaryTreeNode<T> Add(T element)
        {
            if (RootNode == null)
            {
                RootNode = new BinaryTreeNode<T>(element);
                HeapInsertionNode = RootNode;
                HeapRemovalNode = RootNode; // Update removal node
                return RootNode;
            }
            else
            {
                BinaryTreeNode<T> furthest = HeapInsertionNode;
                BinaryTreeNode<T> newNode = new BinaryTreeNode<T>(element);
                BinaryTreeNode<T> node = newNode;
                HeapRemovalNode = node; // Update removal node
                if(HeapInsertionNode.Left == null)
                {
                    HeapInsertionNode.Left = node;
                    // Keep current insertion point
                }
                else
                {
                    HeapInsertionNode.Right = node;

                    // Change insertion point to neighbour.
                    BinaryTreeNode<T> sibling = HeapInsertionNode.Sibling;
                    
                    if(HeapInsertionNode.ParentNode == null)
                    { 
                        // Since we are inserting to the root (and this is right), we must form a new level.
                        HeapInsertionNode = RootNode.LeftMostNode;
                    }
                    else
                    {
                        if (HeapInsertionNode.IsRight)
                        {
                            // We need to find the next neighbour on the same depth.
                            BinaryTreeNode<T> current = HeapInsertionNode;

                            // Calculate actions to root.
                            while(current != null)
                            {
                                if (current.IsLeft)
                                {
                                    // We must navigate to the sibling right node, down to the furthest left node.
                                    HeapInsertionNode = current.Sibling.LeftMostNode;
                                    break;
                                }
                                current = current.ParentNode;
                            }

                            if (current == null) // Must form a new level.
                                HeapInsertionNode = RootNode.LeftMostNode;
                        }
                        else
                        {
                            HeapInsertionNode = sibling; // Must now insert into sibling.
                        }
                    }
                }
                // If new node is less than parent, then heap order is disturbed.
                BinaryTreeNode<T> parent = furthest;
                while(parent != null && node.Value.CompareTo(parent.Value) < 0)
                {
                    // Up-heap algorithm
                    // Simply swap the values of the node.
                    node.SwapValue(parent);
                    node = parent;
                    parent = parent.ParentNode;
                }
                return newNode;
            }
        }


        /// <summary>
        /// Removes the root node from the heap, returning the smallest value in the tree.
        /// </summary>
        /// <returns>a shallow copy of the root node of the tree</returns>
        public BinaryTreeNode<T> Remove()
        {
            if (RootNode == null) return null;
            return Remove(RootNode.Value);
        }

        public override void Remove(BinaryTreeNode<T> node)
        {
            if (RootNode == null || HeapRemovalNode == null)
            {
                return;
            }

            // Update insertion reference
            HeapInsertionNode = HeapRemovalNode.ParentNode;

            // Replace root key with last node.
            HeapRemovalNode.SwapValue(node);

            BinaryTreeNode<T> removedNode = HeapRemovalNode;

            // Find next removal node
            // We need to find the next neighbour on the same depth.
            BinaryTreeNode<T> current = HeapRemovalNode;

            // Remove last node.
            if (removedNode == RootNode)
            {
                RootNode = null;
                HeapInsertionNode = null;
                HeapRemovalNode = null;
                return;
            }
            else
            {
                removedNode.Remove();

                // Restore heap-order property.
                Downheap(node);
            }

            // Calculate next removal node.
            while (current != null)
            {
                if (current.IsRight)
                {
                    BinaryTreeNode<T> sibling = current.Sibling;
                    if (sibling!=null)
                    {
                        // Next removal is sibling's right-most node.
                        HeapRemovalNode = sibling.RightMostNode;
                        break;
                    }
                }
                current = current.ParentNode;
            }

            if (current == null) // Must remove from last level.
                HeapRemovalNode = RootNode.RightMostNode;

        }

        /// <summary>
        /// Performs the downheap algorithm on the
        /// given node.
        /// </summary>
        /// <param name="node">the node to order</param>
        private static void Downheap(BinaryTreeNode<T> node)
        {
            // Check if heap order is volated.
            bool violated = node.Left != null && node.Left.Value.CompareTo(node.Value) < 0
                            || node.Right != null && node.Right.Value.CompareTo(node.Value) < 0;
            
            if(violated)
            {
                if(node.Right == null)
                {
                    node.SwapValue(node.Left);
                    Downheap(node.Left);
                }
                else if (node.Left == null)
                {
                    node.SwapValue(node.Right);
                    Downheap(node.Right);
                }
                else
                {
                    int comparison = node.Left.Value.CompareTo(node.Right.Value);
                    if(comparison <= 0)
                    {
                        // Left is smaller or equal
                        node.SwapValue(node.Left);
                        Downheap(node.Left);
                    }
                    else
                    {
                        // Right is smaller
                        node.SwapValue(node.Right);
                        Downheap(node.Right);
                    }
                }
            }
        }
        #endregion
        #endregion


        public override string ToString()
        {
            return $"Heap({Size})";
        }
    }
}
