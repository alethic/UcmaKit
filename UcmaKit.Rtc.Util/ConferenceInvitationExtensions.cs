using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Rtc.Collaboration;

namespace UcmaKit.Rtc.Util
{

    public static class ConferenceInvitationExtensions
    {

        public static Task AcceptAsync(this ConferenceInvitation self, ConferenceInvitationAcceptOptions options)
        {
            return Task.Factory.FromAsync<ConferenceInvitationAcceptOptions>(
                self.BeginAccept,
                self.EndAccept,
                options,
                null);
        }

        public static Task AcceptAsync(this ConferenceInvitation self)
        {
            return Task.Factory.FromAsync(
                self.BeginAccept,
                self.EndAccept,
                null);
        }

        public static Task<ConferenceInvitationResponse> DeliverAsync(this ConferenceInvitation self, string destinationUri, ConferenceInvitationDeliverOptions options)
        {
            return Task.Factory.FromAsync<string, ConferenceInvitationDeliverOptions, ConferenceInvitationResponse>(
                self.BeginDeliver,
                self.EndDeliver,
                destinationUri,
                options,
                null);
        }

        public static Task<ConferenceInvitationResponse> DeliverAsync(this ConferenceInvitation self, string destinationUri)
        {
            return Task.Factory.FromAsync<string, ConferenceInvitationResponse>(
                self.BeginDeliver,
                self.EndDeliver,
                destinationUri,
                null);
        }

    }

}
