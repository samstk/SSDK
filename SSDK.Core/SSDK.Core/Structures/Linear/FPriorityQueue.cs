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
    /// than the native priority queue.
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
        /// Dictionary used for storing and sorting queue values
        /// </summary>
        private SortedDictionary<TPriority, List<TElement>> _QueueDict = new ();
        
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
        public void Enqueue(TElement element, TPriority priority)
        {
            if (_QueueDict.ContainsKey(priority))
                _QueueDict[priority].Add(element);
            else _QueueDict.Add(priority, CreateListFor(element));

            _HeapCount++;
        }

        /// <summary>
        /// Creates a list with the element already enqueued (added).
        /// </summary>
        /// <param name="element">the element to initial add</param>
        /// <returns>the list</returns>
        private List<TElement> CreateListFor(TElement element)
        {
            List<TElement> list = new List<TElement>();
            list.Add(element);
            return list;
        }

        /// <summary>
        /// Dequeues a single element from the priority queue. 
        /// </summary>
        /// <returns>the element that has the highest (lowest) priority</returns>
        public TElement Dequeue()
        {
            if (_HeapCount == 0) throw new InvalidOperationException("No items queued");

            (TPriority priority, List<TElement> elements) = _QueueDict.First();
            TElement element = elements[0];
            if (elements.Count == 1)
            {
                _QueueDict.Remove(priority); // Leave for GC cleanup.
            }
            else elements.RemoveAt(0); // Remove immediately
            
            _HeapCount--;
            return element;
        }
        
        /// <summary>
        /// Removes the given element from the queue, assuming it is exists.
        /// </summary>
        /// <param name="element">the element to remove</param>
        /// <param name="priority">the priority it was added with</param>
        public void Remove(TElement element, TPriority priority)
        {
            List<TElement> elements = _QueueDict[priority];
            if (elements.Count == 1)
                _QueueDict.Remove(priority); // Leave for GC cleanup.
            else elements.Remove(element);

            _HeapCount--;
        }

        /// <summary>
        /// Returns true if the queue contains the given key.
        /// </summary>
        /// <param name="element">the element to check</param>
        /// <param name="priority">the priority of the element that was used to enqueue</param>
        /// <returns>true if the queue contains the element</returns>
        public bool ContainsKey(TElement element, TPriority priority)
        {
            if (_QueueDict.ContainsKey(priority))
                return _QueueDict[priority].Contains(element);
            return false;
        }
        /// <summary>
        /// Sets the priority of a given queue item, assuming it already exists in the queue with the old
        /// priority.
        /// </summary>
        /// <param name="item">the item to set priority of</param>
        /// <param name="newPriority">the new priority of the item</param>
        public void SetPriority(TElement element, TPriority oldPriority, TPriority newPriority)
        {
            // Must remove item and then re-add it.
            Remove(element, oldPriority);
            Enqueue(element, newPriority);
        }
        #endregion
        #endregion
    }
}
