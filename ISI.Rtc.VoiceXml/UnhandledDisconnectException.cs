using System;

using Microsoft.Speech.VoiceXml.Common;

namespace ISI.Rtc.VoiceXml
{

    public class UnhandledDisconnectException : Exception
    {

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="state"></param>
        public UnhandledDisconnectException(DisconnectState state)
            : base(state.ToString())
        {
            State = state;
        }

        /// <summary>
        /// State of disconnect.
        /// </summary>
        public DisconnectState State { get; private set; }

    }

}
