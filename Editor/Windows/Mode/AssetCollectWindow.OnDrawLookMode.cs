/*|============|*|
|*|Author:     |*| Star fire
|*|Date:       |*| 2023-12-01
|*|E-Mail:     |*| xinansky99@foxmail.com
|*|============|*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace AIO.UEditor
{
    public partial class AssetCollectWindow
    {
        /// <summary>
        /// 绘制 资源查询模式 导航栏
        /// </summary>
        private void OnDrawHeaderLookMode()
        {
            if (Data.Packages.Length == 0 ||
                LookModeDisplayPackages is null ||
                LookModeDisplayPackages.Length == 0)
            {
                EditorGUILayout.Separator();
                return;
            }

            Data.CurrentPackageIndex = EditorGUILayout.Popup(Data.CurrentPackageIndex, LookModeDisplayPackages,
                GEStyle.PreDropDown,
                GP_Width_100);

            if (!Data.IsGroupValid())
            {
                EditorGUILayout.Separator();
                return;
            }

            var packageName = LookModeDisplayPackages[Data.CurrentPackageIndex];
            if (GUI.changed)
            {
                if (!LookModeDisplayGroups.ContainsKey(packageName))
                {
                    LookModeDisplayGroups[packageName] = GetGroupDisPlayNames(Data.CurrentPackage.Groups);
                }
            }

            if (!LookModeDisplayGroups.ContainsKey(packageName))
            {
                EditorGUILayout.Separator();
                return;
            }

            if (Data.CurrentGroupIndex >= LookModeDisplayGroups[packageName].Length)
                Data.CurrentGroupIndex = LookModeDisplayGroups[packageName].Length - 1;

            Data.CurrentGroupIndex = EditorGUILayout.Popup(
                Data.CurrentGroupIndex, LookModeDisplayGroups[packageName],
                GEStyle.PreDropDown, GP_Width_100);

            if (!Data.IsCollectValid())
            {
                EditorGUILayout.Separator();
                return;
            }

            if (GUI.changed)
            {
                if (!LookModeData.ContainsKey((Data.CurrentPackageIndex, Data.CurrentGroupIndex)))
                {
                    UpdateDataLookModeCollector(Data.CurrentPackageIndex, Data.CurrentGroupIndex);
                }
            }

            SearchText = LookModeData[(Data.CurrentPackageIndex, Data.CurrentGroupIndex)].Count > 300
                ? EditorGUILayout.DelayedTextField(SearchText, GEStyle.SearchTextField)
                : EditorGUILayout.TextField(SearchText, GEStyle.SearchTextField);

            if (GUILayout.Button(GC_CLEAR, GEStyle.TEtoolbarbutton, GP_Width_25))
            {
                GUI.FocusControl(null);
                SearchText = string.Empty;
            }

            if (LookModeDisplayCollectors[(Data.CurrentPackageIndex, Data.CurrentGroupIndex)].Length > 0)
            {
                if (LookModeDisplayCollectors[(Data.CurrentPackageIndex, Data.CurrentGroupIndex)].Length == 1)
                {
                    LookModeDisplayCollectorsIndex = 0;
                    EditorGUILayout.Popup(0,
                        LookModeDisplayCollectors[(Data.CurrentPackageIndex, Data.CurrentGroupIndex)],
                        GEStyle.PreDropDown, GP_Width_100);
                }
                else if (LookModeDisplayCollectors[(Data.CurrentPackageIndex, Data.CurrentGroupIndex)].Length >= 31)
                {
                    LookModeDisplayCollectorsIndex = EditorGUILayout.Popup(LookModeDisplayCollectorsIndex,
                        LookModeDisplayCollectors[(Data.CurrentPackageIndex, Data.CurrentGroupIndex)],
                        GEStyle.PreDropDown, GP_Width_100);
                }
                else
                {
                    LookModeDisplayCollectorsIndex = EditorGUILayout.MaskField(LookModeDisplayCollectorsIndex,
                        LookModeDisplayCollectors[(Data.CurrentPackageIndex, Data.CurrentGroupIndex)],
                        GEStyle.PreDropDown, GP_Width_100);
                }
            }
            else LookModeDisplayCollectorsIndex = 0;

            if (LookModeDisplayTypes[(Data.CurrentPackageIndex, Data.CurrentGroupIndex)].Length > 0)
            {
                if (LookModeDisplayTypes[(Data.CurrentPackageIndex, Data.CurrentGroupIndex)].Length == 1)
                {
                    LookModeDisplayTypeIndex = 0;
                    EditorGUILayout.Popup(0,
                        LookModeDisplayTypes[(Data.CurrentPackageIndex, Data.CurrentGroupIndex)],
                        GEStyle.PreDropDown, GP_Width_100);
                }
                else
                {
                    LookModeDisplayTypeIndex = EditorGUILayout.MaskField(LookModeDisplayTypeIndex,
                        LookModeDisplayTypes[(Data.CurrentPackageIndex, Data.CurrentGroupIndex)],
                        GEStyle.PreDropDown, GP_Width_100);
                }
            }
            else LookModeDisplayTypeIndex = 0;

            if (LookModeDisplayTags[(Data.CurrentPackageIndex, Data.CurrentGroupIndex)].Length > 0)
            {
                if (LookModeDisplayTags[(Data.CurrentPackageIndex, Data.CurrentGroupIndex)].Length == 1)
                {
                    LookModeDisplayTagsIndex = 0;
                    EditorGUILayout.Popup(0,
                        LookModeDisplayTags[(Data.CurrentPackageIndex, Data.CurrentGroupIndex)],
                        GEStyle.PreDropDown, GP_Width_100);
                }
                else
                {
                    LookModeDisplayTagsIndex = EditorGUILayout.MaskField(LookModeDisplayTagsIndex,
                        LookModeDisplayTags[(Data.CurrentPackageIndex, Data.CurrentGroupIndex)],
                        GEStyle.PreDropDown, GP_Width_100);
                }
            }
            else LookModeDisplayTagsIndex = 0;

            if (GUI.changed)
            {
                if (TempTable.GetOrDefault<int>(nameof(Data.CurrentPackageIndex)) != Data.CurrentPackageIndex ||
                    TempTable.GetOrDefault<int>(nameof(Data.CurrentGroupIndex)) != Data.CurrentGroupIndex)
                {
                    CurrentPageValues.Clear();
                    CurrentPageValues.Add(LookModeData[(Data.CurrentPackageIndex, Data.CurrentGroupIndex)]);
                    CurrentPageValues.PageIndex = 0;
                    LookModeCollectorsALLSize = CurrentPageValues.Sum(data => data.Size);
                    TempTable[nameof(Data.CurrentPackageIndex)] = Data.CurrentPackageIndex;
                    TempTable[nameof(Data.CurrentGroupIndex)] = Data.CurrentGroupIndex;
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
                    CurrentPageValues.Add(LookModeData[(Data.CurrentPackageIndex, Data.CurrentGroupIndex)]
                        .Where(data => !LookModeDataFilter(data)));
                    CurrentPageValues.PageIndex = 0;
                    LookModeCollectorsALLSize = CurrentPageValues.Sum(data => data.Size);
                    TempTable[nameof(SearchText)] = SearchText;
                    TempTable[nameof(LookModeDisplayCollectorsIndex)] = LookModeDisplayCollectorsIndex;
                    TempTable[nameof(LookModeDisplayTypeIndex)] = LookModeDisplayTypeIndex;
                    TempTable[nameof(LookModeDisplayTagsIndex)] = LookModeDisplayTagsIndex;
                }
            }

            OnDrawHeaderLookPageSetting();
        }

        /// <summary>
        /// 绘制 资源查询模式 页码
        /// </summary>
        private void OnDrawHeaderLookPageSetting()
        {
            if (CurrentPageValues.Count <= 0) return;

            GUI.enabled = CurrentPageValues.PageIndex != 0;
            if (GUILayout.Button(GC_LookMode_Page_MaxLeft, GEStyle.TEtoolbarbutton, GP_Width_25))
            {
                CurrentPageValues.PageIndex = 0;
                return;
            }

            GUI.enabled = CurrentPageValues.PageIndex > 0;
            if (GUILayout.Button(GC_LookMode_Page_Left, GEStyle.TEtoolbarbutton, GP_Width_25))
            {
                CurrentPageValues.PageIndex -= 1;
                return;
            }

            GUI.enabled = true;
            GUILayout.Label($"{CurrentPageValues.PageIndex + 1}/{CurrentPageValues.PageCount}", GEStyle.MeTimeLabel,
                GP_Width_40);

            GUI.enabled = CurrentPageValues.PageIndex + 1 < CurrentPageValues.PageCount;
            if (GUILayout.Button(GC_LookMode_Page_MaxRight, GEStyle.TEtoolbarbutton, GP_Width_25))
            {
                CurrentPageValues.PageIndex += 1;
                return;
            }

            GUI.enabled = CurrentPageValues.PageIndex + 1 != CurrentPageValues.PageCount;
            if (GUILayout.Button(GC_LookMode_Page_Right, GEStyle.TEtoolbarbutton, GP_Width_25))
            {
                CurrentPageValues.PageIndex = CurrentPageValues.PageCount - 1;
                return;
            }

            GUI.enabled = true;
            if (GUILayout.Button(GC_LookMode_Page_Size, GEStyle.TEtoolbarbutton, GP_Width_25))
            {
                LookDataPageSizeMenu.ShowAsContext();
            }

            if (GUILayout.Button(GC_REFRESH, GEStyle.TEtoolbarbutton, GP_Width_25))
            {
                LookModeCurrentSelectAsset = null;
                SearchText = string.Empty;
                LookModeDisplayTagsIndex = LookModeDisplayTypeIndex = LookModeDisplayCollectorsIndex = 0;
                UpdateDataLookMode();
            }
        }

        /// <summary>
        /// 资源查询模式 页签资源排序
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
            LookModeSortEnableSize = false;
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
        /// 绘制 资源查询模式
        /// </summary>
        partial void OnDrawLookMode()
        {
            if (Data.Packages.Length == 0)
            {
                GELayout.HelpBox("当前无包资源数据");
                return;
            }

            if (Data.CurrentPackage.Groups.Length == 0)
            {
                GELayout.HelpBox("当前无组资源数据");
                return;
            }

            if (Data.CurrentGroup.Collectors.Length == 0)
            {
                GELayout.HelpBox("当前无收集器资源数据");
                return;
            }

            if (!LookModeShowAssetDetail) ViewDetailList.width = CurrentWidth;
            else ViewDetailList.MaxWidth = CurrentWidth - 300;
            ViewDetailList.height = CurrentHeight - DrawHeaderHeight;
            ViewDetailList.IsAllowHorizontal = LookModeShowAssetDetail;
            ViewDetailList.Draw(OnDrawLookModeAssetList, GEStyle.Badge);
            if (LookModeShowAssetDetail)
            {
                ViewDetails.IsShow = true;
                ViewDetails.x = ViewDetailList.width;
                ViewDetails.width = CurrentWidth - ViewDetails.x;
                ViewDetails.height = ViewDetailList.height - 3;
                ViewDetails.Draw(OnDrawLookModeAssetDetail, GEStyle.Badge);
            }
        }

        private void OnDrawLookModeAssetList()
        {
            var headerRect = new Rect(0, 0, ViewDetailList.width, 20);
            if (LookModeShowAssetDetail)
            {
                headerRect.width -= ViewDetailList.DragHorizontalWidth;
            }

            OnDrawLookModeHeader(headerRect);
            if (CurrentPageValues.Count <= 0) return;

            LookModeShowAssetListView.width = headerRect.width - 15;
            LookModeShowAssetListView.height = headerRect.height * CurrentPageValues.CurrentPageValues.Length;

            var rect = new Rect(0, headerRect.y + headerRect.height, headerRect.width,
                ViewDetailList.height - headerRect.y - headerRect.height);
            OnDrawLookDataScroll = GUI.BeginScrollView(rect, OnDrawLookDataScroll, LookModeShowAssetListView);

            headerRect.x = 1;
            headerRect.width -= 2;
            LookModeCollectorsPageSize = 0;
            LookModeCollectorsPageIndex = 0;
            foreach (var data in CurrentPageValues.CurrentPageValues)
            {
                LookModeCollectorsPageSize += data.Size;
                OnDrawLookDataItem(headerRect, data, LookModeCollectorsPageIndex++);
                headerRect.y += headerRect.height;
            }

            GUI.EndScrollView();
        }

        /// <summary>
        /// 绘制 资源查询模式 资源详情
        /// </summary>
        private void OnDrawLookModeAssetDetail()
        {
            using (new EditorGUILayout.VerticalScope(GEStyle.GridList))
            {
                using (new EditorGUILayout.HorizontalScope(GP_Height_30))
                {
                    EditorGUILayout.LabelField(LookModeCurrentSelectAssetDataInfo.Address, GEStyle.AMMixerHeader,
                        GP_Height_30);
                }

                using (new EditorGUILayout.HorizontalScope(GEStyle.toolbarbuttonLeft))
                {
                    EditorGUILayout.LabelField(GC_LookMode_Detail_IsSubAsset, GP_Width_100);
                    EditorGUILayout.LabelField($"{AssetDatabase.IsSubAsset(LookModeCurrentSelectAsset)}");
                }

                using (new EditorGUILayout.HorizontalScope(GEStyle.toolbarbuttonLeft))
                {
                    EditorGUILayout.LabelField(GC_LookMode_Detail_Size, GP_Width_100);
                    EditorGUILayout.LabelField(LookModeCurrentSelectAssetDataInfo.SizeStr);
                }

                using (new EditorGUILayout.HorizontalScope(GEStyle.toolbarbuttonLeft))
                {
                    EditorGUILayout.LabelField(GC_LookMode_Detail_Asset, GP_Width_100);
                    EditorGUILayout.ObjectField(LookModeCurrentSelectAsset, LookModeCurrentSelectAsset.GetType(),
                        false);
                    if (GELayout.Button(GC_OPEN_FOLDER, GEStyle.IconButton, 16))
                        EditorUtility.RevealInFinder(LookModeCurrentSelectAssetDataInfo.AssetPath);
                }

                using (new EditorGUILayout.HorizontalScope(GEStyle.toolbarbuttonLeft))
                {
                    EditorGUILayout.LabelField(GC_LookMode_Detail_GUID, GP_Width_100);
                    EditorGUILayout.LabelField(LookModeCurrentSelectAssetDataInfo.GUID);
                    GELayout.ButtonCopy(GC_COPY, LookModeCurrentSelectAssetDataInfo.GUID, 16, GEStyle.IconButton);
                }

                using (new EditorGUILayout.HorizontalScope(GEStyle.toolbarbuttonLeft))
                {
                    EditorGUILayout.LabelField(GC_LookMode_Detail_Type, GP_Width_100);
                    EditorGUILayout.LabelField(LookModeCurrentSelectAssetDataInfo.Type);
                    GELayout.ButtonCopy(GC_COPY, LookModeCurrentSelectAssetDataInfo.Type, 16, GEStyle.IconButton);
                }

                using (new EditorGUILayout.HorizontalScope(GEStyle.toolbarbuttonLeft))
                {
                    EditorGUILayout.LabelField(GC_LookMode_Detail_Path, GP_Width_100);
                    EditorGUILayout.LabelField(LookModeCurrentSelectAssetDataInfo.AssetPath);
                    GELayout.ButtonCopy(GC_COPY, LookModeCurrentSelectAssetDataInfo.AssetPath, 16, GEStyle.IconButton);
                }

                using (new EditorGUILayout.HorizontalScope(GEStyle.toolbarbuttonLeft))
                {
                    EditorGUILayout.LabelField(GC_LookMode_Detail_LastWriteTime, GP_Width_100);
                    var timer =
                        LookModeCurrentSelectAssetDataInfo.LastWriteTime.ToString("yyyy-MM-dd hh:mm:ss");
                    EditorGUILayout.LabelField(timer);
                    GELayout.ButtonCopy(GC_COPY, timer, 16, GEStyle.IconButton);
                }

                if (!string.IsNullOrEmpty(LookModeCurrentSelectAssetDataInfo.Tags))
                {
                    using (new EditorGUILayout.HorizontalScope(GEStyle.toolbarbuttonLeft))
                    {
                        EditorGUILayout.LabelField(GC_LookMode_Detail_Tags, GP_Width_100);
                        EditorGUILayout.LabelField(LookModeCurrentSelectAssetDataInfo.Tags);
                        GELayout.ButtonCopy(GC_COPY, LookModeCurrentSelectAssetDataInfo.Tags, 16, GEStyle.IconButton);
                    }
                }

                if (Dependencies.Count > 0)
                {
                    using (new EditorGUILayout.VerticalScope(GEStyle.ProjectBrowserHeaderBgMiddle))
                    {
                        EditorGUILayout.LabelField(
                            $"Dependencies({Dependencies.Count})[{DependenciesSize.ToConverseStringFileSize()}]",
                            GEStyle.HeaderLabel);
                    }

                    using (new EditorGUILayout.VerticalScope(GEStyle.Badge))
                    {
                        using (new EditorGUILayout.HorizontalScope(GEStyle.Toolbar))
                        {
                            DependenciesSearchText =
                                EditorGUILayout.TextField(DependenciesSearchText, GEStyle.SearchTextField);
                            if (!string.IsNullOrEmpty(DependenciesSearchText))
                            {
                                if (GUILayout.Button("✘", GEStyle.toolbarbuttonLeft, GTOption.Width(21)))
                                {
                                    GUI.FocusControl(null);
                                    DependenciesSearchText = string.Empty;
                                }
                            }
                        }

                        LookModeShowAssetDetailScroll = GELayout.BeginScrollView(LookModeShowAssetDetailScroll);
                        foreach (var dependency in Dependencies)
                        {
                            if (!string.IsNullOrEmpty(DependenciesSearchText))
                            {
                                if (!DependenciesSearchText.Contains(dependency.Value.Name,
                                        StringComparison.CurrentCultureIgnoreCase) &&
                                    !DependenciesSearchText.Contains(dependency.Value.Type,
                                        StringComparison.CurrentCultureIgnoreCase) &&
                                    !dependency.Value.Name.Contains(DependenciesSearchText,
                                        StringComparison.CurrentCultureIgnoreCase) &&
                                    !dependency.Value.Type.Contains(DependenciesSearchText,
                                        StringComparison.CurrentCultureIgnoreCase)
                                   ) continue;
                            }

                            using (new EditorGUILayout.HorizontalScope(GEStyle.toolbarbuttonLeft))
                            {
                                EditorGUILayout.LabelField(dependency.Value.Name, GUILayout.MinWidth(100),
                                    GUILayout.ExpandWidth(true));
                                EditorGUILayout.LabelField(dependency.Value.Size.ToConverseStringFileSize(),
                                    GUILayout.MinWidth(50), GUILayout.MaxWidth(125));
                                EditorGUILayout.ObjectField(dependency.Value.Object, dependency.Value.GetType(), false,
                                    GUILayout.MinWidth(50), GUILayout.MaxWidth(150));
                                if (GUILayout.Button(GC_LookMode_Object_Select, GEStyle.IconButton, GTOption.Width(16)))
                                {
                                    EditorUtility.RevealInFinder(dependency.Key);
                                    Selection.activeObject = dependency.Value.Object;
                                }
                            }
                        }

                        GELayout.EndScrollView();
                    }
                }

                EditorGUILayout.Space();
                EditorGUILayout.Separator();
            }
        }

        /// <summary>
        /// 资源查询模式 资源过滤器
        /// </summary>
        private bool LookModeDataFilter(AssetDataInfo data)
        {
            var filter = 0;
            if (IsFilterCollectors(
                    LookModeDisplayCollectorsIndex, data.CollectPath,
                    LookModeDisplayCollectors?.GetOrDefault((Data.CurrentPackageIndex, Data.CurrentGroupIndex),
                        Array.Empty<string>()))
               ) filter++;

            if (IsFilterTypes(LookModeDisplayTypeIndex, data.AssetPath,
                    LookModeDisplayTypes?.GetOrDefault((Data.CurrentPackageIndex, Data.CurrentGroupIndex),
                        Array.Empty<string>()))
               ) filter++;

            if (IsFilterTags(LookModeDisplayTagsIndex, data.Tags.Split(';', ',', ' '),
                    LookModeDisplayTags?.GetOrDefault((Data.CurrentPackageIndex, Data.CurrentGroupIndex),
                        Array.Empty<string>()))
               ) filter++;

            if (IsFilterSearch(SearchText, data))
                filter++;

            return filter != 4;
        }

        private void OnDrawLookModeHeader(Rect rect)
        {
            var rect1 = new Rect(rect)
            {
                x = 0,
                width = rect.width - 160,
            };

            var rect2 = new Rect(rect)
            {
                x = rect1.x + rect1.width,
                width = 80
            };

            var rect3 = new Rect(rect2)
            {
                x = rect2.x + rect2.width,
            };

            GUI.Box(rect1, "", GEStyle.TEtoolbarbutton);
            GUI.Box(rect2, "", GEStyle.TEtoolbarbutton);
            GUI.Box(rect3, "", GEStyle.TEtoolbarbutton);

            var rect1Content = new GUIContent(
                $"    Asset[{LookModeCollectorsPageSize.ToConverseStringFileSize()}\\{LookModeCollectorsALLSize.ToConverseStringFileSize()}]");
            var rect2Content = new GUIContent("    Size");
            var rect3Content = new GUIContent("    Ago");

            if (LookModeSortEnableAssetName) rect1Content.image = GC_LookMode_Data_Sort.image;
            else if (LookModeSortEnableSize) rect2Content.image = GC_LookMode_Data_Sort.image;
            else if (LookModeSortEnableLastWrite) rect3Content.image = GC_LookMode_Data_Sort.image;

            if (GUI.Button(rect1, rect1Content, GEStyle.HeaderLabel))
            {
                LookModeSortEnableAssetNameToMin = !LookModeSortEnableAssetNameToMin;
                LookModeDataPageValueSort(ESort.AssetName, LookModeSortEnableAssetNameToMin);
            }

            if (GUI.Button(rect2, rect2Content, GEStyle.HeaderLabel))
            {
                LookModeSortEnableSizeToMin = !LookModeSortEnableSizeToMin;
                LookModeDataPageValueSort(ESort.FileSize, LookModeSortEnableSizeToMin);
            }

            if (GUI.Button(rect3, rect3Content, GEStyle.HeaderLabel))
            {
                LookModeSortEnableLastWriteToMin = !LookModeSortEnableLastWriteToMin;
                LookModeDataPageValueSort(ESort.LastWrite, LookModeSortEnableLastWriteToMin);
            }
        }

        /// <summary>
        /// 绘制资源展示面板
        /// </summary>
        private void OnDrawLookDataItem(Rect rect, AssetDataInfo data, int index)
        {
            var rect1 = new Rect(rect)
            {
                x = 10,
                width = rect.width - 160
            };

            var rect2 = new Rect(rect)
            {
                x = rect1.x + rect1.width,
                width = 80
            };

            var rect3 = new Rect(rect2)
            {
                x = rect2.x + rect2.width,
            };

            GUI.Box(rect, "", GEStyle.ProjectBrowserHeaderBgMiddle);
            OnDrawShading(rect);

            if (CurrentSelectAssetIndex == index) GUI.Box(rect, "", GEStyle.SelectionRect);

            EditorGUIUtility.SetIconSize(new Vector2(rect.height - 4, rect.height - 4));
            var content = EditorGUIUtility.ObjectContent(AssetDatabase.LoadMainAssetAtPath(data.AssetPath), null);
            content.text = data.Address;
            EditorGUI.LabelField(rect1, content, EditorStyles.label);
            EditorGUI.LabelField(rect2, data.SizeStr, EditorStyles.label);
            EditorGUI.LabelField(rect3, data.GetLatestTime(), EditorStyles.label);

            if (Event.current.isMouse && rect.Contains(Event.current.mousePosition))
            {
                if (Event.current.button == 1)
                {
                    GUI.FocusControl(null);
                    onDrawLookDataItemMenu = new GenericMenu();
                    onDrawLookDataItemMenu.AddItem(new GUIContent("Select Asset"), false,
                        () => { Selection.activeObject = AssetDatabase.LoadMainAssetAtPath(data.AssetPath); });
                    onDrawLookDataItemMenu.AddItem(new GUIContent("Copy Address"), false,
                        () => { GUIUtility.systemCopyBuffer = data.Address; });
                    onDrawLookDataItemMenu.AddItem(new GUIContent("Copy AssetPath"), false,
                        () => { GUIUtility.systemCopyBuffer = data.AssetPath; });
                    onDrawLookDataItemMenu.AddItem(new GUIContent("Copy GUID"), false,
                        () => { GUIUtility.systemCopyBuffer = data.GUID; });
                    onDrawLookDataItemMenu.AddItem(new GUIContent("Copy Type"), false,
                        () => { GUIUtility.systemCopyBuffer = data.Type; });
                    onDrawLookDataItemMenu.AddItem(new GUIContent("Copy Tags"), false,
                        () => { GUIUtility.systemCopyBuffer = data.Tags; });
                    onDrawLookDataItemMenu.ShowAsContext();
                }
                else UpdateCurrentSelectAsset(index);

                Event.current.Use();
            }
        }

        private void UpdateCurrentSelectAsset(int index)
        {
            GUI.FocusControl(null);
            if (index < 0 || CurrentPageValues.CurrentPageValues.Length == 0)
            {
                LookModeCurrentSelectAsset = null;
                Dependencies.Clear();
                DependenciesSize = 0;
                return;
            }

            CurrentSelectAssetIndex = index;
            var data = CurrentPageValues.CurrentPageValues[index];
            LookModeCurrentSelectAsset = AssetDatabase.LoadAssetAtPath<Object>(data.AssetPath);
            LookModeCurrentSelectAssetDataInfo = data;
            Dependencies.Clear();
            DependenciesSize = 0;
            foreach (var dependency in
                     AssetDatabase.GetDependencies(LookModeCurrentSelectAssetDataInfo.AssetPath))
            {
                var temp = AssetDatabase.LoadAssetAtPath<Object>(dependency);
                if (LookModeCurrentSelectAsset == temp) continue;
                Dependencies[dependency] = new DependenciesInfo
                {
                    Object = temp,
                    Type = AssetDatabase.GetMainAssetTypeAtPath(dependency)?.FullName,
                    AssetPath = dependency,
                    Name = temp.name,
                    Size = new FileInfo(dependency).Length
                };
                if (string.IsNullOrEmpty(Dependencies[dependency].Type)) Dependencies[dependency].Type = "Unknown";
                DependenciesSize += Dependencies[dependency].Size;
            }

            DependenciesSearchText = string.Empty;
            ViewDetailList.width = CurrentWidth / 2;
        }

        /// <summary>
        /// 更新资源查询模式收集器
        /// </summary>
        private void UpdateDataLookModeCollector(int packageIndex, int groupIndex)
        {
            var i = packageIndex;
            var j = groupIndex;
            if (Data.Packages is null || i < 0) return;
            if (Data.Packages.Length <= i) return;
            if (Data.Packages[i] is null || Data.Packages[i].Groups is null || j < 0) return;
            if (Data.Packages[i].Groups.Length <= j) return;

            LookModeDisplayGroups[LookModeDisplayPackages[i]] = Array.Empty<string>();
            LookModeDisplayCollectors[(i, j)] = Array.Empty<string>();
            LookModeDisplayTags[(i, j)] = Array.Empty<string>();
            LookModeDisplayTypes[(i, j)] = Array.Empty<string>();
            Task.Factory.StartNew(() =>
            {
                LookModeDisplayTags[(i, j)] = Data.Packages[i].Groups[j].GetTags();
                LookModeDisplayGroups[LookModeDisplayPackages[i]] = GetGroupDisPlayNames(Data.Packages[i].Groups);
                LookModeDisplayCollectors[(i, j)] = GetCollectorDisPlayNames(Data.Packages[i].Groups[j].Collectors);
                LookModeDisplayCollectors[(i, j)] = GetCollectorDisPlayNames(LookModeDisplayCollectors[(i, j)]);
            });

            LookModeData[(i, j)] = new List<AssetDataInfo>();
            var listTypes = new List<string>();
            for (var k = 0; k < Data.Packages[i].Groups[j].Collectors.Length; k++)
            {
                Data.Packages[i].Groups[j].Collectors[k].CollectAssetTask(
                    Data.Packages[i].Name,
                    Data.Packages[i].Groups[j].Name,
                    dic =>
                    {
                        foreach (var pair in dic)
                        {
                            listTypes.Add(pair.Value.Type);
                            LookModeData[(i, j)].Add(pair.Value);
                            CurrentPageValues.Add(pair.Value);
                            LookModeCollectorsALLSize += pair.Value.Size;
                        }

                        LookModeDisplayTypes[(i, j)] = listTypes.Distinct().ToArray();
                        CurrentPageValues.PageIndex = CurrentPageValues.PageIndex;
                        Repaint();
                    });
            }
        }

        private void UpdateDataLookMode()
        {
            GUI.FocusControl(null);
            if (!Data.IsCollectValid()) return;

            LookModeDisplayPackages = new string[Data.Packages.Length];

            if (LookModeDisplayCollectors is null) LookModeDisplayCollectors = new Dictionary<(int, int), string[]>();
            else LookModeDisplayCollectors.Clear();

            if (LookModeDisplayTags is null) LookModeDisplayTags = new Dictionary<(int, int), string[]>();
            else LookModeDisplayTags.Clear();

            if (LookModeDisplayTypes is null)
                LookModeDisplayTypes = new Dictionary<(int, int), string[]>();
            else LookModeDisplayTypes.Clear();

            if (LookModeDisplayGroups is null) LookModeDisplayGroups = new Dictionary<string, string[]>();
            else LookModeDisplayGroups.Clear();

            if (LookModeData is null) LookModeData = new Dictionary<(int, int), List<AssetDataInfo>>();
            else LookModeData.Clear();

            for (var i = 0; i < Data.Packages.Length; i++) LookModeDisplayPackages[i] = Data.Packages[i].Name;

            LookModeCollectorsALLSize = 0;
            LookModeCollectorsPageSize = 0;
            CurrentPageValues.Clear();
            CurrentPageValues.PageIndex = 0;

            UpdateDataLookModeCollector(Data.CurrentPackageIndex, Data.CurrentGroupIndex);
        }
    }
}