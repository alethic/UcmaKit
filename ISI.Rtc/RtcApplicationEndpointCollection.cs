using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Rtc.Collaboration;

namespace ISI.Rtc
{

    public class RtcApplicationEndpointCollection : IEnumerable<ApplicationEndpoint>
    {

        List<ApplicationEndpoint> l = new List<ApplicationEndpoint>();

        /// <summary>
        /// Adds an endpoint.
        /// </summary>
        /// <param name="endpoint"></param>
        internal void Add(ApplicationEndpoint endpoint)
        {
            l.Add(endpoint);
        }

        /// <summary>
        /// Removes the endpoint.
        /// </summary>
        /// <param name="endpoint"></param>
        /// <returns></returns>
        internal bool Remove(ApplicationEndpoint endpoint)
        {
            return l.Remove(endpoint);
        }

        /// <summary>
        /// Gets the endpoint at the given index.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public ApplicationEndpoint this[int index]
        {
            get { return l[index]; }
        }

        /// <summary>
        /// Gets the endpoint with the given uri.
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        public ApplicationEndpoint this[string uri]
        {
            get { return l.FirstOrDefault(i => i.OwnerUri == uri || i.OwnerPhoneUri == uri); }
        }

        /// <summary>
        /// Gets the default routing endpoint.
        /// </summary>
        public ApplicationEndpoint DefaultEndpoint
        {
            get { return l.FirstOrDefault(i => i.IsDefaultRoutingEndpoint == true); }
        }

        public IEnumerator<ApplicationEndpoint> GetEnumerator()
        {
            return l.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

    }

}
