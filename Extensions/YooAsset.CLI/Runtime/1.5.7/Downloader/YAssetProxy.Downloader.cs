#if SUPPORT_YOOASSET
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using YooAsset;

namespace AIO.UEngine.YooAsset
{
    partial class Proxy
    {
        public override IASDownloader GetDownloader(DownlandAssetEvent dEvent = default)
        {
            var packages = AssetSystem.PackageConfigs;
            if (packages is null) return new YASDownloader(null, dEvent);
            var dictionary = new Dictionary<string, ResPackage>(packages.Count);
            foreach (var config in packages)
            {
                if (string.IsNullOrEmpty(config.Name)) continue;
                if (dictionary.ContainsKey(config.Name)) continue;
                if (Dic.TryGetValue(config.Name, out var package))
                    dictionary[config.Name] = package;
            }

            return new YASDownloader(dictionary, dEvent);
        }

        #region Nested type: YASDownloader

        private class YASDownloader : AOperation, IASDownloader
        {
            /// <summary>
            ///     是否允许使用流量下载
            /// </summary>
            private bool AllowReachableCarrier;

            private readonly Dictionary<string, long> CurrentValueDict = new Dictionary<string, long>();

            private long FirstValue = -1;

            private          Dictionary<string, UpdatePackageManifestOperation> ManifestOperations;
            private          IDictionary<string, ResPackage> Packages;
            private          Dictionary<string, PreDownloadContentOperation> PreDownloadContentOperations;
            private          Dictionary<string, ResourceDownloaderOperation> ResourceDownloaderOperations;
            private readonly Dictionary<string, long> TotalValueDict = new Dictionary<string, long>();
            private          Dictionary<string, UpdatePackageVersionOperation> VersionOperations;

            public YASDownloader(IDictionary<string, ResPackage> packages, IDownlandAssetEvent iEvent)
            {
                Packages = packages;

                VersionOperations            = new Dictionary<string, UpdatePackageVersionOperation>();
                ManifestOperations           = new Dictionary<string, UpdatePackageManifestOperation>();
                PreDownloadContentOperations = new Dictionary<string, PreDownloadContentOperation>();
                ResourceDownloaderOperations = new Dictionary<string, ResourceDownloaderOperation>();

                Event                 = iEvent;
                OnNetReachableNot     = iEvent.OnNetReachableNot;
                OnNetReachableCarrier = iEvent.OnNetReachableCarrier;
                OnDiskSpaceNotEnough  = iEvent.OnDiskSpaceNotEnough;
                OnWritePermissionNot  = iEvent.OnWritePermissionNot;
                OnReadPermissionNot   = iEvent.OnReadPermissionNot;

                if (Event.OnProgress is null)
                    Event.OnProgress = info => AssetSystem.Log(info.ToString());

                if (Event.OnComplete is null)
                    Event.OnComplete = info => AssetSystem.Log(info.ToString());
            }

            #region IASDownloader Members

            public bool Flow => Packages?.Count > 0;

            #endregion

            protected override void OnDispose()
            {
                Packages                     = null;
                VersionOperations            = null;
                DownloadTags                 = null;
                ManifestOperations           = null;
                PreDownloadContentOperations = null;
                ResourceDownloaderOperations = null;
                OnError                      = null;
                OnProgress                   = null;
                OnComplete                   = null;
                OnNetReachableNot            = null;
                OnNetReachableCarrier        = null;
                OnDiskSpaceNotEnough         = null;
                OnWritePermissionNot         = null;
                OnReadPermissionNot          = null;
            }

            private bool OnWaitBegin()
            {
                CurrentValueDict.Clear();
                TotalValueDict.Clear();

                foreach (var pair in ResourceDownloaderOperations)
                {
                    TotalValueDict[pair.Key]   = pair.Value.TotalDownloadBytes;
                    CurrentValueDict[pair.Key] = pair.Value.CurrentDownloadBytes;
                }

                if (FirstValue < 0)
                {
                    FirstValue   = TotalValue = TotalValueDict.Sum(pair => pair.Value);
                    CurrentValue = CurrentValueDict.Sum(pair => pair.Value);
                }
                else
                {
                    CurrentValueDict[nameof(FirstValue)] = FirstValue - TotalValueDict.Sum(pair => pair.Value);
                }

                CurrentValue = CurrentValueDict.Sum(pair => pair.Value);
                var endValue  = TotalValue - CurrentValue;
                var diskSpace = AssetSystem.GetAvailableDiskSpace();
                if (diskSpace < endValue) // 检查磁盘空间是否足够
                {
                    State = EProgressState.Fail;
                    if (OnDiskSpaceNotEnough is null)
                        throw new SystemException(
                            $"Out of disk space : {diskSpace.ToConverseStringFileSize()} < {endValue.ToConverseStringFileSize()}");
                    AssetSystem.LogException(
                        $"Out of disk space : {diskSpace.ToConverseStringFileSize()} < {endValue.ToConverseStringFileSize()}");
                    OnDiskSpaceNotEnough.Invoke(Report);
                    return false;
                }

                foreach (var pair in ResourceDownloaderOperations.ToArray().Where(pair =>
                                                                                      pair.Value.Status !=
                                                                                      EOperationStatus.Succeed))
                {
                    pair.Value.OnStartDownloadFileCallback = OnStartDownloadFileCallback;
                    pair.Value.OnDownloadProgressCallback  = OnUpdateProgress;
                    pair.Value.OnDownloadErrorCallback     = OnDownloadError;
                    continue;

                    void OnUpdateProgress(
                        int  totalDownloadCount, int  currentDownloadCount,
                        long totalDownloadBytes, long currentDownloadBytes)
                    {
                        if (State != EProgressState.Running) return;
                        switch (Application.internetReachability)
                        {
                            case NetworkReachability.NotReachable:
                                Pause();
                                OnNetReachableNot?.Invoke(Report);
                                return;
                            case NetworkReachability.ReachableViaCarrierDataNetwork:
                                if (AllowReachableCarrier) break;
                                Pause();
                                OnNetReachableCarrier?.Invoke(Report, () =>
                                {
                                    AllowReachableCarrier = true;
                                    Resume();
                                });
                                break;
                        }

                        CurrentValueDict[pair.Key] = pair.Value.CurrentDownloadBytes;
                        CurrentValue               = CurrentValueDict.Sum(item => item.Value);
                    }
                }

                return true;
            }

            protected override async Task OnWaitAsync()
            {
                while (Application.internetReachability == NetworkReachability.NotReachable) await Task.Delay(1);

                await UpdateHeaderTask();
                CollectNeedBegin();
                if (!OnWaitBegin())
                {
                    State = EProgressState.Fail;
                    return;
                }

                foreach (var operation in ResourceDownloaderOperations.Values.Where(
                             operation => !operation.IsDone || operation.Status != EOperationStatus.Succeed))
                {
                    while (State != EProgressState.Running)
                        switch (State)
                        {
                            case EProgressState.Finish:
                            case EProgressState.Fail:
                                return;
                            case EProgressState.Cancel:
                                CurrentInfo = "Resource download : Cancel";
                                if (Event.OnError is null) throw new TaskCanceledException();
                                Event.OnError.Invoke(new TaskCanceledException());
                                return;
                            case EProgressState.Pause:
                                CurrentInfo = "Resource download : Pause";
                                await Task.Delay(100);
                                break;
                        }

                    operation.BeginDownload();
                    await operation.Task;
                    if (operation.Status == EOperationStatus.Succeed) continue;
                    State = EProgressState.Fail;
                    return;
                }

                State = EProgressState.Finish;
                await AssetSystem.CleanUnusedCacheTask();
                if (DownloadAll) AssetSystem.WhiteAll = true;
                else if (DownloadTags.Count > 0) AssetSystem.AddWhite(AssetSystem.GetAddressByTag(DownloadTags));
            }

            protected override IEnumerator OnWaitCo()
            {
                while (Application.internetReachability == NetworkReachability.NotReachable)
                    yield return new WaitForSeconds(1);

                yield return UpdateHeaderCo();
                CollectNeedBegin();
                if (!OnWaitBegin())
                {
                    State = EProgressState.Fail;
                    yield break;
                }

                foreach (var operation in ResourceDownloaderOperations.Values.Where(
                             operation => !operation.IsDone || operation.Status != EOperationStatus.Succeed))
                {
                    while (State != EProgressState.Running)
                        switch (State)
                        {
                            case EProgressState.Finish:
                            case EProgressState.Fail:
                                yield break;
                            case EProgressState.Cancel:
                                CurrentInfo = "Resource download : Cancel";
                                if (Event.OnError is null) throw new TaskCanceledException();
                                Event.OnError.Invoke(new TaskCanceledException());
                                yield break;
                            case EProgressState.Pause:
                                CurrentInfo = "Resource download : Pause";
                                yield return new WaitForSeconds(0.1f);
                                break;
                        }

                    operation.BeginDownload();
                    yield return operation;

                    if (operation.Status == EOperationStatus.Succeed) continue;
                    State = EProgressState.Fail;
                    yield break;
                }

                State = EProgressState.Finish;
                yield return AssetSystem.CleanUnusedCacheCO();
                if (DownloadAll) AssetSystem.WhiteAll = true;
                else if (DownloadTags.Count > 0) AssetSystem.AddWhite(AssetSystem.GetAddressByTag(DownloadTags));
            }

            private void OnStartDownloadFileCallback(string filename, long sizeBytes)
            {
                CurrentInfo = $"Resource download : [{filename}:{sizeBytes}]";
                AssetSystem.Log($"Resource download : [{filename}:{sizeBytes}]");
            }

            private void OnDownloadError(string filename, string error)
            {
                var ex = new SystemException($"{filename} : {error}");
                if (Event.OnError is null) throw ex;
                Event.OnError.Invoke(ex);
                AssetSystem.LogError($"Resource download failure : [{filename} -> {error}]");
            }

            protected override void OnPause()
            {
                foreach (var operation in ResourceDownloaderOperations)
                    operation.Value.PauseDownload();
            }

            protected override void OnResume()
            {
                foreach (var operation in ResourceDownloaderOperations)
                    operation.Value.ResumeDownload();
            }

            protected override void OnCancel()
            {
                foreach (var operation in ResourceDownloaderOperations)
                    operation.Value.CancelDownload();
            }

            #region Event

            Action IProgressEvent.OnBegin
            {
                get => Event.OnBegin;
                set => Event.OnBegin = value;
            }

            Action IProgressEvent.OnResume
            {
                get => Event.OnResume;
                set => Event.OnResume = value;
            }

            Action IProgressEvent.OnPause
            {
                get => Event.OnPause;
                set => Event.OnPause = value;
            }

            Action IProgressEvent.OnCancel
            {
                get => Event.OnCancel;
                set => Event.OnCancel = value;
            }

            public Action<IProgressInfo> OnProgress
            {
                get => Event.OnProgress;
                set => Event.OnProgress = value;
            }

            public Action<IProgressReport> OnComplete
            {
                get => Event.OnComplete;
                set => Event.OnComplete = value;
            }

            public Action<Exception> OnError
            {
                get => Event.OnError;
                set => Event.OnError = value;
            }

            public Action<IProgressReport>         OnNetReachableNot     { get; set; }
            public Action<IProgressReport, Action> OnNetReachableCarrier { get; set; }
            public Action<IProgressReport>         OnDiskSpaceNotEnough  { get; set; }
            public Action<IProgressReport>         OnWritePermissionNot  { get; set; }
            public Action<IProgressReport>         OnReadPermissionNot   { get; set; }

            #endregion

            #region Update Package Header

            private void UpdatePackageManifestBegin()
            {
                foreach (var asset in Packages.Values)
                {
                    if (asset.Mode != EPlayMode.HostPlayMode) continue;
                    ManifestOperations[asset.Config.Name] = asset.UpdatePackageManifestAsync(asset.Config.Version);
                }
            }

            private void UpdatePackageManifestEnd()
            {
                foreach (var pair in ManifestOperations.Where(pair => pair.Value.Status != EOperationStatus.Succeed))
                {
                    State = EProgressState.Fail;
                    Event.OnError?.Invoke(new SystemException(pair.Value.Error));
                    Packages.Remove(pair.Key);
                }
            }

            protected override void OnBegin()
            {
                DownloadTags.Clear();
                DownloadAll = false;
                PreDownloadContentOperations.Clear();
                ResourceDownloaderOperations.Clear();
            }

            /// <summary>
            ///     头信息是否已经请求
            /// </summary>
            private bool RequestedHeader;

            /// <summary>
            ///     更新资源包头信息
            /// </summary>
            private IEnumerator UpdateHeaderCo()
            {
                if (RequestedHeader) yield break;

                if (!Flow) yield break;
                UpdatePackageManifestBegin(); // 向网络端请求并更新补丁清单
                foreach (var operation in ManifestOperations.Values.ToArray()) yield return operation;
                UpdatePackageManifestEnd();

                if (!Flow) yield break; // 异步向网络端请求最新的资源版本
                UpdatePackageVersionBegin();
                foreach (var operation in VersionOperations.Values.ToArray()) yield return operation;
                UpdatePackageVersionEnd();

                if (!Flow) yield break;
                foreach (var pair in PreDownloadContentOperations)
                {
                    yield return pair.Value;
                    if (pair.Value.Status == EOperationStatus.Succeed)
                    {
                        Packages.Remove(pair.Key);
                        continue;
                    }

                    State = EProgressState.Fail;
                    AssetSystem.LogException("校验本地资源完整性失败");
                    var ex = new SystemException($"[{pair.Key} -> {pair.Value.Error}]");
                    if (Event.OnError is null) throw ex;
                    Event.OnError.Invoke(ex);
                    yield break;
                }

                RequestedHeader = true;
            }

            /// <summary>
            ///     更新资源包头信息
            /// </summary>
            private async Task UpdateHeaderTask()
            {
                if (RequestedHeader) return;

                if (!Flow) return;
                UpdatePackageManifestBegin(); // 向网络端请求并更新补丁清单
                foreach (var operation in ManifestOperations.Values.ToArray()) await operation.Task;
                UpdatePackageManifestEnd();

                if (!Flow) return;
                UpdatePackageVersionBegin(); // 异步向网络端请求最新的资源版本
                foreach (var operation in VersionOperations.Values.ToArray()) await operation.Task;
                UpdatePackageVersionEnd();

                if (!Flow) return;
                foreach (var pair in PreDownloadContentOperations)
                {
                    await pair.Value.Task;
                    if (pair.Value.Status == EOperationStatus.Succeed)
                    {
                        Packages.Remove(pair.Key);
                        continue;
                    }

                    State = EProgressState.Fail;
                    AssetSystem.LogException("校验本地资源完整性失败");
                    var ex = new SystemException($"[{pair.Key} -> {pair.Value.Error}]");
                    if (Event.OnError is null) throw ex;
                    Event.OnError.Invoke(ex);
                    return;
                }

                RequestedHeader = true;
            }

            private void UpdatePackageVersionBegin()
            {
                foreach (var asset in Packages.Values)
                {
                    if (asset.Mode != EPlayMode.HostPlayMode) continue;
                    VersionOperations[asset.Config.Name] = asset.UpdatePackageVersionAsync();
                }
            }

            private void UpdatePackageVersionEnd()
            {
                foreach (var pair in VersionOperations)
                {
                    if (!Packages.TryGetValue(pair.Key, out var asset)) continue;
                    switch (pair.Value.Status)
                    {
                        case EOperationStatus.Succeed: // 本地版本与网络版本不一致
                            if (asset.Config.Version.Equals(pair.Value.PackageVersion)) continue;
                            asset.Config.Version = pair.Value.PackageVersion; // 更新本地版本
                            break;
                        default:
                            // 如果获取远端资源版本失败，说明当前网络无连接。
                            // 在正常开始游戏之前，需要验证本地清单内容的完整性。
                            if (PreDownloadContentOperations.ContainsKey(pair.Key)) continue;
                            var operation = asset.PreDownloadContentAsync(asset.GetPackageVersion());
                            PreDownloadContentOperations[pair.Key] = operation;
                            break;
                    }
                }
            }

            #endregion

            #region Download

            private List<string> DownloadTags = new List<string>();
            private bool         DownloadAll;

            public void CollectNeedAll()
            {
                DownloadAll = true;
            }

            public void CollectNeedTag(params string[] tags)
            {
                foreach (var item in tags) DownloadTags.Add(item);
            }

            /// <summary>
            ///     创建补丁下载器
            /// </summary>
            private void CollectNeedBegin()
            {
                ResourceDownloaderOperations.Clear();
                if (DownloadAll)
                {
                    foreach (var name in ManifestOperations.Keys)
                    {
                        if (!Packages.TryGetValue(name, out var asset)) continue;
                        var key = string.Concat("ALL-", name);
                        // if (ResourceDownloaderOperations.ContainsKey(key)) continue;
                        var operation = asset.CreateResourceDownloader();
                        if (operation is null) continue;
                        if (operation.TotalDownloadBytes <= 0) continue;
                        ResourceDownloaderOperations[key] = operation;
                    }
                }
                else
                {
                    if (DownloadTags is null || DownloadTags.Count == 0) return;
                    var tags = DownloadTags.ToArray();
                    foreach (var name in ManifestOperations.Keys)
                    {
                        if (!Packages.TryGetValue(name, out var asset)) continue;
                        var key = string.Concat("Tag-", name);
                        // if (ResourceDownloaderOperations.ContainsKey(key)) continue;
                        var operation = asset.CreateBundleDownloader(asset.GetAssetInfos(tags));
                        if (operation is null) continue;
                        if (operation.TotalDownloadBytes <= 0) continue;
                        ResourceDownloaderOperations[key] = operation;
                    }
                }
            }

            #endregion
        }

        #endregion
    }
}

#endif