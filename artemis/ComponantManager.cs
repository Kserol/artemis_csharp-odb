using System;

namespace Artemis
{
    //TODO
    public class ComponantManager : BaseSystem
    {

        public World World { get; private set; }

        public ComponentTypeFactory TypeFactory { get; internal set; }

        protected override void ProcessSystem()
        {
            throw new NotImplementedException();
        }

        internal T Create<T>(int entityId)
        {
            throw new NotImplementedException();
        }

        internal void AddComponent(int entityId, ComponentType type, IComponent component)
        {
            throw new NotImplementedException();
        }

        internal void RemoveComponent(int entityId, ComponentType type)
        {
            throw new NotImplementedException();
        }
    }
}