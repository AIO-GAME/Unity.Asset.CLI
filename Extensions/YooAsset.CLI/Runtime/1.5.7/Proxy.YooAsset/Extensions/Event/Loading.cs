/*|✩ - - - - - |||
|||✩ Author:   ||| -> xi nan
|||✩ Date:     ||| -> 2023-08-11
|||✩ Document: ||| ->
|||✩ - - - - - |*/

#if SUPPORT_YOOASSET

using System;
using System.Collections.Generic;
using System.Linq;
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

            private bool AllowReachableCarrier = false;

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
                operation.OnStartDownloadFileCallback += OnStartDownloadFile;
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

                void OnStartDownloadFile(string fileName, long sizeBytes)
                {
                    if (_Progress.State != EProgressState.Running)
                    {
                        _Progress.State = EProgressState.Pause;
                        foreach (var item in _Data.Values) item.Operation.PauseDownload();
                        return;
                    }

                    switch (Application.internetReachability)
                    {
                        default:
                        case NetworkReachability.NotReachable:
                            _Progress.State = EProgressState.Pause;
                            foreach (var item in _Data.Values) item.Operation.PauseDownload();
                            Event.OnNetReachableNot?.Invoke(_Progress);
                            return;
                        case NetworkReachability.ReachableViaLocalAreaNetwork:
                        case NetworkReachability.ReachableViaCarrierDataNetwork:
                            if (AllowReachableCarrier) break;
                            _Progress.State = EProgressState.Pause;
                            foreach (var item in _Data.Values) item.Operation.PauseDownload();
                            Event.OnNetReachableCarrier?.Invoke(_Progress, () =>
                            {
                                AllowReachableCarrier = true;
                                _Progress.State = EProgressState.Running;
                                foreach (var item in _Data.Values) item.Operation.ResumeDownload();
                            });
                            break;
                        // case NetworkReachability.ReachableViaLocalAreaNetwork:
                        //    break;
                    }
                }

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
                    
                    switch (Application.internetReachability)
                    {
                        default:
                        case NetworkReachability.NotReachable:
                            _Progress.State = EProgressState.Pause;
                            foreach (var item in _Data.Values) item.Operation.PauseDownload();
                            Event.OnNetReachableNot?.Invoke(_Progress);
                            return;
                        case NetworkReachability.ReachableViaLocalAreaNetwork:
                        case NetworkReachability.ReachableViaCarrierDataNetwork:
                            if (AllowReachableCarrier) break;
                            _Progress.State = EProgressState.Pause;
                            foreach (var item in _Data.Values) item.Operation.PauseDownload();
                            Event.OnNetReachableCarrier?.Invoke(_Progress, () =>
                            {
                                AllowReachableCarrier = true;
                                _Progress.State = EProgressState.Running;
                                foreach (var item in _Data.Values) item.Operation.ResumeDownload();
                            });
                            return;
                        // case NetworkReachability.ReachableViaLocalAreaNetwork:
                        //    break;
                    }
                    
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