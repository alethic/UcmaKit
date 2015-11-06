using System.Threading.Tasks;

namespace UcmaKit.Rtc.Acd
{

    /// <summary>
    /// Describes the result of an <see cref="AcdAction"/>.
    /// </summary>
    public struct AcdActionResult
    {

        /// <summary>
        /// Indicates the execution is complete.
        /// </summary>
        public static readonly AcdActionResult Complete = new AcdActionResult(AcdActionResultStatus.Complete);

        /// <summary>
        /// Indicates the execution should continue.
        /// </summary>
        public static readonly AcdActionResult Continue = new AcdActionResult(AcdActionResultStatus.Continue);

        /// <summary>
        /// Indicates the execution is complete.
        /// </summary>
        public static readonly Task<AcdActionResult> CompleteTask = Task.FromResult(Complete);

        /// <summary>
        /// Indicates the execution should continue.
        /// </summary>
        public static readonly Task<AcdActionResult> ContinueTask = Task.FromResult(Continue);

        /// <summary>
        /// Implicitly converts an <see cref="AcdActionResult" /> to a <see cref="Boolean"/> representing completion.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static implicit operator bool(AcdActionResult value)
        {
            return value.Status == AcdActionResultStatus.Complete;
        }

        /// <summary>
        /// Implicitly converts an <see cref="Boolean" /> to a <see cref="AcdActionResult"/> representing completion.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static implicit operator AcdActionResult(bool value)
        {
            return value ? Complete : Continue;
        }

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="status"></param>
        public AcdActionResult(AcdActionResultStatus status)
            : this()
        {
            Status = status;
        }

        /// <summary>
        /// Gets whether or not the call is complete.
        /// </summary>
        public AcdActionResultStatus Status { get; private set; }

    }

}
