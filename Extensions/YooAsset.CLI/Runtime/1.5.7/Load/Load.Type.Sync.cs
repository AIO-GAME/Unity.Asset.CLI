#if SUPPORT_YOOASSET

using System.Linq;
using UnityEngine.Scripting;

namespace AIO.UEngine.YooAsset
{
    partial class Proxy
    {
        [Preserve]
        private ResPackage AutoGetPackageSync(string location)
        {
            PackageDebug(LoadType.Sync, location);
            foreach (var package in Dic.Values.Where(package => package.CheckLocationValid(location)))
            {
                if (AssetSystem.IsWhite(location)) return package;

                if (package.IsNeedDownloadFromRemote(location))
                {
                    AssetSystem.LOG.Exception($"不支持同步加载远程资源 [{package.PackageName} : {location}]");
                    return null;
                }
#if UNITY_EDITOR
                AddSequenceRecord(package, package.GetAssetInfo(location));
#endif
                return package;
            }

            AssetSystem.LOG.Exception($"资源查找失败 [auto : {location}]");
            return null;
        }

        [Preserve]
        private ResPackage AutoGetPackageSync(string packageName, string location)
        {
            PackageDebug(LoadType.Sync, packageName, location);
            if (!Dic.TryGetValue(packageName, out var package))
            {
                AssetSystem.LOG.Exception($"目标资源包不存在 [{packageName} : {location}]");
                return null;
            }

            if (AssetSystem.IsWhite(location)) return package;
            if (package.IsNeedDownloadFromRemote(location))
            {
                AssetSystem.LOG.Exception($"不支持同步加载远程资源 [{package.PackageName} : {location}]");
                return null;
            }

            if (!package.CheckLocationValid(location))
            {
                AssetSystem.LOG.Exception($"[{package.PackageName} : {package.GetPackageVersion()}] 传入地址验证无效 {location}");
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