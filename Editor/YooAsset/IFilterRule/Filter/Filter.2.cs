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
    [DisplayName("过滤 SpriteAtlas Prefab")]
    public class FilterRuleSpriteAtlasPrefab : IFilterRule
    {
        public bool IsCollectAsset(FilterRuleData data)
        {
            if (new CollectSpriteAtlas().IsCollectAsset(data)) return false;
            if (new CollectPrefab().IsCollectAsset(data)) return false;
            return true;
        }
    }

    [DisplayName("过滤 Scene Prefab")]
    public class FilterScenePrefab : IFilterRule
    {
        public bool IsCollectAsset(FilterRuleData data)
        {
            if (new CollectScene().IsCollectAsset(data)) return false;
            if (new CollectPrefab().IsCollectAsset(data)) return false;
            return true;
        }
    }
}
#endif