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

namespace AIO.UEngine.YooAsset
{
    internal partial class YAssetSystem
    {
        public class LoadingInfo : IASNetLoading
        {
            /// <summary>
            /// 当前下载进度
            /// </summary>
            public IProgressHandle Progress => _Progress;

            /// <summary>
            /// 当前下载进度
            /// </summary>
            private AProgress _Progress;

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

            private void Finish()
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
            }

            /// <summary>
            /// 恢复下载
            /// </summary>
            public void Resume()
            {
                foreach (var operation in operations.Values) operation.ResumeDownload();
            }

            internal void RegisterEvent(AssetInfo info, DownloaderOperation operation)
            {
                if (!AssetSystem.HasEvent_OnDownloading()) return; // 没有注册事件
                var local = info.AssetPath;
                if (operations.ContainsKey(local))
                {
                    AssetSystem.LogErrorFormat("当前资源正在下载中: {0}", local);
                    return;
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

                    _Progress.Total = TotalDownloadedBytesList.Values.Sum();
                    _Progress.Current = CurrentDownloadedBytesList.Values.Sum();
                    AssetSystem.InvokeDownloading(_Progress);
                }

                void OnDownloadOver(bool isSucceed)
                {
                    CurrentDownloaadeCountList.Remove(local);
                    TotalDownloadCountList.Remove(local);
                    CurrentDownloadedBytesList.Remove(local);
                    TotalDownloadedBytesList.Remove(local);
                    operations.Remove(local);
                    Update();
                }

                CurrentDownloaadeCountList[local] = operation.CurrentDownloadCount; // 当前下载数量
                TotalDownloadCountList[local] = operation.TotalDownloadCount; // 总下载数量
                CurrentDownloadedBytesList[local] = operation.CurrentDownloadBytes; // 当前下载大小
                TotalDownloadedBytesList[local] = operation.TotalDownloadBytes; // 总下载大小
                Update();

                operation.OnDownloadProgressCallback += OnDownloadProgressCallback;
                operation.OnDownloadOverCallback += OnDownloadOver;
                operations.Add(local, operation);
            }

            private void Update()
            {
                _Progress.Total = TotalDownloadedBytesList.Values.Sum();
                _Progress.Current = CurrentDownloadedBytesList.Values.Sum();
                AssetSystem.InvokeDownloading(_Progress);
            }
        }

        /// <summary>
        /// 资源加载器 - 加载资源
        /// </summary>
        private static readonly LoadingInfo LoadHandle = new LoadingInfo();

        internal static DownloaderOperation CreateDownloaderOperation(YAssetPackage package, AssetInfo location)
        {
            var operation = package.CreateBundleDownloader(location,
                AssetSystem.Parameter.LoadingMaxTimeSlice,
                AssetSystem.Parameter.DownloadFailedTryAgain,
                AssetSystem.Parameter.Timeout);
            if (AssetSystem.Parameter.EnableSequenceRecord)
            {
                var record = new AssetSystem.SequenceRecord
                {
                    PackageName = package.PackageName,
                    Location = location.Address,
                    Time = DateTime.Now,
                    Bytes = operation.TotalDownloadBytes,
                    Count = operation.TotalDownloadCount,
                    AssetPath = location.AssetPath,
                };
                Console.WriteLine(record);
                AssetSystem.AddSequenceRecord(record);
            }
            LoadHandle.RegisterEvent(location, operation);
            return operation;
        }

        /// <summary>
        /// 资源加载器 - 参数
        /// </summary>
        public static event Func<YAssetPackage, YAssetParameters> GetParameter;
    }
}
#endif