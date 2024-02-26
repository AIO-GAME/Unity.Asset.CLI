#if SUPPORT_YOOASSET
using UnityEngine;

namespace AIO.UEngine.YooAsset
{
    internal partial class YAssetSystem
    {
        /// <summary>
        /// 获取本地包裹的版本信息
        /// </summary>
        public static string GetPackageVersionDefault()
        {
            return DefaultPackage?.GetPackageVersion();
        }

        /// <summary>
        /// 获取本地包裹的版本信息
        /// </summary>
        public static string GetPackageVersion(string packageName)
        {
            return Dic.TryGetValue(packageName, out var asset) ? asset.GetPackageVersion() : string.Empty;
        }

        /// <summary>
        /// 资源回收（卸载引用计数为零的资源）
        /// </summary>
        /// <param name="packageName">指定包名</param>
        /// <param name="isForce">是否强制回收</param>
        public static void UnloadUnusedAssets(string packageName, bool isForce = false)
        {
            if (Dic.TryGetValue(packageName, out var value))
            {
                if (isForce)
                {
                    value.Package.ForceUnloadAllAssets();
                }
                else
                {
                    Resources.UnloadUnusedAssets();
                    value.Package.UnloadUnusedAssets();
                }
            }
        }

        /// <summary>
        /// 资源回收（卸载引用计数为零的资源）
        /// </summary>
        /// <param name="isForce">是否强制回收</param>
        public static void UnloadUnusedALLAssets(bool isForce = false)
        {
            if (isForce)
            {
                foreach (var value in Dic.Values)
                    value.Package.ForceUnloadAllAssets();
            }
            else
            {
                Resources.UnloadUnusedAssets();
                foreach (var value in Dic.Values)
                    value.Package.UnloadUnusedAssets();
            }
        }
    }
}
#endif