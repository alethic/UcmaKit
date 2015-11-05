using System;
using System.Linq;
using System.Threading.Tasks;
using ISI.Rtc.Util;
using Microsoft.Rtc.Collaboration;
using Microsoft.Rtc.Collaboration.AudioVideo;
using Microsoft.Rtc.Collaboration.ConferenceManagement;
using Microsoft.Rtc.Signaling;
using Nito.AsyncEx;

namespace ISI.Rtc.Queue
{

    public class QueueHoldConference
    {

        readonly AsyncLock sync;
        readonly LocalEndpoint endpoint;
        readonly string holdUri;

        string conferenceId;
        Conference conference;
        Conversation trustedConversation;
        AudioVideoCall trustedCall;
        string holdAgentUri;

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="endpoint"></param>
        /// <param name="holdUri"></param>
        /// <param name="conferenceId"></param>
        public QueueHoldConference(LocalEndpoint endpoint, string holdUri, string conferenceId = null)
        {
            this.sync = new AsyncLock();
            this.endpoint = endpoint;
            this.holdUri = holdUri;
            this.conferenceId = conferenceId;
        }

        /// <summary>
        /// Gets the <see cref="Conference"/> with the specified ID.
        /// </summary>
        /// <param name="conferenceId"></param>
        /// <returns></returns>
        async Task<Conference> GetConference(string conferenceId)
        {
            return await endpoint.ConferenceServices.GetConferenceAsync(conferenceId);
        }

        /// <summary>
        /// Creates a new <see cref="Conference"/>.
        /// </summary>
        /// <returns></returns>
        async Task<Conference> CreateConference()
        {
            return await endpoint.ConferenceServices.ScheduleConferenceAsync(new ConferenceScheduleInformation()
            {
                AccessLevel = ConferenceAccessLevel.Everyone,
                ExpiryTime = DateTime.Now.AddHours(4),
            });
        }

        /// <summary>
        /// Gets the existing <see cref="Conference"/> with the specified ID or creates a new one.
        /// </summary>
        /// <param name="conferenceId"></param>
        /// <returns></returns>
        async Task<Conference> GetOrCreateConference()
        {
            // obtain existing or new conference
            conference = conferenceId != null ? await GetConference(conferenceId) : await CreateConference();
            conferenceId = conference.ConferenceId;
            return conference;
        }

        /// <summary>
        /// Checks whether we are joined to the conference as a trusted participant.
        /// </summary>
        /// <returns></returns>
        async Task EnsureTrustedParticipant()
        {
            if (trustedConversation == null ||
                trustedConversation.ConferenceSession.ConferenceUri != conference.ConferenceUri)
            {
                // join conference as trusted participant
                trustedConversation = new Conversation(endpoint);
                await trustedConversation.ConferenceSession.JoinAsync(conference.ConferenceUri, new ConferenceJoinOptions() { JoinMode = JoinMode.TrustedParticipant });
                trustedCall = new AudioVideoCall(trustedConversation);
                await trustedCall.EstablishAsync();
            }
        }

        /// <summary>
        /// Checks for the hold participant.
        /// </summary>
        /// <returns></returns>
        async Task EnsureHoldParticipant()
        {
            // update conference data
            await GetOrCreateConference();

            // check for existing hold participant
            var holdParticipant = conference.Participants
                .Where(i => i.Uri == holdUri)
                .FirstOrDefault();
            if (holdParticipant == null)
                await InviteHoldParticipant();
        }

        /// <summary>
        /// Issues an invitation for the hold agent
        /// </summary>
        /// <returns></returns>
        async Task InviteHoldParticipant()
        {
            // invite the hold music service to the conference
            var holdInvitation = new ConferenceInvitation(trustedConversation);
            var holdResponse = await holdInvitation.DeliverAsync(holdUri);
        }

        /// <summary>
        /// Initializes the conference session and the hold music.
        /// </summary>
        /// <returns></returns>
        async Task Initialize()
        {
            await GetOrCreateConference();
            await EnsureTrustedParticipant();
            await EnsureHoldParticipant();
        }

        public async Task<RealTimeAddress> GetConferenceUri()
        {
            if (conference == null)
                using (var lck = await sync.LockAsync())
                    if (conference == null)
                        await Initialize();

            return new RealTimeAddress(conference.ConferenceUri);
        }

    }

}
