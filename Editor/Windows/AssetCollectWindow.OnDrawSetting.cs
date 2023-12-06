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
            using (GELayout.VHorizontal(GEStyle.PreToolbar))
            {
                GELayout.Label("⇘", GEStyle.ObjectPickerTab, GTOption.Width(15));
                if (GELayout.Button("Setting", GEStyle.ObjectPickerTab, GTOption.Width(DrawSettingWidth - 40)))
                {
                    ShowSetting = false;
                    GUI.FocusControl(null);
                }
            }

            var width = GTOption.Width(DrawSettingWidth - 20);
            using (GELayout.Vertical(GEStyle.GridList, width))
            {
                Data.EnableAddressable = GELayout.ToggleLeft("开启地址化", Data.EnableAddressable, width);
                Data.LocationToLower = GELayout.ToggleLeft("定位地址小写", Data.LocationToLower, width);
                Data.IncludeAssetGUID = GELayout.ToggleLeft("包含资源GUID", Data.IncludeAssetGUID, width);
                Data.UniqueBundleName = GELayout.ToggleLeft("唯一Bundle名称", Data.UniqueBundleName, width);
            }
        }
    }
}