using System.Collections;
using System.Collections.Generic;

namespace Daliboris.Statistiky.Core.Models.Jevy.XML
{
    // Nadrazená třida, kde jsou uloženy všechny skupiny jevů
    public class SkupinaJevu : ICollection<Statistiky.Jevy>
    {
        private readonly SortedDictionary<string, Statistiky.Jevy> mgsdcJevy = new SortedDictionary<string, Statistiky.Jevy>();

        #region Implementation of IEnumerable

        private bool mIsReadOnly = false;

        public bool ContainsID(string strID)
        {
            return mgsdcJevy.ContainsKey(strID);
        }

        public Statistiky.Jevy this[string strID]
        {
            get { return mgsdcJevy[strID]; }
            set { mgsdcJevy[strID] = value; }
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"></see> that can be used to iterate through the collection.
        /// </returns>
        /// <filterpriority>1</filterpriority>
        public IEnumerator<Statistiky.Jevy> GetEnumerator()
        {
            return mgsdcJevy.Values.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"></see> object that can be used to iterate through the collection.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        #region Implementation of ICollection<Jevy>

        /// <summary>
        /// Adds an item to the <see cref="T:System.Collections.Generic.ICollection`1"></see>.
        /// </summary>
        /// <param name="item">The object to add to the <see cref="T:System.Collections.Generic.ICollection`1"></see>.</param>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1"></see> is read-only.</exception>
        public void Add(Statistiky.Jevy item)
        {
            if (mgsdcJevy != null) mgsdcJevy.Add(item.ID, item);
        }

        /// <summary>
        /// Removes all items from the <see cref="T:System.Collections.Generic.ICollection`1"></see>.
        /// </summary>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1"></see> is read-only. </exception>
        public void Clear()
        {
            mgsdcJevy.Clear();
        }

        /// <summary>
        /// Determines whether the <see cref="T:System.Collections.Generic.ICollection`1"></see> contains a specific value.
        /// </summary>
        /// <returns>
        /// true if item is found in the <see cref="T:System.Collections.Generic.ICollection`1"></see>; otherwise, false.
        /// </returns>
        /// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.ICollection`1"></see>.</param>
        public bool Contains(Statistiky.Jevy item)
        {
            return mgsdcJevy.ContainsKey(item.ID);
        }

        /// <summary>
        /// Copies the elements of the <see cref="T:System.Collections.Generic.ICollection`1"></see> to an <see cref="T:System.Array"></see>, starting at a particular <see cref="T:System.Array"></see> index.
        /// </summary>
        /// <param name="array">The one-dimensional <see cref="T:System.Array"></see> that is the destination of the elements copied from <see cref="T:System.Collections.Generic.ICollection`1"></see>. The <see cref="T:System.Array"></see> must have zero-based indexing.</param>
        /// <param name="arrayIndex">The zero-based index in array at which copying begins.</param>
        /// <exception cref="T:System.ArgumentOutOfRangeException">arrayIndex is less than 0.</exception>
        /// <exception cref="T:System.ArgumentNullException">array is null.</exception>
        /// <exception cref="T:System.ArgumentException">array is multidimensional.-or-arrayIndex is equal to or greater than the length of array.-or-The number of elements in the source <see cref="T:System.Collections.Generic.ICollection`1"></see> is greater than the available space from arrayIndex to the end of the destination array.-or-Type T cannot be cast automatically to the type of the destination array.</exception>
        public void CopyTo(Statistiky.Jevy[] array, int arrayIndex)
        {
            mgsdcJevy.Values.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Removes the first occurrence of a specific object from the <see cref="T:System.Collections.Generic.ICollection`1"></see>.
        /// </summary>
        /// <returns>
        /// true if item was successfully removed from the <see cref="T:System.Collections.Generic.ICollection`1"></see>; otherwise, false. This method also returns false if item is not found in the original <see cref="T:System.Collections.Generic.ICollection`1"></see>.
        /// </returns>
        /// <param name="item">The object to remove from the <see cref="T:System.Collections.Generic.ICollection`1"></see>.</param>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1"></see> is read-only.</exception>
        public bool Remove(Statistiky.Jevy item)
        {
            return mgsdcJevy.Remove(item.ID);
        }

        /// <summary>
        /// Gets the number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1"></see>.
        /// </summary>
        /// <returns>
        /// The number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1"></see>.
        /// </returns>
        public int Count
        {
            get { return mgsdcJevy.Count; }
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="T:System.Collections.Generic.ICollection`1"></see> is read-only.
        /// </summary>
        /// <returns>
        /// true if the <see cref="T:System.Collections.Generic.ICollection`1"></see> is read-only; otherwise, false.
        /// </returns>
        public bool IsReadOnly
        {
            get { return mIsReadOnly; }
        }

        public static SkupinaJevu operator +(SkupinaJevu skjv1, SkupinaJevu skjv2)
        {
            SkupinaJevu skjv = new SkupinaJevu();
            if (skjv1 != null)
            {
                foreach (Statistiky.Jevy jvs in skjv1)
                {
                    Statistiky.Jevy jv = new Statistiky.Jevy(jvs.Druh, jvs.Zdroj.CelaCesta, jvs.Jazyk, jvs.Identifikator);
                    jv.Popis = jvs.Popis;
                    foreach (Jev j in jvs)
                    {
                        jv.Add(j);
                    }

                    skjv.Add(jv);
                }
            }

            if (skjv2 != null)
            {
                foreach (Statistiky.Jevy jvs in skjv2)
                {
                    if (skjv.ContainsID(jvs.ID))
                    {
                        Statistiky.Jevy jv = skjv[jvs.ID];
                        foreach (Jev j in jvs)
                        {
                            jv.Add(j);
                        }
                    }
                    else
                    {
                        Statistiky.Jevy jv = new Statistiky.Jevy(jvs.Druh, jvs.Zdroj.CelaCesta, jvs.Jazyk, jvs.Identifikator);
                        jv.Popis = jvs.Popis;
                        foreach (Jev j in jvs)
                        {
                            jv.Add(j);
                        }

                        skjv.Add(jv);
                    }
                }
            }

            return skjv;
        }

        #endregion
    }
}