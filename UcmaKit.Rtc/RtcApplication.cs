using System;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Rtc.Collaboration;
using Microsoft.Rtc.Collaboration.AudioVideo;

using Nito.AsyncEx;

using UcmaKit.Rtc.Util;

namespace UcmaKit.Rtc
{

    /// <summary>
    /// Provides a basic UCMA platform.
    /// </summary>
    public abstract class RtcApplication : IDisposable
    {

        CancellationTokenSource shutdownCancellationTokenSource;

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="configuration"></param>
        public RtcApplication(RtcApplicationConfigurationElement configuration)
        {
            if (configuration == null)
                throw new ArgumentNullException("configuration");

            // set configuration
            Configuration = configuration;

            // initialize
            State = RtcApplicationState.Stopped;
            Endpoints = new RtcApplicationEndpointCollection();
        }

        /// <summary>
        /// Context that invokes actions on the synchronization thread.
        /// </summary>
        RunnableSynchronizationContext SynchronizationContext { get; set; }

        /// <summary>
        /// Thread that drives application.
        /// </summary>
        Thread SynchronizationThread { get; set; }

        /// <summary>
        /// Gets the configuration for the platform.
        /// </summary>
        public RtcApplicationConfigurationElement Configuration { get; private set; }

        /// <summary>
        /// Gets the current platform state.
        /// </summary>
        public RtcApplicationState State { get; private set; }

        /// <summary>
        /// Underlying platform.
        /// </summary>
        public CollaborationPlatform CollaborationPlatform { get; private set; }

        /// <summary>
        /// Set of active endpoints.
        /// </summary>
        public RtcApplicationEndpointCollection Endpoints { get; private set; }

        /// <summary>
        /// Signaled when the endpoints are loaded.
        /// </summary>
        AsyncAutoResetEvent PlatformEndpointsEstablishedWh { get;  set; }

        /// <summary>
        /// Dispatches the given action.
        /// </summary>
        /// <param name="action"></param>
        public void Dispatch(Action action)
        {
            SynchronizationContext.Dispatch(action);
        }

        /// <summary>
        /// Invokes the given action.
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public Task InvokeAsync(Action action)
        {
            return SynchronizationContext.InvokeAsync(action);
        }

        /// <summary>
        /// Invokes the given function.
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="func"></param>
        /// <returns></returns>
        public Task<TResult> InvokeAsync<TResult>(Func<TResult> func)
        {
            return SynchronizationContext.InvokeAsync(func);
        }

        /// <summary>
        /// Override this method to implement startup behavior.
        /// </summary>
        protected virtual Task OnStartup()
        {
            return Task.FromResult(true);
        }

        /// <summary>
        /// Override this method to implement shutdown behavior.
        /// </summary>
        /// <returns></returns>
        protected virtual Task OnShutdown()
        {
            return Task.FromResult(true);
        }

        /// <summary>
        /// Finds the appropriate certificate given either a thumbprint or a subject name.
        /// </summary>
        /// <param name="thumbprint"></param>
        /// <param name="subjectName"></param>
        /// <returns></returns>
        X509Certificate2 GetServerCertificate(string thumbprint, string subjectName)
        {
            var stores = new[] 
            { 
                new X509Store(StoreLocation.CurrentUser), 
                new X509Store(StoreLocation.LocalMachine),
            };

            // open the stores
            foreach (var store in stores)
                store.Open(OpenFlags.ReadOnly);

            var certificates = Enumerable.Empty<X509Certificate2>();

            // search by thumbprint
            if (!string.IsNullOrWhiteSpace(thumbprint))
                certificates = certificates
                    .Concat(stores.SelectMany(i => i.Certificates.Find(X509FindType.FindByThumbprint, thumbprint, true).OfType<X509Certificate2>()));

            // search by subjectname
            if (!string.IsNullOrWhiteSpace(subjectName))
                certificates = certificates
                    .Concat(stores.SelectMany(i => i.Certificates.Find(X509FindType.FindBySubjectName, subjectName, true).OfType<X509Certificate2>()));

            // get certificates
            certificates = certificates.ToList();

            // close the stores
            foreach (var store in stores)
                store.Close();

            // return first certificate with private key
            return certificates
                .Where(i => i.HasPrivateKey)
                .FirstOrDefault();
        }

        /// <summary>
        /// Creates the <see cref="CollaborationPlatform"/> instance.
        /// </summary>
        /// <returns></returns>
        CollaborationPlatform CreateCollaborationPlatform()
        {
            return Configuration.AutoProvision ? CreateProvisionedCollaborationPlatform() : CreateServerCollaborationPlatform();
        }

        /// <summary>
        /// Creates a <see cref="CollaborationPlatform"/> that auto provisions.
        /// </summary>
        /// <returns></returns>
        CollaborationPlatform CreateProvisionedCollaborationPlatform()
        {
            var userAgent = Configuration.UserAgent.TrimToNull();
            var applicationId = Configuration.ApplicationId.TrimToNull();

            if (userAgent == null)
                throw new ConfigurationErrorsException("Auto provisioning requires a configured UserAgent.");
            if (applicationId == null)
                throw new ConfigurationErrorsException("Auto provisioning requires a configured ApplicationId.");

            return new CollaborationPlatform(new ProvisionedApplicationPlatformSettings(Configuration.UserAgent, Configuration.ApplicationId));
        }

        /// <summary>
        /// Creates a <see cref="CollaborationPlatform"/> that is manually provisioned.
        /// </summary>
        /// <returns></returns>
        CollaborationPlatform CreateServerCollaborationPlatform()
        {
            var userAgent = Configuration.UserAgent.TrimToNull();
            var gruu = Configuration.Gruu.TrimToNull();
            var localhost = Configuration.Localhost.TrimToNull() ?? Dns.GetHostEntry("localhost").HostName;
            var port = Configuration.Port;
            var certificateThumbprint = Configuration.CertificateThumbprint.TrimToNull();
            var certificateSubjectName = Configuration.CertificateSubjectName.TrimToNull();

            if (userAgent == null)
                throw new ConfigurationErrorsException("Manual provisioning requires a configured UserAgent.");
            if (gruu == null)
                throw new ConfigurationErrorsException("Manual provisioning requires a configured ApplicationId.");
            if (localhost == null)
                throw new ConfigurationErrorsException("Manual provisioning requires a localhost name.");
            if (port == 0)
                throw new ConfigurationErrorsException("Manual provisioning requires a port.");

            // attempt to find certificate
            var c = GetServerCertificate(certificateThumbprint, certificateSubjectName ?? localhost);

            // configure settings
            var s = c != null ? new ServerPlatformSettings(userAgent, localhost, port, gruu, c) : new ServerPlatformSettings(userAgent, localhost, port, gruu);

            // generate platform
            return new CollaborationPlatform(s);
        }

        /// <summary>
        /// Starts the platform.
        /// </summary>
        /// <returns></returns>
        public async Task Start()
        {
            if (State == RtcApplicationState.Stopped)
            {
                State = RtcApplicationState.Starting;

                // establish new synchronization context
                if (SynchronizationContext == null)
                {
                    SynchronizationContext = new RunnableSynchronizationContext();
                    SynchronizationThread = new Thread(SynchronizationContext.Run);
                    SynchronizationThread.IsBackground = true;
                    SynchronizationThread.Start();
                }

                // initiate startup
                await await InvokeAsync(async () =>
                {
                    shutdownCancellationTokenSource = new CancellationTokenSource();

                    // establish platform
                    if (CollaborationPlatform == null)
                        CollaborationPlatform = CreateCollaborationPlatform();

                    // establish endpoint provisioning wait handle, if auto-provisioned
                    PlatformEndpointsEstablishedWh = new AsyncAutoResetEvent();
                    if (Configuration.AutoProvision)
                        // auto provisioning will invoke a settings handler, which will allow us to proceed
                        CollaborationPlatform.RegisterForApplicationEndpointSettings(OnPlatformApplicationEndpointSettingsCb);
                    else
                        // manual provisioning: set the handle so we proceed anyways
                        PlatformEndpointsEstablishedWh.Set();

                    // proceed with startup
                    await CollaborationPlatform.StartupAsync();
                    await PlatformEndpointsEstablishedWh.WaitAsync();
                    await EstablishApplicationEndpoints();
                    await OnStartup();
                });

                State = RtcApplicationState.Started;
            }
        }

        /// <summary>
        /// Creates application endpoint settings with default configured information.
        /// </summary>
        /// <param name="ownerUri"></param>
        /// <returns></returns>
        protected ApplicationEndpointSettings CreateApplicationEndpointSettings(string ownerUri)
        {
            if (string.IsNullOrWhiteSpace(Configuration.Registrar))
                throw new ConfigurationErrorsException("Cannot create manually provisioned endpoint settings without Registrar.");
            if (Configuration.RegistrarPort == 0)
                throw new ConfigurationErrorsException("Cannot create manually provisioned endpoint settings without RegistrarPort.");

            return new ApplicationEndpointSettings(ownerUri, Configuration.Registrar, Configuration.RegistrarPort);
        }

        /// <summary>
        /// Invoked when endpoint settings are delivered.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        void OnPlatformApplicationEndpointSettingsCb(object sender, ApplicationEndpointSettingsDiscoveredEventArgs args)
        {
            Dispatch(() => OnPlatformApplicationEndpointSettings(sender, args));
        }

        /// <summary>
        /// Creates and establishes a new application endpoint.
        /// </summary>
        /// <param name="settings"></param>
        /// <returns></returns>
        protected async Task<ApplicationEndpoint> EstablishApplicationEndpoint(ApplicationEndpointSettings settings)
        {
            // establish new endpoint
            var endpoint = new ApplicationEndpoint(CollaborationPlatform, settings);
            endpoint.RegisterForIncomingCall<AudioVideoCall>(OnIncomingAudioVideoCallCb);
            endpoint.RegisterForIncomingCall<InstantMessagingCall>(OnIncomingInstantMessagingCallCb);
            await endpoint.EstablishAsync();

            // add endpoint to list of endpoints and return
            Endpoints.Add(endpoint);
            OnEndpointAvailable(new RtcApplicationEndpointEventArgs(endpoint));
            return endpoint;
        }

        /// <summary>
        /// Invoked when endpoint settings are delivered.
        /// </summary>
        /// <param name="args"></param>
        async void OnPlatformApplicationEndpointSettings(object sender, ApplicationEndpointSettingsDiscoveredEventArgs args)
        {
            try
            {
                // already stopped
                if (State != RtcApplicationState.Started &&
                    State != RtcApplicationState.Starting)
                    return;

                // allow user to modify settings
                await InterceptPlatformApplicationEndpointSettings(args.ApplicationEndpointSettings);

                // establish new endpoint
                await EstablishApplicationEndpoint(args.ApplicationEndpointSettings);
            }
            catch (Exception e)
            {
                OnUnhandledException(new ExceptionEventArgs(e));
            }
            finally
            {
                PlatformEndpointsEstablishedWh.Set();
            }
        }

        /// <summary>
        /// Invoked when application settings are discovered. Override this method to alter the discovered settings.
        /// </summary>
        /// <param name="settings"></param>
        /// <returns></returns>
        protected virtual Task InterceptPlatformApplicationEndpointSettings(ApplicationEndpointSettings settings)
        {
            return Task.FromResult(false);
        }

        /// <summary>
        /// Initiates a refresh of the manually provisioned application endpoints.
        /// </summary>
        protected Task RefreshApplicationEndpoints()
        {
            return EstablishApplicationEndpoints();
        }

        /// <summary>
        /// Invoked to register manual application endpoints.
        /// </summary>
        /// <returns></returns>
        protected virtual async Task EstablishApplicationEndpoints()
        {
            foreach (var endpointConfiguration in Configuration.Endpoints)
            {
                // already configured
                if (Endpoints.Any(i => i.OwnerUri == endpointConfiguration.Uri))
                    return;

                var settings = CreateApplicationEndpointSettings(endpointConfiguration.Uri);
                settings.OwnerPhoneUri = endpointConfiguration.PhoneUri;
                settings.IsDefaultRoutingEndpoint = !Endpoints.Any(i => i.IsDefaultRoutingEndpoint) ? endpointConfiguration.IsDefaultRoutingEndpoint : false;
                settings.AutomaticPresencePublicationEnabled = true;
                settings.UseRegistration = false;

                var capabilities = settings.Presence.PreferredServiceCapabilities;
                capabilities.InstantMessagingSupport = CapabilitySupport.UnSupported;
                capabilities.AudioSupport = CapabilitySupport.Supported;
                capabilities.ApplicationSharingSupport = CapabilitySupport.UnSupported;
                capabilities.VideoSupport = CapabilitySupport.UnSupported;

                await EstablishApplicationEndpoint(settings);
            }
        }

        /// <summary>
        /// Invoked when a new incoming audio call is received.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        void OnIncomingAudioVideoCallCb(object sender, CallReceivedEventArgs<AudioVideoCall> args)
        {
            Dispatch(() => OnIncomingAudioVideoCall(sender, args));
        }

        /// <summary>
        /// Invoked when a new incoming audio call is received.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        protected virtual async void OnIncomingAudioVideoCall(object sender, CallReceivedEventArgs<AudioVideoCall> args)
        {
            // cancel hunting when call terminates
            var callCts = new CancellationTokenSource();
            args.Call.StateChanged += (s, a) =>
            {
                if (a.State == CallState.Terminating ||
                    a.State == CallState.Terminated)
                    callCts.Cancel();
            };

            try
            {
                await HandleAudioVideoCall(args, callCts.Token);
            }
            catch (Exception e)
            {
                OnUnhandledException(new ExceptionEventArgs(e, ExceptionSeverityLevel.Error));
            }

            // terminate call if not terminated
            try
            {
                if (args.Call.State != CallState.Terminated &&
                    args.Call.State != CallState.Terminating)
                    await args.Call.TerminateAsync();
            }
            catch (Exception e)
            {
                OnUnhandledException(new ExceptionEventArgs(e, ExceptionSeverityLevel.Error));
            }
        }

        /// <summary>
        /// Override to handle incoming <see cref="AudioVideoCall"/>s. Returned task should indicate completion of call.
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        protected virtual Task HandleAudioVideoCall(CallReceivedEventArgs<AudioVideoCall> args, CancellationToken cancellationToken)
        {
            return Task.FromResult(true);
        }

        /// <summary>
        /// Invoked when a new incoming instant messaging call is received.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        void OnIncomingInstantMessagingCallCb(object sender, CallReceivedEventArgs<InstantMessagingCall> args)
        {
            Dispatch(() => OnIncomingInstantMessagingCall(sender, args));
        }

        /// <summary>
        /// Invoked when a new incoming instant messaging call is received.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        async void OnIncomingInstantMessagingCall(object sender, CallReceivedEventArgs<InstantMessagingCall> args)
        {
            // cancel hunting when call terminates
            var callCts = new CancellationTokenSource();
            args.Call.StateChanged += (s, a) =>
            {
                if (a.State == CallState.Terminating ||
                    a.State == CallState.Terminated)
                    callCts.Cancel();
            };

            try
            {
                await HandleInstantMessagingCall(args, callCts.Token);
            }
            catch (Exception e)
            {
                OnUnhandledException(new ExceptionEventArgs(e, ExceptionSeverityLevel.Error));
            }

            // terminate call if not terminated
            try
            {
                if (args.Call.State != CallState.Terminated &&
                    args.Call.State != CallState.Terminating)
                    await args.Call.TerminateAsync();
            }
            catch (Exception e)
            {
                OnUnhandledException(new ExceptionEventArgs(e, ExceptionSeverityLevel.Error));
            }
        }

        /// <summary>
        /// Override to handle incoming <see cref="InstantMessagingCall"/>s. Returned task should indicate completion of call.
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        protected virtual Task HandleInstantMessagingCall(CallReceivedEventArgs<InstantMessagingCall> args, CancellationToken cancellationToken)
        {
            return Task.FromResult(true);
        }

        /// <summary>
        /// Shuts down all of the application endpoints.
        /// </summary>
        /// <returns></returns>
        async Task ShutdownApplicationEndpoints()
        {
            // unregister from receiving new calls
            foreach (var e in Endpoints)
            {
                e.UnregisterForIncomingCall<AudioVideoCall>(OnIncomingAudioVideoCallCb);
                e.UnregisterForIncomingCall<InstantMessagingCall>(OnIncomingInstantMessagingCallCb);
            }

            // drain each endpoint (wait for all calls to exit)
            foreach (var e in Endpoints)
                await e.DrainAsync();

            // terminate each endpoint
            foreach (var e in Endpoints)
                await e.TerminateAsync();

            // empty list
            foreach (var e in Endpoints.ToList())
            {
                Endpoints.Remove(e);
                OnEndpointUnavailable(new RtcApplicationEndpointEventArgs(e));
            }
        }

        /// <summary>
        /// Stops the platform.
        /// </summary>
        /// <returns></returns>
        public async Task Shutdown()
        {
            if (State == RtcApplicationState.Started)
            {
                State = RtcApplicationState.Stopping;

                // initiate shutdown
                await await InvokeAsync(async () =>
                {
                    if (Configuration.AutoProvision)
                        CollaborationPlatform.UnregisterForApplicationEndpointSettings(OnPlatformApplicationEndpointSettingsCb);

                    shutdownCancellationTokenSource.Cancel();
                    await OnShutdown();
                    await ShutdownApplicationEndpoints();
                    await CollaborationPlatform.ShutdownAsync();
                });

                State = RtcApplicationState.Stopped;
            }
        }

        /// <summary>
        /// Implements IDisposable.Dispose.
        /// </summary>
        public void Dispose()
        {
            // stop platform if started
            if (State == RtcApplicationState.Started)
                Shutdown().Wait();

            // dispose of context
            if (SynchronizationContext != null)
            {
                SynchronizationContext.Complete().Wait();
                SynchronizationContext = null;
            }

            // dispose of thread
            if (SynchronizationThread != null)
            {
                SynchronizationThread.Join();
                SynchronizationThread = null;
            }

            // dispose of collaboration platform
            if (CollaborationPlatform != null)
                CollaborationPlatform = null;
        }

        /// <summary>
        /// Raised when an endpoint is added.
        /// </summary>
        public event EventHandler<RtcApplicationEndpointEventArgs> EndpointAvailable;

        /// <summary>
        /// Raises the EndpointAvailable event.
        /// </summary>
        /// <param name="args"></param>
        void OnEndpointAvailable(RtcApplicationEndpointEventArgs args)
        {
            if (EndpointAvailable != null)
                EndpointAvailable(this, args);
        }

        /// <summary>
        /// Raised when an endpoint is removed.
        /// </summary>
        public event EventHandler<RtcApplicationEndpointEventArgs> EndpointUnavailable;

        /// <summary>
        /// Raises the EndpointUnavailable event.
        /// </summary>
        /// <param name="args"></param>
        void OnEndpointUnavailable(RtcApplicationEndpointEventArgs args)
        {
            if (EndpointUnavailable != null)
                EndpointUnavailable(this, args);
        }

        /// <summary>
        /// Raised when an exception occurs.
        /// </summary>
        public event EventHandler<ExceptionEventArgs> UnhandledException;

        /// <summary>
        /// Raises the UnhandledException event.
        /// </summary>
        /// <param name="args"></param>
        protected void OnUnhandledException(ExceptionEventArgs args)
        {
            if (UnhandledException != null)
                UnhandledException(this, args);
        }

    }

}
