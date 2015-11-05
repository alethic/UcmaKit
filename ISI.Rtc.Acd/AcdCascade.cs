using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Rtc.Collaboration;
using Microsoft.Rtc.Collaboration.AudioVideo;

using ISI.Rtc.Util;

namespace ISI.Rtc.Acd
{

    /// <summary>
    /// Invokes each level in parallel, one after the other, after a delay. If the level proceeding a level with a
    /// delay finishes before the delay, the delay is exited early.
    /// </summary>
    public class AcdCascade : AcdAction, IEnumerable<AcdCascadeLevel>
    {

        IEnumerable<AcdCascadeLevel> levels;

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        public AcdCascade()
            : this(Enumerable.Empty<AcdCascadeLevel>())
        {

        }

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="levels"></param>
        public AcdCascade(IEnumerable<AcdCascadeLevel> levels)
        {
            this.levels = levels;
        }

        /// <summary>
        /// Adds the given level.
        /// </summary>
        /// <param name="level"></param>
        public void Add(AcdCascadeLevel level)
        {
            levels = levels.Append(level);
        }

        /// <summary>
        /// Adds the given level.
        /// </summary>
        /// <param name="level"></param>
        public void Add(TimeSpan delay, AcdAction dispatcher)
        {
            Add(new AcdCascadeLevel(delay, dispatcher));
        }

        public override async Task<AcdActionResult> Execute(LocalEndpoint localEndpoint, AudioVideoCall call, CancellationToken cancellationToken)
        {
            // collect executing items
            var cs = new CancellationTokenSource();
            var ct = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, cs.Token).Token;

            // each level of the cascade waiting either on it's delay or the previous cascade level
            var tl = levels
                .SelectWithPreviousResult<AcdCascadeLevel, Task<AcdActionResult>>(async (i, j) =>
                {
                    try
                    {
                        // is previous level, wait for it and delay; else just delay
                        var d = Task.Delay(i.Delay, ct);
                        var t = j != null ? Task.WhenAny(d, j) : d;
                        await t;

                        // somebody else won
                        if (ct.IsCancellationRequested)
                            return AcdActionResult.Continue;

                        var result = await i.Action.Execute(localEndpoint, call, ct);
                        if (result)
                            cs.Cancel();

                        return result;
                    }
                    catch (OperationCanceledException)
                    {
                        // ignore
                    }

                    return AcdActionResult.Continue;
                });

            // wait for all tasks to complete and return whether anybody succeeded
            return (await Task.WhenAll(tl))
                .Any(i => i);
        }

        public IEnumerator<AcdCascadeLevel> GetEnumerator()
        {
            return levels.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

    }

}
