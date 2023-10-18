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
    [DisplayName("过滤 SpriteAtlas Prefab Material")]
    public class FilterSpriteAtlasPrefabMaterial : IFilterRule
    {
        public bool IsCollectAsset(FilterRuleData data)
        {
            if (new CollectSpriteAtlas().IsCollectAsset(data)) return false;
            if (new CollectPrefab().IsCollectAsset(data)) return false;
            if (new CollectMaterial().IsCollectAsset(data)) return false;
            return true;
        }
    }

    [DisplayName("过滤 Scene Prefab Material")]
    public class FilterScenePrefabMaterial : IFilterRule
    {
        public bool IsCollectAsset(FilterRuleData data)
        {
            if (new CollectScene().IsCollectAsset(data)) return false;
            if (new CollectPrefab().IsCollectAsset(data)) return false;
            if (new CollectMaterial().IsCollectAsset(data)) return false;
            return true;
        }
    }

    [DisplayName("过滤 Scene Prefab Asset")]
    public class FilterScenePrefabAsset : IFilterRule
    {
        public bool IsCollectAsset(FilterRuleData data)
        {
            if (new CollectScene().IsCollectAsset(data)) return false;
            if (new CollectPrefab().IsCollectAsset(data)) return false;
            if (new CollectAsset().IsCollectAsset(data)) return false;
            return true;
        }
    }

    [DisplayName("过滤 SpriteAtlas Prefab Scene")]
    public class FilterSpriteAtlasPrefabScene : IFilterRule
    {
        public bool IsCollectAsset(FilterRuleData data)
        {
            if (new CollectScene().IsCollectAsset(data)) return false;
            if (new CollectPrefab().IsCollectAsset(data)) return false;
            if (new CollectSpriteAtlas().IsCollectAsset(data)) return false;
            return true;
        }
    }
}
#endif