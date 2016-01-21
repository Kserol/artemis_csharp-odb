using System;
using Artemis.Utils;
using System.Collections.Generic;

namespace Artemis
{
    public class AspectSubscriptionManager:BaseSystem
    {
        private readonly Dictionary<AspectPromise, EntitySubscription> subscriptionMap;
        private Bag<EntitySubscription> subscriptions;

        private  Bag<int> changedIds = new Bag<int>();
        private  Bag<int> deletedIds = new Bag<int>();

        public AspectSubscriptionManager()
        {
            subscriptionMap = new Dictionary<AspectPromise, EntitySubscription>();
            subscriptions = new Bag<EntitySubscription>();
        }


        /**
         * Get subscription to all entities matching {@link Aspect}.
         *
         * Will create a new subscription if not yet available for
         * given {@link Aspect} match.
         *
         * @param builder Aspect to match.
         * @return {@link EntitySubscription} for aspect.
         */
        public EntitySubscription Get(AspectPromise builder)
        {
            return ( subscriptionMap.ContainsKey(builder) ) ? subscriptionMap[builder] : this.CreateSubscription(builder);
        }

        private EntitySubscription CreateSubscription(AspectPromise builder)
        {
            EntitySubscription entitySubscription = new EntitySubscription(World, builder);
            subscriptionMap[builder] = entitySubscription;
            subscriptions.Add(entitySubscription);

            World.EntityManager.Synchronize(entitySubscription);

            return entitySubscription;
        }

        public void Process(BitSet changed, BitSet deleted)
        {
            ToEntityIntBags(changed, deleted);

            // note: processAll != process
            //TODO subscriptions[0].ProcessAll(changedIds, deletedIds);
            
            foreach (var s in subscriptions)
            {
                EntitySubscription subscriber = s;
                subscriber.Process(changedIds, deletedIds);
            }
        }

        private void ToEntityIntBags(BitSet changed, BitSet deleted)
        {
            changedIds = changed.ToIntBag();
            deletedIds = deleted.ToIntBag();

            changed.Clear();
            deleted.Clear();
        }

        public void ProcessComponentIdentity(int id, BitSet componentBits)
        {
            foreach (var s in subscriptions)
            {
                EntitySubscription subscriber = s;
                subscriber.ProcessComponentIdentity(id, componentBits);
            }
        }

        protected override void ProcessSystem()
        {
        }
    }
}