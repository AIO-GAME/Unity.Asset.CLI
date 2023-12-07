/*|============|*|
|*|Author:     |*| Star fire
|*|Date:       |*| 2023-12-01
|*|E-Mail:     |*| xinansky99@foxmail.com
|*|============|*/

using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace AIO.UEditor
{
    public partial class AssetCollectWindow
    {
        private string[] LookModdePackages;
        private Dictionary<string, string[]> LookModdeGroups;
        private Dictionary<(int, int), string[]> LookModdeCollectors;
        private Dictionary<(int, int), string[]> LookModdeCollectorTypes;
        private string SearchText = string.Empty;
        private Dictionary<(int, int), List<AssetDataInfo>> LookData;

        private int LookModdeCollectorsTypeIndex = -1;
        private int LookModdeCollectorsIndex = -1;

        /// <summary>
        /// 收集器全部资源大小
        /// </summary>
        private long LookModdeCollectorsALLSize = 0L;

        private void OnDrawHeaderLook()
        {
            if (Data.Packages.Length == 0)
            {
                EditorGUILayout.Separator();
                return;
            }

            CurrentPackageIndex = EditorGUILayout.Popup(CurrentPackageIndex, LookModdePackages, GEStyle.PreDropDown,
                GTOption.Width(100));

            if (Data.Packages[CurrentPackageIndex].Groups.Length == 0)
            {
                EditorGUILayout.Separator();
                return;
            }

            if (CurrentGroupIndex >= LookModdeGroups[LookModdePackages[CurrentPackageIndex]].Length)
                CurrentGroupIndex = LookModdeGroups[LookModdePackages[CurrentPackageIndex]].Length - 1;
            CurrentGroupIndex = EditorGUILayout.Popup(CurrentGroupIndex,
                LookModdeGroups[LookModdePackages[CurrentPackageIndex]], GEStyle.PreDropDown, GTOption.Width(100));

            if (Data.Packages[CurrentPackageIndex].Groups[CurrentGroupIndex].Collectors.Length == 0)
            {
                EditorGUILayout.Separator();
                return;
            }

            EditorGUILayout.Separator();

            SearchText = GELayout.Field(SearchText, GEStyle.SearchTextField);
            if (GELayout.Button(GC_DEL, GEStyle.TEtoolbarbutton, 24))
            {
                GUI.FocusControl(null);
                SearchText = string.Empty;
                return;
            }

            LookModdeCollectorsIndex = GELayout.Mask(LookModdeCollectorsIndex,
                LookModdeCollectors[(CurrentPackageIndex, CurrentGroupIndex)], GEStyle.PreDropDown,
                GTOption.Width(100));

            if (LookModdeCollectorTypes[(CurrentPackageIndex, CurrentGroupIndex)].Length > 0)
            {
                LookModdeCollectorsTypeIndex = GELayout.Mask(LookModdeCollectorsTypeIndex,
                    LookModdeCollectorTypes[(CurrentPackageIndex, CurrentGroupIndex)],
                    GEStyle.PreDropDown, GTOption.Width(100));
            }

            if (GELayout.Button(GC_REFRESH, GEStyle.TEtoolbarbutton, 24))
            {
                SearchText = string.Empty;
                LookModdeCollectorsTypeIndex = LookModdeCollectorsIndex = 0;
                UpdateDataLook();
            }
        }

        partial void OnDrawLook()
        {
            if (Data.Packages.Length == 0)
            {
                GELayout.HelpBox("请添包数据");
                return;
            }

            if (Data.Packages.Length >= CurrentPackageIndex)
            {
                CurrentPackageIndex = 0;
            }

            if (Data.Packages[CurrentPackageIndex].Groups.Length == 0)
            {
                GELayout.HelpBox("请添组数据");
                return;
            }

            if (Data.Packages[CurrentPackageIndex].Groups[CurrentGroupIndex].Collectors.Length == 0)
            {
                GELayout.HelpBox("请添加收集器数据");
                return;
            }

            using (GELayout.Vertical(GEStyle.GridList))
            {
                OnDrawLookDataHeader();
                LookModdeCollectorsALLSize = 0;
                OnDrawLookDataScroll = GELayout.VScrollView(() =>
                {
                    foreach (var data in LookData[(CurrentPackageIndex, CurrentGroupIndex)]
                                 .Where(data => !Filter(data)))
                    {
                        LookModdeCollectorsALLSize += data.Size;
                        using (GELayout.VHorizontal())
                        {
                            OnDrawLookDataItem(data);
                        }
                    }
                }, OnDrawLookDataScroll);
            }
        }

        private bool Filter(AssetDataInfo data)
        {
            var filter = 0;
            if (LookModdeCollectorsIndex < 1) filter++;
            else
            {
                var status = 1L;
                foreach (var display in LookModdeCollectors[(CurrentPackageIndex, CurrentGroupIndex)])
                {
                    if ((LookModdeCollectorsIndex & status) == status &&
                        display == data.CollectPath.Replace('/', '\\'))
                    {
                        filter++;
                        break;
                    }

                    status *= 2;
                }
            }

            if (LookModdeCollectorsTypeIndex < 1) filter++;
            else
            {
                var objectType = AssetDatabase.GetMainAssetTypeAtPath(data.AssetPath).FullName;
                var status = 1L;
                foreach (var display in LookModdeCollectorTypes[(CurrentPackageIndex, CurrentGroupIndex)])
                {
                    if ((LookModdeCollectorsTypeIndex & status) == status && objectType != display)
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

        private static readonly Color ROW_SHADING_COLOR = new Color(0f, 0f, 0f, 0.04f);

        private void OnDrawLookDataHeader()
        {
            var rect = EditorGUILayout.GetControlRect();
            rect.width = position.width;
            rect.x = 0f;
            GUI.Box(rect, "", GEStyle.INThumbnailShadow);
            var rect3 = new Rect(rect)
            {
                x = 20,
                width = 250f,
            };

            var rect4 = new Rect(rect)
            {
                x = rect3.x + rect3.width,
                width = rect.width - (rect3.x + rect3.width + 150 + 20)
            };

            var rect5 = new Rect(rect)
            {
                x = rect4.x + rect4.width + 20,
                width = 150
            };

            EditorGUI.LabelField(rect3, new GUIContent("可寻址地址"), GEStyle.HeaderLabel);
            EditorGUI.LabelField(rect4, new GUIContent("资源文件"), GEStyle.HeaderLabel);
            EditorGUI.LabelField(rect5, new GUIContent($"大小:{LookModdeCollectorsALLSize.ToConverseStringFileSize()}"),
                GEStyle.HeaderLabel);
        }

        private static void OnDrawShading(Rect rect)
        {
            if (Mathf.FloorToInt((rect.y - 4f) / 16f % 2f) != 0) return;
            var rect2 = new Rect(rect);
            rect2.width += rect.x + 16f;
            rect2.height += 1f;
            rect2.x = 0f;
            EditorGUI.DrawRect(rect2, ROW_SHADING_COLOR);
            rect2.height = 1f;
            EditorGUI.DrawRect(rect2, ROW_SHADING_COLOR);
            rect2.y += 16f;
            EditorGUI.DrawRect(rect2, ROW_SHADING_COLOR);
        }

        private void OnDrawLookDataItem(AssetDataInfo data)
        {
            var rect = EditorGUILayout.GetControlRect();
            rect.width = position.width;
            rect.x = 0f;
            GUI.Box(rect, "", GEStyle.ProjectBrowserHeaderBgMiddle);
            OnDrawShading(rect);
            EditorGUIUtility.SetIconSize(new Vector2(rect.height, rect.height));
            var rect1 = new Rect(rect)
            {
                x = 10,
                width = 250f,
            };

            var rect2 = new Rect(rect)
            {
                x = rect1.x + rect1.width,
                width = rect.width - (rect1.x + rect1.width + 150 + 20)
            };

            var rect3 = new Rect(rect)
            {
                x = rect2.x + rect2.width + 30,
                width = 150
            };

            var content = EditorGUIUtility.ObjectContent(AssetDatabase.LoadMainAssetAtPath(data.AssetPath), null);
            content.text = data.AssetPath;

            EditorGUI.LabelField(rect1, new GUIContent(data.Address), EditorStyles.label);

            if (GUI.Button(rect2, content, GEStyle.INObjectField))
                Selection.activeObject = AssetDatabase.LoadAssetAtPath(data.AssetPath, typeof(Object));

            EditorGUI.LabelField(rect3, data.Size.ToConverseStringFileSize(), EditorStyles.label);

            GELayout.Separator();
        }

        private void UpdateDataLook()
        {
            GUI.FocusControl(null);
            LookModdePackages = new string[Data.Packages.Length];

            if (LookModdeCollectors is null) LookModdeCollectors = new Dictionary<(int, int), string[]>();
            else LookModdeCollectors.Clear();

            if (LookModdeCollectorTypes is null) LookModdeCollectorTypes = new Dictionary<(int, int), string[]>();
            else LookModdeCollectorTypes.Clear();

            if (LookModdeGroups is null) LookModdeGroups = new Dictionary<string, string[]>();
            else LookModdeGroups.Clear();

            if (LookData is null) LookData = new Dictionary<(int, int), List<AssetDataInfo>>();
            else LookData.Clear();

            var CatchList = new Dictionary<string, string>();
            var RepeatList = new Dictionary<string, string>();

            for (var i = 0; i < Data.Packages.Length; i++)
            {
                LookModdePackages[i] = Data.Packages[i].Name;
                LookModdeGroups[LookModdePackages[i]] = new string[Data.Packages[i].Groups.Length];

                for (var j = 0; j < Data.Packages[i].Groups.Length; j++)
                {
                    LookModdeGroups[LookModdePackages[i]][j] = Data.Packages[i].Groups[j].Name;

                    LookModdeCollectors[(i, j)] = new string[Data.Packages[i].Groups[j].Collectors.Length];
                    LookData[(i, j)] = new List<AssetDataInfo>();
                    var ListType = new List<string>();
                    for (var k = 0; k < Data.Packages[i].Groups[j].Collectors.Length; k++)
                    {
                        Data.Packages[i].Groups[j].Collectors[k]
                            .CollectAsset(Data.Packages[i].Name, Data.Packages[i].Groups[j].Name);
                        LookModdeCollectors[(i, j)][k] =
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
                            if (!ListType.Contains(type.FullName)) ListType.Add(type.FullName);
                            LookData[(i, j)].Add(assetDataInfo.Value);
                        }
                    }

                    LookModdeCollectorTypes[(i, j)] = ListType.ToArray();
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
        }
    }
}