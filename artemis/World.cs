using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Artemis
{
    public class World
    {
        public EntityManager EntityManager { get; private set; }

        internal ComponantManager GetComponentManager()
        {
            throw new NotImplementedException();
        }

        internal Entity GetEntity(int entityId)
        {
            throw new NotImplementedException();
        }

        internal object GetAspectSubscriptionManager()
        {
            throw new NotImplementedException();
        }
    }
}
