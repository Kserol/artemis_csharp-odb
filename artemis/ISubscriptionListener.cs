using System;
using Artemis.Utils;

namespace Artemis
{
    public interface ISubscriptionListener
    {
        void Removed(Bag<int> removed);

         void Inserted(Bag<int> inserted);

    }
}