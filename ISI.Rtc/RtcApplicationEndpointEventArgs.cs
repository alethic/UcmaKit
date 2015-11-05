using System;

using Microsoft.Rtc.Collaboration;

namespace ISI.Rtc
{

    public class RtcApplicationEndpointEventArgs : EventArgs
    {

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="endpoint"></param>
        internal RtcApplicationEndpointEventArgs(ApplicationEndpoint endpoint)
        {
            Endpoint = endpoint;
        }

        /// <summary>
        /// Gets the endpoint.
        /// </summary>
        public ApplicationEndpoint Endpoint { get; private set; }

    }

}
