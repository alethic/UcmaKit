using System;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Rtc.Collaboration;
using Microsoft.Rtc.Collaboration.AudioVideo;

namespace UcmaKit.Rtc.Acd
{

    public class AcdApplication : UcmaKit.Rtc.RtcApplication
    {

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="configuration"></param>
        public AcdApplication(RtcApplicationConfigurationElement configuration)
            : base(configuration)
        {
            DefaultAction = new AcdHold()
            {
                Action = new AcdDelay(TimeSpan.FromSeconds(5)),
            };
        }

        /// <summary>
        /// Gets the default action to be used to execute calls when no specified action is specified.
        /// </summary>
        public AcdAction DefaultAction { get; set; }

        /// <summary>
        /// Gets the action to be used to excute the specified call.
        /// </summary>
        /// <param name="call"></param>
        /// <returns></returns>
        public virtual AcdAction GetAction(AudioVideoCall call)
        {
            return new AcdParallel()
            {
                new AcdContactTransfer(new AcdContact("sip:jhaltom@isillc.com")),
                new AcdContactTransfer(new AcdContact("sip:office-front@isillc.com")),
                new AcdContactTransfer(new AcdContact("tel:+112146410503")),
            };
        }

        protected override async Task HandleAudioVideoCall(CallReceivedEventArgs<AudioVideoCall> args, CancellationToken cancellationToken)
        {
            var action = GetAction(args.Call) ?? DefaultAction;
            if (action != null)
                await action.Execute(args.Call.Conversation.Endpoint, args.Call, cancellationToken);
        }

    }

}
