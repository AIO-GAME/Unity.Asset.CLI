/*|============|*|
|*|Author:     |*| Star fire
|*|Date:       |*| 2023-12-01
|*|E-Mail:     |*| xinansky99@foxmail.com
|*|============|*/

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;

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

        public void Refresh()
        {
            if (Collectors is null || Collectors.Length == 0) return;
            foreach (var collect in Collectors)
            {
                if (!AHelper.IO.Exists(collect.CollectPath)) continue;
                collect.Path = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(collect.CollectPath);
            }
        }

        public AssetCollectItem this[int index]
        {
            get => Collectors[index];
            set => Collectors[index] = value;
        }

        public AssetCollectItem GetCollector(string collectPath)
        {
            return Collectors
                .Where(collectItem => !(collectItem is null))
                .FirstOrDefault(collectItem => collectItem.CollectPath == collectPath);
        }

        public string[] GetTags()
        {
            var dictionary = new Dictionary<string, bool>();
            foreach (var collect in Collectors)
            {
                if (collect.Type != EAssetCollectItemType.MainAssetCollector) continue;
                if (string.IsNullOrEmpty(collect.Tags)) continue;
                foreach (var tag in collect.Tags.Split(';')) dictionary[tag] = true;
            }

            if (string.IsNullOrEmpty(Tags)) return dictionary.Keys.ToArray();

            foreach (var tag in Tags.Split(';')) dictionary[tag] = true;
            return dictionary.Keys.ToArray();
        }

        public void Save()
        {
            if (Collectors is null || Collectors.Length == 0) return;
            foreach (var collect in Collectors) collect.UpdateData();
        }

        public void Dispose()
        {
            if (Collectors is null || Collectors.Length == 0) return;
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