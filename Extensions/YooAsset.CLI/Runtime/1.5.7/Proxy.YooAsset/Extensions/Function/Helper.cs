/*|✩ - - - - - |||
|||✩ Author:   ||| -> xi nan
|||✩ Date:     ||| -> 2023-08-21
|||✩ Document: ||| ->
|||✩ - - - - - |*/

#if SUPPORT_YOOASSET
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using YooAsset;

namespace AIO.UEngine.YooAsset
{
    internal static partial class YAssetSystem
    {
        private enum LoadType
        {
            /// <summary>
            /// 同步加载
            /// </summary>
            Sync,

            /// <summary>
            /// 协程加载
            /// </summary>
            Coroutine,

            /// <summary>
            /// 异步加载
            /// </summary>
            Async
        }

        [Conditional("DEBUG")]
        private static void PackageDebug(LoadType type, string location)
        {
#if UNITY_EDITOR
            AssetSystem.Log($"Load {type.ToString()} : [auto : {location}] -> {GetAssetInfo(location)?.AssetPath}");
#else
            AssetSystem.Log("{0} : [auto : {1}]", type.ToString(), location);
#endif
        }

        [Conditional("DEBUG")]
        private static void PackageDebug(LoadType type, string packageName, string location)
        {
#if UNITY_EDITOR
            AssetSystem.Log(
                $"Load {type.ToString()} : [{packageName} : {location}] -> {GetAssetInfo(location)?.AssetPath}");
#else
            AssetSystem.Log("{0} : [{1} : {2}]", type.ToString(), packageName, location);
#endif
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static IEnumerator GetAutoPackageCO(AssetInfo location, Action<YAssetPackage> cb)
        {
            yield return GetAutoPackageCO(location.Address, cb);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static IEnumerator GetAutoPackageCO(string packageName, AssetInfo location, Action<YAssetPackage> cb)
        {
            yield return GetAutoPackageCO(packageName, location.Address, cb);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static YAssetPackage GetAutoPackageSync(AssetInfo location)
        {
            return GetAutoPackageSync(location.Address);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static YAssetPackage GetAutoPackageSync(string packageName, AssetInfo location)
        {
            return GetAutoPackageSync(packageName, location.Address);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Task<YAssetPackage> GetAutoPackageTask(AssetInfo location)
        {
            return GetAutoPackageTask(location.Address);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Task<YAssetPackage> GetAutoPackageTask(string packageName, AssetInfo location)
        {
            return GetAutoPackageTask(packageName, location.Address);
        }

        #region CO

        private static IEnumerator GetAutoPackageCO(string location, Action<YAssetPackage> cb)
        {
#if !UNITY_2019_4_OR_NEWER
            if (location.EndsWith('/') || location.EndsWith('\\'))
#else
            if (location.EndsWith("/") || location.EndsWith("\\"))
#endif
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

#if UNITY_EDITOR
                AddSequenceRecord(package, package.GetAssetInfo(location));
#endif
                cb.Invoke(package);
                yield break;
            }

            AssetSystem.LogException($"资源查找失败 [auto : {location}]");
            cb.Invoke(null);
        }

        private static IEnumerator GetAutoPackageCO(string packageName, string location, Action<YAssetPackage> cb)
        {
#if !UNITY_2019_4_OR_NEWER
            if (location.EndsWith('/') || location.EndsWith('\\'))
#else
            if (location.EndsWith("/") || location.EndsWith("\\"))
#endif
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

        #endregion

        internal static YAssetPackage GetAutoPackageSync(string location)
        {
#if !UNITY_2019_4_OR_NEWER
            if (location.EndsWith('/') || location.EndsWith('\\'))
#else
            if (location.EndsWith("/") || location.EndsWith("\\"))
#endif
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

        private static YAssetPackage GetAutoPackageSync(string packageName, string location)
        {
#if !UNITY_2019_4_OR_NEWER
            if (location.EndsWith('/') || location.EndsWith('\\'))
#else
            if (location.EndsWith("/") || location.EndsWith("\\"))
#endif
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

        private static async Task<YAssetPackage> GetAutoPackageTask(string location)
        {
#if !UNITY_2019_4_OR_NEWER
            if (location.EndsWith('/') || location.EndsWith('\\'))
#else
            if (location.EndsWith("/") || location.EndsWith("\\"))
#endif
            {
                AssetSystem.LogException($"资源查找失败 [auto : {location}]");
                return null;
            }

            PackageDebug(LoadType.Async, location);
            foreach (var package in Dic.Values.Where(package => package.CheckLocationValid(location)))
            {
                if (AssetSystem.IsWhite(location)) return package;
                if (package.IsNeedDownloadFromRemote(location))
                {
                    var info = package.GetAssetInfo(location);
                    if (info is null) throw new SystemException($"无法获取资源信息 {location}");

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

        private static async Task<YAssetPackage> GetAutoPackageTask(string packageName, string location)
        {
#if !UNITY_2019_4_OR_NEWER
            if (location.EndsWith('/') || location.EndsWith('\\'))
#else
            if (location.EndsWith("/") || location.EndsWith("\\"))
#endif
            {
                AssetSystem.LogException($"资源查找失败 [auto : {location}]");
                return null;
            }

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
                        $"资源获取失败 [{package.PackageName} : {package.GetPackageVersion()}] {location} -> {operation.Error}");
                    return null;
                }
            }
#if UNITY_EDITOR
            AddSequenceRecord(package, package.GetAssetInfo(location));
#endif
            if (package.CheckLocationValid(location)) return package;

            AssetSystem.LogException($"[{package.PackageName} : {package.GetPackageVersion()}] 传入地址验证无效 {location}");
            return null;
        }

        /// <summary>
        /// 是否已经加载
        /// </summary>
        /// <param name="location">寻址地址</param>
        /// <returns>Ture 已经加载 False 未加载</returns>
        public static bool IsAlreadyLoad(string location)
        {
            return ReferenceOPHandle.ContainsKey(location);
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

        /// <summary>
        /// 引用计数
        /// </summary>
        private static Dictionary<string, OperationHandleBase> ReferenceOPHandle { get; set; } =
            new Dictionary<string, OperationHandleBase>();

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
    }
}
#endif