using System;

namespace Artemis
{
    public enum TaxonomyType
    {
        BASIC, POOLED, PACKED
    }
    public class ComponentType 
    {


        /** The class type of the component type. */
        private readonly Type type;
        private readonly TaxonomyType taxonomy;

        private bool packedHasWorldConstructor = false;

        /** Ordinal for fast lookups. */
        private readonly int index;

        public ComponentType(Type type, int index)
        {

            this.index = index;
            this.type = type;
            /* TODO  if (ClassReflection.isAssignableFrom(PackedComponent.class, type)) {
               taxonomy = Taxonomy.PACKED;
               packedHasWorldConstructor = hasWorldConstructor(type);
       } else if (ClassReflection.isAssignableFrom(PooledComponent.class, type)) {
               taxonomy = Taxonomy.POOLED;
           } else {
               taxonomy = Taxonomy.BASIC;
           }*/
        }
        /*
            private static bool HasWorldConstructor(T type)
        {
            Constructor[] constructors = ClassReflection.getConstructors(type);
            for (int i = 0; constructors.length > i; i++)
            {
                    Class[] types = constructors[i].getParameterTypes();
                if (types.length == 1 && types[0] == World.class)
                        return true;
                }

                return false;
            }*/

        /**
         * Get the component type's index.
         *
         * Index is distinct for each {@link World} instance,
         * allowing for fast lookups.
         *
         * @return the component types index
         */
        public int Index
        {
            get
            {
                return index;
            }
        }

        public TaxonomyType Taxonomy
        {
            get
            {
                return taxonomy;
            }
        }

        /**
         * @return {@code true} if of taxonomy packed.
         */
        public bool IsPackedComponent()
        {
            return taxonomy == TaxonomyType.PACKED;
        }

        /**
         * @return {@code Class} that this type represents.
         */
        public Type Type
        {
            get
            {
                return type;
            }
        }

        public override string ToString()
        {
            //TODO return "ComponentType[" + ClassReflection.getSimpleName(type) + "] (" + index + ")";
            return base.ToString();
        }
    }
}