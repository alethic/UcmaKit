using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Rtc.Collaboration;
using Microsoft.Rtc.Collaboration.Presence;

using Nito.AsyncEx;

using ISI.Rtc.Util;

namespace ISI.Rtc
{

    /// <summary>
    /// Provides acccess to cached presence information for a local endpoint.
    /// </summary>
    public class RtcPresenceContext
    {

        Dictionary<string, PresenceState> cache =
            new Dictionary<string, PresenceState>();
        AsyncManualResetEvent cacheWh =
            new AsyncManualResetEvent(false);

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="endpoint"></param>
        internal RtcPresenceContext(LocalEndpoint endpoint)
        {
            Endpoint = endpoint;
        }

        /// <summary>
        /// Gets the endpoint that is providing presence information.
        /// </summary>
        public LocalEndpoint Endpoint { get; private set; }

        /// <summary>
        /// Returns <c>true</c> if the given endpoint is available.
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<PresenceState> GetPresenceStateAsync(string uri, CancellationToken cancellationToken)
        {
            // cache contains our data
            lock (cache)
                if (cache.ContainsKey(uri))
                    return cache[uri];

            // endpoint is down
            if (Endpoint.State != LocalEndpointState.Established)
                return null;

            // we associate the presence context with ourselves
            var view = Endpoint.PresenceServices.GetRemotePresenceViews()
                .FirstOrDefault(i => i.ApplicationContext == this);
            if (view == null)
            {
                view = new RemotePresenceView(Endpoint, new RemotePresenceViewSettings()
                {
                    SubscriptionMode = RemotePresenceViewSubscriptionMode.Default,
                    PollingInterval = TimeSpan.FromSeconds(15),
                });
                view.ApplicationContext = this;
                view.PresenceNotificationReceived += view_PresenceNotificationReceived;
            }

            // subscribe to the requested uri if not already done
            if (!view.GetPresentities().Contains(uri))
            {
                view.StartSubscribingToPresentities(new[] { new RemotePresentitySubscriptionTarget(uri) });
                cacheWh.Reset();
            }

            // wait a bit for state to arrive
            var t1 = WaitPresenceStateAsync(uri);
            var d1 = Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
            await Task.WhenAny(t1, d1);

            // state arrived
            if (t1.IsCompleted)
                return await t1;

            // taking too long; initiate a manual query
            var t2 = GetPresenceQueryAsync(uri, cancellationToken);
            var d2 = Task.Delay(TimeSpan.FromSeconds(5), cancellationToken);
            await Task.WhenAny(t1, t2, d2);

            // state finally arrived
            if (t1.IsCompleted)
                return await t1;

            // manual query completed
            if (t2.IsCompleted && t2.Result != null)
                // insert manual query result into cache
                lock (cache)
                    return cache[uri] = t2.Result;

            // could not obtain state in any way
            return null;
        }

        /// <summary>
        /// Initiates a wait for cached presence state.
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        async Task<PresenceState> WaitPresenceStateAsync(string uri)
        {
            // wait 10 seconds for data
            for (int i = 0; i < 20; i++)
            {
                lock (cache)
                    if (cache.ContainsKey(uri))
                        return cache[uri];

                // wait for presence data
                await Task.WhenAny(cacheWh.WaitAsync(), Task.Delay(TimeSpan.FromSeconds(.5)));
            }

            return null;
        }

        /// <summary>
        /// Invoked when the presence view is notified of new data.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        void view_PresenceNotificationReceived(object sender, RemotePresentitiesNotificationEventArgs args)
        {
            // insert new data into cache
            lock (cache)
                foreach (var i in args.Notifications)
                    cache[i.PresentityUri] = i.AggregatedPresenceState;

            // pulse anybody waiting
            cacheWh.Set();
        }

        /// <summary>
        /// Gets the presence state using a simple query.
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        async Task<PresenceState> GetPresenceQueryAsync(string uri, CancellationToken cancellationToken)
        {
            // default category set
            var categories = Endpoint.PresenceServices.PresenceSubscriptionCategories
                .ToArray();

            // initiate query
            return (await Endpoint.PresenceServices.PresenceQueryAsync(new[] { uri }, categories))
                .EmptyIfNull()
                .Where(i => i.PresentityUri == uri)
                .Select(i => i.AggregatedPresenceState)
                .FirstOrDefault();
        }

    }

}
