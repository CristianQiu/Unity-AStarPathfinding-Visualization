using System.Collections.Generic;
using System.Diagnostics;

namespace Shared
{
    /// <summary>
    /// Class to hold the different debug render channels that can be activated and deactivated to show or hide the debugging.
    /// </summary>
    public static class DebugRenderChannels
    {
        #region Private Attributes

        private static readonly List<bool> ChannelsActiveState = new List<bool>((int) DebugRenderChannel.Count);

        #endregion

        #region Initialization Methods

        /// <summary>
        /// Constructor.
        /// </summary>
        static DebugRenderChannels()
        {
            // register all the channels enabled by default
            for (int i = 0; i < (int) DebugRenderChannel.Count; i++)
                ChannelsActiveState.Add(true);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Get whether the given channel is active or not.
        /// </summary>
        /// <param name="channel"></param>
        /// <returns></returns>
        public static bool IsChannelActive(DebugRenderChannel channel)
        {
            return ChannelsActiveState[(int) channel];
        }

        /// <summary>
        /// Set the active state of the given render channel.
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="on"></param>
        [Conditional("DEBUG_RENDER")]
        public static void SetChannelEnabledState(DebugRenderChannel channel, bool on)
        {
            ChannelsActiveState[(int) channel] = on;
        }

        #endregion
    }
}