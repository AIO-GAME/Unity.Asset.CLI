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
        public enum ESort
        {
            [InspectorName("大小")] FileSize,
            [InspectorName("最后修改时间")] LastWrite,
            [InspectorName("名称")] AssetName,
            [InspectorName("资源类型")] ObjectType,
        }

        /// <summary>
        /// 查看模式 资源排序
        /// </summary>
        private ESort LookModeSort = ESort.LastWrite;

        /// <summary>
        /// 查询模式 数据
        /// </summary>
        private Dictionary<(int, int), List<AssetDataInfo>> LookModeData;

        /// <summary>
        /// 包列表
        /// </summary>
        private string[] LookModeDisplayPackages;

        /// <summary>
        /// 组列表
        /// </summary>
        private Dictionary<string, string[]> LookModeDisplayGroups;

        /// <summary>
        /// 收集器列表
        /// </summary>
        private Dictionary<(int, int), string[]> LookModeDisplayCollectors;

        /// <summary>
        /// 收集器类型列表
        /// </summary>
        private Dictionary<(int, int), string[]> LookModeDisplayCollectorTypes;

        /// <summary>
        /// 搜索文本
        /// </summary>
        private string SearchText = string.Empty;

        /// <summary>
        /// 当前选择包类型索引
        /// </summary>
        private int LookModeDisplayCollectorsTypeIndex = -1;

        /// <summary>
        /// 当前选择收集器索引
        /// </summary>
        private int LookModeDisplayCollectorsIndex = -1;

        /// <summary>
        /// 收集器全部资源大小
        /// </summary>
        private long LookModeCollectorsALLSize = 0L;

        /// <summary>
        /// 是否显示资源详情
        /// </summary>
        private bool LookModeShowAssetDetail => !string.IsNullOrEmpty(LookModeCurrentSelectAssetDataInfo.GUID) &&
                                                LookModeCurrentSelectAsset != null;

        /// <summary>
        /// 拖拽区域
        /// </summary>
        private Rect OnDrawLookDataSpaceRect;

        /// <summary>
        /// 是否在查询模式拖拽分页大小
        /// </summary>
        private bool OnDrawLookDataSpaceRectDrag;

        private float LookModeShowAssetListWidth = LookModeShowAssetDetailMinWidth;

        /// <summary>
        /// 资源详情界面最小宽度
        /// </summary>
        private const int LookModeShowAssetDetailMinWidth = 400;

        /// <summary>
        /// 资源详情界面最小宽度
        /// </summary>
        private const int LookModeShowAssetListMinWidth = 200;

        private void OnDrawHeaderLook()
        {
            if (Data.Packages.Length == 0)
            {
                EditorGUILayout.Separator();
                return;
            }

            CurrentPackageIndex = EditorGUILayout.Popup(CurrentPackageIndex, LookModeDisplayPackages,
                GEStyle.PreDropDown,
                GTOption.Width(100));

            if (Data.Packages.Length <= CurrentPackageIndex || CurrentPackageIndex < 0) CurrentPackageIndex = 0;
            if (Data.Packages[CurrentPackageIndex].Groups.Length == 0)
            {
                EditorGUILayout.Separator();
                return;
            }

            if (CurrentGroupIndex >= LookModeDisplayGroups[LookModeDisplayPackages[CurrentPackageIndex]].Length)
                CurrentGroupIndex = LookModeDisplayGroups[LookModeDisplayPackages[CurrentPackageIndex]].Length - 1;
            CurrentGroupIndex = EditorGUILayout.Popup(CurrentGroupIndex,
                LookModeDisplayGroups[LookModeDisplayPackages[CurrentPackageIndex]], GEStyle.PreDropDown,
                GTOption.Width(100));

            if (Data.Packages[CurrentPackageIndex].Groups[CurrentGroupIndex].Collectors.Length == 0)
            {
                EditorGUILayout.Separator();
                return;
            }

            EditorGUILayout.Separator();

            SearchText = GELayout.FieldDelayed(SearchText, GEStyle.SearchTextField);
            if (GELayout.Button(GC_DEL, GEStyle.TEtoolbarbutton, 24))
            {
                GUI.FocusControl(null);
                SearchText = string.Empty;
                return;
            }

            LookModeDisplayCollectorsIndex = GELayout.Mask(LookModeDisplayCollectorsIndex,
                LookModeDisplayCollectors[(CurrentPackageIndex, CurrentGroupIndex)], GEStyle.PreDropDown,
                GTOption.Width(100));

            if (LookModeDisplayCollectorTypes[(CurrentPackageIndex, CurrentGroupIndex)].Length > 0)
            {
                LookModeDisplayCollectorsTypeIndex = GELayout.Mask(LookModeDisplayCollectorsTypeIndex,
                    LookModeDisplayCollectorTypes[(CurrentPackageIndex, CurrentGroupIndex)],
                    GEStyle.PreDropDown, GTOption.Width(100));
            }

            if (GELayout.Button(GC_REFRESH, GEStyle.TEtoolbarbutton, 24))
            {
                LookModeCurrentSelectAsset = null;
                SearchText = string.Empty;
                LookModeDisplayCollectorsTypeIndex = LookModeDisplayCollectorsIndex = 0;
                UpdateDataLook();
            }
        }

        /// <summary>
        /// 用户当前选择的资源实体
        /// </summary>
        private Object LookModeCurrentSelectAsset;

        /// <summary>
        /// 选择的资源实体配置
        /// </summary>
        private AssetDataInfo LookModeCurrentSelectAssetDataInfo;

        public Dictionary<string, Object> Dependencies = new Dictionary<string, Object>();

        private void LookModeSortData(ESort sort, bool minToMax)
        {
            if (!LookModeData.ContainsKey((CurrentPackageIndex, CurrentGroupIndex))) return;
            LookModeSort = sort;
            var list = LookModeData[(CurrentPackageIndex, CurrentGroupIndex)];
            switch (LookModeSort)
            {
                case ESort.FileSize:
                    list.Sort((data1, data2) =>
                    {
                        if (data1.Size < data2.Size) return minToMax ? 1 : -1;
                        if (data1.Size > data2.Size) return minToMax ? -1 : 1;
                        return 0;
                    });
                    break;
                case ESort.LastWrite:
                    LookModeSortEnableAssetName = false;
                    LookModeSortEnableLastWrite = true;
                    list.Sort((data1, data2) =>
                    {
                        if (data1.LastWriteTime < data2.LastWriteTime) return minToMax ? 1 : -1;
                        if (data1.LastWriteTime > data2.LastWriteTime) return minToMax ? -1 : 1;
                        return 0;
                    });
                    break;
                case ESort.AssetName:
                    LookModeSortEnableAssetName = true;
                    LookModeSortEnableLastWrite = false;
                    list.Sort((data1, data2) => // 实现文件名 排序 
                    {
                        var name1 = data1.Address;
                        var name2 = data2.Address;
                        if (name1 == name2) return 0;
                        return minToMax
                            ? string.Compare(name1, name2, StringComparison.Ordinal)
                            : string.Compare(name2, name1, StringComparison.Ordinal);
                    });
                    break;
                default:
                    return;
            }
        }

        partial void OnDrawLook()
        {
            if (Data.Packages.Length == 0)
            {
                GELayout.HelpBox("请添包数据");
                return;
            }

            if (Data.Packages.Length <= CurrentPackageIndex) CurrentPackageIndex = 0;
            if (Data.Packages[CurrentPackageIndex].Groups.Length == 0)
            {
                GELayout.HelpBox("请添组数据");
                return;
            }

            if (Data.Packages[CurrentPackageIndex].Groups.Length <= CurrentGroupIndex) CurrentGroupIndex = 0;
            if (Data.Packages[CurrentPackageIndex].Groups[CurrentGroupIndex].Collectors.Length == 0)
            {
                GELayout.HelpBox("请添加收集器数据");
                return;
            }

            LookModeShowAssetListRect = new Rect(0, DrawHeaderHeight,
                LookModeShowAssetDetail ? LookModeShowAssetListWidth : CurrentWidth,
                CurrentHeight - DrawHeaderHeight);
            GUILayout.BeginArea(LookModeShowAssetListRect, GEStyle.Badge);

            var headerRect = new Rect(0, 0, LookModeShowAssetListRect.width, 20);
            OnDrawLookDataHeader(headerRect);
            LookModeCollectorsALLSize = 0;
            LookModeShowAssetListRect.y = 20;
            LookModeShowAssetListRect.height -= 20;
            LookModeShowAssetListView.width = LookModeShowAssetListRect.width - 15;
            var datas = LookModeData[(CurrentPackageIndex, CurrentGroupIndex)]
                .Where(data => !Filter(data)).ToArray();
            LookModeShowAssetListView.height = 20 * datas.Length;
            OnDrawLookDataScroll = GUI.BeginScrollView(
                LookModeShowAssetListRect,
                OnDrawLookDataScroll,
                LookModeShowAssetListView
            );

            headerRect.x = 1;
            headerRect.width -= 2;
            foreach (var data in datas)
            {
                LookModeCollectorsALLSize += data.Size;
                OnDrawLookDataItem(headerRect, data);
                headerRect.y += headerRect.height;
            }

            GUI.EndScrollView();
            GUILayout.EndArea();

            if (LookModeShowAssetDetail)
            {
                OnDrawLookDataSpaceRect = new Rect(LookModeShowAssetListRect.width, LookModeShowAssetListRect.y,
                    10, LookModeShowAssetListRect.height);

                LookModeShowAssetDetailRect = new Rect(
                    LookModeShowAssetListRect.width + 10,
                    LookModeShowAssetListRect.y + 3,
                    CurrentWidth - LookModeShowAssetListRect.width - 10,
                    LookModeShowAssetListRect.height + 23);
                GUILayout.BeginArea(LookModeShowAssetDetailRect, GEStyle.Badge);
                OnDrawAssetDetail();
                GUILayout.EndArea();

                EditorGUIUtility.AddCursorRect(OnDrawLookDataSpaceRect, MouseCursor.ResizeHorizontal);
            }
        }

        public override void EventMouseDown(in Event eventData)
        {
            if (OnDrawLookDataSpaceRect.Contains(eventData.mousePosition))
            {
                OnDrawLookDataSpaceRectDrag = true;
            }
        }

        public override void EventMouseDrag(in Event eventData)
        {
            if (OnDrawLookDataSpaceRectDrag)
            {
                var temp = LookModeShowAssetListWidth + eventData.delta.x;
                if (temp < LookModeShowAssetListMinWidth)
                    LookModeShowAssetListWidth = LookModeShowAssetListMinWidth;
                else if (temp > CurrentWidth - LookModeShowAssetDetailMinWidth)
                    LookModeShowAssetListWidth = CurrentWidth - LookModeShowAssetDetailMinWidth;
                else LookModeShowAssetListWidth = temp;
            }
        }

        public override void EventMouseUp(in Event eventData)
        {
            OnDrawLookDataSpaceRectDrag = false;
        }

        private void OnDrawAssetDetail()
        {
            using (GELayout.Vertical(GEStyle.GridList))
            {
                using (GELayout.VHorizontal(GTOption.Height(30)))
                {
                    EditorGUILayout.LabelField(LookModeCurrentSelectAssetDataInfo.Address, GEStyle.AMMixerHeader,
                        GTOption.Height(30));
                    if (GELayout.Button(EditorGUIUtility.IconContent("d_scenepicking_pickable_hover"),
                            GEStyle.IconButton))
                    {
                        EditorUtility.RevealInFinder(LookModeCurrentSelectAssetDataInfo.AssetPath);
                    }
                }

                using (GELayout.VHorizontal(GEStyle.ProjectBrowserHeaderBgMiddle))
                {
                    EditorGUILayout.PrefixLabel(new GUIContent("Asset", "资源实例"));
                    EditorGUILayout.ObjectField(LookModeCurrentSelectAsset,
                        LookModeCurrentSelectAsset.GetType(), false);
                }

                using (GELayout.VHorizontal(GEStyle.ProjectBrowserHeaderBgMiddle))
                {
                    EditorGUILayout.PrefixLabel(new GUIContent("GUID", "GUID"));
                    EditorGUILayout.LabelField(LookModeCurrentSelectAssetDataInfo.GUID);
                }

                using (GELayout.VHorizontal(GEStyle.ProjectBrowserHeaderBgMiddle))
                {
                    EditorGUILayout.PrefixLabel(new GUIContent("Type", "资源类型"));
                    EditorGUILayout.LabelField(AssetDatabase
                        .GetMainAssetTypeAtPath(LookModeCurrentSelectAssetDataInfo.AssetPath)?.FullName);
                }

                using (GELayout.VHorizontal(GEStyle.ProjectBrowserHeaderBgMiddle))
                {
                    EditorGUILayout.PrefixLabel(new GUIContent("Path", "资源文件路径"));
                    EditorGUILayout.LabelField(LookModeCurrentSelectAssetDataInfo.AssetPath);
                }

                using (GELayout.VHorizontal(GEStyle.ProjectBrowserHeaderBgMiddle))
                {
                    EditorGUILayout.PrefixLabel(new GUIContent("Size", "文件大小"));
                    EditorGUILayout.LabelField(LookModeCurrentSelectAssetDataInfo.Size.ToConverseStringFileSize());
                }

                using (GELayout.VHorizontal(GEStyle.ProjectBrowserHeaderBgMiddle))
                {
                    EditorGUILayout.PrefixLabel(new GUIContent("Last Write Time", "最后写入时间"));
                    EditorGUILayout.LabelField(
                        LookModeCurrentSelectAssetDataInfo.LastWriteTime.ToString("yyyy-MM-dd hh:mm:ss"));
                }

                using (GELayout.VHorizontal(GEStyle.ProjectBrowserHeaderBgMiddle))
                {
                    EditorGUILayout.PrefixLabel(new GUIContent("IsSubAsset", "资源是否构成了其他资源的一部分？"));
                    EditorGUILayout.LabelField($"{AssetDatabase.IsSubAsset(LookModeCurrentSelectAsset)}");
                }

                using (GELayout.Vertical(GEStyle.ProjectBrowserHeaderBgMiddle))
                {
                    EditorGUILayout.LabelField($"Dependencies({Dependencies.Count})", GEStyle.HeaderLabel);
                }

                using (GELayout.Vertical(GEStyle.Badge))
                {
                    // EditorGUILayout.DelayedTextField("", GEStyle.SearchTextField);
                    LookModeShowAssetDetailScroll = GELayout.BeginScrollView(LookModeShowAssetDetailScroll);
                    foreach (var dependency in Dependencies)
                    {
                        using (GELayout.VHorizontal(GEStyle.toolbarbuttonLeft))
                        {
                            EditorGUILayout.LabelField(dependency.Value.name, GTOption.Width(true));
                            EditorGUILayout.ObjectField(dependency.Value, dependency.Value.GetType(), false,
                                GTOption.Width(150));
                            if (GUILayout.Button(EditorGUIUtility.IconContent("d_scenepicking_pickable_hover"),
                                    GEStyle.IconButton, GTOption.Width(16)))
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

        private Rect LookModeShowAssetListRect;
        private Rect LookModeShowAssetListView;
        private Rect LookModeShowAssetDetailRect;
        private Vector2 LookModeShowAssetDetailScroll;

        private bool Filter(AssetDataInfo data)
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

            if (LookModeDisplayCollectorsTypeIndex < 1) filter++;
            else
            {
                var objectType = AssetDatabase.GetMainAssetTypeAtPath(data.AssetPath).FullName;
                var status = 1L;
                foreach (var display in LookModeDisplayCollectorTypes[(CurrentPackageIndex, CurrentGroupIndex)])
                {
                    if ((LookModeDisplayCollectorsTypeIndex & status) == status && objectType != display)
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

            return filter != 3;
        }

        private static readonly Color ROW_SHADING_COLOR = new Color(0f, 0f, 0f, 0.2f);

        private void OnDrawLookDataHeader(Rect rect)
        {
            var rect1 = new Rect(rect)
            {
                x = 0,
                width = rect.width - 80,
            };

            var rect2 = new Rect(rect)
            {
                x = rect1.x + rect1.width,
                width = 80
            };
            GUI.Box(rect1, "", GEStyle.TEtoolbarbutton);
            GUI.Box(rect2, "", GEStyle.TEtoolbarbutton);

            if (LookModeSortEnableAssetName)
            {
                var temp = new Rect(rect1);
                temp.x -= 3;
                temp.y += 2;
                temp.width = 16;
                GUI.Box(temp, EditorGUIUtility.IconContent("d_pulldown@2x"));
            }

            if (LookModeSortEnableLastWrite)
            {
                var temp = new Rect(rect2);
                temp.x -= 3;
                temp.y += 2;
                temp.width = 16;
                GUI.Box(temp, EditorGUIUtility.IconContent("d_pulldown@2x"));
            }

            if (GUI.Button(
                    rect1,
                    new GUIContent($"    Asset[{LookModeCollectorsALLSize.ToConverseStringFileSize()}]"),
                    GEStyle.HeaderLabel))
            {
                LookModeSortEnableLastWrite = false;
                LookModeSortEnableAssetNameToMin = !LookModeSortEnableAssetNameToMin;
                LookModeSortData(ESort.AssetName, LookModeSortEnableAssetNameToMin);
            }

            if (GUI.Button(
                    rect2,
                    new GUIContent("    Ago"),
                    GEStyle.HeaderLabel))
            {
                LookModeSortEnableLastWrite = true;
                LookModeSortEnableLastWriteToMin = !LookModeSortEnableLastWriteToMin;
                LookModeSortData(ESort.LastWrite, LookModeSortEnableLastWriteToMin);
            }
        }

        private bool LookModeSortEnableAssetName;
        private bool LookModeSortEnableAssetNameToMin;

        private bool LookModeSortEnableLastWrite;
        private bool LookModeSortEnableLastWriteToMin;

        private static void OnDrawShading(Rect rect)
        {
            if (Mathf.FloorToInt((rect.y - 4f) / rect.height % 2f) != 0) return;
            var rect2 = new Rect(rect);
            rect2.width += rect.x + rect.height;
            rect2.height += 1f;
            rect2.x = 0f;
            EditorGUI.DrawRect(rect2, ROW_SHADING_COLOR);
            rect2.height = 1f;
            EditorGUI.DrawRect(rect2, ROW_SHADING_COLOR);
            rect2.y += rect.height;
            EditorGUI.DrawRect(rect2, ROW_SHADING_COLOR);
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

                GUI.FocusControl(null);
            }

            OnDrawShading(rect);

            var rect1 = new Rect(rect)
            {
                x = 10,
                width = rect.width - 80
            };

            var rect2 = new Rect(rect)
            {
                x = rect1.x + rect1.width,
                width = 80
            };


            EditorGUIUtility.SetIconSize(new Vector2(rect.height - 4, rect.height - 4));
            var content = EditorGUIUtility.ObjectContent(AssetDatabase.LoadMainAssetAtPath(data.AssetPath), null);
            content.text = data.Address;
            EditorGUI.LabelField(rect1, content, EditorStyles.label);
            EditorGUI.LabelField(rect2, data.GetLatestTime(), EditorStyles.label);
        }

        private void UpdateDataLook()
        {
            GUI.FocusControl(null);
            LookModeDisplayPackages = new string[Data.Packages.Length];

            if (LookModeDisplayCollectors is null) LookModeDisplayCollectors = new Dictionary<(int, int), string[]>();
            else LookModeDisplayCollectors.Clear();

            if (LookModeDisplayCollectorTypes is null)
                LookModeDisplayCollectorTypes = new Dictionary<(int, int), string[]>();
            else LookModeDisplayCollectorTypes.Clear();

            if (LookModeDisplayGroups is null) LookModeDisplayGroups = new Dictionary<string, string[]>();
            else LookModeDisplayGroups.Clear();

            if (LookModeData is null) LookModeData = new Dictionary<(int, int), List<AssetDataInfo>>();
            else LookModeData.Clear();

            var CatchList = new Dictionary<string, string>();
            var RepeatList = new Dictionary<string, string>();

            for (var i = 0; i < Data.Packages.Length; i++)
            {
                var page = Data.Packages[i].Groups.Length > 15;
                LookModeDisplayPackages[i] = Data.Packages[i].Name;
                LookModeDisplayGroups[LookModeDisplayPackages[i]] = new string[Data.Packages[i].Groups.Length];

                for (var j = 0; j < Data.Packages[i].Groups.Length; j++)
                {
                    var GroupName = Data.Packages[i].Groups[j].Name;
                    LookModeDisplayGroups[LookModeDisplayPackages[i]][j] = page
                        ? string.Concat(char.ToUpper(GroupName[0]), '/', GroupName)
                        : Data.Packages[i].Groups[j].Name;

                    LookModeDisplayCollectors[(i, j)] = new string[Data.Packages[i].Groups[j].Collectors.Length];
                    LookModeData[(i, j)] = new List<AssetDataInfo>();
                    var ListType = new List<string>();
                    for (var k = 0; k < Data.Packages[i].Groups[j].Collectors.Length; k++)
                    {
                        Data.Packages[i].Groups[j].Collectors[k]
                            .CollectAsset(Data.Packages[i].Name, Data.Packages[i].Groups[j].Name);
                        LookModeDisplayCollectors[(i, j)][k] =
                            Data.Packages[i].Groups[j].Collectors[k].CollectPath.Replace('/', '\\');

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

                            var type = AssetDatabase.GetMainAssetTypeAtPath(assetDataInfo.Value.AssetPath);
                            if (type is null)
                            {
                                if (!ListType.Contains("Unknown")) ListType.Add("Unknown");
                            }
                            else if (!ListType.Contains(type.FullName)) ListType.Add(type.FullName);

                            LookModeData[(i, j)].Add(assetDataInfo.Value);
                        }
                    }

                    LookModeDisplayCollectorTypes[(i, j)] = ListType.ToArray();
                }
            }

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

            LookModeSortData(ESort.LastWrite, true);
        }
    }
}