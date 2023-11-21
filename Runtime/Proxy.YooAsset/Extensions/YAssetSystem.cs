/***************************************************
* Copyright(C) 2021 by DefaultCompany              *
* All Rights Reserved By Author lihongliu.         *
* Author:            XiNan                         *
* Email:             1398581458@qq.com             *
* Version:           0.1                           *
* UnityVersion:      2018.4.36f1                   *
* Date:              2021-11-22                    *
* Nowtime:           18:41:14                      *
* Description:                                     *
* History:                                         *
***************************************************/

#if SUPPORT_UNITASK
using ATask = Cysharp.Threading.Tasks.UniTask;
#else
using ATask = System.Threading.Tasks.Task;
#endif

#if SUPPORT_YOOASSET
using System.Collections.Generic;
using UnityEngine;
using YooAsset;

namespace AIO.UEngine.YooAsset
{
    /// <summary>
    /// 资源加载管理器
    /// 该类只提供封装API函数
    /// </summary>
    internal static partial class YAssetSystem
    {
        private static Dictionary<string, YAssetPackage> Dic { get; set; }

        /// <summary>
        /// 配置文件
        /// </summary>
        private static AssetPakList Config { get; set; }

        /// <summary>
        /// 主包
        /// </summary>
        private static YAssetPackage DefaultPackage { get; set; }

        /// <summary>
        /// 主包
        /// </summary>
        private static string DefaultPackageName { get; set; }

        public static YAssetPackage GetPackage(in string key)
        {
            Dic.TryGetValue(key, out var value);
            return value;
        }

        private static bool isInitialize;

        public static void Initialize()
        {
            if (isInitialize == false)
            {
#if UNITY_WEBGL // 此处为了适配WX小游戏，因为WX小游戏不支持WebGL缓存
                YooAssets.SetCacheSystemDisableCacheOnWebGL();
#endif

                Dic = new Dictionary<string, YAssetPackage>(8);
                Config = new AssetPakList();
                YooAssets.Initialize();
                YooAssets.SetOperationSystemMaxTimeSlice(30);
            }

            Config.Packages.Clear();
            Dic.Clear();

            var packageConfigs = AssetSystem.PackageConfigs;
            if (packageConfigs != null)
                foreach (var package in packageConfigs)
                    Config.Packages.Add(package);

            foreach (var item in Config.Packages)
            {
                var pak = new YAssetPackage(item);
                if (pak.Config.IsDefault)
                {
                    DefaultPackage = pak;
                    DefaultPackageName = item.Name;
                }

                if (Dic.ContainsKey(item.Name))
                {
                    Debug.LogErrorFormat("Asset Package Name Repeat : {0}", item.Name);
                    continue;
                }

                Dic.Add(item.Name, pak);
                Debug.LogFormat("Asset Package Info : {0}", item);
            }

            isInitialize = true;
        }
    }
}
#endif