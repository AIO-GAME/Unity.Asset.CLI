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
using Object = UnityEngine.Object;

namespace AIO.UEditor
{
    /// <summary>
    /// 资源收集配置
    /// </summary>
    [Serializable]
    public class AssetCollectRoot : ScriptableObject, IDisposable
    {
        [Description("开启地址化")] public bool EnableAddressable = true;

        [Description("Bundle名称唯一")] public bool UniqueBundleName = true;

        [Description("包含资源GUID")] public bool IncludeAssetGUID;

        /// <summary>
        /// 资源收集配置
        /// </summary>
        [SerializeField] public AssetCollectPackage[] Packages;

        #region OnGUI

        /// <summary>
        /// 当前选择包下标
        /// </summary>
        public int CurrentPackageIndex;

        /// <summary>
        /// 当前选择组下标
        /// </summary>
        public int CurrentGroupIndex;

        public AssetCollectPackage CurrentPackage
        {
            get
            {
                if (Packages is null || Packages.Length == 0)
                {
                    Packages = new AssetCollectPackage[]
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
                    CurrentPackage.Groups = new AssetCollectGroup[]
                    {
                        new AssetCollectGroup()
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

        public sealed override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public AssetCollectPackage this[int index]
        {
            get => Packages[index];
            set => Packages[index] = value;
        }

        public AssetCollectPackage GetPackage(string packageName)
        {
            return Packages.Where(package => !(package is null)).FirstOrDefault(package => package.Name == packageName);
        }

        private static AssetCollectRoot _Instance;

        public static void Sort()
        {
            for (var i = 0; i < _Instance.Packages.Length; i++)
            {
                if (_Instance.Packages[i] is null)
                {
                    _Instance.Packages[i] = new AssetCollectPackage();
                    _Instance.Packages[i].Groups = Array.Empty<AssetCollectGroup>();
                    continue;
                }

                if (_Instance.Packages[i].Groups is null)
                {
                    _Instance.Packages[i].Groups = Array.Empty<AssetCollectGroup>();
                    continue;
                }

                for (var j = 0; j < _Instance.Packages[i].Groups.Length; j++)
                {
                    _Instance.Packages[i].Groups = _Instance.Packages[i].Groups
                        .OrderByDescending(group => group.Name)
                        .ToArray();
                    if (_Instance.Packages[i].Groups[j].Collectors is null ||
                        _Instance.Packages[i].Groups[j].Collectors.Length == 0)
                        continue;

                    for (var k = 0; k < _Instance.Packages[i].Groups[j].Collectors.Length; k++)
                    {
                        var collect = _Instance.Packages[i].Groups[j].Collectors[k];
                        if (string.IsNullOrEmpty(collect.CollectPath)) continue;
                        collect.PathP = AssetDatabase.LoadAssetAtPath<Object>(collect.CollectPath);
                    }

                    _Instance.Packages[i].Groups[j].Collectors = _Instance.Packages[i].Groups[j].Collectors
                        .OrderByDescending(collect => collect.CollectPath)
                        .ToArray();
                }
            }

            _Instance.Packages = _Instance.Packages.OrderByDescending(package => package.Name).ToArray();
        }

        /// <summary>
        /// 获取资源收集配置
        /// </summary>
        public static AssetCollectRoot GetOrCreate(bool isSort = true)
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

            if (isSort) Sort();
            return _Instance;
        }

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

        public void Save()
        {
            Dispose();
            if (!Equals(null)) EditorUtility.SetDirty(this);
        }

        private void OnDisable()
        {
            Save();
        }

        public void Dispose()
        {
            if (Packages is null) return;
            foreach (var package in Packages) package.Dispose();
        }
    }
}