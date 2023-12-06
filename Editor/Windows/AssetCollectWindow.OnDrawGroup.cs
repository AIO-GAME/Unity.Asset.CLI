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
        partial void OnDrawGroup()
        {
            using (GELayout.VHorizontal(GEStyle.PreToolbar))
            {
                if (GELayout.Button(Content_ADD, GEStyle.ObjectPickerTab, 20, 20))
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

                    GUI.FocusControl(null);
                }

                GELayout.Label("|", GTOption.Width(4));
                if (GELayout.Button("Group", GEStyle.ObjectPickerTab, GTOption.Width(DrawGroupWidth - 50)))
                {
                    ShowGroup = false;
                    GUI.FocusControl(null);
                }
            }

            using (GELayout.Vertical(GEStyle.GridList))
            {
                for (var i = Data.Packages[CurrentPackageIndex].Groups.Length - 1; i >= 0; i--)
                {
                    using (GELayout.VHorizontal(GTOption.Height(25)))
                    {
                        if (GELayout.Button(Content_DEL, GEStyle.ObjectPickerTab, 20, 20))
                        {
                            Data.Packages[CurrentPackageIndex].Groups =
                                Data.Packages[CurrentPackageIndex].Groups.RemoveAt(i);
                            if (--CurrentGroupIndex < 0) CurrentGroupIndex = 0;
                            if (CurrentGroupIndex >= Data.Packages[CurrentPackageIndex].Groups.Length)
                            {
                                ShowGroup = false;
                            }

                            GUI.FocusControl(null);
                            return;
                        }

                        GELayout.Label("|", GTOption.Width(5));
                        var label = string.IsNullOrEmpty(Data.Packages[CurrentPackageIndex].Groups[i].Description)
                            ? Data.Packages[CurrentPackageIndex].Groups[i].Name
                            : string.Concat(Data.Packages[CurrentPackageIndex].Groups[i].Name, '(',
                                Data.Packages[CurrentPackageIndex].Groups[i].Description, ')');

                        var style = CurrentGroupIndex == i
                            ? GEStyle.PRInsertion
                            : GEStyle.ObjectPickerTab;
                        if (GELayout.Button(label, style, GTOption.Height(20), GTOption.Width(DrawGroupWidth - 50)))
                        {
                            CurrentGroupIndex = i;
                            ShowGroup = true;
                            GUI.FocusControl(null);
                        }
                    }
                }
            }
        }
    }
}