#if SUPPORT_YOOASSET
using System.Linq;

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
        public static string GetPackageVersion(string package)
        {
            return !Dic.TryGetValue(package, out var asset) ? null : asset.GetPackageVersion();
        }

        /// <summary>
        /// 资源回收（卸载引用计数为零的资源）
        /// </summary>
        /// <param name="package">指定包名</param>
        /// <param name="isForce">是否强制回收</param>
        public static void UnloadUnusedAssets(string package, bool isForce = false)
        {
            if (!Dic.TryGetValue(package, out var asset)) return;
            asset.UnloadUnusedAssets(isForce);
        }

        /// <summary>
        /// 资源回收（卸载引用计数为零的资源）
        /// </summary>
        /// <param name="isForce">是否强制回收</param>
        public static void UnloadUnusedALLAssets(bool isForce = false)
        {
            foreach (var key in Dic.Keys.ToArray())
                Dic[key].UnloadUnusedAssets(isForce);
        }
    }
}
#endif