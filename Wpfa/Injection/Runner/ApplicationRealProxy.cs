using System;
using System.Runtime.Remoting.Activation;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Remoting.Proxies;
using System.Runtime.Remoting.Services;

namespace Wpfa.Injection.Runner
{
    internal class ApplicationRealProxy : RealProxy
    {
        private readonly string _exeFile;
        private readonly IApplicationInjector _injector;

        public ApplicationRealProxy(Type fixtureType, string exeFile, IApplicationInjector injector)
            : base(fixtureType)
        {
            _exeFile = exeFile;
            _injector = injector;
        }

        public override IMessage Invoke(IMessage msg)
        {
            var constructionCallMessage = msg as IConstructionCallMessage;
            if (constructionCallMessage != null)
            {
                if (!_injector.Attach(_exeFile))
                {
                    throw new InvalidOperationException("Not attached");
                }

                _injector.Insider.CreateInstance(constructionCallMessage.ActivationType, constructionCallMessage.Args);

                return EnterpriseServicesHelper.CreateConstructionReturnMessage(constructionCallMessage, (MarshalByRefObject)GetTransparentProxy());
            }

            var methodCall = msg as IMethodCallMessage;
            if (methodCall != null)
            {
                return _injector.Insider.Invoke(methodCall);
            }

            throw new InvalidOperationException();
        }
    }
}