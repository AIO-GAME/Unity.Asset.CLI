/*|✩ - - - - - |||
|||✩ Author:   ||| -> xi nan
|||✩ Date:     ||| -> 2023-08-11
|||✩ Document: ||| ->
|||✩ - - - - - |*/

#if SUPPORT_YOOASSET
using System;
using System.Collections.Generic;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
#endif
using YooAsset;

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
            
            public EProgressState State { get; private set; }

            /// <summary>
            /// 当前下载进度
            /// </summary>
            private AProgress _Progress { get; }

            /// <summary>
            /// 总下载大小
            /// </summary>
            public long TotalDownloadBytes => TotalDownloadedBytesList.Values.Sum();

            /// <summary>
            /// 当前下载大小
            /// </summary>
            public long CurrentDownloadBytes => CurrentDownloadedBytesList.Values.Sum();

            /// <summary>
            /// 总下载数量
            /// </summary>
            public int TotalDownloadCount => TotalDownloadCountList.Values.Sum();

            /// <summary>
            /// 当前下载数量
            /// </summary>
            public int CurrentDownloadCount => CurrentDownloaadeCountList.Values.Sum();

            /// <summary>
            /// 总下载大小
            /// </summary>
            private Dictionary<string, long> TotalDownloadedBytesList;

            /// <summary>
            /// 当前下载大小
            /// </summary>
            private Dictionary<string, long> CurrentDownloadedBytesList;

            /// <summary>
            /// 当前下载数量
            /// </summary>
            private Dictionary<string, int> CurrentDownloaadeCountList;

            /// <summary>
            /// 总下载数量
            /// </summary>
            private Dictionary<string, int> TotalDownloadCountList;

            private Dictionary<string, DownloaderOperation> operations;

            internal LoadingInfo()
            {
                operations = new Dictionary<string, DownloaderOperation>();
                TotalDownloadCountList = new Dictionary<string, int>();
                CurrentDownloaadeCountList = new Dictionary<string, int>();
                CurrentDownloadedBytesList = new Dictionary<string, long>();
                TotalDownloadedBytesList = new Dictionary<string, long>();
                _Progress = new AProgress();
            }

            public void Finish()
            {
                TotalDownloadCountList.Clear();
                CurrentDownloaadeCountList.Clear();
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
                State = EProgressState.Pause;
            }

            /// <summary>
            /// 恢复下载
            /// </summary>
            public void Resume()
            {
                foreach (var operation in operations.Values) operation.ResumeDownload();
                State = EProgressState.Running;
            }

            internal void RegisterEvent(AssetInfo info, DownloaderOperation operation)
            {
                var local = info.AssetPath;
                if (operations.ContainsKey(local))
                {
                    AssetSystem.LogError("当前资源正在下载中: {0}", local);
                    return;
                }

                CurrentDownloaadeCountList[local] = operation.CurrentDownloadCount; // 当前下载数量
                TotalDownloadCountList[local] = operation.TotalDownloadCount; // 总下载数量
                CurrentDownloadedBytesList[local] = operation.CurrentDownloadBytes; // 当前下载大小
                TotalDownloadedBytesList[local] = operation.TotalDownloadBytes; // 总下载大小
                Update();

                operation.OnDownloadProgressCallback += OnDownloadProgressCallback;
                operation.OnDownloadOverCallback += OnDownloadOver;
                operations.Add(local, operation);
                State = EProgressState.Running;
                return;

                void OnDownloadOver(bool isSucceed)
                {
                    CurrentDownloaadeCountList.Remove(local);
                    TotalDownloadCountList.Remove(local);
                    CurrentDownloadedBytesList.Remove(local);
                    TotalDownloadedBytesList.Remove(local);
                    operations.Remove(local);
                    Update();
                    State = operations.Count > 0 ? EProgressState.Running : EProgressState.Finish;
                }

                void OnDownloadProgressCallback(
                    int totalDownloadCount,
                    int currentDownloadCount,
                    long totalDownloadBytes,
                    long currentDownloadBytes)
                {
                    CurrentDownloaadeCountList[local] = currentDownloadCount;
                    TotalDownloadCountList[local] = totalDownloadCount;
                    TotalDownloadedBytesList[local] = totalDownloadBytes;
                    CurrentDownloadedBytesList[local] = currentDownloadBytes;

                    _Progress.TotalValue = TotalDownloadedBytesList.Values.Sum();
                    _Progress.CurrentValue = CurrentDownloadedBytesList.Values.Sum();
                    AssetSystem.MainDownloadHandle.Event.OnProgress?.Invoke(_Progress);
                }
            }

            private void Update()
            {
                _Progress.TotalValue = TotalDownloadedBytesList.Values.Sum();
                _Progress.CurrentValue = CurrentDownloadedBytesList.Values.Sum();
                AssetSystem.MainDownloadHandle.Event.OnProgress?.Invoke(_Progress);
            }
        }

        internal static DownloaderOperation CreateDownloaderOperation(YAssetPackage package, AssetInfo location)
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

            if (AssetSystem.MainDownloadHandle is LoadingInfo loading) loading.RegisterEvent(location, operation);
            return operation;
        }

        /// <summary>
        /// 资源加载器 - 参数
        /// </summary>
        public static event Func<YAssetPackage, YAssetParameters> GetParameter;
    }
}
#endif