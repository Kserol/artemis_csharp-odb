using Artemis.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Artemis
{
    public sealed class Entity
    {
        /// <summary>
        /// Entity identifier in the world
        /// </summary>
        private int id;

        /// <summary>
        /// The world this belong to
        /// </summary>
        private readonly World world;

        internal Entity(World world,int id)
        {
            this.world = world;
            this.id = id;
        }

        /// <summary>
        /// The internal id for this entity within the framework. Id is zero or greater.
        /// No other entity will have the same ID, but ID's are however reused so  another entity may acquire this ID if the previous entity was deleted.
        /// </summary>
        public int Id
        {
            get
            {
                return id;
            }
        }

        public BitSet ComponentBits
        {
            get { return world.EntityManager.ComponentBits(id); }
            
        }

        /// <summary>
        /// Get Entity editor
        /// </summary>
        /// <returns></returns>
        public EntityEdit Edit()
        {
            return world.EditPool.ObtainEditor(id);
        }

        /// <summary>
        /// Checks if the entity has been added to the world and has not been deleted from it.
        /// </summary>
        /// <returns></returns>
        public bool IsActive()
        {
            return world.EntityManager.IsActive(id);
        }


        public override string ToString()
        {
            return "Entity[" + id + "]";
        }

        /// <summary>
        /// <para>Gets the component.</para>
        /// <para>This is the preferred method to use when</para>
        /// <para>retrieving a component from a entity.</para>
        /// <para>It will provide good performance.</para>
        /// </summary>
        /// <typeparam name="T">the expected return component type.</typeparam>
        /// <returns>component that matches, or null if none is found.</returns>
        public T GetComponent<T>() where T : IComponent
        {
            var ct = this.world.ComponentManager.TypeFactory.GetTypeFor(typeof(T));
            return (T)GetComponent(ct);
        }

        public IComponent GetComponent(ComponentType type)
        {
            return world.ComponentManager.GetComponent(id, type);
        }


        public void DeleteFromWorld()
        {
            world.Delete(id);
        }

        public World World { get { return world; } }

        public int CompositionId
        {
            get
            {
                return world.EntityManager.GetIdentity(id);
            }
        }

        public override bool Equals(object o)
        {
            if (this == o) return true;
            if (o == null || this.GetType() != o.GetType()) return false;

            Entity entity = (Entity)o;

            return id == entity.id;
        }

        public override int GetHashCode()
        {
            return id;
        }

    }
}
