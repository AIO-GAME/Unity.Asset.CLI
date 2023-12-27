/*|============|*|
|*|Author:     |*| Star fire
|*|Date:       |*| 2023-12-01
|*|E-Mail:     |*| xinansky99@foxmail.com
|*|============|*/

using System;
using AIO.UEngine;
using UnityEditor;
using UnityEngine;

namespace AIO.UEditor
{
    public partial class AssetCollectWindow
    {
        private void OnDrawHeaderEditorMode()
        {
            if (!ViewPackageList.IsShow &&
                GUILayout.Button("⇘ Package", GEStyle.TEtoolbarbutton, GP_Width_75, GP_Height_20))
            {
                GUI.FocusControl(null);
                ViewPackageList.IsShow = true;
            }

            if (Data.Packages.Length <= CurrentPackageIndex || CurrentPackageIndex < 0 ||
                Data.Packages[CurrentPackageIndex] is null)
            {
                GUI.FocusControl(null);
                ViewGroupList.IsShow = false;
            }
            else if (!ViewGroupList.IsShow &&
                     GUILayout.Button("⇘ Group", GEStyle.TEtoolbarbutton, GP_Width_75, GP_Height_20))
            {
                GUI.FocusControl(null);
                ViewGroupList.IsShow = true;
            }

            TempBuilder.Clear();
            if (CurrentPackageIndex >= 0 && Data.Packages.Length > CurrentPackageIndex)
            {
                TempBuilder.Append(Data.Packages[CurrentPackageIndex].Name);
                if (!string.IsNullOrEmpty(Data.Packages[CurrentPackageIndex].Description))
                {
                    TempBuilder.Append('(');
                    TempBuilder.Append(Data.Packages[CurrentPackageIndex].Description);
                    TempBuilder.Append(')');
                }

                if (CurrentGroupIndex >= 0 && Data.Packages.Length > CurrentPackageIndex &&
                    Data.Packages[CurrentPackageIndex].Groups.Length > CurrentGroupIndex)
                {
                    TempBuilder.Append(" / ");
                    TempBuilder.Append(Data.Packages[CurrentPackageIndex].Groups[CurrentGroupIndex].Name);
                    if (!string.IsNullOrEmpty(Data.Packages[CurrentPackageIndex].Groups[CurrentGroupIndex]
                            .Description))
                    {
                        TempBuilder.Append('(');
                        TempBuilder.Append(Data.Packages[CurrentPackageIndex].Groups[CurrentGroupIndex]
                            .Description);
                        TempBuilder.Append(')');
                    }
                }
            }

            GUILayout.Label(TempBuilder.ToString(), GEStyle.toolbarbuttonRight, GP_Height_20, GP_Width_EXPAND);

#if SUPPORT_YOOASSET
            if (GUILayout.Button(GC_ToConvert_YooAsset, GEStyle.TEtoolbarbutton, GP_Width_30, GP_Height_20))
            {
                GUI.FocusControl(null);
                try
                {
                    ConvertYooAsset.Convert(Data);
                    EditorUtility.DisplayDialog("转换", "转换 YooAsset 成功", "确定");
                }
                catch (Exception e)
                {
                    EditorUtility.DisplayDialog("转换", "转换 YooAsset 失败\n" + e.Message, "确定");
                }
            }
#endif

            if (GUILayout.Button(GC_Select_ASConfig, GEStyle.TEtoolbarbutton, GP_Width_30, GP_Height_20))
            {
                GUI.FocusControl(null);
                Selection.activeObject = AssetCollectRoot.GetOrCreate();
            }

            if (GUILayout.Button(GC_SAVE, GEStyle.TEtoolbarbutton, GP_Width_30, GP_Height_20))
            {
                GUI.FocusControl(null);
                Data.Save();
                EditorUtility.DisplayDialog("保存", "保存成功", "确定");
            }
        }

        partial void OnDrawHeaderBuildMode();
    }
}