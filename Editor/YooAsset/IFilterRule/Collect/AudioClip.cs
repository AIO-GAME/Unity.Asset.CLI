/*|✩ - - - - - |||
|||✩ Author:   ||| -> XINAN
|||✩ Date:     ||| -> 2023-08-14
|||✩ Document: ||| ->
|||✩ - - - - - |*/
#if SUPPORT_YOOASSET

using System.IO;
using YooAsset.Editor;

namespace AIO.UEditor.YooAsset
{
    [DisplayName("收集 AudioClip")]
    public class CollectAudioClip : IFilterRule
    {
        public bool IsCollectAsset(FilterRuleData data)
        {
            var Extension = Path.GetExtension(data.AssetPath).ToLower();
            return
                // 有损压缩
                Extension == ".mp3" ||
                Extension == ".wma" ||
                Extension == ".aac" ||
                Extension == ".ogg" ||
                // 无损压缩
                Extension == ".flac" ||
                Extension == ".ape" ||
                Extension == ".tta" ||
                Extension == ".wv" ||
                Extension == ".tak" ||
                Extension == ".wmv" ||
                Extension == ".amr" ||
                Extension == ".midi" ||
                Extension == ".aiff" ||
                Extension == ".mpeg";
        }
    }
}
#endif