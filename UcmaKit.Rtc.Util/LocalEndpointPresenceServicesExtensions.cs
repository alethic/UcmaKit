using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.Rtc.Collaboration;
using Microsoft.Rtc.Collaboration.Presence;

namespace UcmaKit.Rtc.Util
{

    public static class LocalEndpointPresenceServicesExtensions
    {

        public static Task<IEnumerable<RemotePresentityNotification>> PresenceQueryAsync(this LocalEndpointPresenceServices self, IEnumerable<string> targets, string[] categories)
        {
            return Task.Factory.FromAsync<IEnumerable<string>, string[], EventHandler<RemotePresentitiesNotificationEventArgs>, IEnumerable<RemotePresentityNotification>>(
                self.BeginPresenceQuery,
                self.EndPresenceQuery,
                targets,
                categories,
                null,
                null);
        }

    }

}
