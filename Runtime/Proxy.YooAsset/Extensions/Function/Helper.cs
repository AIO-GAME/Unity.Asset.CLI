/*|✩ - - - - - |||
|||✩ Author:   ||| -> xi nan
|||✩ Date:     ||| -> 2023-08-21
|||✩ Document: ||| ->
|||✩ - - - - - |*/

#if SUPPORT_YOOASSET
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using YooAsset;

namespace AIO.UEngine.YooAsset
{
    internal static partial class YAssetSystem
    {
        #region CO

        private static IEnumerator GetAutoPackageCO(AssetInfo location, Action<YAssetPackage> cb)
        {
            yield return GetAutoPackageCO(location.Address, cb);
        }

        private static IEnumerator GetAutoPackageCO(string location, Action<YAssetPackage> cb)
        {
            foreach (var package in Dic.Values.Where(package => package.CheckLocationValid(location)))
            {
                if (package.IsNeedDownloadFromRemote(location))
                {
                    var info = package.GetAssetInfo(location);
                    if (info is null)
                        AssetSystem.LogException("无法获取资源信息 [{0} : {1}]", package.PackageName, location);
                    else
                    {
                        var operation = package.CreateBundleDownloader(info,
                            AssetSystem.Parameter.LoadingMaxTimeSlice,
                            AssetSystem.Parameter.DownloadFailedTryAgain,
                            AssetSystem.Parameter.Timeout);
                        RegisterEvent(package.PackageName, location, operation);
                        operation.BeginDownload();
                        yield return operation;
                        if (operation.Status != EOperationStatus.Succeed)
                        {
                            AssetSystem.LogException("获取远端资源失败 [{0} : {1}] {2} -> {3}", package.PackageName,
                                package.GetPackageVersion(), location, operation.Error);
                        }
                    }
                }

                cb?.Invoke(package);
                yield break;
            }

            cb?.Invoke(null);
            AssetSystem.LogException("资源查找失败 [auto : {0}]", location);
        }

        private static IEnumerator GetAutoPackageCO(string packagename, AssetInfo location, Action<YAssetPackage> cb)
        {
            yield return GetAutoPackageCO(packagename, location.Address, cb);
        }

        private static IEnumerator GetAutoPackageCO(string packagename, string location, Action<YAssetPackage> cb)
        {
            AssetSystem.LogFormat("Load Assets Coroutine : [{0} : {1}]", packagename, location);
            if (!Dic.TryGetValue(packagename, out var package))
            {
                AssetSystem.LogException("目标资源包不存在 [{0} : {1}]", packagename, location);
                yield break;
            }

            if (package.IsNeedDownloadFromRemote(location))
            {
                var info = package.GetAssetInfo(location);
                if (info is null) AssetSystem.LogException("无法获取资源信息 [{0} : {1}]", packagename, location);
                else
                {
                    var operation = package.CreateBundleDownloader(info,
                        AssetSystem.Parameter.LoadingMaxTimeSlice,
                        AssetSystem.Parameter.DownloadFailedTryAgain,
                        AssetSystem.Parameter.Timeout);
                    RegisterEvent(package.PackageName, location, operation);
                    operation.BeginDownload();
                    yield return operation;
                    if (operation.Status != EOperationStatus.Succeed)
                    {
                        AssetSystem.LogException("资源获取失败 [{0} : {1}] {2} -> {3}", package.PackageName,
                            package.GetPackageVersion(), location, operation.Error);
                    }
                }
            }

            if (package.CheckLocationValid(location)) cb?.Invoke(package);
            else
            {
                AssetSystem.LogException("[{0} : {1}] 传入地址验证无效 {2}", package.PackageName, package.GetPackageVersion(),
                    location);
                cb?.Invoke(null);
            }
        }

        #endregion


        private static YAssetPackage GetAutoPackageSync(AssetInfo location)
        {
            return GetAutoPackageSync(location.Address);
        }

        private static YAssetPackage GetAutoPackageSync(string location)
        {
            AssetSystem.LogFormat("Load Assets Sync : [auto : {0}]", location);
            foreach (var package in Dic.Values.Where(package => package.CheckLocationValid(location)))
            {
                if (package.IsNeedDownloadFromRemote(location))
                    AssetSystem.LogException($"不支持同步加载远程资源 [{package.PackageName} : {location}]");
                return package;
            }

            AssetSystem.LogException($"资源查找失败 [auto : {location}]");
            return null;
        }

        private static YAssetPackage GetAutoPackageSync(string packagename, AssetInfo location)
        {
            return GetAutoPackageSync(packagename, location.Address);
        }

        private static YAssetPackage GetAutoPackageSync(string packagename, string location)
        {
            AssetSystem.LogFormat("Load Assets Sync : [{0} : {1}]", packagename, location);
            if (!Dic.TryGetValue(packagename, out var package))
            {
                AssetSystem.LogException(string.Format("目标资源包不存在 [{0} : {1}]", packagename, location));
                return null;
            }

            if (package.IsNeedDownloadFromRemote(location))
                AssetSystem.LogException(string.Format("不支持同步加载远程资源 [{0} : {1}]", package.PackageName, location));

            if (!package.CheckLocationValid(location))
            {
                AssetSystem.LogException(string.Format("[{0} : {1}] 传入地址验证无效 {2}", package.PackageName,
                    package.GetPackageVersion(), location));
                return null;
            }

            return package;
        }

        private static Task<YAssetPackage> GetAutoPackageTask(AssetInfo location)
        {
            return GetAutoPackageTask(location.Address);
        }

        private static async Task<YAssetPackage> GetAutoPackageTask(string location)
        {
            AssetSystem.LogFormat("Load Assets Async : [auto : {0}]", location);
            foreach (var package in Dic.Values.Where(package => package.CheckLocationValid(location)))
            {
                if (!package.IsNeedDownloadFromRemote(location)) return package;

                var info = package.GetAssetInfo(location);
                if (info is null) throw new SystemException(string.Format("无法获取资源信息 {0}", location));
                var operation = package.CreateBundleDownloader(info,
                    AssetSystem.Parameter.LoadingMaxTimeSlice,
                    AssetSystem.Parameter.DownloadFailedTryAgain,
                    AssetSystem.Parameter.Timeout);
                RegisterEvent(package.PackageName, location, operation);
                operation.BeginDownload();
                await operation.Task;
                if (operation.Status == EOperationStatus.Succeed) return package;
                AssetSystem.LogException("资源获取失败 [{0} : {1}] {2} -> {3}", package.PackageName,
                    package.GetPackageVersion(), location, operation.Error);
                return package;
            }

            AssetSystem.LogException($@"资源查找失败 [auto : {location}]");
            return null;
        }

        private static Task<YAssetPackage> GetAutoPackageTask(string packagename, AssetInfo location)
        {
            return GetAutoPackageTask(packagename, location.Address);
        }

        private static async Task<YAssetPackage> GetAutoPackageTask(string packagename, string location)
        {
            AssetSystem.LogFormat("Load Assets Async : [{0} : {1}]", packagename, location);
            if (!Dic.TryGetValue(packagename, out var package))
            {
                AssetSystem.LogException($"目标资源包不存在 [{packagename} : {location}]");
                return null;
            }

            if (package.IsNeedDownloadFromRemote(location))
            {
                var info = package.GetAssetInfo(location);
                if (info is null) AssetSystem.LogException($"无法获取资源信息 [{packagename} : {location}]");
                else
                {
                    var operation = package.CreateBundleDownloader(info,
                        AssetSystem.Parameter.LoadingMaxTimeSlice,
                        AssetSystem.Parameter.DownloadFailedTryAgain,
                        AssetSystem.Parameter.Timeout);
                    RegisterEvent(package.PackageName, location, operation);
                    operation.BeginDownload();
                    await operation.Task;
                    if (operation.Status != EOperationStatus.Succeed)
                        AssetSystem.LogException(
                            $"资源获取失败 [{package.PackageName} : {package.GetPackageVersion()}] {location} -> {operation.Error}");
                }
            }

            if (package.CheckLocationValid(location)) return package;

            AssetSystem.LogException($"[{package.PackageName} : {package.GetPackageVersion()}] 传入地址验证无效 {location}");
            return null;
        }

        /// <summary>
        /// 引用计数
        /// </summary>
        private static Dictionary<string, OperationHandleBase> ReferenceOPHandle { get; set; } =
            new Dictionary<string, OperationHandleBase>();

        /// <summary>
        /// 是否已经加载
        /// </summary>
        /// <param name="location">寻址地址</param>
        /// <returns>Ture 已经加载 False 未加载</returns>
        public static bool IsAlreadyLoad(string location)
        {
            return ReferenceOPHandle.ContainsKey(location);
        }

        private static MethodInfo ReleaseInternal
        {
            get
            {
                if (_ReleaseInternal is null)
                {
                    _ReleaseInternal = typeof(OperationHandleBase).GetMethod("ReleaseInternal",
                        BindingFlags.Instance | BindingFlags.NonPublic);
                }

                return _ReleaseInternal;
            }
        }

        private static MethodInfo _ReleaseInternal;

        public static void Destroy()
        {
            if (!isInitialize) return;

            foreach (var item in ReferenceOPHandle.Keys.ToArray())
                ReleaseInternal?.Invoke(ReferenceOPHandle[item], null);
            ReferenceOPHandle.Clear();

            YooAssets.Destroy();
            isInitialize = false;
        }
    }
}
#endif