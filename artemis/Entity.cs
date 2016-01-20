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

        internal EntityEdit Edit()
        {
            throw new NotImplementedException();
        }
    }
}
