using System.Collections;
using System.Collections.Generic;
using System.Linq;

using ISI.Rtc.Util;

namespace ISI.Rtc.Acd
{

    /// <summary>
    /// Serves as the base class for call actions  that dispatch to multiple other actions.
    /// </summary>
    public abstract class AcdMultiDelegate : AcdAction, IEnumerable<AcdAction>
    {

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        protected AcdMultiDelegate()
            : this(Enumerable.Empty<AcdAction>())
        {

        }

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="delegates"></param>
        protected AcdMultiDelegate(IEnumerable<AcdAction> delegates)
        {
            Delegates = delegates;
        }

        /// <summary>
        /// Gets or sets the set of actions to delegate to.
        /// </summary>
        public IEnumerable<AcdAction> Delegates { get; set; }

        /// <summary>
        /// Adds the given path.
        /// </summary>
        /// <param name="action"></param>
        public void Add(AcdAction action)
        {
            Delegates = Delegates.Append(action);
        }

        public IEnumerator<AcdAction> GetEnumerator()
        {
            return Delegates.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

    }

}
