using System;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Rtc.Collaboration;
using Microsoft.Rtc.Collaboration.AudioVideo;

namespace UcmaKit.Rtc.Acd
{

    /// <summary>
    /// Handles a call by waiting for a duration. Never actually results in a completion.
    /// </summary>
    public class AcdDelay : AcdAction
    {

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="duration"></param>
        public AcdDelay(TimeSpan duration)
        {
            Duration = duration;
        }

        /// <summary>
        /// Amount of time to delay.
        /// </summary>
        public TimeSpan Duration { get; set; }

        public override async Task<AcdActionResult> Execute(LocalEndpoint localEndpoint, AudioVideoCall call, CancellationToken cancellationToken)
        {
            try
            {
                await Task.Delay(Duration, cancellationToken);
            }
            catch (OperationCanceledException)
            {
                // ignore
            }

            return AcdActionResult.Continue;
        }

    }

}
