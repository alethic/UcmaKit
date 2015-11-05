using System.Diagnostics;
using System.ServiceProcess;
using System.Threading;

namespace ISI.Rtc.Service
{

    public abstract class RtcService : ServiceBase
    {

        Thread th;
        ManualResetEvent wh;
        RtcApplicationRuntime runtime;

        protected override void OnStart(string[] args)
        {
            if (runtime == null)
            {
                runtime = new RtcApplicationRuntime();
                runtime.UnhandledException += Runtime_UnhandledException;
            }

            runtime.Start().Wait();

            // keep the process alive
            wh = new ManualResetEvent(false);
            th = new Thread(ThreadRun);
            th.Start();

            base.OnStart(args);
        }

        protected override void OnStop()
        {
            if (runtime != null)
            {
                runtime.Shutdown().Wait();
                runtime.Dispose();
                runtime.UnhandledException -= Runtime_UnhandledException;
                runtime = null;
            }

            if (th != null)
            {
                wh.Set();
                th.Join();
            }

            base.OnStop();
        }

        /// <summary>
        /// Does no work.
        /// </summary>
        void ThreadRun()
        {
            while (!wh.WaitOne())
                continue;
        }

        void Runtime_UnhandledException(object sender, ExceptionEventArgs args)
        {
            switch (args.Severity)
            {
                case ExceptionSeverityLevel.Info:
                    EventLog.WriteEntry(args.Exception.ToString(), EventLogEntryType.Information);
                    break;
                case ExceptionSeverityLevel.Warning:
                    EventLog.WriteEntry(args.Exception.ToString(), EventLogEntryType.Warning);
                    break;
                case ExceptionSeverityLevel.Error:
                    EventLog.WriteEntry(args.Exception.ToString(), EventLogEntryType.Error);
                    break;
                case ExceptionSeverityLevel.Fatal:
                    EventLog.WriteEntry(args.Exception.ToString(), EventLogEntryType.FailureAudit);
                    //Stop();
                    break;
            }
        }

    }

}
