using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ISI.Rtc
{

    public sealed class RtcApplicationRuntime : IDisposable
    {

        /// <summary>
        /// Set of loaded applications.
        /// </summary>
        List<RtcApplication> applications = new List<RtcApplication>();

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        public RtcApplicationRuntime()
            : this(RtcConfigurationSection.Default)
        {

        }

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="configurationSectionName"></param>
        public RtcApplicationRuntime(string configurationSectionName)
            : this(RtcConfigurationSection.GetSection(configurationSectionName))
        {

        }

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="configuration"></param>
        public RtcApplicationRuntime(RtcConfigurationSection configuration)
        {
            // construct applications
            foreach (var cfg in configuration.Applications)
                applications.Add((RtcApplication)Activator.CreateInstance(cfg.Type, new[] { cfg }));

            foreach (var app in applications)
                app.UnhandledException += application_UnhandledException;
        }

        /// <summary>
        /// Invoked when any of the loaded applications has an unhandled exception.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        void application_UnhandledException(object sender, ExceptionEventArgs args)
        {
            OnUnhandledException(args);
        }

        /// <summary>
        /// Gets the set of loaded applications.
        /// </summary>
        public IEnumerable<RtcApplication> Applications
        {
            get { return applications; }
        }

        /// <summary>
        /// Starts the runtime.
        /// </summary>
        /// <returns></returns>
        public async Task Start()
        {
            foreach (var a in applications)
                await a.Start();
        }

        /// <summary>
        /// Stops the runtime.
        /// </summary>
        /// <returns></returns>
        public async Task Shutdown()
        {
            foreach (var a in applications)
                await a.Shutdown();
        }

        /// <summary>
        /// Raised when an unhandled exception occurs.
        /// </summary>
        public event EventHandler<ExceptionEventArgs> UnhandledException;

        /// <summary>
        /// Raises the UnhandledException event.
        /// </summary>
        /// <param name="args"></param>
        void OnUnhandledException(ExceptionEventArgs args)
        {
            if (UnhandledException != null)
                UnhandledException(this, args);
        }

        /// <summary>
        /// Disposes of the instance.
        /// </summary>
        public void Dispose()
        {
            foreach (var a in applications)
                a.Shutdown().Wait();

            foreach (var application in Applications)
                application.Dispose();
        }

    }

}
