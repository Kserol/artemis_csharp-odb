using Artemis.Utils;
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
        public ComponentManager ComponentManager { get; private set; }
        public AspectSubscriptionManager AspectSubscriptionManager { get; private set; }
        public EntityEditPool EditPool { get; private set; }

        public Dictionary<Type, BaseSystem> Systems { get; internal set; }

        internal SystemInvocationStrategy InvocationStrategy
        {
            get
            {
                return invocationStrategy;
            }

            set
            {
                invocationStrategy = value;
            }
        }

        internal void UpdateEntityStates()
        {
            throw new NotImplementedException();
        }

        private Bag<BaseSystem> systemsBag;

        private SystemInvocationStrategy invocationStrategy;

        private IInjector injector;

        /** The time passed since the last update. */
        public float delta;

        private BitSet changed;
        private BitSet deleted;

        public World():this(new WorldConfiguration())
        {

        }

        /// <summary>
        /// Creates a new world.
        /// </summary>
        /// <param name="configuration"></param>
        public World(WorldConfiguration configuration)
        {

            Systems = new Dictionary<Type, BaseSystem>();
            systemsBag = configuration.Systems;

            changed = new BitSet();
            deleted = new BitSet();

            this.ComponentManager = (ComponentManager)configuration.Systems[WorldConfiguration.COMPONENT_MANAGER_IDX];
            this.EntityManager = (EntityManager)configuration.Systems[WorldConfiguration.ENTITY_MANAGER_IDX];
            this.AspectSubscriptionManager = (AspectSubscriptionManager)configuration.Systems[WorldConfiguration.ASPECT_SUBSCRIPTION_MANAGER_IDX];

            this.ComponentManager = this.ComponentManager == null ? new ComponentManager(configuration.ExpectedEntityCount) : this.ComponentManager;
            this.EntityManager = this.EntityManager == null ? new EntityManager(configuration.ExpectedEntityCount) : this.EntityManager;
            this.AspectSubscriptionManager = this.AspectSubscriptionManager == null ? new AspectSubscriptionManager() : this.AspectSubscriptionManager;
            this.EditPool = new EntityEditPool(this.EntityManager);

            injector = configuration.Injector;
            if (injector == null)
            {
                injector = new CachedInjector();
            }

            configuration.Initialize(this, injector, this.AspectSubscriptionManager);

            if (InvocationStrategy == null)
            {
                InvocationStrategy = new InvocationStrategy();
            }
        }


        internal Entity GetEntity(int entityId)
        {
            throw new NotImplementedException();
        }

  

        internal void Delete(int id)
        {
            throw new NotImplementedException();
        }
    }
}
