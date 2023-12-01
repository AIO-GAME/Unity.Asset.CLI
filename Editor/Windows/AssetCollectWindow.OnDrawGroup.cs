/*|============|*|
|*|Author:     |*| Star fire
|*|Date:       |*| 2023-12-01
|*|E-Mail:     |*| xinansky99@foxmail.com
|*|============|*/

using System.Collections.Generic;

namespace AIO.UEditor
{
    public partial class AssetCollectWindow
    {
        partial void OnDrawGroup()
        {
            using (GELayout.Vertical(GEStyle.GridList))
            {
                using (GELayout.VHorizontal())
                {
                    if (GELayout.Button("Group", GEStyle.PreButton, GTOption.Height(20)))
                        ShowGroup = false;

                    if (GELayout.Button("+", GEStyle.PreButton, 20))
                    {
                        Data.Packages[CurrentPackageIndex].Groups = Data.Packages[CurrentPackageIndex].Groups.Add(
                            new AssetCollectGroup
                            {
                                Name = "Default Group",
                                Description = Data.Packages[CurrentPackageIndex].Groups.Length.ToString(),
                                Collectors = new AssetCollectItem[] { }
                            });

                        if (Data.Packages[CurrentPackageIndex].Groups.Length == 1)
                        {
                            CurrentGroupIndex = 0;
                            ShowGroup = true;
                        }
                    }
                }

                for (var i = Data.Packages[CurrentPackageIndex].Groups.Length - 1; i >= 0; i--)
                {
                    using (GELayout.VHorizontal())
                    {
                        var label = string.IsNullOrEmpty(Data.Packages[CurrentPackageIndex].Groups[i].Description)
                            ? Data.Packages[CurrentPackageIndex].Groups[i].Name
                            : string.Concat(Data.Packages[CurrentPackageIndex].Groups[i].Name, '(',
                                Data.Packages[CurrentPackageIndex].Groups[i].Description, ')');

                        var style = CurrentGroupIndex == i
                            ? GEStyle.PreButton
                            : GEStyle.RLFooterButton;
                        if (GELayout.Button(label, style))
                        {
                            CurrentGroupIndex = i;
                            ShowGroup = true;
                        }

                        if (GELayout.Button("-", GEStyle.PreButton, 20))
                        {
                            Data.Packages[CurrentPackageIndex].Groups =
                                Data.Packages[CurrentPackageIndex].Groups.RemoveAt(i);
                            if (--CurrentGroupIndex < 0) CurrentGroupIndex = 0;
                            if (CurrentGroupIndex >= Data.Packages[CurrentPackageIndex].Groups.Length)
                            {
                                ShowGroup = false;
                            }


                            return;
                        }
                    }
                }
            }
        }
    }
}