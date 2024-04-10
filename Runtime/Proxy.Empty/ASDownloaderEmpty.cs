#region

using System;
using System.Collections;
using System.Threading.Tasks;

#endregion

namespace AIO
{
    internal class ASDownloaderEmpty : AOperation, IASDownloader
    {
        public ASDownloaderEmpty(DownlandAssetEvent dEvent)
        {
            Event = dEvent;
        }

        #region IASDownloader Members

        public bool Flow => false;

        public void CollectNeedAll()
        {
            AssetSystem.WhiteAll = true;
        }

        public void CollectNeedTag(params string[] tags)
        {
            AssetSystem.AddWhite(AssetSystem.GetAddressByTag(tags));
        }

        Action IProgressEvent.                              OnBegin               { get; set; }
        Action<Exception> IProgressEvent.                   OnError               { get; set; }
        Action IProgressEvent.                              OnResume              { get; set; }
        Action IProgressEvent.                              OnPause               { get; set; }
        Action IProgressEvent.                              OnCancel              { get; set; }
        Action<IProgressInfo> IProgressEvent.               OnProgress            { get; set; }
        Action<IProgressReport> IProgressEvent.             OnComplete            { get; set; }
        Action<IProgressReport> IDownlandAssetEvent.        OnNetReachableNot     { get; set; }
        Action<IProgressReport, Action> IDownlandAssetEvent.OnNetReachableCarrier { get; set; }
        Action<IProgressReport> IDownlandAssetEvent.        OnDiskSpaceNotEnough  { get; set; }
        Action<IProgressReport> IDownlandAssetEvent.        OnWritePermissionNot  { get; set; }
        Action<IProgressReport> IDownlandAssetEvent.        OnReadPermissionNot   { get; set; }

        #endregion

        protected override void OnWait()
        {
            Event.OnComplete?.Invoke(Report);
        }

        protected override Task OnWaitAsync()
        {
            Event.OnComplete?.Invoke(Report);
            return base.OnWaitAsync();
        }

        protected override IEnumerator OnWaitCo()
        {
            Event.OnComplete?.Invoke(Report);
            yield break;
        }
    }
}