using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Rtc.Collaboration;
using Microsoft.Rtc.Collaboration.AudioVideo;

namespace ISI.Rtc.Acd
{

    /// <summary>
    /// Dispatches the call to the nested actions in parallel.
    /// </summary>
    public class AcdParallel : AcdMultiDelegate
    {

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        public AcdParallel()
            : base()
        {

        }

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="delegates"></param>
        public AcdParallel(IEnumerable<AcdAction> delegates)
            : base(delegates)
        {

        }

        public override async Task<AcdActionResult> Execute(LocalEndpoint localEndpoint, AudioVideoCall call, CancellationToken cancellationToken)
        {
            var cs = new CancellationTokenSource();
            var ct = CancellationTokenSource.CreateLinkedTokenSource(cs.Token, cancellationToken).Token;

            // initiate agent calls
            var ops = this
                .Select(async i =>
                {
                    try
                    {
                        // we were canceled
                        if (ct.IsCancellationRequested)
                            return AcdActionResult.Continue;

                        // initiate delegate, cancel the rest on success
                        var result = await i.Execute(localEndpoint, call, ct);
                        if (result)
                            cs.Cancel();

                        return result;
                    }
                    catch (OperationCanceledException)
                    {
                        // ignore
                    }

                    return AcdActionResult.Continue;
                })
                .ToArray();

            // did any complete the call
            return (await Task.WhenAll(ops))
                .Any(i => i);
        }

    }

}
