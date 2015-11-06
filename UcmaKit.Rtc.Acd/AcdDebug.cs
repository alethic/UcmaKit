using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Rtc.Collaboration;
using Microsoft.Rtc.Collaboration.AudioVideo;

namespace UcmaKit.Rtc.Acd
{

    /// <summary>
    /// Serves as useful debug tool. Waits a random amount of time, and returns a random success value.
    /// </summary>
    public class AcdDebug : AcdAction
    {

        static readonly Random random = new Random();

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        public AcdDebug(string name)
        {
            Name = name;
        }

        public string Name { get; set; }

        public override async Task<AcdActionResult> Execute(LocalEndpoint localEndpoint, AudioVideoCall call, CancellationToken cancellationToken)
        {
            Trace.TraceInformation("Debug: {0} start", Name);
            await Task.Delay(TimeSpan.FromSeconds(random.Next(3, 30)), cancellationToken);
            var r = cancellationToken.IsCancellationRequested ? false : (random.Next(1, 100) <= 20);
            Trace.TraceInformation("Debug: {0} stop {1}", Name, r);

            return r;
        }

    }

}
