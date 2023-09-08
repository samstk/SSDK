using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSDK.Core.Structures.Trees
{
    /// <summary>
    /// An AVL tree is a balanced binary search tree
    /// (i.e. using the tree's Add and Remove methods, the height
    /// of any node in the tree differs by at most one)
    /// </summary>
    public class AVLSearchTree<T> : BinarySearchTree<T>
        where T : IComparable
    {
        #region Methods
        public void Restructure(BinaryTreeNode<T> searchNode)
        {
            BinaryTreeNode<T> z = null;
            int oldCount = AllNodes.Count;

            while (searchNode != null)
            {
                if (!searchNode.IsBalanced)
                {
                    z = searchNode;
                    break;
                }
                else searchNode = searchNode.ParentNode;
            }

            if (z == null) return; // No unbroken balance.

            // Find y, the child of z with the larger height.
            BinaryTreeNode<T> y = z.LargestChild;

            if (y == null) return; // No unbroken balance

            // Find x, the child of y with the larger height.
            BinaryTreeNode<T> x = y.LargestChild;
            if (x == null) return;
            if (y.IsLeft && x.IsLeft) // Check case one (left and left)
            {
                BinaryTreeNode<T>
                    t0 = x.Left,
                    t1 = x.Right,
                    t2 = y.Right,
                    t3 = z.Right;

                // Tri-node restructuring (y is new highest-parent)
                //
                //              z                                    y
                //             / \                                 /   \
                //            y   t3                              x     z
                //           / \            ->                   / \   / \
                //          x  t2                              t0  t1 t2 t3
                //         / \
                //       t0  t1  

                if (z.ParentNode != null) z.ParentNode.SwapNode(z, y);
                else RootNode = y;

                z.Left = t2;

                y.Left = x;
                y.Right = z;
            }
            else if (y.IsLeft && x.IsRight) // Check case two (left and right)
            {
                BinaryTreeNode<T>
                    t0 = y.Left,
                    t1 = x.Left,
                    t2 = x.Right,
                    t3 = z.Right;

                // Tri-node restructuring (y is new highest-parent)
                //
                //              z                                    x
                //             / \                                 /   \
                //            y   t3                              y     z
                //           / \            ->                   / \   / \
                //          t0   x                              t0 t1 t2 t3
                //              / \
                //             t1 t2  

                if (z.ParentNode != null) z.ParentNode.SwapNode(z, x);
                else RootNode = x;

                x.Left = y;
                x.Right = z;

                y.Left = t0;
                y.Right = t1;
                z.Left = t2;
                z.Right = t3;
            }
            else if (x.IsLeft) // Check case three (right and left)
            {
                BinaryTreeNode<T>
                    t0 = z.Left,
                    t1 = x.Left,
                    t2 = x.Right,
                    t3 = y.Right;

                // Tri-node restructuring (y is new highest-parent)
                //
                //              z                                    x
                //             / \                                 /   \
                //            t0  y                               z     y
                //               / \            ->               / \   / \
                //              x  t3                           t0 t1 t2 t3
                //             / \
                //            t1 t2  

                if (z.ParentNode != null) z.ParentNode.SwapNode(z, x);
                else RootNode = x;

                x.Left = z;
                x.Right = y;
                z.Right = t1;
                y.Left = t2;
            }
            else if (x.IsRight) // Check case four (right and right)
            {
                BinaryTreeNode<T>
                    t0 = z.Left,
                    t1 = y.Left,
                    t2 = x.Left,
                    t3 = x.Right;

                // Tri-node restructuring (y is new highest-parent)
                //
                //              z                                    y
                //             / \                                 /   \
                //            t0  y                               z     x
                //               / \            ->               / \   / \
                //              t1  x                           t0 t1 t2 t3
                //                 / \
                //                t2 t3  

                if (z.ParentNode != null)
                    z.ParentNode.SwapNode(z, y);
                else RootNode = y;

                y.Left = z;
                y.Right = x;
                z.Left = t0;
                z.Right = t1;
                x.Left = t2;
                x.Right = t3;
                if (z.ParentNode != null)
                    Restructure(z.ParentNode); // Continue for any additional inbalances.
            }
            
            
        }
        public override BinaryTreeNode<T> Add(T element)
        {
            // Add per normal BST
            BinaryTreeNode<T> newNode = base.Add(element);

            if(newNode.Depth < 2)
            {
                return newNode; // No need to restructure
            }

            // First find Z, the first unbalanced node encountered.
            BinaryTreeNode<T> zSearchNode = newNode.ParentNode;
            
            // Perform re-structure operation to node-to-root.
            Restructure(zSearchNode);
            
            return newNode;
        }
        #endregion
    }
}
