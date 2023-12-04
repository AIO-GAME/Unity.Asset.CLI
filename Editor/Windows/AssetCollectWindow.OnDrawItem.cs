/*|============|*|
|*|Author:     |*| Star fire
|*|Date:       |*| 2023-12-01
|*|E-Mail:     |*| xinansky99@foxmail.com
|*|============|*/

using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace AIO.UEditor
{
    public partial class AssetCollectWindow
    {
        private static string GetInfo(IEnumerable<string> Displays, long source)
        {
            var _sb = new StringBuilder();
            if (source <= -1)
            {
                _sb.Append("当前选中: ").Append(source).Append(" -> ");
                foreach (var disPlay in Displays) _sb.Append(disPlay).Append(";");
                return _sb.ToString();
            }

            _sb.Append("当前选中: ").Append(source).Append(" -> ");

            var status = 1L;
            foreach (var display in Displays)
            {
                if ((source & status) == status) _sb.Append(display).Append(";");
                status *= 2;
            }

            return _sb.ToString();
        }

        partial void OnDrawItem(AssetCollectItem item)
        {
            using (GELayout.Vertical(GEStyle.DDHeaderStyle))
            {
                using (GELayout.VHorizontal())
                {
                    if (GELayout.Button("-", 30))
                    {
                        Data.Packages[CurrentPackageIndex].Groups[CurrentGroupIndex].Collectors =
                            Data.Packages[CurrentPackageIndex].Groups[CurrentGroupIndex].Collectors.Remove(item);
                        return;
                    }

                    item.Path = GELayout.Field(item.Path);
                    item.Type = GELayout.Popup(item.Type, GTOption.Width(80));
                    if (GELayout.Button("List", 50))
                    {
                        OnDrawCurrentItem = item;
                        OnDrawCurrentItem.CollectAsset(
                            Data.Packages[CurrentPackageIndex].Name,
                            Data.Packages[CurrentPackageIndex].Groups[CurrentGroupIndex].Name);
                        return;
                    }
                }

                using (GELayout.VHorizontal())
                {
                    GELayout.Label("定位", GTOption.Width(30));
                    item.Address = GELayout.Popup(item.Address, AssetCollectSetting.MapAddress.Displays);
                    item.LocationFormat = GELayout.Popup(item.LocationFormat, GTOption.Width(80));
                    item.HasExtension = GELayout.ToggleLeft("后缀", item.HasExtension, GTOption.Width(50));
                }

                using (GELayout.VHorizontal())
                {
                    GELayout.Label("收集", GTOption.Width(30));
                    if (item.RuleUseCollectCustom)
                    {
                        item.RuleCollect = GELayout.Field(item.RuleCollect);
                    }
                    else
                    {
                        item.RuleCollectIndex = GELayout.Mask(item.RuleCollectIndex,
                            AssetCollectSetting.MapCollect.Displays,
                            GTOption.Width(80));
                        GELayout.HelpBox(GetInfo(AssetCollectSetting.MapCollect.Displays, item.RuleCollectIndex));
                    }

                    item.RuleUseCollectCustom = GELayout.ToggleLeft(
                        new GUIContent("自定", "自定义收集规则 \n传入文件后缀 \n冒号(;)隔开 \n无需填写点(.)"),
                        item.RuleUseCollectCustom, GTOption.Width(50));
                }

                using (GELayout.VHorizontal())
                {
                    GELayout.Label("过滤", GTOption.Width(30));
                    if (item.RuleUseFilterCustom)
                    {
                        item.RuleFilter = GELayout.Field(item.RuleFilter);
                    }
                    else
                    {
                        item.RuleFilterIndex =
                            GELayout.Mask(item.RuleFilterIndex, AssetCollectSetting.MapFilter.Displays,
                                GTOption.Width(80));
                        GELayout.HelpBox(GetInfo(AssetCollectSetting.MapFilter.Displays, item.RuleFilterIndex));
                    }

                    item.RuleUseFilterCustom = GELayout.ToggleLeft(
                        new GUIContent("自定", "自定义过滤规则 \n传入文件后缀 \n冒号(;)隔开 \n无需填写点(.)"),
                        item.RuleUseFilterCustom, GTOption.Width(50));
                }

                using (GELayout.VHorizontal())
                {
                    GELayout.Label("标签", GTOption.Width(30));
                    item.Tags = GELayout.Field(item.Tags);
                }

                using (GELayout.VHorizontal())
                {
                    GELayout.Label("自定", GTOption.Width(30));
                    item.UserData = GELayout.Field(item.UserData);
                }
            }
        }
    }
}