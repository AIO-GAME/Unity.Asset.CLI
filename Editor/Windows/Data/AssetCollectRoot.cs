﻿/*|============|*|
|*|Author:     |*| Star fire
|*|Date:       |*| 2023-12-01
|*|E-Mail:     |*| xinansky99@foxmail.com
|*|============|*/

using System;
using System.Collections.Generic;
using System.ComponentModel;
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

        [Description("包含资源GUID")] public bool IncludeAssetGuid;

        [Description("地址转小写")] public bool LocationToLower;

        /// <summary>
        /// 资源收集配置
        /// </summary>
        [SerializeField] public AssetCollectPackage[] Packages;

        /// <summary>
        /// 获取资源收集配置
        /// </summary>
        public static AssetCollectRoot GetOrCreate()
        {
            var objects = EHelper.IO.GetScriptableObjects<AssetCollectRoot>();
            if (objects != null && objects.Length > 0)
            {
                foreach (var asset in objects)
                {
                    if (asset is null) continue;
                    if (asset.Packages is null) asset.Packages = new AssetCollectPackage[] { };
                    return asset;
                }
            }

            var collect = CreateInstance<AssetCollectRoot>();
            collect.Packages = new AssetCollectPackage[] { };
            AssetDatabase.CreateAsset(collect, "Assets/Editor/AssetCollectRoot.asset");
            AssetDatabase.SaveAssets();
            return collect;
        }

        public void Save()
        {
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
        }
    }
}