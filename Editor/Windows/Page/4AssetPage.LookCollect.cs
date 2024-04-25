using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace AIO.UEditor
{
    partial class AssetPageLook
    {
        internal class Collect : IAssetPage
        {
            #region IAssetPage

            int IAssetPage.   Order => 4;
            string IAssetPage.Title => "查询模式      [Ctrl + Number4]";

            void IAssetPage.EventMouseDown(in Event evt) { Instance.ViewDetailList.ContainsDragStretch(evt, ViewRect.DragStretchType.Horizontal); }

            void IAssetPage.EventMouseUp(in Event evt) { Instance.ViewDetailList.CancelDragStretch(); }

            void IAssetPage.EventKeyDown(in Event evt, in KeyCode keyCode) { }
            void IAssetPage.EventKeyUp(in   Event evt, in KeyCode keyCode) { }

            void IAssetPage.EventMouseDrag(in Event evt)
            {
                if (Instance.ShowAssetDetail) Instance.ViewDetailList.DraggingStretch(evt, ViewRect.DragStretchType.Horizontal);
            }

            bool IAssetPage.Shortcut(Event evt) =>
                evt.control && evt.type == EventType.KeyDown && (evt.keyCode == KeyCode.Keypad4 || evt.keyCode == KeyCode.Alpha4);

            public void Dispose()
            {
                DisplayPackages           = null;
                LookModeDisplayGroups     = null;
                LookModeDisplayCollectors = null;
                LookModeDisplayTypes      = null;
                LookModeDisplayTags       = null;
                LookModeData              = null;
            }

            #endregion

            private Dictionary<string, string[]>                LookModeDisplayGroups;
            private Dictionary<(int, int), string[]>            LookModeDisplayCollectors;
            private Dictionary<(int, int), string[]>            LookModeDisplayTypes;
            private Dictionary<(int, int), string[]>            LookModeDisplayTags;
            private Dictionary<(int, int), List<AssetDataInfo>> LookModeData;

            /// <summary>
            ///     更新资源查询模式收集器
            /// </summary>
            private void UpdateDataCollector(int packageIndex, int groupIndex)
            {
                var i   = packageIndex;
                var j   = groupIndex;
                var key = (i, j);
                if (Data.Count <= i) return;
                if (Data.Packages[i] is null || Data.Packages[i].Groups is null || j < 0) return;
                if (Data.Packages[i].Count <= j) return;
                LookModeDisplayTags[key]                  = Data.Packages[i].Groups[j].Tags;
                LookModeDisplayGroups[DisplayPackages[i]] = GetGroupDisPlayNames(Data.Packages[i].Groups);
                LookModeDisplayCollectors[key]            = Data.Packages[i].Groups[j].Collectors.GetDisPlayNames();
                LookModeDisplayCollectors[key]            = GetCollectorDisPlayNames(LookModeDisplayCollectors[key]);
                LookModeData[key]                         = new List<AssetDataInfo>();
                LookModeDisplayTypes[(i, j)]              = Array.Empty<string>();
                var listTypes = new List<string>();
                foreach (var item in Data.Packages[i].Groups[j].Collectors)
                    item.CollectAssetAsync(Data.Packages[i].Name, Data.Packages[i].Groups[j].Name, dictionary =>
                    {
                        Runner.StartCoroutine(() =>
                        {
                            listTypes.AddRange(dictionary.Select(pair => pair.Value.Type));
                            LookModeDisplayTypes[(i, j)] = listTypes.Distinct().ToArray();
                        });
                        foreach (var pair in dictionary)
                        {
                            LookModeData[(i, j)].Add(pair.Value);
                            if (DataFilter(pair.Value)) continue;
                            PageValues.Add(pair.Value);
                        }

                        PageValues.PageIndex = PageValues.PageIndex;
                        TreeViewQueryAsset.Reload();
                    });
            }

            /// <summary>
            ///     资源查询模式 资源过滤器
            /// </summary>
            private bool DataFilter(AssetDataInfo data)
            {
                var filter = 0;
                if (IsFilterCollectors(DisplayCollectorsIndex,
                                       data.CollectPath,
                                       LookModeDisplayCollectors?.GetOrDefault((Data.CurrentPackageIndex, Data.CurrentGroupIndex), Array.Empty<string>()))
                   ) filter++;

                if (IsFilterTypes(DisplayTypeIndex,
                                  data.AssetPath,
                                  LookModeDisplayTypes?.GetOrDefault((Data.CurrentPackageIndex, Data.CurrentGroupIndex), Array.Empty<string>()))
                   ) filter++;

                if (IsFilterTags(DisplayTagsIndex,
                                 data.Tags.Split(';', ',', ' '),
                                 LookModeDisplayTags?.GetOrDefault((Data.CurrentPackageIndex, Data.CurrentGroupIndex), Array.Empty<string>()))
                   ) filter++;

                return filter != 3;
            }

            /// <summary>
            ///     绘制 资源查询模式 导航栏
            /// </summary>
            public void OnDrawHeader(Rect rect)
            {
                if (Data.Packages.Length == 0 ||
                    DisplayPackages is null ||
                    DisplayPackages.Length == 0) return;

                var width = rect.width;
                rect.x     = 0;
                rect.width = 100;

                EditorGUI.BeginChangeCheck();
                Data.CurrentPackageIndex = EditorGUI.Popup(rect, Data.CurrentPackageIndex, DisplayPackages, GEStyle.PreDropDown);

                if (!Data.IsValidGroup()) return;

                var PName = DisplayPackages[Data.CurrentPackageIndex];
                if (!LookModeDisplayGroups.ContainsKey(PName))
                    LookModeDisplayGroups[PName] = GetGroupDisPlayNames(Data.CurrentPackage.Groups);

                if (Data.CurrentGroupIndex >= LookModeDisplayGroups[PName].Length)
                    Data.CurrentGroupIndex = LookModeDisplayGroups[PName].Length - 1;

                rect.x                 += rect.width;
                Data.CurrentGroupIndex =  EditorGUI.Popup(rect, Data.CurrentGroupIndex, LookModeDisplayGroups[PName], GEStyle.PreDropDown);

                if (!Data.IsValidCollect())
                {
                    EditorGUI.EndChangeCheck();
                    return;
                }

                if (EditorGUI.EndChangeCheck())
                {
                    if (!LookModeData.ContainsKey((Data.CurrentPackageIndex, Data.CurrentGroupIndex)))
                        UpdateDataCollector(Data.CurrentPackageIndex, Data.CurrentGroupIndex);

                    PageValues.Clear();
                    PageValues.Add(LookModeData[(Data.CurrentPackageIndex, Data.CurrentGroupIndex)]);
                    PageValues.PageIndex = 0;
                    TreeViewQueryAsset.Reload();
                }

                EditorGUI.BeginChangeCheck();
                var collectors = LookModeDisplayCollectors[(Data.CurrentPackageIndex, Data.CurrentGroupIndex)];
                if (collectors.Length > 0)
                {
                    rect.x     += rect.width;
                    rect.width =  100;
                    switch (collectors.Length)
                    {
                        case 1:
                            DisplayCollectorsIndex = 0;
                            EditorGUI.Popup(rect, 0, collectors, GEStyle.PreDropDown);
                            break;
                        default:
                        {
                            DisplayCollectorsIndex = collectors.Length >= 31
                                ? EditorGUI.Popup(rect, DisplayCollectorsIndex, collectors, GEStyle.PreDropDown)
                                : EditorGUI.MaskField(rect, DisplayCollectorsIndex, collectors, GEStyle.PreDropDown);
                            break;
                        }
                    }
                }
                else DisplayCollectorsIndex = 0;

                var types = LookModeDisplayTypes[(Data.CurrentPackageIndex, Data.CurrentGroupIndex)];
                if (types.Length > 0)
                {
                    rect.x     += rect.width;
                    rect.width =  100;
                    if (types.Length == 1)
                    {
                        DisplayTypeIndex = 0;
                        EditorGUI.Popup(rect, 0, types, GEStyle.PreDropDown);
                    }
                    else
                    {
                        DisplayTypeIndex = EditorGUI.MaskField(rect, DisplayTypeIndex, types, GEStyle.PreDropDown);
                    }
                }
                else DisplayTypeIndex = 0;

                var tags = LookModeDisplayTags[(Data.CurrentPackageIndex, Data.CurrentGroupIndex)];
                if (tags.Length > 0)
                {
                    rect.x     += rect.width;
                    rect.width =  100;
                    if (tags.Length == 1)
                    {
                        DisplayTagsIndex = 0;
                        EditorGUI.Popup(rect, 0, tags, GEStyle.PreDropDown);
                    }
                    else
                    {
                        DisplayTagsIndex = EditorGUI.MaskField(rect, DisplayTagsIndex, tags, GEStyle.PreDropDown);
                    }
                }
                else DisplayTagsIndex = 0;

                if (EditorGUI.EndChangeCheck())
                {
                    PageValues.Clear();
                    lock (LookModeData)
                    {
                        PageValues.Add(LookModeData[(Data.CurrentPackageIndex, Data.CurrentGroupIndex)].Where(data => !DataFilter(data)));
                    }

                    PageValues.PageIndex = 0;
                    TreeViewQueryAsset.Reload();
                }

                rect.x     += rect.width + 3;
                rect.width =  width - 30 - 30 - rect.x - (PageValues.Count <= 0 ? 0 : 190);
                TreeViewQueryAsset.searchString = LookModeData[(Data.CurrentPackageIndex, Data.CurrentGroupIndex)].Count > 300
                    ? EditorGUI.DelayedTextField(rect, TreeViewQueryAsset.searchString, GEStyle.SearchTextField)
                    : EditorGUI.TextField(rect, TreeViewQueryAsset.searchString, GEStyle.SearchTextField);

                rect.x     += rect.width;
                rect.width =  30;
                if (GUI.Button(rect, GC_CLEAR, GEStyle.TEtoolbarbutton))
                {
                    GUI.FocusControl(null);
                    TreeViewQueryAsset.searchString = string.Empty;
                }

                rect.x     += rect.width;
                rect.width =  190;
                Instance.OnDrawPageSetting(rect);

                rect.x     = width - 30;
                rect.width = 30;
                if (GUI.Button(rect, GC_REFRESH, GEStyle.TEtoolbarbutton))
                {
                    Instance.SelectAsset            = null;
                    TreeViewQueryAsset.searchString = string.Empty;
                    DisplayTagsIndex                = DisplayTypeIndex = DisplayCollectorsIndex = 0;
                    UpdateData();
                }
            }

            public void UpdateData()
            {
                GUI.FocusControl(null);
                if (!Data.IsValidCollect()) return;

                DisplayPackages = new string[Data.Packages.Length];
                for (var i = 0; i < Data.Packages.Length; i++) DisplayPackages[i] = Data.Packages[i].Name;

                if (LookModeDisplayCollectors is null) LookModeDisplayCollectors = new Dictionary<(int, int), string[]>();
                if (LookModeDisplayTags is null) LookModeDisplayTags             = new Dictionary<(int, int), string[]>();
                if (LookModeDisplayTypes is null) LookModeDisplayTypes           = new Dictionary<(int, int), string[]>();
                if (LookModeDisplayGroups is null) LookModeDisplayGroups         = new Dictionary<string, string[]>();
                if (LookModeData is null) LookModeData                           = new Dictionary<(int, int), List<AssetDataInfo>>();

                PageValues.Clear();
                PageValues.PageIndex = 0;

                UpdateDataCollector(Data.CurrentPackageIndex, Data.CurrentGroupIndex);
                TreeViewQueryAsset.Reload();
            }

            /// <summary>
            ///     绘制 资源查询模式
            /// </summary>
            public void OnDrawContent(Rect rect)
            {
                if (Data.Count == 0)
                {
                    GUI.Label(rect, "当前无资源数据", GEStyle.CenteredLabel);
                    return;
                }

                if (Data.CurrentPackage.Count == 0)
                {
                    GUI.Label(rect, "当前无包资源数据", GEStyle.CenteredLabel);
                    return;
                }

                if (Data.CurrentGroup.Count == 0)
                {
                    GUI.Label(rect, "当前无组资源数据", GEStyle.CenteredLabel);
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
                    Instance.ViewDetailList.width = rect.width - 10;
                }

                Instance.ViewDetailList.Draw(TreeViewQueryAsset.OnGUI, GEStyle.INThumbnailShadow);
                Instance.ViewDetails.Draw(Instance.OnDrawAssetDetail, GEStyle.INThumbnailShadow);
            }
        }
    }
}