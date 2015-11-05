namespace ISI.Rtc.Acd
{

    /// <summary>
    /// Describes the result of an <see cref="AcdAction"/>'s execution.
    /// </summary>
    public enum AcdActionResultStatus
    {

        /// <summary>
        /// Indicates the action completed the call.
        /// </summary>
        Complete,

        /// <summary>
        /// Indicates the action did not complete the call and execution shold continue.
        /// </summary>
        Continue,

    }

}
