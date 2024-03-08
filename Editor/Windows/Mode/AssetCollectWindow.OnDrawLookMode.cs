using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
                GEStyle.PreDropDown, GP_MAX_Width_100, GP_MIN_Width_50);

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
                GEStyle.PreDropDown, GP_MAX_Width_100, GP_MIN_Width_50);

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
                        GEStyle.PreDropDown, GP_MAX_Width_100, GP_MIN_Width_50);
                }
                else if (LookModeDisplayCollectors[(Data.CurrentPackageIndex, Data.CurrentGroupIndex)].Length >= 31)
                {
                    LookModeDisplayCollectorsIndex = EditorGUILayout.Popup(LookModeDisplayCollectorsIndex,
                        LookModeDisplayCollectors[(Data.CurrentPackageIndex, Data.CurrentGroupIndex)],
                        GEStyle.PreDropDown, GP_MAX_Width_100, GP_MIN_Width_50);
                }
                else
                {
                    LookModeDisplayCollectorsIndex = EditorGUILayout.MaskField(LookModeDisplayCollectorsIndex,
                        LookModeDisplayCollectors[(Data.CurrentPackageIndex, Data.CurrentGroupIndex)],
                        GEStyle.PreDropDown, GP_MAX_Width_100, GP_MIN_Width_50);
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
                        GEStyle.PreDropDown, GP_MAX_Width_100, GP_MIN_Width_50);
                }
                else
                {
                    LookModeDisplayTypeIndex = EditorGUILayout.MaskField(LookModeDisplayTypeIndex,
                        LookModeDisplayTypes[(Data.CurrentPackageIndex, Data.CurrentGroupIndex)],
                        GEStyle.PreDropDown, GP_MAX_Width_100, GP_MIN_Width_50);
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
                        GEStyle.PreDropDown, GP_MAX_Width_100, GP_MIN_Width_50);
                }
                else
                {
                    LookModeDisplayTagsIndex = EditorGUILayout.MaskField(LookModeDisplayTagsIndex,
                        LookModeDisplayTags[(Data.CurrentPackageIndex, Data.CurrentGroupIndex)],
                        GEStyle.PreDropDown, GP_MAX_Width_100, GP_MIN_Width_50);
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
                    lock (LookModeData)
                    {
                        CurrentPageValues.Add(LookModeData[(Data.CurrentPackageIndex, Data.CurrentGroupIndex)]
                            .Where(data => !LookModeDataFilter(data)));
                    }

                    CurrentPageValues.PageIndex = 0;
                    LookModeCollectorsALLSize = CurrentPageValues.Sum(data => data.Size);
                    TempTable[nameof(SearchText)] = SearchText;
                    TempTable[nameof(LookModeDisplayCollectorsIndex)] = LookModeDisplayCollectorsIndex;
                    TempTable[nameof(LookModeDisplayTypeIndex)] = LookModeDisplayTypeIndex;
                    TempTable[nameof(LookModeDisplayTagsIndex)] = LookModeDisplayTagsIndex;
                }
            }

            OnDrawHeaderLookPageSetting();

            if (GUILayout.Button(GC_REFRESH, GEStyle.TEtoolbarbutton, GP_Width_25))
            {
                LookCurrentSelectAsset = null;
                SearchText = string.Empty;
                LookModeDisplayTagsIndex = LookModeDisplayTypeIndex = LookModeDisplayCollectorsIndex = 0;
                UpdateDataLookMode();
            }
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
            if (Data.Length == 0)
            {
                GELayout.HelpBox("当前无包资源数据");
                return;
            }

            if (Data.CurrentPackage.Length == 0)
            {
                GELayout.HelpBox("当前无组资源数据");
                return;
            }

            if (Data.CurrentGroup.Length == 0)
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
                    EditorGUILayout.LabelField(LookCurrentSelectAssetDataInfo.Address, GEStyle.AMMixerHeader,
                        GP_Height_30);
                }

                using (new EditorGUILayout.HorizontalScope(GEStyle.toolbarbuttonLeft))
                {
                    EditorGUILayout.LabelField("Package", GP_Width_100);
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
                    EditorGUILayout.ObjectField(LookCurrentSelectAsset, LookCurrentSelectAsset.GetType(),
                        false);
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
                    var timer =
                        LookCurrentSelectAssetDataInfo.LastWriteTime.ToString("yyyy-MM-dd hh:mm:ss");
                    EditorGUILayout.LabelField(timer);
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
                        EditorGUILayout.LabelField(
                            Config.SequenceRecord.ContainsGUID(LookCurrentSelectAssetDataInfo.GUID)
                                ? "是"
                                : "否");
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
            var rect4 = new Rect(rect)
            {
                width = 80,
                x = rect.width - 80
            };

            var rect3 = new Rect(rect4)
            {
                x = rect4.x - 80,
                width = 80
            };

            var rect2 = new Rect(rect3);
            if (LookModeShowAssetDetail)
            {
                rect2.width = 0;
                rect2.x = rect3.x;
            }
            else
            {
                rect2.width = (rect.width - rect3.width - rect4.width) / 2 - 100;
                rect2.x = rect3.x - rect2.width;
            }

            var rect1 = new Rect(rect2)
            {
                x = 0,
                width = rect2.x
            };


            GUI.Box(rect1, "", GEStyle.TEtoolbarbutton);
            if (!LookModeShowAssetDetail) GUI.Box(rect2, "", GEStyle.TEtoolbarbutton);
            GUI.Box(rect3, "", GEStyle.TEtoolbarbutton);
            GUI.Box(rect4, "", GEStyle.TEtoolbarbutton);

            var rect1Content = new GUIContent(
                $"    Asset[{LookModeCollectorsPageSize.ToConverseStringFileSize()}\\{LookModeCollectorsALLSize.ToConverseStringFileSize()}]");
            var rect2Content = new GUIContent("    AssetPath");
            var rect3Content = new GUIContent("    Size");
            var rect4Content = new GUIContent("    Ago");
            if (LookModeSortEnableAssetName) rect1Content.image = GC_LookMode_Data_Sort.image;
            else if (LookModeSortEnableSize) rect3Content.image = GC_LookMode_Data_Sort.image;
            else if (LookModeSortEnableLastWrite) rect4Content.image = GC_LookMode_Data_Sort.image;

            if (GUI.Button(rect1, rect1Content, GEStyle.HeaderLabel))
            {
                LookModeSortEnableAssetNameToMin = !LookModeSortEnableAssetNameToMin;
                LookModeDataPageValueSort(ESort.AssetName, LookModeSortEnableAssetNameToMin);
            }

            if (!LookModeShowAssetDetail) GUI.Label(rect2, rect2Content, GEStyle.HeaderLabel);

            if (GUI.Button(rect3, rect3Content, GEStyle.HeaderLabel))
            {
                LookModeSortEnableSizeToMin = !LookModeSortEnableSizeToMin;
                LookModeDataPageValueSort(ESort.FileSize, LookModeSortEnableSizeToMin);
            }

            if (GUI.Button(rect4, rect4Content, GEStyle.HeaderLabel))
            {
                LookModeSortEnableLastWriteToMin = !LookModeSortEnableLastWriteToMin;
                LookModeDataPageValueSort(ESort.LastWrite, LookModeSortEnableLastWriteToMin);
            }
        }

        // #D6EAF8 
        private static readonly Color ROW_FP_COLOR = new Color(0.839f, 0.918f, 0.973f, 0.1f);

        /// <summary>
        /// 绘制资源展示面板
        /// </summary>
        private void OnDrawLookDataItem(Rect rect, AssetDataInfo data, int index)
        {
            var rect4 = new Rect(rect)
            {
                width = 80,
                x = rect.width - 80 + 10
            };

            var rect3 = new Rect(rect4)
            {
                x = rect4.x - 80,
                width = 80 + 10
            };

            var rect2 = new Rect(rect3);
            if (LookModeShowAssetDetail)
            {
                rect2.width = 0;
                rect2.x = rect3.x;
            }
            else
            {
                rect2.width = (rect.width - rect3.width - rect4.width) / 2 - 100 + 10;
                rect2.x = rect3.x - rect2.width;
            }

            var rect1 = new Rect(rect2)
            {
                x = 10,
                width = rect2.x - 10
            };

            if (CurrentSelectAssetIndex == index) GUI.Box(rect, "", GEStyle.SelectionRect);
            else
            {
                GUI.Box(rect, "", GEStyle.ProjectBrowserHeaderBgMiddle);
                OnDrawShading(rect);
            }

            EditorGUIUtility.SetIconSize(new Vector2(rect.height - 4, rect.height - 4));
            var content = EditorGUIUtility.ObjectContent(AssetDatabase.LoadMainAssetAtPath(data.AssetPath), null);
            content.text = data.Address;
            EditorGUI.LabelField(rect1, content, EditorStyles.label);
            if (!LookModeShowAssetDetail) EditorGUI.LabelField(rect2, data.AssetPath, EditorStyles.label);

            EditorGUI.LabelField(rect3, data.SizeStr, EditorStyles.label);
            EditorGUI.LabelField(rect4, data.GetLatestTime(), EditorStyles.label);

            if (Config.EnableSequenceRecord && WindowMode != Mode.LookFirstPackage)
            {
                var rect5 = new Rect(rect)
                {
                    width = 22,
                    x = rect.width - 20
                };
                if (Config.SequenceRecord.ContainsGUID(data.GUID))
                {
                    if (GUI.Button(rect5, GC_FP_Cancel, GEStyle.TEtoolbarbutton))
                    {
                        Config.SequenceRecord.UpdateLocal();
                        Config.SequenceRecord.RemoveAssetPath(data.AssetPath);
                        Config.SequenceRecord.Save();
                        // UpdatePageValuesFirstPackageMode();
                    }
                }
                else
                {
                    if (GUI.Button(rect5, GC_FP_OK, GEStyle.TEtoolbarbutton))
                    {
                        Config.SequenceRecord.Add(new AssetSystem.SequenceRecord
                        {
                            GUID = data.GUID,
                            AssetPath = data.AssetPath,
                            Location = data.Address,
                            PackageName = data.Package,
                            Bytes = data.Size,
                            Count = 1,
                            Time = DateTime.MinValue
                        });
                        Config.SequenceRecord.Save();
                    }
                }
            }

            var currentEvent = Event.current;
            if (currentEvent.isMouse && rect.Contains(currentEvent.mousePosition))
            {
                if (Event.current.button == 1)
                {
                    GUI.FocusControl(null);
                    onDrawLookDataItemMenu = new GenericMenu();
                    onDrawLookDataItemMenu.AddItem(new GUIContent("打开 资源所在文件夹"), false,
                        () => { EditorUtility.RevealInFinder(data.AssetPath); });
                    if (AHelper.IO.ExistsFile(data.AssetPath))
                    {
                        onDrawLookDataItemMenu.AddItem(new GUIContent("打开 使用默认程序打开"), false,
                            () => { EditorUtility.OpenWithDefaultApp(data.AssetPath); });
                        onDrawLookDataItemMenu.AddItem(new GUIContent("选择 资源"), false,
                            () => { Selection.activeObject = AssetDatabase.LoadMainAssetAtPath(data.AssetPath); });
                    }

                    onDrawLookDataItemMenu.AddItem(new GUIContent("复制 可寻址路径"), false,
                        () => { GUIUtility.systemCopyBuffer = data.Address; });
                    onDrawLookDataItemMenu.AddItem(new GUIContent("复制 资源路径"), false,
                        () => { GUIUtility.systemCopyBuffer = data.AssetPath; });
                    onDrawLookDataItemMenu.AddItem(new GUIContent("复制 GUID"), false,
                        () => { GUIUtility.systemCopyBuffer = data.GUID; });
                    onDrawLookDataItemMenu.AddItem(new GUIContent("复制 资源类型"), false,
                        () => { GUIUtility.systemCopyBuffer = data.Type; });
                    if (!string.IsNullOrEmpty(data.Tags))
                    {
                        onDrawLookDataItemMenu.AddItem(new GUIContent("复制 标签列表"), false,
                            () => { GUIUtility.systemCopyBuffer = data.Tags; });
                    }

                    if (Config.EnableSequenceRecord)
                    {
                        if (WindowMode != Mode.LookFirstPackage)
                        {
                            if (!Config.SequenceRecord.ContainsGUID(data.GUID))
                            {
                                onDrawLookDataItemMenu.AddItem(new GUIContent("添加 首包列表"), false, () =>
                                {
                                    Config.SequenceRecord.Add(new AssetSystem.SequenceRecord
                                    {
                                        GUID = data.GUID,
                                        AssetPath = data.AssetPath,
                                        Location = data.Address,
                                        PackageName = data.Package,
                                        Bytes = data.Size,
                                        Count = 1,
                                        Time = DateTime.MinValue
                                    });
                                    Config.SequenceRecord.Save();
                                });
                            }
                        }
                        else
                        {
                            onDrawLookDataItemMenu.AddItem(new GUIContent("移除 首包列表"), false, () =>
                            {
                                if (LookCurrentSelectAssetDataInfo.GUID == data.GUID)
                                {
                                    LookCurrentSelectAsset = null;
                                }

                                Config.SequenceRecord.UpdateLocal();
                                Config.SequenceRecord.RemoveAssetPath(data.AssetPath);
                                Config.SequenceRecord.Save();
                                UpdatePageValuesFirstPackageMode();
                            });
                        }
                    }

                    onDrawLookDataItemMenu.ShowAsContext();
                }
                else UpdateCurrentSelectAsset(index);

                currentEvent.Use();
            }
        }

        private void CancelCurrentSelectAsset()
        {
            GUI.FocusControl(null);
            LookCurrentSelectAsset = null;
            Dependencies.Clear();
            DependenciesSize = 0;
            DependenciesSearchText = string.Empty;
        }

        private void UpdateCurrentSelectAsset(int index)
        {
            GUI.FocusControl(null);
            if (index < 0 || CurrentPageValues.CurrentPageValues.Length == 0)
            {
                LookCurrentSelectAsset = null;
                Dependencies.Clear();
                DependenciesSize = 0;
                return;
            }

            CurrentSelectAssetIndex = index;
            LookCurrentSelectAssetDataInfo = CurrentPageValues.CurrentPageValues[index];
            LookCurrentSelectAsset = AssetDatabase.LoadAssetAtPath<Object>(LookCurrentSelectAssetDataInfo.AssetPath);
            Dependencies.Clear();
            DependenciesSize = 0;
            foreach (var dependency in AssetDatabase.GetDependencies(LookCurrentSelectAssetDataInfo.AssetPath))
            {
                var temp = AssetDatabase.LoadAssetAtPath<Object>(dependency);
                if (LookCurrentSelectAsset == temp) continue;
                Dependencies[dependency] = new DependenciesInfo
                {
                    Object = temp,
                    AssetPath = dependency,
                    Name = temp.name,
                    Size = new FileInfo(dependency).Length
                };
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
            var key = (i, j);
            if (Data.Length <= i) return;
            if (Data.Packages[i] is null || Data.Packages[i].Groups is null || j < 0) return;
            if (Data.Packages[i].Length <= j) return;
            LookModeDisplayTags[key] = Data.Packages[i].Groups[j].AllTags;
            LookModeDisplayGroups[LookModeDisplayPackages[i]] = GetGroupDisPlayNames(Data.Packages[i].Groups);
            LookModeDisplayCollectors[key] = GetCollectorDisPlayNames(Data.Packages[i].Groups[j].Collectors);
            LookModeDisplayCollectors[key] = GetCollectorDisPlayNames(LookModeDisplayCollectors[key]);
            LookModeData[key] = new List<AssetDataInfo>();
            LookModeDisplayTypes[(i, j)] = Array.Empty<string>();
            var listTypes = new List<string>();
            for (var k = 0; k < Data.Packages[i].Groups[j].Collectors.Length; k++)
            {
                Data.Packages[i].Groups[j].Collectors[k].CollectAssetAsync(
                    Data.Packages[i].Name, Data.Packages[i].Groups[j].Name, dic =>
                    {
                        Runner.StartCoroutine(() =>
                        {
                            foreach (var pair in dic)
                            {
                                listTypes.Add(pair.Value.Type);
                            }

                            LookModeDisplayTypes[(i, j)] = listTypes.Distinct().ToArray();
                            Repaint();
                        });
                        foreach (var pair in dic)
                        {
                            LookModeData[(i, j)].Add(pair.Value);
                            if (!LookModeDataFilter(pair.Value))
                            {
                                CurrentPageValues.Add(pair.Value);
                                LookModeCollectorsALLSize += pair.Value.Size;
                            }
                        }

                        CurrentPageValues.PageIndex = CurrentPageValues.PageIndex;
                    });
            }
        }

        private void UpdateDataLookMode()
        {
            GUI.FocusControl(null);
            if (!Data.IsCollectValid()) return;

            LookModeDisplayPackages = new string[Data.Packages.Length];
            for (var i = 0; i < Data.Packages.Length; i++) LookModeDisplayPackages[i] = Data.Packages[i].Name;

            if (LookModeDisplayCollectors is null) LookModeDisplayCollectors = new Dictionary<(int, int), string[]>();
            if (LookModeDisplayTags is null) LookModeDisplayTags = new Dictionary<(int, int), string[]>();
            if (LookModeDisplayTypes is null) LookModeDisplayTypes = new Dictionary<(int, int), string[]>();
            if (LookModeDisplayGroups is null) LookModeDisplayGroups = new Dictionary<string, string[]>();
            if (LookModeData is null) LookModeData = new Dictionary<(int, int), List<AssetDataInfo>>();

            LookModeCollectorsALLSize = 0;
            LookModeCollectorsPageSize = 0;
            CurrentPageValues.Clear();
            CurrentPageValues.PageIndex = 0;

            UpdateDataLookModeCollector(Data.CurrentPackageIndex, Data.CurrentGroupIndex);
        }
    }
}