/*|✩ - - - - - |||
|||✩ Author:   ||| -> XINAN
|||✩ Date:     ||| -> 2023-08-02
|||✩ Document: ||| ->
|||✩ - - - - - |*/

#if SUPPORT_YOOASSET
using YooAsset.Editor;

namespace AIO.UEditor.YooAsset
{
    [DisplayName("过滤 Prefab")]
    public class FilterPrefab : IFilterRule
    {
        public bool IsCollectAsset(FilterRuleData data)
        {
            IFilterRule rule = new CollectPrefab();
            if (rule.IsCollectAsset(data)) return false;
            return true;
        }
    }

    [DisplayName("过滤 SpriteAtlas")]
    public class FilterSpriteAtlas : IFilterRule
    {
        public bool IsCollectAsset(FilterRuleData data)
        {
            IFilterRule rule = new CollectSpriteAtlas();
            if (rule.IsCollectAsset(data)) return false;
            return true;
        }
    }
}
#endif