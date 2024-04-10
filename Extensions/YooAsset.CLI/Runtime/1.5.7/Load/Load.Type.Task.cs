#if SUPPORT_YOOASSET

using System.Threading.Tasks;
using YooAsset;

namespace AIO.UEngine.YooAsset
{
    partial class Proxy
    {
        private static async Task<bool> LoadCheckOPTask(OperationHandleBase operation)
        {
            if (!operation.IsValid)
            {
                AssetSystem.LogError(operation.LastError);
                return false;
            }

            await operation.Task;
            if (operation.Status != EOperationStatus.Succeed)
            {
                AssetSystem.LogError(operation.LastError);
                return false;
            }

            return true;
        }


        private async Task<ResPackage> GetAutoPackageTask(string location)
        {
            PackageDebug(LoadType.Async, location);
            foreach (var package in Dic.Values)
            {
                if (!package.CheckLocationValid(location)) continue;
                if (AssetSystem.IsWhite(location)) return package;

                if (package.IsNeedDownloadFromRemote(location))
                {
                    var info = package.GetAssetInfo(location);
                    if (info is null)
                    {
                        AssetSystem.LogException($"无法获取资源信息 {location}");
                        return null;
                    }

                    var operation = CreateDownloaderOperation(package, info);
                    await WaitTask(operation, info);
                    if (operation.Status != EOperationStatus.Succeed)
                    {
                        AssetSystem.LogException(
                            $"资源获取失败 [{package.PackageName} : {package.GetPackageVersion()}] {location} -> {operation.Error}");
                        return null;
                    }
                }

#if UNITY_EDITOR
                AddSequenceRecord(package, package.GetAssetInfo(location));
#endif
                return package;
            }

            AssetSystem.LogException($"资源查找失败 [auto : {location}]");
            return null;
        }


        private async Task<ResPackage> GetAutoPackageTask(string packageName, string location)
        {
            PackageDebug(LoadType.Async, packageName, location);
            if (!Dic.TryGetValue(packageName, out var package))
            {
                AssetSystem.LogException($"目标资源包不存在 [{packageName} : {location}]");
                return null;
            }

            if (AssetSystem.IsWhite(location)) return package;

            if (package.IsNeedDownloadFromRemote(location))
            {
                var info = package.GetAssetInfo(location);
                if (info is null)
                {
                    AssetSystem.LogException($"无法获取资源信息 [{packageName} : {location}]");
                    return null;
                }

                var operation = CreateDownloaderOperation(package, info);
                await WaitTask(operation, info);
                if (operation.Status != EOperationStatus.Succeed)
                {
                    AssetSystem.LogException(
                        $"资源获取失败 [{packageName} : {package.GetPackageVersion()}] {location} -> {operation.Error}");
                    return null;
                }
            }
#if UNITY_EDITOR
            AddSequenceRecord(package, package.GetAssetInfo(location));
#endif
            if (package.CheckLocationValid(location)) return package;

            AssetSystem.LogException($"[{packageName} : {package.GetPackageVersion()}] 传入地址验证无效 {location}");
            return null;
        }
    }
}
#endif