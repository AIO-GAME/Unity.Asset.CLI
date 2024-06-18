#if SUPPORT_YOOASSET

#region

using System;
using System.Collections;
using System.Diagnostics;
using System.Threading.Tasks;
using UnityEngine;
using YooAsset;
#if UNITY_2022_1_OR_NEWER
using Unity.Profiling;
#endif
#if UNITY_EDITOR
using UnityEditor;
#endif

#endregion

namespace AIO.UEngine.YooAsset
{
    partial class Proxy
    {
        private static readonly AProgress Progress = new AProgress();

        private static void WaitNotReachable(AssetInfo location)
        {
            if (AssetSystem.DownloadEvent.OnNetReachableNot is null)
                throw new Exception("NetReachableNot is null");

            Progress.State        = EProgressState.Fail;
            Progress.TotalValue   = DownloaderOperations[location.AssetPath].TotalDownloadBytes;
            Progress.CurrentValue = DownloaderOperations[location.AssetPath].CurrentDownloadBytes;
            AssetSystem.DownloadEvent.OnNetReachableNot.Invoke(Progress);
        }

        private static void WaitReachableViaCarrierDataNetwork(AssetInfo location)
        {
            if (AssetSystem.DownloadEvent.OnNetReachableCarrier is null)
                throw new Exception($"OnNetReachableCarrier is null => {location.AssetPath} loading fail");

            AssetSystem.StatusStop = true;
            Progress.State         = EProgressState.Pause;
            Progress.TotalValue    = DownloaderOperations[location.AssetPath].TotalDownloadBytes;
            Progress.CurrentValue  = DownloaderOperations[location.AssetPath].CurrentDownloadBytes;
            AssetSystem.DownloadEvent.OnNetReachableCarrier.Invoke(Progress, AllowReachableCarrier);
        }

        private static async Task WaitTask(DownloaderOperation operation, AssetInfo location)
        {
            if (DownloaderOperations.TryGetValue(location.AssetPath, out var downloaderOperation))
            {
                await downloaderOperation.Task; // 如果已经存在下载任务 则直接返回 避免重复下载
                return;
            }

            DownloaderOperations[location.AssetPath] = operation;
            if (AssetSystem.StatusStop) await Task.Delay(100);
            switch (Application.internetReachability)
            {
                default:
                case NetworkReachability.NotReachable:
                {
                    WaitNotReachable(location);
                    return;
                }
                case NetworkReachability.ReachableViaCarrierDataNetwork:
                {
                    if (AssetSystem.AllowReachableCarrier) break;
                    WaitReachableViaCarrierDataNetwork(location);
                    while (AssetSystem.StatusStop) await Task.Delay(100);
                    break;
                }
                case NetworkReachability.ReachableViaLocalAreaNetwork:
                    break;
            }

            if (AssetSystem.HandleReset) return;
            if (!DownloaderOperations.TryGetValue(location.AssetPath, out var downloader)) return;
            if (downloader.Status == EOperationStatus.Failed) return;
            downloader.BeginDownload();
            await downloader.Task;
            if (AssetSystem.DownloadHandle is LoadingInfo loading) loading.RegisterEvent(location, downloader);
            DownloaderOperations.Remove(location.AssetPath);
        }

        private static void AllowReachableCarrier()
        {
            AssetSystem.AllowReachableCarrier = true;
            AssetSystem.StatusStop            = false;
            AssetSystem.HandleReset           = false;
        }

        private static IEnumerator WaitCO(DownloaderOperation operation, AssetInfo location)
        {
            if (DownloaderOperations.TryGetValue(location.AssetPath, out var downloaderOperation))
            {
                yield return downloaderOperation; // 如果已经存在下载任务 则直接返回 避免重复下载
                yield break;
            }

            DownloaderOperations[location.AssetPath] = operation;
            if (AssetSystem.StatusStop) yield return new WaitForSeconds(0.1f);
            switch (Application.internetReachability)
            {
                default:
                case NetworkReachability.NotReachable:
                {
                    WaitNotReachable(location);
                    yield break;
                }
                case NetworkReachability.ReachableViaCarrierDataNetwork:
                {
                    if (AssetSystem.AllowReachableCarrier) break;
                    WaitReachableViaCarrierDataNetwork(location);
                    while (AssetSystem.StatusStop) yield return new WaitForSeconds(0.1f);
                    break;
                }
                case NetworkReachability.ReachableViaLocalAreaNetwork:
                    break;
            }

            if (AssetSystem.HandleReset) yield break;
            if (!DownloaderOperations.TryGetValue(location.AssetPath, out operation)) yield break;
            if (operation.Status == EOperationStatus.Failed) yield break;
            operation.BeginDownload();
            yield return operation;
            if (AssetSystem.DownloadHandle is LoadingInfo loading)
                loading.RegisterEvent(location, operation);
        }

        private static DownloaderOperation CreateDownloaderOperation(ResPackage package, AssetInfo location)
        {
            return DownloaderOperations.TryGetValue(location.AssetPath, out var operation)
                ? operation
                : package.CreateBundleDownloader(location);
        }

        [Conditional("UNITY_EDITOR")]
#if UNITY_2022_1_OR_NEWER
        [IgnoredByDeepProfiler]
#endif
        private static void AddSequenceRecord(ResPackage package, AssetInfo location)
        {
#if UNITY_EDITOR
            if (!AssetSystem.Parameter.EnableSequenceRecord) return;
            var record = new AssetSystem.SequenceRecord
            {
                PackageName = package.PackageName,
                Location    = location.Address,
                AssetPath   = location.AssetPath
            };
            record.SetGUID(AssetDatabase.AssetPathToGUID(location.AssetPath));
            AssetSystem.AddSequenceRecord(record);
#endif
        }
    }
}
#endif