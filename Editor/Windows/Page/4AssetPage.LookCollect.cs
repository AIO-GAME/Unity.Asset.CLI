using System;
using System.Collections.Concurrent;
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

            void IAssetPage.EventKeyDown(in Event evt, in KeyCode keyCode) { }

            void IAssetPage.EventKeyUp(in Event evt, in KeyCode keyCode) { }

            void IAssetPage.EventMouseDown(in Event evt) { Instance.ViewDetailList.ContainsDragStretch(evt, ViewRect.DragStretchType.Horizontal); }

            void IAssetPage.EventMouseUp(in Event evt) { Instance.ViewDetailList.CancelDragStretch(); }

            void IAssetPage.EventMouseDrag(in Event evt)
            {
                if (Instance.ShowAssetDetail) Instance.ViewDetailList.DraggingStretch(evt, ViewRect.DragStretchType.Horizontal);
            }

            bool IAssetPage.Shortcut(Event evt) => evt.control && evt.type == EventType.KeyDown && (evt.keyCode == KeyCode.Keypad4 || evt.keyCode == KeyCode.Alpha4);

            public void Dispose()
            {
                DisplayPackages   = null;
                DisplayGroupNames = null;
                CollectorDisplays = null;
                TypeDisplays      = null;
                TagDisplays       = null;
                DataDic           = null;
            }

            #endregion

            private Dictionary<string, string[]>                         DisplayGroupNames;
            private Dictionary<(int, int), string[]>                     CollectorDisplays;
            private Dictionary<(int, int), string[]>                     TypeDisplays;
            private Dictionary<(int, int), string[]>                     TagDisplays;
            private Dictionary<(int, int), ConcurrentBag<AssetDataInfo>> DataDic;

            private Rect OnDrawGroup(Rect rect)
            {
                using var scope = new EditorGUI.ChangeCheckScope();
                Data.CurrentPackageIndex = EditorGUI.Popup(rect, Data.CurrentPackageIndex, DisplayPackages, GEStyle.PreDropDown);

                if (!Data.IsValidGroup()) return rect;

                var package = DisplayPackages[Data.CurrentPackageIndex];
                if (!DisplayGroupNames.ContainsKey(package))
                    DisplayGroupNames[package] = GetGroupDisPlayNames(Data.CurrentPackage.Groups);

                if (Data.CurrentGroupIndex >= DisplayGroupNames[package].Length)
                    Data.CurrentGroupIndex = DisplayGroupNames[package].Length - 1;

                rect.x                 += rect.width;
                Data.CurrentGroupIndex =  EditorGUI.Popup(rect, Data.CurrentGroupIndex, DisplayGroupNames[package], GEStyle.PreDropDown);

                if (!Data.IsValidCollect() || !scope.changed) return rect;
                if (!DataDic.ContainsKey((Data.CurrentPackageIndex, Data.CurrentGroupIndex)))
                    UpdateDataCollector(Data.CurrentPackageIndex, Data.CurrentGroupIndex);
                else
                {
                    PageValues.Clear();
                    PageValues.Add(DataDic[(Data.CurrentPackageIndex, Data.CurrentGroupIndex)]);
                    PageValues.PageIndex = 0;
                    TreeViewQueryAsset.Reload(PageValues);
                }

                return rect;
            }

            private Rect OnDrawCollector(Rect rect)
            {
                using var scope      = new EditorGUI.ChangeCheckScope();
                var       collectors = CollectorDisplays[(Data.CurrentPackageIndex, Data.CurrentGroupIndex)];
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

                var types = TypeDisplays[(Data.CurrentPackageIndex, Data.CurrentGroupIndex)];
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

                var tags = TagDisplays[(Data.CurrentPackageIndex, Data.CurrentGroupIndex)];
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

                if (!scope.changed) return rect;
                PageValues.Clear();
                lock (DataDic)
                {
                    PageValues.Add(DataDic[(Data.CurrentPackageIndex, Data.CurrentGroupIndex)].Where(data => !FilterData(data)));
                }

                PageValues.PageIndex = 0;
                TreeViewQueryAsset.Reload();

                return rect;
            }

            public void OnDrawHeader(Rect rect)
            {
                if (Data.Packages.Length == 0 ||
                    DisplayPackages is null ||
                    DisplayPackages.Length == 0) return;

                var width = rect.width;
                rect.x     = 0;
                rect.width = 100;
                rect       = OnDrawGroup(rect);
                rect       = OnDrawCollector(rect);

                rect.x     += rect.width + 3;
                rect.width =  width - 30 - rect.x - (PageValues.Count <= 0 ? 0 : 190);
                SearchAssetText(rect);

                rect.x     += rect.width;
                rect.width =  190;
                Instance.OnDrawPageSetting(rect);

                rect.x     = width - 30;
                rect.width = 30;

                if (GUI.Button(rect, Instance.GC_REFRESH, GEStyle.TEtoolbarbutton))
                {
                    Instance.SelectAsset            = null;
                    TreeViewQueryAsset.searchString = string.Empty;
                    DisplayTagsIndex                = DisplayTypeIndex = DisplayCollectorsIndex = 0;
                    UpdateData();
                }
            }

            private void UpdateDataCollector(int packageIndex, int groupIndex)
            {
                PageValues.Clear();
                var i = packageIndex;
                var j = groupIndex;
                if (Data.Count <= i
                 || Data.Packages[i] is null
                 || Data.Packages[i].Groups is null
                 || j < 0
                 || Data.Packages[i].Count <= j
                   ) return;

                var key     = (i, j);
                var package = Data.Packages[i];
                var group   = package.Groups[j];

                DisplayGroupNames[DisplayPackages[i]] = GetGroupDisPlayNames(package.Groups);
                TagDisplays[key]                      = group.Tags;
                CollectorDisplays[key]                = GetCollectorDisPlayNames(group.Collectors.GetDisPlayNames());
                DisplayCollectorsIndex                = 0;
                DataDic[key]                          = new ConcurrentBag<AssetDataInfo>();
                TypeDisplays[(i, j)]                  = Array.Empty<string>();

                var toLower      = Config.LoadPathToLower;
                var hasExtension = Config.HasExtension;
                var listTypes    = new ConcurrentBag<string>();

                var count = group.Collectors.Length;
                var index = 0;

                foreach (var item in group.Collectors)
                {
                    if (item.AllowThread)
                        Runner.StartTask(Collect, item);
                    else
                        Runner.StartCoroutine(Collect, item);
                }

                TreeViewQueryAsset.Reload(PageValues);
                return;

                void Collect(AssetCollectItem item)
                {
                    item.CollectAssetAsync(package, group, toLower, hasExtension);
                    foreach (var variable in item.DataInfos.Values)
                    {
                        DataDic[(i, j)].Add(variable);
                    }

                    if (count != ++index) return;

                    Runner.StartCoroutine(UpdateType, item);
                    Runner.StartTask(End);
                }

                void UpdateType(AssetCollectItem item)
                {
                    foreach (var type in item.DataInfos.Values.Select(dataInfo => dataInfo.Type))
                    {
                        listTypes.Add(type);
                    }

                    TypeDisplays[(i, j)] = listTypes.Distinct().ToArray();
                }

                void End()
                {
                    lock (PageValues)
                    {
                        PageValues.Add(DataDic[(i, j)].Where(data => !FilterData(data)));
                        PageValues.PageIndex = PageValues.PageIndex;
                        Runner.StartCoroutine(() => { TreeViewQueryAsset.Reload(PageValues); });
                    }
                }
            }

            private bool FilterData(AssetDataInfo data)
            {
                var filter   = 0;
                var position = (Data.CurrentPackageIndex, Data.CurrentGroupIndex);
                var def      = Array.Empty<string>();
                if (IsFilterCollectors(DisplayCollectorsIndex, data.CollectPath, CollectorDisplays?.GetOrDefault(position, def))
                   ) filter++;

                if (IsFilterTypes(DisplayTypeIndex, data, TypeDisplays?.GetOrDefault(position, def))
                   ) filter++;

                if (IsFilterTags(DisplayTagsIndex, data.Tags, TagDisplays?.GetOrDefault(position, def))
                   ) filter++;

                return filter != 3;
            }

            public void UpdateData()
            {
                TreeViewQueryAsset.searchString = string.Empty;
                GUI.FocusControl(null);
                if (!Data.IsValidCollect()) return;

                DisplayPackages = new string[Data.Packages.Length];
                for (var i = 0; i < Data.Packages.Length; i++) DisplayPackages[i] = Data.Packages[i].Name;

                if (CollectorDisplays is null) CollectorDisplays = new Dictionary<(int, int), string[]>();
                if (TagDisplays is null) TagDisplays             = new Dictionary<(int, int), string[]>();
                if (TypeDisplays is null) TypeDisplays           = new Dictionary<(int, int), string[]>();
                if (DisplayGroupNames is null) DisplayGroupNames = new Dictionary<string, string[]>();
                if (DataDic is null) DataDic                     = new Dictionary<(int, int), ConcurrentBag<AssetDataInfo>>();

                PageValues.Clear();
                PageValues.PageIndex = 0;

                UpdateDataCollector(Data.CurrentPackageIndex, Data.CurrentGroupIndex);
                TreeViewQueryAsset.Reload(PageValues);
            }

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
                    Instance.ViewDetailList.width = rect.width - 5;
                }

                Instance.ViewDetailList.Draw(TreeViewQueryAsset.OnGUI, GEStyle.INThumbnailShadow);
                Instance.ViewDetails.Draw(Instance.OnDrawAssetDetail, GEStyle.INThumbnailShadow);
            }
        }
    }
}
