using System.Threading.Tasks;

using ISI.Rtc.Util;

using Microsoft.Rtc.Collaboration;
using Microsoft.Rtc.Collaboration.AudioVideo;
using Microsoft.Rtc.Signaling;
using Nito.AsyncEx;

namespace ISI.Rtc.Queue
{

    public class QueueAgentConference
    {

        readonly AsyncLock sync;
        readonly LocalEndpoint endpoint;

        Conversation trustedConversation;
        AudioVideoCall trustedCall;

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="endpoint"></param>
        public QueueAgentConference(LocalEndpoint endpoint)
        {
            this.sync = new AsyncLock();
            this.endpoint = endpoint;
        }

        /// <summary>
        /// Initializes the conference session and the hold music.
        /// </summary>
        /// <returns></returns>
        async Task Initialize()
        {
            // generate new conference and trusted participant
            trustedConversation = new Conversation(endpoint);
            await trustedConversation.ConferenceSession.JoinAsync(new ConferenceJoinOptions() { JoinMode = JoinMode.TrustedParticipant });
        }

        public async Task<RealTimeAddress> GetConferenceUri()
        {
            if (trustedConversation == null)
                using (var lck = await sync.LockAsync())
                    if (trustedConversation == null)
                        await Initialize();

            return trustedConversation.ConferenceSession;
        }

    }

}
