using IFilterRule = AIO.UEditor.IAssetRuleFilter;

namespace AIO.UEditor
{
    partial class RuleCollect
    {
        #region Nested type: CollectAudioClip

        public class CollectAudioClip : IFilterRule
        {
            #region IFilterRule Members

            public string DisplayFilterName => "Audio/ALL";

            public bool IsCollectAsset(AssetRuleData data)
            {
                return new CollectAudioClipMP3().IsCollectAsset(data) ||
                       new CollectAudioClipOGG().IsCollectAsset(data) ||
                       new CollectAudioClipFLAC().IsCollectAsset(data) ||
                       data.Extension == "mpeg" ||
                       data.Extension == "aac" ||
                       data.Extension == "wmv" ||
                       data.Extension == "wma" ||
                       new CollectAudioClipMIXER().IsCollectAsset(data);
            }

            #endregion
        }

        #endregion

        #region Nested type: CollectAudioClipFLAC

        public class CollectAudioClipFLAC : IFilterRule
        {
            #region IFilterRule Members

            public string DisplayFilterName => "Audio/*.flac";

            public bool IsCollectAsset(AssetRuleData data)
            {
                return data.Extension == "flac";
            }

            #endregion
        }

        #endregion

        #region Nested type: CollectAudioClipMIXER

        public class CollectAudioClipMIXER : IFilterRule
        {
            #region IFilterRule Members

            public string DisplayFilterName => "Audio/*.mixer";

            public bool IsCollectAsset(AssetRuleData data)
            {
                return data.Extension == "mixer";
            }

            #endregion
        }

        #endregion

        #region Nested type: CollectAudioClipMP3

        public class CollectAudioClipMP3 : IFilterRule
        {
            #region IFilterRule Members

            public string DisplayFilterName => "Audio/*.mp3";

            public bool IsCollectAsset(AssetRuleData data)
            {
                return data.Extension == "mp3";
            }

            #endregion
        }

        #endregion

        #region Nested type: CollectAudioClipOGG

        public class CollectAudioClipOGG : IFilterRule
        {
            #region IFilterRule Members

            public string DisplayFilterName => "Audio/*.ogg";

            public bool IsCollectAsset(AssetRuleData data)
            {
                return data.Extension == "ogg";
            }

            #endregion
        }

        #endregion
    }
}