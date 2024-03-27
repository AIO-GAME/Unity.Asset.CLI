using System;
using UnityEditor;
using UnityEngine;

namespace AIO.UEditor
{
    public partial class AssetCollectWindow
    {
        partial void OnDrawPackage()
        {
            using (new EditorGUILayout.VerticalScope(GEStyle.GridList))
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    if (GUILayout.Button(GC_ADD, GEStyle.TEtoolbarbutton, GP_Width_20))
                    {
                        Data.Packages = Data.Packages.Add(new AssetCollectPackage
                        {
                            Name = "Default Package",
                            Description = Data.Packages.Length.ToString(),
                            Groups = Array.Empty<AssetCollectGroup>()
                        });

                        if (Data.Packages.Length == 1)
                        {
                            Data.CurrentPackageIndex = 0;
                            ViewGroupList.IsShow = true;
                        }

                        GUI.FocusControl(null);
                    }

                    if (GUILayout.Button("Package", GEStyle.PreToolbar))
                    {
                        ViewPackageList.IsShow = false;
                        GUI.FocusControl(null);
                    }
                }

                for (var i = Data.Packages.Length - 1; i >= 0; i--)
                {
                    using (new EditorGUILayout.HorizontalScope())
                    {
                        if (GUILayout.Button(GC_DEL, GEStyle.TEtoolbarbutton, GP_Width_20))
                        {
                            if (EditorUtility.DisplayDialog("Delete Package",
                                    $"Are you sure you want to delete {Data.Packages[i].Name}?", "Yes", "No"))
                            {
                                Data.Packages = Data.Packages.RemoveAt(i).Exclude();
                                if (--Data.CurrentPackageIndex < 0) Data.CurrentPackageIndex = 0;
                                if (Data.CurrentPackageIndex >= Data.Packages.Length) ViewGroupList.IsShow = false;
                            }

                            GUI.FocusControl(null);
                            return;
                        }

                        var label = string.IsNullOrEmpty(Data.Packages[i].Description)
                            ? Data.Packages[i].Name
                            : string.Concat(Data.Packages[i].Name, '(', Data.Packages[i].Description, ')');

                        var style = Data.CurrentPackageIndex == i
                            ? GEStyle.PRInsertion
                            : GEStyle.ObjectPickerTab;
                        if (GUILayout.Button(label, style, GTOption.WidthMin(100)))
                        {
                            Data.CurrentPackageIndex = i;
                            Data.CurrentGroupIndex = 0;
                            ViewGroupList.IsShow = true;
                            if (Data.CurrentGroup.Length > 0)
                            {
                                Data.CurrentGroup.Refresh();
                                CurrentCurrentCollectorsIndex = Data.CurrentGroup.Length - 1;
                                OnDrawItemListScroll.y = 0;
                            }
                            else
                            {
                                CurrentCurrentCollectorsIndex = 0;
                            }


                            GUI.FocusControl(null);
                        }
                    }
                }
            }
        }
    }
}