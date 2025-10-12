#region namespace

using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using AIO.UEngine;
using UnityEngine.Scripting;

#endregion

namespace AIO
{
    [Preserve]
    [StructLayout(LayoutKind.Auto)]
    internal sealed class OperationDownloadTag : OperationAction
    {
        /// <summary>
        /// 是否下载全部
        /// </summary>
        [Preserve]
        private readonly bool DownlandAll;

        /// <summary>
        /// 下载事件
        /// </summary>
        [Preserve]
        private readonly DownlandAssetEvent assetEvent;

        /// <summary>
        /// 标签列表
        /// </summary>
        [Preserve]
        private readonly string[] tags;

        /// <summary>
        /// 下载器
        /// </summary>
        [Preserve]
        private IASDownloader downloader;

        [Preserve]
        protected override void OnDispose() { downloader = null; }

        [Preserve]
        protected override void CreateSync()
        {
            if (AssetSystem.Parameter.ASMode == EASMode.Remote)
            {
                if (downloader is null)
                {
                    downloader = AssetSystem.Proxy.GetDownloader(assetEvent);
                    downloader.Begin();
                    if (DownlandAll) downloader.CollectNeedAll();
                    else if (tags != null) downloader.CollectNeedTag(tags);
                    downloader.Wait();
                }
                else downloader.Wait();
            }
            else
            {
                assetEvent.OnComplete?.Invoke(new AProgress { State = EProgressState.Finish });
            }

            foreach (var tag in AssetSystem.GetAddressByTag(tags)) AssetSystem.WhiteListLocal.Add(tag);
            IsDone = true;
        }

        #region CO

        [Preserve]
        protected override IEnumerator CreateCoroutine()
        {
            if (AssetSystem.Parameter.ASMode == EASMode.Remote)
            {
                if (downloader is null)
                {
                    downloader = AssetSystem.Proxy.GetDownloader(assetEvent);
                    downloader.Begin();
                    if (DownlandAll) downloader.CollectNeedAll();
                    else if (tags != null) downloader.CollectNeedTag(tags);
                    yield return downloader.WaitCo();
                }
                else yield return downloader.WaitCo();
            }
            else
            {
                assetEvent.OnComplete?.Invoke(new AProgress { State = EProgressState.Finish });
            }

            foreach (var tag in AssetSystem.GetAddressByTag(tags)) AssetSystem.WhiteListLocal.Add(tag);
            InvokeOnCompleted();
        }

        #endregion

        #region Task

        [Preserve]
        protected override TaskAwaiter CreateAsync()
        {
            if (AssetSystem.Parameter.ASMode == EASMode.Remote)
            {
                downloader = AssetSystem.Proxy.GetDownloader(assetEvent);
                downloader.Begin();
                if (DownlandAll) downloader.CollectNeedAll();
                else if (tags != null) downloader.CollectNeedTag(tags);
                var awaiter = downloader.WaitAsync().GetAwaiter();
                awaiter.OnCompleted(InvokeOnCompleted);
                return awaiter;
            }
            else
            {
                var awaiter = Task.CompletedTask.GetAwaiter();
                awaiter.OnCompleted(InvokeOnCompleted);
                return awaiter;
            }
        }

        #endregion

        #region Constructor

        [Preserve]
        public OperationDownloadTag(bool isAll, DownlandAssetEvent assetEvent)
        {
            DownlandAll     = isAll;
            this.assetEvent = assetEvent;
        }

        [Preserve]
        public OperationDownloadTag(string[] tags, DownlandAssetEvent assetEvent)
        {
            DownlandAll     = false;
            this.tags       = tags;
            this.assetEvent = assetEvent;
        }

        #endregion
    }

    partial class AssetSystem
    {
        /// <summary>
        ///     获取下载器
        /// </summary>
        [DebuggerNonUserCode, DebuggerHidden] [Preserve]
        public static IASDownloader GetDownloader(DownlandAssetEvent assetEvent = default(DownlandAssetEvent))
        {
            return Parameter.ASMode != EASMode.Remote
                ? new ASDownloaderEmpty(assetEvent)
                : Proxy.GetDownloader(assetEvent);
        }

        /// <summary>
        ///     预下载指定标签资源
        /// </summary>
        [DebuggerNonUserCode, DebuggerHidden] [Preserve]
        public static IOperationAction DownloadTag(string tag, DownlandAssetEvent assetEvent = default) { return new OperationDownloadTag(new[] { tag }, assetEvent); }

        /// <summary>
        ///     预下载指定标签资源
        /// </summary>
        [DebuggerNonUserCode, DebuggerHidden] [Preserve]
        public static IOperationAction DownloadTagWithRecord(string tag, DownlandAssetEvent assetEvent = default) { return new OperationDownloadTag(new[] { TagsRecord, tag }, assetEvent); }

        /// <summary>
        ///     预下载指定标签资源
        /// </summary>
        [DebuggerNonUserCode, DebuggerHidden] [Preserve]
        public static IOperationAction DownloadTag(IEnumerable<string> tag, DownlandAssetEvent assetEvent = default) { return new OperationDownloadTag(tag.ToArray(), assetEvent); }

        /// <summary>
        ///     预下载指定标签资源 + 记录序列资源
        /// </summary>
        [DebuggerNonUserCode, DebuggerHidden] [Preserve]
        public static IOperationAction DownloadTagWithRecord(IEnumerable<string> tag, DownlandAssetEvent assetEvent = default) { return new OperationDownloadTag(new[] { TagsRecord }.Concat(tag).ToArray(), assetEvent); }

        /// <summary>
        ///     预下载记录序列资源
        /// </summary>
        [DebuggerNonUserCode, DebuggerHidden] [Preserve]
        public static IOperationAction DownloadRecord(DownlandAssetEvent assetEvent = default) { return new OperationDownloadTag(new[] { TagsRecord }, assetEvent); }

        /// <summary>
        ///     预下载全部远端资源
        /// </summary>
        [DebuggerNonUserCode, DebuggerHidden] [Preserve]
        public static IOperationAction DownloadAll(DownlandAssetEvent assetEvent = default) { return new OperationDownloadTag(true, assetEvent); }

        /// <summary>
        ///     动态下载远端资源
        /// </summary>
        [DebuggerNonUserCode, DebuggerHidden] [Preserve]
        public static IOperationAction DownloadHeader(DownlandAssetEvent assetEvent = default) { return new OperationDownloadTag(false, assetEvent); }
    }
}