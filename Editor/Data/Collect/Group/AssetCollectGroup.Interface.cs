using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace AIO.UEditor
{
    partial class AssetCollectGroup : IDisposable, IEqualityComparer<AssetCollectGroup>, IList<AssetCollectItem>
    {
        public AssetCollectItem this[int index]
        {
            get => Collectors[index];
            set => Collectors[index] = value;
        }

        public void Dispose()
        {
            if (Collectors is null) return;
            foreach (var collect in Collectors) collect.Dispose();
        }

        /// <summary>
        ///     获取迭代器
        /// </summary>
        /// <returns>迭代器</returns>
        public IEnumerator<AssetCollectItem> GetEnumerator()
        {
            if (Collectors is null)
            {
                Collectors = Array.Empty<AssetCollectItem>();
                yield break;
            }

            foreach (var collect in Collectors) yield return collect;
        }

        public int IndexOf(AssetCollectItem item)
        {
            if (Collectors is null)
            {
                Collectors = Array.Empty<AssetCollectItem>();
                return -1;
            }

            for (var i = 0; i < Collectors.Length; i++)
            {
                if (Collectors[i].Equals(item)) return i;
            }

            return -1;
        }

        public void Insert(int index, AssetCollectItem item)
        {
            if (Collectors is null) Collectors = new AssetCollectItem[index + 1];
            if (Collectors.Length <= index)
            {
                var temp                                            = new AssetCollectItem[index + 1];
                for (var i = 0; i < Collectors.Length; i++) temp[i] = Collectors[i];
                Collectors = temp;
            }
            else Collectors[index] = item;
        }

        public void RemoveAt(int index)
        {
            if (Collectors is null) return;
            if (Collectors.Length <= index) return;
            Collectors = Collectors.RemoveAt(index);
        }

        public void Add(AssetCollectItem item) => Collectors = Collectors is null ? new[] { item } : Collectors.Add(item);

        public void Clear() => Collectors = Array.Empty<AssetCollectItem>();

        public bool Contains(AssetCollectItem item) => IndexOf(item) != -1;

        public void CopyTo(AssetCollectItem[] array, int arrayIndex)
        {
            if (Collectors is null) return;
            if (array.Length <= arrayIndex) return;
            if (array.Length < Collectors.Length + arrayIndex) return;
            for (var i = 0; i < Collectors.Length; i++) array[i + arrayIndex] = Collectors[i];
        }

        public bool Remove(AssetCollectItem item)
        {
            var index = IndexOf(item);
            if (index == -1) return false;
            RemoveAt(index);
            return true;
        }

        #region IEnumerator

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        #endregion

        #region IEqualityComparer<AssetCollectGroup>

        bool IEqualityComparer<AssetCollectGroup>.Equals(AssetCollectGroup x, AssetCollectGroup y)
        {
            if (x is null) return y is null;
            if (y is null) return false;
            return x.GetHashCode() == y.GetHashCode();
        }

        int IEqualityComparer<AssetCollectGroup>.GetHashCode(AssetCollectGroup obj)
        {
            if (obj.Equals(null)) return 0;
            unchecked
            {
                var hashCode = (obj.Name.GetHashCode() * 397) ^
                               (!string.IsNullOrEmpty(obj.Tag) ? obj.Tag.GetHashCode() : 0);

                hashCode = (hashCode * 397) ^
                           (!string.IsNullOrEmpty(obj.Description) ? obj.Description.GetHashCode() : 0);

                return obj.Collectors is null
                    ? hashCode
                    : obj.Collectors.Aggregate(hashCode, (current, item) => (current * 397) ^ item.GetHashCode());
            }
        }

        #endregion

        #region IList<AssetCollectGroup>

        int ICollection<AssetCollectItem>. Count      => Count;
        bool ICollection<AssetCollectItem>.IsReadOnly => false;

        int IList<AssetCollectItem>.IndexOf(AssetCollectItem item) => IndexOf(item);

        void ICollection<AssetCollectItem>.Add(AssetCollectItem item) => Add(item);

        void ICollection<AssetCollectItem>.Clear() => Clear();

        bool ICollection<AssetCollectItem>.Contains(AssetCollectItem item) => Contains(item);

        void ICollection<AssetCollectItem>.CopyTo(AssetCollectItem[] array, int arrayIndex) => CopyTo(array, arrayIndex);

        void IList<AssetCollectItem>.Insert(int index, AssetCollectItem item) => Insert(index, item);

        bool ICollection<AssetCollectItem>.Remove(AssetCollectItem item) => Remove(item);

        void IList<AssetCollectItem>.RemoveAt(int index) => RemoveAt(index);

        #endregion
    }
}