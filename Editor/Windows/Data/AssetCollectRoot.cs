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
    [Serializable]
    public class AssetCollectRoot : ScriptableObject
    {
        [Description("开启地址化")] public bool EnableAddressable = true;

        [Description("Bundle名称唯一")] public bool UniqueBundleName = true;

        [Description("包含资源GUID")] public bool IncludeAssetGUID;

        /// <summary>
        /// 资源收集配置
        /// </summary>
        [SerializeField] public AssetCollectPackage[] Packages;

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

        /// <summary>
        /// 获取资源收集配置
        /// </summary>
        public static AssetCollectRoot GetOrCreate()
        {
            if (_Instance) return _Instance;
            var objects = EHelper.IO.GetScriptableObjects<AssetCollectRoot>();
            if (objects != null && objects.Length > 0)
            {
                foreach (var asset in objects)
                {
                    if (asset is null) continue;
                    if (asset.Packages is null) asset.Packages = new AssetCollectPackage[] { };
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
            }


            for (var i = 0; i < _Instance.Packages.Length; i++)
            {
                if (_Instance.Packages[i] is null)
                    _Instance.Packages[i] = new AssetCollectPackage();
                if (_Instance.Packages[i].Groups is null)
                    _Instance.Packages[i].Groups = new AssetCollectGroup[] { };

                for (var j = 0; j < _Instance.Packages[i].Groups.Length; j++)
                {
                    _Instance.Packages[i].Groups = _Instance.Packages[i].Groups
                        .OrderByDescending(group => group.Name).ToArray();
                }
            }

            _Instance.Packages = _Instance.Packages.OrderByDescending(package => package.Name).ToArray();
            return _Instance;
        }

        public string[] GetTags()
        {
            var dictionary = new Dictionary<string, bool>();
            foreach (var package in Packages)
            {
                foreach (var group in package.Groups)
                {
                    foreach (var collect in group.Collectors)
                    {
                        if (string.IsNullOrEmpty(collect.Tags)) continue;
                        foreach (var tag in collect.Tags.Split(';')) dictionary[tag] = true;
                    }

                    if (string.IsNullOrEmpty(group.Tags)) continue;
                    foreach (var tag in group.Tags.Split(';')) dictionary[tag] = true;
                }
            }

            return dictionary.Keys.ToArray();
        }

        public string[] GetTags(string packageName)
        {
            var dictionary = new Dictionary<string, bool>();
            foreach (var package in Packages)
            {
                if (package.Name != packageName) continue;
                foreach (var group in package.Groups)
                {
                    foreach (var collect in group.Collectors)
                    {
                        if (collect.Type != EAssetCollectItemType.MainAssetCollector) continue;
                        if (string.IsNullOrEmpty(collect.Tags)) continue;
                        foreach (var tag in collect.Tags.Split(';')) dictionary[tag] = true;
                    }

                    if (string.IsNullOrEmpty(group.Tags)) continue;
                    foreach (var tag in group.Tags.Split(';')) dictionary[tag] = true;
                }
            }

            return dictionary.Keys.ToArray();
        }

        public string[] GetTags(string packageName, string groupName)
        {
            var dictionary = new Dictionary<string, bool>();
            foreach (var package in Packages)
            {
                if (package.Name != packageName) continue;
                foreach (var group in package.Groups)
                {
                    if (group.Name != groupName) continue;
                    foreach (var collect in group.Collectors)
                    {
                        if (collect.Type != EAssetCollectItemType.MainAssetCollector) continue;
                        if (string.IsNullOrEmpty(collect.Tags)) continue;
                        foreach (var tag in collect.Tags.Split(';')) dictionary[tag] = true;
                    }

                    if (string.IsNullOrEmpty(group.Tags)) continue;
                    foreach (var tag in group.Tags.Split(';')) dictionary[tag] = true;
                }
            }

            return dictionary.Keys.ToArray();
        }

        public string[] GetTags(string packageName, string groupName, string collectPath)
        {
            var dictionary = new Dictionary<string, bool>();
            foreach (var package in Packages)
            {
                if (package.Name != packageName) continue;
                foreach (var group in package.Groups)
                {
                    if (group.Name != groupName) continue;
                    foreach (var collect in group.Collectors)
                    {
                        if (collect.Type != EAssetCollectItemType.MainAssetCollector) continue;
                        if (collect.CollectPath != collectPath) continue;
                        if (string.IsNullOrEmpty(collect.Tags)) continue;
                        foreach (var tag in collect.Tags.Split(';')) dictionary[tag] = true;
                    }

                    if (string.IsNullOrEmpty(group.Tags)) continue;
                    foreach (var tag in group.Tags.Split(';')) dictionary[tag] = true;
                }
            }

            return dictionary.Keys.ToArray();
        }

        public void Save()
        {
            EditorUtility.SetDirty(this);
        }
    }
}