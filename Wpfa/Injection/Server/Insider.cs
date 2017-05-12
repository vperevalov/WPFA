using System;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Messaging;
using System.Windows;
using System.Windows.Threading;

namespace Wpfa.Injection.Server
{
    internal class Insider : MarshalByRefObject, IInsider
    {
        private MarshalByRefObject _object;

        private static Application Application
        {
            get { return Application.Current; }
        }

        public void CreateInstance(Type type, object[] args)
        {
            UiInvoke(() => CreateInstanceInternal(type, args));
        }

        public override object InitializeLifetimeService()
        {
            return null;
        }

        private void CreateInstanceInternal(Type type, object[] args)
        {
            AppDomain.CurrentDomain.SetData(WellKnownNames.CreateInstanceNormallyMarker, true);

            _object = (MarshalByRefObject)Activator.CreateInstance(type, args);
        }

        public IMessage Invoke(IMethodCallMessage msg)
        {
            IMethodReturnMessage result = null;

            UiInvoke(() => InvokeInternal(msg, out result));

            return result;
        }

        private void InvokeInternal(IMethodCallMessage msg, out IMethodReturnMessage result)
        {
            result = RemotingServices.ExecuteMessage(_object, msg);
        }

        public bool IsAlive()
        {
            return Application != null;
        }

        private void UiInvoke(Action action)
        {
            var dispatcher = Application.Dispatcher;

            dispatcher.Invoke(action, DispatcherPriority.ApplicationIdle);
        }
    }
}