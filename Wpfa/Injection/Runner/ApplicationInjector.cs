using System;
using System.Diagnostics;
using System.Threading;
using ManagedInjector;
using Wpfa.Injection.Server;

namespace Wpfa.Injection.Runner
{
    internal class ApplicationInjector : IApplicationInjector
    {
        private Process _process;

        public IInsider Insider
        {
            get { return (IInsider)Activator.GetObject(typeof(Insider), InsiderServiceUrl); }
        }

        private static string InsiderServiceUrl
        {
            get { return string.Format("ipc://{0}/{1}", WellKnownNames.ChannelName, WellKnownNames.InsiderObjectName); }
        }

        public bool Attach(string app)
        {
            _process = Process.Start(app);

            return AttachWithTries(_process, 10);
        }

        private bool AttachWithTries(Process process, int times)
        {
            bool result;
            try
            {
                LaunchInjector(process);

                Thread.Sleep(500);

                result = Insider.IsAlive();
            }
            catch (Exception)
            {
                result = false;
            }

            if (result)
            {
                return true;
            }

            if (times > 0)
            {
                Thread.Sleep(1000);

                return AttachWithTries(process, times - 1);
            }

            return false;
        }

        private static void LaunchInjector(Process processName)
        {
            Type injectinType = typeof(InjectionBody);

            string codebase = injectinType.Assembly.CodeBase;

            codebase = codebase.Replace("file:///", string.Empty);

            Injector.Launch(
                processName.MainWindowHandle,
                codebase,
                injectinType.FullName,
                "SetupServer");
        }
        
        ~ApplicationInjector()
        {
            if (_process != null && !_process.HasExited)
            {
                _process.Kill();
            }
        }
    }
}