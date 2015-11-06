using System;
using System.Collections.Concurrent;
using Microsoft.Rtc.Collaboration;
using Microsoft.Rtc.Collaboration.AudioVideo;

namespace UcmaKit.Rtc.Queue
{

    public class QueueApplication :
        UcmaKit.Rtc.RtcApplication
    {

        readonly Lazy<QueueHoldConference> conference;
        readonly ConcurrentBag<QueueCall> calls;

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="configuration"></param>
        public QueueApplication(RtcApplicationConfigurationElement configuration)
            : base(configuration)
        {
            this.conference = new Lazy<QueueHoldConference>(() => new QueueHoldConference(Endpoints[0], "tel:+19726388630"));
            this.calls = new ConcurrentBag<QueueCall>();
        }

        protected override async void OnIncomingAudioVideoCall(object sender, CallReceivedEventArgs<AudioVideoCall> args)
        {
            // generate new call and move to the queue
            var call = new QueueCall(this, args.Call);
            await call.Queue();
            calls.Add(call);
        }

        public QueueHoldConference Conference
        {
            get { return conference.Value; }
        }

    }

}
