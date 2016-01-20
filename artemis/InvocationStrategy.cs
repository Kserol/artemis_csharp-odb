using Artemis.Utils;
using System;

namespace Artemis
{
    public class InvocationStrategy : SystemInvocationStrategy
    {
        protected override void Process(Bag<BaseSystem> systems)
        {
            foreach ( var s in systems )
            {
                BaseSystem system = (BaseSystem)s;
                system.Process();
                UpdateEntityStates();
            }
        }
    }
}