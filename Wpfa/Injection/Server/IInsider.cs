using System;
using System.Runtime.Remoting.Messaging;
namespace Wpfa.Injection.Server
{
    internal interface IInsider
    {
        void CreateInstance(Type type, object[] args);

        IMessage Invoke(IMethodCallMessage msg);

        bool IsAlive();
    }
}