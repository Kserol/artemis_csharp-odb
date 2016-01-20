using Artemis.Utils;
using System;

namespace Artemis
{
    public sealed class EntityEdit 
    {
        private int entityId;
        private ComponentManager cm;
        private readonly BitSet componentBits;

        public BitSet ComponentBits
        {
            get
            {
                return componentBits;
            }
        }

        public EntityEdit(World world)
        {
            cm = world.ComponentManager;
            componentBits = new BitSet();
        }

        /// <summary>
        /// Create new component instance
        /// if exist, replace and retires old component
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T Create<T>() where T :IComponent,new()
        {
            T component = cm.Create<T>(this.entityId);
            ComponentType componentType = cm.TypeFactory.GetTypeFor(typeof(T));
            this.componentBits.Set(componentType.Index);

            return component;
        }

        /// <summary>
        /// Add a component
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="component"></param>
        /// <returns></returns>
        public EntityEdit Add<T>(T component) where T:IComponent
        {
            return Add(component, cm.TypeFactory.GetTypeFor(typeof(T)));
        }

        /// <summary>
        /// Faster add component
        /// </summary>
        /// <param name="component"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public EntityEdit Add(IComponent component, ComponentType type)
        {
            /*if (type.Taxonomy != TaxonomyType.BASIC)
            {
                throw new InvalidOperationException("Use EntityEdit.Create<T>() for adding non-basic component types");
            }*/

            cm.AddComponent(entityId, type, component);
            this.componentBits.Set(type.Index);

            return this;
        }

        /// <summary>
        /// Get the entity
        /// </summary>
        /// <returns>Entity this EntityEdit work on</returns>
        public Entity Entity
        {
            get
            {
                return cm.World.GetEntity(entityId);
            }
        }

        public int EntityId
        {
            get
            {
                return entityId;
            }
        }

        public EntityEdit Remove<T>()
        {
            return Remove(typeof(T));
        }

        public EntityEdit Remove(ComponentType type)
        {
            if (this.componentBits.Get(type.Index))
            {
                cm.RemoveComponent(entityId, type);
                this.componentBits.Clear(type.Index);
            }
            return this;
        }

        public EntityEdit Remove(Type type)
        {
            return Remove(cm.TypeFactory.GetTypeFor(type));
        }

    }
}