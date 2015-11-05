using System;

namespace ISI.Rtc.Acd
{

    /// <summary>
    /// Describes a range of time.
    /// </summary>
    public struct AcdTimeRange
    {

        /// <summary>
        /// Covers a full day.
        /// </summary>
        public static AcdTimeRange Day = new AcdTimeRange(
            new TimeSpan(0, 0, 0),
            new TimeSpan(24, 0, 0));

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="thru"></param>
        public AcdTimeRange(TimeSpan from, TimeSpan thru)
            : this()
        {
            From = from;
            Thru = thru;
        }

        /// <summary>
        /// Starting time.
        /// </summary>
        public TimeSpan From { get; set; }

        /// <summary>
        /// Ending time.
        /// </summary>
        public TimeSpan Thru { get; set; }

    }

}
