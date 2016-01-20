using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Artemis.Utils
{
    public class Bag<T> : ImmutableBag<T>
    {
        /// <summary>The elements.</summary>
        private T[] data;
        protected int size = 0;

        //Construct an empty bag, default capacity of 64
        public Bag(int capacity = 64)
        {
            this.data = new T[capacity];
        }


        public bool IsEmpty
        {
            get
            {
                return this.Size == 0;
            }
        }

        public int Size
        {
            get
            {
                return size;
            }
        }

        public int Capacity
        {
            get
            {
                return data.Length;
            }
        }

        /// <summary>Grows this instance.</summary>
        private void Grow()
        {
            int newCapacity = (data.Length * 7) / 4 + 1;
            Grow(newCapacity);
        }

        /// <summary>Grows the specified new capacity.</summary>
        /// <param name="newCapacity">The new capacity.</param>
        private void Grow(int newCapacity)
        {
            T[] oldElements = this.data;
            this.data = new T[newCapacity];
            Array.Copy(oldElements, 0, this.data, 0, data.Length);
        }

        /// <summary>Returns the element at the specified position in Bag.</summary>
        /// <param name="index">The index.</param>
        /// <returns>The element from the specified position in Bag.</returns>
        public T this[int index]
        {
            get
            {
                return this.data[index];
            }

            set
            {
                if (index >= this.data.Length)
                {
                    this.Grow(index * 2);
                    this.size = index + 1;
                }
                else if (index >= this.size)
                {
                    this.size = index + 1;
                }

                this.data[index] = value;
            }
        }

        /// <summary>Removes the specified index.</summary>
        /// <param name="index">The index.</param>
        /// <returns>The removed element.</returns>
        public T Remove(int index)
        {
            // Make copy of element to remove so it can be returned.
            T result = this.data[index];

            // Overwrite item to remove with last element.
            this.data[index] = this.data[--this.size];

            // Null last element, so garbage collector can do its work.
            this.data[this.size] = default(T);
            return result;
        }

        /// <summary>Removes the last.</summary>
        /// <returns>The last element.</returns>
        public T RemoveLast()
        {
            if (this.Size > 0)
            {
                
                T result = this.data[--this.size];

                // default(T) if class = null.
                this.data[this.size] = default(T);
                return result;
            }

            return default(T);
        }

        /// <summary>
        /// <para>Removes the first occurrence of the specified element from this Bag, if it is present.</para>
        /// <para>If the Bag does not contain the element, it is unchanged.</para>
        /// <para>Does this by overwriting it was last element then removing last element.</para>
        /// </summary>
        /// <param name="element">The element to be removed from this list, if present.</param>
        /// <returns><see langword="true"/> if this list contained the specified element, otherwise <see langword="false"/>.</returns>
        public bool Remove(T element)
        {
            for (int index = this.Size - 1; index >= 0; --index)
            {
                if (element.Equals(this.data[index]))
                {
                    // Overwrite item to remove with last element.
                    this.data[index] = this.data[--this.size];
                    this.data[this.size] = default(T);

                    return true;
                }
            }

            return false;
        }

        /// <summary>Removes all matching elements.</summary>
        /// <param name="bag">The bag.</param>
        /// <returns><see langword="true" /> if found matching elements, <see langword="false" /> otherwise.</returns>
        public bool RemoveAll(ImmutableBag<T> bag)
        {
            bool isResult = false;
            for (int index = bag.Size - 1; index >= 0; --index)
            {
                if (this.Remove(bag[index]))
                {
                    isResult = true;
                }
            }

            return isResult;
        }

        /// <summary>
        /// Adds the specified element to the end of this bag.
        /// If needed also increases the capacity of the bag.
        /// </summary>
        /// <param name="element">The element to be added to this list.</param>
        public void Add(T element)
        {
            // is size greater than capacity increase capacity
            if (this.Size >= this.data.Length)
            {
                this.Grow();
            }

            this.data[this.size] = element;
            this.size++;
        }

        /// <summary>Adds a range of elements into this bag.</summary>
        /// <param name="rangeOfElements">The elements to add.</param>
        public void AddRange(Bag<T> rangeOfElements)
        {
            for (int index = 0, j = rangeOfElements.Size; j > index; ++index)
            {
                this.Add(rangeOfElements[index]);
            }
        }


        /// <summary>Determines whether bag contains the specified element.</summary>
        /// <param name="element">The element.</param>
        /// <returns><see langword="true"/> if bag contains the specified element; otherwise, <see langword="false"/>.</returns>
        public bool Contains(T element)
        {
            for (int index = this.size - 1; index >= 0; --index)
            {
                if (element.Equals(this.data[index]))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Checks if the internal storage supports this index.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public bool IsIndexWithinBounds(int index)
        {
            return index < this.Capacity;
        }

        /// <summary>
        /// Check if an item, if added at the given item will fit into the bag.
	    /// If not, the bag capacity will be increased to hold an item at the index.
        /// </summary>
        /// <param name="index"></param>
        public void EnsureCapacity(int index)
        {
            if (index >= data.Length)
            {
                Grow(index);
            }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Bag(");
            for (int i = 0; size > i; i++)
            {
                if (i > 0) sb.Append(", ");
                sb.Append(data[i]);
            }
            sb.Append(')');
            return sb.ToString();
        }

        public override int GetHashCode()
        {
            int hash = 0;
            for (int i = 0, s = size; s > i; i++)
            {
                hash = (127 * hash) + data[i].GetHashCode();
            }

            return hash;
        }

        /// <summary>Returns an enumerator that iterates through a collection.</summary>
        /// <returns>An <see cref="T:System.Collections.Generic.IEnumerator`1" /> object that can be used to iterate through the collection.</returns>
        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return new BagEnumerator<T>(this);
        }

        /// <summary>Returns an enumerator that iterates through a collection.</summary>
        /// <returns>An <see cref="T:System.Collections.Generic.IEnumerator`1" /> object that can be used to iterate through the collection.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return new BagEnumerator<T>(this);
        }

    }
}
