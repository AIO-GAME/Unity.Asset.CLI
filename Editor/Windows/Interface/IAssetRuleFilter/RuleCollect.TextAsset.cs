using IFilterRule = AIO.UEditor.IAssetRuleFilter;

namespace AIO.UEditor
{
    partial class RuleCollect
    {
        #region Nested type: CollectTextAsset

        public class CollectTextAsset : IFilterRule
        {
            #region IFilterRule Members

            public string DisplayFilterName => "TextAsset/ALL";

            public bool IsCollectAsset(AssetRuleData data)
            {
                return new CollectTextAssetAsset().IsCollectAsset(data) ||
                       new CollectTextAssetLua().IsCollectAsset(data) ||
                       new CollectTextAssetTxt().IsCollectAsset(data) ||
                       new CollectTextAssetJson().IsCollectAsset(data) ||
                       new CollectTextAssetYaml().IsCollectAsset(data) ||
                       new CollectTextAssetBytes().IsCollectAsset(data) ||
                       new CollectTextAssetXml().IsCollectAsset(data);
            }

            #endregion
        }

        #endregion

        #region Nested type: CollectTextAssetAsset

        public class CollectTextAssetAsset : IFilterRule
        {
            #region IFilterRule Members

            public string DisplayFilterName => "TextAsset/*.asset";

            public bool IsCollectAsset(AssetRuleData data)
            {
                return data.Extension == "asset";
            }

            #endregion
        }

        #endregion

        #region Nested type: CollectTextAssetBytes

        public class CollectTextAssetBytes : IFilterRule
        {
            #region IFilterRule Members

            public string DisplayFilterName => "TextAsset/*.bytes";

            public bool IsCollectAsset(AssetRuleData data)
            {
                return data.Extension == "bytes";
            }

            #endregion
        }

        #endregion

        #region Nested type: CollectTextAssetJson

        public class CollectTextAssetJson : IFilterRule
        {
            #region IFilterRule Members

            public string DisplayFilterName => "TextAsset/*.json";

            public bool IsCollectAsset(AssetRuleData data)
            {
                return data.Extension == "json";
            }

            #endregion
        }

        #endregion

        #region Nested type: CollectTextAssetLua

        public class CollectTextAssetLua : IFilterRule
        {
            #region IFilterRule Members

            public string DisplayFilterName => "TextAsset/*.lua";

            public bool IsCollectAsset(AssetRuleData data)
            {
                return data.Extension == "lua";
            }

            #endregion
        }

        #endregion

        #region Nested type: CollectTextAssetTxt

        public class CollectTextAssetTxt : IFilterRule
        {
            #region IFilterRule Members

            public string DisplayFilterName => "TextAsset/*.txt";

            public bool IsCollectAsset(AssetRuleData data)
            {
                return data.Extension == "txt";
            }

            #endregion
        }

        #endregion

        #region Nested type: CollectTextAssetXml

        public class CollectTextAssetXml : IFilterRule
        {
            #region IFilterRule Members

            public string DisplayFilterName => "TextAsset/*.xml";

            public bool IsCollectAsset(AssetRuleData data)
            {
                return data.Extension == "xml";
            }

            #endregion
        }

        #endregion

        #region Nested type: CollectTextAssetYaml

        public class CollectTextAssetYaml : IFilterRule
        {
            #region IFilterRule Members

            public string DisplayFilterName => "TextAsset/*.yaml";

            public bool IsCollectAsset(AssetRuleData data)
            {
                return data.Extension == "yaml";
            }

            #endregion
        }

        #endregion
    }
}