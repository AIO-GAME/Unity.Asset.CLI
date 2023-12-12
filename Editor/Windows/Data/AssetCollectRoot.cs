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
        [Description("开启地址化")] public bool EnableAddressable;

        [Description("Bundle名称唯一")] public bool UniqueBundleName;

        [Description("包含资源GUID")] public bool IncludeAssetGUID;

        [Description("地址转小写")] public bool LocationToLower;

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

        /// <summary>
        /// 获取资源收集配置
        /// </summary>
        public static AssetCollectRoot GetOrCreate()
        {
            AssetCollectRoot collect = null;
            var objects = EHelper.IO.GetScriptableObjects<AssetCollectRoot>();
            if (objects != null && objects.Length > 0)
            {
                foreach (var asset in objects)
                {
                    if (asset is null) continue;
                    if (asset.Packages is null) asset.Packages = new AssetCollectPackage[] { };
                    collect = asset;
                    break;
                }
            }

            if (collect is null)
            {
                collect = CreateInstance<AssetCollectRoot>();
                collect.Packages = new AssetCollectPackage[] { };
                AssetDatabase.CreateAsset(collect, "Assets/Editor/AssetCollectRoot.asset");
                AssetDatabase.SaveAssets();
            }


            for (var i = 0; i < collect.Packages.Length; i++)
            {
                if (collect.Packages[i] is null)
                    collect.Packages[i] = new AssetCollectPackage();
                if (collect.Packages[i].Groups is null)
                    collect.Packages[i].Groups = new AssetCollectGroup[] { };

                for (var j = 0; j < collect.Packages[i].Groups.Length; j++)
                {
                    collect.Packages[i].Groups = collect.Packages[i].Groups
                        .OrderByDescending(group => group.Name).ToArray();
                }
            }

            collect.Packages = collect.Packages.OrderByDescending(package => package.Name).ToArray();
            return collect;
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
                        if (string.IsNullOrEmpty(collect.Tags)) continue;
                        foreach (var tag in collect.Tags.Split(';')) dictionary[tag] = true;
                    }

                    if (string.IsNullOrEmpty(group.Tags)) continue;
                    foreach (var tag in group.Tags.Split(';')) dictionary[tag] = true;
                }
            }

            return dictionary.Keys.ToArray();
        }

        public string[] GetTags(string packageName, string groupName, string GUID)
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
                        if (collect.GUID != GUID) continue;
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