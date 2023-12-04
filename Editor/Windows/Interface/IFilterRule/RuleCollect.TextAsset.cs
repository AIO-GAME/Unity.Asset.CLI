/*|============|*|
|*|Author:     |*| Star fire
|*|Date:       |*| 2023-12-04
|*|E-Mail:     |*| xinansky99@foxmail.com
|*|============|*/

using System.Collections.Generic;
using System.Linq;
using System.Text;
using FilterRuleData = AIO.UEditor.AssetInfoData;
using IFilterRule = AIO.UEditor.IAssetRuleFilter;

namespace AIO.UEditor
{
    public partial class RuleCollect
    {
        public class CollectTextAssetAsset : IFilterRule
        {
            public string DisplayFilterName => "TextAsset/*.asset";

            public bool IsCollectAsset(FilterRuleData data) => data.Extension == "asset";
        }

        public class CollectTextAssetLua : IFilterRule
        {
            public string DisplayFilterName => "TextAsset/*.lua";

            public bool IsCollectAsset(FilterRuleData data) => data.Extension == "lua";
        }

        public class CollectTextAssetTxt : IFilterRule
        {
            public string DisplayFilterName => "TextAsset/*.txt";

            public bool IsCollectAsset(FilterRuleData data) => data.Extension == "txt";
        }

        public class CollectTextAssetJson : IFilterRule
        {
            public string DisplayFilterName => "TextAsset/*.json";

            public bool IsCollectAsset(FilterRuleData data) => data.Extension == "json";
        }

        public class CollectTextAssetXml : IFilterRule
        {
            public string DisplayFilterName => "TextAsset/*.xml";

            public bool IsCollectAsset(FilterRuleData data) => data.Extension == "xml";
        }

        public class CollectTextAssetBytes : IFilterRule
        {
            public string DisplayFilterName => "TextAsset/*.bytes";

            public bool IsCollectAsset(FilterRuleData data) => data.Extension == "bytes";
        }

        public class CollectTextAssetYaml : IFilterRule
        {
            public string DisplayFilterName => "TextAsset/*.yaml";

            public bool IsCollectAsset(FilterRuleData data) => data.Extension == "yaml";
        }

        public class CollectTextAsset : IFilterRule
        {
            public string DisplayFilterName => "TextAsset/ALL";

            public bool IsCollectAsset(FilterRuleData data) =>
                new CollectTextAssetAsset().IsCollectAsset(data) ||
                new CollectTextAssetLua().IsCollectAsset(data) ||
                new CollectTextAssetTxt().IsCollectAsset(data) ||
                new CollectTextAssetJson().IsCollectAsset(data) ||
                new CollectTextAssetYaml().IsCollectAsset(data) ||
                new CollectTextAssetBytes().IsCollectAsset(data) ||
                new CollectTextAssetXml().IsCollectAsset(data);
        }
    }
}