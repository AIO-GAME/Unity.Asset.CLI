using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;
using Object = UnityEngine.Object;

namespace AIO.UEditor
{
    [Serializable, Description("资源收集组")]
    public sealed partial class AssetCollectGroup
    {
        [InspectorName("是否启用")]
        public bool Enable = true;

        [InspectorName("组名称")]
        public string Name;

        [InspectorName("组描述")]
        public string Description;

        /// <remarks>使用;分割</remarks>
        [FormerlySerializedAs("Tags")]
        [InspectorName("资源标签")]
        public string Tag;

        [InspectorName("资源收集器")]
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