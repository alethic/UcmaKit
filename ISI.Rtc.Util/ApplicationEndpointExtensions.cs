using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.Rtc.Collaboration;
using Microsoft.Rtc.Signaling;

namespace ISI.Rtc.Util
{

    public static class ApplicationEndpointExtensions
    {

        public static Task<SipResponseData> EstablishAsync(this ApplicationEndpoint self)
        {
            return Task.Factory.FromAsync<SipResponseData>(
                self.BeginEstablish,
                self.EndEstablish,
                null);
        }

        public static Task<SipResponseData> EstablishAsync(this ApplicationEndpoint self, IEnumerable<SignalingHeader> additionalHeaders)
        {
            return Task.Factory.FromAsync<IEnumerable<SignalingHeader>, SipResponseData>(
                self.BeginEstablish,
                self.EndEstablish,
                additionalHeaders,
                null);
        }

        public static Task TerminateAsync(this ApplicationEndpoint self)
        {
            return Task.Factory.FromAsync(
                self.BeginTerminate,
                self.EndTerminate,
                null);
        }

        public static Task DrainAsync(this ApplicationEndpoint self)
        {
            return Task.Factory.FromAsync(
                self.BeginDrain,
                self.EndDrain,
                null);
        }

    }

}
