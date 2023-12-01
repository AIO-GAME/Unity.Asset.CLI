/*|============|*|
|*|Author:     |*| Star fire
|*|Date:       |*| 2023-12-01
|*|E-Mail:     |*| xinansky99@foxmail.com
|*|============|*/

namespace AIO.UEditor
{
    public partial class AssetCollectWindow
    {
        private bool ShowPackageInfo = true;
        private bool ShowCollectors = true;

        partial void OnDrawGroupList()
        {
            using (GELayout.Vertical(GEStyle.GridList))
            {
                if (GELayout.Button("Package Info", GEStyle.PreButton, GTOption.Width(true), GTOption.Height(20)))
                {
                    ShowPackageInfo = !ShowPackageInfo;
                }

                if (Data.Packages.Length == 0) return;

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

                using (GELayout.VHorizontal())
                {
                    if (GELayout.Button("Collectors", GEStyle.PreButton, GTOption.Width(true), GTOption.Height(20)))
                    {
                        ShowCollectors = !ShowCollectors;
                    }

                    if (GELayout.Button("+", GEStyle.PreButton, 20))
                    {
                        Data.Packages[CurrentPackageIndex].Groups[CurrentGroupIndex].Collectors =
                            Data.Packages[CurrentPackageIndex].Groups[CurrentGroupIndex].Collectors.Add(
                                new AssetCollectItem());
                    }
                }

                if (ShowCollectors)
                {
                    for (var i = Data.Packages[CurrentPackageIndex].Groups[CurrentGroupIndex].Collectors.Length - 1;
                         i >= 0;
                         i--)
                    {
                        OnDrawItem(Data.Packages[CurrentPackageIndex].Groups[CurrentGroupIndex].Collectors[i]);
                        GELayout.Space();
                    }
                }
            }
        }
    }
}