/*|✩ - - - - - |||
|||✩ Author:   ||| -> XINAN
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
using UnityEngine;
using YooAsset;

namespace AIO.UEngine.YooAsset
{
    internal static partial class YAssetSystem
    {
        private static IEnumerator GetAutoPackageCO(AssetInfo location, Action<YAssetPackage> cb)
        {
            yield return GetAutoPackageCO(location.Address, cb);
        }

        private static IEnumerator GetAutoPackageCO(string location, Action<YAssetPackage> cb)
        {
            if (AssetSystem.Parameter.OutputLog) Debug.LogFormat("Load Assets Coroutine : [auto : {0}]", location);
            foreach (var package in Dic.Values.Where(package => package.CheckLocationValid(location)))
            {
                if (package.IsNeedDownloadFromRemote(location))
                {
                    var info = package.GetAssetInfo(location);
                    if (info is null)
                        throw new SystemException(string.Format("无法获取资源信息 [{0} : {1}]", package.PackageName, location));
                    var operation = package.CreateBundleDownloader(info, AssetSystem.Parameter.LoadingMaxTimeSlice,
                        AssetSystem.Parameter.DownloadFailedTryAgain,
                        AssetSystem.Parameter.Timeout);
                    RegisterEvent(package.PackageName, location, operation);
                    operation.BeginDownload();
                    yield return operation;
                    switch (operation.Status)
                    {
                        case EOperationStatus.Succeed: break;
                        case EOperationStatus.None:
                        case EOperationStatus.Failed:
                        default:
                            throw new SystemException(string.Format("资源获取失败 [{0} : {1}] {2} -> {3}",
                                package.PackageName, package.GetPackageVersion(), location, operation.Error));
                    }
                }

                cb?.Invoke(package);
                yield break;
            }
#if UNITY_EDITOR
            throw new SystemException(string.Format("资源查找失败 [auto : {0}]", location));
#endif
        }

        private static IEnumerator GetAutoPackageCO(string packagename, AssetInfo location, Action<YAssetPackage> cb)
        {
            yield return GetAutoPackageCO(packagename, location.Address, cb);
        }

        private static IEnumerator GetAutoPackageCO(string packagename, string location, Action<YAssetPackage> cb)
        {
#if UNITY_EDITOR
            if (AssetSystem.Parameter.OutputLog)
                Debug.LogFormat("Load Assets Coroutine : [{0} : {1}]", packagename, location);
#endif
            if (!Dic.TryGetValue(packagename, out var package))
                throw new SystemException(string.Format("目标资源包不存在 [{0} : {1}]", packagename, location));

            if (package.IsNeedDownloadFromRemote(location))
            {
                var info = package.GetAssetInfo(location);
                if (info is null)
                    throw new SystemException(string.Format("无法获取资源信息 [{0} : {1}]", packagename, location));
                var operation = package.CreateBundleDownloader(info, AssetSystem.Parameter.LoadingMaxTimeSlice,
                    AssetSystem.Parameter.DownloadFailedTryAgain,
                    AssetSystem.Parameter.Timeout);
                RegisterEvent(package.PackageName, location, operation);
                operation.BeginDownload();
                yield return operation;
                switch (operation.Status)
                {
                    case EOperationStatus.Succeed: break;
                    case EOperationStatus.None:
                    case EOperationStatus.Failed:
                    default:
                        throw new SystemException(string.Format("资源获取失败 [{0} : {1}] {2} -> {3}",
                            package.PackageName,
                            package.GetPackageVersion(),
                            location,
                            operation.Error));
                }
            }

            if (!package.CheckLocationValid(location))
                throw new SystemException(string.Format("[{0} : {1}] 传入地址验证无效 {2}",
                    package.PackageName,
                    package.GetPackageVersion(),
                    location));

            cb?.Invoke(package);
        }

        private static YAssetPackage GetAutoPackageSync(AssetInfo location)
        {
            return GetAutoPackageSync(location.Address);
        }

        private static YAssetPackage GetAutoPackageSync(string location)
        {
#if UNITY_EDITOR
            if (AssetSystem.Parameter.OutputLog) Debug.LogFormat("Load Assets Sync : [auto : {0}]", location);
#endif
            foreach (var package in Dic.Values.Where(package => package.CheckLocationValid(location)))
            {
                if (package.IsNeedDownloadFromRemote(location))
                    throw new SystemException($"不支持同步加载远程资源 [{package.PackageName} : {location}]");

                return package;
            }
#if UNITY_EDITOR
            throw new SystemException($"资源查找失败 [auto : {location}]");
#else
            return null;
#endif
        }

        private static YAssetPackage GetAutoPackageSync(string packagename, AssetInfo location)
        {
            return GetAutoPackageSync(packagename, location.Address);
        }

        private static YAssetPackage GetAutoPackageSync(string packagename, string location)
        {
#if UNITY_EDITOR
            if (AssetSystem.Parameter.OutputLog)
                Debug.LogFormat("Load Assets Sync : [{0} : {1}]", packagename, location);
#endif
            if (!Dic.TryGetValue(packagename, out var package))
                throw new SystemException(string.Format("目标资源包不存在 [{0} : {1}]", packagename, location));

            if (package.IsNeedDownloadFromRemote(location))
                throw new SystemException(string.Format("不支持同步加载远程资源 [{0} : {1}]", package.PackageName, location));

            if (!package.CheckLocationValid(location))
                throw new SystemException(string.Format("[{0} : {1}] 传入地址验证无效 {2}", package.PackageName,
                    package.GetPackageVersion(), location));

            return package;
        }

        private static Task<YAssetPackage> GetAutoPackageTask(AssetInfo location)
        {
            return GetAutoPackageTask(location.Address);
        }

        private static async Task<YAssetPackage> GetAutoPackageTask(string location)
        {
#if UNITY_EDITOR
            if (AssetSystem.Parameter.OutputLog) Debug.LogFormat("Load Assets Async : [auto : {0}]", location);
#endif
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
                switch (operation.Status)
                {
                    case EOperationStatus.Succeed: break;
                    default:
                        throw new SystemException(string.Format("资源获取失败 [{0} : {1}] {2} -> {3}",
                            package.PackageName,
                            package.GetPackageVersion(),
                            location,
                            operation.Error));
                }

                return package;
            }
#if UNITY_EDITOR
            throw new SystemException($@"资源查找失败 [auto : {location}]");
#else
            return null;
#endif
        }

        private static Task<YAssetPackage> GetAutoPackageTask(string packagename, AssetInfo location)
        {
            return GetAutoPackageTask(packagename, location.Address);
        }

        private static async Task<YAssetPackage> GetAutoPackageTask(string packagename, string location)
        {
#if UNITY_EDITOR
            if (AssetSystem.Parameter.OutputLog)
                Debug.LogFormat("Load Assets Async : [{0} : {1}]", packagename, location);
#endif
            if (!Dic.TryGetValue(packagename, out var package))
                throw new SystemException($"目标资源包不存在 [{packagename} : {location}]");

            if (package.IsNeedDownloadFromRemote(location))
            {
                var info = package.GetAssetInfo(location);
                if (info is null) throw new SystemException($"无法获取资源信息 [{packagename} : {location}]");
                var operation = package.CreateBundleDownloader(info, AssetSystem.Parameter.LoadingMaxTimeSlice,
                    AssetSystem.Parameter.DownloadFailedTryAgain,
                    AssetSystem.Parameter.Timeout);
                RegisterEvent(package.PackageName, location, operation);
                operation.BeginDownload();
                await operation.Task;
                switch (operation.Status)
                {
                    case EOperationStatus.Succeed: break;
                    default:
                        throw new SystemException(
                            $"资源获取失败 [{package.PackageName} : {package.GetPackageVersion()}] {location} -> {operation.Error}");
                }
            }

            if (!package.CheckLocationValid(location))
                throw new SystemException(
                    $"[{package.PackageName} : {package.GetPackageVersion()}] 传入地址验证无效 {location}");

            return package;
        }

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

        private static T GetHandle<T>(string location) where T : OperationHandleBase
        {
            if (ReferenceOPHandle.ContainsKey(location)) return (T)ReferenceOPHandle[location];
            return null;
        }

        private static T GetHandle<T>(AssetInfo location) where T : OperationHandleBase
        {
            return GetHandle<T>(location.Address);
        }

        private static void AddHandle<T>(string location, T operation) where T : OperationHandleBase
        {
            if (operation is null) return;
            ReferenceOPHandle.Set(location, operation);
        }

        private static void AddHandle<T>(AssetInfo location, T operation) where T : OperationHandleBase
        {
            if (operation is null) return;
            ReferenceOPHandle.Set(location.Address, operation);
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

        public static void FreeHandle(string location)
        {
            if (!ReferenceOPHandle.TryGetValue(location, out var value)) return;
            ReleaseInternal?.Invoke(value, null);
            ReferenceOPHandle.Remove(location);
        }

        public static void FreeHandle(IEnumerable<string> locations)
        {
            foreach (var location in locations)
            {
                if (!ReferenceOPHandle.TryGetValue(location, out var value)) continue;
                ReleaseInternal?.Invoke(value, null);
                ReferenceOPHandle.Remove(location);
            }
        }

        public static void FreeHandle(IEnumerable<AssetInfo> locations)
        {
            foreach (var location in locations) FreeHandle(location.Address);
        }

        public static void FreeHandle(AssetInfo location)
        {
            FreeHandle(location.Address);
        }

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