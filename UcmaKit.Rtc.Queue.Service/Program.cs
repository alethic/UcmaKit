using System.Threading.Tasks;

using Topshelf;

namespace ISI.Rtc.Queue.Service
{

    public class Program
    {

        public static int Main(string[] args)
        {
            return (int)HostFactory.Run(x =>
            {
                x.Service<RtcApplicationRuntime>(s =>
                {
                    s.ConstructUsing(() => new RtcApplicationRuntime());
                    s.WhenStarted(h => Task.Run(async () => await h.Start()).Wait());
                    s.WhenStopped(h => Task.Run(async () => await h.Shutdown()).Wait());
                });
                x.SetServiceName("ISI.Rtc.Queue");
                x.RunAsNetworkService();
            });
        }

    }

}
