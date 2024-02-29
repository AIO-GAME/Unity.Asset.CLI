using IFilterRule = AIO.UEditor.IAssetRuleFilter;

namespace AIO.UEditor
{
    public partial class RuleCollect
    {
        public class CollectFontOTF : IFilterRule
        {
            public string DisplayFilterName => "Font/*.otf";

            public bool IsCollectAsset(AssetRuleData data) => data.Extension == "otf";
        }

        public class CollectFontTTC : IFilterRule
        {
            public string DisplayFilterName => "Font/*.ttc";

            public bool IsCollectAsset(AssetRuleData data) => data.Extension == "ttc";
        }

        public class CollectFontTTF : IFilterRule
        {
            public string DisplayFilterName => "Font/*.ttf";

            public bool IsCollectAsset(AssetRuleData data) => data.Extension == "ttf";
        }

        public class CollectFont : IFilterRule
        {
            public string DisplayFilterName => "Font/ALL";

            public bool IsCollectAsset(AssetRuleData data) =>
                new CollectFontOTF().IsCollectAsset(data) ||
                new CollectFontTTC().IsCollectAsset(data) ||
                new CollectFontTTF().IsCollectAsset(data);
        }
    }
}