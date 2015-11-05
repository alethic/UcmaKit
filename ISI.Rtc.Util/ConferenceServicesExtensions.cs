using System.Threading.Tasks;

using Microsoft.Rtc.Collaboration;
using Microsoft.Rtc.Collaboration.ConferenceManagement;

namespace ISI.Rtc.Util
{

    public static class ConferenceServicesExtensions
    {

        public static Task<Conference> GetConferenceAsync(this ConferenceServices self, string conferenceId)
        {
            return Task.Factory.FromAsync<string, Conference>(
                self.BeginGetConference,
                self.EndGetConference,
                conferenceId,
                null);
        }

        public static Task<Conference> ScheduleConferenceAsync(this ConferenceServices self, ConferenceScheduleInformation information)
        {
            return Task.Factory.FromAsync<ConferenceScheduleInformation, Conference>(
                self.BeginScheduleConference,
                self.EndScheduleConference,
                information,
                null);
        }

    }

}
