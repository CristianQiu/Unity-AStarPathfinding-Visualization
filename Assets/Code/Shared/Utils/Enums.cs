namespace Shared
{
    #region Directions

    /// <summary>
    /// The main four directions.
    /// </summary>
    public enum FourDirection
    {
        South,
        West,
        East,
        North
    }

    /// <summary>
    /// The main four directions, plus their intermediate ones.
    /// </summary>
    public enum EightDirection
    {
        SouthWest,
        South,
        SouthEast,
        West,
        East,
        NorthWest,
        North,
        NorthEast
    }

    #endregion

    #region Logging

    /// <summary>
    /// The different log levels.
    /// </summary>
    public enum LogLvl
    {
        Info,
        Warning,
        Error,
        Assertion,
        Exception,
    }

    /// <summary>
    /// The different log channels.
    /// </summary>
    public enum LogChannel
    {
        Misc,
        IO,
        GameLogic,
        AI,
        Render,

        Count
    }

    #endregion

    #region Rendering

    /// <summary>
    /// The different debug render channels.
    /// </summary>
    public enum DebugRenderChannel
    {
        GridGraph,

        Count
    }

    #endregion
}