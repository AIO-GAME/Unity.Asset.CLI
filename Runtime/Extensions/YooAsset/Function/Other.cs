#if SUPPORT_YOOASSET
using System;
using System.Collections.Generic;
using System.Linq;
using YooAsset;

namespace AIO.UEngine.YooAsset
{
    internal partial class YAssetSystem
    {
        /// <summary>
        /// 获取本地包裹的版本信息
        /// </summary>
        public static string GetPackageVersionDefault()
        {
            return DefaultPackage.GetPackageVersion();
        }

        /// <summary>
        /// 获取本地包裹的版本信息
        /// </summary>
        public static string GetPackageVersion(string package)
        {
            if (!Dic.TryGetValue(package, out var asset)) return null;
            return asset.GetPackageVersion();
        }

        /// <summary>
        /// 资源回收（卸载引用计数为零的资源）
        /// </summary>
        public static void UnloadUnusedAssets(string package)
        {
            if (!Dic.TryGetValue(package, out var asset)) return;
            asset.UnloadUnusedAssets();
        }

        /// <summary>
        /// 资源回收（卸载引用计数为零的资源）
        /// </summary>
        public static void UnloadUnusedALLAssets()
        {
            foreach (var key in Dic.Keys.ToArray())
                Dic[key].UnloadUnusedAssets();
        }

        /// <summary>
        /// 强制回收所有资源
        /// </summary>
        public static void ForceUnloadALLAssets()
        {
            foreach (var key in Dic.Keys.ToArray())
                Dic[key].ForceUnloadAllAssets();
        }

        /// <summary>
        /// 强制回收所有资源
        /// </summary>
        public static void ForceUnloadAssets(string package)
        {
            if (!Dic.TryGetValue(package, out var asset)) return;
            asset.ForceUnloadAllAssets();
        }
    }
}
#endif