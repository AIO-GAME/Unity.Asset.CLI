using System;
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

        partial void OnDrawEditorMode(Rect rect)
        {
            var temp = rect.width - ViewCollectorsList.MinWidth;
            if (ViewPackageList.IsShow && ViewPackageList.IsAllowHorizontal)
                temp -= ViewPackageList.MinWidth;
            if (ViewGroupList.IsShow && ViewGroupList.IsAllowHorizontal)
                temp -= ViewGroupList.MinWidth;

            ViewGroupList.MaxWidth   = temp;
            ViewPackageList.MaxWidth = temp;
            ViewGroupList.height     = ViewCollectorsList.height = ViewPackageList.height = rect.height;

            ViewCollectorsList.x = 5;
            if (ViewPackageList.IsShow)
            {
                ViewPackageList.x = ViewCollectorsList.x;
                ViewPackageList.Draw(ViewTreePackage.OnGUI, GEStyle.INThumbnailShadow);
                ViewCollectorsList.x = ViewPackageList.width + ViewPackageList.x;
            }

            if (ViewGroupList.IsShow)
            {
                ViewGroupList.x = ViewCollectorsList.x;
                ViewGroupList.Draw(ViewTreeGroup.OnGUI, GEStyle.INThumbnailShadow);
                ViewCollectorsList.x = ViewGroupList.width + ViewGroupList.x;
            }

            ViewCollectorsList.width = rect.width - ViewCollectorsList.x - 5;
            ViewCollectorsList.Draw(ViewTreeCollector.OnGUI);
        }

        partial void OnDrawHeaderEditorMode(Rect rect)
        {
            rect.height = 20;
            rect.x      = rect.width - 30;
            rect.width  = 30;
            if (GUI.Button(rect, GC_SAVE, GEStyle.TEtoolbarbutton))
            {
                try
                {
                    GUI.FocusControl(null);
                    Data.Save();
#if UNITY_2021_1_OR_NEWER
                    AssetDatabase.SaveAssetIfDirty(Data);
#else
                    AssetDatabase.SaveAssets();
#endif
                    if (EditorUtility.DisplayDialog("保存", "保存成功", "确定")) AssetDatabase.Refresh();
                }
                catch (Exception)
                {
                    // ignored
                }
            }

            rect.width =  30;
            rect.x     -= rect.width;
            if (GUI.Button(rect, GC_SORT, GEStyle.TEtoolbarbutton))
            {
                Data.Sort();
            }

            rect.width =  30;
            rect.x     -= rect.width;
            if (GUI.Button(rect, GC_REFRESH, GEStyle.TEtoolbarbutton))
            {
                Data.Refresh();
            }

            rect.width =  30;
            rect.x     -= rect.width;
            if (GUI.Button(rect, GC_Select_ASConfig, GEStyle.TEtoolbarbutton))
            {
                GUI.FocusControl(null);
                Selection.activeObject = AssetCollectRoot.GetOrCreate();
            }

            rect.width =  75;
            rect.x     -= rect.width;
            if (GUI.Button(rect, GC_ToConvert, GEStyle.TEtoolbarbutton))
            {
                try
                {
                    GUI.FocusControl(null);
                    AssetProxyEditor.ConvertConfig(Data, false);
                    EditorUtility.DisplayDialog("转换", "转换 YooAsset 成功", "确定");
                }
                catch (Exception)
                {
                    // ignored
                }
            }

            rect.width =  75;
            rect.x     -= rect.width;
            if (GUI.Button(rect, GC_MERGE, GEStyle.TEtoolbarbutton))
            {
                if (EditorUtility.DisplayDialog("合并", "确定合并当前资源包的所有组和收集器?", "确定", "取消"))
                    Data.MergeCollector(Data.CurrentPackage.Name);
            }

            rect.x     = 0;
            rect.width = ViewPackageList.width + ViewPackageList.DragStretchHorizontalWidth / 2;
            if (GUI.Button(rect, $"{(ViewPackageList.IsShow ? "⇘" : "⇗")} Package", GEStyle.TEtoolbarbutton))
            {
                ViewPackageList.IsShow = !ViewPackageList.IsShow;
            }

            rect.x     += rect.width;
            rect.width =  ViewGroupList.width + ViewGroupList.DragStretchHorizontalWidth / 2;
            if (GUI.Button(rect, $"{(ViewGroupList.IsShow ? "⇘" : "⇗")} Group", GEStyle.TEtoolbarbutton))
            {
                ViewGroupList.IsShow = !ViewGroupList.IsShow;
            }

            TempBuilder.Clear();
            if (Data.IsValidPackage())
            {
                TempBuilder.Append(Data.CurrentPackage.Name);
                if (!string.IsNullOrEmpty(Data.CurrentPackage.Description))
                {
                    TempBuilder.Append('(').Append(Data.CurrentPackage.Description).Append(')');
                }
            }
            else ViewGroupList.IsShow = false;

            if (Data.IsValidGroup())
            {
                TempBuilder.Append(" / ").Append(Data.CurrentGroup.Name);
                if (!string.IsNullOrEmpty(Data.CurrentGroup.Description))
                {
                    TempBuilder.Append('(').Append(Data.CurrentGroup.Description).Append(')');
                }
            }

            rect.x     = 0;
            rect.width = CurrentWidth;
            EditorGUI.DropShadowLabel(rect, TempBuilder.ToString());
        }
    }
}