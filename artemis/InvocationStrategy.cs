using Artemis.Utils;
using System;

namespace Artemis
{
    public class InvocationStrategy : SystemInvocationStrategy
    {
        public override void Process(Bag<BaseSystem> systems)
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