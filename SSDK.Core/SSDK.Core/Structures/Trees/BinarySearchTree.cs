using SSDK.Core.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSDK.Core.Structures.Trees
{
    /// <summary>
    /// A binary tree that maintains the root node is the smallest
    /// element. Tree nodes in a heap tree must not be modified directly.
    /// </summary>
    public class BinarySearchTree<T> : BinaryTree<T>
        where T : IComparable
    {
        #region Properties & Fields

        #endregion
        #region Methods
        #region Cloning
        public override BinaryTree<T> Clone(bool deep = true)
        {
            return (BinarySearchTree<T>)base.Clone(() => new BinarySearchTree<T>()
            {

            }, deep);
        }
        #endregion
        #region Modification
        public override BinaryTreeNode<T> Add(T element)
        {
            BinaryTreeNode<T> newNode = new BinaryTreeNode<T>(element);
            if (RootNode == null)
            {
                // New node is the new root node.
                RootNode = newNode;
                newNode.CacheHeight = 0; // Height 0 is new root node.
            }
            else
            {
                // Search for position to insert.
                BinaryTreeNode<T> searchNode = RootNode;
                while (searchNode != null)
                {
                    int comparison = element.CompareTo(searchNode.Value);

                    if (comparison < 0) // Element is less than
                    {
                        if (searchNode.Left == null)
                        {
                            // Insert below current node
                            searchNode.Left = newNode;
                            break;
                        }
                        else searchNode = searchNode.Left;
                    }
                    else if (comparison > 0) // Element is greater than
                    {
                        if (searchNode.Right == null)
                        {
                            // Insert below current node
                            searchNode.Right = newNode;
                            break;
                        }
                        else searchNode = searchNode.Right;
                    }
                    else
                        throw new ConflictingDataException("Element already exists in the BST.");
                }
            }
            return newNode;
        }

        public override void Remove(BinaryTreeNode<T> elementNode)
        {
            if (elementNode.IsLeafNode)
            {
                // Simply remove the node
                BinaryTreeNode<T> parentNode = elementNode;
                elementNode.Remove();
            }
            else
            {
                bool leftNull = elementNode.Left == null;
                bool rightNull = elementNode.Right == null;
                if(!leftNull && rightNull)
                {
                    // Only left exists, so replace current node with left.
                    elementNode.Left.SwapValue(elementNode);
                    elementNode.Right = elementNode.Left.Right;
                    elementNode.Left = elementNode.Left.Left;
                }
                else if (!rightNull && leftNull)
                {
                    // Only right exists, so replace current node with right.
                    elementNode.Right.SwapValue(elementNode);
                    elementNode.Left = elementNode.Right.Left;
                    elementNode.Right = elementNode.Right.Right;
                }
                else
                {
                    // Both children exist, replace current node with in-order successor.
                    BinaryTreeNode<T> inOrderSuccessor = elementNode.InOrderSuccessor;
                    inOrderSuccessor.SwapValue(elementNode);
                    Remove(inOrderSuccessor);
                }
            }


        }
        #endregion
        #region Searching
        public override BinaryTreeNode<T> Search(T searchFor)
        {
            BinaryTreeNode<T> current = RootNode;

            while (current != null)
            {
                if (current.Value.Equals(searchFor))
                {
                    return current;
                }

                if (searchFor.CompareTo(current.Value) < 0)
                {
                    // Move left
                    current = current.Left;
                }
                else
                {
                    // Move right
                    current = current.Right;
                }
            }

            return null; // Cannot find node.
        }
        #endregion
        #endregion
    }
}
