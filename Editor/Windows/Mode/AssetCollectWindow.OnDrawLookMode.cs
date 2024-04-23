using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace AIO.UEditor
{
    partial class AssetCollectWindow
    {
        /// <summary>
        ///     绘制 资源查询模式 导航栏
        /// </summary>
        partial void OnDrawHeaderLookMode(Rect rect)
        {
            if (Data.Packages.Length == 0 ||
                LookModeDisplayPackages is null ||
                LookModeDisplayPackages.Length == 0) return;

            var width = rect.width;
            rect.x                   = 0;
            rect.width               = 100;
            Data.CurrentPackageIndex = EditorGUI.Popup(rect, Data.CurrentPackageIndex, LookModeDisplayPackages, GEStyle.PreDropDown);

            if (!Data.IsValidGroup()) return;

            var PName = LookModeDisplayPackages[Data.CurrentPackageIndex];

            if (!LookModeDisplayGroups.ContainsKey(PName))
                LookModeDisplayGroups[PName] = GetGroupDisPlayNames(Data.CurrentPackage.Groups);

            if (Data.CurrentGroupIndex >= LookModeDisplayGroups[PName].Length)
                Data.CurrentGroupIndex = LookModeDisplayGroups[PName].Length - 1;

            rect.x                 += rect.width;
            Data.CurrentGroupIndex =  EditorGUI.Popup(rect, Data.CurrentGroupIndex, LookModeDisplayGroups[PName], GEStyle.PreDropDown);

            if (!Data.IsValidCollect()) return;

            if (GUI.changed && !LookModeData.ContainsKey((Data.CurrentPackageIndex, Data.CurrentGroupIndex)))
                UpdateDataLookModeCollector(Data.CurrentPackageIndex, Data.CurrentGroupIndex);

            if (LookModeDisplayCollectors[(Data.CurrentPackageIndex, Data.CurrentGroupIndex)].Length > 0)
            {
                rect.x     += rect.width;
                rect.width =  100;
                if (LookModeDisplayCollectors[(Data.CurrentPackageIndex, Data.CurrentGroupIndex)].Length == 1)
                {
                    LookModeDisplayCollectorsIndex = 0;
                    EditorGUI.Popup(rect, 0, LookModeDisplayCollectors[(Data.CurrentPackageIndex, Data.CurrentGroupIndex)], GEStyle.PreDropDown);
                }
                else if (LookModeDisplayCollectors[(Data.CurrentPackageIndex, Data.CurrentGroupIndex)].Length >= 31)
                {
                    LookModeDisplayCollectorsIndex = EditorGUI.Popup(
                        rect, LookModeDisplayCollectorsIndex, LookModeDisplayCollectors[(Data.CurrentPackageIndex, Data.CurrentGroupIndex)], GEStyle.PreDropDown);
                }
                else
                {
                    LookModeDisplayCollectorsIndex = EditorGUI.MaskField(
                        rect, LookModeDisplayCollectorsIndex, LookModeDisplayCollectors[(Data.CurrentPackageIndex, Data.CurrentGroupIndex)], GEStyle.PreDropDown);
                }
            }
            else LookModeDisplayCollectorsIndex = 0;

            if (LookModeDisplayTypes[(Data.CurrentPackageIndex, Data.CurrentGroupIndex)].Length > 0)
            {
                rect.x     += rect.width;
                rect.width =  100;
                if (LookModeDisplayTypes[(Data.CurrentPackageIndex, Data.CurrentGroupIndex)].Length == 1)
                {
                    LookModeDisplayTypeIndex = 0;
                    EditorGUI.Popup(rect, 0, LookModeDisplayTypes[(Data.CurrentPackageIndex, Data.CurrentGroupIndex)], GEStyle.PreDropDown);
                }
                else
                {
                    LookModeDisplayTypeIndex = EditorGUI.MaskField(
                        rect, LookModeDisplayTypeIndex, LookModeDisplayTypes[(Data.CurrentPackageIndex, Data.CurrentGroupIndex)], GEStyle.PreDropDown);
                }
            }
            else LookModeDisplayTypeIndex = 0;

            if (LookModeDisplayTags[(Data.CurrentPackageIndex, Data.CurrentGroupIndex)].Length > 0)
            {
                rect.x     += rect.width;
                rect.width =  100;
                if (LookModeDisplayTags[(Data.CurrentPackageIndex, Data.CurrentGroupIndex)].Length == 1)
                {
                    LookModeDisplayTagsIndex = 0;
                    EditorGUI.Popup(rect, 0, LookModeDisplayTags[(Data.CurrentPackageIndex, Data.CurrentGroupIndex)], GEStyle.PreDropDown);
                }
                else
                {
                    LookModeDisplayTagsIndex = EditorGUI.MaskField(
                        rect, LookModeDisplayTagsIndex, LookModeDisplayTags[(Data.CurrentPackageIndex, Data.CurrentGroupIndex)], GEStyle.PreDropDown);
                }
            }
            else LookModeDisplayTagsIndex = 0;

            rect.x     += rect.width;
            rect.width =  width - 190 - 30 - 30 - rect.x;
            SearchText = LookModeData[(Data.CurrentPackageIndex, Data.CurrentGroupIndex)].Count > 300
                ? EditorGUI.DelayedTextField(rect, SearchText, GEStyle.SearchTextField)
                : EditorGUI.TextField(rect, SearchText, GEStyle.SearchTextField);

            rect.x     += rect.width;
            rect.width =  30;
            if (GUI.Button(rect, GC_CLEAR, GEStyle.TEtoolbarbutton))
            {
                GUI.FocusControl(null);
                SearchText = string.Empty;
            }

            if (GUI.changed)
            {
                if (TempTable.GetOrDefault<int>(nameof(Data.CurrentPackageIndex)) != Data.CurrentPackageIndex ||
                    TempTable.GetOrDefault<int>(nameof(Data.CurrentGroupIndex)) != Data.CurrentGroupIndex)
                {
                    CurrentPageValues.Clear();
                    CurrentPageValues.Add(LookModeData[(Data.CurrentPackageIndex, Data.CurrentGroupIndex)]);
                    CurrentPageValues.PageIndex                 = 0;
                    LookModeCollectorsALLSize                   = CurrentPageValues.Sum(data => data.Size);
                    TempTable[nameof(Data.CurrentPackageIndex)] = Data.CurrentPackageIndex;
                    TempTable[nameof(Data.CurrentGroupIndex)]   = Data.CurrentGroupIndex;
                    ViewTreeQueryAsset.Reload();
                }

                if (
                    TempTable.GetOrDefault<string>(nameof(SearchText)) != SearchText ||
                    TempTable.GetOrDefault<int>(nameof(LookModeDisplayCollectorsIndex)) !=
                    LookModeDisplayCollectorsIndex ||
                    TempTable.GetOrDefault<int>(nameof(LookModeDisplayTypeIndex)) !=
                    LookModeDisplayTypeIndex ||
                    TempTable.GetOrDefault<int>(nameof(LookModeDisplayTagsIndex)) !=
                    LookModeDisplayTagsIndex
                )
                {
                    CurrentPageValues.Clear();
                    lock (LookModeData)
                    {
                        CurrentPageValues.Add(LookModeData[(Data.CurrentPackageIndex, Data.CurrentGroupIndex)].
                                                  Where(data => !LookModeDataFilter(data)));
                    }

                    CurrentPageValues.PageIndex                       = 0;
                    LookModeCollectorsALLSize                         = CurrentPageValues.Sum(data => data.Size);
                    TempTable[nameof(SearchText)]                     = ViewTreeQueryAsset.searchString = SearchText;
                    TempTable[nameof(LookModeDisplayCollectorsIndex)] = LookModeDisplayCollectorsIndex;
                    TempTable[nameof(LookModeDisplayTypeIndex)]       = LookModeDisplayTypeIndex;
                    TempTable[nameof(LookModeDisplayTagsIndex)]       = LookModeDisplayTagsIndex;
                    ViewTreeQueryAsset.Reload();
                }
            }

            rect.x     += rect.width;
            rect.width =  190;
            OnDrawHeaderLookPageSetting(rect);

            rect.x     = width - 30;
            rect.width = 30;
            if (GUI.Button(rect, GC_REFRESH, GEStyle.TEtoolbarbutton))
            {
                LookCurrentSelectAsset          = null;
                ViewTreeQueryAsset.searchString = SearchText               = string.Empty;
                LookModeDisplayTagsIndex        = LookModeDisplayTypeIndex = LookModeDisplayCollectorsIndex = 0;
                UpdateDataLookMode();
            }
        }

        /// <summary>
        ///     绘制 资源查询模式 页码 宽190
        /// </summary>
        private void OnDrawHeaderLookPageSetting(Rect rect)
        {
            if (CurrentPageValues.Count <= 0) return;

            using (new EditorGUI.DisabledScope(CurrentPageValues.PageIndex <= 0))
            {
                rect.width = 30;
                if (GUI.Button(rect, GC_LookMode_Page_MaxLeft, GEStyle.TEtoolbarbutton))
                {
                    CurrentPageValues.PageIndex = 0;
                    ViewTreeQueryAsset.Reload();
                    return;
                }
            }

            using (new EditorGUI.DisabledScope(CurrentPageValues.PageIndex == 0))
            {
                rect.x     += rect.width;
                rect.width =  30;
                if (GUI.Button(rect, GC_LookMode_Page_Left, GEStyle.TEtoolbarbutton))
                {
                    CurrentPageValues.PageIndex -= 1;
                    ViewTreeQueryAsset.Reload();
                    return;
                }
            }

            rect.x     += rect.width;
            rect.width =  40;
            GUI.Label(rect, $"{CurrentPageValues.PageIndex + 1}/{CurrentPageValues.PageCount}", GEStyle.MeTimeLabel);

            using (new EditorGUI.DisabledScope(CurrentPageValues.PageIndex + 1 >= CurrentPageValues.PageCount))
            {
                rect.x     += rect.width;
                rect.width =  30;
                if (GUI.Button(rect, GC_LookMode_Page_Right, GEStyle.TEtoolbarbutton))
                {
                    CurrentPageValues.PageIndex += 1;
                    ViewTreeQueryAsset.Reload();
                    return;
                }

                rect.x     += rect.width;
                rect.width =  30;
                if (GUI.Button(rect, GC_LookMode_Page_MaxRight, GEStyle.TEtoolbarbutton))
                {
                    CurrentPageValues.PageIndex = CurrentPageValues.PageCount - 1;
                    ViewTreeQueryAsset.Reload();
                    return;
                }
            }

            rect.x     += rect.width;
            rect.width =  30;
            if (GUI.Button(rect, GC_LookMode_Page_Size, GEStyle.TEtoolbarbutton))
            {
                LookDataPageSizeMenu.ShowAsContext();
                ViewTreeQueryAsset.Reload();
            }
        }

        /// <summary>
        ///     资源查询模式 页签资源排序
        /// </summary>
        /// <param name="sort">排序模式</param>
        /// <param name="minToMax">是否为从小到大</param>
        private void LookModeDataPageValueSort(ESort sort, bool minToMax)
        {
            LookModeSort = sort;
            if ((LookModeSort == ESort.FileSize && LookModeSortEnableSize) ||
                (LookModeSort == ESort.AssetName && LookModeSortEnableAssetName) ||
                (LookModeSort == ESort.LastWrite && LookModeSortEnableLastWrite)
               )
            {
                CurrentPageValues.Reverse();
                return;
            }

            LookModeSortEnableAssetName = false;
            LookModeSortEnableLastWrite = false;
            LookModeSortEnableSize      = false;
            switch (LookModeSort)
            {
                case ESort.FileSize:
                    LookModeSortEnableSize = true;
                    CurrentPageValues.Sort((data1, data2) =>
                    {
                        if (data1.Size < data2.Size) return minToMax ? 1 : -1;
                        if (data1.Size > data2.Size) return minToMax ? -1 : 1;
                        return 0;
                    });

                    break;
                case ESort.LastWrite:
                    LookModeSortEnableLastWrite = true;
                    CurrentPageValues.Sort((data1, data2) =>
                    {
                        if (data1.LastWriteTime < data2.LastWriteTime) return minToMax ? 1 : -1;
                        if (data1.LastWriteTime > data2.LastWriteTime) return minToMax ? -1 : 1;
                        return 0;
                    });
                    break;
                case ESort.AssetName:
                    LookModeSortEnableAssetName = true;
                    CurrentPageValues.Sort((data1, data2) => // 实现文件名 排序 
                    {
                        var name1 = data1.Address;
                        var name2 = data2.Address;
                        if (name1 == name2) return 0;
                        return minToMax
                            ? string.Compare(name1, name2, StringComparison.Ordinal)
                            : string.Compare(name2, name1, StringComparison.Ordinal);
                    });
                    break;
                case ESort.ObjectType:
                default:
                    return;
            }
        }

        /// <summary>
        ///     绘制 资源查询模式
        /// </summary>
        partial void OnDrawLookMode(Rect rect)
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
        ///     绘制 资源查询模式 资源详情
        /// </summary>
        private void OnDrawLookModeAssetDetail(Rect cellRect)
        {
            EditorGUILayout.LabelField(LookCurrentSelectAssetDataInfo.Address, GEStyle.AMMixerHeader);

            using (new EditorGUILayout.HorizontalScope(GEStyle.toolbarbuttonLeft))
            {
                EditorGUILayout.LabelField("Package");
                EditorGUILayout.LabelField(LookCurrentSelectAssetDataInfo.Package);
            }

            using (new EditorGUILayout.HorizontalScope(GEStyle.toolbarbuttonLeft))
            {
                EditorGUILayout.LabelField("Group", GP_Width_100);
                EditorGUILayout.LabelField(LookCurrentSelectAssetDataInfo.Group);
            }

            using (new EditorGUILayout.HorizontalScope(GEStyle.toolbarbuttonLeft))
            {
                EditorGUILayout.LabelField(GC_LookMode_Detail_IsSubAsset, GP_Width_100);
                EditorGUILayout.LabelField($"{AssetDatabase.IsSubAsset(LookCurrentSelectAsset)}");
            }

            using (new EditorGUILayout.HorizontalScope(GEStyle.toolbarbuttonLeft))
            {
                EditorGUILayout.LabelField(GC_LookMode_Detail_Size, GP_Width_100);
                EditorGUILayout.LabelField(LookCurrentSelectAssetDataInfo.SizeStr);
            }

            using (new EditorGUILayout.HorizontalScope(GEStyle.toolbarbuttonLeft))
            {
                EditorGUILayout.LabelField(GC_LookMode_Detail_Asset, GP_Width_100);
                EditorGUILayout.ObjectField(LookCurrentSelectAsset, LookCurrentSelectAsset.GetType(), false);
            }

            using (new EditorGUILayout.HorizontalScope(GEStyle.toolbarbuttonLeft))
            {
                EditorGUILayout.LabelField(GC_LookMode_Detail_GUID, GP_Width_100);
                EditorGUILayout.LabelField(LookCurrentSelectAssetDataInfo.GUID);
            }

            using (new EditorGUILayout.HorizontalScope(GEStyle.toolbarbuttonLeft))
            {
                EditorGUILayout.LabelField(GC_LookMode_Detail_Type, GP_Width_100);
                EditorGUILayout.LabelField(LookCurrentSelectAssetDataInfo.Type);
            }

            using (new EditorGUILayout.HorizontalScope(GEStyle.toolbarbuttonLeft))
            {
                EditorGUILayout.LabelField(GC_LookMode_Detail_Path, GP_Width_100);
                EditorGUILayout.LabelField(LookCurrentSelectAssetDataInfo.AssetPath);
            }

            using (new EditorGUILayout.HorizontalScope(GEStyle.toolbarbuttonLeft))
            {
                EditorGUILayout.LabelField(GC_LookMode_Detail_LastWriteTime, GP_Width_100);
                EditorGUILayout.LabelField(LookCurrentSelectAssetDataInfo.LastWriteTime.ToString("yyyy-MM-dd hh:mm:ss"));
            }

            if (!string.IsNullOrEmpty(LookCurrentSelectAssetDataInfo.Tags))
            {
                using (new EditorGUILayout.HorizontalScope(GEStyle.toolbarbuttonLeft))
                {
                    EditorGUILayout.LabelField(GC_LookMode_Detail_Tags, GP_Width_100);
                    EditorGUILayout.LabelField(LookCurrentSelectAssetDataInfo.Tags);
                    GELayout.ButtonCopy(GC_COPY, LookCurrentSelectAssetDataInfo.Tags, 16, GEStyle.IconButton);
                }
            }

            if (Config.EnableSequenceRecord)
            {
                using (new EditorGUILayout.HorizontalScope(GEStyle.toolbarbuttonLeft))
                {
                    EditorGUILayout.LabelField("首包资源", GP_Width_100);
                    EditorGUILayout.LabelField(Config.SequenceRecord.ContainsGUID(LookCurrentSelectAssetDataInfo.GUID) ? "是" : "否");
                }
            }

            if (Dependencies.Count > 0)
            {
                EditorGUILayout.LabelField($"Dependencies({Dependencies.Count})[{DependenciesSize.ToConverseStringFileSize()}]", GEStyle.HeaderLabel);
                using (new EditorGUILayout.HorizontalScope(GEStyle.Toolbar))
                {
                    DependenciesSearchText = EditorGUILayout.TextField(DependenciesSearchText, GEStyle.SearchTextField);
                    if (!string.IsNullOrEmpty(DependenciesSearchText))
                        if (GUILayout.Button("✘", GEStyle.toolbarbuttonLeft, GTOptions.Width(21)))
                        {
                            GUI.FocusControl(null);
                            DependenciesSearchText = string.Empty;
                        }
                }

                LookModeShowAssetDetailScroll = GELayout.BeginScrollView(LookModeShowAssetDetailScroll);
                foreach (var dependency in Dependencies)
                {
                    if (!string.IsNullOrEmpty(DependenciesSearchText))
                        if (!DependenciesSearchText.Contains(dependency.Value.Name, StringComparison.CurrentCultureIgnoreCase) &&
                            !DependenciesSearchText.Contains(dependency.Value.Type, StringComparison.CurrentCultureIgnoreCase) &&
                            !dependency.Value.Name.Contains(DependenciesSearchText, StringComparison.CurrentCultureIgnoreCase) &&
                            !dependency.Value.Type.Contains(DependenciesSearchText, StringComparison.CurrentCultureIgnoreCase)
                           )
                            continue;

                    using (new EditorGUILayout.HorizontalScope(GEStyle.toolbarbuttonLeft))
                    {
                        EditorGUILayout.LabelField(dependency.Value.Name, GUILayout.MinWidth(100), GUILayout.ExpandWidth(true));
                        EditorGUILayout.LabelField(dependency.Value.Size.ToConverseStringFileSize(), GUILayout.MinWidth(50), GUILayout.MaxWidth(125));
                        EditorGUILayout.ObjectField(dependency.Value.Object, dependency.Value.GetType(), false, GUILayout.MinWidth(50), GUILayout.MaxWidth(150));
                        if (GUILayout.Button(GC_LookMode_Object_Select, GEStyle.IconButton, GTOptions.Width(16)))
                        {
                            EditorUtility.RevealInFinder(dependency.Key);
                            Selection.activeObject = dependency.Value.Object;
                        }
                    }
                }

                GELayout.EndScrollView();
            }
        }

        /// <summary>
        ///     资源查询模式 资源过滤器
        /// </summary>
        private bool LookModeDataFilter(AssetDataInfo data)
        {
            var filter = 0;
            if (IsFilterCollectors(
                    LookModeDisplayCollectorsIndex,
                    data.CollectPath,
                    LookModeDisplayCollectors?.
                        GetOrDefault((Data.CurrentPackageIndex, Data.CurrentGroupIndex), Array.Empty<string>()))
               ) filter++;

            if (IsFilterTypes(
                    LookModeDisplayTypeIndex,
                    data.AssetPath,
                    LookModeDisplayTypes?.GetOrDefault((Data.CurrentPackageIndex, Data.CurrentGroupIndex), Array.Empty<string>()))
               ) filter++;

            if (IsFilterTags(
                    LookModeDisplayTagsIndex,
                    data.Tags.Split(';', ',', ' '),
                    LookModeDisplayTags?.GetOrDefault((Data.CurrentPackageIndex, Data.CurrentGroupIndex), Array.Empty<string>()))
               ) filter++;

            if (IsFilterSearch(SearchText, data))
                filter++;

            return filter != 4;
        }

        /// <summary>
        ///     更新资源查询模式收集器
        /// </summary>
        private void UpdateDataLookModeCollector(int packageIndex, int groupIndex)
        {
            var i = packageIndex;
            var j = groupIndex;
            var key = (i, j);
            if (Data.Count <= i) return;
            if (Data.Packages[i] is null || Data.Packages[i].Groups is null || j < 0) return;
            if (Data.Packages[i].Count <= j) return;
            LookModeDisplayTags[key]                          = Data.Packages[i].Groups[j].Tags;
            LookModeDisplayGroups[LookModeDisplayPackages[i]] = GetGroupDisPlayNames(Data.Packages[i].Groups);
            LookModeDisplayCollectors[key]                    = Data.Packages[i].Groups[j].Collectors.GetDisPlayNames();
            LookModeDisplayCollectors[key]                    = GetCollectorDisPlayNames(LookModeDisplayCollectors[key]);
            LookModeData[key]                                 = new List<AssetDataInfo>();
            LookModeDisplayTypes[(i, j)]                      = Array.Empty<string>();
            var listTypes = new List<string>();
            foreach (var item in Data.Packages[i].Groups[j].Collectors)
                item.CollectAssetAsync(Data.Packages[i].Name, Data.Packages[i].Groups[j].Name, dictionary =>
                {
                    Runner.StartCoroutine(() =>
                    {
                        listTypes.AddRange(dictionary.Select(pair => pair.Value.Type));
                        LookModeDisplayTypes[(i, j)] = listTypes.Distinct().ToArray();
                        Repaint();
                    });
                    foreach (var pair in dictionary)
                    {
                        LookModeData[(i, j)].Add(pair.Value);
                        if (LookModeDataFilter(pair.Value)) continue;
                        CurrentPageValues.Add(pair.Value);
                        LookModeCollectorsALLSize += pair.Value.Size;
                    }

                    CurrentPageValues.PageIndex = CurrentPageValues.PageIndex;
                    ViewTreeQueryAsset.Reload();
                });
        }

        private void UpdateDataLookMode()
        {
            GUI.FocusControl(null);
            if (!Data.IsValidCollect()) return;

            LookModeDisplayPackages = new string[Data.Packages.Length];
            for (var i = 0; i < Data.Packages.Length; i++) LookModeDisplayPackages[i] = Data.Packages[i].Name;

            if (LookModeDisplayCollectors is null) LookModeDisplayCollectors = new Dictionary<(int, int), string[]>();
            if (LookModeDisplayTags is null) LookModeDisplayTags             = new Dictionary<(int, int), string[]>();
            if (LookModeDisplayTypes is null) LookModeDisplayTypes           = new Dictionary<(int, int), string[]>();
            if (LookModeDisplayGroups is null) LookModeDisplayGroups         = new Dictionary<string, string[]>();
            if (LookModeData is null) LookModeData                           = new Dictionary<(int, int), List<AssetDataInfo>>();

            LookModeCollectorsALLSize  = 0;
            LookModeCollectorsPageSize = 0;
            CurrentPageValues.Clear();
            CurrentPageValues.PageIndex = 0;

            UpdateDataLookModeCollector(Data.CurrentPackageIndex, Data.CurrentGroupIndex);
            ViewTreeQueryAsset.Reload();
        }
    }
}