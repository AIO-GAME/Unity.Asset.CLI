/*|============|*|
|*|Author:     |*| Star fire
|*|Date:       |*| 2023-12-01
|*|E-Mail:     |*| xinansky99@foxmail.com
|*|============|*/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace AIO.UEditor
{
    /// <summary>
    /// 资源收集配置
    /// </summary>
    [Description("资源收集配置")]
    [Serializable]
    [HelpURL(
        "https://github.com/AIO-GAME/Unity.Asset.CLI/blob/main/.github/API_USAGE/ToolWindow.md#asset-system-%E5%B7%A5%E5%85%B7%E8%AF%B4%E6%98%8E")]
#if UNITY_2021_1_OR_NEWER
    [Icon("Packages/com.aio.package/Resources/Editor/Setting/icon_interests.png")]
#else
    [ScriptIcon(IconRelative = "Packages/com.aio.package/Resources/Editor/Setting/icon_interests.png")]
#endif
    public class AssetCollectRoot : ScriptableObject
    {
        private static AssetCollectRoot _Instance;

        /// <summary>
        /// 获取资源包数量
        /// </summary>
        public int Length
        {
            get
            {
                if (Packages is null) Packages = Array.Empty<AssetCollectPackage>();
                return Packages.Length;
            }
        }

        /// <summary>
        /// 资源收集器排序
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
                if (Packages[i] is null)
                {
                    Packages[i] = new AssetCollectPackage();
                    Packages[i].Groups = Array.Empty<AssetCollectGroup>();
                    continue;
                }

                if (Packages[i].Groups is null)
                {
                    Packages[i].Groups = Array.Empty<AssetCollectGroup>();
                    continue;
                }

                for (var j = 0; j < Packages[i].Groups.Length; j++)
                {
                    Packages[i].Groups = Packages[i].Groups
                        .OrderByDescending(group => group.Name)
                        .ToArray();
                    if (Packages[i].Groups[j].Collectors is null ||
                        Packages[i].Groups[j].Collectors.Length == 0)
                        continue;

                    Packages[i].Groups[j].Collectors = Packages[i].Groups[j].Collectors
                        .OrderByDescending(collect => collect.CollectPath)
                        .ToArray();
                }
            }

            Packages = Packages.OrderByDescending(package => package.Name).ToArray();
        }

        /// <summary>
        /// 刷新资源收集配置
        /// </summary>
        public void Refresh()
        {
            foreach (var package in Packages)
            {
                foreach (var group in package.Groups)
                {
                    if (group.Collectors is null ||
                        group.Collectors.Length == 0)
                        continue;

                    foreach (var collect in group.Collectors)
                    {
                        if (!AHelper.IO.Exists(collect.CollectPath)) continue;
                        collect.Path = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(collect.CollectPath);
                    }
                }
            }
        }

        /// <summary>
        /// 合并所有收集器至主收集器
        /// </summary>
        public void MergeCollector(string packageName)
        {
            var collectors = new List<AssetCollectItem>();
            var tags = new List<string>();
            foreach (var package in Packages)
            {
                if (packageName != package.Name) continue;
                foreach (var group in package.Groups)
                {
                    if (group.Collectors is null ||
                        group.Collectors.Length == 0)
                        continue;

                    tags.AddRange(group.Tags.Split(';'));
                    collectors.AddRange(group.Collectors);
                }

                break;
            }

            Packages = Packages.Add(new AssetCollectPackage
            {
                Name = $"Merge_Package_{packageName}",
                Description = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"),
                Groups = new AssetCollectGroup[]
                {
                    new AssetCollectGroup()
                    {
                        Name = "Merge Group",
                        Description = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"),
                        Collectors = collectors.ToArray(),
                        Tags = string.Join(";", tags.Distinct().ToArray())
                    }
                }
            });
        }

        /// <summary>
        /// 获取资源收集配置
        /// </summary>
        public static AssetCollectRoot GetOrCreate()
        {
            if (_Instance is null)
            {
                var objects = EHelper.IO.GetScriptableObjects<AssetCollectRoot>();
                if (objects != null && objects.Length > 0)
                {
                    foreach (var asset in objects)
                    {
                        if (asset is null) continue;
                        if (asset.Packages is null)
                        {
                            asset.Packages = Array.Empty<AssetCollectPackage>();
                            _Instance = asset;
                            return _Instance;
                        }

                        _Instance = asset;
                        break;
                    }
                }

                if (_Instance is null)
                {
                    _Instance = CreateInstance<AssetCollectRoot>();
                    _Instance.Packages = new AssetCollectPackage[] { };
                    AssetDatabase.CreateAsset(_Instance, "Assets/Editor/AssetCollectRoot.asset");
                    AssetDatabase.SaveAssets();
                    return _Instance;
                }
            }

            return _Instance;
        }

        [InspectorName("开启地址化")] public bool EnableAddressable = true;

        [InspectorName("Bundle名称唯一")] public bool UniqueBundleName = true;

        [InspectorName("包含资源GUID")] public bool IncludeAssetGUID;

        /// <summary>
        /// 资源收集配置
        /// </summary>
        [InspectorName("收集包")] [SerializeField]
        public AssetCollectPackage[] Packages;

        public AssetCollectPackage this[int index]
        {
            get => Packages[index];
            set => Packages[index] = value;
        }

        public AssetCollectPackage GetPackage(string packageName)
        {
            return Packages.Where(package => !(package is null)).FirstOrDefault(package => package.Name == packageName);
        }

        public string[] GetPackageNames()
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
        /// 当前选择包下标
        /// </summary>
        [HideInInspector] public int CurrentPackageIndex;

        /// <summary>
        /// 当前选择组下标
        /// </summary>
        [HideInInspector] public int CurrentGroupIndex;

        public AssetCollectPackage CurrentPackage
        {
            get
            {
                if (Packages is null || Packages.Length == 0)
                {
                    Packages = new[]
                    {
                        new AssetCollectPackage()
                        {
                            Name = "Default Package",
                            Description = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"),
                        }
                    };
                    CurrentPackageIndex = 0;
                }
                else if (CurrentPackageIndex < 0)
                {
                    CurrentPackageIndex = 0;
                }
                else if (Packages.Length <= CurrentPackageIndex)
                {
                    CurrentPackageIndex = Packages.Length - 1;
                }

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
                            Name = "Default Group",
                            Description = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"),
                            Collectors = Array.Empty<AssetCollectItem>()
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

        public bool IsPackageValid()
        {
            if (Packages is null)
            {
                Packages = Array.Empty<AssetCollectPackage>();
                return false;
            }

            return Packages.Length > 0;
        }

        public bool IsGroupValid()
        {
            if (!IsPackageValid()) return false;

            if (CurrentPackageIndex < 0) CurrentPackageIndex = 0;
            if (Packages.Length <= CurrentPackageIndex) CurrentPackageIndex = 0;

            if (Packages[CurrentPackageIndex].Groups is null)
            {
                Packages[CurrentPackageIndex].Groups = Array.Empty<AssetCollectGroup>();
                return false;
            }

            return Packages[CurrentPackageIndex].Groups.Length > 0;
        }

        public bool IsCollectValid()
        {
            if (!IsGroupValid()) return false;

            if (CurrentGroupIndex < 0) CurrentGroupIndex = 0;
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
                foreach (var group in package.Groups)
                {
                    list.AddRange(group.Collectors.Where(collect => !string.IsNullOrEmpty(collect.Tags))
                        .SelectMany(collect => collect.Tags.Split(';')));

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

        public void FoldoutOff()
        {
            if (Packages is null) return;
            for (var i = 0; i < Packages.Length; i++)
            {
                if (Packages[i] is null) continue;
                if (Packages[i].Groups is null) continue;
                for (var j = 0; j < Packages[i].Groups.Length; j++)
                {
                    if (Packages[i].Groups[j] is null) continue;
                    if (Packages[i].Groups[j].Collectors is null) continue;
                    for (var k = 0; k < Packages[i].Groups[j].Collectors.Length; k++)
                    {
                        if (Packages[i].Groups[j].Collectors[k] is null) continue;
                        Packages[i].Groups[j].Collectors[k].Folded = false;
                    }
                }
            }
        }

        public void FoldoutOn()
        {
            if (Packages is null) return;
            for (var i = 0; i < Packages.Length; i++)
            {
                if (Packages[i] is null) continue;
                if (Packages[i].Groups is null) continue;
                for (var j = 0; j < Packages[i].Groups.Length; j++)
                {
                    if (Packages[i].Groups[j] is null) continue;
                    if (Packages[i].Groups[j].Collectors is null) continue;
                    for (var k = 0; k < Packages[i].Groups[j].Collectors.Length; k++)
                    {
                        if (Packages[i].Groups[j].Collectors[k] is null) continue;
                        Packages[i].Groups[j].Collectors[k].Folded = true;
                    }
                }
            }
        }

        [MenuItem("Assets/获取资源可寻址路径", true, 1000)]
        private static bool IsSelectAsset()
        {
            return Selection.activeObject != null &&
                   AssetDatabase.Contains(Selection.activeObject) &&
                   !AssetDatabase.IsValidFolder(AssetDatabase.GetAssetPath(Selection.activeObject)); // 不能是文件夹
        }

        /// <summary>
        /// 根据资源路径查找资源可寻址路径
        /// </summary>
        /// <param name="assetPath">资源路径</param>
        /// <param name="limitPackage">限制包名 只查找指定包资源 空则忽略</param>
        /// <returns>
        /// Item1 包名
        /// Item2 组名
        /// Item3 可寻址路径
        /// </returns>
        public static Tuple<string, string, string> AssetToAddress(string assetPath, string limitPackage = "")
        {
            if (string.IsNullOrEmpty(assetPath))
                return new Tuple<string, string, string>(string.Empty, string.Empty, string.Empty);
            var root = GetOrCreate();
            foreach (var package in root.Packages)
            {
                if (package is null) continue;
                if (!string.IsNullOrEmpty(limitPackage))
                {
                    if (limitPackage != package.Name) continue;
                }

                foreach (var group in package.Groups)
                {
                    if (group is null) continue;
                    foreach (var item in group.Collectors)
                    {
                        if (item is null) continue;
                        if (item.Type != EAssetCollectItemType.MainAssetCollector) continue;
                        if (!assetPath.StartsWith(item.CollectPath)) continue;
                        var address = item.GetAddress(assetPath);
                        if (string.IsNullOrEmpty(address)) continue;
                        return new Tuple<string, string, string>(package.Name, group.Name, address);
                    }
                }
            }

            return new Tuple<string, string, string>(string.Empty, string.Empty, string.Empty);
        }

        public static Tuple<string, string, string> GUIDToAddress(string guid)
        {
            if (string.IsNullOrEmpty(guid))
                return new Tuple<string, string, string>(string.Empty, string.Empty, string.Empty);
            var assetPath = AssetDatabase.GUIDToAssetPath(guid);
            var root = GetOrCreate();
            foreach (var package in root.Packages)
            {
                if (package is null) continue;
                foreach (var group in package.Groups)
                {
                    if (group is null) continue;
                    foreach (var item in group.Collectors)
                    {
                        if (item is null) continue;
                        if (item.Type != EAssetCollectItemType.MainAssetCollector) continue;
                        if (!assetPath.StartsWith(item.CollectPath)) continue;
                        // 是否是否被过滤
                        var address = item.GetAddress(assetPath);
                        if (string.IsNullOrEmpty(address)) continue;
                        return new Tuple<string, string, string>(package.Name, group.Name, address);
                    }
                }
            }

            return new Tuple<string, string, string>(string.Empty, string.Empty, string.Empty);
        }

        /// <summary>
        /// 根据GUID查找资源可寻址路径
        /// </summary>
        [MenuItem("Assets/获取资源可寻址路径", false, 1000)]
        private static void FindAssetLocal()
        {
            var obj = Selection.activeObject;
            var path = AssetDatabase.GetAssetPath(obj);
            var guid = AssetDatabase.AssetPathToGUID(path);
            if (string.IsNullOrEmpty(guid)) return;
            var list = (from package in GetOrCreate().Packages
                where !(package is null)
                from @group in package.Groups
                where !(@group is null)
                from item in @group.Collectors
                where !(item is null)
                where item.Type == EAssetCollectItemType.MainAssetCollector
                where path.StartsWith(item.CollectPath)
                let address = item.GetAddress(path)
                where !string.IsNullOrEmpty(address)
                select (package.Name, @group.Name, address)).ToList();

            if (list.Count == 0) Debug.Log($"未找到资源 [{path}] 的可寻址路径");
            else
            {
                var str = string.Join("\n", list.Select(tuple =>
                    $"\nPackage : {tuple.Item1}\nGroup   : {tuple.Item2}\nAddress : {tuple.Item3}\n"));
                Debug.Log($"查找资源 [{path}] 的可寻址路径:\n{str}");
            }
        }
    }
}