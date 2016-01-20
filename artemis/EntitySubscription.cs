using System;
using Artemis.Utils;

namespace Artemis
{
    /**
 * Maintains the list of entities matched by an aspect. Entity subscriptions
 * are automatically updated during {@link com.artemis.World#process()}.
 * Any {@link com.artemis.EntitySubscription.SubscriptionListener | listeners}
 * are informed when entities are added or removed.
 */
    public class EntitySubscription
    {
        private Aspect aspect;
        private AspectPromise promise;
        private BitSet aspectCache;

        private Bag<int> entities;
        private BitSet activeEntityIds;
        private EntityManager em;

        private readonly Bag<ISubscriptionListener> listeners;

        private readonly BitSet insertedIds;
	    private readonly BitSet removedIds;

	    private  Bag<int> inserted;
	    private  Bag<int> removed;
	    private bool dirty;

        public EntitySubscription(World world, AspectPromise builder)
        {
            aspect = builder.Build(world);
            promise = builder;
            aspectCache = new BitSet();
            em = world.EntityManager;

            activeEntityIds = new BitSet();
            entities = new Bag<int>();

            listeners = new Bag<ISubscriptionListener>();

            insertedIds = new BitSet();
            removedIds = new BitSet();

            inserted = new Bag<int>();
            removed = new Bag<int>();
        }

        /// <summary>
        ///  Never remove elements from the bag, as this will lead to undefined behavior.
        /// </summary>
        /// <returns></returns>
        public Bag<int> GetEntities()
        {
            if (dirty)
            {
                RebuildCompressedActives();
                dirty = false;
            }
            return entities;
        }

        public BitSet ActiveEntityIds
        {
            get { return this.activeEntityIds; }
        }
        
        public Aspect Aspect
        {
            get { return this.aspect; }
        }

        public AspectPromise AspectPromise
        {
            get { return this.promise; }
        }


        /// <summary>
        ///  A new unique component composition detected, check if this subscription's aspect is interested in it.
        /// </summary>
        /// <param name="i"></param>
        /// <param name="componentBits"></param>
        public void ProcessComponentIdentity(int id, BitSet componentBits)
        {
            aspectCache.Set(id, aspect.IsInterested(componentBits));
        }

        public void RebuildCompressedActives()
        {
            BitSet bs = activeEntityIds;
            int size = bs.Cardinality();
            entities.EnsureCapacity(size);
            int[] activesArray = entities.data;
            for (int i = bs.NextSetBit(0), index = 0; i >= 0; i = bs.NextSetBit(i + 1))
            {
                activesArray[index++] = i;
            }
        }

        public void Check(int id)
        {
            bool interested = aspectCache.Get(em.GetIdentity(id));
            bool contains = activeEntityIds.Get(id);

            if (interested && !contains)
            {
                Insert(id);
            }
            else if (!interested && contains)
            {
                Remove(id);
            }
        }

        private void Remove(int entityId)
        {
            activeEntityIds.Clear(entityId);
            removedIds.Set(entityId);
        }

        private void Insert(int entityId)
        {
            activeEntityIds.Set(entityId);
            insertedIds.Set(entityId);
        }
        
        public void Process(Bag<int> changed, Bag<int> deleted)
        {
            this.Deleted(deleted);
            this.Changed(changed);

            dirty |= InformEntityChanges();
        }

        public bool InformEntityChanges()
        {
            if (insertedIds.IsEmpty() && removedIds.IsEmpty())
                return false;

            TransferBitsToInts();
            for (int i = 0, s = listeners.Size; s > i; i++)
            {
                if (removed.Size > 0)
                    listeners[i].Removed(removed);

                if (inserted.Size > 0)
                    listeners[i].Inserted(inserted);
            }

            inserted.Clear();
            removed.Clear();

            return true;
        }

        private void TransferBitsToInts()
        {
            inserted = insertedIds.ToIntBag();
            removed = removedIds.ToIntBag();
            insertedIds.Clear();
            removedIds.Clear();
        }

        private  void Changed(Bag<int> entities)
        {
            foreach(var e in entities)
            {
                Check(e);
            }
        }

        private void Deleted(Bag<int> entities)
        {
            foreach (var e in entities)
            {
                Deleted(e);
            }
        }

        private void Deleted(int entityId)
        {
            if (activeEntityIds.Get(entityId))
                Remove(entityId);
        }
        public void AddSubscriptionListener(ISubscriptionListener listener)
        {
            listeners.Add(listener);
        }
        
    }
}