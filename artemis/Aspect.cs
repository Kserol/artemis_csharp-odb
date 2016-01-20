using Artemis.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Artemis
{
    /// <summary>
    /// An Aspect is used by systems as a matcher against entities, to check if a
    /// system is interested in an entity.
    /// Aspects define what sort of component types an entity must possess, or not
    /// possess.
    /// </summary>
    public class Aspect
    {
        /** Component bits the entity must all possess. */
        internal BitSet allSet;
        /** Component bits the entity must not possess. */
        internal BitSet exclusionSet;
        /** Component bits of which the entity must possess at least one. */
        internal BitSet oneSet;

        public Aspect()
        {
            this.allSet = new BitSet();
            this.exclusionSet = new BitSet();
            this.oneSet = new BitSet();
        }


        public BitSet AllSet
        {
            get { return allSet; }      
            internal set { allSet = value; }    
        }

        public BitSet ExclusionSet
        {
            get { return exclusionSet; }
            internal set { exclusionSet = value; }
        }

        public BitSet OneSet
        {
            get { return oneSet; }
            internal set { OneSet = value; }
        }

        public bool IsInterested(Entity e)
        {
            return IsInterested(e.ComponentBits);
        }

        /**
         * Returns whether this Aspect would accept the given set.
         */
        public bool IsInterested(BitSet componentBits)
        {
            // Check if the entity possesses ALL of the components defined in the aspect.
            if (!allSet.IsEmpty())
            {
                for (int i = allSet.NextSetBit(0); i >= 0; i = allSet.NextSetBit(i + 1))
                {
                    if (!componentBits.Get(i))
                    {
                        return false;
                    }
                }
            }

            // If we are STILL interested,
            // Check if the entity possesses ANY of the exclusion components, if it does then the system is not interested.
            if (!exclusionSet.IsEmpty() && exclusionSet.Intersects(componentBits))
            {
                return false;
            }

            // If we are STILL interested,
            // Check if the entity possesses ANY of the components in the oneSet. If so, the system is interested.
            if (!oneSet.IsEmpty() && !oneSet.Intersects(componentBits))
            {
                return false;
            }

            return true;
        }


        /// <summary>Returns an Empty Aspect (does not filter anything - i.e. rejects everything).</summary>
        /// <returns>The Aspect.</returns>
        public static Aspect Empty()
        {
            return new Aspect();
        }

        /// <summary>Excludes the specified types.</summary>
        /// <param name="types">The types.</param>
        /// <returns>The specified Aspect.</returns>
        public static AspectPromise Exclude(params Type[] types)
        {
            return (new AspectPromise()).Exclude(types);
        }

        /// <summary>Ones the specified types.</summary>
        /// <param name="types">The types.</param>
        /// <returns>The specified Aspect.</returns>
        public static AspectPromise One(params Type[] types)
        {
            return (new AspectPromise()).One(types);
        }

        public static AspectPromise All(params Type[] types)
        {
            return (new AspectPromise()).All(types);
        }

        /// <summary>Gets all.</summary>
        /// <param name="types">The types.</param>
        /// <returns>The specified Aspect.</returns>
        public Aspect GetAll(params Type[] types)
        {
            return this;
        }


    }


}
