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
        partial void OnDrawPackage()
        {
            using (GELayout.Vertical(GEStyle.GridList))
            {
                using (GELayout.VHorizontal())
                {
                    if (GELayout.Button("Package", GEStyle.PreButton, GTOption.Height(20))) ShowPackage = false;
                    if (GELayout.Button("+", GEStyle.PreButton, 20))
                    {
                        Data.Packages = Data.Packages.Add(new AssetCollectPackage
                        {
                            Name = "Default Package",
                            Description = Data.Packages.Length.ToString(),
                            Groups = new AssetCollectGroup[] { }
                        });

                        if (Data.Packages.Length == 1)
                        {
                            CurrentPackageIndex = 0;
                            ShowGroup = true;
                        }
                    }
                }

                for (var i = Data.Packages.Length - 1; i >= 0; i--)
                {
                    using (GELayout.VHorizontal())
                    {
                        var label = string.IsNullOrEmpty(Data.Packages[i].Description)
                            ? Data.Packages[i].Name
                            : string.Concat(Data.Packages[i].Name, '(', Data.Packages[i].Description, ')');

                        var style = CurrentPackageIndex == i
                            ? GEStyle.PreButton
                            : GEStyle.RLFooterButton;
                        if (GELayout.Button(label, style))
                        {
                            CurrentPackageIndex = i;
                            ShowGroup = true;
                        }

                        if (GELayout.Button("-", GEStyle.PreButton, 20))
                        {
                            Data.Packages = Data.Packages.RemoveAt(i);
                            if (--CurrentPackageIndex < 0) CurrentPackageIndex = 0;
                            if (CurrentPackageIndex >= Data.Packages.Length)
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