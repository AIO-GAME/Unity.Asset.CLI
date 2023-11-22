/*|✩ - - - - - |||
|||✩ Author:   ||| -> xi nan
|||✩ Date:     ||| -> 2023-08-22
|||✩ Document: ||| ->
|||✩ - - - - - |*/

#if SUPPORT_YOOASSET
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AIO.UEngine.YooAsset;
using UnityEngine;
using YooAsset;

namespace AIO.UEngine
{
    public partial class YAssetProxy
    {
        private class YASDownloader : IASDownloader
        {
            /// <summary>
            /// 总下载数量
            /// </summary>
            public int TotalDownloadCount { get; private set; }

            /// <summary>
            /// 总下载大小
            /// </summary>
            public long TotalDownloadBytes { get; private set; }

            /// <summary>
            /// 当前已经完成的下载总数量
            /// </summary>
            public int CurrentDownloadCount
            {
                get { return DownloadCountList.Sum(item => item.Value); }
            }

            /// <summary>
            /// 当前已经完成的下载总大小
            /// </summary>
            public long CurrentDownloadBytes
            {
                get { return DownloadBytesList.Sum(item => item.Value); }
            }

            /// <summary>
            /// 下载进度
            /// </summary>
            public double Progress => CurrentDownloadBytes / (double)TotalDownloadBytes;

            private Dictionary<string, UpdatePackageManifestOperation> ManifestOperations;

            private IDictionary<string, YAssetPackage> Packages;

            private Dictionary<string, DownloaderOperation> DownloaderOperations;

            private Dictionary<string, int> DownloadCountList;

            private Dictionary<string, long> DownloadBytesList;

            private Dictionary<string, UpdatePackageVersionOperation> VersionOperations;

            public YASDownloader(IDictionary<string, YAssetPackage> packages)
            {
                Packages = packages;

                VersionOperations = new Dictionary<string, UpdatePackageVersionOperation>();
                ManifestOperations = new Dictionary<string, UpdatePackageManifestOperation>();
                DownloaderOperations = new Dictionary<string, DownloaderOperation>();

                DownloadBytesList = new Dictionary<string, long>();
                DownloadCountList = new Dictionary<string, int>();
            }

            /// <summary>
            /// 创建补丁下载器 异步
            /// </summary>
            /// <returns>Ture:需要下载 False:不需要下载</returns>
            public bool CreateDownloader()
            {
                if (Packages.Count <= 0) return false;
                DownloadBytesList.Clear();
                foreach (var name in ManifestOperations.Keys)
                {
                    var asset = Packages[name];
                    if (asset.Config.IsSidePlayWithDownload) continue;
                    var version = asset.Config.Version;
                    var operation = asset.CreateResourceDownloader(
                        AssetSystem.Parameter.LoadingMaxTimeSlice,
                        AssetSystem.Parameter.DownloadFailedTryAgain,
                        AssetSystem.Parameter.Timeout);
                    if (operation.TotalDownloadCount <= 0)
                    {
                        Debug.LogFormat("[{0} : {1}] 无需下载更新当前资源包", asset.Config, version);
                        Packages.Remove(name);
                        continue;
                    }

                    TotalDownloadCount += operation.TotalDownloadCount;
                    TotalDownloadBytes += operation.TotalDownloadBytes;

                    DownloadCountList.Add(name, operation.CurrentDownloadCount);
                    DownloadBytesList.Add(name, operation.CurrentDownloadBytes);

                    Debug.LogFormat("创建补丁下载器，准备下载更新当前资源版本所有的资源包文件 [{0} -> {1} ] 文件数量 : {2} , 包体大小 : {3}",
                        asset.Config, version,
                        operation.TotalDownloadCount, operation.TotalDownloadBytes);
                    DownloaderOperations.Add(name, operation);
                }

                return Packages.Count > 0;
            }

            public async Task BeginDownload()
            {
                if (Packages.Count <= 0) return;
                AssetSystem.InvokeNotify(EASEventType.BeginDownload, string.Empty);
                var tasks = new List<Task>();
                foreach (var operation in DownloaderOperations)
                {
                    var key = operation.Key;

                    void OnUpdateProgress(int totalDownloadCount, int currentDownloadCount, long totalDownloadBytes,
                        long currentDownloadBytes)
                    {
                        DownloadCountList[key] = currentDownloadCount;
                        DownloadBytesList[key] = currentDownloadBytes;

                        AssetSystem.InvokeDownloading(this);
                    }

                    void OnUpdateDownloadOver(bool isSucceed)
                    {
                        if (isSucceed) AssetSystem.InvokeNotify(EASEventType.DownlandPackageSuccess, key);
                        else
                            AssetSystem.InvokeNotify(EASEventType.DownlandPackageFailure,
                                DownloaderOperations[key].Error);
                    }

                    operation.Value.OnDownloadOverCallback = OnUpdateDownloadOver;
                    operation.Value.OnDownloadProgressCallback = OnUpdateProgress;
                    operation.Value.OnDownloadErrorCallback = (filename, error) =>
                    {
                        var concat = string.Concat(filename, ":", error);
                        AssetSystem.InvokeNotify(EASEventType.DownlandFileFailure, concat);
                    };

                    operation.Value.BeginDownload();
                    tasks.Add(operation.Value.Task);
                }
#if UNITY_WEBGL
                foreach (var task in tasks) await task;
#else
                await Task.WhenAll(tasks);
#endif
                AssetSystem.InvokeNotify(EASEventType.HotUpdateDownloadFinish, string.Empty);
            }

            /// <summary>
            /// 向网络端请求并更新补丁清单 异步
            /// </summary>
            public async Task<bool> UpdatePackageManifestTask()
            {
                AssetSystem.InvokeNotify(EASEventType.UpdatePackageManifest, string.Empty);
                if (Packages.Count <= 0) return false;
                foreach (var asset in Packages.Values)
                {
                    var version = asset.Config.Version;
                    Debug.LogFormat("向网络端请求并更新补丁清单 -> [{0} -> {1}] ", asset.Config.Name, version);
                    var opManifest = asset.UpdatePackageManifestAsync(version,
                        AssetSystem.Parameter.AutoSaveVersion,
                        AssetSystem.Parameter.Timeout);
                    ManifestOperations.Add(asset.Config.Name, opManifest);
                    await opManifest.Task;
                    switch (opManifest.Status)
                    {
                        case EOperationStatus.Succeed:
                            break;
                        case EOperationStatus.None:
                        case EOperationStatus.Failed:
                        default:
                            Debug.LogErrorFormat("[{0} -> {1} : {2}] -> {3}", asset.Config.Name, version,
                                opManifest.Status, opManifest.Error);
                            Packages.Remove(asset.Config.Name);
                            break;
                    }
                }

                return Packages.Count > 0;
            }

            /// <summary>
            /// 异步向网络端请求最新的资源版本
            /// </summary>
            /// <returns>
            /// Ture: 有更新
            /// False: 无更新
            /// </returns>
            public async Task<bool> UpdatePackageVersionTask()
            {
                AssetSystem.InvokeNotify(EASEventType.UpdatePackageVersion, string.Empty);
                if (Packages.Count <= 0) return false;
                var tasks = new List<Task>();
                foreach (var asset in Packages.Values)
                {
                    Debug.LogFormat("向网络端请求最新的资源版本 -> [{0} -> Local : {1}]", asset.PackageName, asset.Config.Version);
                    if (asset.Mode == EPlayMode.HostPlayMode)
                    {
                        var opVersion = asset.UpdatePackageVersionAsync(
                            AssetSystem.Parameter.AppendTimeTicks,
                            AssetSystem.Parameter.Timeout);
                        VersionOperations.Add(asset.Config.Name, opVersion);
                        tasks.Add(opVersion.Task);
                    }
                }

#if UNITY_WEBGL
                if (tasks.Count > 0) { foreach (var task in tasks) await task; }
#else
                if (tasks.Count > 0) await Task.WhenAll(tasks.ToArray());
#endif
                foreach (var opVersion in VersionOperations)
                {
                    var package = Packages[opVersion.Key];
                    switch (opVersion.Value.Status)
                    {
                        case EOperationStatus.Succeed: // 本地版本与网络版本不一致
                            var version = package.Config.Version;
                            if (version != opVersion.Value.PackageVersion)
                                package.Config.Version = opVersion.Value.PackageVersion;
                            break;
                        case EOperationStatus.None:
                        case EOperationStatus.Failed:
                        default:
                            // 如果获取远端资源版本失败，说明当前网络无连接。
                            // 在正常开始游戏之前，需要验证本地清单内容的完整性。
                            var packageVersion = package.GetPackageVersion();
                            var operation = package.PreDownloadContentAsync(packageVersion);
                            await operation.Task;
                            if (operation.Status != EOperationStatus.Succeed)
                            {
                                Console.WriteLine($"请检查本地网络，有新的游戏内容需要更新！-> {opVersion.Key}");
                                break;
                            }

                            Packages.Remove(opVersion.Key);
                            break;
                    }
                }

                return Packages.Count > 0;
            }

            public void Dispose()
            {
                Packages = null;
                VersionOperations = null;
                ManifestOperations = null;
                DownloaderOperations = null;
                DownloadBytesList = null;
                DownloadCountList = null;
            }
        }
    }
}
#endif