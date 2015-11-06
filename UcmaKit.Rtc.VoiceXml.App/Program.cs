using System;

namespace UcmaKit.Rtc.VoiceXml.App
{

    class Program
    {

        static void Main(string[] args)
        {
            using (var p = new UcmaKit.Rtc.RtcApplicationRuntime())
            {
                p.UnhandledException += p_UnhandledException;
                p.Start().Wait();
                Console.WriteLine("Started");
                Console.ReadLine();
                p.Shutdown().Wait();
                Console.WriteLine("Stopped");
                Console.ReadLine();
            }
        }

        static void p_UnhandledException(object sender, ExceptionEventArgs e)
        {
            Console.WriteLine(e.Exception);
        }

    }

}
