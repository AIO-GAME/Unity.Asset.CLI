using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using Object = UnityEngine.Object;

namespace AIO.UEditor
{
    [Serializable]
    public sealed class AssetCollectGroup : IDisposable, IEqualityComparer<AssetCollectGroup>, IEnumerable
    {
        /// <summary>
        ///    是否启用
        /// </summary>
        public bool Enable = true;
        
        /// <summary>
        ///     组名
        /// </summary>
        public string Name;

        /// <summary>
        ///     组描述
        /// </summary>
        public string Description;

        /// <summary>
        ///     资源标签 使用;分割
        /// </summary>
        public string Tags;

        /// <summary>
        ///     资源收集配置
        /// </summary>
        public AssetCollectItem[] Collectors;

        /// <summary>
        ///     获取资源收集器数量
        /// </summary>
        public int Length
        {
            get
            {
                if (Collectors is null) Collectors = Array.Empty<AssetCollectItem>();
                return Collectors.Length;
            }
        }

        public AssetCollectItem this[int index]
        {
            get => Collectors[index];
            set => Collectors[index] = value;
        }

        public string[] AllTags
        {
            get
            {
                var dictionary = new List<string>();
                foreach (var collect in Collectors) dictionary.AddRange(collect.AllTags);
                if (string.IsNullOrEmpty(Tags)) return dictionary.Distinct().ToArray();
                dictionary.AddRange(Tags.Split(';', ' ', ','));
                return dictionary.Distinct().ToArray();
            }
        }

        public void Dispose()
        {
            if (Collectors is null) Collectors = Array.Empty<AssetCollectItem>();
            foreach (var collect in Collectors) collect.Dispose();
        }

        public bool Equals(AssetCollectGroup x, AssetCollectGroup y)
        {
            if (x is null) return y is null;
            if (y is null) return false;
            return x.GetHashCode() == y.GetHashCode();
        }

        public int GetHashCode(AssetCollectGroup obj)
        {
            if (obj.Equals(null)) return 0;
            unchecked
            {
                var hashCode = (obj.Name.GetHashCode() * 397) ^
                               (!string.IsNullOrEmpty(obj.Tags) ? obj.Tags.GetHashCode() : 0);

                hashCode = (hashCode * 397) ^
                           (!string.IsNullOrEmpty(obj.Description) ? obj.Description.GetHashCode() : 0);

                return obj.Collectors is null
                    ? hashCode
                    : obj.Collectors.Aggregate(hashCode, (current, item) => (current * 397) ^ item.GetHashCode());
            }
        }

        /// <summary>
        ///     刷新资源
        /// </summary>
        public void Refresh()
        {
            if (Collectors is null) Collectors = Array.Empty<AssetCollectItem>();
            foreach (var collect in Collectors)
            {
                if (!AHelper.IO.Exists(collect.CollectPath)) continue;
                collect.Path = AssetDatabase.LoadAssetAtPath<Object>(collect.CollectPath);
            }
        }

        /// <param name="collectPath">收集器资源路径 会被转化成 GUID</param>
        /// <returns>收集器</returns>
        public AssetCollectItem GetByPath(string collectPath)
        {
            if (Collectors is null)
            {
                Collectors = Array.Empty<AssetCollectItem>();
                return null;
            }

            var guid = AssetDatabase.AssetPathToGUID(collectPath);
            return Collectors.Where(collectItem => collectItem != null).
                              FirstOrDefault(collectItem => collectItem.GUID == guid);
        }

        /// <param name="guid">收集器资源路径GUID</param>
        /// <returns>收集器</returns>
        public AssetCollectItem GetByGUID(string guid)
        {
            if (Collectors is null)
            {
                Collectors = Array.Empty<AssetCollectItem>();
                return null;
            }

            return Collectors.Where(collectItem => collectItem != null).
                              FirstOrDefault(collectItem => collectItem.GUID == guid);
        }

        public void Save()
        {
            if (Collectors is null) Collectors = Array.Empty<AssetCollectItem>();
            foreach (var collect in Collectors) collect.UpdateData();
        }

        public IEnumerator GetEnumerator()
        {
            if (Collectors is null) Collectors = Array.Empty<AssetCollectItem>();
            return Collectors.GetEnumerator();
        }

        public override int     GetHashCode()   => GetHashCode(this);
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}