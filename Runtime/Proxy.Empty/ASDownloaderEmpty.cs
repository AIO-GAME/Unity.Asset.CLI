/*|============|*|
|*|Author:     |*| Star fire
|*|Date:       |*| 2024-01-04
|*|E-Mail:     |*| xinansky99@foxmail.com
|*|============|*/

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

        public void CollectNeedRecord()
        {
            AssetSystem.AddWhite(AssetSystem.SequenceRecords.Select(x => x.Location));
        }

        protected override IEnumerator OnWaitCo()
        {
            Event.OnComplete?.Invoke(Report);
            yield break;
        }
    }
}