using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Rtc.Collaboration;
using Microsoft.Rtc.Collaboration.AudioVideo;

namespace UcmaKit.Rtc.Acd
{

    /// <summary>
    /// Dispatches the call to the nested delegates in order.
    /// </summary>
    public class AcdSerial : AcdMultiDelegate
    {

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        public AcdSerial()
            : base()
        {

        }

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="delegates"></param>
        public AcdSerial(IEnumerable<AcdAction> delegates)
            : base(delegates)
        {

        }

        public override async Task<AcdActionResult> Execute(LocalEndpoint localEndpoint, AudioVideoCall call, CancellationToken cancellationToken)
        {
            foreach (var dispatcher in this)
            {
                if (cancellationToken.IsCancellationRequested)
                    return AcdActionResult.Continue;

                try
                {
                    if (await dispatcher.Execute(localEndpoint, call, cancellationToken))
                        return AcdActionResult.Complete;
                }
                catch (OperationCanceledException)
                {
                    // ignore
                }
            }

            return AcdActionResult.Continue;
        }

    }

}
