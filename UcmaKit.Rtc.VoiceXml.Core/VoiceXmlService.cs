using System.ServiceProcess;

using UcmaKit.Rtc.Service;

namespace UcmaKit.Rtc.VoiceXml.Service
{

    public partial class VoiceXmlService : RtcService
    {

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        public VoiceXmlService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            base.OnStart(args);
        }

        protected override void OnStop()
        {
            base.OnStop();
        }

        public static void Main()
        {
            ServiceBase.Run(new[] { new VoiceXmlService() });
        }

    }

}
