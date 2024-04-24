using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace AIO.UEditor
{
    partial class AssetCollectWindow
    {
        /// <summary>
        ///     绘制 标签模式 导航栏
        /// </summary>
        partial void OnDrawHeaderTagsMode(Rect rect)
        {
            if (!Data.IsValidCollect())
            {
                return;
            }

            var width = rect.width;
            rect.x     = 0;
            rect.width = 0;
            if (TagsModeDisplayCollectors?.Length > 0)
            {
                rect.x     += rect.width;
                rect.width =  100;
                switch (TagsModeDisplayCollectors.Length)
                {
                    case 1:
                        LookModeDisplayCollectorsIndex = 0;
                        EditorGUI.Popup(rect, 0, TagsModeDisplayCollectors, GEStyle.PreDropDown);
                        break;
                    default:
                    {
                        LookModeDisplayCollectorsIndex = TagsModeDisplayCollectors.Length >= 31
                            ? EditorGUI.Popup(rect, LookModeDisplayCollectorsIndex, TagsModeDisplayCollectors, GEStyle.PreDropDown)
                            : EditorGUI.MaskField(rect, LookModeDisplayCollectorsIndex, TagsModeDisplayCollectors, GEStyle.PreDropDown);

                        break;
                    }
                }
            }

            if (TagsModeDisplayTypes?.Length > 0)
            {
                rect.x                   += rect.width;
                rect.width               =  100;
                LookModeDisplayTypeIndex =  EditorGUI.MaskField(rect, LookModeDisplayTypeIndex, TagsModeDisplayTypes, GEStyle.PreDropDown);
            }

            if (TagsModeDisplayTags?.Length > 0)
            {
                rect.x                   += rect.width;
                rect.width               =  100;
                LookModeDisplayTagsIndex =  EditorGUI.MaskField(rect, LookModeDisplayTagsIndex, TagsModeDisplayTags, GEStyle.PreDropDown);
            }

            if (GUI.changed)
                if (TempTable.GetOrDefault<string>(nameof(ViewTreeQueryAsset.searchString)) != ViewTreeQueryAsset.searchString
                 || TempTable.GetOrDefault<int>(nameof(LookModeDisplayCollectorsIndex)) != LookModeDisplayCollectorsIndex
                 || TempTable.GetOrDefault<int>(nameof(LookModeDisplayTypeIndex)) != LookModeDisplayTypeIndex
                 || TempTable.GetOrDefault<int>(nameof(LookModeDisplayTagsIndex)) != LookModeDisplayTagsIndex)
                {
                    CurrentPageValues.Clear();
                    CurrentPageValues.Add(CurrentTagValues.Where(data => !TagsModeDataFilter(data)));
                    CurrentPageValues.PageIndex                        = 0;
                    LookModeCollectorsALLSize                          = CurrentPageValues.Sum(data => data.Size);
                    TempTable[nameof(ViewTreeQueryAsset.searchString)] = ViewTreeQueryAsset.searchString = ViewTreeQueryAsset.searchString;
                    TempTable[nameof(LookModeDisplayCollectorsIndex)]  = LookModeDisplayCollectorsIndex;
                    TempTable[nameof(LookModeDisplayTypeIndex)]        = LookModeDisplayTypeIndex;
                    TempTable[nameof(LookModeDisplayTagsIndex)]        = LookModeDisplayTagsIndex;
                    ViewTreeQueryAsset.ReloadAndSelect(0);
                }

            rect.x     += rect.width + 3;
            rect.width =  width - 190 - 30 - 30 - rect.x;
            ViewTreeQueryAsset.searchString = CurrentTagValues.Count > 300
                ? EditorGUI.DelayedTextField(rect, ViewTreeQueryAsset.searchString, GEStyle.SearchTextField)
                : EditorGUI.TextField(rect, ViewTreeQueryAsset.searchString, GEStyle.SearchTextField);

            rect.x     += rect.width;
            rect.width =  30;
            if (GUI.Button(rect, GC_CLEAR, GEStyle.TEtoolbarbutton))
            {
                GUI.FocusControl(null);
                ViewTreeQueryAsset.searchString = string.Empty;
            }

            rect.x     += rect.width;
            rect.width =  190;
            OnDrawHeaderLookPageSetting(rect);

            rect.x     = width - 30;
            rect.width = 30;
            if (GUI.Button(rect, GC_SAVE, GEStyle.TEtoolbarbutton))
            {
                GUI.FocusControl(null);
                Config.SequenceRecord.Save();
                Config.Save();
#if UNITY_2021_1_OR_NEWER
                AssetDatabase.SaveAssetIfDirty(Config);
#else
                AssetDatabase.SaveAssets();
#endif
                if (EditorUtility.DisplayDialog("保存", "保存成功", "确定")) AssetDatabase.Refresh();
            }

            rect.x     -= rect.width;
            rect.width =  30;
            if (GUI.Button(rect, GC_REFRESH, GEStyle.TEtoolbarbutton))
            {
                LookCurrentSelectAsset          = null;
                ViewTreeQueryAsset.searchString = string.Empty;
                LookModeDisplayTagsIndex        = LookModeDisplayTypeIndex = LookModeDisplayCollectorsIndex = 0;
                UpdateDataTagsMode();
            }
        }

        /// <summary>
        ///     绘制 标签模式 资源列表
        /// </summary>
        partial void OnDrawTagsMode(Rect rect)
        {
            if (Data.Packages.Length == 0)
            {
                GELayout.HelpBox("当前无包资源数据");
                return;
            }

            ViewDetailList.height = CurrentHeight - DrawHeaderHeight;
            if (LookModeShowAssetDetail)
            {
                ViewDetailList.width = CurrentWidth - 410;
                ViewDetails.IsShow   = true;
                ViewDetails.y        = 0;
                ViewDetails.x        = ViewDetailList.width + ViewDetailList.x + 5;
                ViewDetails.width    = CurrentWidth - ViewDetails.x - 5;
                ViewDetails.height   = ViewDetailList.height;
            }
            else
            {
                ViewDetails.IsShow   = false;
                ViewDetailList.width = CurrentWidth - 10;
            }

            ViewDetailList.Draw(ViewTreeQueryAsset.OnGUI, GEStyle.INThumbnailShadow);
            ViewDetails.Draw(OnDrawLookModeAssetDetail, GEStyle.INThumbnailShadow);
        }

        /// <summary>
        ///     更新数据 标签模式
        /// </summary>
        private void UpdateDataTagsMode()
        {
            GUI.FocusControl(null);
            LookCurrentSelectAsset = null;

            CurrentPageValues.Clear();
            CurrentPageValues.PageIndex = 0;
            CurrentTagValues.Clear();
            LookModeCollectorsALLSize  = 0;
            LookModeCollectorsPageSize = 0;

            if (Data.Packages.Length == 0) return;
            TagsModeDisplayCollectors = new[]
            {
                "ALL"
            };
            TagsModeDisplayTypes = Array.Empty<string>();
            TagsModeDisplayTags  = Data.GetTags();

            var listTypes = new List<string>();
            var listItems = new List<AssetCollectItem>();
            foreach (var package in Data.Packages)
            {
                if (package.Groups is null) continue;
                foreach (var group in package.Groups)
                {
                    if (group.Collectors is null) continue;
                    var flag = !string.IsNullOrEmpty(group.Tag);
                    foreach (var collector in group.Collectors)
                    {
                        if (!flag && string.IsNullOrEmpty(collector.Tags)) continue;
                        listItems.Add(collector);
                        collector.CollectAssetAsync(package.Name, group.Name, dic =>
                        {
                            Runner.StartCoroutine(() =>
                            {
                                listTypes.AddRange(dic.Select(pair => pair.Value.Type));
                                TagsModeDisplayTypes = listTypes.Distinct().ToArray();
                                Repaint();
                            });
                            foreach (var pair in dic)
                            {
                                CurrentTagValues.Add(pair.Value);
                                if (LookModeDataFilter(pair.Value)) continue;
                                CurrentPageValues.Add(pair.Value);
                                LookModeCollectorsALLSize += pair.Value.Size;
                            }

                            CurrentPageValues.PageIndex = CurrentPageValues.PageIndex;
                            ViewTreeQueryAsset.ReloadAndSelect(0);
                        });
                    }
                }
            }

            TagsModeDisplayCollectors = GetCollectorDisPlayNames(listItems.GetDisPlayNames());
            if (LookModeDisplayCollectorsIndex < 0) LookModeDisplayCollectorsIndex = 0;
            ViewTreeQueryAsset.Reload();
        }

        /// <summary>
        ///     标签模式 资源过滤器
        /// </summary>
        private bool TagsModeDataFilter(AssetDataInfo data)
        {
            var filter = 0;
            if (IsFilterCollectors(LookModeDisplayCollectorsIndex, data.CollectPath, TagsModeDisplayCollectors))
                filter++;

            if (IsFilterTypes(LookModeDisplayTypeIndex, data.AssetPath, TagsModeDisplayTypes))
                filter++;

            if (IsFilterTags(LookModeDisplayTagsIndex, data.Tags.Split(';', ',', ' '), TagsModeDisplayTags))
                filter++;

            if (IsFilterSearch(ViewTreeQueryAsset.searchString, data))
                filter++;

            return filter != 4;
        }
    }
}