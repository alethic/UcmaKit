using System;

namespace ISI.Rtc
{

    public class ExceptionEventArgs : EventArgs
    {

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="exception"></param>
        public ExceptionEventArgs(Exception exception)
            : this(exception, ExceptionSeverityLevel.Error)
        {

        }

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="exception"></param>
        /// <param name="severity"></param>
        public ExceptionEventArgs(Exception exception, ExceptionSeverityLevel severity)
        {
            Exception = exception;
            Severity = severity;
        }

        /// <summary>
        /// Exception that occured.
        /// </summary>
        public Exception Exception { get; private set; }

        /// <summary>
        /// Severity level of exception.
        /// </summary>
        public ExceptionSeverityLevel Severity { get; private set; }

    }

}
