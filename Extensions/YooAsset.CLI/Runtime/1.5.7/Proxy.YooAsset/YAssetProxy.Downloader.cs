/*|✩ - - - - - |||
|||✩ Author:   ||| -> xi nan
|||✩ Date:     ||| -> 2023-08-22
|||✩ Document: ||| ->
|||✩ - - - - - |*/

#if SUPPORT_YOOASSET
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AIO.UEngine.YooAsset;
using UnityEngine;
using YooAsset;

namespace AIO.UEngine
{
    public partial class YAssetProxy
    {
        private class YASDownloader : AOperation, IASDownloader
        {
            private IDictionary<string, YAssetPackage> Packages;

            private Dictionary<string, UpdatePackageManifestOperation> ManifestOperations;
            private Dictionary<string, UpdatePackageVersionOperation> VersionOperations;

            private Dictionary<string, PreDownloadContentOperation> PreDownloadContentOperations;
            private Dictionary<string, ResourceDownloaderOperation> ResourceDownloaderOperations;

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

            public Action<IProgressReport> OnNetReachableNot { get; set; }
            public Action<IProgressReport, Action> OnNetReachableCarrier { get; set; }
            public Action<IProgressReport> OnDiskSpaceNotEnough { get; set; }
            public Action<IProgressReport> OnWritePermissionNot { get; set; }
            public Action<IProgressReport> OnReadPermissionNot { get; set; }

            #endregion

            public YASDownloader(IDictionary<string, YAssetPackage> packages, IDownlandAssetEvent iEvent)
            {
                Packages = packages;

                VersionOperations = new Dictionary<string, UpdatePackageVersionOperation>();
                ManifestOperations = new Dictionary<string, UpdatePackageManifestOperation>();
                PreDownloadContentOperations = new Dictionary<string, PreDownloadContentOperation>();
                ResourceDownloaderOperations = new Dictionary<string, ResourceDownloaderOperation>();

                Event = iEvent;
                OnNetReachableNot = iEvent.OnNetReachableNot;
                OnNetReachableCarrier = iEvent.OnNetReachableCarrier;
                OnDiskSpaceNotEnough = iEvent.OnDiskSpaceNotEnough;
                OnWritePermissionNot = iEvent.OnWritePermissionNot;
                OnReadPermissionNot = iEvent.OnReadPermissionNot;

                if (Event.OnProgress is null)
                    Event.OnProgress = info => AssetSystem.Log(info.ToString());

                if (Event.OnComplete is null)
                    Event.OnComplete = info => AssetSystem.Log(info.ToString());
            }

            /// <summary>
            /// 是否允许使用流量下载
            /// </summary>
            private bool AllowReachableCarrier;

            public bool Flow => Packages?.Count > 0;

            protected override void OnDispose()
            {
                Packages = null;
                VersionOperations = null;
                Tags = null;
                ManifestOperations = null;
                PreDownloadContentOperations = null;
                ResourceDownloaderOperations = null;
            }

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
                Tags.Clear();
                PreDownloadContentOperations.Clear();
                ResourceDownloaderOperations.Clear();
            }

            /// <summary>
            /// 更新资源包头信息
            /// </summary>
            public IEnumerator UpdateHeader()
            {
                if (!Flow) yield break;
                UpdatePackageManifestBegin(); // 向网络端请求并更新补丁清单
                foreach (var pair in ManifestOperations) yield return pair.Value;
                UpdatePackageManifestEnd();

                if (!Flow) yield break; // 异步向网络端请求最新的资源版本
                UpdatePackageVersionBegin();
                foreach (var pair in VersionOperations) yield return pair.Value;
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
                            var version = asset.Config.Version;
                            if (version != pair.Value.PackageVersion)
                                asset.Config.Version = pair.Value.PackageVersion; // 更新本地版本
                            break;
                        default:
                            // 如果获取远端资源版本失败，说明当前网络无连接。
                            // 在正常开始游戏之前，需要验证本地清单内容的完整性。
                            var packageVersion = asset.GetPackageVersion();
                            var operation = asset.PreDownloadContentAsync(packageVersion);
                            PreDownloadContentOperations.Add(pair.Key, operation);
                            break;
                    }
                }
            }

            #endregion

            #region DownloadAll

            public void CollectNeedAll()
            {
                if (!Flow) return;
                foreach (var name in ManifestOperations.Keys)
                {
                    if (!Packages.TryGetValue(name, out var asset)) continue;
                    var operation = asset.CreateResourceDownloader();
                    if (operation is null) continue;
                    if (operation.TotalDownloadCount <= 0) continue;
                    ResourceDownloaderOperations[string.Concat("ALL-", name, '-', DateTime.Now.Ticks)] = operation;
                }

                OpenDownloadAll = true;
            }

            #endregion

            #region Download

            private Dictionary<string, byte> Tags = new Dictionary<string, byte>();
            private bool OpenDownloadAll;

            /// <summary>
            /// 创建补丁下载器
            /// </summary>
            private void CollectNeedTagBegin(params string[] tags)
            {
                if (tags is null || tags.Length == 0) return;
                foreach (var name in ManifestOperations.Keys)
                {
                    if (!Packages.TryGetValue(name, out var asset)) continue;
                    Tags[name] = 1;
                    var operation =
                        asset.CreateBundleDownloader(asset.GetAssetInfos(tags)); // 此处暂默认 YooAsset 处理了重复资源的问题
                    if (operation is null) continue;
                    if (operation.TotalDownloadCount <= 0) continue;
                    ResourceDownloaderOperations[string.Concat("Tag-", name, '-', DateTime.Now.Ticks)] = operation;
                }
            }

            public void CollectNeedTag(params string[] tags)
            {
                CollectNeedTagBegin(tags.ToArray());
            }

            #endregion

            private Dictionary<string, long> CurrentValueDict = new Dictionary<string, long>();
            private Dictionary<string, long> TotalValueDict = new Dictionary<string, long>();

            protected override async Task OnWaitAsync()
            {
                CurrentValueDict.Clear();
                TotalValueDict.Clear();

                foreach (var pair in ResourceDownloaderOperations)
                {
                    CurrentValueDict[pair.Key] = pair.Value.CurrentDownloadBytes;
                    TotalValueDict[pair.Key] = pair.Value.TotalDownloadBytes;
                }

                TotalValue = TotalValueDict.Sum(pair => pair.Value);
                CurrentValue = CurrentValueDict.Sum(pair => pair.Value);
                var endValue = TotalValue - CurrentValue;
                if (AssetSystem.GetAvailableDiskSpace() < endValue) // 检查磁盘空间是否足够
                {
                    State = EProgressState.Fail;

                    if (OnDiskSpaceNotEnough is null)
                        throw new IOException(
                            $"Out of disk space : {AssetSystem.GetAvailableDiskSpace().ToConverseStringFileSize()} < {endValue.ToConverseStringFileSize()}");

                    OnDiskSpaceNotEnough.Invoke(Report);
                    AssetSystem.LogException(
                        $"Out of disk space : {AssetSystem.GetAvailableDiskSpace().ToConverseStringFileSize()} < {endValue.ToConverseStringFileSize()}");
                    return;
                }

                foreach (var pair in ResourceDownloaderOperations)
                {
                    CurrentInfo = $"Resource Download -> [{pair.Key}]";
                    while (State != EProgressState.Running)
                    {
                        switch (State)
                        {
                            case EProgressState.Finish:
                            case EProgressState.Fail:
                                return;
                            case EProgressState.Cancel:
                                if (Event.OnError is null) throw new TaskCanceledException();
                                Event.OnError.Invoke(new TaskCanceledException());
                                return;
                            case EProgressState.Pause:
                                await Task.Delay(100);
                                return;
                        }
                    }

                    pair.Value.OnDownloadProgressCallback = OnUpdateProgress;
                    pair.Value.OnDownloadErrorCallback = OnDownloadError;
                    pair.Value.BeginDownload();
                    await pair.Value.Task;
                    if (pair.Value.Status == EOperationStatus.Succeed) continue;
                    State = EProgressState.Fail;
                    return;

                    void OnUpdateProgress(int _, int __, long totalDownloadBytes, long currentDownloadBytes)
                    {
                        if (State != EProgressState.Running) return;
                        switch (Application.internetReachability)
                        {
                            default:
                            case NetworkReachability.NotReachable:
                                OnNetReachableNot?.Invoke(Report);
                                Pause();
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
                            case NetworkReachability.ReachableViaLocalAreaNetwork:
                                break;
                        }

                        TotalValueDict[pair.Key] = totalDownloadBytes;
                        TotalValue = TotalValueDict.Sum(item => item.Value);
                        CurrentValueDict[pair.Key] = currentDownloadBytes;
                        CurrentValue = CurrentValueDict.Sum(item => item.Value);
                    }
                }

                State = EProgressState.Finish;

                if (OpenDownloadAll) AssetSystem.WhiteAll = true;
                else if (Tags.Count > 0) AssetSystem.AddWhite(AssetSystem.GetAssetInfos(Tags.Keys));
            }

            protected override IEnumerator OnWaitCo()
            {
                CurrentValueDict.Clear();
                TotalValueDict.Clear();

                foreach (var pair in ResourceDownloaderOperations)
                {
                    TotalValueDict[pair.Key] = pair.Value.TotalDownloadBytes;
                    CurrentValueDict[pair.Key] = pair.Value.CurrentDownloadBytes;
                }

                TotalValue = TotalValueDict.Sum(pair => pair.Value);
                CurrentValue = CurrentValueDict.Sum(pair => pair.Value);

                if (AssetSystem.GetAvailableDiskSpace() < TotalValue - CurrentValue) // 检查磁盘空间是否足够
                {
                    State = EProgressState.Fail;
                    if (OnDiskSpaceNotEnough is null)
                        throw new SystemException(
                            $"Out of disk space : {AssetSystem.GetAvailableDiskSpace().ToConverseStringFileSize()} < {TotalValue.ToConverseStringFileSize()}");
                    AssetSystem.LogException(
                        $"Out of disk space : {AssetSystem.GetAvailableDiskSpace().ToConverseStringFileSize()} < {TotalValue.ToConverseStringFileSize()}");
                    OnDiskSpaceNotEnough.Invoke(Report);
                    yield break;
                }

                foreach (var pair in ResourceDownloaderOperations)
                {
                    CurrentInfo = $"Resource download : [{pair.Key}]";
                    while (State != EProgressState.Running)
                    {
                        switch (State)
                        {
                            case EProgressState.Finish:
                            case EProgressState.Fail:
                                yield break;
                            case EProgressState.Cancel:
                                if (Event.OnError is null) throw new TaskCanceledException();
                                Event.OnError.Invoke(new TaskCanceledException());
                                yield break;
                            case EProgressState.Pause:
                                yield return new WaitForSeconds(0.1f);
                                break;
                        }
                    }

                    pair.Value.OnDownloadProgressCallback = OnUpdateProgress;
                    pair.Value.OnDownloadErrorCallback = OnDownloadError;
                    pair.Value.BeginDownload();
                    yield return pair.Value;

                    if (pair.Value.Status == EOperationStatus.Succeed) continue;

                    State = EProgressState.Fail;
                    yield break;

                    void OnUpdateProgress(int _, int __, long totalDownloadBytes, long currentDownloadBytes)
                    {
                        if (State != EProgressState.Running) return;
                        switch (Application.internetReachability)
                        {
                            default:
                            case NetworkReachability.NotReachable:
                                OnNetReachableNot?.Invoke(Report);
                                Pause();
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
                            case NetworkReachability.ReachableViaLocalAreaNetwork:
                                break;
                        }

                        TotalValueDict[pair.Key] = totalDownloadBytes;
                        TotalValue = TotalValueDict.Sum(item => item.Value);
                        CurrentValueDict[pair.Key] = currentDownloadBytes;
                        CurrentValue = CurrentValueDict.Sum(item => item.Value);
                    }
                }

                State = EProgressState.Finish;
                if (OpenDownloadAll) AssetSystem.WhiteAll = true;
                else if (Tags.Count > 0) AssetSystem.AddWhite(AssetSystem.GetAssetInfos(Tags.Keys));
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
        }
    }
}

#endif