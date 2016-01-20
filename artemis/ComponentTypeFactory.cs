using Artemis.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Artemis
{
    /**
     * Tracks all component types in a single world.
     * @see ComponentType
     */
    public class ComponentTypeFactory
    {
        private readonly Dictionary<Type, ComponentType> componentTypes = new Dictionary<Type, ComponentType>();

        /** Amount of generated component types. */
        private int componentTypeCount = 0;
        /** Index of this component type in componentTypes. */
        private readonly Bag<ComponentType> types = new Bag<ComponentType>();

        public ComponentType GetTypeFor(Type component)
        {
            ComponentType result;
            if (!componentTypes.TryGetValue(component, out result))
            {
                int index = componentTypeCount++;
                result = new ComponentType(component, index);
                componentTypes.Add(component, result);
                types[index] = result;
            }

            return result;
        }

        /**
         * Gets component type by index.
         * <p>
         * @param index maps to {@link ComponentType}
         * @return the component's {@link ComponentType}
         */
        public ComponentType GetTypeFor(int index)
        {
            return types[index];
        }

        /**
         * Get the index of the component type of given component class.
         *
         * @param c
         *			the component class to get the type index for
         *
         * @return the component type's index
         */
        public int GetIndexFor(Type c)
        {
            return GetTypeFor(c).Index;
        }

        protected TaxonomyType GetTaxonomy(int index)
        {
            return types[index].Taxonomy;
        }

        protected bool IsPackedComponent(int index)
        {
            return types[index].IsPackedComponent();
        }


    }
}
