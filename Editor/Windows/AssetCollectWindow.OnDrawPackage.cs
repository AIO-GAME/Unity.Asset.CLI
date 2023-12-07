/*|============|*|
|*|Author:     |*| Star fire
|*|Date:       |*| 2023-12-01
|*|E-Mail:     |*| xinansky99@foxmail.com
|*|============|*/

using System.Collections.Generic;
using UnityEngine;

namespace AIO.UEditor
{
    public partial class AssetCollectWindow
    {
        partial void OnDrawPackage()
        {
            using (GELayout.VHorizontal(GEStyle.PreToolbar))
            {
                if (GELayout.Button("✚", GEStyle.ObjectPickerTab, 20, 20))
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

                    GUI.FocusControl(null);
                }

                GELayout.Label("|", GTOption.Width(5));
                if (GELayout.Button("Package", GEStyle.ObjectPickerTab, GTOption.Width(DrawPackageWidth - 50)))
                {
                    ShowPackage = false;
                    GUI.FocusControl(null);
                }
            }

            using (GELayout.Vertical(GEStyle.GridList))
            {
                for (var i = Data.Packages.Length - 1; i >= 0; i--)
                {
                    using (GELayout.VHorizontal(GTOption.Height(25)))
                    {
                        if (GELayout.Button(GC_DEL, GEStyle.ObjectPickerTab, 20, 20))
                        {
                            Data.Packages = Data.Packages.RemoveAt(i);
                            if (--CurrentPackageIndex < 0) CurrentPackageIndex = 0;
                            if (CurrentPackageIndex >= Data.Packages.Length) ShowGroup = false;
                            GUI.FocusControl(null);
                            return;
                        }

                        GELayout.Label("|", GTOption.Width(5));
                        var label = string.IsNullOrEmpty(Data.Packages[i].Description)
                            ? Data.Packages[i].Name
                            : string.Concat(Data.Packages[i].Name, '(', Data.Packages[i].Description, ')');

                        var style = CurrentPackageIndex == i
                            ? GEStyle.PRInsertion
                            : GEStyle.ObjectPickerTab;
                        if (GELayout.Button(label, style, GTOption.Height(20), GTOption.Width(DrawPackageWidth - 50)))
                        {
                            CurrentPackageIndex = i;
                            ShowGroup = true;
                            GUI.FocusControl(null);
                        }
                    }
                }
            }
        }
    }
}