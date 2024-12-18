﻿#region

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

#endregion

namespace AIO.UEditor
{
    /// <summary>
    ///     资源收集配置
    /// </summary>
    [Description("资源收集配置")]
    [Serializable]
    [HelpURL("https://github.com/AIO-GAME/Unity.Asset.CLI/blob/main/.github/API_USAGE/ToolWindow.md#asset-system-%E5%B7%A5%E5%85%B7%E8%AF%B4%E6%98%8E")]
#if UNITY_2021_1_OR_NEWER
    [Icon("Packages/com.aio.cli.asset/Resources/Editor/Icon/pencils.png")]
#else
    [ScriptIcon(IconRelative = "Packages/com.aio.cli.asset/Resources/Editor/Icon/pencils.png")]
#endif
    public partial class AssetCollectRoot : ScriptableObject
    {
        private static AssetCollectRoot _Instance;

        [InspectorName("开启地址化")]
        public bool EnableAddressable = true;

        [InspectorName("Bundle名称唯一")]
        public bool UniqueBundleName = true;

        [InspectorName("包含资源GUID")]
        public bool IncludeAssetGUID;

        [InspectorName("首包打包规则")]
        public PackRule SequenceRecordPackRule = PackRule.PackGroup;

        /// <summary>
        ///     资源收集配置
        /// </summary>
        [SerializeField, InspectorName("收集包")]
        public AssetCollectPackage[] Packages;

        public enum PackRule
        {
            [InspectorName("文件路径")]
            PackSeparately,

            [InspectorName("父类文件夹路径")]
            PackDirectory,

            [InspectorName("收集器下顶级文件夹路径")]
            PackTopDirectory,

            [InspectorName("收集器路径")]
            PackCollector,

            [InspectorName("分组名称")]
            PackGroup
        }

        /// <summary>
        ///    遍历操作
        /// </summary>
        public void ForEach(Action<AssetCollectPackage> action)
        {
            if (Packages is null) return;
            foreach (var package in Packages) action?.Invoke(package);
        }

        /// <summary>
        ///     刷新资源收集配置
        /// </summary>
        public void Refresh()
        {
            foreach (var package in Packages)
            {
                if (package.Groups is null) package.Groups = Array.Empty<AssetCollectGroup>();
                foreach (var group in package.Groups)
                {
                    if (group.Collectors is null) group.Collectors = Array.Empty<AssetCollectItem>();
                    foreach (var collect in group.Collectors)
                    {
                        if (!AHelper.IO.Exists(collect.CollectPath)) continue;
                        collect.Path = AssetDatabase.LoadAssetAtPath<Object>(collect.CollectPath);
                    }
                }
            }
        }

        /// <summary>
        ///     合并所有收集器至主收集器
        /// </summary>
        public void MergeCollector(string packageName)
        {
            if (string.IsNullOrEmpty(packageName)) return;
            var collectors = new List<AssetCollectItem>();
            var tags       = new List<string>();
            foreach (var package in Packages)
            {
                if (packageName != package.Name) continue;
                foreach (var group in package.Groups)
                {
                    if (group.Collectors is null) group.Collectors = Array.Empty<AssetCollectItem>();
                    if (group.Collectors.Length == 0) continue;

                    tags.AddRange(group.Tag.Split(';'));
                    collectors.AddRange(group.Collectors);
                }

                break;
            }

            Packages = Packages.Add(new AssetCollectPackage
            {
                Name        = $"Merge_Package_{packageName}",
                Description = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"),
                Groups = new[]
                {
                    new AssetCollectGroup
                    {
                        Name        = "Merge Group",
                        Description = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"),
                        Collectors  = collectors.ToArray(),
                        Tag         = string.Join(";", tags.Distinct())
                    }
                }
            });
        }

        /// <summary>
        ///    获取或创建资源收集配置
        /// </summary>
        /// <param name="packageName">包名</param>
        /// <returns>资源收集配置</returns>
        public AssetCollectPackage GetByName(string packageName)
        {
            if (string.IsNullOrEmpty(packageName)) return null;
            return Packages.Where(package => !(package is null)).FirstOrDefault(package => package.Name == packageName);
        }

        /// <summary>
        ///   获取或创建资源收集配置
        /// </summary>
        /// <param name="packageName">包名</param>
        /// <param name="groupName">组名</param>
        /// <returns>资源收集配置</returns>
        public AssetCollectGroup GetByName(string packageName, string groupName)
        {
            if (string.IsNullOrEmpty(packageName)) return null;
            if (string.IsNullOrEmpty(groupName)) return null;
            return Packages.Where(package => !(package is null))
                           .FirstOrDefault(package => package.Name == packageName)
                           ?.Groups.Where(group => !(group is null))
                           .FirstOrDefault(group => group.Name == groupName);
        }

        /// <summary>
        ///   获取或创建资源收集配置
        /// </summary>
        /// <param name="packageName">包名</param>
        /// <param name="groupName">组名</param>
        /// <param name="collectPath">收集路径</param>
        /// <returns>资源收集配置</returns>
        public AssetCollectItem GetByName(string packageName, string groupName, string collectPath)
        {
            if (string.IsNullOrEmpty(packageName)) return null;
            if (string.IsNullOrEmpty(groupName)) return null;
            if (string.IsNullOrEmpty(collectPath)) return null;
            return Packages.Where(package => !(package is null))
                           .FirstOrDefault(package => package.Name == packageName)
                           ?.Groups.Where(group => !(group is null))
                           .FirstOrDefault(group => group.Name == groupName)
                           ?.Collectors.Where(collect => !(collect is null))
                           .FirstOrDefault(collect => collect.CollectPath == collectPath);
        }

        public string[] GetNames() { return Packages?.Select(package => package.Name).ToArray(); }

        public void Save()
        {
            if (Equals(null))
            {
                Debug.LogWarning("AssetCollectRoot is null");
                return;
            }

            for (var index = Packages.Length - 1; index >= 0; index--) Packages[index].Save();
            EditorUtility.SetDirty(this);
        }

        private void OnDisable() { Save(); }

        public void Foldout(bool isFolded = true)
        {
            if (Packages is null) return;
            foreach (var package in Packages)
            {
                if (package?.Groups is null) continue;
                foreach (var group in package.Groups)
                {
                    if (group?.Collectors is null) continue;
                    foreach (var collector in group.Collectors)
                    {
                        if (collector is null) continue;
                        collector.Folded = isFolded;
                    }
                }
            }
        }

        #region GetTags

        /// <summary>
        ///    资源标签列表
        /// </summary>
        public string[] Tags => GetTags();

        /// <summary>
        ///    获取所有标签
        /// </summary>
        public string[] GetTags()
        {
            if (Packages is null) return Array.Empty<string>();
            var list = new List<string>();
            foreach (var package in Packages)
            {
                if (package is null) continue;
                if (package.Groups is null) package.Groups = Array.Empty<AssetCollectGroup>();
                if (package.Groups.Length == 0) continue;

                foreach (var group in package.Groups)
                {
                    if (group is null) continue;
                    if (group.Collectors is null) group.Collectors = Array.Empty<AssetCollectItem>();
                    if (group.Collectors.Length == 0) continue;

                    list.AddRange(group.Collectors.Where(collect => !string.IsNullOrEmpty(collect.Tags)).SelectMany(collect => collect.Tags.Split(';')));

                    if (string.IsNullOrEmpty(group.Tag)) continue;
                    list.AddRange(group.Tag.Split(';'));
                }
            }

            return list.Distinct().ToArray();
        }

        public string[] GetTags(string packageName)
        {
            if (Packages is null) return Array.Empty<string>();
            var list = new List<string>();
            foreach (var package in Packages)
            {
                if (package.Name != packageName) continue;
                foreach (var group in package.Groups)
                {
                    list.AddRange(from collect in @group.Collectors
                                  where collect.Type == EAssetCollectItemType.MainAssetCollector
                                  where !string.IsNullOrEmpty(collect.Tags)
                                  from tag in collect.Tags.Split(';')
                                  select tag);

                    if (string.IsNullOrEmpty(group.Tag)) continue;
                    list.AddRange(group.Tag.Split(';'));
                }
            }

            return list.Distinct().ToArray();
        }

        public string[] GetTags(string packageName, string groupName)
        {
            if (Packages is null) return Array.Empty<string>();
            var list = new List<string>();
            foreach (var package in Packages)
            {
                if (package.Name != packageName) continue;
                foreach (var group in package.Groups)
                {
                    if (group.Name != groupName) continue;
                    list.AddRange(from collect in @group.Collectors
                                  where collect.Type == EAssetCollectItemType.MainAssetCollector
                                  where !string.IsNullOrEmpty(collect.Tags)
                                  from tag in collect.Tags.Split(';')
                                  select tag);

                    if (string.IsNullOrEmpty(group.Tag)) continue;
                    list.AddRange(group.Tag.Split(';'));
                }
            }

            return list.Distinct().ToArray();
        }

        public string[] GetTags(string packageName, string groupName, string collectPath)
        {
            if (Packages is null) return Array.Empty<string>();
            var list = new List<string>();
            foreach (var package in Packages)
            {
                if (package.Name != packageName) continue;
                foreach (var group in package.Groups)
                {
                    if (group.Name != groupName) continue;
                    list.AddRange(from collect in @group.Collectors
                                  where collect.Type == EAssetCollectItemType.MainAssetCollector
                                  where collect.CollectPath == collectPath
                                  where !string.IsNullOrEmpty(collect.Tags)
                                  from tag in collect.Tags.Split(';')
                                  select tag);

                    if (string.IsNullOrEmpty(group.Tag)) continue;
                    list.AddRange(group.Tag.Split(';'));
                }
            }

            return list.Distinct().ToArray();
        }

        #endregion
    }
}