using System.Threading.Tasks;

using Microsoft.Rtc.Collaboration;

namespace UcmaKit.Rtc.Util
{

    public static class CollaborationPlatformExtensions
    {

        public static Task StartupAsync(this CollaborationPlatform self)
        {
            return Task.Factory.FromAsync(
                self.BeginStartup,
                self.EndStartup,
                null);
        }

        public static Task ShutdownAsync(this CollaborationPlatform self)
        {
            return Task.Factory.FromAsync(
                self.BeginShutdown,
                self.EndShutdown,
                null);
        }

    }

}
