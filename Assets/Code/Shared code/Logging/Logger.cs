using System.Diagnostics;
using Debug = UnityEngine.Debug;

namespace Shared
{
    /// <summary>
    /// Class wrapping Unity's log system to add functionality on top of it.
    /// </summary>
    public static class Logger
    {
        #region Public Attributes

        public static LogLvl minimumLvl = LogLvl.Info;

        #endregion

        #region Private Attributes

        private const LogChannel DefaultChannel = LogChannel.Misc;

        #endregion

        #region Logging Methods

        /// <summary>
        /// Log a message in the given channel and level.
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="channel"></param>
        /// <param name="level"></param>
        [Conditional("DEBUG_LOG")]
        private static void Log(string msg, LogChannel channel, LogLvl level)
        {
            // cease log that we do not want
            if ((int)level < (int)minimumLvl || !LogChannels.IsChannelActive(channel))
                return;

            string channelStr = LogChannels.GetChannelName(channel);
            string finalMsg = string.Format("[ {0} ] - {1}", channelStr, msg);

            switch (level)
            {
                case LogLvl.Info:
                    Debug.Log(finalMsg);
                    break;

                case LogLvl.Warning:
                    Debug.LogWarning(finalMsg);
                    break;

                case LogLvl.Error:
                    Debug.LogError(finalMsg);
                    break;

                case LogLvl.Assertion:
                    Debug.LogAssertion(finalMsg);
                    break;

                case LogLvl.Exception:
                    // TODO: Implementation
                    break;

                default:
                    Debug.LogFormat("The log level {0} does not exist", level.ToString());
                    break;
            }
        }

        /// <summary>
        /// Log an info message in the default channel (Misc).
        /// </summary>
        /// <param name="msg"></param>
        [Conditional("DEBUG_LOG")]
        public static void Log(string msg)
        {
            Log(msg, DefaultChannel, LogLvl.Info);
        }

        /// <summary>
        /// Log a warning message in the default channel (Misc).
        /// </summary>
        /// <param name="msg"></param>
        [Conditional("DEBUG_LOG")]
        public static void LogWarning(string msg)
        {
            Log(msg, DefaultChannel, LogLvl.Warning);
        }

        /// <summary>
        /// Log an error message in the default channel (Misc).
        /// </summary>
        /// <param name="msg"></param>
        [Conditional("DEBUG_LOG")]
        public static void LogError(string msg)
        {
            Log(msg, DefaultChannel, LogLvl.Error);
        }

        /// <summary>
        /// Log an assertion message in the default channel (Misc).
        /// </summary>
        /// <param name="msg"></param>
        [Conditional("DEBUG_LOG")]
        public static void LogAssertion(string msg)
        {
            Log(msg, DefaultChannel, LogLvl.Assertion);
        }

        /// <summary>
        /// Log an info message in the given channel.
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="channel"></param>
        [Conditional("DEBUG_LOG")]
        public static void Log(string msg, LogChannel channel)
        {
            Log(msg, channel, LogLvl.Info);
        }

        /// <summary>
        /// Log a warning message in the given channel.
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="channel"></param>
        [Conditional("DEBUG_LOG")]
        public static void LogWarning(string msg, LogChannel channel)
        {
            Log(msg, channel, LogLvl.Warning);
        }

        /// <summary>
        /// Log an error message in the given channel.
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="channel"></param>
        [Conditional("DEBUG_LOG")]
        public static void LogError(string msg, LogChannel channel)
        {
            Log(msg, channel, LogLvl.Error);
        }

        /// <summary>
        /// Log an assertion message in the given channel.
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="channel"></param>
        [Conditional("DEBUG_LOG")]
        public static void LogAssertion(string msg, LogChannel channel)
        {
            Log(msg, channel, LogLvl.Assertion);
        }

        #endregion

        #region Log Format Methods

        /// <summary>
        /// Log a formatted message in the given channel and level.
        /// </summary>
        /// <param name="format"></param>
        /// <param name="channel"></param>
        /// <param name="level"></param>
        /// <param name="args"></param>
        [Conditional("DEBUG_LOG")]
        private static void LogFormat(string format, LogChannel channel, LogLvl level, params object[] args)
        {
            Log(string.Format(format, args), channel, level);
        }

        /// <summary>
        /// Log a formatted info message in the default channel.
        /// </summary>
        /// <param name="format"></param>
        /// <param name="args"></param>
        [Conditional("DEBUG_LOG")]
        public static void LogFormat(string format, params object[] args)
        {
            LogFormat(format, DefaultChannel, LogLvl.Info, args);
        }

        /// <summary>
        /// Log a formatted warning message in the default channel (Misc).
        /// </summary>
        /// <param name="format"></param>
        /// <param name="args"></param>
        [Conditional("DEBUG_LOG")]
        public static void LogWarningFormat(string format, params object[] args)
        {
            LogFormat(format, DefaultChannel, LogLvl.Warning, args);
        }

        /// <summary>
        /// Log a formatted error message in the default channel (Misc).
        /// </summary>
        /// <param name="format"></param>
        /// <param name="args"></param>
        [Conditional("DEBUG_LOG")]
        public static void LogErrorFormat(string format, params object[] args)
        {
            LogFormat(format, DefaultChannel, LogLvl.Error, args);
        }

        /// <summary>
        /// Log a formatted assertion message in the default channel (Misc).
        /// </summary>
        /// <param name="format"></param>
        /// <param name="args"></param>
        [Conditional("DEBUG_LOG")]
        public static void LogAssertionFormat(string format, params object[] args)
        {
            LogFormat(format, DefaultChannel, LogLvl.Assertion, args);
        }

        /// <summary>
        /// Log a formatted info message in the given channel.
        /// </summary>
        /// <param name="format"></param>
        /// <param name="channel"></param>
        /// <param name="args"></param>
        [Conditional("DEBUG_LOG")]
        public static void LogFormat(string format, LogChannel channel, params object[] args)
        {
            LogFormat(format, channel, LogLvl.Info, args);
        }

        /// <summary>
        /// Log a formatted warning message in the given channel.
        /// </summary>
        /// <param name="format"></param>
        /// <param name="channel"></param>
        /// <param name="args"></param>
        [Conditional("DEBUG_LOG")]
        public static void LogWarningFormat(string format, LogChannel channel, params object[] args)
        {
            LogFormat(format, channel, LogLvl.Warning, args);
        }

        /// <summary>
        /// Log a formatted error message in the given channel.
        /// </summary>
        /// <param name="format"></param>
        /// <param name="channel"></param>
        /// <param name="args"></param>
        [Conditional("DEBUG_LOG")]
        public static void LogErrorFormat(string format, LogChannel channel, params object[] args)
        {
            LogFormat(format, channel, LogLvl.Error, args);
        }

        /// <summary>
        /// Log a formatted assertion message in the given channel.
        /// </summary>
        /// <param name="format"></param>
        /// <param name="channel"></param>
        /// <param name="args"></param>
        [Conditional("DEBUG_LOG")]
        public static void LogAssertionFormat(string format, LogChannel channel, params object[] args)
        {
            LogFormat(format, channel, LogLvl.Assertion, args);
        }

        #endregion
    }
}