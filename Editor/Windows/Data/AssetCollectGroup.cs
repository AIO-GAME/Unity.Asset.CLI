/*|============|*|
|*|Author:     |*| Star fire
|*|Date:       |*| 2023-12-01
|*|E-Mail:     |*| xinansky99@foxmail.com
|*|============|*/

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using Object = UnityEngine.Object;

namespace AIO.UEditor
{
    [Serializable]
    public class AssetCollectGroup : IDisposable, IEqualityComparer<AssetCollectGroup>
    {
        /// <summary>
        /// 组名
        /// </summary>
        public string Name;

        /// <summary>
        /// 组描述
        /// </summary>
        public string Description;

        /// <summary>
        /// 资源标签 使用;分割
        /// </summary>
        public string Tags;

        /// <summary>
        /// 资源收集配置
        /// </summary>
        public AssetCollectItem[] Collectors;

        /// <summary>
        /// 获取资源收集器数量
        /// </summary>
        public int Length
        {
            get
            {
                if (Collectors is null) Collectors = Array.Empty<AssetCollectItem>();
                return Collectors.Length;
            }
        }

        /// <summary>
        /// 刷新资源
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

        public AssetCollectItem this[int index]
        {
            get => Collectors[index];
            set => Collectors[index] = value;
        }

        /// <summary>
        /// 获取资源收集器
        /// </summary>
        /// <param name="collectPath">收集器资源路径 会被转化成 GUID</param>
        /// <returns>收集器</returns>
        public AssetCollectItem GetCollector(string collectPath)
        {
            if (Collectors is null)
            {
                Collectors = Array.Empty<AssetCollectItem>();
                return null;
            }

            var guid = AssetDatabase.AssetPathToGUID(collectPath);
            return Collectors
                .Where(collectItem => collectItem != null)
                .FirstOrDefault(collectItem => collectItem.GUID == guid);
        }

        /// <summary>
        /// 获取资源收集器
        /// </summary>
        /// <param name="guid">收集器资源路径GUID</param>
        /// <returns>收集器</returns>
        public AssetCollectItem GetCollectorGUID(string guid)
        {
            if (Collectors is null)
            {
                Collectors = Array.Empty<AssetCollectItem>();
                return null;
            }

            return Collectors
                .Where(collectItem => collectItem != null)
                .FirstOrDefault(collectItem => collectItem.GUID == guid);
        }

        /// <summary>
        /// 获取所有标签
        /// </summary>
        /// <returns>标签列表</returns>
        public string[] GetTags()
        {
            var dictionary = new List<string>();
            foreach (var collect in Collectors) dictionary.AddRange(collect.GetTags());
            if (string.IsNullOrEmpty(Tags)) return dictionary.Distinct().ToArray();
            dictionary.AddRange(Tags.Split(';', ' ', ','));
            return dictionary.Distinct().ToArray();
        }

        /// <summary>
        /// 保存
        /// </summary>
        public void Save()
        {
            if (Collectors is null) Collectors = Array.Empty<AssetCollectItem>();
            foreach (var collect in Collectors) collect.UpdateData();
        }

        public void Dispose()
        {
            if (Collectors is null) Collectors = Array.Empty<AssetCollectItem>();
            foreach (var collect in Collectors) collect.Dispose();
        }

        public bool Equals(AssetCollectGroup x, AssetCollectGroup y)
        {
            if (x is null)
            {
                if (y is null) return true;
                return false;
            }

            if (y is null) return false;

            return x.GetHashCode() == y.GetHashCode();
        }

        public override int GetHashCode()
        {
            return GetHashCode(this);
        }

        public int GetHashCode(AssetCollectGroup obj)
        {
            if (obj.Equals(null)) return 0;
            unchecked
            {
                var hashCode = obj.Name.GetHashCode();
                hashCode = (hashCode * 397) ^ (!string.IsNullOrEmpty(obj.Tags) ? obj.Tags.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^
                           (!string.IsNullOrEmpty(obj.Description) ? obj.Description.GetHashCode() : 0);
                if (obj.Collectors != null)
                    hashCode = obj.Collectors.Aggregate(hashCode,
                        (current, item) => (current * 397) ^ item.GetHashCode());
                return hashCode;
            }
        }
    }
}