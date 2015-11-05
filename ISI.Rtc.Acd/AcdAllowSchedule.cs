using System;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Rtc.Collaboration;
using Microsoft.Rtc.Collaboration.AudioVideo;

namespace ISI.Rtc.Acd
{

    /// <summary>
    /// Restricts the call from proceeding to nested delegate unless it falls within the specified schedule.
    /// </summary>
    public class AcdAllowSchedule : AcdDelegate
    {

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        public AcdAllowSchedule()
            : base()
        {

        }

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="dispatcher"></param>
        public AcdAllowSchedule(AcdAction dispatcher)
            : base(dispatcher)
        {

        }

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="schedule"></param>
        public AcdAllowSchedule(AcdSchedule schedule)
            : base()
        {
            Schedule = schedule;
        }

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="schedule"></param>
        /// <param name="dispatcher"></param>
        public AcdAllowSchedule(AcdSchedule schedule, AcdAction dispatcher)
            : base(dispatcher)
        {
            Schedule = schedule;
        }

        /// <summary>
        /// Gets or sets the schedule to filter calls within.
        /// </summary>
        public AcdSchedule Schedule { get; set; }

        public override Task<AcdActionResult> Execute(LocalEndpoint localEndpoint, AudioVideoCall call, CancellationToken cancellationToken)
        {
            if (Schedule == null || Schedule.InSchedule(DateTime.Now))
                return base.Execute(localEndpoint, call, cancellationToken);
            else
                return AcdActionResult.ContinueTask;
        }

    }

}
