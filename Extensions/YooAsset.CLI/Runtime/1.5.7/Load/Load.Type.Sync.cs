/*|✩ - - - - - |||
|||✩ Author:   ||| -> xi nan
|||✩ Date:     ||| -> 2023-08-11
|||✩ Document: ||| ->
|||✩ - - - - - |*/

#if SUPPORT_YOOASSET

using System.Linq;
using YooAsset;

namespace AIO.UEngine.YooAsset
{
    partial class Proxy
    {
        private static bool LoadCheckOPSync(OperationHandleBase operation)
        {
            if (!operation.IsValid)
            {
                AssetSystem.LogError("操作句柄失效 -> {0}", operation.LastError);
                return false;
            }

            if (operation.Status == EOperationStatus.Failed)
            {
                AssetSystem.LogError("资源加载失败 -> {0}", operation.LastError);
                return false;
            }

            return true;
        }

        private ResPackage GetAutoPackageSync(string location)
        {
            if (location.EndsWith("/") || location.EndsWith("\\"))
            {
                AssetSystem.LogException($"资源查找失败 [auto : {location}]");
                return null;
            }

            PackageDebug(LoadType.Sync, location);
            foreach (var package in Dic.Values.Where(package => package.CheckLocationValid(location)))
            {
                if (AssetSystem.IsWhite(location)) return package;

                if (package.IsNeedDownloadFromRemote(location))
                {
                    AssetSystem.LogException($"不支持同步加载远程资源 [{package.PackageName} : {location}]");
                    return null;
                }
#if UNITY_EDITOR
                AddSequenceRecord(package, package.GetAssetInfo(location));
#endif
                return package;
            }

            AssetSystem.LogException($"资源查找失败 [auto : {location}]");
            return null;
        }

        private ResPackage GetAutoPackageSync(string packageName, string location)
        {
            if (location.EndsWith("/") || location.EndsWith("\\"))
            {
                AssetSystem.LogException($"资源查找失败 [auto : {location}]");
                return null;
            }

            PackageDebug(LoadType.Sync, packageName, location);
            if (!Dic.TryGetValue(packageName, out var package))
            {
                AssetSystem.LogException($"目标资源包不存在 [{packageName} : {location}]");
                return null;
            }

            if (AssetSystem.IsWhite(location)) return package;
            if (package.IsNeedDownloadFromRemote(location))
            {
                AssetSystem.LogException($"不支持同步加载远程资源 [{package.PackageName} : {location}]");
                return null;
            }

            if (!package.CheckLocationValid(location))
            {
                AssetSystem.LogException(
                    $"[{package.PackageName} : {package.GetPackageVersion()}] 传入地址验证无效 {location}");
                return null;
            }

#if UNITY_EDITOR
            AddSequenceRecord(package, package.GetAssetInfo(location));
#endif
            return package;
        }
    }
}
#endif