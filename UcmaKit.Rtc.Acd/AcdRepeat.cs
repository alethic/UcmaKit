using System;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Rtc.Collaboration;
using Microsoft.Rtc.Collaboration.AudioVideo;

namespace UcmaKit.Rtc.Acd
{

    /// <summary>
    /// Repeats the nested action indefinately.
    /// </summary>
    public class AcdRepeat : AcdDelegate
    {

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        public AcdRepeat()
            : base()
        {

        }

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="action"></param>
        public AcdRepeat(AcdAction action)
            : base(action)
        {

        }

        public override async Task<AcdActionResult> Execute(LocalEndpoint localEndpoint, AudioVideoCall call, CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    // record start time
                    var t = DateTime.UtcNow;

                    // execute delegate
                    if (await base.Execute(localEndpoint, call, cancellationToken))
                        return true;

                    // do not execute more than once a second
                    if (DateTime.UtcNow - t < TimeSpan.FromSeconds(1))
                        await Task.Delay(1000, cancellationToken);
                }
                catch (OperationCanceledException)
                {
                    // ignore
                }
            }

            return false;
        }

    }

}
