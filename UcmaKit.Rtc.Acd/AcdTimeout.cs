using System;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Rtc.Collaboration;
using Microsoft.Rtc.Collaboration.AudioVideo;

namespace UcmaKit.Rtc.Acd
{

    /// <summary>
    /// Dispatches to the nested dispatcher, but cancels after a certain timeout.
    /// </summary>
    public class AcdTimeout : AcdDelegate
    {

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        public AcdTimeout()
            : this(null, TimeSpan.Zero)
        {

        }

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        public AcdTimeout(AcdAction dispatcher)
            : this(dispatcher, TimeSpan.Zero)
        {

        }

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        public AcdTimeout(AcdAction dispatcher, TimeSpan length)
            : base(dispatcher)
        {
            Length = length;
        }

        /// <summary>
        /// Gets or sets the length to allow a call to wait before timing it out.
        /// </summary>
        public TimeSpan Length { get; set; }

        public override async Task<AcdActionResult> Execute(LocalEndpoint localEndpoint, AudioVideoCall call, CancellationToken cancellationToken)
        {
            // if we have a timeout, combine with existing cancellation token
            if (Length.TotalMilliseconds > 0)
                cancellationToken = CancellationTokenSource.CreateLinkedTokenSource(
                    cancellationToken,
                    new CancellationTokenSource(Length).Token)
                    .Token;

            // send to base dispatcher
            try
            {
                return await base.Execute(localEndpoint, call, cancellationToken);
            }
            catch (OperationCanceledException)
            {
                // ignore
            }

            return false;
        }

    }

}
