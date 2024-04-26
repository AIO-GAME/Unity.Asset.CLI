using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace AIO.UEditor
{
    partial class AssetCollectPackage : IDisposable, IEqualityComparer<AssetCollectPackage>, IList<AssetCollectGroup>
    {
        /// <summary>
        ///    获取资源收集组
        /// </summary>
        /// <param name="idxG">组索引</param>
        public AssetCollectGroup this[int idxG]
        {
            get => Groups[idxG];
            set => Groups[idxG] = value;
        }

        /// <summary>
        ///   获取资源收集器
        /// </summary>
        /// <param name="idxG">组索引</param>
        /// <param name="idxC">收集器索引</param>
        public AssetCollectItem this[int idxG, int idxC]
        {
            get => Groups[idxG].Collectors[idxC];
            set => Groups[idxG].Collectors[idxC] = value;
        }

        /// <summary>
        ///     获取迭代器
        /// </summary>
        /// <returns>迭代器</returns>
        public IEnumerator<AssetCollectGroup> GetEnumerator() => ((IEnumerable<AssetCollectGroup>)Groups).GetEnumerator();

        /// <summary>
        ///     获取组数量
        /// </summary>
        public int Count
        {
            get
            {
                if (Groups is null) Groups = Array.Empty<AssetCollectGroup>();
                return Groups.Length;
            }
        }

        public int IndexOf(AssetCollectGroup item)
        {
            if (Groups is null)
            {
                Groups = Array.Empty<AssetCollectGroup>();
                return -1;
            }

            for (var i = 0; i < Groups.Length; i++)
            {
                if (Groups[i] == item) return i;
            }

            return -1;
        }

        public void Insert(int index, AssetCollectGroup item)
        {
            if (Groups is null) Groups = new AssetCollectGroup[index + 1];
            if (Groups.Length <= index)
            {
                var temp                                        = new AssetCollectGroup[index + 1];
                for (var i = 0; i < Groups.Length; i++) temp[i] = Groups[i];
                Groups = temp;
            }
            else Groups[index] = item;
        }

        public void RemoveAt(int index)
        {
            if (Groups is null) return;
            if (Groups.Length <= index) return;
            Groups = Groups.RemoveAt(index);
        }

        public void Add(AssetCollectGroup item) => Groups = Groups is null ? new[] { item } : Groups.Add(item);

        public void Clear() => Groups = Array.Empty<AssetCollectGroup>();

        public bool Contains(AssetCollectGroup item) => IndexOf(item) != -1;

        public void CopyTo(AssetCollectGroup[] array, int arrayIndex)
        {
            if (Groups is null) return;
            if (array.Length <= arrayIndex) return;
            if (array.Length < Groups.Length + arrayIndex) return;
            for (var i = 0; i < Groups.Length; i++) array[i + arrayIndex] = Groups[i];
        }

        public bool Remove(AssetCollectGroup item)
        {
            var index = IndexOf(item);
            if (index == -1) return false;
            RemoveAt(index);
            return true;
        }

        #region IEqualityComparer<AssetCollectPackage>

        bool IEqualityComparer<AssetCollectPackage>.Equals(AssetCollectPackage x, AssetCollectPackage y)
        {
            if (x is null) return y is null;
            if (y is null) return false;
            return x.GetHashCode() == y.GetHashCode();
        }

        int IEqualityComparer<AssetCollectPackage>.GetHashCode(AssetCollectPackage obj)
        {
            if (obj.Equals(null)) return 0;
            unchecked
            {
                var hashCode = (obj.Name.GetHashCode() * 397) ^ (!string.IsNullOrEmpty(obj.Name) ? obj.Name.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (!string.IsNullOrEmpty(obj.Description) ? obj.Description.GetHashCode() : 0);
                return obj.Groups is null
                    ? hashCode
                    : obj.Groups.Aggregate(hashCode, (current, item) => (current * 397) ^ item.GetHashCode());
            }
        }

        #endregion

        #region IDisposable

        void IDisposable.Dispose() => Dispose();

        #endregion

        #region IEnumerator

        IEnumerator<AssetCollectGroup> IEnumerable<AssetCollectGroup>.GetEnumerator() => GetEnumerator();
        IEnumerator IEnumerable.                                      GetEnumerator() => GetEnumerator();

        #endregion

        #region IList<AssetCollectGroup>

        int ICollection<AssetCollectGroup>. Count      => Count;
        bool ICollection<AssetCollectGroup>.IsReadOnly => false;

        int IList<AssetCollectGroup>.IndexOf(AssetCollectGroup item) => IndexOf(item);

        void ICollection<AssetCollectGroup>.Add(AssetCollectGroup item) => Add(item);

        void ICollection<AssetCollectGroup>.Clear() => Clear();

        bool ICollection<AssetCollectGroup>.Contains(AssetCollectGroup item) => Contains(item);

        void ICollection<AssetCollectGroup>.CopyTo(AssetCollectGroup[] array, int arrayIndex) => CopyTo(array, arrayIndex);

        void IList<AssetCollectGroup>.Insert(int index, AssetCollectGroup item) => Insert(index, item);

        bool ICollection<AssetCollectGroup>.Remove(AssetCollectGroup item) => Remove(item);

        void IList<AssetCollectGroup>.RemoveAt(int index) => RemoveAt(index);

        #endregion
    }
}