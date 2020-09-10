namespace Shared
{
    #region Initialization

    /// <summary>
    /// Interface that classes that can be initialized can implement.
    /// </summary>
    public interface IInitializable
    {
        /// <summary>
        /// Get whether the object has been initialized or not.
        /// </summary>
        /// <value></value>
        bool Initialized { get; }

        /// <summary>
        /// Initialize the object and get whether the call was successfuly executed.
        /// </summary>
        /// <param name="force"></param>
        /// <returns></returns>
        bool Init(bool force);
    }

    #endregion

    #region Binary Heap

    /// <summary>
    /// Interface that an element that is going to be stored in a binary heap must implement.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IBinaryHeapable<T>
    {
        /// <summary>
        /// The index at which the element is placed in the heap. It is set to -1 when it is not in
        /// the heap.
        /// </summary>
        /// <value></value>
        int HeapIndex { get; set; }

        /// <summary>
        /// The number of item that has been added to the heap since it started operating. It is set
        /// to 0 every time the object is extracted from the binary heap. The internal countdown of
        /// the heap is reset when it is cleared.
        /// </summary>
        /// <value></value>
        ulong NumHeapItem { get; set; }

        /// <summary>
        /// Used to get whether this object has priority over other object when being shift to the
        /// top of the heap. This can be used to store elements in ascendant or descendant order.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        bool HasPriorityToShiftUp(T other);

        /// <summary>
        /// Used to get whether this object has priority over other object when being shift to the
        /// bottom of the heap. This can be used to store elements in ascendant or descendant order.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        bool HasPriorityToShiftDown(T other);
    }

    #endregion
}