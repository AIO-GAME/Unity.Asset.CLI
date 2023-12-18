/*|✩ - - - - - |||
|||✩ Author:   ||| -> xi nan
|||✩ Date:     ||| -> 2023-08-22
|||✩ Document: ||| ->
|||✩ - - - - - |*/

using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using AIO.UEngine;

namespace AIO
{
    internal class ASDownloaderEmpty : AOperation, IASDownloader
    {
        public bool Flow => false;

        public Task UpdatePackageVersionTask() => Task.CompletedTask;
        public Task UpdatePackageManifestTask() => Task.CompletedTask;
        public Task DownloadRecordTask(AssetSystem.SequenceRecordQueue queue) => Task.CompletedTask;
        public Task DownloadTask() => Task.CompletedTask;
        public Task DownloadTagTask(string tag) => Task.CompletedTask;
        public Task DownloadTagTask(IEnumerable<string> tags) => Task.CompletedTask;
        public Task DownloadRecordTask() => Task.CompletedTask;

        public IEnumerator UpdateHeader()
        {
            yield break;
        }

        public void CollectNeedAll()
        {
        }

        public void CollectNeedTag(params string[] tags)
        {
        }

        public void CollectNeedRecord(AssetSystem.SequenceRecordQueue queue)
        {
        }

        public IEnumerator WaitCo()
        {
            yield break;
        }
    }

    public partial class AssetSystem
    {
        private static IASDownloader Downloader;

        /// <summary>
        /// 获取下载器
        /// </summary>
        [DebuggerNonUserCode, DebuggerHidden]
        public static IASDownloader GetDownloader(DownlandAssetEvent dEvent = default)
        {
            return Parameter.ASMode != EASMode.Remote
                ? new ASDownloaderEmpty()
                : Proxy.GetDownloader(dEvent);
        }

        /// <summary>
        /// 预下载指定标签资源
        /// </summary>
        public static IEnumerator DownloadTag(string tag, DownlandAssetEvent dEvent = default)
        {
            yield return DownloadTag(new[] { tag }, dEvent);
        }

        /// <summary>
        /// 预下载指定标签资源
        /// </summary>
        public static IEnumerator DownloadTagWithRecord(string tag, DownlandAssetEvent dEvent = default)
        {
            yield return DownloadTagWithRecord(new[] { tag }, dEvent);
        }

        /// <summary>
        /// 预下载指定标签资源
        /// </summary>
        public static IEnumerator DownloadTag(IEnumerable<string> tag, DownlandAssetEvent dEvent = default)
        {
            var enumerable = tag as string[] ?? tag.ToArray();
            if (Parameter.ASMode == EASMode.Remote)
            {
                using (var handle = Proxy.GetDownloader(dEvent))
                {
                    yield return handle.UpdateHeader();
                    handle.Begin();
                    handle.CollectNeedTag(enumerable);
                    yield return handle.WaitCo();
                }
            }

            WhiteListLocal.AddRange(GetAssetInfos(enumerable));
        }

        /// <summary>
        /// 预下载指定标签资源 + 记录序列资源
        /// </summary>
        public static IEnumerator DownloadTagWithRecord(IEnumerable<string> tag, DownlandAssetEvent dEvent = default)
        {
            var enumerable = tag as string[] ?? tag.ToArray();
            if (Parameter.ASMode == EASMode.Remote)
            {
                using (var handle = Proxy.GetDownloader(dEvent))
                {
                    yield return handle.UpdateHeader();
                    handle.Begin();
                    handle.CollectNeedTag(enumerable);
                    handle.CollectNeedRecord(SequenceRecords);
                    yield return handle.WaitCo();
                }
            }

            WhiteListLocal.AddRange(SequenceRecords.Select(x => x.Location));
            WhiteListLocal.AddRange(GetAssetInfos(enumerable));
        }

        /// <summary>
        /// 预下载记录序列资源
        /// </summary>
        public static IEnumerator DownloadRecord(DownlandAssetEvent dEvent = default)
        {
            if (Parameter.ASMode == EASMode.Remote)
            {
                using (var handle = Proxy.GetDownloader(dEvent))
                {
                    yield return handle.UpdateHeader();
                    handle.Begin();
                    handle.CollectNeedRecord(SequenceRecords);
                    yield return handle.WaitCo();
                }
            }

            WhiteListLocal.AddRange(SequenceRecords.Select(x => x.Location));
        }

        /// <summary>
        /// 预下载全部远端资源
        /// </summary>
        [DebuggerNonUserCode, DebuggerHidden]
        public static IEnumerator DownloadAll(DownlandAssetEvent dEvent = default)
        {
            if (Parameter.ASMode != EASMode.Remote) yield break;
            using (var handle = Proxy.GetDownloader(dEvent))
            {
                yield return handle.UpdateHeader();
                handle.Begin();
                handle.CollectNeedAll();
                yield return handle.WaitCo();
            }
        }

        /// <summary>
        /// 动态下载远端资源
        /// </summary>
        [DebuggerNonUserCode, DebuggerHidden]
        public static IEnumerator DownloadHeader(DownlandAssetEvent dEvent = default)
        {
            if (Parameter.ASMode != EASMode.Remote) yield break;
            using (var handle = Proxy.GetDownloader(dEvent))
            {
                yield return handle.UpdateHeader();
            }
        }
    }
}