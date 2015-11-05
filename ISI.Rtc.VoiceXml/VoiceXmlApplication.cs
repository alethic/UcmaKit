using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Rtc.Collaboration;
using Microsoft.Rtc.Collaboration.AudioVideo;
using Microsoft.Rtc.Collaboration.AudioVideo.VoiceXml;
using Microsoft.Rtc.Signaling;
using Microsoft.Speech.VoiceXml.Common;

using Nito.AsyncEx;

using ISI.Rtc.Util;
using ISI.Rtc.VoiceXml.Data;

namespace ISI.Rtc.VoiceXml
{

    public class VoiceXmlApplication : ISI.Rtc.RtcApplication
    {

        Timer timer;
        Dictionary<RealTimeAddress, Uri> applications;

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="configuration"></param>
        public VoiceXmlApplication(RtcApplicationConfigurationElement configuration)
            : base(configuration)
        {
            applications = new Dictionary<RealTimeAddress, Uri>();
        }

        /// <summary>
        /// Invoked on start up.
        /// </summary>
        /// <returns></returns>
        protected override Task OnStartup()
        {
            timer = new Timer(timer_Callback, null, 0, (long)TimeSpan.FromMinutes(5).TotalMilliseconds);
            return Task.FromResult(true);
        }

        /// <summary>
        /// Invoked periodically on a timer.
        /// </summary>
        /// <param name="state"></param>
        void timer_Callback(object state)
        {
            RefreshApplications();
        }

        /// <summary>
        /// Refreshes the known applications.
        /// </summary>
        void RefreshApplications()
        {
            try
            {
                using (var m = new VoiceXmlModelContainer())
                    applications = m.Applications
                        .Where(i => i.Uri != null)
                        .SelectMany(i => i.Endpoints)
                        .Where(i => i.Address != null)
                        .Select(i => new { i.Address, i.Application.Uri })
                        .Distinct()
                        .AsEnumerable()
                        .ToDictionary(i => new RealTimeAddress(i.Address), i => new Uri(i.Uri));
            }
            catch (Exception e)
            {
                // raise exception
                OnUnhandledException(new ExceptionEventArgs(e, ExceptionSeverityLevel.Error));
            }
        }

        /// <summary>
        /// Invoked on shutdown.
        /// </summary>
        /// <returns></returns>
        protected override Task OnShutdown()
        {
            if (timer != null)
            {
                timer.Dispose();
                timer = null;
            }

            return Task.FromResult(true);
        }

        protected override async Task HandleAudioVideoCall(CallReceivedEventArgs<AudioVideoCall> args, CancellationToken cancellationToken)
        {
            try
            {
                // only new conversations supported
                if (!args.IsNewConversation)
                    return;

                // locate destination application URI
                var uri = applications.GetOrDefault(new RealTimeAddress(args.Call.Conversation.LocalParticipant.Uri));
                if (uri == null)
                    throw new Exception(string.Format("No configured Application for {0}.", args.Call.Conversation.LocalParticipant.Uri));

                // signals activation of flow
                var active = new TaskCompletionSource();

                // subscribe to flow configuration requested
                args.Call.AudioVideoFlowConfigurationRequested += (s, a) =>
                {
                    // subscribe to flow state changes
                    if (a.Flow != null)
                        a.Flow.StateChanged += (s2, a2) =>
                        {
                            if (a2.PreviousState != MediaFlowState.Active &&
                                a2.State == MediaFlowState.Active)
                                Dispatch(() =>
                                {
                                    active.TrySetResult();
                                });
                        };
                };

                // accept the call and wait for an active flow
                await args.Call.AcceptAsync();
                await active.Task;

                // initiate browser
                using (var browser = new Browser())
                {
                    var result = new TaskCompletionSource<VoiceXmlResult>();
                    browser.SessionCompleted += (s3, a3) => OnSessionCompleted(result, a3);
                    browser.Disconnecting += (s3, a3) => OnDisconnecting(result, a3);
                    browser.Disconnected += (s3, a3) => OnDisconnected(result, a3);
                    browser.Transferring += (s3, a3) => OnTransferring(result, a3);
                    browser.Transferred += (s3, a3) => OnTransfered(result, a3);
                    browser.SetAudioVideoCall(args.Call);
                    browser.RunAsync(uri, null);
                    await result.Task;
                }
            }
            catch (Exception e)
            {
                OnUnhandledException(new ExceptionEventArgs(e, ExceptionSeverityLevel.Error));
            }
        }

        /// <summary>
        /// Invoked when the session is complete. Sets the results of the session on the given task source.
        /// </summary>
        /// <param name="tcs"></param>
        /// <param name="args"></param>
        void OnSessionCompleted(TaskCompletionSource<VoiceXmlResult> tcs, SessionCompletedEventArgs args)
        {
            if (tcs.Task.IsCompleted)
                return;

            Dispatch(() =>
            {
                if (tcs.Task.IsCompleted)
                    return;

                if (args.Cancelled)
                    tcs.SetCanceled();
                else if (args.Error != null)
                    tcs.SetException(args.Error);
                else if (args.Result.UnhandledThrowElement != null)
                    tcs.SetException(new UnhandledPageThrowException(args.Result.UnhandledThrowElement));
                else
                    tcs.SetResult(args.Result);
            });
        }

        /// <summary>
        /// Invoked when the browser is disconnecting.
        /// </summary>
        /// <param name="tcs"></param>
        /// <param name="args"></param>
        void OnDisconnecting(TaskCompletionSource<VoiceXmlResult> tcs, DisconnectingEventArgs args)
        {
            if (tcs.Task.IsCompleted)
                return;

            Dispatch(() =>
            {

            });
        }

        /// <summary>
        /// Invoked when the browser is disconnected.
        /// </summary>
        /// <param name="tcs"></param>
        /// <param name="args"></param>
        void OnDisconnected(TaskCompletionSource<VoiceXmlResult> tcs, DisconnectedEventArgs args)
        {
            Dispatch(() =>
            {
                if (tcs.Task.IsCompleted)
                    return;

                tcs.SetResult(null);
            });
        }

        /// <summary>
        /// Invoked when the browser transfers the call.
        /// </summary>
        /// <param name="tcs"></param>
        /// <param name="args"></param>
        void OnTransferring(TaskCompletionSource<VoiceXmlResult> tcs, TransferringEventArgs args)
        {
            Dispatch(() =>
            {

            });
        }

        /// <summary>
        /// Invoked when the browser transfers the call.
        /// </summary>
        /// <param name="tcs"></param>
        /// <param name="args"></param>
        void OnTransfered(TaskCompletionSource<VoiceXmlResult> tcs, TransferredEventArgs args)
        {
            Dispatch(() =>
            {

            });
        }

    }

}
