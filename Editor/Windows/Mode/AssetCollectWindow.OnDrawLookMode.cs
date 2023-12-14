/*|============|*|
|*|Author:     |*| Star fire
|*|Date:       |*| 2023-12-01
|*|E-Mail:     |*| xinansky99@foxmail.com
|*|============|*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
            if (Data.Packages.Length == 0)
            {
                EditorGUILayout.Separator();
                return;
            }

            CurrentPackageIndex = EditorGUILayout.Popup(CurrentPackageIndex, LookModeDisplayPackages,
                GEStyle.PreDropDown,
                GP_Width_100);

            if (Data.Packages[CurrentPackageIndex].Groups.Length == 0)
            {
                EditorGUILayout.Separator();
                return;
            }

            var packageName = LookModeDisplayPackages[CurrentPackageIndex];
            if (GUI.changed)
            {
                if (!LookModeDisplayGroups.ContainsKey(packageName))
                {
                    UpdateDataLookModeGroup(CurrentPackageIndex);
                }
            }

            if (CurrentGroupIndex >= LookModeDisplayGroups[packageName].Length)
                CurrentGroupIndex = LookModeDisplayGroups[packageName].Length - 1;
            CurrentGroupIndex = EditorGUILayout.Popup(
                CurrentGroupIndex, LookModeDisplayGroups[packageName],
                GEStyle.PreDropDown, GP_Width_100);

            if (Data.Packages[CurrentPackageIndex].Groups[CurrentGroupIndex].Collectors.Length == 0)
            {
                EditorGUILayout.Separator();
                return;
            }

            if (GUI.changed)
            {
                if (!LookModeData.ContainsKey((CurrentPackageIndex, CurrentGroupIndex)))
                {
                    UpdateDataLookModeCollector(CurrentPackageIndex, CurrentGroupIndex);
                }
            }

            SearchText = EditorGUILayout.DelayedTextField(SearchText, GEStyle.SearchTextField);
            if (GUILayout.Button(GC_DEL, GEStyle.TEtoolbarbutton, GP_Width_25))
            {
                GUI.FocusControl(null);
                SearchText = string.Empty;
            }

            if (LookModeDisplayCollectors[(CurrentPackageIndex, CurrentGroupIndex)].Length > 0)
            {
                LookModeDisplayCollectorsIndex = EditorGUILayout.MaskField(LookModeDisplayCollectorsIndex,
                    LookModeDisplayCollectors[(CurrentPackageIndex, CurrentGroupIndex)],
                    GEStyle.PreDropDown, GP_Width_100);
            }
            else LookModeDisplayCollectorsIndex = 0;

            if (LookModeDisplayTypes[(CurrentPackageIndex, CurrentGroupIndex)].Length > 0)
            {
                LookModeDisplayTypeIndex = EditorGUILayout.MaskField(LookModeDisplayTypeIndex,
                    LookModeDisplayTypes[(CurrentPackageIndex, CurrentGroupIndex)],
                    GEStyle.PreDropDown, GP_Width_100);
            }
            else LookModeDisplayTypeIndex = 0;

            if (LookModeDisplayTags[(CurrentPackageIndex, CurrentGroupIndex)].Length > 0)
            {
                LookModeDisplayTagsIndex = EditorGUILayout.MaskField(LookModeDisplayTagsIndex,
                    LookModeDisplayTags[(CurrentPackageIndex, CurrentGroupIndex)],
                    GEStyle.PreDropDown, GP_Width_100);
            }
            else LookModeDisplayTagsIndex = 0;

            if (GUI.changed)
            {
                if (TempTable.GetOrDefault<int>(nameof(CurrentPackageIndex)) != CurrentPackageIndex ||
                    TempTable.GetOrDefault<int>(nameof(CurrentGroupIndex)) != CurrentGroupIndex)
                {
                    CurrentPageValues.Clear();
                    CurrentPageValues.Add(LookModeData[(CurrentPackageIndex, CurrentGroupIndex)]);
                    CurrentPageValues.PageIndex = 0;
                    LookModeCollectorsALLSize = CurrentPageValues.Sum(data => data.Size);
                    TempTable[nameof(CurrentPackageIndex)] = CurrentPackageIndex;
                    TempTable[nameof(CurrentGroupIndex)] = CurrentGroupIndex;
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
                    CurrentPageValues.Add(LookModeData[(CurrentPackageIndex, CurrentGroupIndex)]
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

            if (GUILayout.Button(GC_REFRESH, GEStyle.TEtoolbarbutton, GP_Width_25))
            {
                LookModeCurrentSelectAsset = null;
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

            if (Data.Packages[CurrentPackageIndex].Groups.Length == 0)
            {
                GELayout.HelpBox("当前无组资源数据");
                return;
            }

            if (Data.Packages[CurrentPackageIndex].Groups[CurrentGroupIndex].Collectors.Length == 0)
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
            foreach (var data in CurrentPageValues.CurrentPageValues)
            {
                LookModeCollectorsPageSize += data.Size;
                OnDrawLookDataItem(headerRect, data);
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
                    if (GELayout.Button(GC_LookMode_Object_Select, GEStyle.IconButton))
                    {
                        EditorUtility.RevealInFinder(LookModeCurrentSelectAssetDataInfo.AssetPath);
                    }
                }

                using (new EditorGUILayout.HorizontalScope(GEStyle.ProjectBrowserHeaderBgMiddle))
                {
                    EditorGUILayout.PrefixLabel(GC_LookMode_Detail_Asset);
                    EditorGUILayout.ObjectField(LookModeCurrentSelectAsset,
                        LookModeCurrentSelectAsset.GetType(), false);
                }

                using (new EditorGUILayout.HorizontalScope(GEStyle.ProjectBrowserHeaderBgMiddle))
                {
                    EditorGUILayout.PrefixLabel(GC_LookMode_Detail_GUID);
                    EditorGUILayout.LabelField(LookModeCurrentSelectAssetDataInfo.GUID);
                }

                using (new EditorGUILayout.HorizontalScope(GEStyle.ProjectBrowserHeaderBgMiddle))
                {
                    EditorGUILayout.PrefixLabel(GC_LookMode_Detail_Type);
                    EditorGUILayout.LabelField(AssetDatabase
                        .GetMainAssetTypeAtPath(LookModeCurrentSelectAssetDataInfo.AssetPath)?.FullName);
                }

                using (new EditorGUILayout.HorizontalScope(GEStyle.ProjectBrowserHeaderBgMiddle))
                {
                    EditorGUILayout.PrefixLabel(GC_LookMode_Detail_Path);
                    EditorGUILayout.LabelField(LookModeCurrentSelectAssetDataInfo.AssetPath);
                }

                using (new EditorGUILayout.HorizontalScope(GEStyle.ProjectBrowserHeaderBgMiddle))
                {
                    EditorGUILayout.PrefixLabel(GC_LookMode_Detail_Size);
                    EditorGUILayout.LabelField(LookModeCurrentSelectAssetDataInfo.SizeStr);
                }

                using (new EditorGUILayout.HorizontalScope(GEStyle.ProjectBrowserHeaderBgMiddle))
                {
                    EditorGUILayout.PrefixLabel(GC_LookMode_Detail_LastWriteTime);
                    EditorGUILayout.LabelField(
                        LookModeCurrentSelectAssetDataInfo.LastWriteTime.ToString("yyyy-MM-dd hh:mm:ss"));
                }

                using (new EditorGUILayout.HorizontalScope(GEStyle.ProjectBrowserHeaderBgMiddle))
                {
                    EditorGUILayout.PrefixLabel(GC_LookMode_Detail_IsSubAsset);
                    EditorGUILayout.LabelField($"{AssetDatabase.IsSubAsset(LookModeCurrentSelectAsset)}");
                }

                using (new EditorGUILayout.HorizontalScope(GEStyle.ProjectBrowserHeaderBgMiddle))
                {
                    EditorGUILayout.PrefixLabel(GC_LookMode_Detail_Tags);
                    EditorGUILayout.LabelField(LookModeCurrentSelectAssetDataInfo.Tags);
                }

                using (new EditorGUILayout.VerticalScope(GEStyle.ProjectBrowserHeaderBgMiddle))
                {
                    EditorGUILayout.LabelField($"Dependencies({Dependencies.Count})", GEStyle.HeaderLabel);
                }

                using (new EditorGUILayout.VerticalScope(GEStyle.Badge))
                {
                    // EditorGUILayout.DelayedTextField("", GEStyle.SearchTextField);
                    LookModeShowAssetDetailScroll = GELayout.BeginScrollView(LookModeShowAssetDetailScroll);
                    foreach (var dependency in Dependencies)
                    {
                        using (new EditorGUILayout.HorizontalScope(GEStyle.toolbarbuttonLeft))
                        {
                            EditorGUILayout.LabelField(dependency.Value.name, GUILayout.ExpandWidth(true));
                            EditorGUILayout.ObjectField(dependency.Value, dependency.Value.GetType(), false,
                                GP_Width_150);
                            if (GUILayout.Button(GC_LookMode_Object_Select, GEStyle.IconButton, GTOption.Width(16)))
                            {
                                EditorUtility.RevealInFinder(dependency.Key);
                                Selection.activeObject = dependency.Value;
                            }
                        }
                    }

                    GELayout.EndScrollView();
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
            if (LookModeDisplayCollectorsIndex < 1) filter++;
            else
            {
                var status = 1L;
                foreach (var display in LookModeDisplayCollectors[(CurrentPackageIndex, CurrentGroupIndex)])
                {
                    if ((LookModeDisplayCollectorsIndex & status) == status &&
                        display == data.CollectPath.Replace('/', '\\'))
                    {
                        filter++;
                        break;
                    }

                    status *= 2;
                }
            }

            if (LookModeDisplayTypeIndex < 1) filter++;
            else
            {
                var objectType = AssetDatabase.GetMainAssetTypeAtPath(data.AssetPath)?.FullName;
                if (string.IsNullOrEmpty(objectType)) objectType = "Unknown";
                var status = 1L;
                foreach (var display in LookModeDisplayTypes[(CurrentPackageIndex, CurrentGroupIndex)])
                {
                    if ((LookModeDisplayTypeIndex & status) == status && objectType == display)
                    {
                        filter++;
                        break;
                    }

                    status *= 2;
                }
            }

            if (LookModeDisplayTagsIndex < 1) filter++;
            else
            {
                var status = 1L;
                foreach (var display in LookModeDisplayTags[(CurrentPackageIndex, CurrentGroupIndex)])
                {
                    if ((LookModeDisplayTagsIndex & status) == status &&
                        data.Tags.Split(';').Contains(display))
                    {
                        filter++;
                        break;
                    }

                    status *= 2;
                }
            }


            if (string.IsNullOrEmpty(SearchText)) filter++;
            else
            {
                if (data.AssetPath.Contains(SearchText) || data.Address.Contains(SearchText))
                {
                    filter++;
                }
                else if (SearchText.Contains(data.AssetPath) || SearchText.Contains(data.Address))
                {
                    filter++;
                }
            }

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

            if (LookModeSortEnableAssetName)
            {
                var temp = new Rect(rect1);
                temp.x -= 3;
                temp.y += 2;
                temp.width = 16;
                GUI.Box(temp, GC_LookMode_Data_Sort);
            }
            else if (LookModeSortEnableSize)
            {
                var temp = new Rect(rect2);
                temp.x -= 3;
                temp.y += 2;
                temp.width = 16;
                GUI.Box(temp, GC_LookMode_Data_Sort);
            }
            else if (LookModeSortEnableLastWrite)
            {
                var temp = new Rect(rect3);
                temp.x -= 3;
                temp.y += 2;
                temp.width = 16;
                GUI.Box(temp, GC_LookMode_Data_Sort);
            }

            if (GUI.Button(
                    rect1,
                    new GUIContent(
                        $"    Asset[{LookModeCollectorsPageSize.ToConverseStringFileSize()}\\{LookModeCollectorsALLSize.ToConverseStringFileSize()}]"),
                    GEStyle.HeaderLabel))
            {
                LookModeSortEnableLastWrite = false;
                LookModeSortEnableAssetNameToMin = !LookModeSortEnableAssetNameToMin;
                LookModeDataPageValueSort(ESort.AssetName, LookModeSortEnableAssetNameToMin);
            }

            if (GUI.Button(
                    rect2,
                    new GUIContent("    Size"),
                    GEStyle.HeaderLabel))
            {
                LookModeSortEnableSize = true;
                LookModeSortEnableSizeToMin = !LookModeSortEnableSizeToMin;
                LookModeDataPageValueSort(ESort.FileSize, LookModeSortEnableSizeToMin);
            }

            if (GUI.Button(
                    rect3,
                    new GUIContent("    Ago"),
                    GEStyle.HeaderLabel))
            {
                LookModeSortEnableLastWrite = true;
                LookModeSortEnableLastWriteToMin = !LookModeSortEnableLastWriteToMin;
                LookModeDataPageValueSort(ESort.LastWrite, LookModeSortEnableLastWriteToMin);
            }
        }

        /// <summary>
        /// 绘制资源展示面板
        /// </summary>
        private void OnDrawLookDataItem(Rect rect, AssetDataInfo data)
        {
            if (GUI.Button(rect, "", GEStyle.ProjectBrowserHeaderBgMiddle))
            {
                LookModeCurrentSelectAsset = AssetDatabase.LoadAssetAtPath<Object>(data.AssetPath);
                LookModeCurrentSelectAssetDataInfo = data;
                Selection.activeObject = LookModeCurrentSelectAsset;
                Dependencies.Clear();

                foreach (var dependency in
                         AssetDatabase.GetDependencies(LookModeCurrentSelectAssetDataInfo.AssetPath))
                {
                    Dependencies[dependency] = AssetDatabase.LoadAssetAtPath<Object>(dependency);
                }

                ViewDetailList.width = CurrentWidth / 2;
                GUI.FocusControl(null);
            }

            OnDrawShading(rect);

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


            EditorGUIUtility.SetIconSize(new Vector2(rect.height - 4, rect.height - 4));
            var content = EditorGUIUtility.ObjectContent(AssetDatabase.LoadMainAssetAtPath(data.AssetPath), null);
            content.text = data.Address;
            EditorGUI.LabelField(rect1, content, EditorStyles.label);
            EditorGUI.LabelField(rect2, data.SizeStr, EditorStyles.label);
            EditorGUI.LabelField(rect3, data.GetLatestTime(), EditorStyles.label);
        }

        /// <summary>
        /// 更新资源查询模式组
        /// </summary>
        private void UpdateDataLookModeGroup(int packageIndex)
        {
            var page = Data.Packages[packageIndex].Groups.Length > 15;
            LookModeDisplayGroups[LookModeDisplayPackages[packageIndex]] =
                new string[Data.Packages[packageIndex].Groups.Length];
            for (var j = 0; j < Data.Packages[packageIndex].Groups.Length; j++)
            {
                var GroupName = Data.Packages[packageIndex].Groups[j].Name;
                LookModeDisplayGroups[LookModeDisplayPackages[packageIndex]][j] = page
                    ? string.Concat(char.ToUpper(GroupName[0]), '/', GroupName)
                    : Data.Packages[packageIndex].Groups[j].Name;
            }
        }

        /// <summary>
        /// 更新资源查询模式收集器
        /// </summary>
        private void UpdateDataLookModeCollector(int packageIndex, int groupIndex)
        {
            var i = packageIndex;
            var j = groupIndex;
            if (Data.Packages.Length <= i || i < 0) return;
            if (Data.Packages[i].Groups.Length <= j || j < 0) return;

            var CatchList = new Dictionary<string, string>();
            var RepeatList = new Dictionary<string, string>();

            if (!LookModeDisplayGroups.ContainsKey(LookModeDisplayPackages[i])) UpdateDataLookModeGroup(i);

            LookModeDisplayTags[(i, j)] = Data.Packages[i].Groups[j].GetTags();
            LookModeDisplayCollectors[(i, j)] = new string[Data.Packages[i].Groups[j].Collectors.Length];
            LookModeData[(i, j)] = new List<AssetDataInfo>();
            var ListType = new Dictionary<string, byte>();
            for (var k = 0; k < Data.Packages[i].Groups[j].Collectors.Length; k++)
            {
                Data.Packages[i].Groups[j].Collectors[k]
                    .CollectAsset(Data.Packages[i].Name, Data.Packages[i].Groups[j].Name);
                LookModeDisplayCollectors[(i, j)][k] =
                    Data.Packages[i].Groups[j].Collectors[k].CollectPath.Replace('/', '\\').Trim('\\');

                foreach (var assetDataInfo in Data.Packages[i].Groups[j].Collectors[k].AssetDataInfos)
                {
                    if (CatchList.ContainsKey(assetDataInfo.Key))
                    {
                        if (RepeatList.ContainsKey(assetDataInfo.Key))
                        {
                            RepeatList[assetDataInfo.Key] =
                                $"{RepeatList[assetDataInfo.Key]}\n{assetDataInfo.Value.Address}({assetDataInfo.Value.AssetPath})";
                        }
                        else
                            RepeatList[assetDataInfo.Key] =
                                $"{assetDataInfo.Value.Address}({assetDataInfo.Value.AssetPath})";
                    }
                    else CatchList.Add(assetDataInfo.Key, assetDataInfo.Value.Address);

                    ListType[assetDataInfo.Value.Type] = 1;
                    LookModeData[(i, j)].Add(assetDataInfo.Value);
                }
            }

            LookModeDisplayTypes[(i, j)] = ListType.Keys.ToArray();

            if (RepeatList.Count > 0)
            {
                var builder = new StringBuilder();
                builder.Append("以下资源重复：\n");
                foreach (var s in RepeatList)
                {
                    builder.Append(s);
                    builder.Append('\n');
                }

                GELayout.HelpBox(builder.ToString());
            }
        }

        private void UpdateDataLookMode()
        {
            GUI.FocusControl(null);
            if (Data.Packages.Length == 0)
            {
                CurrentPackageIndex = CurrentGroupIndex = 0;
                return;
            }

            if (Data.Packages.Length <= CurrentPackageIndex || CurrentPackageIndex < 0)
                CurrentPackageIndex = 0;

            if (Data.Packages[CurrentPackageIndex].Groups.Length == 0)
            {
                CurrentGroupIndex = 0;
                return;
            }

            if (Data.Packages[CurrentPackageIndex].Groups.Length <= CurrentGroupIndex || CurrentGroupIndex < 0)
                CurrentGroupIndex = 0;

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

            UpdateDataLookModeCollector(CurrentPackageIndex, CurrentGroupIndex);
            CurrentPageValues.Clear();
            CurrentPageValues.Add(LookModeData[(CurrentPackageIndex, CurrentGroupIndex)]
                .Where(data => !LookModeDataFilter(data)));
            CurrentPageValues.PageIndex = 0;
            LookModeCollectorsALLSize = CurrentPageValues.Sum(data => data.Size);
            LookModeCollectorsPageSize = 0;
            LookModeDataPageValueSort(ESort.LastWrite, true);
        }
    }
}