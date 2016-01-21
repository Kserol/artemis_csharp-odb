using Artemis.Blackboard;
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

        public BlackBoard BlackBoard{get;set;}

        internal Dictionary<Type, BaseSystem> Systems { get; set; }

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
            // changed can be populated by EntityTransmuters and Archetypes,
            // bypassing the editPool.
            while (!changed.IsEmpty() || EditPool.ProcessEntities())
                AspectSubscriptionManager.Process(changed, deleted);

            ComponentManager.Clean();
        }

        private Bag<BaseSystem> systemsBag;

        private SystemInvocationStrategy invocationStrategy;

        private IInjector injector;

        /** The time passed since the last update. */
        public float delta;

        // Access by Entity pool REFACT
        internal BitSet changed;
        internal BitSet deleted;

        public World():this(new WorldConfiguration())
        {

        }

        /// <summary>
        /// Creates a new world.
        /// </summary>
        /// <param name="configuration"></param>
        public World(WorldConfiguration configuration)
        {
            BlackBoard = new BlackBoard();
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
            InvocationStrategy.World = this;
        }

        public T CreateFactory<T>() where T:IEntityFactory
        {
            Type tType = typeof(T);
            return (T)Activator.CreateInstance(tType);
	    }

        public EntityEdit Edit(int entityId)
        {
            return EditPool.ObtainEditor(entityId);
        }

        public T GetSystem<T>() where T :BaseSystem
        {
            return (T)this.Systems[typeof(T)];
        }

        public long Delta
        {
            get; set;
        }

        public void DeleteEntity(Entity e)
        {
            Delete(e.Id);
        }

        /**
         * Delete the entity from the world.
         *
         * The entity is considered to be in a final state once invoked;
         * adding or removing components from an entity scheduled for
         * deletion will likely throw exceptions.
         *
         * @param entityId
         * 		the entity to delete
         */
        public void Delete(int entityId)
        {
            EditPool.Delete(entityId);
        }

        /**
         * Create and return a new or reused entity instance. Entity is
         * automatically added to the world.
         *
         * @return entity
         * @see #create() recommended alternative.
         */
        public Entity CreateEntity()
        {
            Entity e = this.EntityManager.CreateEntityInstance();
            e.Edit();
            return e;
        }

        /**
         * Create and return a new or reused entity id. Entity is
         * automatically added to the world.
         *
         * @return assigned entity id, where id >= 0.
         */
        public int Create()
        {
            int entityId = this.EntityManager.Create();
            Edit(entityId);
            return entityId;
        }

        /**
         * Create and return an {@link Entity} wrapping a new or reused entity instance.
         * Entity is automatically added to the world.
         *
         * Use {@link Entity#edit()} to set up your newly created entity.
         *
         * You can also create entities using:
         * - {@link com.artemis.utils.EntityBuilder} Convenient entity creation. Not useful when pooling.
         * - {@link com.artemis.Archetype} Fastest, low level, no parameterized components.
         * - {@link com.artemis.EntityFactory} Fast, clean and convenient. For fixed composition entities. Requires some setup.
         * Best choice for parameterizing pooled components.
         *
         * @see #create(int) recommended alternative.
         * @return entity
         */
        public Entity CreateEntity(Archetype archetype)
        {
            Entity e = this.EntityManager.CreateEntityInstance(archetype);
            this.ComponentManager.AddComponents(e.Id, archetype);
            changed.Set(e.Id);
            return e;
        }

        public int Create(Archetype archetype)
        {
            int entityId = this.EntityManager.Create(archetype);
            this.ComponentManager.AddComponents(entityId, archetype);
            changed.Set(entityId);
            return entityId;
        }

        internal Entity GetEntity(int entityId)
        {
            return  this.EntityManager.GetEntity(entityId);
        }


        public void Process()
        {
            UpdateEntityStates();
            invocationStrategy.Process(systemsBag);
        }
    }
}
