using System.Collections.Generic;
using System.Diagnostics;

namespace Shared
{
    /// <summary>
    /// Class to hold the different log channels that can be activated and deactivated to show or
    /// hide the debugging.
    /// </summary>
    public static class LogChannels
    {
        #region Private Attributes

        private static readonly List<string> Channels = new List<string>((int)LogChannel.Count);
        private static readonly List<bool> ChannelsActiveState = new List<bool>((int)LogChannel.Count);

        #endregion

        #region Initialization Methods

        /// <summary>
        /// Constructor.
        /// </summary>
        static LogChannels()
        {
            // initialize the log channels strings
            for (int i = 0; i < (int)LogChannel.Count; ++i)
                Channels.Add(((LogChannel)i).ToString());

            // they are active by default
            for (int i = 0; i < (int)LogChannel.Count; ++i)
                ChannelsActiveState.Add(true);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Get the given cached channel string name.
        /// </summary>
        /// <param name="channel"></param>
        /// <returns></returns>
        public static string GetChannelName(LogChannel channel)
        {
            return LogChannels.Channels[(int)channel];
        }

        /// <summary>
        /// Get whether the given channel is active or not.
        /// </summary>
        /// <param name="channel"></param>
        /// <returns></returns>
        public static bool IsChannelActive(LogChannel channel)
        {
            return ChannelsActiveState[(int)channel];
        }

        /// <summary>
        /// Set the active state of the given log channel.
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="on"></param>
        [Conditional("DEBUG_LOG")]
        public static void SetChannelEnabledState(LogChannel channel, bool on)
        {
            ChannelsActiveState[(int)channel] = on;
        }

        #endregion
    }
}