using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using AIO.UEngine;

namespace AIO
{
    partial class AssetSystem
    {
        /// <summary>
        /// 获取下载器
        /// </summary>
        [DebuggerNonUserCode, DebuggerHidden]
        public static IASDownloader GetDownloader(DownlandAssetEvent dEvent = default)
        {
            return Parameter.ASMode != EASMode.Remote
                ? new ASDownloaderEmpty(dEvent)
                : Proxy.GetDownloader(dEvent);
        }

        /// <summary>
        /// 预下载指定标签资源
        /// </summary>
        [DebuggerNonUserCode, DebuggerHidden]
        public static IEnumerator DownloadTag(string tag, DownlandAssetEvent dEvent = default)
        {
            yield return DownloadTag(new[] { tag }, dEvent);
        }

        /// <summary>
        /// 预下载指定标签资源
        /// </summary>
        [DebuggerNonUserCode, DebuggerHidden]
        public static IEnumerator DownloadTagWithRecord(string tag, DownlandAssetEvent dEvent = default)
        {
            yield return DownloadTag(new[] { TagsRecord, tag }, dEvent);
        }

        /// <summary>
        /// 预下载指定标签资源
        /// </summary>
        [DebuggerNonUserCode, DebuggerHidden]
        public static IEnumerator DownloadTag(IEnumerable<string> tag, DownlandAssetEvent dEvent = default)
        {
            var enumerable = tag as string[] ?? tag.ToArray();
            if (Parameter.ASMode == EASMode.Remote)
            {
                using (var handle = Proxy.GetDownloader(dEvent))
                {
                    handle.Begin();
                    handle.CollectNeedTag(enumerable);
                    yield return handle.WaitCo();
                }
            }
            else WhiteListLocal.AddRange(GetAddressByTag(enumerable));
        }

        /// <summary>
        /// 预下载指定标签资源 + 记录序列资源
        /// </summary>
        [DebuggerNonUserCode, DebuggerHidden]
        public static IEnumerator DownloadTagWithRecord(IEnumerable<string> tag, DownlandAssetEvent dEvent = default)
        {
            var tags = new List<string>(tag) { TagsRecord };
            if (Parameter.ASMode == EASMode.Remote)
            {
                using (var handle = Proxy.GetDownloader(dEvent))
                {
                    handle.Begin();
                    handle.CollectNeedTag(tags.ToArray());
                    yield return handle.WaitCo();
                }
            }
            else
            {
                WhiteListLocal.AddRange(GetAddressByTag(tags));
                dEvent.OnComplete?.Invoke(new AProgress { State = EProgressState.Finish });
            }
        }

        /// <summary>
        /// 预下载记录序列资源
        /// </summary>
        [DebuggerNonUserCode, DebuggerHidden]
        public static IEnumerator DownloadRecord(DownlandAssetEvent dEvent = default)
        {
            if (Parameter.ASMode == EASMode.Remote)
            {
                using (var handle = Proxy.GetDownloader(dEvent))
                {
                    handle.Begin();
                    handle.CollectNeedTag(TagsRecord);
                    yield return handle.WaitCo();
                }
            }
            else
            {
                WhiteListLocal.AddRange(GetAddressByTag(TagsRecord));
                dEvent.OnComplete?.Invoke(new AProgress { State = EProgressState.Finish });
            }
        }

        /// <summary>
        /// 预下载全部远端资源
        /// </summary>
        [DebuggerNonUserCode, DebuggerHidden]
        public static IEnumerator DownloadAll(DownlandAssetEvent dEvent = default)
        {
            if (Parameter.ASMode == EASMode.Remote)
            {
                using (var handle = Proxy.GetDownloader(dEvent))
                {
                    handle.Begin();
                    handle.CollectNeedAll();
                    yield return handle.WaitCo();
                }
            }
            else
            {
                WhiteAll = true;
                dEvent.OnComplete?.Invoke(new AProgress { State = EProgressState.Finish });
            }
        }

        /// <summary>
        /// 动态下载远端资源
        /// </summary>
        [DebuggerNonUserCode, DebuggerHidden]
        public static IEnumerator DownloadHeader(DownlandAssetEvent dEvent = default)
        {
            if (Parameter.ASMode == EASMode.Remote)
            {
                using (var handle = Proxy.GetDownloader(dEvent))
                {
                    handle.Begin();
                    yield return handle.WaitCo();
                }
            }
            else dEvent.OnComplete?.Invoke(new AProgress { State = EProgressState.Finish });
        }
    }
}