#if SUPPORT_YOOASSET
using System.IO;
using YooAsset.Editor;
using UnityEngine;

namespace AIO.UEditor.YooAsset
{
    [DisplayName("收集 Prefab")]
    public class CollectPrefab : IFilterRule
    {
        public bool IsCollectAsset(FilterRuleData data)
        {
            var Extension = Path.GetExtension(data.AssetPath).ToLower();
            return
                Extension == ".prefab";
        }
    }
}
#endif