﻿/*|✩ - - - - - |||
|||✩ Author:   ||| -> XINAN
|||✩ Date:     ||| -> 2023-08-14
|||✩ Document: ||| ->
|||✩ - - - - - |*/
#if SUPPORT_YOOASSET
using System.IO;
using YooAsset.Editor;
using UnityEngine;

namespace AIO.UEditor.YooAsset
{
    [DisplayName("收集 SQL")]
    public class CollectSQL : IFilterRule
    {
        public bool IsCollectAsset(FilterRuleData data)
        {
            var Extension = Path.GetExtension(data.AssetPath).ToLower();
            return
                Extension == ".sql";
        }
    }
}
#endif