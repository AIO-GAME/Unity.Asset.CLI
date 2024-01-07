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
using System.Text;
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

            /// <summary>
            /// 记录下载的字节数 分阶段记录
            /// </summary>
            private long TempDownloadBytes;

            private string TempDownloadName;

            #region Event

            /// <summary>
            /// 网络不可用
            /// </summary>
            private Action<IProgressReport> OnNetReachableNot { get; }

            /// <summary>
            /// 移动网络 是否允许在移动网络条件下下载
            /// </summary>
            private Action<IProgressReport, Action> OnNetReachableCarrier { get; }

            /// <summary>
            /// 磁盘空间不足
            /// </summary>
            private Action<IProgressReport> OnDiskSpaceNotEnough { get; }

            /// <summary>
            /// 无写入权限
            /// </summary>
            private Action<IProgressReport> OnWritePermissionNot { get; }

            /// <summary>
            /// 无读取权限
            /// </summary>
            private Action<IProgressReport> OnReadPermissionNot { get; }

            #endregion

            public YASDownloader(IDictionary<string, YAssetPackage> packages, DownlandAssetEvent iEvent)
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
            }

            private bool AllowReachableCarrier;

            public bool Flow => Packages.Count > 0;

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
                OpenRecord = false;
                Tags.Clear();
                PreDownloadContentOperations.Clear();
                ResourceDownloaderOperations.Clear();
            }

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
                    Event.OnError?.Invoke(new SystemException($"校验本地资源完整性失败 -> [{pair.Key} -> {pair.Value.Error}]"));
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

                    TotalValue += operation.TotalDownloadBytes - operation.CurrentDownloadBytes;
                    StartValue += operation.CurrentDownloadBytes;
                    ResourceDownloaderOperations[string.Concat("ALL-", name)] = operation;
                }

                OpenDownloadAll = true;
            }

            #endregion

            #region Download

            private static Dictionary<string, List<AssetInfo>> ToYoo(AssetSystem.SequenceRecordQueue Records)
            {
                var list = new Dictionary<string, List<AssetInfo>>();
                if (Records is null) return list;
                foreach (var record in Records)
                {
                    var info = YooAssets.GetPackage(record.PackageName).GetAssetInfo(record.Location);
                    if (info is null) continue;
                    if (!YooAssets.GetPackage(record.PackageName).IsNeedDownloadFromRemote(info)) continue;
                    if (!list.ContainsKey(record.PackageName)) list.Add(record.PackageName, new List<AssetInfo>());
                    if (list[record.PackageName].Contains(info)) continue;
                    list[record.PackageName].Add(info);
                }

                return list;
            }

            public void CollectNeedRecord()
            {
                if (AssetSystem.SequenceRecords is null || AssetSystem.SequenceRecords.Count == 0) return;
                OpenRecord = true;
                foreach (var pair in ToYoo(AssetSystem.SequenceRecords))
                {
                    var operation = YAssetSystem.CreateBundleDownloader(pair.Key, pair.Value.ToArray());
                    if (operation is null) continue;
                    if (operation.TotalDownloadCount <= 0) continue;

                    TotalValue += operation.TotalDownloadBytes - operation.CurrentDownloadBytes;
                    StartValue += operation.CurrentDownloadBytes;
                    ResourceDownloaderOperations[string.Concat("Record-", pair.Key)] = operation;
                }
            }

            private Dictionary<string, byte> Tags = new Dictionary<string, byte>();
            private bool OpenRecord;
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
                    var operation = asset.CreateBundleDownloader(asset.GetAssetInfos(tags));
                    if (operation is null) continue;
                    if (operation.TotalDownloadCount <= 0) continue;
                    TotalValue += operation.TotalDownloadBytes - operation.CurrentDownloadBytes;
                    StartValue += operation.CurrentDownloadBytes;
                    ResourceDownloaderOperations[string.Concat("Tag-", name)] = operation;
                }
            }

            public void CollectNeedTag(params string[] tags)
            {
                CollectNeedTagBegin(tags.ToArray());
            }

            #endregion

            private static IEnumerator WaitCO(IEnumerable<AsyncOperationBase> operations)
            {
                return operations.GetEnumerator();
            }

            protected override async Task OnWaitAsync()
            {
                foreach (var pair in ResourceDownloaderOperations)
                {
                    TempDownloadName = pair.Key;
                    CurrentInfo = string.Format("Resource Download -> [{0}]", pair.Key);
                    TempDownloadBytes = CurrentValue;

                    while (State != EProgressState.Running)
                    {
                        switch (State)
                        {
                            case EProgressState.Finish:
                            case EProgressState.Fail:
                                return;
                            case EProgressState.Cancel:
                                Event.OnError?.Invoke(new TaskCanceledException());
                                Event.OnComplete?.Invoke(Report);
                                return;
                            case EProgressState.Pause:
                                await Task.Delay(100);
                                return;
                        }
                    }

                    // 检查磁盘空间是否足够
                    if (AssetSystem.GetAvailableDiskSpace() < pair.Value.TotalDownloadBytes)
                    {
                        OnDiskSpaceNotEnough?.Invoke(Report);
                        return;
                    }

                    pair.Value.OnDownloadProgressCallback = OnUpdateProgress;
                    pair.Value.OnDownloadErrorCallback = OnUpdateDownloadError;
                    pair.Value.BeginDownload();
                    await pair.Value.Task;
                }

                State = EProgressState.Finish;

                if (OpenDownloadAll)
                {
                    AssetSystem.WhiteAll = true;
                }
                else
                {
                    if (Tags.Count > 0)
                        AssetSystem.AddWhite(AssetSystem.GetAssetInfos(Tags.Keys));

                    if (OpenRecord)
                        AssetSystem.AddWhite(AssetSystem.SequenceRecords.Select(record => record.Location));
                }
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

            private void OnUpdateProgress(
                int totalDownloadCount,
                int currentDownloadCount,
                long totalDownloadBytes,
                long currentDownloadBytes)
            {
                switch (Application.internetReachability)
                {
                    default:
                    case NetworkReachability.NotReachable:
                        OnNetReachableNot?.Invoke(Report);
                        Pause();
                        return;
                    case NetworkReachability.ReachableViaCarrierDataNetwork:
                        if (AllowReachableCarrier) return;
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

                if (State == EProgressState.Fail) return;
                CurrentValue = TempDownloadBytes + currentDownloadBytes;
            }

            private Dictionary<string, string> ErrorDict = new Dictionary<string, string>();

            private void OnUpdateDownloadError(string filename, string error)
            {
                ErrorDict.Add(filename, error);
            }

            protected override IEnumerator OnWaitCo()
            {
                if (State == EProgressState.Cancel ||
                    State == EProgressState.Fail ||
                    State == EProgressState.Finish)
                {
                    Finish();
                    yield break;
                }

                foreach (var pair in ResourceDownloaderOperations)
                {
                    TempDownloadName = pair.Key;
                    CurrentInfo = string.Format("Resource Download -> [{0}]", pair.Key);
                    TempDownloadBytes = CurrentValue;

                    while (State != EProgressState.Running)
                    {
                        switch (State)
                        {
                            case EProgressState.Finish:
                            case EProgressState.Fail:
                                yield break;
                            case EProgressState.Cancel:
                                Event.OnError?.Invoke(new TaskCanceledException());
                                Event.OnComplete?.Invoke(Report);
                                yield break;
                            case EProgressState.Pause:
                                yield return new WaitForSeconds(0.1f);
                                break;
                        }
                    }

                    // 检查磁盘空间是否足够
                    if (AssetSystem.GetAvailableDiskSpace() < pair.Value.TotalDownloadBytes)
                    {
                        OnDiskSpaceNotEnough?.Invoke(Report);
                        yield break;
                    }

                    pair.Value.OnDownloadProgressCallback = OnUpdateProgress;
                    pair.Value.OnDownloadErrorCallback = OnUpdateDownloadError;
                    pair.Value.BeginDownload();
                    yield return pair.Value;
                }

                State = EProgressState.Finish;

                if (OpenDownloadAll)
                {
                    AssetSystem.WhiteAll = true;
                }
                else
                {
                    if (Tags.Count > 0)
                        AssetSystem.AddWhite(AssetSystem.GetAssetInfos(Tags.Keys));

                    if (OpenRecord)
                        AssetSystem.AddWhite(AssetSystem.SequenceRecords.Select(record => record.Location));
                }

                if (ErrorDict.Count > 0)
                {
                    State = EProgressState.Fail;
                    if (Event.OnError != null)
                    {
                        var str = new StringBuilder("下载资源包文件失败\n");
                        foreach (var pair in ErrorDict)
                        {
                            AssetSystem.LogError($"下载资源包文件失败 -> [{pair.Key} -> {pair.Value}]");
                            str.AppendLine(pair.Key);
                        }

                        Event.OnError.Invoke(new SystemException(str.ToString()));
                    }
#if UNITY_EDITOR
                    else
                    {
                        foreach (var pair in ErrorDict)
                        {
                            AssetSystem.LogError($"下载资源包文件失败 -> [{pair.Key} -> {pair.Value}]");
                        }
                    }
#endif
                }
                else Finish();
            }


            #region End

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

            #endregion
        }
    }
}

#endif