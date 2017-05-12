using Wpfa.Injection.Server;

namespace Wpfa.Injection.Runner
{
    internal interface IApplicationInjector
    {
        bool Attach(string app);

        IInsider Insider { get; }
    }
}