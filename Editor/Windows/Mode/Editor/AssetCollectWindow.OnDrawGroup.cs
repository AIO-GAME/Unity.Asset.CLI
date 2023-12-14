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
        /// <summary>
        /// 绘制组
        /// </summary>
        partial void OnDrawGroup()
        {
            using (new EditorGUILayout.VerticalScope(GEStyle.GridList))
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    if (GUILayout.Button(GC_ADD, GEStyle.TEtoolbarbutton, GP_Width_20))
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
                            ViewGroupList.IsShow = true;
                        }

                        GUI.FocusControl(null);
                    }

                    if (GUILayout.Button("Group", GEStyle.PreToolbar))
                    {
                        ViewGroupList.IsShow = false;
                        GUI.FocusControl(null);
                    }
                }

                if (Data.Packages.Length <= CurrentPackageIndex || CurrentPackageIndex < 0)
                {
                    CurrentPackageIndex = 0;
                    return;
                }

                for (var i = Data.Packages[CurrentPackageIndex].Groups.Length - 1; i >= 0; i--)
                {
                    using (new EditorGUILayout.HorizontalScope())
                    {
                        if (GUILayout.Button(GC_DEL, GEStyle.TEtoolbarbutton, GP_Width_20))
                        {
                            Data.Packages[CurrentPackageIndex].Groups =
                                Data.Packages[CurrentPackageIndex].Groups.RemoveAt(i);
                            if (--CurrentGroupIndex < 0) CurrentGroupIndex = 0;
                            if (CurrentGroupIndex >= Data.Packages[CurrentPackageIndex].Groups.Length)
                                ViewGroupList.IsShow = false;
                            GUI.FocusControl(null);
                            return;
                        }

                        var label = string.IsNullOrEmpty(Data.Packages[CurrentPackageIndex].Groups[i].Description)
                            ? Data.Packages[CurrentPackageIndex].Groups[i].Name
                            : string.Concat(Data.Packages[CurrentPackageIndex].Groups[i].Name, '(',
                                Data.Packages[CurrentPackageIndex].Groups[i].Description, ')');

                        var style = CurrentGroupIndex == i
                            ? GEStyle.PRInsertion
                            : GEStyle.ObjectPickerTab;
                        if (GUILayout.Button(label, style))
                        {
                            CurrentGroupIndex = i;
                            ViewGroupList.IsShow = true;
                            GUI.FocusControl(null);
                        }
                    }
                }
            }
        }
    }
}