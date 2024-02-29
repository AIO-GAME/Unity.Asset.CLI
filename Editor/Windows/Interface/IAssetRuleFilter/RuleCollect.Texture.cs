using IFilterRule = AIO.UEditor.IAssetRuleFilter;

namespace AIO.UEditor
{
    public partial class RuleCollect
    {
        public class CollectTextureSpriteAtlas : IFilterRule
        {
            public string DisplayFilterName => "Texture/*.spriteAtlas";

            public bool IsCollectAsset(AssetRuleData data) => data.Extension == "spriteatlas";
        }

        public class CollectTexturePNG : IFilterRule
        {
            public string DisplayFilterName => "Texture/*.png";

            public bool IsCollectAsset(AssetRuleData data) => data.Extension == "png";
        }

        public class CollectTextureJPG : IFilterRule
        {
            public string DisplayFilterName => "Texture/*.jpg";

            public bool IsCollectAsset(AssetRuleData data) => data.Extension == "jpg";
        }

        public class CollectTextureTGA : IFilterRule
        {
            public string DisplayFilterName => "Texture/*.tga";

            public bool IsCollectAsset(AssetRuleData data) => data.Extension == "tga";
        }

        public class CollectTexturePSD : IFilterRule
        {
            public string DisplayFilterName => "Texture/*.psd";

            public bool IsCollectAsset(AssetRuleData data) => data.Extension == "psd";
        }

        public class CollectTextureBMP : IFilterRule
        {
            public string DisplayFilterName => "Texture/*.bmp";

            public bool IsCollectAsset(AssetRuleData data) => data.Extension == "bmp";
        }

        public class CollectTextureEXR : IFilterRule
        {
            public string DisplayFilterName => "Texture/*.exr";

            public bool IsCollectAsset(AssetRuleData data) => data.Extension == "exr";
        }

        public class CollectTextureHDR : IFilterRule
        {
            public string DisplayFilterName => "Texture/*.hdr";

            public bool IsCollectAsset(AssetRuleData data) => data.Extension == "hdr";
        }

        public class CollectTexture : IFilterRule
        {
            public string DisplayFilterName => "Texture/ALL";

            public bool IsCollectAsset(AssetRuleData data) =>
                new CollectTextureSpriteAtlas().IsCollectAsset(data) ||
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
    }
}