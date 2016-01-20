

namespace Artemis
{
    public sealed class Archetype
    {
        private readonly ComponentType<IComponent>[] types;
	    public int CompositionId { get; private set; }

        /**
         * @param types Desired composition of derived components.
         * @param compositionId uniquely identifies component composition.
         */
        public Archetype(ComponentType<IComponent>[] types, int compositionId)
        {
            this.types = types;
            this.CompositionId = compositionId;
        }
    }
}