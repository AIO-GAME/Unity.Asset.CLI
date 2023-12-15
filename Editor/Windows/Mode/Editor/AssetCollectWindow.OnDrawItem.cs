/*|============|*|
|*|Author:     |*| Star fire
|*|Date:       |*| 2023-12-01
|*|E-Mail:     |*| xinansky99@foxmail.com
|*|============|*/

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

        partial void OnDrawItem(AssetCollectItem item)
        {
            using (GELayout.Vertical())
            {
                using (GELayout.VHorizontal(GEStyle.Toolbar))
                {
                    if (GELayout.Button(item.Folded ? GC_FOLDOUT : GC_FOLDOUT_ON,
                            GEStyle.TEtoolbarbutton, GP_Width_30))
                    {
                        item.Folded = !item.Folded;
                        GUI.FocusControl(null);
                    }

                    item.Type = GELayout.Popup(item.Type, GEStyle.PreDropDown, GTOption.Width(80));
                    item.Path = GELayout.Field(item.Path);

                    if (GELayout.Button(GC_DEL, GEStyle.TEtoolbarbutton, 24))
                    {
                        Data.Packages[CurrentPackageIndex].Groups[CurrentGroupIndex].Collectors =
                            Data.Packages[CurrentPackageIndex].Groups[CurrentGroupIndex].Collectors.Remove(item);
                        GUI.FocusControl(null);
                        return;
                    }

                    if (GELayout.Button(GC_OPEN, GEStyle.TEtoolbarbutton, 24))
                    {
                        if (item.Type == EAssetCollectItemType.MainAssetCollector)
                        {
                            var status = 1;
                            foreach (var collector in Data.Packages[CurrentPackageIndex].Groups[CurrentGroupIndex]
                                         .Collectors)
                            {
                                if (collector.CollectPath != item.CollectPath)
                                {
                                    status *= 2;
                                    continue;
                                }

                                LookModeDisplayCollectorsIndex = status;
                                break;
                            }

                            GUI.FocusControl(null);
                            WindowMode = Mode.Look;
                            UpdateDataLookMode();
                            return;
                        }
                        else
                        {
                            EditorUtility.DisplayDialog("打开", "只有动态资源才能查询", "确定");
                        }
                    }
                }

                if (item.Folded) return;

                using (GELayout.VHorizontal())
                {
                    GELayout.Label("定位", GP_Width_25);
                    item.LocationFormat = GELayout.Popup(item.LocationFormat, GEStyle.PreDropDown, GTOption.Width(80));
                    item.Address = GELayout.Popup(item.Address, AssetCollectSetting.MapAddress.Displays,
                        GEStyle.PreDropDown);

                    item.RulePackIndex = GELayout.Popup(item.RulePackIndex, AssetCollectSetting.MapPacks.Displays,
                        GEStyle.PreDropDown);

                    item.LoadType = GELayout.Popup(item.LoadType, GEStyle.PreDropDown, GP_Width_75);
                    item.HasExtension =
                        GELayout.ToggleLeft("后缀", item.HasExtension, GTOption.Width(42));
                }

                using (GELayout.VHorizontal())
                {
                    GELayout.Label("收集", GP_Width_25);
                    if (item.RuleUseCollectCustom)
                    {
                        item.RuleCollect = GELayout.Field(item.RuleCollect);
                    }
                    else
                    {
                        item.RuleCollectIndex = GELayout.Mask(item.RuleCollectIndex,
                            AssetCollectSetting.MapCollect.Displays, GEStyle.PreDropDown,
                            GTOption.Width(80));
                        GELayout.HelpBox(GetInfo(AssetCollectSetting.MapCollect.Displays, item.RuleCollectIndex));
                    }

                    item.RuleUseCollectCustom = GELayout.ToggleLeft(
                        new GUIContent("自定", "自定义收集规则 \n传入文件后缀 \n冒号(;)隔开 \n无需填写点(.)"),
                        item.RuleUseCollectCustom, GTOption.Width(42));
                }

                using (GELayout.VHorizontal())
                {
                    GELayout.Label("过滤", GP_Width_25);
                    if (item.RuleUseFilterCustom)
                    {
                        item.RuleFilter = GELayout.Field(item.RuleFilter);
                    }
                    else
                    {
                        item.RuleFilterIndex =
                            GELayout.Mask(item.RuleFilterIndex, AssetCollectSetting.MapFilter.Displays,
                                GEStyle.PreDropDown, GTOption.Width(80));
                        GELayout.HelpBox(GetInfo(AssetCollectSetting.MapFilter.Displays, item.RuleFilterIndex));
                    }

                    item.RuleUseFilterCustom = GELayout.ToggleLeft(
                        new GUIContent("自定", "自定义过滤规则 \n传入文件后缀 \n冒号(;)隔开 \n无需填写点(.)"),
                        item.RuleUseFilterCustom, GTOption.Width(42));
                }

                using (GELayout.VHorizontal())
                {
                    GELayout.Label("自定", GP_Width_25);
                    item.UserData = GELayout.Field(item.UserData);
                }

                if (item.Type == EAssetCollectItemType.MainAssetCollector)
                {
                    using (GELayout.VHorizontal())
                    {
                        GELayout.Label("标签", GP_Width_25);
                        item.Tags = GELayout.Field(item.Tags);
                    }
                }
            }
        }
    }
}