using System.Threading.Tasks;

using Microsoft.Rtc.Collaboration;
using Microsoft.Rtc.Collaboration.AudioVideo;

namespace ISI.Rtc.Util
{

    public static class AudioVideoCallExtensions
    {

        public static Task<CallMessageData> TransferAsync(this AudioVideoCall self, Call callToReplace)
        {
            return Task.Factory.FromAsync<Call, CallMessageData>(
                self.BeginTransfer,
                self.EndTransfer,
                callToReplace,
                null);
        }

        public static Task<CallMessageData> TransferAsync(this AudioVideoCall self, string targetUri)
        {
            return Task.Factory.FromAsync<string, CallMessageData>(
                self.BeginTransfer,
                self.EndTransfer,
                targetUri,
                null);
        }

        public static Task<CallMessageData> TransferAsync(this AudioVideoCall self, Call callToReplace, CallTransferOptions callTransferOptions)
        {
            return Task.Factory.FromAsync<Call, CallTransferOptions, CallMessageData>(
                self.BeginTransfer,
                self.EndTransfer,
                callToReplace,
                callTransferOptions,
                null);
        }

        public static Task<CallMessageData> TransferAsync(this AudioVideoCall self, string targetUri, CallTransferOptions callTransferOptions)
        {
            return Task.Factory.FromAsync<string, CallTransferOptions, CallMessageData>(
                self.BeginTransfer,
                self.EndTransfer,
                targetUri,
                callTransferOptions,
                null);
        }

    }

}
