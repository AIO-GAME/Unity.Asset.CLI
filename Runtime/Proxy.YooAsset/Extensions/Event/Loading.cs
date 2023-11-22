/*|✩ - - - - - |||
|||✩ Author:   ||| -> XINAN
|||✩ Date:     ||| -> 2023-08-11
|||✩ Document: ||| ->
|||✩ - - - - - |*/

#if SUPPORT_YOOASSET
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
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
            public double Progress
            {
                get
                {
                    if (operations.Count == 0) return 1;
                    return CurrentDownloadBytes / (double)TotalDownloadBytes;
                }
            }

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

            internal void RegisterEvent(string local, DownloaderOperation operation)
            {
                if (!AssetSystem.HasEvent_OnDownloading()) return;
                if (operations.ContainsKey(local))
                {
                    Debug.LogErrorFormat("当前资源正在下载中: {0}", local);
                    return;
                }

                void OnDownloadProgressCallback(int totalDownloadCount, int currentDownloadCount,
                    long totalDownloadBytes, long currentDownloadBytes)
                {
                    CurrentDownloaadeCountList[local] = currentDownloadCount;
                    TotalDownloadCountList[local] = totalDownloadCount;
                    TotalDownloadedBytesList[local] = totalDownloadBytes;
                    CurrentDownloadedBytesList[local] = currentDownloadBytes;
                    AssetSystem.InvokeDownloading(this);
                }

                void OnDownloadOver(bool isSucceed)
                {
                    CurrentDownloaadeCountList.Remove(local);
                    TotalDownloadCountList.Remove(local);
                    CurrentDownloadedBytesList.Remove(local);
                    TotalDownloadedBytesList.Remove(local);
                    operations.Remove(local);
                    AssetSystem.InvokeDownloading(this);
                }

                CurrentDownloaadeCountList.Add(local, operation.CurrentDownloadCount); // 当前下载数量
                TotalDownloadCountList.Add(local, operation.TotalDownloadCount); // 总下载数量
                CurrentDownloadedBytesList.Add(local, operation.CurrentDownloadBytes); // 当前下载大小
                TotalDownloadedBytesList.Add(local, operation.TotalDownloadBytes); // 总下载大小
                operations.Add(local, operation);

                operation.OnDownloadProgressCallback += OnDownloadProgressCallback;
                operation.OnDownloadOverCallback += OnDownloadOver;
            }
        }

        private static LoadingInfo loadingInfo = new LoadingInfo();

        internal static void RegisterEvent(string package, string location, DownloaderOperation operation)
        {
            var record = new AssetSystem.SequenceRecord()
            {
                Name = package,
                Location = location,
                Time = DateTime.Now,
                Bytes = operation.TotalDownloadBytes,
                Count = operation.TotalDownloadCount,
                AssetPath = GetAssetInfo(package, location).AssetPath
            };
            AssetSystem.AddSequenceRecord(record);
            loadingInfo.RegisterEvent(location, operation);
        }

        /// <summary>
        /// 资源加载器 - 参数
        /// </summary>
        public static event Func<YAssetPackage, YAssetParameters> GetParameter;
    }
}
#endif