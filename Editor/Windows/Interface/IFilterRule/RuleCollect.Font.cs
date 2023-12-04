/*|============|*|
|*|Author:     |*| Star fire
|*|Date:       |*| 2023-12-04
|*|E-Mail:     |*| xinansky99@foxmail.com
|*|============|*/

using FilterRuleData = AIO.UEditor.AssetInfoData;
using IFilterRule = AIO.UEditor.IAssetRuleFilter;

namespace AIO.UEditor
{
    public partial class RuleCollect
    {
        public class CollectFontOTF : IFilterRule
        {
            public string DisplayFilterName => "Font/*.otf";

            public bool IsCollectAsset(FilterRuleData data) => data.Extension == "otf";
        }

        public class CollectFontTTC : IFilterRule
        {
            public string DisplayFilterName => "Font/*.ttc";

            public bool IsCollectAsset(FilterRuleData data) => data.Extension == "ttc";
        }

        public class CollectFontTTF : IFilterRule
        {
            public string DisplayFilterName => "Font/*.ttf";

            public bool IsCollectAsset(FilterRuleData data) => data.Extension == "ttf";
        }

        public class CollectFont : IFilterRule
        {
            public string DisplayFilterName => "Font/ALL";

            public bool IsCollectAsset(FilterRuleData data) =>
                new CollectFontOTF().IsCollectAsset(data) ||
                new CollectFontTTC().IsCollectAsset(data) ||
                new CollectFontTTF().IsCollectAsset(data);
        }
    }
}