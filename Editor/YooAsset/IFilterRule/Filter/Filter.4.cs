/*|✩ - - - - - |||
|||✩ Author:   ||| -> XINAN
|||✩ Date:     ||| -> 2023-08-18
|||✩ Document: ||| ->
|||✩ - - - - - |*/
#if SUPPORT_YOOASSET
using System.IO;
using YooAsset.Editor;

namespace AIO.UEditor.YooAsset
{
    [DisplayName("过滤 SpriteAtlas Prefab Material AudioClip")]
    public class FilterSpriteAtlasPrefabMaterialAudioClip : IFilterRule
    {
        public bool IsCollectAsset(FilterRuleData data)
        {
            var rule = new CollectAudioClip();
            if (new CollectSpriteAtlas().IsCollectAsset(data)) return false;
            if (new CollectPrefab().IsCollectAsset(data)) return false;
            if (new CollectMaterial().IsCollectAsset(data)) return false;
            if (new CollectAudioClip().IsCollectAsset(data)) return false;
            return true;
        }
    }
}
#endif