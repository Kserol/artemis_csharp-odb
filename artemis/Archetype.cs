

namespace Artemis
{
    public sealed class Archetype
    {
        private readonly ComponentType[] types;
	    public int CompositionId { get; private set; }

        public ComponentType[] Types
        {
            get
            {
                return types;
            }
        }

        /**
         * @param types Desired composition of derived components.
         * @param compositionId uniquely identifies component composition.
         */
        public Archetype(ComponentType[] types, int compositionId)
        {
            this.types = types;
            this.CompositionId = compositionId;
        }
    }
}