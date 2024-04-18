using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace AIO.UEditor
{
    public partial class AssetCollectWindow
    {
        public static void OpenCollectItem(AssetCollectItem item)
        {
            if (!Instance) return;

            if (item.Type == EAssetCollectItemType.MainAssetCollector)
            {
                var list = Data.CurrentGroup.Collectors.GetDisPlayNames();
                if (list.Length > 31)
                {
                    LookModeDisplayCollectorsIndex = 0;
                    for (var i = 0; i < list.Length; i++)
                        if (list[i] == item.CollectPath)
                        {
                            LookModeDisplayCollectorsIndex = i + 1;
                            break;
                        }
                }
                else
                {
                    var status = 1;
                    foreach (var collector in list)
                    {
                        if (collector != item.CollectPath)
                        {
                            status *= 2;
                            continue;
                        }

                        LookModeDisplayCollectorsIndex = status;
                        break;
                    }
                }

                Instance.UpdateDataLookMode();
                WindowMode                             = Mode.Look;
                Instance.TempTable[nameof(WindowMode)] = WindowMode;
                return;
            }

            EditorUtility.DisplayDialog("打开", "只有动态资源才能查询", "确定");
        }
    }
}