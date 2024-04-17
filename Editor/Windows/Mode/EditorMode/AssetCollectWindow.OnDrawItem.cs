using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace AIO.UEditor
{
    public partial class AssetCollectWindow
    {
        private static string GetInfo(IEnumerable<string> Displays, long source)
        {
            if (source <= -1) return "全选所有条件";
            if (source == 0) return "忽略当前条件";

            var builder = new StringBuilder().Append("当前选中: ").Append(source).Append(" -> ");
            var status = 1L;
            foreach (var display in Displays)
            {
                if ((source & status) == status) builder.Append(display).Append(";");
                status *= 2;
            }

            return builder.ToString();
        }

        public static void OpenCollectItem(AssetCollectItem item)
        {
            if (!Instance) return;

            if (item.Type == EAssetCollectItemType.MainAssetCollector)
            {
                var list = Data.CurrentGroup.Collectors.GetDisPlayNames();
                if (list.Length > 31)
                {
                    LookModeDisplayCollectorsIndex = 0;
                    for (var i = 0; i < list.Length; i++)
                        if (list[i] == item.CollectPath)
                        {
                            LookModeDisplayCollectorsIndex = i + 1;
                            break;
                        }
                }
                else
                {
                    var status = 1;
                    foreach (var collector in list)
                    {
                        if (collector != item.CollectPath)
                        {
                            status *= 2;
                            continue;
                        }

                        LookModeDisplayCollectorsIndex = status;
                        break;
                    }
                }

                Instance.UpdateDataLookMode();
                WindowMode                             = Mode.Look;
                Instance.TempTable[nameof(WindowMode)] = WindowMode;
                return;
            }

            EditorUtility.DisplayDialog("打开", "只有动态资源才能查询", "确定");
        }

        private void OnDrawItem(AssetCollectItem item, int index)
        {
            using (new EditorGUILayout.HorizontalScope(GEStyle.Toolbar))
            {
                if (item.Path is null) GUI.enabled = false;
                if (GUILayout.Button(item.Folded ? GC_FOLDOUT : GC_FOLDOUT_ON,
                                     GEStyle.TEtoolbarbutton, GP_Width_30))
                {
                    item.Folded                   = !item.Folded;
                    CurrentCurrentCollectorsIndex = index;
                    GUI.FocusControl(null);
                }

                item.Type = GELayout.Popup(item.Type, GEStyle.PreDropDown, GP_Width_80);

                if (item.Path is null) GUI.enabled = true;
                item.Path = EditorGUILayout.ObjectField(item.Path, typeof(Object),
                                                        false, GTOption.WidthMin(120), GTOption.WidthMax(240));
                if (item.Path is null) GUI.enabled = false;

                if (!item.Folded)
                {
                    item.Address = GELayout.Popup(item.Address, AssetCollectSetting.MapAddress.Displays,
                                                  GEStyle.PreDropDown);

                    item.RulePackIndex = GELayout.Popup(item.RulePackIndex, AssetCollectSetting.MapPacks.Displays,
                                                        GEStyle.PreDropDown);

                    item.LoadType = GELayout.Popup(item.LoadType, GEStyle.PreDropDown, GP_Width_75);
                }
                else
                {
                    if (!string.IsNullOrEmpty(item.CollectPath))
                    {
                        if (GUILayout.Button(item.CollectPath, GEStyle.toolbarbutton))
                            _ = PrPlatform.Open.Path(item.CollectPath).Async();
                    }
                    else
                    {
                        item.Folded = false;
                        return;
                    }
                }

                if (GELayout.Button(GC_OPEN, GEStyle.TEtoolbarbutton, 24))
                {
                    GUI.FocusControl(null);
                    if (item.Type == EAssetCollectItemType.MainAssetCollector)
                    {
                        var list = Data.CurrentGroup.Collectors.GetDisPlayNames();
                        if (list.Length > 31)
                        {
                            LookModeDisplayCollectorsIndex = 0;
                            for (var i = 0; i < list.Length; i++)
                                if (list[i] == item.CollectPath)
                                {
                                    LookModeDisplayCollectorsIndex = i + 1;
                                    break;
                                }
                        }
                        else
                        {
                            var status = 1;
                            foreach (var collector in list)
                            {
                                if (collector != item.CollectPath)
                                {
                                    status *= 2;
                                    continue;
                                }

                                LookModeDisplayCollectorsIndex = status;
                                break;
                            }
                        }

                        UpdateDataLookMode();
                        WindowMode                    = Mode.Look;
                        TempTable[nameof(WindowMode)] = WindowMode;
                        Repaint();
                        return;
                    }

                    EditorUtility.DisplayDialog("打开", "只有动态资源才能查询", "确定");
                    return;
                }

                if (item.Path is null) GUI.enabled = true;
                if (GELayout.Button(GC_DEL, GEStyle.TEtoolbarbutton, 24))
                {
                    GUI.FocusControl(null);
                    if (EditorUtility.DisplayDialog("删除", "确定删除当前收集器?", "确定", "取消"))
                    {
                        if (CurrentCurrentCollectorsIndex == index)
                        {
                            if (index == Data.CurrentGroup.Count - 1)
                            {
                                CurrentCurrentCollectorsIndex = index - 1;
                                OnDrawItemListScroll.y        = 0;
                            }
                            else
                            {
                                CurrentCurrentCollectorsIndex = index;
                                OnDrawItemListScroll.y        = index * 27;
                            }
                        }
                        else if (CurrentCurrentCollectorsIndex > index)
                        {
                            CurrentCurrentCollectorsIndex--;
                            OnDrawItemListScroll.y += 27;
                        }

                        if (ItemCollectorsSearching) ItemCollectorsSearchResult.Remove(item);
                        Data.CurrentGroup.Collectors = Data.CurrentGroup.Collectors.Remove(item).Exclude();
                        if (Data.CurrentGroup.Collectors.Length == 0)
                        {
                            ItemCollectorsSearch = null;
                            ItemCollectorsSearchResult.Clear();
                        }

                        return;
                    }
                }
            }

            if (!item.Folded) return;

            using (new EditorGUILayout.VerticalScope(GEStyle.PreBackground))
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    EditorGUILayout.LabelField(GC_Edit_Address, GP_Width_25);
                    if (Config.LoadPathToLower)
                    {
                        GUI.enabled = false;
                        GELayout.Popup(EAssetLocationFormat.ToLower, GEStyle.PreDropDown, GP_Width_80);
                        GUI.enabled = true;
                    }
                    else
                    {
                        item.LocationFormat = GELayout.Popup(item.LocationFormat, GEStyle.PreDropDown, GP_Width_80);
                    }

                    item.Address = GELayout.Popup(item.Address, AssetCollectSetting.MapAddress.Displays,
                                                  GEStyle.PreDropDown);

                    item.RulePackIndex = GELayout.Popup(item.RulePackIndex, AssetCollectSetting.MapPacks.Displays,
                                                        GEStyle.PreDropDown);

                    item.LoadType = GELayout.Popup(item.LoadType, GEStyle.PreDropDown, GP_Width_75);
                    if (Config.HasExtension)
                    {
                        GUI.enabled       = false;
                        item.HasExtension = GELayout.ToggleLeft("后缀", true, GTOption.Width(42));
                        GUI.enabled       = true;
                    }
                    else item.HasExtension = GELayout.ToggleLeft("后缀", item.HasExtension, GTOption.Width(42));
                }

                using (new EditorGUILayout.HorizontalScope())
                {
                    EditorGUILayout.LabelField("收集", GP_Width_25);
                    if (item.RuleUseCollectCustom)
                    {
                        item.RuleCollect = GELayout.Field(item.RuleCollect);
                    }
                    else
                    {
                        item.RuleCollectIndex = GELayout.Mask(
                            item.RuleCollectIndex,
                            AssetCollectSetting.MapCollect.Displays,
                            GEStyle.PreDropDown, GTOption.Width(80));
                        GELayout.HelpBox(GetInfo(AssetCollectSetting.MapCollect.Displays, item.RuleCollectIndex));
                    }

                    item.RuleUseCollectCustom = GELayout.ToggleLeft(
                        new GUIContent("自定",
                                       "自定义收集规则 \n传入文件后缀 \n[冒号(;)/空格( )/逗号(,)]隔开 \n可无需填写点(.)"),
                        item.RuleUseCollectCustom, GTOption.Width(42));
                }

                using (new EditorGUILayout.HorizontalScope())
                {
                    EditorGUILayout.LabelField("过滤", GP_Width_25);
                    if (item.RuleUseFilterCustom)
                    {
                        item.RuleFilter = GELayout.Field(item.RuleFilter);
                    }
                    else
                    {
                        item.RuleFilterIndex = GELayout.Mask(
                            item.RuleFilterIndex,
                            AssetCollectSetting.MapFilter.Displays,
                            GEStyle.PreDropDown, GTOption.Width(80));
                        GELayout.HelpBox(GetInfo(AssetCollectSetting.MapFilter.Displays, item.RuleFilterIndex));
                    }

                    item.RuleUseFilterCustom = GELayout.ToggleLeft(
                        new GUIContent("自定",
                                       "自定义过滤规则 \n传入文件后缀 \n[冒号(;)/空格( )/逗号(,)]隔开\n可无需填写点(.)"),
                        item.RuleUseFilterCustom, GTOption.Width(42));
                }

                if (AssetCollectSetting.MapAddress.Displays[item.Address].Contains("UserData"))
                    using (new EditorGUILayout.HorizontalScope())
                    {
                        EditorGUILayout.LabelField("自定", GP_Width_25);
                        item.UserData = GELayout.Field(item.UserData);
                    }

                if (item.Type == EAssetCollectItemType.MainAssetCollector)
                    using (new EditorGUILayout.HorizontalScope())
                    {
                        EditorGUILayout.LabelField(GC_Edit_Tags, GP_Width_25);
                        item.Tags = GELayout.Field(item.Tags);
                    }
            }
        }
    }
}