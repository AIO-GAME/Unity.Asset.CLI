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
        public class CollectAudioClipMP3 : IFilterRule
        {
            public string DisplayFilterName => "Audio/*.mp3";

            public bool IsCollectAsset(FilterRuleData data) => data.Extension == "mp3";
        }

        public class CollectAudioClipOGG : IFilterRule
        {
            public string DisplayFilterName => "Audio/*.ogg";

            public bool IsCollectAsset(FilterRuleData data) => data.Extension == "ogg";
        }

        public class CollectAudioClipFLAC : IFilterRule
        {
            public string DisplayFilterName => "Audio/*.flac";

            public bool IsCollectAsset(FilterRuleData data) => data.Extension == "flac";
        }

        public class CollectAudioClipMIXER : IFilterRule
        {
            public string DisplayFilterName => "Audio/*.mixer";

            public bool IsCollectAsset(FilterRuleData data) => data.Extension == "mixer";
        }

        public class CollectAudioClip : IFilterRule
        {
            public string DisplayFilterName => "Audio/ALL";

            public bool IsCollectAsset(FilterRuleData data) =>
                new CollectAudioClipMP3().IsCollectAsset(data) ||
                new CollectAudioClipOGG().IsCollectAsset(data) ||
                new CollectAudioClipFLAC().IsCollectAsset(data) ||
                data.Extension == "mpeg" ||
                data.Extension == "aac" ||
                data.Extension == "wmv" ||
                data.Extension == "wma" ||
                new CollectAudioClipMIXER().IsCollectAsset(data);
        }
    }
}