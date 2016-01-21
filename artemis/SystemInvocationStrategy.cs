using Artemis.Utils;

namespace Artemis
{
    public abstract class SystemInvocationStrategy
    {

        /** World to operate on. */
        protected World world;

        /** World to operate on. */
        public World World
        {
            get
            {
                 return world;
            }
            set
            {
                this.world = value;
            }
        }

        /** Called during world initialization phase. */
        protected virtual void Initialize() { }

        /** Call to inform all systems and subscription of world state changes. */
        protected virtual void UpdateEntityStates()
        {
            world.UpdateEntityStates();
        }

        /** Process all systems. */
        public abstract void Process(Bag<BaseSystem> systems);
    }
}