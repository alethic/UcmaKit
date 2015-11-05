using System;
using System.ComponentModel;
using System.ServiceProcess;

namespace ISI.Rtc.Acd.Service
{

    [RunInstaller(true)]
    public class AcdInstaller : System.Configuration.Install.Installer
    {

        public AcdInstaller()
        {
            var pi = new ServiceProcessInstaller();
            pi.Account = ServiceAccount.NetworkService;

            var si = new ServiceInstaller();
            si.StartType = ServiceStartMode.Automatic;
            si.DelayedAutoStart = true;
            si.ServiceName = "ISI Acd UCMA";

            Installers.Add(si);
            Installers.Add(pi);
        }

        public static void Main()
        {
            Console.WriteLine("Usage: InstallUtil.exe [<service>.exe]");
        }

    }

}
