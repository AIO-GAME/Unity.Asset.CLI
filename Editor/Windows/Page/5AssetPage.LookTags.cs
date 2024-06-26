﻿using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace AIO.UEditor
{
    partial class AssetPageLook
    {
        internal class Tags : IAssetPage
        {
            #region IAssetPage

            int IAssetPage.   Order => 5;
            string IAssetPage.Title => "查询标签      [Ctrl + Number5]";

            void IAssetPage.EventMouseDown(in Event evt) { Instance.ViewDetailList.ContainsDragStretch(evt, ViewRect.DragStretchType.Horizontal); }

            void IAssetPage.EventMouseUp(in Event evt) { Instance.ViewDetailList.CancelDragStretch(); }

            void IAssetPage.EventKeyDown(in Event evt, in KeyCode keyCode) { }
            void IAssetPage.EventKeyUp(in   Event evt, in KeyCode keyCode) { }

            void IAssetPage.EventMouseDrag(in Event evt)
            {
                if (Instance.ShowAssetDetail) Instance.ViewDetailList.DraggingStretch(evt, ViewRect.DragStretchType.Horizontal);
            }

            bool IAssetPage.Shortcut(Event evt) =>
                evt.control && evt.type == EventType.KeyDown && (evt.keyCode == KeyCode.Keypad5 || evt.keyCode == KeyCode.Alpha5);

            public void Dispose() { DisplayPackages = null; }

            #endregion

            public void OnDrawHeader(Rect rect)
            {
                if (!Data.IsValidCollect()) return;

                var width = rect.width;
                rect.x     = 0;
                rect.width = 0;
                EditorGUI.BeginChangeCheck();

                if (DisplayCollectors?.Length > 0)
                {
                    rect.x     += rect.width;
                    rect.width =  100;
                    switch (DisplayCollectors.Length)
                    {
                        case 1:
                            DisplayCollectorsIndex = 0;
                            EditorGUI.Popup(rect, 0, DisplayCollectors, GEStyle.PreDropDown);
                            break;
                        default:
                        {
                            DisplayCollectorsIndex = DisplayCollectors.Length >= 31
                                ? EditorGUI.Popup(rect, DisplayCollectorsIndex, DisplayCollectors, GEStyle.PreDropDown)
                                : EditorGUI.MaskField(rect, DisplayCollectorsIndex, DisplayCollectors, GEStyle.PreDropDown);

                            break;
                        }
                    }
                }

                if (DisplayTypes?.Length > 0)
                {
                    rect.x           += rect.width;
                    rect.width       =  100;
                    DisplayTypeIndex =  EditorGUI.MaskField(rect, DisplayTypeIndex, DisplayTypes, GEStyle.PreDropDown);
                }

                if (DisplayTags?.Length > 0)
                {
                    rect.x           += rect.width;
                    rect.width       =  100;
                    DisplayTagsIndex =  EditorGUI.MaskField(rect, DisplayTagsIndex, DisplayTags, GEStyle.PreDropDown);
                }

                if (EditorGUI.EndChangeCheck())
                {
                    lock (PageValues)
                    {
                        PageValues.Clear();
                        PageValues.Add(Values.Where(data => !FilterData(data)));
                        PageValues.PageIndex = 0;
                    }

                    TreeViewQueryAsset.ReloadAndSelect(0);
                }

                rect.x += rect.width + 3;

                rect.width = width - 30 - 30 - rect.x - (PageValues.Count <= 0 ? 0 : 190) - 3;
                SearchAssetText(rect);

                rect.x     += rect.width + 3;
                rect.width =  190;
                Instance.OnDrawPageSetting(rect);

                rect.x     = width - 30;
                rect.width = 30;
                if (GUI.Button(rect, Instance.GC_SAVE, GEStyle.TEtoolbarbutton))
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
                if (GUI.Button(rect, Instance.GC_REFRESH, GEStyle.TEtoolbarbutton))
                {
                    Instance.SelectAsset            = null;
                    TreeViewQueryAsset.searchString = string.Empty;
                    DisplayTagsIndex                = DisplayTypeIndex = DisplayCollectorsIndex = 0;
                    UpdateData();
                }
            }

            /// <summary>
            ///     绘制 标签模式 资源列表
            /// </summary>
            public void OnDrawContent(Rect rect)
            {
                if (Data.Packages.Length == 0)
                {
                    GELayout.HelpBox("当前无包资源数据");
                    return;
                }

                Instance.ViewDetailList.x      = rect.x + 5;
                Instance.ViewDetailList.height = rect.height + 5;
                if (Instance.ShowAssetDetail)
                {
                    Instance.ViewDetailList.MaxWidth = rect.width - Instance.ViewDetails.MinWidth - 10;

                    Instance.ViewDetails.IsShow = true;
                    Instance.ViewDetails.y      = 0;
                    Instance.ViewDetails.x      = Instance.ViewDetailList.x + Instance.ViewDetailList.width;
                    Instance.ViewDetails.width  = rect.width - Instance.ViewDetails.x - 5;
                    if (Instance.ViewDetails.width < Instance.ViewDetails.MinWidth)
                    {
                        Instance.ViewDetails.width    = Instance.ViewDetails.MinWidth - 5;
                        Instance.ViewDetailList.width = rect.width - Instance.ViewDetails.width - 10;
                    }

                    Instance.ViewDetails.height = Instance.ViewDetailList.height;
                }
                else
                {
                    Instance.ViewDetails.IsShow   = false;
                    Instance.ViewDetailList.width = rect.width - 5;
                }

                Instance.ViewDetailList.Draw(TreeViewQueryAsset.OnGUI, GEStyle.INThumbnailShadow);
                Instance.ViewDetails.Draw(Instance.OnDrawAssetDetail, GEStyle.INThumbnailShadow);
            }

            /// <summary>
            ///     更新数据 标签模式
            /// </summary>
            public void UpdateData()
            {
                GUI.FocusControl(null);
                Instance.SelectAsset            = null;
                TreeViewQueryAsset.searchString = string.Empty;
                lock (PageValues)
                {
                    PageValues.Clear();
                    PageValues.PageIndex = 0;
                    TreeViewQueryAsset.Reload(PageValues);
                    TreeViewQueryAsset.Select(0);
                }

                lock (Values) Values.Clear();

                if (Data.Packages.Length == 0) return;

                DisplayCollectors = new[] { "ALL" };
                DisplayTypes      = Array.Empty<string>();
                DisplayTags       = Data.GetTags();

                var toLower      = Config.LoadPathToLower;
                var hasExtension = Config.HasExtension;
                var listTypes    = new List<string>();
                var listItems    = new List<AssetCollectItem>();
                var index        = 0;
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
                            if (collector.AllowThread)
                                Runner.StartTask(() => Collect(package, group, collector));
                            else
                                Runner.StartCoroutine(() => Collect(package, group, collector));
                        }
                    }
                }

                DisplayCollectors = GetCollectorDisPlayNames(listItems.GetDisPlayNames());
                if (DisplayCollectorsIndex < 0) DisplayCollectorsIndex = 0;
                return;

                void Collect(AssetCollectPackage package, AssetCollectGroup group, AssetCollectItem item)
                {
                    item.CollectAssetAsync(package, group, toLower, hasExtension);
                    lock (Values) Values.AddRange(item.DataInfos.Values);
                    if (listItems.Count != ++index) return;
                    Runner.StartCoroutine(() =>
                    {
                        lock (Values) listTypes.AddRange(Values.Where(dataInfo => !listTypes.Contains(dataInfo.Type)).Select(dataInfo => dataInfo.Type));
                        Runner.StartTask(End);
                    });
                }

                void End()
                {
                    DisplayTypes = listTypes.ToArray();
                    lock (PageValues)
                    {
                        PageValues.Add(Values.Where(data => !FilterData(data)));
                        Runner.StartCoroutine(() =>
                        {
                            lock (PageValues)
                            {
                                PageValues.PageIndex = PageValues.PageIndex;
                                TreeViewQueryAsset.Reload(PageValues);
                            }
                        });
                    }
                }
            }

            /// <summary>
            ///     标签模式 资源过滤器
            /// </summary>
            private static bool FilterData(AssetDataInfo data)
            {
                var filter = 0;
                if (IsFilterCollectors(DisplayCollectorsIndex, data.CollectPath, DisplayCollectors)) filter++;
                if (IsFilterTypes(DisplayTypeIndex, data, DisplayTypes)) filter++;
                if (IsFilterTags(DisplayTagsIndex, data.Tags, DisplayTags)) filter++;
                return filter != 3;
            }
        }
    }
}