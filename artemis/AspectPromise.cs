using Artemis.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Artemis
{
    public class AspectPromise
    {
        private readonly Bag<Type> allTypes;
        private readonly Bag<Type> exclusionTypes;
        private readonly Bag<Type> oneTypes;

        public AspectPromise()
        {
            allTypes = new Bag<Type>();
            exclusionTypes = new Bag<Type>();
            oneTypes = new Bag<Type>();
        }

        public AspectPromise All(params Type[] types)
        {
            foreach (Type t in types)
            {
                allTypes.Add(t);
            }
            return this;
        }

        public AspectPromise Copy()
        {
            AspectPromise b = new AspectPromise();
            b.allTypes.AddRange(allTypes);
            b.exclusionTypes.AddRange(exclusionTypes);
            b.oneTypes.AddRange(oneTypes);
            return b;
        }


        public AspectPromise One(params Type[] types)
        {
            foreach (Type t in types)
            {
                oneTypes.Add(t);
            }
            return this;
        }

        public AspectPromise Exclude(params Type[] types)
        {
            foreach (Type t in types)
            {
                exclusionTypes.Add(t);
            }
            return this;
        }

        public Aspect Build (World w)
        {
            ComponentTypeFactory tf = w.ComponentManager.TypeFactory;
            Aspect aspect = new Aspect();
            this.Associate(tf, allTypes, aspect.AllSet);
            this.Associate(tf, exclusionTypes, aspect.ExclusionSet);
            this.Associate(tf, oneTypes, aspect.OneSet);

            return aspect;
        }

        public void Associate(ComponentTypeFactory tf, Bag<Type> types, BitSet componentBits)
        {
            foreach (var t in types)
            {
                componentBits.Set(tf.GetIndexFor(t));
            }
        }

        
        public override bool Equals(Object o)
        {
            if (this == o) return true;
            if (o == null || o.GetType() != o.GetType()) return false;

            AspectPromise builder = (AspectPromise)o;

            if (!allTypes.Equals(builder.allTypes))
                return false;
            if (!exclusionTypes.Equals(builder.exclusionTypes))
                return false;
            if (!oneTypes.Equals(builder.oneTypes))
                return false;

            return true;
        }

        public override int GetHashCode()
        {
            int result = allTypes.GetHashCode();
            result = 31 * result + exclusionTypes.GetHashCode();
            result = 31 * result + oneTypes.GetHashCode();
            return result;
        }
    }
}
