using System;
using System.Runtime.Remoting.Proxies;
using Wpfa.Injection;
using Wpfa.Injection.Runner;

namespace Wpfa
{
    public class ProcessIsolationAttribute : ProxyAttribute
    {
        private readonly string _fileName;

        public ProcessIsolationAttribute(string fileName)
        {
            _fileName = fileName;
        }

        private static bool NeedCreateNormally
        {
            get
            {
                object marker = AppDomain.CurrentDomain.GetData(WellKnownNames.CreateInstanceNormallyMarker);
                
                bool needCreateNormally = marker is bool && (bool) marker;
               
                return needCreateNormally;
            }
        }

        public override MarshalByRefObject CreateInstance(Type serverType)
        {
            if (NeedCreateNormally)
            {
                return base.CreateInstance(serverType);
            }

            return (MarshalByRefObject)new ApplicationRealProxy(serverType, _fileName, new ApplicationInjector()).GetTransparentProxy();
        }
    }
}