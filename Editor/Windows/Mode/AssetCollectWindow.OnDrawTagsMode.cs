/*|============|*|
|*|Author:     |*| Star fire
|*|Date:       |*| 2023-12-01
|*|E-Mail:     |*| xinansky99@foxmail.com
|*|============|*/

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace AIO.UEditor
{
    public partial class AssetCollectWindow
    {
        /// <summary>
        /// 绘制 标签模式 导航栏
        /// </summary>
        private void OnDrawHeaderTagsMode()
        {
            if (!Data.IsCollectValid())
            {
                EditorGUILayout.Separator();
                return;
            }

            SearchText = EditorGUILayout.TextField(SearchText, GEStyle.SearchTextField);
            if (GUILayout.Button(GC_CLEAR, GEStyle.TEtoolbarbutton, GP_Width_25))
            {
                GUI.FocusControl(null);
                SearchText = string.Empty;
            }

            if (TagsModeDisplayCollectors.Length > 0)
            {
                if (TagsModeDisplayCollectors.Length > 31)
                {
                    LookModeDisplayCollectorsIndex = EditorGUILayout.Popup(LookModeDisplayCollectorsIndex,
                        TagsModeDisplayCollectors, GEStyle.PreDropDown, GP_Width_100);
                }
                else
                    LookModeDisplayCollectorsIndex = EditorGUILayout.MaskField(LookModeDisplayCollectorsIndex,
                        TagsModeDisplayCollectors, GEStyle.PreDropDown, GP_Width_100);
            }

            if (TagsModeDisplayTypes.Length > 0)
            {
                LookModeDisplayTypeIndex = EditorGUILayout.MaskField(LookModeDisplayTypeIndex,
                    TagsModeDisplayTypes, GEStyle.PreDropDown, GP_Width_100);
            }

            if (TagsModeDisplayTags.Length > 0)
            {
                LookModeDisplayTagsIndex = EditorGUILayout.MaskField(LookModeDisplayTagsIndex,
                    TagsModeDisplayTags, GEStyle.PreDropDown, GP_Width_100);
            }


            if (GUI.changed)
            {
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
                    CurrentPageValues.Add(CurrentTagValues.Where(data => !TagsModeDataFilter(data)));
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
                UpdateDataTagsMode();
            }
        }

        /// <summary>
        /// 绘制 标签模式 资源列表
        /// </summary>
        partial void OnDrawTagsMode()
        {
            if (Data.Packages.Length == 0)
            {
                GELayout.HelpBox("当前无包资源数据");
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

        /// <summary>
        /// 更新数据 标签模式
        /// </summary>
        private void UpdateDataTagsMode()
        {
            GUI.FocusControl(null);
            LookModeCurrentSelectAsset = null;

            CurrentPageValues.Clear();
            CurrentPageValues.PageIndex = 0;
            CurrentTagValues.Clear();
            LookModeCollectorsALLSize = 0;
            LookModeCollectorsPageSize = 0;

            if (Data.Packages.Length == 0) return;
            TagsModeDisplayCollectors = new string[] { "ALL" };
            TagsModeDisplayTypes = Array.Empty<string>();
            TagsModeDisplayTags = Data.GetTags();
            var collectors = new List<string>();
            var listTypes = new List<string>();
            foreach (var package in Data.Packages)
            {
                if (package.Groups is null) continue;
                foreach (var group in package.Groups)
                {
                    if (group.Collectors is null) continue;
                    var flag = !string.IsNullOrEmpty(group.Tags);
                    collectors.AddRange(GetCollectorDisPlayNames(group.Collectors, collector => flag || !string.IsNullOrEmpty(collector.Tags)));
                    foreach (var collector in group.Collectors)
                    {
                        if (collector.Type != EAssetCollectItemType.MainAssetCollector) continue;
                        if (string.IsNullOrEmpty(collector.CollectPath)) continue;
                        if (!flag && string.IsNullOrEmpty(collector.Tags)) continue;
                        collector.CollectAssetTask(package.Name, group.Name, dic =>
                        {
                            foreach (var pair in dic)
                            {
                                listTypes.Add(pair.Value.Type);
                                CurrentTagValues.Add(pair.Value);
                                if (!LookModeDataFilter(pair.Value))
                                {
                                    CurrentPageValues.Add(pair.Value);
                                    LookModeCollectorsALLSize += pair.Value.Size;
                                }
                            }

                            CurrentPageValues.PageIndex = CurrentPageValues.PageIndex;
                            TagsModeDisplayTypes = listTypes.Distinct().ToArray();
                            Repaint();
                        });
                    }
                }
            }

            if (collectors.Count > 31)
            {
                TagsModeDisplayCollectors = new string[collectors.Count + 1];
                TagsModeDisplayCollectors[0] = "ALL";
                var tempIndex = 1;
                foreach (var key in collectors)
                {
                    TagsModeDisplayCollectors[tempIndex++] = key;
                }
            }
            else TagsModeDisplayCollectors = collectors.ToArray();

            if (LookModeDisplayCollectorsIndex < 0) LookModeDisplayCollectorsIndex = 0;
        }

        /// <summary>
        /// 标签模式 资源过滤器
        /// </summary>
        private bool TagsModeDataFilter(AssetDataInfo data)
        {
            var filter = 0;
            if (TagsModeDisplayCollectors.Length > 31)
            {
                if (LookModeDisplayCollectorsIndex == 0 ||
                    TagsModeDisplayCollectors[LookModeDisplayCollectorsIndex] ==
                    data.CollectPath.Replace('/', '\\').TrimEnd('\\'))
                    filter++;
            }
            else
            {
                if (LookModeDisplayCollectorsIndex < 1) filter++;
                else
                {
                    var status = 1L;
                    foreach (var display in TagsModeDisplayCollectors)
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
            }


            if (LookModeDisplayTypeIndex < 1) filter++;
            else
            {
                var objectType = AssetDatabase.GetMainAssetTypeAtPath(data.AssetPath)?.FullName;
                if (string.IsNullOrEmpty(objectType)) objectType = "Unknown";
                var status = 1L;
                foreach (var display in TagsModeDisplayTypes)
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
                foreach (var display in TagsModeDisplayTags)
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
                if (data.AssetPath.Contains(SearchText, StringComparison.CurrentCultureIgnoreCase) ||
                    data.Address.Contains(SearchText, StringComparison.CurrentCultureIgnoreCase))
                {
                    filter++;
                }
                else if (SearchText.Contains(data.AssetPath, StringComparison.CurrentCultureIgnoreCase) ||
                         SearchText.Contains(data.Address, StringComparison.CurrentCultureIgnoreCase))
                {
                    filter++;
                }
            }

            return filter != 4;
        }
    }
}