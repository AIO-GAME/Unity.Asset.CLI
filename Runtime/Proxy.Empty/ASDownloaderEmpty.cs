/*|============|*|
|*|Author:     |*| Star fire
|*|Date:       |*| 2024-01-04
|*|E-Mail:     |*| xinansky99@foxmail.com
|*|============|*/

using System;
using System.Collections;
using System.Linq;

namespace AIO
{
    internal class ASDownloaderEmpty : AOperation, IASDownloader
    {
        public bool Flow => false;

        public ASDownloaderEmpty(DownlandAssetEvent dEvent)
        {
            Event = dEvent;
        }

        public IEnumerator UpdateHeader()
        {
            yield break;
        }

        public void CollectNeedAll()
        {
            AssetSystem.WhiteAll = true;
        }

        public void CollectNeedTag(params string[] tags)
        {
            AssetSystem.AddWhite(AssetSystem.GetAssetInfos(tags));
        }

        protected override IEnumerator OnWaitCo()
        {
            Event.OnComplete?.Invoke(Report);
            yield break;
        }

        public Action<IProgressInfo> OnProgress { get; set; }
        public Action<IProgressReport> OnComplete { get; set; }

        /// <summary>开始回调</summary>
        public Action OnBegin { get; set; }

        public Action<Exception> OnError { get; set; }

        /// <summary>恢复</summary>
        public Action OnResume { get; set; }

        /// <summary>暂停</summary>
        public Action OnPause { get; set; }

        /// <summary>取消</summary>
        public Action OnCancel { get; set; }

        public Action<IProgressReport> OnNetReachableNot { get; set; }
        public Action<IProgressReport, Action> OnNetReachableCarrier { get; set; }
        public Action<IProgressReport> OnDiskSpaceNotEnough { get; set; }
        public Action<IProgressReport> OnWritePermissionNot { get; set; }
        public Action<IProgressReport> OnReadPermissionNot { get; set; }
    }
}