/*|✩ - - - - - |||
|||✩ Author:   ||| -> xi nan
|||✩ Date:     ||| -> 2023-08-15
|||✩ Document: ||| ->
|||✩ - - - - - |*/

#if SUPPORT_YOOASSET

using System;
using System.Collections;
using System.Linq;
using YooAsset;

namespace AIO.UEngine.YooAsset
{
    partial class Proxy
    {
        private static IEnumerator LoadCheckOPCo(OperationHandleBase operation, Action<bool> cb)
        {
            if (!operation.IsValid)
            {
                AssetSystem.LogError(operation.LastError);
                cb?.Invoke(false);
                yield break;
            }

            yield return operation;
            if (operation.Status != EOperationStatus.Succeed)
            {
                AssetSystem.LogError(operation.LastError);
                cb?.Invoke(false);
                yield break;
            }

            cb?.Invoke(operation.Status == EOperationStatus.Succeed);
        }

        private IEnumerator GetAutoPackageCO(string location, Action<ResPackage> cb)
        {
            if (location.EndsWith("/") || location.EndsWith("\\"))
            {
                cb.Invoke(null);
                AssetSystem.LogException($"资源查找失败 [auto : {location}]");
                yield break;
            }

            PackageDebug(LoadType.Coroutine, location);
            foreach (var package in Dic.Values.Where(package => package.CheckLocationValid(location)))
            {
                if (AssetSystem.IsWhite(location))
                {
                    cb.Invoke(package);
                    yield break;
                }

                if (package.IsNeedDownloadFromRemote(location))
                {
                    var info = package.GetAssetInfo(location);
                    if (info is null)
                    {
                        AssetSystem.LogException($"无法获取资源信息 [{package.PackageName} : {location}]");
                        cb.Invoke(null);
                        yield break;
                    }

                    var operation = CreateDownloaderOperation(package, info);
                    yield return WaitCO(operation, info);
                    if (operation.Status != EOperationStatus.Succeed)
                    {
                        AssetSystem.LogException("获取远端资源失败 [{0} : {1}] {2} -> {3}",
                            package.PackageName, package.GetPackageVersion(), location, operation.Error);
                        cb.Invoke(null);
                        yield break;
                    }
                }

                AddSequenceRecord(package, package.GetAssetInfo(location));
                cb.Invoke(package);
                yield break;
            }

            AssetSystem.LogException($"资源查找失败 [auto : {location}]");
            cb.Invoke(null);
        }

        private IEnumerator GetAutoPackageCO(string packageName, string location, Action<ResPackage> cb)
        {
            if (location.EndsWith("/") || location.EndsWith("\\"))
            {
                AssetSystem.LogException($"资源查找失败 [auto : {location}]");
                cb.Invoke(null);
                yield break;
            }

            PackageDebug(LoadType.Coroutine, packageName, location);
            if (!Dic.TryGetValue(packageName, out var package))
            {
                AssetSystem.LogException($"目标资源包不存在 [{packageName} : {location}]");
                cb.Invoke(null);
                yield break;
            }

            if (AssetSystem.IsWhite(location))
            {
                cb.Invoke(package);
                yield break;
            }

            if (package.IsNeedDownloadFromRemote(location))
            {
                var info = package.GetAssetInfo(location);
                if (info is null)
                {
                    AssetSystem.LogException($"无法获取资源信息 [{packageName} : {location}]");
                    cb.Invoke(null);
                    yield break;
                }

                var operation = CreateDownloaderOperation(package, info);
                yield return WaitCO(operation, info);
                if (operation.Status != EOperationStatus.Succeed)
                {
                    AssetSystem.LogException("资源获取失败 [{0} : {1}] {2} -> {3}",
                        package.PackageName, package.GetPackageVersion(), location, operation.Error);
                    cb.Invoke(null);
                    yield break;
                }
            }

#if UNITY_EDITOR
            AddSequenceRecord(package, package.GetAssetInfo(location));
#endif
            if (package.CheckLocationValid(location)) cb.Invoke(package);
            else
            {
                AssetSystem.LogException(
                    $"[{package.PackageName} : {package.GetPackageVersion()}] 传入地址验证无效 {location}");
                cb.Invoke(null);
            }
        }
    }
}
#endif