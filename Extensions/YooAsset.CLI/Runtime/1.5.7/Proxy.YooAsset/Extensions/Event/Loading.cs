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

            public IDownlandAssetEvent Event { get; }

            /// <summary>
            /// 当前下载进度
            /// </summary>
            private AProgress _Progress { get; }

            /// <summary>
            /// 下载操作句柄
            /// </summary>
            private Dictionary<string, Info> _Data;

            public void Dispose()
            {
                foreach (var operation in _Data.Values) operation.Operation.CancelDownload();
                _Data.Clear();
            }

            internal LoadingInfo()
            {
                Event = new DownlandAssetEvent();
                _Data = new Dictionary<string, Info>();
                _Progress = new AProgress();
            }

            public void Finish()
            {
                foreach (var operation in _Data.Values) operation.Operation.CancelDownload();
                _Data.Clear();
                _Progress.State = EProgressState.Finish;
            }

            /// <summary>
            /// 暂停下载
            /// </summary>
            public void Pause()
            {
                foreach (var operation in _Data.Values) operation.Operation.PauseDownload();
                _Progress.State = EProgressState.Pause;
            }

            /// <summary>
            /// 恢复下载
            /// </summary>
            public void Resume()
            {
                _Progress.State = EProgressState.Running;
                foreach (var operation in _Data.Values) operation.Operation.ResumeDownload();
            }

            internal void RegisterEvent(AssetInfo info, DownloaderOperation operation)
            {
                if (_Data.ContainsKey(info.AssetPath)) return;

                var local = info.AssetPath;
                _Data[local] = new Info
                {
                    Current = operation.CurrentDownloadBytes,
                    Total = operation.TotalDownloadBytes,
                    Operation = operation
                };
                Update();

                operation.OnDownloadProgressCallback += OnDownloadProgressCallback;
                operation.OnDownloadOverCallback += OnDownloadOver;
                operation.OnDownloadErrorCallback += OnDownloadError;
                _Progress.State = EProgressState.Running;
                return;

                void OnDownloadError(string fileName, string error)
                {
                    AssetSystem.DownloadHandle.Event.OnError?.Invoke(new Exception($"{fileName} : {error}"));
                }

                void OnDownloadOver(bool isSucceed)
                {
                    if (isSucceed)
                    {
                        _Data.Remove(local);
                        Update();
                    }
                    else
                    {
                        _Data.Remove(local);
                        AssetSystem.DownloadHandle.Event.OnError?.Invoke(new Exception($"Downloading Fail : {local}"));
                        Update();
                    }

                    _Progress.State = _Data.Count > 0 ? EProgressState.Running : EProgressState.Finish;
                    if (_Progress.State == EProgressState.Finish) Event.OnComplete?.Invoke(_Progress);
                }

                void OnDownloadProgressCallback(int _, int __, long total, long current)
                {
                    _Data[local].Total = total;
                    _Data[local].Current = current;
                    Update();
                }
            }

            private void Update()
            {
                if (AssetSystem.DownloadHandle.Event.OnProgress is null) return;
                _Progress.TotalValue = _Data.Values.Sum(v => v.Total);
                _Progress.CurrentValue = _Data.Values.Sum(v => v.Current);
                AssetSystem.DownloadHandle.Event.OnProgress?.Invoke(_Progress);
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