using System;
using System.Collections;

namespace AIO
{
    internal class ASDownloaderEmpty : AOperation, IASDownloader
    {
        public bool Flow => false;

        public ASDownloaderEmpty(DownlandAssetEvent dEvent)
        {
            Event = dEvent;
        }

        public void CollectNeedAll()
        {
            AssetSystem.WhiteAll = true;
        }

        public void CollectNeedTag(params string[] tags)
        {
            AssetSystem.AddWhite(AssetSystem.GetAddressByTag(tags));
        }

        protected override IEnumerator OnWaitCo()
        {
            Event.OnComplete?.Invoke(Report);
            yield break;
        }

        /// <summary>开始回调</summary>
        public new Action OnBegin { get; set; }

        /// <summary>恢复</summary>
        public new Action OnResume { get; set; }

        /// <summary>暂停</summary>
        public new Action OnPause { get; set; }

        /// <summary>取消</summary>
        public new Action OnCancel { get; set; }

        public Action<Exception> OnError { get; set; }
        public Action<IProgressInfo> OnProgress { get; set; }
        public Action<IProgressReport> OnComplete { get; set; }
        public Action<IProgressReport> OnNetReachableNot { get; set; }
        public Action<IProgressReport, Action> OnNetReachableCarrier { get; set; }
        public Action<IProgressReport> OnDiskSpaceNotEnough { get; set; }
        public Action<IProgressReport> OnWritePermissionNot { get; set; }
        public Action<IProgressReport> OnReadPermissionNot { get; set; }
    }
}