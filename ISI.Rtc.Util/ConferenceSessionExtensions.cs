using System.Threading.Tasks;

using Microsoft.Rtc.Collaboration;

namespace ISI.Rtc.Util
{

    public static class ConferenceSessionExtensions
    {

        public static Task JoinAsync(this ConferenceSession self, ConferenceJoinOptions options)
        {
            return Task.Factory.FromAsync<ConferenceJoinOptions>(
                self.BeginJoin,
                self.EndJoin,
                options,
                null);
        }

        public static Task JoinAsync(this ConferenceSession self, string conferenceUri, ConferenceJoinOptions options)
        {
            return Task.Factory.FromAsync<string, ConferenceJoinOptions>(
                self.BeginJoin,
                self.EndJoin,
                conferenceUri,
                options,
                null);
        }

    }

}
