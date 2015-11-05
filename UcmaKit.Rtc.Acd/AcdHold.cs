using System;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Rtc.Collaboration;
using Microsoft.Rtc.Collaboration.AudioVideo;

namespace ISI.Rtc.Acd
{

    /// <summary>
    /// Puts the call on hold for the duration of the operation, after a specified delay. Being placed on hold requires
    /// accepting the call. Set a delay to avoid accepting the call unless neccessary.
    /// </summary>
    public class AcdHold : AcdDelegate
    {

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        public AcdHold()
            : this(null)
        {

        }

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="action"></param>
        public AcdHold(AcdAction action)
            : this(action, AcdHoldAudio.Ring)
        {

        }

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="dispatcher"></param>
        public AcdHold(AcdAction dispatcher, AcdHoldAudio audio)
            : base(dispatcher)
        {
            Audio = audio;
        }

        /// <summary>
        /// Gets the delay from when the call enters before we put it on hold.
        /// </summary>
        public TimeSpan Delay { get; set; }

        /// <summary>
        /// Gets or sets the audio choice.
        /// </summary>
        public AcdHoldAudio Audio { get; set; }

        public override async Task<AcdActionResult> Execute(LocalEndpoint localEndpoint, AudioVideoCall call, CancellationToken cancellationToken)
        {
            using (var hold = new AcdHoldContext(Audio))
            {
                // initiate delayed hold operation
                var s = new CancellationTokenSource();
                var h = Hold(hold, call, s.Token);

                // invoke next action
                var r = await base.Execute(localEndpoint, call, cancellationToken);

                // wait for delayed hold operation to exit
                s.Cancel();
                await h;

                return r;
            }
        }

        /// <summary>
        /// Delays before putting the call on hold.
        /// </summary>
        /// <param name="hold"></param>
        /// <param name="call"></param>
        /// <returns></returns>
        async Task Hold(AcdHoldContext hold, AudioVideoCall call, CancellationToken cancellationToken)
        {
            try
            {
                await Task.Delay(Delay, cancellationToken);
                await hold.HoldAsync(call);
            }
            catch (OperationCanceledException)
            {
                // ignore
            }
        }

    }

}
