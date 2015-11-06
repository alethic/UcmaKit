using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Rtc.Collaboration;
using Microsoft.Rtc.Signaling;

namespace UcmaKit.Rtc.Util
{

    public static class CallExtensions
    {

        public static Task<CallMessageData> AcceptAsync(this Call self)
        {
            return Task.Factory.FromAsync<CallMessageData>(
                self.BeginAccept,
                self.EndAccept,
                null);
        }

        public static Task<CallMessageData> AcceptAsync(this Call self, CallAcceptOptions options)
        {
            return Task.Factory.FromAsync<CallAcceptOptions, CallMessageData>(
                self.BeginAccept,
                self.EndAccept,
                options,
                null);
        }

        public static Task<CallMessageData> EstablishAsync(this Call self)
        {
            return Task.Factory.FromAsync<CallMessageData>(
                self.BeginEstablish,
                self.EndEstablish,
                null);
        }

        public static Task<CallMessageData> EstablishAsync(this Call self, CallEstablishOptions options)
        {
            return Task.Factory.FromAsync<CallEstablishOptions, CallMessageData>(
                self.BeginEstablish,
                self.EndEstablish,
                options,
                null);
        }

        public static Task<CallMessageData> EstablishAsync(this Call self, string destinationUri, CallEstablishOptions options)
        {
            return Task.Factory.FromAsync<string, CallEstablishOptions, CallMessageData>(
                self.BeginEstablish,
                self.EndEstablish,
                destinationUri,
                options,
                null);
        }

        public static Task<CallMessageData> EstablishAsync(this Call self, string destinationUri, CallEstablishOptions options, CancellationToken cancellationToken)
        {
            return EstablishAsync(self, destinationUri, options)
                .WithCancellation(cancellationToken, () =>
                    self.TerminateAsync());
        }

        public static Task TerminateAsync(this Call self)
        {
            return Task.Factory.FromAsync(
                self.BeginTerminate,
                self.EndTerminate,
                null);
        }

        public static Task TerminateAsync(this Call self, CallTerminateOptions options)
        {
            return Task.Factory.FromAsync(
                self.BeginTerminate,
                self.EndTerminate,
                options,
                null);
        }

        public static Task TerminateAsync(this Call self, IEnumerable<SignalingHeader> signalingHeaders)
        {
            return Task.Factory.FromAsync(
                self.BeginTerminate,
                self.EndTerminate,
                signalingHeaders,
                null);
        }

    }

}
