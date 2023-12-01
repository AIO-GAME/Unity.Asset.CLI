/*|============|*|
|*|Author:     |*| Star fire
|*|Date:       |*| 2023-12-01
|*|E-Mail:     |*| xinansky99@foxmail.com
|*|============|*/

using UnityEngine;

namespace AIO.UEditor
{
    public partial class AssetCollectWindow
    {
        partial void OnDrawSetting()
        {
            var width = GTOption.Width(DrawSettingWidth - 20);
            using (GELayout.Vertical(GEStyle.GridList, width))
            {
                if (GELayout.Button("Setting", GEStyle.PreButton, GTOption.Height(20))) ShowSetting = false;
                Data.EnableAddressable = GELayout.ToggleLeft("开启地址化", Data.EnableAddressable, width);
                Data.LocationToLower = GELayout.ToggleLeft("定位地址小写", Data.LocationToLower, width);
                Data.IncludeAssetGuid = GELayout.ToggleLeft("包含资源GUID", Data.IncludeAssetGuid, width);
                Data.UniqueBundleName = GELayout.ToggleLeft("唯一Bundle名称", Data.UniqueBundleName, width);
            }
        }
    }
}