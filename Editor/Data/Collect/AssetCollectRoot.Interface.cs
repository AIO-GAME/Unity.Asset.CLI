#region

using System;
using System.Collections;
using System.Collections.Generic;

#endregion

namespace AIO.UEditor
{
    partial class AssetCollectRoot : IList<AssetCollectPackage>
    {
        public AssetCollectPackage this[int idxP]
        {
            get => Packages[idxP];
            set => Packages[idxP] = value;
        }

        public AssetCollectGroup this[int idxP, int idxG]
        {
            get => Packages[idxP].Groups[idxG];
            set => Packages[idxP].Groups[idxG] = value;
        }

        public AssetCollectItem this[int idxP, int idxG, int idxC]
        {
            get => Packages[idxP].Groups[idxG].Collectors[idxC];
            set => Packages[idxP].Groups[idxG].Collectors[idxC] = value;
        }

        public int IndexOf(AssetCollectPackage item)
        {
            if (Packages is null)
            {
                Packages = Array.Empty<AssetCollectPackage>();
                return -1;
            }

            for (var i = 0; i < Packages.Length; i++)
            {
                if (Packages[i] == item) return i;
            }

            return -1;
        }

        public void Insert(int index, AssetCollectPackage item)
        {
            if (Packages is null) Packages = new AssetCollectPackage[index + 1];
            if (Packages.Length <= index)
            {
                var temp = new AssetCollectPackage[index + 1];
                for (var i = 0; i < Packages.Length; i++) temp[i] = Packages[i];
                Packages = temp;
            }
            else Packages[index] = item;
        }

        public void RemoveAt(int index)
        {
            if (Packages is null) return;
            if (Packages.Length <= index) return;
            Packages = Packages.RemoveAt(index);
        }

        public void Add(AssetCollectPackage item)
            => Packages = Packages is null ? new[] { item } : Packages.Add(item);

        public void Clear()
            => Packages = Array.Empty<AssetCollectPackage>();

        public bool Contains(AssetCollectPackage item)
            => IndexOf(item) != -1;

        public IEnumerator<AssetCollectPackage> GetEnumerator()
            => ((IEnumerable<AssetCollectPackage>)Packages).GetEnumerator();

        public void CopyTo(AssetCollectPackage[] array, int arrayIndex)
        {
            if (Packages is null) return;
            if (array.Length <= arrayIndex) return;
            if (array.Length < Packages.Length + arrayIndex) return;
            for (var i = 0; i < Packages.Length; i++) array[i + arrayIndex] = Packages[i];
        }

        public bool Remove(AssetCollectPackage item)
        {
            var index = IndexOf(item);
            if (index == -1) return false;
            RemoveAt(index);
            return true;
        }

        /// <summary>
        ///     获取资源包数量
        /// </summary>
        public int Count
        {
            get
            {
                if (Packages is null) Packages = Array.Empty<AssetCollectPackage>();
                return Packages.Length;
            }
        }

        #region ICollection

        bool ICollection<AssetCollectPackage>.IsReadOnly => false;

        #endregion

        #region IEnumerable

        IEnumerator<AssetCollectPackage> IEnumerable<AssetCollectPackage>.GetEnumerator() => GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        #endregion
    }
}