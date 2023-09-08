using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SSDK.Core.Structures.Trees
{
    /// <summary>
    /// An AVL tree is a balanced binary search tree
    /// (i.e. using the tree's Add and Remove methods, the height
    /// of any node in the tree differs by at most one)
    /// </summary>
    public class SplaySearchTree<T> : BinarySearchTree<T>
        where T : IComparable
    {
        #region Methods
        public override BinaryTreeNode<T> Add(T element)
        {
            // Add per normal BST
            BinaryTreeNode<T> newNode = base.Add(element);

            if (RootNode == newNode)
                return newNode; // No need to splay

            Splay(newNode);

            return newNode;
        }

        /// <summary>
        /// Splays the given node to the root.
        /// </summary>
        /// <param name="x">the node to splay to the root</param>
        private void Splay(BinaryTreeNode<T> x)
        {
            while (x != RootNode) // Continue to splay until at root
            {
                BinaryTreeNode<T> parent = x.ParentNode;
                if (parent == null) return; // At root

                if (x.IsLeft)
                { // Right rotation
                    BinaryTreeNode<T>
                        t1 = x.Left,
                        t2 = x.Right,
                        t3 = parent.Right;

                    // Splay restructuring
                    //
                    //              p                                    x
                    //             / \                                 /  \
                    //            x   t3                              t1   p
                    //           / \                ->                    / \
                    //          t1  t2                                   t2 t3

                    // Swap x and parent
                    if (parent.ParentNode != null)
                    {
                        parent.ParentNode.SwapNode(parent, x);
                    }
                    else RootNode = x;

                    x.Left = t1;
                    x.Right = parent;
                    parent.Left = t2;
                    parent.Right = t3;
                }
                else // Left rotation
                {
                    BinaryTreeNode<T>
                        t1 = parent.Left,
                        t2 = x.Left,
                        t3 = x.Right;

                    // Splay restructuring
                    //
                    //              p                                    x
                    //             / \                                 /  \
                    //           t1   x                               p   t3 
                    //               / \            ->               / \
                    //              t2  t3                          t1 t2

                    // Swap x and parent
                    if (parent.ParentNode != null)
                    {
                        parent.ParentNode.SwapNode(parent, x);
                    }
                    else RootNode = x;

                    parent.Left = t1;
                    parent.Right = t2;
                    x.Right = t3;
                    x.Left = parent;
                }
            }
        }

        public override void Remove(BinaryTreeNode<T> elementNode)
        {
            BinaryTreeNode<T> parentNode = elementNode.ParentNode;
            
            // Remove as per normal BST
            base.Remove(elementNode);

            // Splay previous parent to root
            if (parentNode != null) Splay(parentNode);
        }

        public override BinaryTreeNode<T> Search(T searchFor)
        {
            BinaryTreeNode<T> searchResult = base.Search(searchFor);

            // Splay to root
            if (searchResult != null)
                Splay(searchResult);

            return searchResult;
        }
        #endregion
    }
}
