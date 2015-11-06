using System.Threading;
using System.Threading.Tasks;

using Microsoft.Rtc.Collaboration;
using Microsoft.Rtc.Collaboration.AudioVideo;

namespace UcmaKit.Rtc.Acd
{

    /// <summary>
    /// Serves as a base class for call actions that delegate to a single other call action.
    /// </summary>
    public abstract class AcdDelegate : AcdAction
    {

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        public AcdDelegate()
        {

        }

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="action"></param>
        public AcdDelegate(AcdAction action)
        {
            Action = action;
        }

        /// <summary>
        /// Gets the action this action dispatches to.
        /// </summary>
        public AcdAction Action { get; set; }

        public override Task<AcdActionResult> Execute(LocalEndpoint localEndpoint, AudioVideoCall call, CancellationToken cancellationToken)
        {
            return Action != null ? Action.Execute(localEndpoint, call, cancellationToken) : AcdActionResult.ContinueTask;
        }

    }

}
