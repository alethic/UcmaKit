using System;
using System.Threading.Tasks;

using UcmaKit.Rtc.Util;

using Microsoft.Rtc.Collaboration;
using Microsoft.Rtc.Collaboration.AudioVideo;

namespace UcmaKit.Rtc.Queue
{

    public class QueueCall
    {

        readonly Guid id;
        readonly QueueApplication application;
        readonly AudioVideoCall call;
        readonly string uri;
        readonly string phoneUri;
        readonly string displayName;

        internal AudioVideoCall b2bCall;
        internal BackToBackCall b2b;

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="application"></param>
        /// <param name="call"></param>
        public QueueCall(QueueApplication application, AudioVideoCall call)
        {
            this.id = Guid.NewGuid();
            this.application = application;
            this.call = call;

            // incoming call identity
            this.uri = "sip:" + id.ToString("N") + "@" + call.Conversation.Endpoint.DefaultDomain;
            this.phoneUri = call.RemoteEndpoint.Participant.PhoneUri.TrimToNull() ?? call.RemoteEndpoint.Participant.OtherPhoneUri.TrimToNull();
            this.displayName = call.RemoteEndpoint.Participant.DisplayName.TrimToNull();

            this.call.StateChanged += call_StateChanged;
        }

        void call_StateChanged(object sender, CallStateChangedEventArgs args)
        {
            if (args.State == CallState.Terminated)
            {
                // call removed
            }
        }

        /// <summary>
        /// Disconnects the call from the existing BackToBack if present.
        /// </summary>
        /// <returns></returns>
        async Task DisconnectB2B()
        {
            if (b2bCall != null)
            {
                // wait for termination of b2b leg
                await b2bCall.TerminateAsync();

                // clear existing b2b call
                b2bCall = null;
                b2b = null;
            }
        }

        /// <summary>
        /// Places the call in the call queue.
        /// </summary>
        /// <returns></returns>
        public async Task Queue()
        {
            await DisconnectB2B();

            // obtain queue URI
            var queueAddress = await application.Conference.GetConferenceUri();

            // second leg of call to conference
            b2bCall = new AudioVideoCall(new Conversation(call.Conversation.Endpoint));
            b2bCall.Conversation.Impersonate(uri, phoneUri, displayName);
            await b2bCall.Conversation.ConferenceSession.JoinAsync(queueAddress.Uri, null);

            // join first and second leg
            var leg1B2B = new BackToBackCallSettings(call);
            var leg2B2B = new BackToBackCallSettings(b2bCall);

            // establish back to back call
            b2b = new BackToBackCall(leg1B2B, leg2B2B);
            await b2b.EstablishAsync();
        }

    }

}
