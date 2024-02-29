#if SUPPORT_YOOASSET

using System;
using System.Collections.Generic;
using System.Linq;
using YooAsset;

namespace AIO.UEngine.YooAsset
{
    partial class Proxy
    {
        private class LoadingInfo : IASNetLoading, IDisposable
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
                foreach (var operation in DicInfo.Values) operation.Operation.CancelDownload();
                DicInfo.Clear();
                _Progress.State = EProgressState.Cancel;
            }

            public void CleanEvent()
            {
                Event.OnWritePermissionNot = null;
                Event.OnReadPermissionNot = null;
                Event.OnNetReachableCarrier = null;
                Event.OnNetReachableNot = null;
                Event.OnDiskSpaceNotEnough = null;
                Event.OnError = null;
                Event.OnProgress = null;
                Event.OnComplete = null;
                Event.OnCancel = null;
                Event.OnPause = null;
                Event.OnResume = null;
                Event.OnBegin = null;
            }

            public IDownlandAssetEvent Event { get; }

            /// <summary>
            /// 当前下载进度
            /// </summary>
            private AProgress _Progress { get; }

            /// <summary>
            /// 下载操作句柄
            /// </summary>
            private Dictionary<string, Info> DicInfo { get; set; }

            internal LoadingInfo()
            {
                Event = new DownlandAssetEvent();
                DicInfo = new Dictionary<string, Info>();
                _Progress = new AProgress();

                foreach (var operation in DownloaderOperations.Values) operation.CancelDownload();
                DownloaderOperations.Clear();
                AssetSystem.HandleReset = false;
            }

            internal void RegisterEvent(AssetInfo info, DownloaderOperation operation)
            {
                if (DicInfo.ContainsKey(info.AssetPath)) return;

                var local = info.AssetPath;
                DicInfo[local] = new Info
                {
                    Current = operation.CurrentDownloadBytes,
                    Total = operation.TotalDownloadBytes,
                    Operation = operation
                };

                operation.OnDownloadProgressCallback += OnDownloadProgress;
                operation.OnDownloadOverCallback += OnDownloadOver;
                operation.OnDownloadErrorCallback += OnDownloadError;

                if (_Progress.State == EProgressState.Pause)
                {
                    operation.PauseDownload();
                    foreach (var item in DicInfo.Values) item.Operation.PauseDownload();
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
                        DicInfo.Remove(local);
                        Update();
                    }
                    else
                    {
                        AssetSystem.DownloadHandle.Event.OnError?.Invoke(new Exception($"Downloading Fail : {local}"));
                        DicInfo.Remove(local);
                        Update();
                    }

                    _Progress.State = DicInfo.Count > 0 ? EProgressState.Running : EProgressState.Finish;
                    if (_Progress.State == EProgressState.Finish) Event.OnComplete?.Invoke(_Progress);
                }

                void OnDownloadProgress(int _, int __, long total, long current)
                {
                    if (_Progress.State != EProgressState.Running) return;
                    DicInfo[local].Total = total;
                    DicInfo[local].Current = current;
                    Update();
                }
            }

            private void Update()
            {
                _Progress.TotalValue = DicInfo.Values.Sum(v => v.Total);
                _Progress.CurrentValue = DicInfo.Values.Sum(v => v.Current);
                AssetSystem.DownloadHandle.Event.OnProgress?.Invoke(_Progress);
            }

            public void Dispose()
            {
                foreach (var operation in DicInfo.Values)
                {
                    operation.Operation.CancelDownload();
                    operation.Operation = null;
                }

                DicInfo.Clear();
                DicInfo = null;
            }
        }
    }
}
#endif