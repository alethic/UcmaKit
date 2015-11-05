using System;

namespace ISI.Rtc.Acd
{

    /// <summary>
    /// Describes a level in the cascade.
    /// </summary>
    public class AcdCascadeLevel
    {

        /// <summary>
        /// Initializes a new instance
        /// </summary>
        public AcdCascadeLevel()
            : this(TimeSpan.Zero)
        {

        }

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="delay"></param>
        public AcdCascadeLevel(TimeSpan delay)
            : this(delay, null)
        {

        }

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="action"></param>
        public AcdCascadeLevel(AcdAction action)
            : this(TimeSpan.Zero, action)
        {

        }

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="delay"></param>
        /// <param name="action"></param>
        public AcdCascadeLevel(AcdAction action, TimeSpan delay)
            : this(delay, action)
        {

        }

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="delay"></param>
        /// <param name="action"></param>
        public AcdCascadeLevel(TimeSpan delay, AcdAction action)
        {
            Delay = delay;
            Action = action;
        }

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="delay">Delay in milliseconds.</param>
        /// <param name="action"></param>
        public AcdCascadeLevel(int delay, AcdAction action)
            : this(TimeSpan.FromMilliseconds(delay), action)
        {

        }

        /// <summary>
        /// Delay from start of cascade before delegating to the action.
        /// </summary>
        public TimeSpan Delay { get; set; }

        /// <summary>
        /// Action to invoke.
        /// </summary>
        public AcdAction Action { get; set; }

    }

}
