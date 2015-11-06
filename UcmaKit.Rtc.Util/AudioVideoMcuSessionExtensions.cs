using System.Threading.Tasks;

using Microsoft.Rtc.Collaboration;
using Microsoft.Rtc.Collaboration.AudioVideo;

namespace UcmaKit.Rtc.Util
{

    public static class AudioVideoMcuSessionExtensions
    {

        public static Task TransferAsync(this AudioVideoMcuSession self, AudioVideoCall call, McuTransferOptions mcuTransferOptions)
        {
            return Task.Factory.FromAsync<AudioVideoCall, McuTransferOptions>(
                self.BeginTransfer,
                self.EndTransfer,
                call,
                mcuTransferOptions,
                null);
        }

        public static Task EnableMuteAllModeAsync(this AudioVideoMcuSession self, EnableMuteAllModeOptions options)
        {
            return Task.Factory.FromAsync<EnableMuteAllModeOptions>(
                self.BeginEnableMuteAllMode,
                self.EndEnableMuteAllMode,
                options,
                null);
        }

        public static Task DisableMuteAllModeAsync(this AudioVideoMcuSession self)
        {
            return Task.Factory.FromAsync(
                self.BeginDisableMuteAllMode,
                self.EndDisableMuteAllMode,
                null);
        }

    }

}
