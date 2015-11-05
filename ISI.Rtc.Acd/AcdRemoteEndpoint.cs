namespace ISI.Rtc.Acd
{

    /// <summary>
    /// Describes a remote endpoint.
    /// </summary>
    public class AcdRemoteEndpoint
    {

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="uri"></param>
        public AcdRemoteEndpoint(string uri)
        {
            Uri = uri;
        }

        /// <summary>
        /// Gets the endpoint URI.
        /// </summary>
        public string Uri { get; private set; }

    }

}
