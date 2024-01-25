/*|✩ - - - - - |||
|||✩ Author:   ||| -> xi nan
|||✩ Date:     ||| -> 2023-08-11
|||✩ Document: ||| ->
|||✩ - - - - - |*/

#if SUPPORT_YOOASSET

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YooAsset;
#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
#endif

namespace AIO.UEngine.YooAsset
{
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
                temp = false;
                end = true;
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

        private static bool AllowReachableCarrier = false;

        private static async Task WaitTask(DownloaderOperation operation)
        {
            operation.BeginDownload();
            switch (Application.internetReachability)
            {
                default:
                case NetworkReachability.NotReachable:
                {
                    var progress = new AProgress();
                    progress.TotalValue = operation.TotalDownloadBytes;
                    progress.CurrentValue = operation.CurrentDownloadBytes;
                    AssetSystem.DownloadEvent.OnNetReachableNot?.Invoke(progress);
                    operation.CancelDownload();
                    break;
                }
                case NetworkReachability.ReachableViaLocalAreaNetwork:
                case NetworkReachability.ReachableViaCarrierDataNetwork:
                {
                    if (AllowReachableCarrier) break;
                    var progress = new AProgress();
                    progress.TotalValue = operation.TotalDownloadBytes;
                    progress.CurrentValue = operation.CurrentDownloadBytes;
                    operation.PauseDownload();
                    AssetSystem.DownloadEvent.OnNetReachableCarrier?.Invoke(progress, () =>
                    {
                        AllowReachableCarrier = true;
                        operation.ResumeDownload();
                    });
                    break;
                }
                // case NetworkReachability.ReachableViaLocalAreaNetwork:
                //  break;
            }

            await operation.Task;
        }

        private static bool temp = false;

        /// <summary>
        /// 检测下载器是否重置
        /// </summary>
        private static bool end = false;

        private static IEnumerator WaitCO(DownloaderOperation operation)
        {
            if (temp) yield return new WaitForSeconds(0.1f);
            switch (Application.internetReachability)
            {
                default:
                case NetworkReachability.NotReachable:
                {
                    var progress = new AProgress();
                    progress.TotalValue = operation.TotalDownloadBytes;
                    progress.CurrentValue = operation.CurrentDownloadBytes;
                    AssetSystem.DownloadEvent.OnNetReachableNot?.Invoke(progress);
                    yield break;
                }
                case NetworkReachability.ReachableViaLocalAreaNetwork:
                case NetworkReachability.ReachableViaCarrierDataNetwork:
                {
                    if (AllowReachableCarrier) break;
                    temp = true;

                    AssetSystem.DownloadEvent.OnNetReachableCarrier?.Invoke(new AProgress
                    {
                        TotalValue = operation.TotalDownloadBytes,
                        CurrentValue = operation.CurrentDownloadBytes,
                    }, () =>
                    {
                        AllowReachableCarrier = true;
                        temp = false;
                    });
                    while (temp) yield return new WaitForSeconds(0.1f);
                    break;
                }
                // case NetworkReachability.ReachableViaLocalAreaNetwork:
                //  break;
            }

            if (end == false)
            {
                operation.CancelDownload();
                yield break;
            }

            operation.BeginDownload();
            yield return operation;
        }

        private static DownloaderOperation CreateDownloaderOperation(YAssetPackage package, AssetInfo location)
        {
            var operation = package.CreateBundleDownloader(location);
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
            if (AssetSystem.DownloadHandle is LoadingInfo loading) loading.RegisterEvent(location, operation);
            return operation;
        }
    }
}
#endif