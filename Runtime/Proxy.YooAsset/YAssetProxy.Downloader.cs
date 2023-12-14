/*|✩ - - - - - |||
|||✩ Author:   ||| -> xi nan
|||✩ Date:     ||| -> 2023-08-22
|||✩ Document: ||| ->
|||✩ - - - - - |*/

#if SUPPORT_YOOASSET
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AIO.UEngine.YooAsset;
using YooAsset;

namespace AIO.UEngine
{
    public partial class YAssetProxy
    {
        private class YASDownloader : IASDownloader
        {
            private IDictionary<string, YAssetPackage> Packages;

            private Dictionary<string, long> DownloadBytesList;

            private Dictionary<string, UpdatePackageManifestOperation> ManifestOperations;

            private Dictionary<string, DownloaderOperation> DownloaderOperations;

            private Dictionary<string, UpdatePackageVersionOperation> VersionOperations;

            private Dictionary<string, PreDownloadContentOperation> PreDownloadContentOperations;

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

                DownloadBytesList = new Dictionary<string, long>();
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

                DownloadBytesList = null;
            }

            private static async Task WaitTask(IEnumerable<AsyncOperationBase> operations)
            {
                var tasks = operations.Select(operation => operation.Task).ToArray();
#if UNITY_WEBGL
                foreach (var task in tasks) await task;
#else
                if (tasks.Length > 0) await Task.WhenAll(tasks);
#endif
            }

            private static IEnumerator WaitCO(IEnumerable<AsyncOperationBase> operations)
            {
                return operations.GetEnumerator();
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
                        asset.Config.Version,
                        AssetSystem.Parameter.AutoSaveVersion,
                        AssetSystem.Parameter.Timeout);
                    ManifestOperations.Add(asset.Config.Name, operation);
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

            /// <summary>
            /// 向网络端请求并更新补丁清单 异步
            /// </summary>
            public async Task UpdatePackageManifestTask()
            {
                if (!Flow) return;
                UpdatePackageManifestBegin();
                await WaitTask(ManifestOperations.Values);
                UpdatePackageManifestEnd();
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
                    var opVersion = asset.UpdatePackageVersionAsync(
                        AssetSystem.Parameter.AppendTimeTicks,
                        AssetSystem.Parameter.Timeout);
                    VersionOperations.Add(asset.Config.Name, opVersion);
                }
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
                foreach (var pair in PreDownloadContentOperations)
                {
                    if (pair.Value.Status != EOperationStatus.Succeed)
                    {
                        AssetSystem.Log($"请检查本地网络，有新的游戏内容需要更新！-> {pair.Key}");
                        break;
                    }

                    Packages.Remove(pair.Key);
                }
            }

            /// <summary>
            /// 异步向网络端请求最新的资源版本
            /// </summary>
            public async Task UpdatePackageVersionTask()
            {
                if (!Flow) return;
                UpdatePackageVersionBegin();
                await WaitTask(VersionOperations.Values);
                UpdatePackageVersionEnd();
                await WaitTask(PreDownloadContentOperations.Values);
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
                yield return WaitCO(PreDownloadContentOperations.Values);
            }

            #endregion

            #region Download

            /// <summary>
            /// 创建补丁下载器 异步
            /// </summary>
            private void DownloaderBegin()
            {
                AssetSystem.InvokeNotify(EASEventType.BeginDownload, string.Empty);
                DownloadBytesList.Clear();
                Progress.Total = 0;
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

                    Progress.Total += operation.TotalDownloadBytes;

                    DownloadBytesList[name] = operation.CurrentDownloadBytes;

                    AssetSystem.LogFormat("创建补丁下载器，准备下载更新当前资源版本所有的资源包文件 [{0} -> {1} ] 文件数量 : {2} , 包体大小 : {3}",
                        asset.Config, version,
                        operation.TotalDownloadCount, operation.TotalDownloadBytes);

                    void OnUpdateProgress(int totalDownloadCount, int currentDownloadCount, long totalDownloadBytes,
                        long currentDownloadBytes)
                    {
                        DownloadBytesList[name] = currentDownloadBytes;

                        AssetSystem.InvokeDownloading(Progress);
                    }

                    void OnUpdateDownloadOver(bool isSucceed)
                    {
                        if (isSucceed) AssetSystem.InvokeNotify(EASEventType.DownlandPackageSuccess, name);
                        else
                            AssetSystem.InvokeNotify(EASEventType.DownlandPackageFailure,
                                DownloaderOperations[name].Error);
                    }

                    void OnUpdateDownloadError(string filename, string error)
                    {
                        AssetSystem.InvokeNotify(EASEventType.DownlandPackageFailure,
                            string.Concat(filename, ":", error));
                    }

                    operation.OnDownloadOverCallback = OnUpdateDownloadOver;
                    operation.OnDownloadProgressCallback = OnUpdateProgress;
                    operation.OnDownloadErrorCallback = OnUpdateDownloadError;

                    DownloaderOperations.Add(name, operation);
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

            public async Task DownloadTask()
            {
                if (!Flow) return;
                DownloaderBegin();
                foreach (var pair in DownloaderOperations) pair.Value.BeginDownload();
                await WaitTask(DownloaderOperations.Values);
                DownloaderEnd();
            }

            public IEnumerator DownloadCO()
            {
                if (!Flow) yield break;
                DownloaderBegin();
                foreach (var pair in DownloaderOperations) pair.Value.BeginDownload();
                yield return WaitCO(DownloaderOperations.Values);
                DownloaderEnd();
            }

            #endregion

            #region Download Record

            private void DownloadRecordBegin(AssetSystem.SequenceRecordQueue queue)
            {
                DownloadBytesList.Clear();

                foreach (var pair in queue.ToYoo())
                {
                    AssetSystem.Log($"创建序列下载器，准备下载更新当前资源版本所有的资源包文件 [{pair.Key}]");
                    var name = pair.Key;
                    var assetInfos = pair.Value.ToArray();
                    var operation = YAssetSystem.CreateBundleDownloader(pair.Key, assetInfos,
                        AssetSystem.Parameter.LoadingMaxTimeSlice,
                        AssetSystem.Parameter.DownloadFailedTryAgain);
                    Progress.Total += operation.TotalDownloadBytes;


                    void OnUpdateProgress(int totalDownloadCount, int currentDownloadCount, long totalDownloadBytes,
                        long currentDownloadBytes)
                    {
                        DownloadBytesList[name] = currentDownloadBytes;

                        AssetSystem.InvokeDownloading(Progress);
                    }

                    void OnUpdateDownloadOver(bool isSucceed)
                    {
                        if (isSucceed) AssetSystem.InvokeNotify(EASEventType.DownlandPackageSuccess, name);
                        else
                            AssetSystem.InvokeNotify(EASEventType.DownlandPackageFailure,
                                DownloaderOperations[name].Error);
                    }

                    void OnUpdateDownloadError(string filename, string error)
                    {
                        AssetSystem.InvokeNotify(EASEventType.DownlandPackageFailure,
                            string.Concat(filename, ":", error));
                    }

                    operation.OnDownloadOverCallback = OnUpdateDownloadOver;
                    operation.OnDownloadProgressCallback = OnUpdateProgress;
                    operation.OnDownloadErrorCallback = OnUpdateDownloadError;

                    ResourceDownloaderOperations.Add(pair.Key, operation);
                }
            }

            public async Task DownloadRecordTask(AssetSystem.SequenceRecordQueue queue)
            {
                if (!Flow) return;
                DownloadRecordBegin(queue);
                foreach (var pair in ResourceDownloaderOperations) pair.Value.BeginDownload();
                await WaitTask(ResourceDownloaderOperations.Values);
            }

            public IEnumerator DownloadRecordCO(AssetSystem.SequenceRecordQueue queue)
            {
                if (queue is null) yield break;
                DownloadRecordBegin(queue);
                foreach (var pair in ResourceDownloaderOperations) pair.Value.BeginDownload();
                yield return WaitCO(ResourceDownloaderOperations.Values);
            }

            #endregion

            /// <summary>
            /// 创建补丁下载器 异步
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

                AssetSystem.InvokeNotify(EASEventType.BeginDownload, string.Empty);
                DownloadBytesList.Clear();
                Progress.Total = 0;
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

                    Progress.Total += operation.TotalDownloadBytes;

                    DownloadBytesList[name] = operation.CurrentDownloadBytes;

                    AssetSystem.LogFormat("创建补丁下载器，准备下载更新当前资源版本所有的资源包文件 [{0} -> {1} ] 文件数量 : {2} , 包体大小 : {3}",
                        asset.Config, version,
                        operation.TotalDownloadCount, operation.TotalDownloadBytes);

                    void OnUpdateProgress(int totalDownloadCount, int currentDownloadCount, long totalDownloadBytes,
                        long currentDownloadBytes)
                    {
                        DownloadBytesList[name] = currentDownloadBytes;

                        AssetSystem.InvokeDownloading(Progress);
                    }

                    void OnUpdateDownloadOver(bool isSucceed)
                    {
                        if (isSucceed) AssetSystem.InvokeNotify(EASEventType.DownlandPackageSuccess, name);
                        else
                            AssetSystem.InvokeNotify(EASEventType.DownlandPackageFailure,
                                DownloaderOperations[name].Error);
                    }

                    void OnUpdateDownloadError(string filename, string error)
                    {
                        AssetSystem.InvokeNotify(EASEventType.DownlandPackageFailure,
                            string.Concat(filename, ":", error));
                    }

                    operation.OnDownloadOverCallback = OnUpdateDownloadOver;
                    operation.OnDownloadProgressCallback = OnUpdateProgress;
                    operation.OnDownloadErrorCallback = OnUpdateDownloadError;

                    DownloaderOperations.Add(name, operation);
                }
            }

            public async Task DownloadTagTask(IEnumerable<string> tags)
            {
                if (!Flow) return;
                DownloaderTagBegin(tags.ToArray());
                foreach (var pair in DownloaderOperations) pair.Value.BeginDownload();
                await WaitTask(DownloaderOperations.Values);
                DownloaderEnd();
            }

            public IEnumerator DownloadTagCO(IEnumerable<string> tags)
            {
                if (!Flow) yield break;
                DownloaderTagBegin(tags.ToArray());
                foreach (var pair in DownloaderOperations) pair.Value.BeginDownload();
                yield return WaitCO(DownloaderOperations.Values);
                DownloaderEnd();
            }

            public async Task DownloadTagTask(string tag)
            {
                if (!Flow) return;
                DownloaderTagBegin(tag);
                foreach (var pair in DownloaderOperations) pair.Value.BeginDownload();
                await WaitTask(DownloaderOperations.Values);
                DownloaderEnd();
            }

            public IEnumerator DownloadTagCO(string tag)
            {
                if (!Flow) yield break;
                DownloaderTagBegin(tag);
                foreach (var pair in DownloaderOperations) pair.Value.BeginDownload();
                yield return WaitCO(DownloaderOperations.Values);
                DownloaderEnd();
            }
        }
    }
}

#endif