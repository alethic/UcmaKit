using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.Rtc.Collaboration.AudioVideo;
using Microsoft.Rtc.Signaling;

namespace UcmaKit.Rtc.Util
{

    public static class AudioVideoFlowExtensions
    {

        public static Task HoldAsync(this AudioVideoFlow self, HoldType holdType)
        {
            return Task.Factory.FromAsync<HoldType>(
                self.BeginHold,
                self.EndHold,
                holdType,
                null);
        }

        public static Task HoldAsync(this AudioVideoFlow self, HoldType holdType, IEnumerable<SignalingHeader> headers)
        {
            return Task.Factory.FromAsync<HoldType, IEnumerable<SignalingHeader>>(
                self.BeginHold,
                self.EndHold,
                holdType,
                headers,
                null);
        }

        public static Task HoldAsync(this AudioVideoFlow self, HoldType holdType, params SignalingHeader[] headers)
        {
            return Task.Factory.FromAsync<HoldType, IEnumerable<SignalingHeader>>(
                self.BeginHold,
                self.EndHold,
                holdType,
                headers,
                null);
        }

        public static Task RetrieveAsync(this AudioVideoFlow self)
        {
            return Task.Factory.FromAsync(
                self.BeginRetrieve,
                self.EndRetrieve,
                null);
        }

        public static Task RetrieveAsync(this AudioVideoFlow self, IEnumerable<SignalingHeader> headers)
        {
            return Task.Factory.FromAsync<IEnumerable<SignalingHeader>>(
                self.BeginRetrieve,
                self.EndRetrieve,
                headers,
                null);
        }

        public static Task RetrieveAsync(this AudioVideoFlow self, params SignalingHeader[] headers)
        {
            return Task.Factory.FromAsync<IEnumerable<SignalingHeader>>(
                self.BeginRetrieve,
                self.EndRetrieve,
                headers,
                null);
        }

    }

}
