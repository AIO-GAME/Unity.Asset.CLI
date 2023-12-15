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
    internal struct ASDownloaderEmpty : IASDownloader
    {
        public IProgressHandle Progress { get; }

        public bool Flow => false;

        public ASDownloaderEmpty(IProgressEvent iEvent = null)
        {
            Progress = new AProgress(iEvent);
        }

        public Task UpdatePackageVersionTask() => Task.CompletedTask;
        public Task UpdatePackageManifestTask() => Task.CompletedTask;
        public Task DownloadRecordTask(AssetSystem.SequenceRecordQueue queue) => Task.CompletedTask;
        public Task DownloadTask() => Task.CompletedTask;
        public Task DownloadTagTask(string tag) => Task.CompletedTask;
        public Task DownloadTagTask(IEnumerable<string> tags) => Task.CompletedTask;
        public Task DownloadRecordTask() => Task.CompletedTask;

        public IEnumerator UpdatePackageVersionCO()
        {
            yield break;
        }

        public IEnumerator UpdatePackageManifestCO()
        {
            yield break;
        }

        public void CollectNeedAll()
        {
        }

        public void CollectNeedTag(IEnumerable<string> tags)
        {
        }

        public void CollectNeedRecord(AssetSystem.SequenceRecordQueue queue)
        {
        }

        public IEnumerator WaitCo()
        {
            yield break;
        }

        public void Dispose()
        {
        }
    }

    public partial class AssetSystem
    {
        private static IASDownloader Downloader;

        /// <summary>
        /// 获取下载器
        /// </summary>
        [DebuggerNonUserCode, DebuggerHidden]
        public static IASDownloader GetDownloader(IProgressEvent progress = null)
        {
            return Parameter.ASMode != EASMode.Remote
                ? new ASDownloaderEmpty(progress)
                : Proxy.GetDownloader(progress);
        }

        /// <summary>
        /// 预下载指定标签资源
        /// </summary>
        public static IEnumerator DownloadTag(IEnumerable<string> tag, IProgressEvent progress = null)
        {
            var enumerable = tag as string[] ?? tag.ToArray();
            if (Parameter.ASMode == EASMode.Remote)
            {
                using (var handle = Proxy.GetDownloader(progress))
                {
                    yield return handle.UpdatePackageManifestCO();
                    yield return handle.UpdatePackageVersionCO();
                    handle.CollectNeedTag(enumerable);
                    yield return handle.WaitCo();
                }
            }

            WhiteListLocal.AddRange(GetAssetInfos(enumerable));
        }

        /// <summary>
        /// 预下载指定标签资源 + 记录序列资源
        /// </summary>
        public static IEnumerator DownloadTagWithRecord(IEnumerable<string> tag, IProgressEvent progress = null)
        {
            var enumerable = tag as string[] ?? tag.ToArray();
            if (Parameter.ASMode == EASMode.Remote)
            {
                using (var handle = Proxy.GetDownloader(progress))
                {
                    yield return handle.UpdatePackageManifestCO();
                    yield return handle.UpdatePackageVersionCO();
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
        public static IEnumerator DownloadRecord(IProgressEvent progress = null)
        {
            if (Parameter.ASMode == EASMode.Remote)
            {
                using (var handle = Proxy.GetDownloader(progress))
                {
                    yield return handle.UpdatePackageManifestCO();
                    yield return handle.UpdatePackageVersionCO();
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
        public static IEnumerator DownloadAll(IProgressEvent progress = null)
        {
            if (Parameter.ASMode != EASMode.Remote) yield break;
            using (var handle = Proxy.GetDownloader(progress))
            {
                yield return handle.UpdatePackageManifestCO();
                yield return handle.UpdatePackageVersionCO();
                handle.CollectNeedAll();
                yield return handle.WaitCo();
            }
        }

        /// <summary>
        /// 动态下载远端资源
        /// </summary>
        [DebuggerNonUserCode, DebuggerHidden]
        public static IEnumerator DownloadHeader(IProgressEvent progress = null)
        {
            if (Parameter.ASMode != EASMode.Remote) yield break;
            using (var handle = Proxy.GetDownloader(progress))
            {
                yield return handle.UpdatePackageManifestCO();
                yield return handle.UpdatePackageVersionCO();
                yield return handle.WaitCo();
            }
        }
    }
}