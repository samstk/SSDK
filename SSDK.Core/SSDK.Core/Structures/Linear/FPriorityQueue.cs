using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SSDK.Core.Structures.Linear
{
    /// <summary>
    /// A SSDK implementation of priority queue (min-heap), which has more flexibilility but is slower
    /// than the native priority queue for basic functions.
    /// </summary>
    /// <typeparam name="TElement">the type of the element</typeparam>
    /// <typeparam name="TPriority">the type of the priority</typeparam>
    public sealed class FPriorityQueue<TElement, TPriority>
        where TPriority : IComparable
    {
        #region Properties & Fields
        /// <summary>
        /// Gets the amount of 
        /// </summary>
        public int Count
        {
            get
            {
                return _HeapCount;
            }
        }

        /// <summary>
        /// Count of all elements in the queue.
        /// </summary>
        private int _HeapCount = 0;

        /// <summary>
        /// The array that contains the heap.
        /// </summary>
        private FPriorityQueueItem<TElement, TPriority>[] _Items = new FPriorityQueueItem<TElement, TPriority>[0];
        
        #endregion
        #region Methods
        #region Constructor
        public FPriorityQueue()
        {
            
        }
        #endregion

        #region Queue
        /// <summary>
        /// Enqueues the element into the priority queue with the given priority.
        /// </summary>
        /// <param name="element">the element to enqueue</param>
        /// <param name="priority">the priority to enqueue with</param>
        /// <returns>an item which can be used to update the particular queue item</returns>
        public FPriorityQueueItem<TElement, TPriority> Enqueue(TElement element, TPriority priority)
        {     
            FPriorityQueueItem<TElement, TPriority> newItem = new FPriorityQueueItem<TElement, TPriority>(element, priority, _HeapCount);

            EnqueueItem(newItem);

            return newItem;
        }

        /// <summary>
        /// Enqueues the item into the priority queue.
        /// </summary>
        /// <param name="item">the item to enqueue</param>
        public void EnqueueItem(FPriorityQueueItem<TElement, TPriority> item)
        {
            if (_HeapCount >= _Items.Length)
            {
                // Resize the items array
                FPriorityQueueItem<TElement, TPriority>[] newItems = new FPriorityQueueItem<TElement, TPriority>[2 * _Items.Length + 1];
                Array.Copy(_Items, newItems, _Items.Length);
                _Items = newItems;
            }

            // Inserting into a heap, by placing at end
            _Items[_HeapCount] = item;

            // Maintain heap property, by upheap.
            Upheap(item);

            // Set count and next heap insertion index.
            _HeapCount++;
        }



        /// <summary>
        /// Maintains the heap property by moving the item up if applicable.
        /// </summary>
        /// <param name="itemToDownheap">the item to upheap</param>
        private void Upheap(FPriorityQueueItem<TElement, TPriority> itemToUpheap)
        {
            int index = itemToUpheap.HeapIndex;
            TPriority priority = itemToUpheap.Priority;

            while (index > 0)
            {
                // Compare parent and move up if necessary.
                int parentIndex = (index - 1) / 2;
                FPriorityQueueItem<TElement, TPriority> parentItem = _Items[parentIndex];
                if (parentItem.Priority.CompareTo(itemToUpheap.Priority) >= 0)
                {
                    SwapItem(itemToUpheap, parentItem);
                    index = parentIndex;
                }
                else break; // No upheaps left.
            }
        }
        
        /// <summary>
        /// Swaps two items in the queue.
        /// </summary>
        /// <param name="item1"></param>
        /// <param name="item2"></param>
        private void SwapItem(FPriorityQueueItem<TElement, TPriority> item1, FPriorityQueueItem<TElement, TPriority> item2)
        {
            int index1 = item1.HeapIndex;
            int index2 = item2.HeapIndex;
            _Items[index1] = item2;
            _Items[index2] = item1;
            item2.HeapIndex = index1;
            item1.HeapIndex = index2;
        }
        /// <summary>
        /// Maintains the heap property by moving the item down if applicable.
        /// </summary>
        /// <param name="itemToDownheap">the item to downheap</param>
        private void Downheap(FPriorityQueueItem<TElement, TPriority> itemToDownheap)
        {
            int index = itemToDownheap.HeapIndex;
            int endOfInternalNodesIndex = (_HeapCount - 1) / 2;
            
            while(index <= endOfInternalNodesIndex)
            {
                int leftChildIndex = index * 2 + 1;
                int rightChildIndex = index * 2 + 2;

                // Calculate smallest child
                FPriorityQueueItem<TElement, TPriority> leftChild = _Items[leftChildIndex];
                FPriorityQueueItem<TElement, TPriority> rightChild = _Items[rightChildIndex];
                int smallestChildIndex = -1;
                if (leftChild != null) smallestChildIndex = leftChildIndex;
                if (rightChild != null && rightChild.Priority.CompareTo(leftChild.Priority) < 0)
                    smallestChildIndex = rightChildIndex;

                if (smallestChildIndex == -1) break; // No updates to be done.

                // Swap smallest child if smaller than this.
                FPriorityQueueItem<TElement, TPriority> smallestChild = _Items[smallestChildIndex];

                if (itemToDownheap.Priority.CompareTo(smallestChild.Priority) > 0)
                {
                    SwapItem(itemToDownheap, smallestChild);
                    index = smallestChildIndex;
                }
                else break; // No updates to be done.
            }
        }

        /// <summary>
        /// Dequeues a single element from the priority queue. 
        /// </summary>
        /// <returns>the element that has the highest (lowest) priority</returns>
        public TElement Dequeue()
        {
            return DequeueItem().Element;
        }

        /// <summary>
        ///Dequeues a single queue item from the priority queue. 
        /// </summary>
        /// <returns>the item that has the highest (lowest) priority</returns>
        /// <exception cref="InvalidOperationException">occurs when attempting to remove from an empty queue</exception>
        public FPriorityQueueItem<TElement, TPriority> DequeueItem()
        {
            if (_HeapCount == 0) throw new InvalidOperationException("No items queued");

            FPriorityQueueItem<TElement, TPriority> item = _Items[0];

            Remove(item);

            return item;
        }
        /// <summary>
        /// Removes the given element from the queue, assuming it is exists.
        /// </summary>
        /// <param name="queueItem">the item to remove</param>
        public void Remove(FPriorityQueueItem<TElement, TPriority> queueItem)
        {
            // Swap the given item to the end of the heap.
            _HeapCount--;
            if (queueItem.HeapIndex == _HeapCount) return;
            FPriorityQueueItem<TElement, TPriority> replacement = _Items[_HeapCount];
            SwapItem(queueItem, replacement);
            _Items[_HeapCount] = null;
            // Downheap to maintain heap property
            Downheap(replacement);
        }

        /// <summary>
        /// Sets the priority of a given queue item, assuming it already exists in the queue with the old
        /// priority.
        /// </summary>
        /// <param name="item">the item to set priority of</param>
        /// <param name="newPriority">the new priority of the item</param>
        public void SetPriority(FPriorityQueueItem<TElement, TPriority> item, TPriority newPriority)
        {
            // Must remove item and then re-add it.
            Remove(item);
            item.Priority = newPriority;
            EnqueueItem(item);
        }
        #endregion
        #endregion
    }

    /// <summary>
    /// An item within a flexible priority queue, which contains the element, the priority,
    /// and the index in which it is stored.
    /// </summary>
    /// <typeparam name="TElement"></typeparam>
    /// <typeparam name="TPriority"></typeparam>
    public sealed class FPriorityQueueItem<TElement, TPriority>
    {
        /// <summary>
        /// The element stored in the item.
        /// </summary>
        public TElement Element;

        /// <summary>
        /// The priority of the item.
        /// Use SetPriority in priority queue to update priority.
        /// </summary>
        public TPriority Priority { get; internal set; }
        
        /// <summary>
        /// The index inside the queue heap
        /// </summary>
        public int HeapIndex { get; internal set; }

        /// <summary>
        /// Create a new queue item with the given arguments.
        /// </summary>
        /// <param name="element">the element stored in the item</param>
        /// <param name="priority">the priority of the item</param>
        /// <param name="heapIndex">the index the item is stored in the heap</param>
        public FPriorityQueueItem(TElement element, TPriority priority, int heapIndex)
        {
            Element = element;
            Priority = priority;
            HeapIndex = heapIndex;
        }
    }
}
