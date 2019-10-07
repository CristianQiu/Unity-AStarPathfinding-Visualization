using System;
using System.Collections.Generic;
using UnityEngine;

namespace Shared
{
    /// <summary>
    /// Simple class to register objects.
    /// </summary>
    public static class Registry
    {
        #region Private Attributes

        private const int MaxExpectedRegisteredObjs = 32;
        private static readonly Dictionary<Type, MonoBehaviour> Dictionary = new Dictionary<Type, MonoBehaviour>(MaxExpectedRegisteredObjs);

        #endregion

        #region Methods

        /// <summary>
        /// Insert the given object into the registry.
        /// </summary>
        /// <param name="obj"></param>
        /// <typeparam name="T"></typeparam>
        public static void Insert<T>(T obj) where T : MonoBehaviour
        {
            Type key = typeof(T);

            bool isThere = Dictionary.ContainsKey(key);

            if (!isThere)
                Dictionary.Add(key, obj);
            else
                Logger.LogWarningFormat("Can not register object of type {0} because there is already an instance registered.", key.Name);
        }

        /// <summary>
        /// Remove the given object from the registry.
        /// </summary>
        /// <param name="obj"></param>
        /// <typeparam name="T"></typeparam>
        public static void Remove<T>(T obj) where T : MonoBehaviour
        {
            Type key = typeof(T);

            bool success = Dictionary.Remove(key);

            if (!success)
                Logger.LogWarningFormat("Can not unregister object of type {0} because it was not found in the Registry.", key.Name);
        }

        /// <summary>
        /// Get the given object of type T from the dictionary.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T Get<T>() where T : MonoBehaviour
        {
            Type key = typeof(T);

            bool success = Dictionary.TryGetValue(key, out MonoBehaviour mono);

            T tObj = null;

            if (success)
                tObj = mono as T;
            else
                Logger.LogWarningFormat("Object of type {0} not found in the Registry. Returning null.", key.Name);

            return tObj;
        }

        #endregion
    }
}