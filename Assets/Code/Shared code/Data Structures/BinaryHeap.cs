namespace Shared
{
    /// <summary>
    /// A binary heap used to implement a priority queue where objects are ordered depending on
    /// certain values. Here is a good overview of the datastructure, as well as the time complexity
    /// of its operations: http://www.growingwiththeweb.com/data-structures/binary-heap/overview/
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BinaryHeap<T> where T : class, IBinaryHeapable<T>
    {
        #region Public Attributes

        public const int InvalidIndex = -1;

        #endregion

        #region Private Attributes

        private const int DefaultCapacity = 256;
        private int capacity = 0;
        private ulong totalItemsAdded = 0;

        #endregion

        #region Properties

        public int Count { get; private set; } = 0;
        public T[] Elements { get; private set; } = null;

        #endregion

        #region Initialization Methods

        /// <summary>
        /// Default constructor.
        /// </summary>
        public BinaryHeap() : this(DefaultCapacity)
        {
        }

        /// <summary>
        /// Additional constructor.
        /// </summary>
        /// <param name="initialCapacity"></param>
        public BinaryHeap(int initialCapacity)
        {
            capacity = initialCapacity;
            Elements = new T[initialCapacity];
            Count = 0;
        }

        #endregion

        #region Heap Methods

        /// <summary>
        /// Add an element to the heap and order it according to its priority.
        /// </summary>
        /// <param name="element"></param>
        public void Add(T element)
        {
            if (Count == capacity)
                DoubleElementsCapacity();

            // add it to the last position
            Elements[Count] = element;
            element.HeapIndex = Count;

            // order the element
            ShiftUp(element);

            // keep counting to be able to stablish properly the priorities
            totalItemsAdded++;
            element.NumHeapItem = totalItemsAdded;

            Count++;
        }

        /// <summary>
        /// Extract the object with most priority and reorder the heap.
        /// </summary>
        /// <returns></returns>
        public T ExtractRoot()
        {
            if (Count <= 0)
                return default;

            // get the root
            T min = Elements[0];

            // delete the root by placing the last element there
            Count--;
            T last = Elements[Count];
            SwapElements(min, last);

            // defaultify the spot from where we moved the last element
            Elements[Count] = default;

            // shift down from the root if there is still something
            if (Count > 0)
                ShiftDown(Elements[0]);

            // invalidate the heap index from what we extracted, so that we know it is no longer
            // inside it
            min.HeapIndex = InvalidIndex;
            min.NumHeapItem = 0;

            return min;
        }

        /// <summary>
        /// Update an element that has had its priority changed, and requires to be reordered. It
        /// must be manually called every time said value is changed.
        /// </summary>
        /// <param name="element"></param>
        public void UpdateElementWithChangedVal(T element)
        {
            if (element.HeapIndex == -1)
                return;

            ShiftDown(element);
            ShiftUp(element);
        }

        /// <summary>
        /// Clear the heap.
        /// </summary>
        public void Clear()
        {
            for (int i = 0; i < Count; ++i)
            {
                // set the index back to invalid so that we can look for it in the object T to know
                // that it is no longer inside the heap
                Elements[i].HeapIndex = InvalidIndex;
                Elements[i].NumHeapItem = 0;
                Elements[i] = default;
            }

            // reset counters
            totalItemsAdded = 0;
            Count = 0;
        }

        #endregion

        #region Useful Methods

        /// <summary>
        /// Double the capacity of the elements in case we run out of space, just like the generic
        /// list does.
        /// </summary>
        private void DoubleElementsCapacity()
        {
            capacity *= 2;
            T[] newElements = new T[capacity];

            Elements.CopyTo(newElements, 0);
            Elements = newElements;
        }

        /// <summary>
        /// Move an element up while the priority is greater than its parent.
        /// </summary>
        /// <param name="fromIndex"></param>
        private void ShiftUp(T element)
        {
            T parent = GetParent(element);

            // if there's a parent and has priority
            if (parent != default && element.HasPriorityToShiftUp(parent))
            {
                // swap both elements and keep going
                SwapElements(element, parent);
                ShiftUp(element);
            }
        }

        /// <summary>
        /// Moves an element down while the priority is lower than any of its children.
        /// </summary>
        /// <param name="element"></param>
        private void ShiftDown(T element)
        {
            // get children
            T left = GetLeftChild(element);
            T right = GetRightChild(element);
            T mostPriority = element;

            // calculate which child has the most priority
            if (left != default)
                mostPriority = left.HasPriorityToShiftDown(element) ? left : mostPriority;

            if (right != default)
                mostPriority = right.HasPriorityToShiftDown(mostPriority) ? right : mostPriority;

            // kind of hacky but it will work, since operator != is not allowed and no two elements
            // in the heap can have the same heap index swap with it if any of the children has priority
            if (mostPriority.HeapIndex != element.HeapIndex)
            {
                SwapElements(element, mostPriority);
                ShiftDown(element);
            }
        }

        /// <summary>
        /// Swap an element in the heap by another one.
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        private void SwapElements(T first, T second)
        {
            int firstIndex = first.HeapIndex;
            int secondIndex = second.HeapIndex;

            // swap them
            Elements[firstIndex] = second;
            Elements[secondIndex] = first;

            // change their indexes
            first.HeapIndex = secondIndex;
            second.HeapIndex = firstIndex;
        }

        #endregion

        #region Parent & Children Methods

        /// <summary>
        /// Get the parent of an element.
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        private T GetParent(T element)
        {
            int parentIndex = element.HeapIndex == 0 ? InvalidIndex : (element.HeapIndex - 1) / 2;

            return parentIndex != InvalidIndex ? Elements[parentIndex] : default;
        }

        /// <summary>
        /// Get the left child of an element.
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        private T GetLeftChild(T element)
        {
            int index = element.HeapIndex * 2 + 1;

            return index < Count ? Elements[index] : default;
        }

        /// <summary>
        /// Get the right child of an element.
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        private T GetRightChild(T element)
        {
            int index = element.HeapIndex * 2 + 2;

            return index < Count ? Elements[index] : default;
        }

        #endregion
    }
}