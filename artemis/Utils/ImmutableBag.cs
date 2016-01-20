using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Artemis.Utils
{
    public interface ImmutableBag<T> : IEnumerable<T>
    {
        /// <summary>
        /// Returns the element at the specified position in Bag
        /// </summary>
        /// <param name="index">Index of the Element</param>
        /// <returns>Element at thespecified position</returns>
        T this[int index] { get; }

        /// <summary>
        /// Get the size
        /// </summary>
        int Size { get; }

        /// <summary>
        /// True if Bag is empty
        /// </summary>
        bool IsEmpty { get; }
    }
}
