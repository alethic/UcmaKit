using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Rtc.Collaboration;
using Microsoft.Rtc.Collaboration.AudioVideo;
using Microsoft.Rtc.Signaling;

using Nito.AsyncEx;

using ISI.Rtc.Util;
using System.Net.Mime;
using System.Text;

namespace ISI.Rtc.Acd
{

    /// <summary>
    /// Attempts to establish a new call with the specified endpoint before transferring the original call.
    /// </summary>
    public class AcdTransfer : AcdAction
    {

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        public AcdTransfer()
            : base()
        {

        }

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        public AcdTransfer(AcdRemoteEndpoint endpoint)
            : base()
        {
            Endpoint = endpoint;
        }

        /// <summary>
        /// Gets or sets the uri of the endpoint.
        /// </summary>
        public AcdRemoteEndpoint Endpoint { get; set; }

        public override async Task<AcdActionResult> Execute(LocalEndpoint localEndpoint, AudioVideoCall call, CancellationToken cancellationToken)
        {
            if (Endpoint == null)
                return AcdActionResult.Continue;

            // extract information from incoming caller
            var callParticipant = call.RemoteEndpoint.Participant;
            var callAddress = new RealTimeAddress(callParticipant.Uri, localEndpoint.DefaultDomain, localEndpoint.PhoneContext);
            var callSipUri = new SipUriParser(callAddress.Uri);
            callSipUri.RemoveParameter(new SipUriParameter("user", "phone"));
            var callUri = callSipUri.ToString();
            var callPhoneUri = callParticipant.OtherPhoneUri;
            var callName = callParticipant.DisplayName;

            // impersonate incoming caller to agent
            var remoteConversation = new Conversation(localEndpoint);
            remoteConversation.Impersonate(callUri, callPhoneUri, callName);

            // establish call to endpoint
            var remoteCall = new AudioVideoCall(remoteConversation);
            var remoteOpts = new CallEstablishOptions();
            remoteOpts.Transferor = localEndpoint.OwnerUri;
            remoteOpts.Headers.Add(new SignalingHeader("Ms-Target-Class", "secondary"));

            // initiate call with duration
            var destCallT = remoteCall.EstablishAsync(Endpoint.Uri, remoteOpts, cancellationToken);

            try
            {
                // wait for agent call to complete
                await destCallT;
            }
            catch (OperationCanceledException)
            {
                // ignore
            }
            catch (RealTimeException)
            {
                // ignore
            }

            // ensure two accepted transfers cannot both complete
            using (var lck = await call.GetContext<AsyncLock>().LockAsync())
                if (call.State == CallState.Established)
                    if (remoteCall.State == CallState.Established)
                    {
                        var participant = remoteConversation.RemoteParticipants.FirstOrDefault();
                        if (participant != null)
                        {
                            var endpoint = participant.GetEndpoints().FirstOrDefault(i => i.EndpointType == EndpointType.User);
                            if (endpoint != null)
                            {
                                var ctx = new ConversationContextChannel(remoteConversation, endpoint);

                                // establish conversation context with application
                                await ctx.EstablishAsync(
                                    new Guid("FA44026B-CC48-42DA-AAA8-B849BCB43A21"), 
                                    new ConversationContextChannelEstablishOptions());

                                // send context data
                                await ctx.SendDataAsync(
                                    new ContentType("text/plain"), 
                                    Encoding.UTF8.GetBytes("Id=123"));
                            }
                        }

                        return await TransferAsync(call, remoteCall);
                    }

            // terminate outbound call if still available
            if (remoteCall.State != CallState.Terminating &&
                remoteCall.State != CallState.Terminated)
                await remoteCall.TerminateAsync();

            // we could not complete the transfer attempt
            return AcdActionResult.Continue;
        }

        /// <summary>
        /// Attempt to transfer the call to the given destination with the set transfer type.
        /// </summary>
        /// <param name="call"></param>
        /// <param name="callToReplace"></param>
        /// <param name="transferType"></param>
        /// <returns></returns>
        async Task<bool> TransferAsync(AudioVideoCall call, AudioVideoCall callToReplace)
        {
            try
            {
                // accept call if not accepted
                if (call.State != CallState.Established)
                    await call.AcceptAsync();

                // transfer to answered endpoint
                await call.TransferAsync(callToReplace, new CallTransferOptions(CallTransferType.Attended));
                return true;
            }
            catch (RealTimeException)
            {
                return false;
            }
        }

    }

}
