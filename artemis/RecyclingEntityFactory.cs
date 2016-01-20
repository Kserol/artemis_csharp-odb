using Artemis.Utils;
using System.Collections.Generic;

namespace Artemis
{
    internal class RecyclingEntityFactory
    {
        private readonly EntityManager em;
		private readonly Queue<int> limbo;
		private readonly BitSet recycled;
		private int nextId;

        public RecyclingEntityFactory(EntityManager em)
        {
            this.em = em;
            recycled = new BitSet();
            limbo = new Queue<int>(64);
        }

        public void Free(int entityId)
        {
            limbo.Enqueue(entityId);
            recycled.Set(entityId);
        }

        public Entity Obtain()
        {
            if (limbo.Count == 0)
            {
                Entity e = em.CreateEntity(nextId++);
                em.Entities[e.Id] = e;
                return e;
            }
            else {
                int id = limbo.Dequeue();
                recycled.Set(id, false);
                return em.Entities[id];
            }
        }

        public bool Has(int entityId)
        {
            return recycled.Get(entityId);
        }
    }
}