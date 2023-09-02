using SSDK.Core.Helpers;
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

        public override string ToString()
        {
            return $"Binary-Tree({Size})";
        }
        #endregion

        #region Methods
        #region Visualisation
        #region Visualization
        /// <summary>
        /// Gets the text representation of the binary tree, 
        /// </summary>
        public string TextVisualisation
        {
            get
            {
                List<TreeNode<T>> nodes = AllNodes;
                Dictionary<TreeNode<T>, string> representations = new Dictionary<TreeNode<T>, string>();
                int maxReprLength = 0;
                // Create representations and check max length of representation.
                for (int i = 0; i < nodes.Count; i++)
                {
                    string repr = (
                        Nullable.GetUnderlyingType(typeof(T)) != null 
                        && nodes[i].Value == null
                        ) ? "<null>" : nodes[i].Value.ToString();
                    if(repr.Length > maxReprLength)
                    {
                        maxReprLength = repr.Length;
                    }
                    representations.Add(nodes[i], repr);
                }

                string representation = "";

                int padding = 2;
                int allowedWidth = (maxReprLength + padding * 2) * (int)Math.Pow(2, Height);

                for(int h = 0; h <= Height; h++)
                {
                    string strAt = "".PadLeft(allowedWidth, ' ');
                    string strUnder = "".PadLeft(allowedWidth, ' ');
                    int i = 1;
                    int spacePerCell = (int)(allowedWidth / Math.Pow(2, h));
                    TraverseInLevel((node, level) =>
                    {
                        strAt = strAt.InsertOverwriteCenter(i * spacePerCell - spacePerCell/2, representations[node]);
                        if (node.Left != null && node.Right != null)
                        {
                            strUnder = strUnder.InsertOverwriteCenter(i * spacePerCell - spacePerCell / 2, @"/ \");
                        }
                        else if (node.Left != null)
                        {
                            strUnder = strUnder.InsertOverwriteCenter(i * spacePerCell - spacePerCell / 2, @"/  ");
                        }
                        else if (node.Right != null)
                        {
                            strUnder = strUnder.InsertOverwriteCenter(i * spacePerCell - spacePerCell / 2, @"  \");
                        }
                        i++;
                    }, (l) => l == h, (l) => l > h);
                    representation += strAt + "\n" + strUnder + "\n";
                }

                return representation;
            }
        }
        #endregion
        #endregion
        #endregion
    }
}
