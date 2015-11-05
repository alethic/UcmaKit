using System.Threading.Tasks;

using Microsoft.Rtc.Collaboration;

namespace ISI.Rtc.Util
{

    public static class BackToBackCallExtensions
    {

        public static Task EstablishAsync(this BackToBackCall self)
        {
            return Task.Factory.FromAsync(
                self.BeginEstablish,
                self.EndEstablish,
                null);
        }

        public static Task TerminateAsync(this BackToBackCall self)
        {
            return Task.Factory.FromAsync(
                self.BeginTerminate,
                self.EndTerminate,
                null);
        }

    }

}
