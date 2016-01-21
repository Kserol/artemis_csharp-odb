using System;
using Artemis.Utils;

namespace Artemis
{
    public interface ISubscriptionListener
    {
        void OnRemoved(Bag<int> removed);

         void OnInserted(Bag<int> inserted);

    }
}