using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using AIO.UEngine;

namespace AIO
{
    [StructLayout(LayoutKind.Auto)]
    internal sealed class OperationDownloadTag : OperationAction
    {
        /// <summary>
        /// 是否下载全部
        /// </summary>
        private readonly bool DownlandAll;

        /// <summary>
        /// 下载事件
        /// </summary>
        private readonly DownlandAssetEvent assetEvent;

        /// <summary>
        /// 标签列表
        /// </summary>
        private readonly string[] tags;

        /// <summary>
        /// 下载器
        /// </summary>
        private IASDownloader downloader;

        protected override void OnDispose()
        {
            downloader = null;
        }

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

            AssetSystem.WhiteListLocal.AddRange(AssetSystem.GetAddressByTag(tags));
            InvokeOnCompleted();
        }

        #region CO

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

            AssetSystem.WhiteListLocal.AddRange(AssetSystem.GetAddressByTag(tags));
            InvokeOnCompleted();
        }

        #endregion

        #region Task

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

        public OperationDownloadTag(bool isAll, DownlandAssetEvent assetEvent)
        {
            DownlandAll     = isAll;
            this.assetEvent = assetEvent;
        }

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
        [DebuggerNonUserCode, DebuggerHidden]
        public static IASDownloader GetDownloader(DownlandAssetEvent assetEvent = default)
        {
            return Parameter.ASMode != EASMode.Remote
                ? new ASDownloaderEmpty(assetEvent)
                : Proxy.GetDownloader(assetEvent);
        }

        /// <summary>
        ///     预下载指定标签资源
        /// </summary>
        [DebuggerNonUserCode, DebuggerHidden]
        public static IOperationAction DownloadTag(string tag, DownlandAssetEvent assetEvent = default)
        {
            return new OperationDownloadTag(new[] { tag }, assetEvent);
        }

        /// <summary>
        ///     预下载指定标签资源
        /// </summary>
        [DebuggerNonUserCode, DebuggerHidden]
        public static IOperationAction DownloadTagWithRecord(string tag, DownlandAssetEvent assetEvent = default)
        {
            return new OperationDownloadTag(new[] { TagsRecord, tag }, assetEvent);
        }

        /// <summary>
        ///     预下载指定标签资源
        /// </summary>
        [DebuggerNonUserCode, DebuggerHidden]
        public static IOperationAction DownloadTag(IEnumerable<string> tag, DownlandAssetEvent assetEvent = default)
        {
            return new OperationDownloadTag(tag.ToArray(), assetEvent);
        }

        /// <summary>
        ///     预下载指定标签资源 + 记录序列资源
        /// </summary>
        [DebuggerNonUserCode, DebuggerHidden]
        public static IOperationAction DownloadTagWithRecord(IEnumerable<string> tag, DownlandAssetEvent assetEvent = default)
        {
            return new OperationDownloadTag(new[] { TagsRecord }.Concat(tag).ToArray(), assetEvent);
        }

        /// <summary>
        ///     预下载记录序列资源
        /// </summary>
        [DebuggerNonUserCode, DebuggerHidden]
        public static IOperationAction DownloadRecord(DownlandAssetEvent assetEvent = default)
        {
            return new OperationDownloadTag(new[] { TagsRecord }, assetEvent);
        }

        /// <summary>
        ///     预下载全部远端资源
        /// </summary>
        [DebuggerNonUserCode, DebuggerHidden]
        public static IOperationAction DownloadAll(DownlandAssetEvent assetEvent = default)
        {
            return new OperationDownloadTag(true, assetEvent);
        }

        /// <summary>
        ///     动态下载远端资源
        /// </summary>
        [DebuggerNonUserCode, DebuggerHidden]
        public static IOperationAction DownloadHeader(DownlandAssetEvent assetEvent = default)
        {
            return new OperationDownloadTag(false, assetEvent);
        }
    }
}