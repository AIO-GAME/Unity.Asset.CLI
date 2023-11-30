#if SUPPORT_YOOASSET
using System.IO;
using YooAsset.Editor;

namespace AIO.UEditor.YooAsset
{
    [DisplayName("收集 Shader")]
    public class CollectShader : IFilterRule
    {
        public bool IsCollectAsset(FilterRuleData data)
        {
            var Extension = Path.GetExtension(data.AssetPath).ToLower();
            return
                Extension == ".shader";
        }
    }
}
#endif