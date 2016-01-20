using System;
using System.Collections.Generic;

namespace Artemis
{
    public class CachedInjector:IInjector
    {
        public CachedInjector()
        {
        }

        public void Initialize(World world, Dictionary<string, object> injectables)
        {
        }

        public void Inject(object target)
        {            
        }

        public bool IsInjectable(object target)
        {
            return true; 
        }
    }
}