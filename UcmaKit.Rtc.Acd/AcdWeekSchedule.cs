using System;

namespace UcmaKit.Rtc.Acd
{

    /// <summary>
    /// Describes a weekly schedule.
    /// </summary>
    public class AcdWeekSchedule : AcdSchedule
    {

        AcdTimeRange[] schedule = new AcdTimeRange[7];

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        public AcdWeekSchedule()
            : this(AcdTimeRange.Day)
        {

        }

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="defaultRange"></param>
        public AcdWeekSchedule(AcdTimeRange defaultRange)
        {
            for (var i = DayOfWeek.Sunday; i <= DayOfWeek.Saturday; i++)
                this[i] = defaultRange;
        }

        /// <summary>
        /// Gets the time range covered on the given day of the week.
        /// </summary>
        /// <param name="dayOfWeek"></param>
        /// <returns></returns>
        public AcdTimeRange this[DayOfWeek dayOfWeek]
        {
            get { return schedule[(int)dayOfWeek]; }
            set { schedule[(int)dayOfWeek] = value; }
        }

        public override bool InSchedule(DateTime dateTime)
        {
            var t = this[dateTime.DayOfWeek];
            return dateTime.TimeOfDay >= t.From && dateTime.TimeOfDay < t.Thru;
        }

    }

}
