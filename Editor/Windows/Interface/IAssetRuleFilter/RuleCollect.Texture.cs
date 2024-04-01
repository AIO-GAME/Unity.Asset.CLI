using IFilterRule = AIO.UEditor.IAssetRuleFilter;

namespace AIO.UEditor
{
    partial class RuleCollect
    {
        #region Nested type: CollectTexture

        public class CollectTexture : IFilterRule
        {
            #region IFilterRule Members

            public string DisplayFilterName => "Texture/ALL";

            public bool IsCollectAsset(AssetRuleData data)
            {
                return new CollectTextureSpriteAtlas().IsCollectAsset(data) ||
                       new CollectTexturePNG().IsCollectAsset(data) ||
                       new CollectTextureJPG().IsCollectAsset(data) ||
                       new CollectTextureTGA().IsCollectAsset(data) ||
                       new CollectTexturePSD().IsCollectAsset(data) ||
                       new CollectTextureBMP().IsCollectAsset(data) ||
                       new CollectTextureEXR().IsCollectAsset(data) ||
                       new CollectTextureHDR().IsCollectAsset(data) ||
                       data.Extension == "gif" ||
                       data.Extension == "svg" ||
                       data.Extension == "webp";
            }

            #endregion
        }

        #endregion

        #region Nested type: CollectTextureBMP

        public class CollectTextureBMP : IFilterRule
        {
            #region IFilterRule Members

            public string DisplayFilterName => "Texture/*.bmp";

            public bool IsCollectAsset(AssetRuleData data)
            {
                return data.Extension == "bmp";
            }

            #endregion
        }

        #endregion

        #region Nested type: CollectTextureEXR

        public class CollectTextureEXR : IFilterRule
        {
            #region IFilterRule Members

            public string DisplayFilterName => "Texture/*.exr";

            public bool IsCollectAsset(AssetRuleData data)
            {
                return data.Extension == "exr";
            }

            #endregion
        }

        #endregion

        #region Nested type: CollectTextureHDR

        public class CollectTextureHDR : IFilterRule
        {
            #region IFilterRule Members

            public string DisplayFilterName => "Texture/*.hdr";

            public bool IsCollectAsset(AssetRuleData data)
            {
                return data.Extension == "hdr";
            }

            #endregion
        }

        #endregion

        #region Nested type: CollectTextureJPG

        public class CollectTextureJPG : IFilterRule
        {
            #region IFilterRule Members

            public string DisplayFilterName => "Texture/*.jpg";

            public bool IsCollectAsset(AssetRuleData data)
            {
                return data.Extension == "jpg";
            }

            #endregion
        }

        #endregion

        #region Nested type: CollectTexturePNG

        public class CollectTexturePNG : IFilterRule
        {
            #region IFilterRule Members

            public string DisplayFilterName => "Texture/*.png";

            public bool IsCollectAsset(AssetRuleData data)
            {
                return data.Extension == "png";
            }

            #endregion
        }

        #endregion

        #region Nested type: CollectTexturePSD

        public class CollectTexturePSD : IFilterRule
        {
            #region IFilterRule Members

            public string DisplayFilterName => "Texture/*.psd";

            public bool IsCollectAsset(AssetRuleData data)
            {
                return data.Extension == "psd";
            }

            #endregion
        }

        #endregion

        #region Nested type: CollectTextureSpriteAtlas

        public class CollectTextureSpriteAtlas : IFilterRule
        {
            #region IFilterRule Members

            public string DisplayFilterName => "Texture/*.spriteAtlas";

            public bool IsCollectAsset(AssetRuleData data)
            {
                return data.Extension == "spriteatlas";
            }

            #endregion
        }

        #endregion

        #region Nested type: CollectTextureTGA

        public class CollectTextureTGA : IFilterRule
        {
            #region IFilterRule Members

            public string DisplayFilterName => "Texture/*.tga";

            public bool IsCollectAsset(AssetRuleData data)
            {
                return data.Extension == "tga";
            }

            #endregion
        }

        #endregion
    }
}