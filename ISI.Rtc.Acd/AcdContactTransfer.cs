using System;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;

using Microsoft.Rtc.Collaboration;
using Microsoft.Rtc.Collaboration.AudioVideo;
using Microsoft.Rtc.Collaboration.Presence;

namespace ISI.Rtc.Acd
{

    /// <summary>
    /// Attempts to ring the specified agent and transfers the call to them upon answer.
    /// </summary>
    public class AcdContactTransfer : AcdTransfer
    {

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="agent"></param>
        public AcdContactTransfer()
            : base()
        {

        }

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="contact"></param>
        public AcdContactTransfer(AcdContact contact)
            : base(contact)
        {

        }

        /// <summary>
        /// Gets whether or not the contacts's presense should impact whether they are contacted.
        /// </summary>
        public bool IgnorePresence { get; set; }

        /// <summary>
        /// Whether the contact is already in use.
        /// </summary>
        public bool Busy { get; private set; }

        public override async Task<AcdActionResult> Execute(LocalEndpoint localEndpoint, AudioVideoCall call, CancellationToken cancellationToken)
        {
            if (Endpoint == null)
                return AcdActionResult.Continue;

            // only allow one call through to a contact at a time
            if (Busy)
                return AcdActionResult.Continue;

            try
            {
                Busy = true;

                var available = IgnorePresence || await GetAvailableAsync(localEndpoint, cancellationToken);
                if (available)
                    return await base.Execute(localEndpoint, call, cancellationToken);
            }
            catch (OperationCanceledException)
            {
                // ignore
            }
            finally
            {
                Busy = false;
            }

            return AcdActionResult.Continue;
        }

        /// <summary>
        /// Gets whether or not the contact is available.
        /// </summary>
        /// <param name="localEndpoint"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        async Task<bool> GetAvailableAsync(LocalEndpoint localEndpoint, CancellationToken cancellationToken)
        {
            // obtain presense state
            var presence = await localEndpoint
                .GetPresenceContext()
                .GetPresenceStateAsync(Endpoint.Uri, cancellationToken);
            
            // assume present if no status at all; failsafe
            if (presence == null)
                return true;

            // currently online
            if (presence.Availability == PresenceAvailability.IdleOnline ||
                presence.Availability == PresenceAvailability.None ||
                presence.Availability == PresenceAvailability.Online)
                return true;

            // active within 30 seconds
            if (DateTime.UtcNow - presence.LastActive <= TimeSpan.FromSeconds(30))
                return true;

            return false;
        }

    }

}
