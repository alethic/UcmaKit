using System;

using Microsoft.Speech.VoiceXml.Common;

namespace ISI.Rtc.VoiceXml
{

    public class UnhandledPageThrowException : Exception
    {

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="ev"></param>
        public UnhandledPageThrowException(PageEvent ev)
            : base(ev.Event + ": " + ev.Message)
        {
            Event = ev;
        }

        /// <summary>
        /// Event that was unhandled.
        /// </summary>
        public PageEvent Event { get; private set; }

    }

}
