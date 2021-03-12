using UnityEngine;

namespace Shared
{
    /// <summary>
    /// Class that adds singleton functionality to a MonoBehaviour attached to a Unity GameObject.
    /// It is only meant to be used at runtime, not for custom editors.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [DisallowMultipleComponent]
    public abstract class MonoBehaviourSingleton<T> : MonoBehaviour, IInitializable where T : MonoBehaviourSingleton<T>
    {
        #region Private Attributes

        private static T instance = null;

        #endregion

        #region Properties

        public static T Instance
        {
            get
            {
#if UNITY_EDITOR
                // show a warning if called from the editor and not playing
                if (!Application.isPlaying)
                {
                    Logger.LogWarningFormat("Trying to access a MonoBehaviourSingleton {0} in the editor while not playing. This is not allowed.", typeof(T).Name);
                    return null;
                }
#endif
                // if not yet created try to find one
                if (instance == null)
                {
                    string name = typeof(T).Name;
                    T[] instances = FindObjectsOfType<T>();
                    int length = instances.Length;

                    if (instances != null && length > 0)
                    {
                        instance = instances[0];

                        // deleted script duplicates will be restored once the game stops, let's
                        // show a warning before
                        if (length > 1)
                            Logger.LogWarningFormat("MonoBehaviourSingleton {0} has several instanced scripts across the scene and some of them have been deleted. It is HIGHLY recommended that you only leave one instance of the script you want to use.", name);

                        // now delete any duplicate
                        for (int i = 1; i < length; i++)
                        {
                            T inst = instances[i];
                            Logger.LogWarningFormat("Destroying MonoBehaviourSingleton {0} duplicate...", inst.name);
                            Utils.DestroyProper(inst);
                        }
                    }

                    // none found, create a new one
                    if (instance == null)
                    {
                        GameObject go = new GameObject(name);
                        instance = go.AddComponent<T>();
                    }

                    instance.Init(true);
                }

                return instance;
            }
        }

        public bool Initialized { get; private set; } = false;

        protected abstract bool DestroyOnLoad { get; }

        #endregion

        #region MonoBehaviour Methods

        protected virtual void Awake()
        {
            Instance.Init(false);
        }

        #endregion

        #region Initialization Methods

        /// <summary>
        /// Called the very first time the singleton instance is accessed, and thus, lazily
        /// instanced. This is automatically called in Awake() too.
        /// </summary>
        /// <param name="force"></param>
        /// <returns></returns>
        public virtual bool Init(bool force)
        {
            if (Initialized && !force)
                return false;

            Utils.ResetTransformLocal(transform);
            if (!instance.DestroyOnLoad)
                DontDestroyOnLoad(instance);

            Initialized = true;

            return true;
        }

        #endregion
    }
}