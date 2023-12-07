/*|============|*|
|*|Author:     |*| Star fire
|*|Date:       |*| 2023-12-01
|*|E-Mail:     |*| xinansky99@foxmail.com
|*|============|*/

using UnityEditor;
using UnityEngine;

namespace AIO.UEditor
{
    public partial class AssetCollectWindow
    {
        private bool ShowConfigSetting = false;
        private bool ShowPackageInfo = true;
        private bool ShowCollectors = true;

        private void OnDrawPackageInfo()
        {
            if (Data.Packages.Length == 0) return;
            using (GELayout.Vertical(GEStyle.INThumbnailShadow))
            {
                if (CurrentPackageIndex < 0 || Data.Packages.Length <= CurrentPackageIndex)
                {
                    GELayout.HelpBox("Package Index Error");
                    return;
                }

                if (ShowPackageInfo)
                {
                    Data.Packages[CurrentPackageIndex].Name =
                        GELayout.Field("Package Name", Data.Packages[CurrentPackageIndex].Name);

                    Data.Packages[CurrentPackageIndex].Description =
                        GELayout.Field("Package Description", Data.Packages[CurrentPackageIndex].Description);
                }

                if (Data.Packages[CurrentPackageIndex].Groups.Length == 0) return;

                if (CurrentGroupIndex < 0 ||
                    Data.Packages[CurrentPackageIndex].Groups.Length <= CurrentGroupIndex)
                {
                    GELayout.HelpBox("Groups Index Error");
                    return;
                }

                if (ShowPackageInfo)
                {
                    Data.Packages[CurrentPackageIndex].Groups[CurrentGroupIndex].Name = GELayout.Field("Group Name",
                        Data.Packages[CurrentPackageIndex].Groups[CurrentGroupIndex].Name);

                    Data.Packages[CurrentPackageIndex].Groups[CurrentGroupIndex].Description = GELayout.Field(
                        "Group Description", Data.Packages[CurrentPackageIndex].Groups[CurrentGroupIndex].Description);

                    Data.Packages[CurrentPackageIndex].Groups[CurrentGroupIndex].Tags = GELayout.Field("Group Tags",
                        Data.Packages[CurrentPackageIndex].Groups[CurrentGroupIndex].Tags);
                }
            }
        }

        private void OnDrawItem()
        {
            if (Data.Packages.Length == 0) return;
            if (Data.Packages.Length >= CurrentPackageIndex) CurrentPackageIndex = 0;
            if (Data.Packages[CurrentPackageIndex].Groups.Length == 0) return;
            if (Data.Packages[CurrentPackageIndex].Groups[CurrentGroupIndex].Collectors.Length == 0) return;
            for (var i = Data.Packages[CurrentPackageIndex].Groups[CurrentGroupIndex].Collectors.Length - 1;
                 i >= 0;
                 i--)
            {
                OnDrawItem(Data.Packages[CurrentPackageIndex].Groups[CurrentGroupIndex].Collectors[i]);
                GELayout.Space();
            }
        }

        partial void OnDrawGroupList()
        {
            using (GELayout.Vertical(GEStyle.GridList))
            {
                ShowConfigSetting =
                    GELayout.VFoldoutHeaderGroupWithHelp(OnDrawASConfig, "Config Setting", ShowConfigSetting);

                GELayout.Space();

                ShowPackageInfo =
                    GELayout.VFoldoutHeaderGroupWithHelp(OnDrawPackageInfo, "Package Info", ShowPackageInfo);

                GELayout.Space();

                ShowCollectors = GELayout.VFoldoutHeaderGroupWithHelp(
                    OnDrawItem,
                    "Collectors",
                    ShowCollectors,
                    () =>
                    {
                        Data.Packages[CurrentPackageIndex].Groups[CurrentGroupIndex].Collectors =
                            Data.Packages[CurrentPackageIndex].Groups[CurrentGroupIndex].Collectors.Add(
                                new AssetCollectItem());
                    }, 0, null, new GUIContent("✚"));
            }
        }
    }
}