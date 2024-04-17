using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine.Serialization;
using Object = UnityEngine.Object;

namespace AIO.UEditor
{
    [Serializable]
    public sealed partial class AssetCollectGroup
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
        [FormerlySerializedAs("Tags")]
        public string Tag;

        /// <summary>
        ///     资源收集配置
        /// </summary>
        public AssetCollectItem[] Collectors;

        /// <summary>
        ///     获取资源收集器数量
        /// </summary>
        public int Count
        {
            get
            {
                if (Collectors is null) Collectors = Array.Empty<AssetCollectItem>();
                return Collectors.Length;
            }
        }

        /// <summary>
        ///    获取资源标签
        /// </summary>
        public string[] Tags
        {
            get
            {
                var dictionary = new List<string>();
                foreach (var collect in Collectors) dictionary.AddRange(collect.AllTags);
                if (string.IsNullOrEmpty(Tag)) return dictionary.Distinct().ToArray();
                dictionary.AddRange(Tag.Split(';', ' ', ','));
                return dictionary.Distinct().ToArray();
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

        public void Save()
        {
            if (Collectors is null) Collectors = Array.Empty<AssetCollectItem>();
            foreach (var collect in Collectors) collect.UpdateData();
        }

        public override int GetHashCode() => (this as IEqualityComparer<AssetCollectGroup>).GetHashCode(this);
    }
}