using System.Diagnostics;

namespace Shared
{
    /// <summary>
    /// Class to do asserting stuff. It is highly coupled with the Logger.
    /// </summary>
    public static class Asserter
    {
        #region Methods

        /// <summary>
        /// Assert that a certain condition is met and send a message to Unity's console if the condition is not met.
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="onFailMsg"></param>
        [Conditional("DEBUG_ASSERT")]
        public static void Assert(bool condition, string onFailMsg)
        {
            if (!condition)
                Logger.LogAssertion(onFailMsg);
        }

        /// <summary>
        /// Assert that a certain condition is met and send a formatted message to Unity's console if the condition is not met.
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="onFailMsg"></param>
        /// <param name="args"></param>
        [Conditional("DEBUG_ASSERT")]
        public static void AssertFormat(bool condition, string onFailMsg, params object[] args)
        {
            if (!condition)
                Logger.LogAssertionFormat(onFailMsg, args);
        }

        #endregion
    }
}