using System;
using Artemis.Utils;

namespace Artemis
{
    public class EntityManager:BaseSystem,ISubscriptionListener
    {
        private readonly Bag<Entity> entities;
        private RecyclingEntityFactory recyclingEntityFactory;
        private Bag<int> entityToIdentity = new Bag<int>();
        private int highestSeenIdentity;
        private ComponentIdentityResolver identityResolver = new ComponentIdentityResolver();

        public EntityManager(int initialContainerSize)
        {
            entities = new Bag<Entity>(initialContainerSize);
        }
        
        protected override void ProcessSystem() { }
        
        public override void Initialize()
        {
            recyclingEntityFactory = new RecyclingEntityFactory(this);
            
		        World.AspectSubscriptionManager
				        .Get(Aspect.All())
				        .AddSubscriptionListener(this);           
        }

        internal  Bag<Entity> Entities
        {
            get { return this.entities;  }
        }
           

        /// <summary>
        /// Create new entity
        /// </summary>
        /// <returns>Entity</returns>
        protected Entity CreateEntityInstance()
        {
            Entity e = this.recyclingEntityFactory.Obtain();
            entityToIdentity[e.Id] =  0;
            return e;
        }

        /// <summary>
        /// Create new entity
        /// </summary>
        /// <returns>Id</returns>
        protected int Create()
        {
            int id = recyclingEntityFactory.Obtain().Id;
            entityToIdentity[id] = 0;
            return id;
        }

        protected int Create(Archetype archetype)
        {
            int id = recyclingEntityFactory.Obtain().Id;
            entityToIdentity[id] = archetype.CompositionId;
            return id;
        }

        protected Entity CreateEntityInstance(Archetype archetype)
        {
            Entity e = CreateEntityInstance();
            entityToIdentity[e.Id] = archetype.CompositionId;
            return e;
        }

        public BitSet ComponentBits(int entityId)
        {
            int identityIndex = entityToIdentity[entityId];
            if (identityIndex == 0)
                identityIndex = ForceResolveIdentity(entityId);

            return identityResolver.Composition[identityIndex];
        }

        /// <summary>
        /// Refresh entity composition identity if it changed.
        /// </summary>
        /// <param name="edit"></param>
        private void UpdateCompositionIdentity(EntityEdit edit)
        {
            int identity = CompositionIdentity(edit.ComponentBits);
            entityToIdentity[edit.EntityId] = identity;
        }

        /// <summary>
        /// Fetches unique identifier for composition.
        /// </summary>
        /// <param name="componentBits">composition to fetch unique identifier for</param>
        /// <returns>Unique identifier for passed composition</returns>
        private int CompositionIdentity(BitSet componentBits)
        {
            int identity = identityResolver.GetIdentity(componentBits);
            if (identity > highestSeenIdentity)
            {
                World.AspectSubscriptionManager.ProcessComponentIdentity(identity, componentBits);
                highestSeenIdentity = identity;
            }
            return identity;
        }

        void Deleted(Bag<int> entities)
        {

            foreach (int i in entities)
            {
                int entityId = i;
                // usually never happens but:
                // this happens when an entity is deleted before
                // it is added to the world, ie; created and deleted
                // before World#process has been called
                if (!this.recyclingEntityFactory.Has(entityId))
                {
                    this.recyclingEntityFactory.Free(entityId);
                }
            }
        }

        public bool IsActive(int entityId)
        {
            return !recyclingEntityFactory.Has(entityId);
        }

        protected Entity GetEntity(int entityId)
        {
            return entities[entityId];
        }

        public int GetIdentity(int entityId)
        {
            int identity = entityToIdentity[entityId];
            if (identity == 0)
                identity = ForceResolveIdentity(entityId);

            return identity;
        }

        public void SetIdentity(int entityId, int compositionId)
        {
            entityToIdentity[entityId] = compositionId;
        }

        private int ForceResolveIdentity(int entityId)
        {
            UpdateCompositionIdentity(entities[entityId].Edit());
            return entityToIdentity[entityId];
        }

        public void Synchronize(EntitySubscription es)
        {
            for (int i = 1; highestSeenIdentity >= i; i++)
            {
                BitSet componentBits = identityResolver.Composition[i];
                es.ProcessComponentIdentity(i, componentBits);
            }

            for (int i = 0; i < entities.Size; i++)
            {
                Entity e = entities[i];
                if (e != null && this.IsActive(i))
                    es.Check(e.Id);
            }

            es.InformEntityChanges();
            es.RebuildCompressedActives();
        }

        public Entity CreateEntity(int id)
        {
            return new Entity(World, id);
        }

        public void Removed(Bag<int> removed)
        {
            this.Deleted(removed);
        }

        public void Inserted(Bag<int> inserted)
        {
            
        }

        internal sealed class ComponentIdentityResolver
        {
            private readonly Bag<BitSet> composition;

            public Bag<BitSet> Composition
            {
                get { return composition; }
            }

            public ComponentIdentityResolver()
            {
                composition = new Bag<BitSet>();
                composition.Add(null);
                composition.Add(new BitSet());
            }

            /** Fetch unique identity for passed composition. */
            public int GetIdentity(BitSet components)
            {
                int i=0;
                foreach(var c in composition)
                { 
                    if (components.Equals(c))
                        return i;
                    i++;
                }

                composition.Add((BitSet)components.Clone());

                return composition.Size;
            }
        }
    }
}