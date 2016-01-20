using System;

namespace Artemis
{
    public class EntityEditPool
    {
        private EntityManager entityManager;

        public EntityEditPool(EntityManager entityManager)
        {
            this.entityManager = entityManager;
        }

        internal EntityEdit ObtainEditor(int id)
        {
            throw new NotImplementedException();
        }
    }
}