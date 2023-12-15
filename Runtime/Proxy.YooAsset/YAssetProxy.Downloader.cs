/*|✩ - - - - - |||
|||✩ Author:   ||| -> xi nan
|||✩ Date:     ||| -> 2023-08-22
|||✩ Document: ||| ->
|||✩ - - - - - |*/

#if SUPPORT_YOOASSET
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AIO.UEngine.YooAsset;
using UnityEngine;
using YooAsset;

namespace AIO.UEngine
{
    public partial class YAssetProxy
    {
        private class YASDownloader : IASDownloader
        {
            private IDictionary<string, YAssetPackage> Packages;

            private Dictionary<string, UpdatePackageManifestOperation> ManifestOperations;
            private Dictionary<string, UpdatePackageVersionOperation> VersionOperations;

            private Dictionary<string, PreDownloadContentOperation> PreDownloadContentOperations;
            private Dictionary<string, DownloaderOperation> DownloaderOperations;
            private Dictionary<string, ResourceDownloaderOperation> ResourceDownloaderOperations;

            public IProgressHandle Progress { get; set; }

            public YASDownloader(IDictionary<string, YAssetPackage> packages, IProgressEvent iEvent = null)
            {
                Packages = packages;
                Progress = new AProgress(iEvent);
                VersionOperations = new Dictionary<string, UpdatePackageVersionOperation>();
                ManifestOperations = new Dictionary<string, UpdatePackageManifestOperation>();
                DownloaderOperations = new Dictionary<string, DownloaderOperation>();
                PreDownloadContentOperations = new Dictionary<string, PreDownloadContentOperation>();
                ResourceDownloaderOperations = new Dictionary<string, ResourceDownloaderOperation>();
            }

            private static bool CheckNetwork()
            {
                switch (Application.internetReachability)
                {
                    default:
                    case NetworkReachability.NotReachable:
                        AssetSystem.LogError("当前网络不可用，请检查网络连接！");
                        return false;
                    case NetworkReachability.ReachableViaCarrierDataNetwork:
                        AssetSystem.Log("当前网络为移动网络，请注意流量消耗！");
                        return true;
                    case NetworkReachability.ReachableViaLocalAreaNetwork:
                        AssetSystem.Log("wifi/网线——环境！");
                        return true;
                }
            }

            public bool Flow => Packages.Count > 0;

            public void Dispose()
            {
                Packages = null;
                VersionOperations = null;
                ManifestOperations = null;
                DownloaderOperations = null;
                PreDownloadContentOperations = null;
                ResourceDownloaderOperations = null;
            }

            #region Update Package Manifest

            private void UpdatePackageManifestBegin()
            {
                AssetSystem.InvokeNotify(EASEventType.UpdatePackageManifest, string.Empty);
                foreach (var asset in Packages.Values)
                {
                    if (asset.Mode != EPlayMode.HostPlayMode) continue;
                    AssetSystem.LogFormat("向网络端请求并更新补丁清单 -> [{0} -> {1}] ", asset.Config.Name, asset.Config.Version);
                    var operation = asset.UpdatePackageManifestAsync(
                        asset.Config.Version);
                    ManifestOperations.Add(asset.Config.Name, operation);
                }
            }

            /// <summary>
            /// 向网络端请求并更新补丁清单 异步
            /// </summary>
            public IEnumerator UpdatePackageManifestCO()
            {
                if (!Flow) yield break;
                UpdatePackageManifestBegin();
                yield return WaitCO(ManifestOperations.Values);
                UpdatePackageManifestEnd();
            }

            #endregion

            #region UpdatePackageVersion

            private void UpdatePackageVersionBegin()
            {
                AssetSystem.InvokeNotify(EASEventType.UpdatePackageVersion, string.Empty);

                foreach (var asset in Packages.Values)
                {
                    if (asset.Mode != EPlayMode.HostPlayMode) continue;
                    AssetSystem.LogFormat("向网络端请求最新的资源版本 -> [{0} -> Local : {1}]", asset.PackageName,
                        asset.Config.Version);
                    var opVersion = asset.UpdatePackageVersionAsync();
                    VersionOperations.Add(asset.Config.Name, opVersion);
                }
            }

            /// <summary>
            /// 异步向网络端请求最新的资源版本
            /// </summary>
            public IEnumerator UpdatePackageVersionCO()
            {
                if (!Flow) yield break;
                UpdatePackageVersionBegin();
                yield return WaitCO(VersionOperations.Values);
                UpdatePackageVersionEnd();
            }

            #endregion

            #region DownloadAll

            public void CollectNeedAll()
            {
                if (!Flow) return;
                AssetSystem.InvokeNotify(EASEventType.BeginDownload, string.Empty);

                void OnUpdateProgress(int totalDownloadCount, int currentDownloadCount, long totalDownloadBytes,
                    long currentDownloadBytes)
                {
                    AssetSystem.InvokeDownloading(Progress);
                }

                void OnUpdateDownloadError(string filename, string error)
                {
                    AssetSystem.InvokeNotify(EASEventType.DownlandPackageFailure,
                        string.Concat(filename, ":", error));
                }

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
                        AssetSystem.LogFormat("[{0} : {1}] 无需下载更新当前资源包", asset.Config, version);
                        Packages.Remove(name);
                        continue;
                    }

                    AssetSystem.LogFormat("创建补丁下载器，准备下载更新当前资源版本所有的资源包文件 [{0} -> {1} ] 文件数量 : {2} , 包体大小 : {3}",
                        asset.Config, version,
                        operation.TotalDownloadCount, operation.TotalDownloadBytes);

                    operation.OnDownloadProgressCallback = OnUpdateProgress;
                    operation.OnDownloadErrorCallback = OnUpdateDownloadError;

                    DownloaderOperations.Add(name, operation);
                }

                foreach (var pair in DownloaderOperations) pair.Value.BeginDownload();
            }

            #endregion

            #region Download Record

            public void CollectNeedRecord(AssetSystem.SequenceRecordQueue queue)
            {
                if (queue is null || queue.Count == 0) return;

                void OnUpdateProgress(int totalDownloadCount, int currentDownloadCount, long totalDownloadBytes,
                    long currentDownloadBytes)
                {
                    AssetSystem.InvokeDownloading(Progress);
                }

                void OnUpdateDownloadError(string filename, string error)
                {
                    AssetSystem.InvokeNotify(EASEventType.DownlandPackageFailure,
                        string.Concat(filename, ":", error));
                }

                foreach (var pair in queue.ToYoo())
                {
                    AssetSystem.Log($"创建序列下载器，准备下载更新当前资源版本所有的资源包文件 [{pair.Key}]");
                    var name = pair.Key;
                    var assetInfos = pair.Value.ToArray();
                    var operation = YAssetSystem.CreateBundleDownloader(pair.Key, assetInfos,
                        AssetSystem.Parameter.LoadingMaxTimeSlice,
                        AssetSystem.Parameter.DownloadFailedTryAgain);

                    operation.OnDownloadProgressCallback = OnUpdateProgress;
                    operation.OnDownloadErrorCallback = OnUpdateDownloadError;

                    ResourceDownloaderOperations.Add(pair.Key, operation);
                }

                foreach (var pair in ResourceDownloaderOperations) pair.Value.BeginDownload();
            }

            #endregion

            #region DownloadTag

            /// <summary>
            /// 创建补丁下载器
            /// </summary>
            private void DownloaderTagBegin(params string[] tag)
            {
                if (tag is null || tag.Length == 0)
                {
#if UNITY_EDITOR
                    AssetSystem.LogError("下载标签不能为空");
#endif
                    return;
                }

                void OnUpdateProgress(int totalDownloadCount, int currentDownloadCount, long totalDownloadBytes,
                    long currentDownloadBytes)
                {
                    AssetSystem.InvokeDownloading(Progress);
                }

                void OnUpdateDownloadError(string filename, string error)
                {
                    AssetSystem.InvokeNotify(EASEventType.DownlandPackageFailure,
                        string.Concat(filename, ":", error));
                }

                AssetSystem.InvokeNotify(EASEventType.BeginDownload, string.Empty);
                foreach (var name in ManifestOperations.Keys)
                {
                    var asset = Packages[name];
                    var version = asset.Config.Version;
                    var operation = asset.CreateResourceDownloader(tag,
                        AssetSystem.Parameter.LoadingMaxTimeSlice,
                        AssetSystem.Parameter.DownloadFailedTryAgain,
                        AssetSystem.Parameter.Timeout);
                    if (operation.TotalDownloadCount <= 0)
                    {
                        AssetSystem.LogFormat("[{0} : {1}] 无需下载更新当前资源包", asset.Config, version);
                        Packages.Remove(name);
                        continue;
                    }

                    AssetSystem.LogFormat("创建补丁下载器，准备下载更新当前资源版本所有的资源包文件 [{0} -> {1} ] 文件数量 : {2} , 包体大小 : {3}",
                        asset.Config, version,
                        operation.TotalDownloadCount, operation.TotalDownloadBytes);

                    operation.OnDownloadProgressCallback = OnUpdateProgress;
                    operation.OnDownloadErrorCallback = OnUpdateDownloadError;

                    DownloaderOperations.Add(name, operation);
                }
            }

            public void CollectNeedTag(IEnumerable<string> tags)
            {
                if (!Flow) return;
                DownloaderTagBegin(tags.ToArray());
                foreach (var pair in DownloaderOperations) pair.Value.BeginDownload();
            }

            #endregion

            private static IEnumerator WaitCO(IEnumerable<AsyncOperationBase> operations)
            {
                foreach (var operation in operations)
                {
                    if (operation.Status == EOperationStatus.Processing)
                    {
                        yield return operation;
                    }
                }
            }

            public void Pause()
            {
                foreach (var operation in ResourceDownloaderOperations)
                {
                    operation.Value.PauseDownload();
                }

                foreach (var operation in DownloaderOperations)
                {
                    operation.Value.PauseDownload();
                }
            }

            public void Resume()
            {
                foreach (var operation in ResourceDownloaderOperations)
                {
                    operation.Value.ResumeDownload();
                }

                foreach (var operation in DownloaderOperations)
                {
                    operation.Value.ResumeDownload();
                }
            }

            public void Cancel()
            {
                foreach (var operation in ResourceDownloaderOperations)
                {
                    operation.Value.CancelDownload();
                }

                foreach (var operation in DownloaderOperations)
                {
                    operation.Value.CancelDownload();
                }
            }

            public Func<bool> OnWifi;

            public IEnumerator WaitCo()
            {
                yield return WaitCO(ResourceDownloaderOperations.Values);
                yield return WaitCO(DownloaderOperations.Values);
                UpdatePreDownloadContentEnd();
                DownloaderEnd();
            }

            private void UpdatePackageVersionEnd()
            {
                foreach (var pair in VersionOperations)
                {
                    var package = Packages[pair.Key];
                    switch (pair.Value.Status)
                    {
                        case EOperationStatus.Succeed: // 本地版本与网络版本不一致
                            var version = package.Config.Version;
                            if (version != pair.Value.PackageVersion)
                                package.Config.Version = pair.Value.PackageVersion;
                            break;
                        default:
                            // 如果获取远端资源版本失败，说明当前网络无连接。
                            // 在正常开始游戏之前，需要验证本地清单内容的完整性。
                            var packageVersion = package.GetPackageVersion();
                            var operation = package.PreDownloadContentAsync(packageVersion);
                            PreDownloadContentOperations.Add(pair.Key, operation);
                            break;
                    }
                }
            }

            private void UpdatePreDownloadContentEnd()
            {
                foreach (var pair in PreDownloadContentOperations.Keys.ToArray())
                {
                    if (PreDownloadContentOperations[pair].Status != EOperationStatus.Succeed)
                    {
                        AssetSystem.Log($"请检查本地网络，有新的游戏内容需要更新！-> {pair}");
                        break;
                    }

                    PreDownloadContentOperations.Remove(pair);
                }
            }

            private void UpdatePackageManifestEnd()
            {
                foreach (var pair in ManifestOperations
                             .Where(pair => pair.Value.Status != EOperationStatus.Succeed))
                {
                    AssetSystem.LogErrorFormat("[{0} -> {1} : {2}] -> {3}", pair.Key,
                        Packages[pair.Key].Config.Version,
                        pair.Value.Status, pair.Value.Error);
                    Packages.Remove(pair.Key);
                }
            }

            private void DownloaderEnd()
            {
                foreach (var pair in DownloaderOperations)
                {
                    if (pair.Value.Status == EOperationStatus.Succeed)
                    {
                        AssetSystem.LogFormat("下载资源包文件成功 -> [{0} -> {1}]", pair.Key,
                            Packages[pair.Key].Config.Version);
                        Packages.Remove(pair.Key);
                    }
                    else
                    {
                        AssetSystem.LogErrorFormat("下载资源包文件失败 -> [{0} -> {1} : {2}]", pair.Key,
                            Packages[pair.Key].Config.Version, pair.Value.Error);
                    }
                }

                AssetSystem.InvokeNotify(EASEventType.HotUpdateDownloadFinish, string.Empty);
            }
        }
    }
}

#endif