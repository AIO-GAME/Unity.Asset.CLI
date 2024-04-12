using IFilterRule = AIO.UEditor.IAssetRuleFilter;

namespace AIO.UEditor
{
    partial class RuleCollect
    {
        #region Nested type: CollectTextAsset

        private class CollectTextAsset : IFilterRule
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

        private class CollectTextAssetAsset : IFilterRule
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

        private class CollectTextAssetBytes : IFilterRule
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

        private class CollectTextAssetJson : IFilterRule
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

        private class CollectTextAssetLua : IFilterRule
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

        private class CollectTextAssetTxt : IFilterRule
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

        private class CollectTextAssetXml : IFilterRule
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

        private class CollectTextAssetYaml : IFilterRule
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