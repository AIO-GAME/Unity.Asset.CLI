/*|============|*|
|*|Author:     |*| Star fire
|*|Date:       |*| 2023-12-01
|*|E-Mail:     |*| xinansky99@foxmail.com
|*|============|*/

namespace AIO.UEditor
{
    public partial class AssetCollectWindow
    {
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
                        return;
                    }
                }

                using (GELayout.VHorizontal())
                {
                    GELayout.Label("收集", GTOption.Width(30));
                    item.Collect = GELayout.Field(item.Collect);
                    GELayout.Label("过滤", GTOption.Width(30));
                    item.Filter = GELayout.Field(item.Filter);
                }

                using (GELayout.VHorizontal())
                {
                    GELayout.Label("标签", GTOption.Width(30));
                    item.Tags = GELayout.Field(item.Tags);
                    GELayout.Label("自定义数据", GTOption.Width(75));
                    item.UserData = GELayout.Field(item.UserData);
                }

                using (GELayout.VHorizontal())
                {
                    GELayout.Label("定位", GTOption.Width(30));
                    item.Address = GELayout.Field(item.Address);
                    item.LocationFormat = GELayout.Popup(item.LocationFormat, GTOption.Width(50));
                }
            }
        }
    }
}