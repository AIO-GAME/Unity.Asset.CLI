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
        public class LoadingInfo : IASNetLoading
        {
            /// <summary>
            /// 当前下载进度
            /// </summary>
            public IProgressInfo Progress => _Progress;

            public IDownlandAssetEvent Event { get; set; }

            EProgressState IASNetLoading.State => _Progress.State;

            /// <summary>
            /// 当前下载进度
            /// </summary>
            private AProgress _Progress { get; }

            /// <summary>
            /// 总下载大小
            /// </summary>
            private Dictionary<string, long> TotalDownloadedBytesList;

            /// <summary>
            /// 当前下载大小
            /// </summary>
            private Dictionary<string, long> CurrentDownloadedBytesList;

            private Dictionary<string, DownloaderOperation> operations;

            internal LoadingInfo()
            {
                operations = new Dictionary<string, DownloaderOperation>();
                CurrentDownloadedBytesList = new Dictionary<string, long>();
                TotalDownloadedBytesList = new Dictionary<string, long>();
                Event = new DownlandAssetEvent();
                _Progress = new AProgress();
            }

            public void Finish()
            {
                CurrentDownloadedBytesList.Clear();
                TotalDownloadedBytesList.Clear();
                operations.Clear();
            }

            /// <summary>
            /// 暂停下载
            /// </summary>
            public void Pause()
            {
                foreach (var operation in operations.Values) operation.PauseDownload();
                _Progress.State = EProgressState.Pause;
            }

            /// <summary>
            /// 恢复下载
            /// </summary>
            public void Resume()
            {
                foreach (var operation in operations.Values) operation.ResumeDownload();
                _Progress.State = EProgressState.Running;
            }

            internal void RegisterEvent(AssetInfo info, DownloaderOperation operation)
            {
                var local = info.AssetPath;
                if (operations.ContainsKey(local))
                {
                    AssetSystem.LogError("当前资源 正在下载中 : {0}", local);
                    return;
                }

                CurrentDownloadedBytesList[local] = operation.CurrentDownloadBytes; // 当前下载大小
                TotalDownloadedBytesList[local] = operation.TotalDownloadBytes; // 总下载大小
                Update();
                operation.OnDownloadProgressCallback += OnDownloadProgressCallback;
                operation.OnDownloadOverCallback += OnDownloadOver;
                operation.OnDownloadErrorCallback += (f, r) =>
                {
                    AssetSystem.DownloadHandle.Event.OnError?.Invoke(new Exception($"{f}:{r}"));
                };
                operations.Add(local, operation);
                _Progress.State = EProgressState.Running;
                return;

                void OnDownloadOver(bool isSucceed)
                {
                    if (isSucceed)
                        AssetSystem.DownloadHandle.Event.OnError?.Invoke(new Exception($"Downloading Fail : {local}"));
                    operations.Remove(local);
                    CurrentDownloadedBytesList.Remove(local);
                    TotalDownloadedBytesList.Remove(local);
                    Update();
                    _Progress.State = operations.Count > 0 ? EProgressState.Running : EProgressState.Finish;

                    if (_Progress.State == EProgressState.Finish) Event.OnComplete?.Invoke(_Progress);
                }

                void OnDownloadProgressCallback(
                    int totalDownloadCount,
                    int currentDownloadCount,
                    long totalDownloadBytes,
                    long currentDownloadBytes)
                {
                    TotalDownloadedBytesList[local] = totalDownloadBytes;
                    CurrentDownloadedBytesList[local] = currentDownloadBytes;
                    Update();
                }
            }

            private void Update()
            {
                _Progress.TotalValue = TotalDownloadedBytesList.Values.Sum();
                _Progress.CurrentValue = CurrentDownloadedBytesList.Values.Sum();
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

        /// <summary>
        /// 资源加载器 - 参数
        /// </summary>
        public static event Func<YAssetPackage, YAssetParameters> GetParameter;
    }
}
#endif