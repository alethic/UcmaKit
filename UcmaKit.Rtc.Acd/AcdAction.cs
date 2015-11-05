using System.Threading;
using System.Threading.Tasks;

using Microsoft.Rtc.Collaboration;
using Microsoft.Rtc.Collaboration.AudioVideo;

namespace ISI.Rtc.Acd
{

    /// <summary>
    /// Serves as the base class for elements that accept and process a call.
    /// </summary>
    public abstract class AcdAction
    {

        /// <summary>
        /// Attempts to handle the call. Returns <c>true</c> if the call has been successfully handled.
        /// </summary>
        /// <param name="localEndpoint"></param>
        /// <param name="call"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public abstract Task<AcdActionResult> Execute(LocalEndpoint localEndpoint, AudioVideoCall call, CancellationToken cancellationToken);

    }

}
