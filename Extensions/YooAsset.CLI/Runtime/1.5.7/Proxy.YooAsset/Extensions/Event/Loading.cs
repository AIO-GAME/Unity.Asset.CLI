/*|✩ - - - - - |||
|||✩ Author:   ||| -> xi nan
|||✩ Date:     ||| -> 2023-08-11
|||✩ Document: ||| ->
|||✩ - - - - - |*/


#if SUPPORT_YOOASSET

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using YooAsset;

namespace AIO.UEngine.YooAsset
{
#if UNITY_EDITOR
    using UnityEditor;
#endif

    internal partial class YAssetSystem
    {
        public class LoadingInfo : IASNetLoading, IDisposable
        {
            private class Info
            {
                /// <summary>
                /// 总下载大小
                /// </summary>
                public long Total;

                /// <summary>
                /// 当前下载大小
                /// </summary>
                public long Current;

                /// <summary>
                /// 下载操作句柄
                /// </summary>
                public DownloaderOperation Operation;
            }

            IProgressInfo IASNetLoading.Progress => _Progress;

            EProgressState IASNetLoading.State => _Progress.State;

            public void Cancel()
            {
                foreach (var operation in _Data.Values) operation.Operation.CancelDownload();
                _Data.Clear();
                _Progress.State = EProgressState.Cancel;
            }

            public IDownlandAssetEvent Event { get; }

            /// <summary>
            /// 当前下载进度
            /// </summary>
            private AProgress _Progress { get; }

            /// <summary>
            /// 下载操作句柄
            /// </summary>
            private Dictionary<string, Info> _Data;

            internal LoadingInfo()
            {
                Event = new DownlandAssetEvent();
                _Data = new Dictionary<string, Info>();
                _Progress = new AProgress();

                foreach (var operation in Operations.Values) operation.CancelDownload();
                Operations.Clear();
                AssetSystem.HandleReset = false;
            }

            internal void RegisterEvent(AssetInfo info, DownloaderOperation operation)
            {
                if (_Data.ContainsKey(info.AssetPath)) return;

                var local = info.AssetPath;
                operation.OnDownloadProgressCallback += OnDownloadProgress;
                operation.OnDownloadOverCallback += OnDownloadOver;
                operation.OnDownloadErrorCallback += OnDownloadError;
                _Data[local] = new Info
                {
                    Current = operation.CurrentDownloadBytes,
                    Total = operation.TotalDownloadBytes,
                    Operation = operation
                };

                if (_Progress.State == EProgressState.Pause)
                {
                    operation.PauseDownload();
                    foreach (var item in _Data.Values) item.Operation.PauseDownload();
                }
                else
                {
                    _Progress.State = EProgressState.Running;
                    Update();
                }

                return;

                void OnDownloadError(string fileName, string error)
                {
                    AssetSystem.DownloadHandle.Event.OnError?.Invoke(new Exception($"{fileName} : {error}"));
                }

                void OnDownloadOver(bool isSucceed)
                {
                    if (_Progress.State != EProgressState.Running) return;
                    if (isSucceed)
                    {
                        _Data.Remove(local);
                        Update();
                    }
                    else
                    {
                        AssetSystem.DownloadHandle.Event.OnError?.Invoke(new Exception($"Downloading Fail : {local}"));
                        _Data.Remove(local);
                        Update();
                    }

                    _Progress.State = _Data.Count > 0 ? EProgressState.Running : EProgressState.Finish;
                    if (_Progress.State == EProgressState.Finish) Event.OnComplete?.Invoke(_Progress);
                }

                void OnDownloadProgress(int _, int __, long total, long current)
                {
                    if (_Progress.State != EProgressState.Running) return;
                    _Data[local].Total = total;
                    _Data[local].Current = current;
                    Update();
                }
            }

            private void Update()
            {
                _Progress.TotalValue = _Data.Values.Sum(v => v.Total);
                _Progress.CurrentValue = _Data.Values.Sum(v => v.Current);
                AssetSystem.DownloadHandle.Event.OnProgress?.Invoke(_Progress);
            }

            public void Dispose()
            {
                foreach (var operation in _Data.Values)
                {
                    operation.Operation.CancelDownload();
                    operation.Operation = null;
                }

                _Data.Clear();
            }
        }

        private static async Task WaitTask(DownloaderOperation operation, AssetInfo location)
        {
            if (Operations.ContainsKey(location.AssetPath)) // 如果已经存在下载任务 则直接返回 避免重复下载
            {
                await Operations[location.AssetPath].Task;
                return;
            }

            Operations[location.AssetPath] = operation;
            if (AssetSystem.StatusStop) await Task.Delay(100);
            switch (Application.internetReachability)
            {
                default:
                case NetworkReachability.NotReachable:
                {
                    if (AssetSystem.DownloadEvent.OnNetReachableNot is null)
                        throw new Exception("NetReachableNot is null");

                    AssetSystem.DownloadEvent.OnNetReachableNot.Invoke(new AProgress
                    {
                        TotalValue = Operations[location.AssetPath].TotalDownloadBytes,
                        CurrentValue = Operations[location.AssetPath].CurrentDownloadBytes,
                        State = EProgressState.Fail,
                    });
                    return;
                }
                case NetworkReachability.ReachableViaLocalAreaNetwork:
                case NetworkReachability.ReachableViaCarrierDataNetwork:
                {
                    if (AssetSystem.AllowReachableCarrier) break;
                    if (AssetSystem.DownloadEvent.OnNetReachableCarrier is null)
                        throw new Exception("AssetSystem.DownloadEvent.OnNetReachableCarrier is null");

                    AssetSystem.StatusStop = true;
                    AssetSystem.DownloadEvent.OnNetReachableCarrier.Invoke(new AProgress
                    {
                        State = EProgressState.Pause,
                        TotalValue = Operations[location.AssetPath].TotalDownloadBytes,
                        CurrentValue = Operations[location.AssetPath].CurrentDownloadBytes,
                    }, () =>
                    {
                        AssetSystem.AllowReachableCarrier = true;
                        AssetSystem.StatusStop = false;
                        AssetSystem.HandleReset = false;
                    });
                    while (AssetSystem.StatusStop) await Task.Delay(100);
                    break;
                }
                // case NetworkReachability.ReachableViaLocalAreaNetwork:
                //  break;
            }

            if (AssetSystem.HandleReset) return;
            if (!Operations.ContainsKey(location.AssetPath)) return;
            if (Operations[location.AssetPath].Status == EOperationStatus.Failed) return;
            Operations[location.AssetPath].BeginDownload();
            await Operations[location.AssetPath].Task;
            if (AssetSystem.DownloadHandle is LoadingInfo loading) loading.RegisterEvent(location, Operations[location.AssetPath]);
            Operations.Remove(location.AssetPath);
        }

        private static readonly Dictionary<string, DownloaderOperation> Operations = new Dictionary<string, DownloaderOperation>();

        private static IEnumerator WaitCO(DownloaderOperation operation, AssetInfo location)
        {
            if (Operations.ContainsKey(location.AssetPath)) // 如果已经存在下载任务 则直接返回 避免重复下载
            {
                yield return Operations[location.AssetPath];
                yield break;
            }

            Operations[location.AssetPath] = operation;
            if (AssetSystem.StatusStop) yield return new WaitForSeconds(0.1f);
            switch (Application.internetReachability)
            {
                default:
                case NetworkReachability.NotReachable:
                {
                    if (AssetSystem.DownloadEvent.OnNetReachableNot is null)
                        throw new Exception("NetReachableNot is null");

                    AssetSystem.DownloadEvent.OnNetReachableNot.Invoke(new AProgress
                    {
                        TotalValue = Operations[location.AssetPath].TotalDownloadBytes,
                        CurrentValue = Operations[location.AssetPath].CurrentDownloadBytes,
                        State = EProgressState.Fail,
                    });
                    yield break;
                }
                case NetworkReachability.ReachableViaLocalAreaNetwork:
                case NetworkReachability.ReachableViaCarrierDataNetwork:
                {
                    if (AssetSystem.AllowReachableCarrier) break;
                    if (AssetSystem.DownloadEvent.OnNetReachableCarrier is null)
                        throw new Exception("AssetSystem.DownloadEvent.OnNetReachableCarrier is null");

                    AssetSystem.StatusStop = true;
                    AssetSystem.DownloadEvent.OnNetReachableCarrier.Invoke(new AProgress
                    {
                        State = EProgressState.Pause,
                        TotalValue = Operations[location.AssetPath].TotalDownloadBytes,
                        CurrentValue = Operations[location.AssetPath].CurrentDownloadBytes,
                    }, () =>
                    {
                        AssetSystem.AllowReachableCarrier = true;
                        AssetSystem.StatusStop = false;
                        AssetSystem.HandleReset = false;
                    });
                    while (AssetSystem.StatusStop) yield return new WaitForSeconds(0.1f);
                    break;
                }
                // case NetworkReachability.ReachableViaLocalAreaNetwork:
                //  break;
            }

            if (AssetSystem.HandleReset) yield break;
            if (!Operations.ContainsKey(location.AssetPath)) yield break;
            if (Operations[location.AssetPath].Status == EOperationStatus.Failed) yield break;
            Operations[location.AssetPath].BeginDownload();
            yield return Operations[location.AssetPath];
            if (AssetSystem.DownloadHandle is LoadingInfo loading) loading.RegisterEvent(location, Operations[location.AssetPath]);
            Operations.Remove(location.AssetPath);
        }

        private static DownloaderOperation CreateDownloaderOperation(YAssetPackage package, AssetInfo location)
        {
            return Operations.ContainsKey(location.AssetPath)
                ? Operations[location.AssetPath]
                : package.CreateBundleDownloader(location);
        }

        [Conditional("UNITY_EDITOR")]
        private static void AddSequenceRecord(YAssetPackage package, AssetInfo location, DownloaderOperation operation)
        {
#if UNITY_EDITOR
            if (AssetSystem.Parameter.EnableSequenceRecord)
            {
                AssetSystem.AddSequenceRecord(new AssetSystem.SequenceRecord
                {
                    GUID = AssetDatabase.AssetPathToGUID(location.AssetPath),
                    PackageName = package.PackageName,
                    Location = location.Address,
                    Time = DateTime.Now,
                    Bytes = operation.TotalDownloadBytes,
                    Count = operation.TotalDownloadCount,
                    AssetPath = location.AssetPath,
                });
            }
#endif
        }
    }
}
#endif