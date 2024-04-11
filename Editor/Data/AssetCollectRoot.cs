#region

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
    [Icon("Packages/com.aio.package/Resources/Editor/Setting/icon_interests.png")]
#else
    [ScriptIcon(IconRelative = "Packages/com.aio.package/Resources/Editor/Setting/icon_interests.png")]
#endif
    public partial class AssetCollectRoot : ScriptableObject
    {
        private static AssetCollectRoot _Instance;

        /// <summary>
        ///     资源收集器排序
        /// </summary>
        public void Sort()
        {
            if (Packages is null)
            {
                Packages = Array.Empty<AssetCollectPackage>();
                return;
            }

            for (var i = 0; i < Packages.Length; i++)
            {
                if (Packages[i] is null) Packages[i] = new AssetCollectPackage();
                if (Packages[i].Groups is null)
                {
                    Packages[i].Groups = Array.Empty<AssetCollectGroup>();
                    continue;
                }

                if (Packages[i].Groups.Length == 0) continue;
                for (var j = 0; j < Packages[i].Groups.Length; j++)
                {
                    if (Packages[i].Groups[j].Collectors is null)
                        Packages[i].Groups[j].Collectors = Array.Empty<AssetCollectItem>();

                    if (Packages[i].Groups[j].Collectors.Length == 0) continue;
                    Packages[i].Groups[j].Collectors.Sort(
                        (a, b) => string.Compare(a.CollectPath, b.CollectPath, StringComparison.CurrentCulture));
                }

                Packages[i].Groups.Sort((a, b) => string.Compare(a.Name, b.Name, StringComparison.CurrentCulture));
            }

            Packages.Sort((a, b) => string.Compare(a.Name, b.Name, StringComparison.CurrentCulture));
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
            var tags = new List<string>();
            foreach (var package in Packages)
            {
                if (packageName != package.Name) continue;
                foreach (var group in package.Groups)
                {
                    if (group.Collectors is null) group.Collectors = Array.Empty<AssetCollectItem>();
                    if (group.Collectors.Length == 0) continue;

                    tags.AddRange(group.Tags.Split(';'));
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
                        Tags        = string.Join(";", tags.Distinct())
                    }
                }
            });
        }

        [InspectorName("开启地址化")]
        public bool EnableAddressable = true;

        [InspectorName("Bundle名称唯一")]
        public bool UniqueBundleName = true;

        [InspectorName("包含资源GUID")]
        public bool IncludeAssetGUID;

        [InspectorName("首包打包规则")]
        public PackRule SequenceRecordPackRule = PackRule.PackGroup;

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
        ///     资源收集配置
        /// </summary>
        [SerializeField, InspectorName("收集包")]
        public AssetCollectPackage[] Packages;

        /// <summary>
        ///    获取或创建资源收集配置
        /// </summary>
        /// <param name="packageName">包名</param>
        /// <returns>资源收集配置</returns>
        public AssetCollectPackage GetByName(string packageName)
        {
            return Packages.
                   Where(package => !(package is null)).
                   FirstOrDefault(package => package.Name == packageName);
        }

        /// <summary>
        ///   获取或创建资源收集配置
        /// </summary>
        /// <param name="packageName">包名</param>
        /// <param name="groupName">组名</param>
        /// <returns>资源收集配置</returns>
        public AssetCollectGroup GetByName(string packageName, string groupName)
        {
            return Packages.
                   Where(package => !(package is null)).
                   FirstOrDefault(package => package.Name == packageName)?.
                   Groups.
                   Where(group => !(group is null)).
                   FirstOrDefault(group => group.Name == groupName);
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
            return Packages.
                   Where(package => !(package is null)).
                   FirstOrDefault(package => package.Name == packageName)?.
                   Groups.
                   Where(group => !(group is null)).
                   FirstOrDefault(group => group.Name == groupName)?.
                   Collectors.
                   Where(collect => !(collect is null)).
                   FirstOrDefault(collect => collect.CollectPath == collectPath);
        }

        public string[] GetNames()
        {
            return Packages?.Select(package => package.Name).ToArray();
        }

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

        private void OnDisable()
        {
            Save();
        }

        #region OnGUI

        /// <summary>
        ///     当前选择包下标
        /// </summary>
        [HideInInspector]
        public int CurrentPackageIndex;

        /// <summary>
        ///     当前选择组下标
        /// </summary>
        [HideInInspector]
        public int CurrentGroupIndex;

        public AssetCollectPackage CurrentPackage
        {
            get
            {
                if (Packages is null || Packages.Length == 0)
                {
                    Packages = new[]
                    {
                        new AssetCollectPackage
                        {
                            Name        = "Default Package",
                            Description = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss")
                        }
                    };
                    CurrentPackageIndex = 0;
                }
                else if (CurrentPackageIndex < 0)
                    CurrentPackageIndex = 0;
                else if (Packages.Length <= CurrentPackageIndex)
                    CurrentPackageIndex = Packages.Length - 1;

                return Packages[CurrentPackageIndex];
            }
        }

        public AssetCollectGroup CurrentGroup
        {
            get
            {
                if (CurrentPackage.Groups is null || CurrentPackage.Groups.Length == 0)
                {
                    CurrentPackage.Groups = new[]
                    {
                        new AssetCollectGroup
                        {
                            Name        = "Default Group",
                            Description = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"),
                            Collectors  = Array.Empty<AssetCollectItem>()
                        }
                    };
                    CurrentGroupIndex = 0;
                }
                else if (CurrentGroupIndex < 0)
                {
                    CurrentGroupIndex = 0;
                }
                else if (CurrentPackage.Groups.Length <= CurrentGroupIndex)
                {
                    CurrentGroupIndex = CurrentPackage.Groups.Length - 1;
                }

                return CurrentPackage.Groups[CurrentGroupIndex];
            }
        }

        public bool IsValidPackage()
        {
            if (Packages == null)
                Packages = Array.Empty<AssetCollectPackage>();
            if (CurrentPackageIndex >= Packages.Length)
                CurrentPackageIndex = Packages.Length - 1;
            return Packages.Length > 0;
        }

        public bool IsValidGroup()
        {
            if (!IsValidPackage()) return false;

            if (CurrentPackageIndex < 0) CurrentPackageIndex                = 0;
            if (Packages.Length <= CurrentPackageIndex) CurrentPackageIndex = 0;
            if (Packages[CurrentPackageIndex].Groups != null)
                return Packages[CurrentPackageIndex].Groups.Length > 0;

            Packages[CurrentPackageIndex].Groups = Array.Empty<AssetCollectGroup>();
            return false;
        }

        public bool IsValidCollect()
        {
            if (!IsValidGroup()) return false;

            if (CurrentGroupIndex < 0) CurrentGroupIndex                                            = 0;
            if (Packages[CurrentPackageIndex].Groups.Length <= CurrentGroupIndex) CurrentGroupIndex = 0;

            if (Packages[CurrentPackageIndex].Groups[CurrentGroupIndex].Collectors is null)
            {
                Packages[CurrentPackageIndex].Groups[CurrentGroupIndex].Collectors = Array.Empty<AssetCollectItem>();
                return false;
            }

            return Packages[CurrentPackageIndex].Groups[CurrentGroupIndex].Collectors.Length > 0;
        }

        #endregion

        #region GetTags

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

                    list.AddRange(group.Collectors.
                                        Where(collect => !string.IsNullOrEmpty(collect.Tags)).
                                        SelectMany(collect => collect.Tags.Split(';')));

                    if (string.IsNullOrEmpty(group.Tags)) continue;
                    list.AddRange(group.Tags.Split(';'));
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

                    if (string.IsNullOrEmpty(group.Tags)) continue;
                    list.AddRange(group.Tags.Split(';'));
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

                    if (string.IsNullOrEmpty(group.Tags)) continue;
                    list.AddRange(group.Tags.Split(';'));
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

                    if (string.IsNullOrEmpty(group.Tags)) continue;
                    list.AddRange(group.Tags.Split(';'));
                }
            }

            return list.Distinct().ToArray();
        }

        #endregion

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
    }
}