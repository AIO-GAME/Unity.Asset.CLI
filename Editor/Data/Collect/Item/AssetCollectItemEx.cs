using System.Collections.Generic;
using System.Linq;

namespace AIO.UEditor
{
    public static class AssetCollectItemEx
    {
        /// <summary>
        ///     获取收集器显示名称
        /// </summary>
        public static string[] GetDisPlayNames(this IEnumerable<AssetCollectItem> collectors)
        {
            return (from item in collectors
                    where item.Type == EAssetCollectItemType.MainAssetCollector
                    where !string.IsNullOrEmpty(item.CollectPath)
                    select item.CollectPath
                ).Distinct()
                 .ToArray();
        }
    }
}