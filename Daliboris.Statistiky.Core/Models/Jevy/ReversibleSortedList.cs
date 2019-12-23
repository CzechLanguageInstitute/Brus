using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.ComponentModel;
using System.Collections;

namespace Daliboris.Statistiky
{
    [Serializable, ComVisible(false), DebuggerDisplay("Count = {Count}")]
    public class ReversibleSortedList<TKey, TValue> :
            IDictionary<TKey, TValue>, ICollection<KeyValuePair<TKey, TValue>>,
            IEnumerable<KeyValuePair<TKey, TValue>>,
            IDictionary, ICollection, IEnumerable
    {
        #region SortDirectionComparer class definition
        public class SortDirectionComparer<T> : IComparer<T>
        {
            private System.ComponentModel.ListSortDirection _sortDir;

            public SortDirectionComparer()
            {
                _sortDir = ListSortDirection.Ascending;
            }

            public SortDirectionComparer(ListSortDirection sortDir)
            {
                _sortDir = sortDir;
            }

            public System.ComponentModel.ListSortDirection SortDirection
            {
                get { return _sortDir; }
                set { _sortDir = value; }
            }

            public int Compare(T lhs, T rhs)
            {
                int compareResult =
                    lhs.ToString().CompareTo(rhs.ToString());

                // If order is DESC, reverse this comparison.
                if (SortDirection == ListSortDirection.Descending)
                    compareResult *= -1;
                return compareResult;
            }
        }
        #endregion // SortDirectionComparer

        #region CTORS
        static ReversibleSortedList()
        {
            ReversibleSortedList<TKey, TValue>.emptyKeys = new TKey[0];
            ReversibleSortedList<TKey, TValue>.emptyValues = new TValue[0];
        }

        public ReversibleSortedList()
        {
            this.keys = ReversibleSortedList<TKey, TValue>.emptyKeys;
            this.values = ReversibleSortedList<TKey, TValue>.emptyValues;
            this._size = 0;
            this._sortDirectionComparer = new SortDirectionComparer<TKey>();
            this._currentSortDirection = this._sortDirectionComparer.SortDirection;
        }

        public ReversibleSortedList(SortDirectionComparer<TKey> comparer)
            : this()
        {
            if (comparer != null)
            {
                this._sortDirectionComparer = comparer;
                this._currentSortDirection = _sortDirectionComparer.SortDirection;
            }
        }

        public ReversibleSortedList(IDictionary<TKey, TValue> dictionary)
            : this(dictionary, (SortDirectionComparer<TKey>)null)
        {
        }

        public ReversibleSortedList(int capacity)
        {
            if (capacity < 0)
            {
                throw new ArgumentOutOfRangeException(
                    "capacity", "Non-negative number required");
            }
            this.keys = new TKey[capacity];
            this.values = new TValue[capacity];
            this._sortDirectionComparer = new SortDirectionComparer<TKey>();
            this._currentSortDirection = _sortDirectionComparer.SortDirection;
        }

        public ReversibleSortedList(IDictionary<TKey, TValue> dictionary,
                                    SortDirectionComparer<TKey> comparer)
            : this((dictionary != null) ? dictionary.Count : 0, comparer)
        {
            if (dictionary == null)
            {
                throw new ArgumentNullException("dictionary");
            }
            dictionary.Keys.CopyTo(this.keys, 0);
            dictionary.Values.CopyTo(this.values, 0);
            Array.Sort<TKey, TValue>(this.keys, this.values,
                                            this._sortDirectionComparer);
            this._size = dictionary.Count;
        }

        public ReversibleSortedList(int capacity, SortDirectionComparer<TKey> comparer)
            : this(comparer)
        {
            this.Capacity = capacity;
        }
        #endregion //CTORS

        #region Public Methods
        public void Add(TKey key, TValue value)
        {
            if (key.Equals(null))
            {
                throw new ArgumentNullException("key");
            }
            int num1 = Array.BinarySearch<TKey>(this.keys, 0, this._size, key,
                                                    this._sortDirectionComparer);
            if (num1 >= 0)
            {
                throw new ArgumentException("Attempting to add duplicate");
            }
            this.Insert(~num1, key, value);
        }

        public void Clear()
        {
            this.version++;
            Array.Clear(this.keys, 0, this._size);
            Array.Clear(this.values, 0, this._size);
            this._size = 0;
        }

        public bool ContainsKey(TKey key)
        {
            return (this.IndexOfKey(key) >= 0);
        }

        public bool ContainsValue(TValue value)
        {
            return (this.IndexOfValue(value) >= 0);
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return new ReversibleSortedList<TKey, TValue>.Enumerator<TKey, TValue>(
                        this);
        }
        public int IndexOfKey(TKey key)
        {

            if (key.Equals(null))
            {
                throw new ArgumentNullException("key");
            }
            int num1 = Array.BinarySearch<TKey>(this.keys, 0, this._size, key,
                                                    this._sortDirectionComparer);
            if (num1 < 0)
            {
                return -1;
            }
            return num1;
        }

        public int IndexOfValue(TValue value)
        {
            return Array.IndexOf<TValue>(this.values, value, 0, this._size);
        }

        public bool Remove(TKey key)
        {
            int num1 = this.IndexOfKey(key);
            if (num1 >= 0)
            {
                this.RemoveAt(num1);
            }
            return (num1 >= 0);
        }

        public void RemoveAt(int index)
        {
            if ((index < 0) || (index >= this._size))
            {
                throw new ArgumentOutOfRangeException("index", "Index out of range");
            }
            this._size--;
            if (index < this._size)
            {
                Array.Copy(this.keys, (int)(index + 1), this.keys, index,
                            (int)(this._size - index));
                Array.Copy(this.values, (int)(index + 1), this.values, index,
                            (int)(this._size - index));
            }
            this.keys[this._size] = default(TKey);
            this.values[this._size] = default(TValue);
            this.version++;
        }

        public void Sort()
        {
            // Check if we are already sorted the right way.
            if (this._currentSortDirection !=
                this._sortDirectionComparer.SortDirection)
            {
                // Reverse the arrays as they were already sorted on insert.
                Array.Reverse(this.keys, 0, this._size);
                Array.Reverse(this.values, 0, this._size);
                // Set our current order.
                this._currentSortDirection = this._sortDirectionComparer.SortDirection;
            }
        }

        public void TrimExcess()
        {
            int num1 = (int)(this.keys.Length * 0.9);
            if (this._size < num1)
            {
                this.Capacity = this._size;
            }
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            int num1 = this.IndexOfKey(key);
            if (num1 >= 0)
            {
                value = this.values[num1];
                return true;
            }
            value = default(TValue);
            return false;
        }

        #endregion // Public Methods

        #region Private Methods
        private void EnsureCapacity(int min)
        {
            int num1 = (this.keys.Length == 0) ? 4 : (this.keys.Length * 2);
            if (num1 < min)
            {
                num1 = min;
            }
            this.InternalSetCapacity(num1, false);
        }

        private TValue GetByIndex(int index)
        {
            if ((index < 0) || (index >= this._size))
            {
                throw new ArgumentOutOfRangeException("index", "Index out of range");
            }
            return this.values[index];
        }

        private TKey GetKey(int index)
        {
            if ((index < 0) || (index >= this._size))
            {
                throw new ArgumentOutOfRangeException("index", "Index out of range");
            }
            return this.keys[index];
        }

        private KeyList<TKey, TValue> GetKeyListHelper()
        {
            if (this.keyList == null)
            {
                this.keyList = new KeyList<TKey, TValue>(this);
            }
            return this.keyList;
        }

        private ValueList<TKey, TValue> GetValueListHelper()
        {
            if (this.valueList == null)
            {
                this.valueList = new ValueList<TKey, TValue>(this);
            }
            return this.valueList;
        }

        private void Insert(int index, TKey key, TValue value)
        {
            if (this._size == this.keys.Length)
            {
                this.EnsureCapacity(this._size + 1);
            }
            if (index < this._size)
            {
                Array.Copy(this.keys, index, this.keys, (int)(index + 1),
                             (int)(this._size - index));
                Array.Copy(this.values, index, this.values, (int)(index + 1),
                             (int)(this._size - index));
            }
            this.keys[index] = key;
            this.values[index] = value;
            this._size++;
            this.version++;
        }

        private void InternalSetCapacity(int value, bool updateVersion)
        {
            if (value != this.keys.Length)
            {
                if (value < this._size)
                {
                    throw new ArgumentOutOfRangeException(
                       "value", "Too small capacity");
                }
                if (value > 0)
                {
                    TKey[] localArray1 = new TKey[value];
                    TValue[] localArray2 = new TValue[value];
                    if (this._size > 0)
                    {
                        Array.Copy(this.keys, 0, localArray1, 0, this._size);
                        Array.Copy(this.values, 0, localArray2, 0, this._size);
                    }
                    this.keys = localArray1;
                    this.values = localArray2;
                }
                else
                {
                    this.keys = ReversibleSortedList<TKey, TValue>.emptyKeys;
                    this.values = ReversibleSortedList<TKey, TValue>.emptyValues;
                }
                if (updateVersion)
                {
                    this.version++;
                }
            }
        }

        private static bool IsCompatibleKey(object key)
        {
            if (key.Equals(null))
            {
                throw new ArgumentNullException("key");
            }
            return (key is TKey);
        }

        void ICollection<KeyValuePair<TKey, TValue>>.Add(
                                                KeyValuePair<TKey, TValue> keyValuePair)
        {
            this.Add(keyValuePair.Key, keyValuePair.Value);
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Contains(
                                                     KeyValuePair<TKey, TValue> keyValuePair)
        {
            int num1 = this.IndexOfKey(keyValuePair.Key);
            if ((num1 >= 0) && EqualityComparer<TValue>.Default.Equals(
                                                                            this.values[num1],
                                                                            keyValuePair.
    Value))
            {
                return true;
            }
            return false;
        }

        void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(
                                                                KeyValuePair<TKey,
    TValue>[] array,
                                                                int arrayIndex)
        {
            if (array == null)
            {
                throw new ArgumentNullException("array");
            }
            if ((arrayIndex < 0) || (arrayIndex > array.Length))
            {
                throw new ArgumentOutOfRangeException(
                      "arrayIndex", "Need a non-negative number");
            }
            if ((array.Length - arrayIndex) < this.Count)
            {
                throw new ArgumentException("ArrayPlusOffTooSmall");
            }
            for (int num1 = 0; num1 < this.Count; num1++)
            {
                KeyValuePair<TKey, TValue> pair1;
                pair1 = new KeyValuePair<TKey, TValue>(
                            this.keys[num1], this.values[num1]);
                array[arrayIndex + num1] = pair1;
            }
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Remove(
                                                KeyValuePair<TKey, TValue> keyValuePair)
        {
            int num1 = this.IndexOfKey(keyValuePair.Key);
            if ((num1 >= 0) && EqualityComparer<TValue>.Default.Equals(
                                                         this.values[num1],
                                                         keyValuePair.Value))
            {
                this.RemoveAt(num1);
                return true;
            }
            return false;
        }

        IEnumerator<KeyValuePair<TKey, TValue>>
             IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator()
        {
            return new ReversibleSortedList<TKey, TValue>.Enumerator<TKey, TValue>(
                        this);
        }

        void ICollection.CopyTo(Array array, int arrayIndex)
        {
            if (array == null)
            {
                throw new ArgumentNullException("array");
            }
            if (array.Rank != 1)
            {
                throw new ArgumentException(
                        "MultiDimensional array copies are not supported");
            }
            if (array.GetLowerBound(0) != 0)
            {
                throw new ArgumentException("A non-zero lower bound was provided");
            }
            if ((arrayIndex < 0) || (arrayIndex > array.Length))
            {
                throw new ArgumentOutOfRangeException(
                        "arrayIndex", "Need non negative number");
            }
            if ((array.Length - arrayIndex) < this.Count)
            {
                throw new ArgumentException("Array plus the offset is too small");
            }
            KeyValuePair<TKey, TValue>[] pairArray1 =
                 array as KeyValuePair<TKey, TValue>[];
            if (pairArray1 != null)
            {
                for (int num1 = 0; num1 < this.Count; num1++)
                {
                    pairArray1[num1 + arrayIndex] =
                          new KeyValuePair<TKey, TValue>(this.keys[num1],
                                                               this.values[num1]);
                }
            }
            else
            {
                object[] objArray1 = array as object[];
                if (objArray1 == null)
                {
                    throw new ArgumentException("Invalid array type");
                }
                try
                {
                    for (int num2 = 0; num2 < this.Count; num2++)
                    {
                        objArray1[num2 + arrayIndex] =
                               new KeyValuePair<TKey, TValue>(this.keys[num2],
                                                                    this.values[num2]);
                    }
                }
                catch (ArrayTypeMismatchException)
                {
                    throw new ArgumentException("Invalid array type");
                }
            }
        }

        void IDictionary.Add(object key, object value)
        {
            ReversibleSortedList<TKey, TValue>.VerifyKey(key);
            ReversibleSortedList<TKey, TValue>.VerifyValueType(value);
            this.Add((TKey)key, (TValue)value);
        }

        bool IDictionary.Contains(object key)
        {
            if (ReversibleSortedList<TKey, TValue>.IsCompatibleKey(key))
            {
                return this.ContainsKey((TKey)key);
            }
            return false;
        }

        IDictionaryEnumerator IDictionary.GetEnumerator()
        {
            return new ReversibleSortedList<TKey, TValue>.Enumerator<TKey, TValue>(
                    this);
        }

        void IDictionary.Remove(object key)
        {
            if (ReversibleSortedList<TKey, TValue>.IsCompatibleKey(key))
            {
                this.Remove((TKey)key);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new ReversibleSortedList<TKey, TValue>.Enumerator<TKey, TValue>(
                    this);
        }


        private static void VerifyKey(object key)
        {
            if (key.Equals(null))
            {
                throw new ArgumentNullException("key");
            }
            if (!(key is TKey))
            {
                throw new ArgumentException(
                            "Argument passed is of wrong type", "key");
            }
        }

        private static void VerifyValueType(object value)
        {
            if (!(value is TValue) && ((value != null) || typeof(TValue).IsValueType))
            {
                throw new ArgumentException(
                            "Argument passed is of wrong type", "value");
            }
        }
        #endregion // Private methods

        #region Public Properties
        public int Capacity
        {
            get
            {
                return this.keys.Length;
            }
            set
            {
                this.InternalSetCapacity(value, true);
            }
        }

        public SortDirectionComparer<TKey> Comparer
        {
            get
            {
                return this._sortDirectionComparer;
            }
        }

        public int Count
        {
            get
            {
                return this._size;
            }
        }

        public TValue this[TKey key]
        {
            get
            {
                TValue local1;
                int num1 = this.IndexOfKey(key);
                if (num1 >= 0)
                {
                    return this.values[num1];
                }
                else
                {
                    //throw new KeyNotFoundException(); 
                    local1 = default(TValue);
                    return local1;
                }
            }
            set
            {
                if (key == null)
                {
                    throw new ArgumentNullException("key");
                }
                int num1 = Array.BinarySearch<TKey>(this.keys, 0, this._size, key,
                                                            this._sortDirectionComparer);
                if (num1 >= 0)
                {
                    this.values[num1] = value;
                    this.version++;
                }
                else
                {
                    this.Insert(~num1, key, value);
                }
            }
        }

        public IList<TKey> Keys
        {
            get
            {
                return this.GetKeyListHelper();
            }
        }

        public IList<TValue> Values
        {
            get
            {
                return this.GetValueListHelper();
            }
        }
        #endregion // Public Properties

        #region Private Properties
        bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly
        {
            get
            {
                return false;
            }
        }

        ICollection<TKey> IDictionary<TKey, TValue>.Keys
        {
            get
            {
                return this.GetKeyListHelper();
            }
        }

        ICollection<TValue> IDictionary<TKey, TValue>.Values
        {
            get
            {
                return this.GetValueListHelper();
            }
        }

        bool ICollection.IsSynchronized
        {
            get
            {
                return false;
            }
        }

        object ICollection.SyncRoot
        {
            get
            {
                return this;
            }
        }

        bool IDictionary.IsFixedSize
        {
            get
            {
                return false;
            }
        }

        bool IDictionary.IsReadOnly
        {
            get
            {
                return false;
            }
        }

        object IDictionary.this[object key]
        {
            get
            {
                if (ReversibleSortedList<TKey, TValue>.IsCompatibleKey(key))
                {
                    int num1 = this.IndexOfKey((TKey)key);
                    if (num1 >= 0)
                    {
                        return this.values[num1];
                    }
                }
                return null;
            }
            set
            {
                ReversibleSortedList<TKey, TValue>.VerifyKey(key);
                ReversibleSortedList<TKey, TValue>.VerifyValueType(value);
                this[(TKey)key] = (TValue)value;
            }
        }

        ICollection IDictionary.Keys
        {
            get
            {
                return this.GetKeyListHelper();
            }
        }

        ICollection IDictionary.Values
        {
            get
            {
                return this.GetValueListHelper();
            }
        }
        #endregion // Private properties

        #region Fields
        private const int _defaultCapacity = 4;
        private int _size;
        //private IComparer<TKey> comparer;
        private static TKey[] emptyKeys;
        private static TValue[] emptyValues;
        private KeyList<TKey, TValue> keyList;
        private TKey[] keys;
        private ValueList<TKey, TValue> valueList;
        private TValue[] values;
        private int version;
        // Declare comparison object.
        private SortDirectionComparer<TKey> _sortDirectionComparer = null;
        // Default to ascending.
        private ListSortDirection _currentSortDirection = ListSortDirection.Descending;
        #endregion

        #region Nested Types

        #region Enumerator <K, V>
        [Serializable, StructLayout(LayoutKind.Sequential)]
        private struct Enumerator<K, V> : IEnumerator<KeyValuePair<K, V>>, IDisposable,
                                                IDictionaryEnumerator, IEnumerator
        {
            private ReversibleSortedList<K, V> _ReversibleSortedList;
            private K key;
            private V value;
            private int index;
            private int version;
            internal Enumerator(ReversibleSortedList<K, V> ReversibleSortedList)
            {
                this._ReversibleSortedList = ReversibleSortedList;
                this.index = 0;
                this.version = this._ReversibleSortedList.version;
                this.key = default(K);
                this.value = default(V);
            }
            public void Dispose()
            {
                this.index = 0;
                this.key = default(K);
                this.value = default(V);
            }
            object IDictionaryEnumerator.Key
            {
                get
                {
                    if ((this.index == 0) ||
                        (this.index == (this._ReversibleSortedList.Count + 1)))
                    {
                        throw new InvalidOperationException(
                               "Enumeration operation cannot occur.");
                    }
                    return this.key;
                }
            }
            public bool MoveNext()
            {
                if (this.version != this._ReversibleSortedList.version)
                {
                    throw new InvalidOperationException(
                           "Enumeration failed version check");
                }
                if (this.index < this._ReversibleSortedList.Count)
                {
                    this.key = this._ReversibleSortedList.keys[this.index];
                    this.value = this._ReversibleSortedList.values[this.index];
                    this.index++;
                    return true;
                }
                this.index = this._ReversibleSortedList.Count + 1;
                this.key = default(K);
                this.value = default(V);
                return false;
            }
            DictionaryEntry IDictionaryEnumerator.Entry
            {
                get
                {
                    if ((this.index == 0) ||
                        (this.index == (this._ReversibleSortedList.Count + 1)))
                    {
                        throw new InvalidOperationException(
                               "Enumeration operation cannot happen.");
                    }
                    return new DictionaryEntry(this.key, this.value);
                }
            }
            public KeyValuePair<K, V> Current
            {
                get
                {
                    return new KeyValuePair<K, V>(this.key, this.value);
                }
            }
            object IEnumerator.Current
            {
                get
                {
                    if ((this.index == 0) ||
                        (this.index == (this._ReversibleSortedList.Count + 1)))
                    {
                        throw new InvalidOperationException(
                                "Enumeration operation cannot occur");
                    }
                    return new DictionaryEntry(this.key, this.value);
                }
            }
            object IDictionaryEnumerator.Value
            {
                get
                {
                    if ((this.index == 0) ||
                        (this.index == (this._ReversibleSortedList.Count + 1)))
                    {
                        throw new InvalidOperationException(
                                "Enumeration operation cannot occur");
                    }
                    return this.value;
                }
            }
            void IEnumerator.Reset()
            {
                if (this.version != this._ReversibleSortedList.version)
                {
                    throw new InvalidOperationException(
                            "Enumeration version check failed");
                }
                this.index = 0;
                this.key = default(K);
                this.value = default(V);
            }
        }
        #endregion // Enumerator <K, V>

        #region KeyList<K,V>
        [Serializable]
        private sealed class KeyList<K, V> : IList<K>, ICollection<K>,
                                                   IEnumerable<K>, ICollection, IEnumerable
        {
            // Methods
            internal KeyList(ReversibleSortedList<K, V> dictionary)
            {
                this._dict = dictionary;
            }

            public void Add(K key)
            {
                throw new NotSupportedException("Add is unsupported");
            }

            public void Clear()
            {
                throw new NotSupportedException("Clear is unsupported");
            }

            public bool Contains(K key)
            {
                return this._dict.ContainsKey(key);
            }

            public void CopyTo(K[] array, int arrayIndex)
            {
                Array.Copy(this._dict.keys, 0, array, arrayIndex, this._dict.Count);
            }

            public IEnumerator<K> GetEnumerator()
            {
                return new
                      ReversibleSortedList<K, V>.ReversibleSortedListKeyEnumerator(
                                                          this._dict);
            }

            public int IndexOf(K key)
            {
                if (key == null)
                {
                    throw new ArgumentNullException("key");
                }
                int num1 = Array.BinarySearch<K>(this._dict.keys, 0,
                                                       this._dict.Count, key,
                                                       this._dict._sortDirectionComparer);
                if (num1 >= 0)
                {
                    return num1;
                }
                return -1;
            }

            public void Insert(int index, K value)
            {
                throw new NotSupportedException("Insert is unsupported");
            }

            public bool Remove(K key)
            {
                //throw new NotSupportedException("Remove is unsupported"); 
                return false;
            }

            public void RemoveAt(int index)
            {
                throw new NotSupportedException("RemoveAt is unsupported");
            }

            void ICollection.CopyTo(Array array, int arrayIndex)
            {
                if ((array != null) && (array.Rank != 1))
                {
                    throw new ArgumentException(
                         "MultiDimensional arrays are not unsupported");
                }
                try
                {
                    Array.Copy(this._dict.keys, 0, array, arrayIndex,
                               this._dict.Count);
                }
                catch (ArrayTypeMismatchException atme)
                {
                    throw new ArgumentException("InvalidArrayType", atme);
                }
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return new
                       ReversibleSortedList<K, V>.ReversibleSortedListKeyEnumerator(
                                                                               this._dict);
            }

            // Properties
            public int Count
            {
                get
                {
                    return this._dict._size;
                }
            }

            public bool IsReadOnly
            {
                get
                {
                    return true;
                }
            }

            public K this[int index]
            {
                get
                {
                    return this._dict.GetKey(index);
                }
                set
                {
                    throw new NotSupportedException("Set is an unsupported operation");
                }
            }

            bool ICollection.IsSynchronized
            {
                get
                {
                    return false;
                }
            }

            object ICollection.SyncRoot
            {
                get
                {
                    return this._dict;
                }
            }


            // Fields
            private ReversibleSortedList<K, V> _dict;
        }
        #endregion // KeyList<K,V>

        #region ReversibleSortedListKeyEnumerator definition
        [Serializable]
        private sealed class ReversibleSortedListKeyEnumerator : IEnumerator<TKey>,
                                                                 IDisposable,
                                                                 IEnumerator
        {
            // Methods
            internal ReversibleSortedListKeyEnumerator(
                   ReversibleSortedList<TKey, TValue> ReversibleSortedList)
            {
                this._ReversibleSortedList = ReversibleSortedList;
                this.version = ReversibleSortedList.version;
            }

            public void Dispose()
            {
                this.index = 0;
                this.currentKey = default(TKey);
            }

            public bool MoveNext()
            {
                if (this.version != this._ReversibleSortedList.version)
                {
                    throw new InvalidOperationException(
                        "Enumeration failed version check");
                }
                if (this.index < this._ReversibleSortedList.Count)
                {
                    this.currentKey = this._ReversibleSortedList.keys[this.index];
                    this.index++;
                    return true;
                }
                this.index = this._ReversibleSortedList.Count + 1;
                this.currentKey = default(TKey);
                return false;
            }

            void IEnumerator.Reset()
            {
                if (this.version != this._ReversibleSortedList.version)
                {
                    throw new InvalidOperationException(
                        "Enumeration failed version check");
                }
                this.index = 0;
                this.currentKey = default(TKey);
            }


            // Properties
            public TKey Current
            {
                get
                {
                    return this.currentKey;
                }
            }

            object IEnumerator.Current
            {
                get
                {
                    if ((this.index == 0) || (this.index ==
                           (this._ReversibleSortedList.Count + 1)))
                    {
                        throw new InvalidOperationException(
                                "Enumeration operation could not occur");
                    }
                    return this.currentKey;
                }
            }


            // Fields
            private ReversibleSortedList<TKey, TValue> _ReversibleSortedList;
            private TKey currentKey;
            private int index;
            private int version;
        }
        #endregion //ReversibleSortedListKeyEnumerator definition

        #region ReversibleSortedListValueEnumerator definition
        [Serializable]
        private sealed class ReversibleSortedListValueEnumerator : IEnumerator<TValue>,
                                                                   IDisposable,
                                                                   IEnumerator
        {
            // Methods
            internal ReversibleSortedListValueEnumerator(
                             ReversibleSortedList<TKey, TValue> ReversibleSortedList)
            {
                this._ReversibleSortedList = ReversibleSortedList;
                this.version = ReversibleSortedList.version;
            }

            public void Dispose()
            {
                this.index = 0;
                this.currentValue = default(TValue);
            }

            public bool MoveNext()
            {
                if (this.version != this._ReversibleSortedList.version)
                {
                    throw new InvalidOperationException(
                        "Enumeration failed version check");
                }
                if (this.index < this._ReversibleSortedList.Count)
                {
                    this.currentValue = this._ReversibleSortedList.values[this.index];
                    this.index++;
                    return true;
                }
                this.index = this._ReversibleSortedList.Count + 1;
                this.currentValue = default(TValue);
                return false;
            }

            void IEnumerator.Reset()
            {
                if (this.version != this._ReversibleSortedList.version)
                {
                    throw new InvalidOperationException(
                        "Enumeration failed version check");
                }
                this.index = 0;
                this.currentValue = default(TValue);
            }


            // Properties
            public TValue Current
            {
                get
                {
                    return this.currentValue;
                }
            }

            object IEnumerator.Current
            {
                get
                {
                    if ((this.index == 0) || (this.index ==
                           (this._ReversibleSortedList.Count + 1)))
                    {
                        throw new InvalidOperationException(
                                "Enumeration operation could not occur");
                    }
                    return this.currentValue;
                }
            }


            // Fields
            private ReversibleSortedList<TKey, TValue> _ReversibleSortedList;
            private TValue currentValue;
            private int index;
            private int version;
        }
        #endregion //ReversibleSortedListValueEnumerator

        #region ValueList <K, V> definition
        [Serializable]
        private sealed class ValueList<K, V> : IList<V>, ICollection<V>,
                                                     IEnumerable<V>, ICollection, IEnumerable
        {
            // Methods
            internal ValueList(ReversibleSortedList<K, V> dictionary)
            {
                this._dict = dictionary;
            }

            public void Add(V key)
            {
                throw new NotSupportedException("Add is not supported");
            }

            public void Clear()
            {
                throw new NotSupportedException("Clear is not supported");
            }

            public bool Contains(V value)
            {
                return this._dict.ContainsValue(value);
            }

            public void CopyTo(V[] array, int arrayIndex)
            {
                Array.Copy(this._dict.values, 0, array, arrayIndex, this._dict.Count);
            }

            public IEnumerator<V> GetEnumerator()
            {
                return new
                       ReversibleSortedList<K, V>.ReversibleSortedListValueEnumerator(
                                                                            this._dict);
            }

            public int IndexOf(V value)
            {
                return Array.IndexOf<V>(this._dict.values, value, 0, this._dict.Count);
            }

            public void Insert(int index, V value)
            {
                throw new NotSupportedException("Insert is not supported");
            }

            public bool Remove(V value)
            {
                //throw new NotSupportedException("Remove is not supported"); 
                return false;
            }

            public void RemoveAt(int index)
            {
                throw new NotSupportedException("RemoveAt is not supported");
            }

            void ICollection.CopyTo(Array array, int arrayIndex)
            {
                if ((array != null) && (array.Rank != 1))
                {
                    throw new ArgumentException(
                        "MultiDimensional arrays not supported");
                }
                try
                {
                    Array.Copy(this._dict.values, 0, array, arrayIndex,
                               this._dict.Count);
                }
                catch (ArrayTypeMismatchException atme)
                {
                    throw new ArgumentException("Invalid array type", atme);
                }
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return new
                       ReversibleSortedList<K, V>.ReversibleSortedListValueEnumerator(
                                                                          this._dict);
            }


            // Properties
            public int Count
            {
                get
                {
                    return this._dict._size;
                }
            }

            public bool IsReadOnly
            {
                get
                {
                    return true;
                }
            }

            public V this[int index]
            {
                get
                {
                    return this._dict.GetByIndex(index);
                }
                set
                {
                    throw new NotSupportedException("Set by indexer is not supported");
                }
            }

            bool ICollection.IsSynchronized
            {
                get
                {
                    return false;
                }
            }

            object ICollection.SyncRoot
            {
                get
                {
                    return this._dict;
                }
            }


            // Fields
            private ReversibleSortedList<K, V> _dict;
        }
        #endregion // ValueList <TKey, TValue> definition

        #endregion // Nested types
    }
}
