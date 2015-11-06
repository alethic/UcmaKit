using System;

namespace UcmaKit.Rtc.Acd
{

    /// <summary>
    /// Base type for classes representing a schedule that a date and time can fall within.
    /// </summary>
    public abstract class AcdSchedule
    {

        /// <summary>
        /// Returns <c>true</c> if the specified date time is within the schedule.
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public abstract bool InSchedule(DateTime dateTime);

    }

}
