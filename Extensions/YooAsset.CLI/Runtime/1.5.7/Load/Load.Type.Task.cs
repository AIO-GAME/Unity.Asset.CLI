#if SUPPORT_YOOASSET

using System.Linq;
using System.Threading.Tasks;
using UnityEngine.Scripting;
using YooAsset;

namespace AIO.UEngine.YooAsset
{
    partial class Proxy
    {
        [Preserve]
        private async Task<ResPackage> AutoGetPackageTask(string location)
        {
            PackageDebug(LoadType.Async, location);
            foreach (var package in Dic.Values.Where(package => package.CheckLocationValid(location)))
            {
                if (AssetSystem.IsWhite(location)) return package;

                if (package.IsNeedDownloadFromRemote(location))
                {
                    var info = package.GetAssetInfo(location);
                    if (info is null)
                    {
                        AssetSystem.LOG.Exception($"无法获取资源信息 {location}");
                        return null;
                    }

                    var operation = CreateDownloaderOperation(package, info);
                    await WaitTask(operation, info);
                    if (operation.Status != EOperationStatus.Succeed)
                    {
                        AssetSystem.LOG.Exception($"资源获取失败 [{package.PackageName} : {package.GetPackageVersion()}] {location} -> {operation.Error}");
                        return null;
                    }
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
        private async Task<ResPackage> AutoGetPackageTask(string packageName, string location)
        {
            PackageDebug(LoadType.Async, packageName, location);
            if (!Dic.TryGetValue(packageName, out var package))
            {
                AssetSystem.LOG.Exception($"目标资源包不存在 [{packageName} : {location}]");
                return null;
            }

            if (AssetSystem.IsWhite(location)) return package;

            if (package.IsNeedDownloadFromRemote(location))
            {
                var info = package.GetAssetInfo(location);
                if (info is null)
                {
                    AssetSystem.LOG.Exception($"无法获取资源信息 [{packageName} : {location}]");
                    return null;
                }

                var operation = CreateDownloaderOperation(package, info);
                await WaitTask(operation, info);
                if (operation.Status != EOperationStatus.Succeed)
                {
                    AssetSystem.LOG.Exception(
                                  $"资源获取失败 [{packageName} : {package.GetPackageVersion()}] {location} -> {operation.Error}");
                    return null;
                }
            }
#if UNITY_EDITOR
            AddSequenceRecord(package, package.GetAssetInfo(location));
#endif
            if (package.CheckLocationValid(location)) return package;

            AssetSystem.LOG.Exception($"[{packageName} : {package.GetPackageVersion()}] 传入地址验证无效 {location}");
            return null;
        }
    }
}
#endif