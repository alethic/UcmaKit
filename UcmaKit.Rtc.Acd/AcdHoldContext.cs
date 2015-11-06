using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

using Microsoft.Rtc.Collaboration;
using Microsoft.Rtc.Collaboration.AudioVideo;

using UcmaKit.Rtc.Util;

namespace UcmaKit.Rtc.Acd
{

    /// <summary>
    /// Manages calls on hold.
    /// </summary>
    class AcdHoldContext : IDisposable
    {

        static readonly string LYNC_FILE_NAME = Path.GetTempFileName();
        static readonly string RING_FILE_NAME = Path.GetTempFileName();

        /// <summary>
        /// Initializes the static instance.
        /// </summary>
        static AcdHoldContext()
        {
            // load standard Lync hold audio
            using (var wma = Assembly.GetExecutingAssembly().GetManifestResourceStream("UcmaKit.Rtc.Acd.Audio.Lync.wma"))
            using (var stm = File.OpenWrite(LYNC_FILE_NAME))
                wma.CopyTo(stm);

            // load standard ring tone audio
            using (var wav = Assembly.GetExecutingAssembly().GetManifestResourceStream("UcmaKit.Rtc.Acd.Audio.Ring.wav"))
            using (var stm = File.OpenWrite(RING_FILE_NAME))
                wav.CopyTo(stm);
        }

        Player musicPlayer = new Player();

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        public AcdHoldContext()
        {
            musicPlayer.SetMode(PlayerMode.Automatic);
            musicPlayer.StateChanged += musicPlayer_StateChanged;
        }

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="audio"></param>
        public AcdHoldContext(AcdHoldAudio audio)
            : this()
        {
            Audio = audio;
        }

        /// <summary>
        /// Gets or sets the audio style.
        /// </summary>
        public AcdHoldAudio Audio { get; set; }

        /// <summary>
        /// Invoked when the music player's state changes.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        async void musicPlayer_StateChanged(object sender, PlayerStateChangedEventArgs args)
        {
            if (args.TransitionReason == PlayerStateTransitionReason.PlayCompleted)
                await StartAudioAsync();
        }

        /// <summary>
        /// Gets the chosen media source.
        /// </summary>
        /// <returns></returns>
        MediaSource GetSource()
        {
            switch (Audio)
            {
                case AcdHoldAudio.Lync:
                    return new WmaFileSource(LYNC_FILE_NAME);
                case AcdHoldAudio.Ring:
                    return new WmaFileSource(RING_FILE_NAME);
            }

            throw new Exception("Unknown audio.");
        }

        /// <summary>
        /// Changes the media source.
        /// </summary>
        async Task StartAudioAsync()
        {
            // load new source
            var source = GetSource();
            if (source == null)
                return;

            // load source
            await source.PrepareSourceAsync(MediaSourceOpenMode.Buffered);

            // configure player
            musicPlayer.SetSource(source);
            musicPlayer.Start();
        }

        /// <summary>
        /// Attaches a call to the hold context.
        /// </summary>
        /// <param name="call"></param>
        public async Task HoldAsync(AudioVideoCall call)
        {
            if (call.State == CallState.Terminating ||
                call.State == CallState.Terminated)
                return;

            // accept the call if not accepted yet
            if (call.State != CallState.Established)
                await call.AcceptAsync();

            // start audio on first call
            if (musicPlayer.AudioVideoFlows.Count == 0)
                await StartAudioAsync();

            // put the call on hold and attach player
            await call.Flow.HoldAsync(HoldType.RemoteEndpointMusicOnHold);
            musicPlayer.AttachFlow(call.Flow);
        }

        /// <summary>
        /// Detaches a call from the hold context.
        /// </summary>
        /// <param name="call"></param>
        public async Task RetrieveAsync(AudioVideoCall call)
        {
            if (call.State != CallState.Established)
                return;

            // detach from player
            musicPlayer.DetachFlow(call.Flow);

            // remove call from hold
            if (call.Flow.HoldStatus != HoldType.None)
                await call.Flow.RetrieveAsync();
        }

        /// <summary>
        /// Disposes of the instance.
        /// </summary>
        public void Dispose()
        {
            if (musicPlayer != null)
            {
                // detach existing calls
                foreach (var flow in musicPlayer.AudioVideoFlows.ToList())
                    musicPlayer.DetachFlow(flow);

                // clean up player
                musicPlayer.Stop();
                musicPlayer.RemoveSource();
                musicPlayer = null;
            }

            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Finalizes the instance.
        /// </summary>
        ~AcdHoldContext()
        {
            Dispose();
        }

    }

}
