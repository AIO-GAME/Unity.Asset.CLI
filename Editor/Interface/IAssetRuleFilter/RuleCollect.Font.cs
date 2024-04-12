using IFilterRule = AIO.UEditor.IAssetRuleFilter;

namespace AIO.UEditor
{
    partial class RuleCollect
    {
        #region Nested type: CollectFont

        private class CollectFont : IFilterRule
        {
            #region IFilterRule Members

            public string DisplayFilterName => "Font/ALL";

            public bool IsCollectAsset(AssetRuleData data)
            {
                return new CollectFontOTF().IsCollectAsset(data) ||
                       new CollectFontTTC().IsCollectAsset(data) ||
                       new CollectFontTTF().IsCollectAsset(data);
            }

            #endregion
        }

        #endregion

        #region Nested type: CollectFontOTF

        private class CollectFontOTF : IFilterRule
        {
            #region IFilterRule Members

            public string DisplayFilterName => "Font/*.otf";

            public bool IsCollectAsset(AssetRuleData data)
            {
                return data.Extension == "otf";
            }

            #endregion
        }

        #endregion

        #region Nested type: CollectFontTTC

        private class CollectFontTTC : IFilterRule
        {
            #region IFilterRule Members

            public string DisplayFilterName => "Font/*.ttc";

            public bool IsCollectAsset(AssetRuleData data)
            {
                return data.Extension == "ttc";
            }

            #endregion
        }

        #endregion

        #region Nested type: CollectFontTTF

        private class CollectFontTTF : IFilterRule
        {
            #region IFilterRule Members

            public string DisplayFilterName => "Font/*.ttf";

            public bool IsCollectAsset(AssetRuleData data)
            {
                return data.Extension == "ttf";
            }

            #endregion
        }

        #endregion
    }
}