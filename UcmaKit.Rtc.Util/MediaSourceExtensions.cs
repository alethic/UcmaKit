using System.Threading.Tasks;

using Microsoft.Rtc.Collaboration.AudioVideo;

namespace UcmaKit.Rtc.Util
{

    public static class MediaSourceExtensions
    {

        public static Task PrepareSourceAsync(this MediaSource self, MediaSourceOpenMode mode)
        {
            return Task.Factory.FromAsync(
                self.BeginPrepareSource,
                self.EndPrepareSource,
                mode,
                null);
        }

    }

}
