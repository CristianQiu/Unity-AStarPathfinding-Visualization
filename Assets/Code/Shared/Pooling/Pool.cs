using System.Collections.Generic;

namespace Shared
{
    /// <summary>
    /// Class to create and manage a pool of common objects preallocated into memory.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Pool<T> where T : class, IPoolable<T>, new()
    {
        #region Private Attributes

        private const int DefaultSize = 256;
        private const int NumAddedWhenAllowNews = 1;

        private readonly List<T> freeObjs = null;
        private readonly List<T> usedObjs = null;

        #endregion

        #region Properties

        public bool AllowNews { get; set; } = true;
        public float NumFree { get { return freeObjs.Count; } }
        public float NumUsed { get { return usedObjs.Count; } }
        public float NumTotal { get { return freeObjs.Count + usedObjs.Count; } }

        #endregion

        #region Initialization Methods

        /// <summary>
        /// Default constructor.
        /// </summary>
        public Pool() : this(DefaultSize, true)
        {

        }

        /// <summary>
        /// Additional constructor.
        /// </summary>
        /// <param name="size"></param>
        /// <param name="allowNews"></param>
        public Pool(int size, bool allowNews)
        {
            AllowNews = allowNews;

            freeObjs = new List<T>(size);
            usedObjs = new List<T>(size);

            // prepare the objs
            AddFreeObjects(size);
        }

        /// <summary>
        /// Add the given quantity of free objects to the pool of free objects.
        /// </summary>
        /// <param name="quantity"></param>
        private void AddFreeObjects(int quantity)
        {
            for (int i = 0; i < quantity; i++)
            {
                T newObj = new T();
                newObj.Reset();
                freeObjs.Add(newObj);
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Get a free object and move it to the list of used objects.
        /// </summary>
        /// <returns></returns>
        public T GetOne()
        {
            T obj = null;

            if (freeObjs.Count <= 0)
            {
                if (!AllowNews)
                    Logger.LogWarning("Trying to get a freed object, but the pool is not allowed to grow in size and no more objects are free to return.");
                else
                    AddFreeObjects(NumAddedWhenAllowNews);
            }

            if (freeObjs.Count > 0)
            {
                // get the last one and initialize it
                int last = freeObjs.Count - 1;
                obj = freeObjs[last];
                obj.Init(true);

                // remove it from the freed objects and add it to the used ones
                freeObjs.RemoveAt(last);
                usedObjs.Add(obj);
            }

            return obj;
        }

        /// <summary>
        /// Free the given object from the list of used objects.
        /// </summary>
        /// <param name="obj"></param>
        public void FreeOne(T obj)
        {
            bool success = usedObjs.Remove(obj);

            if (success)
            {
                // reset before it is added to the list of free objs
                obj.Reset();
                freeObjs.Add(obj);
            }
            else
            {
                Logger.LogWarningFormat("Could not find the object that you wanted to free: {0}.", obj.ToString());
            }
        }

        /// <summary>
        /// Free all the objects being used. This is an O(N) operation where N is the number of objects being used.
        /// </summary>
        public void FreeAll()
        {
            // do it in O(N)
            for (int i = usedObjs.Count - 1; i <= 0; i--)
            {
                T obj = usedObjs[i];
                usedObjs.RemoveAt(i);
                freeObjs.Add(obj);
            }
        }

        #endregion
    }
}