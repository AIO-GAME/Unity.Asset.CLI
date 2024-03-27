using System;
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
                        Data.CurrentPackage.Groups = Data.CurrentPackage.Groups.Add(new AssetCollectGroup
                        {
                            Name = "Default Group",
                            Description = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"),
                            Collectors = Array.Empty<AssetCollectItem>()
                        });

                        if (Data.CurrentPackage.Groups.Length == 1)
                        {
                            Data.CurrentGroupIndex = 0;
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

                if (!Data.IsGroupValid()) return;

                for (var i = Data.CurrentPackage.Groups.Length - 1; i >= 0; i--)
                {
                    using (new EditorGUILayout.HorizontalScope())
                    {
                        if (GUILayout.Button(GC_DEL, GEStyle.TEtoolbarbutton, GP_Width_20))
                        {
                            GUI.FocusControl(null);
                            if (EditorUtility.DisplayDialog(
                                    "Delete Group",
                                    $"Are you sure you want to delete {Data.CurrentPackage.Groups[i].Name}?",
                                    "Yes",
                                    "No"))
                            {
                                Data.CurrentPackage.Groups = Data.CurrentPackage.Groups.RemoveAt(i).Exclude();
                                if (--Data.CurrentGroupIndex < 0) Data.CurrentGroupIndex = 0;
                                if (Data.CurrentGroupIndex >= Data.Packages[Data.CurrentPackageIndex].Groups.Length)
                                    ViewGroupList.IsShow = false;
                            }

                            return;
                        }

                        var label = string.IsNullOrEmpty(Data.CurrentPackage.Groups[i].Description)
                            ? Data.CurrentPackage.Groups[i].Name
                            : string.Concat(Data.CurrentPackage.Groups[i].Name, '(',
                                Data.CurrentPackage.Groups[i].Description, ')');

                        var style = Data.CurrentGroupIndex == i
                            ? GEStyle.PRInsertion
                            : GEStyle.ObjectPickerTab;
                        if (GUILayout.Button(label, style, GUILayout.MinWidth(100)))
                        {
                            Data.CurrentGroupIndex = i;
                            ViewGroupList.IsShow = true;

                            if (Data.CurrentGroup.Length > 0)
                            {
                                Data.CurrentGroup.Refresh();
                                CurrentCurrentCollectorsIndex = Data.CurrentGroup.Length - 1;
                            }
                            else CurrentCurrentCollectorsIndex = 0;

                            GUI.FocusControl(null);
                        }
                    }
                }
            }
        }
    }
}