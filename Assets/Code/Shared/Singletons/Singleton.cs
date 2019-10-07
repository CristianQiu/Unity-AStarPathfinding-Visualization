using System;

namespace Shared
{
    /// <summary>
    /// Simple class to provide singleton functionality to a derived class. This singleton is fully thread safe using .Net framework 4.0 (or above) features.
    /// The implementation and explanation can be found here: https://www.codeproject.com/Articles/572263/%2FArticles%2F572263%2FA-Reusable-Base-Class-for-the-Singleton-Pattern-in
    /// For a non generic class singleton creation see: https://docs.microsoft.com/en-us/previous-versions/msp-n-p/ff650316(v=pandp.10)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class Singleton<T> : IInitializable where T : Singleton<T>
    {
        #region Private Attributes

        private static readonly Lazy<T> LazyObj = new Lazy<T>(CreateSingleton, true);

        #endregion

        #region Properties

        public static T Instance
        {
            get
            {
                T instance = LazyObj.Value;

                if (!instance.Initialized && LazyObj.IsValueCreated)
                    instance.Init(true);

                return instance;
            }
        }

        public bool Initialized { get; private set; } = false;

        #endregion

        #region Initialization Methods

        /// <summary>
        /// Create the singleton instance.
        /// </summary>
        /// <returns></returns>
        private static T CreateSingleton()
        {
            return Activator.CreateInstance(typeof(T), true) as T;
        }

        /// <summary>
        /// Called right after the singleton instance is created, useful to attach initialization code.
        /// </summary>
        /// <param name="force"></param>
        /// <returns></returns>
        public virtual bool Init(bool force)
        {
            if (Initialized && !force)
                return false;

            Initialized = true;

            return true;
        }

        #endregion
    }
}