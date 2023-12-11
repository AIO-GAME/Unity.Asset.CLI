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
        private void OnDrawHeaderEditor()
        {
            if (!ShowSetting && GUILayout.Button("⇘ Setting", GEStyle.TEtoolbarbutton, GP_Width_75, GP_Height_20))
            {
                GUI.FocusControl(null);
                ShowSetting = true;
            }

            if (!ShowPackage && GUILayout.Button("⇘ Package", GEStyle.TEtoolbarbutton, GP_Width_75, GP_Height_20))
            {
                GUI.FocusControl(null);
                ShowPackage = true;
            }

            if (Data.Packages.Length <= CurrentPackageIndex || CurrentPackageIndex < 0 ||
                Data.Packages[CurrentPackageIndex] is null)
            {
                GUI.FocusControl(null);
                ShowGroup = false;
            }
            else if (!ShowGroup && GUILayout.Button("⇘ Group", GEStyle.TEtoolbarbutton, GP_Width_75, GP_Height_20))
            {
                GUI.FocusControl(null);
                ShowGroup = true;
            }

            EditorGUILayout.Separator();
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

            GUILayout.Label(TempBuilder.ToString(), GEStyle.flownodetitlebar, GP_Height_20);
            EditorGUILayout.Separator();

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
                Selection.activeObject = ASConfig.GetOrCreate();
            }

            if (GUILayout.Button(GC_SAVE, GEStyle.TEtoolbarbutton, GP_Width_30, GP_Height_20))
            {
                GUI.FocusControl(null);
                Data.Save();
                EditorUtility.DisplayDialog("保存", "保存成功", "确定");
            }
        }

        partial void OnDrawHeaderBuild();

        partial void OnDrawHeader()
        {
            using (GELayout.VHorizontal())
            {
                switch (WindowMode)
                {
                    case Mode.Editor:
                        OnDrawHeaderEditor();
                        break;
                    case Mode.Look:
                        OnDrawHeaderLook();
                        break;
                    case Mode.Build:
                        OnDrawHeaderBuild();
                        break;
                }

                WindowMode = GELayout.Popup(WindowMode, GEStyle.PreDropDown, GP_Width_75, GP_Height_20);

                if (GUI.changed)
                {
                    if (WindowMode != TempTable.GetOrDefault<Mode>(nameof(WindowMode)))
                    {
                        GUI.FocusControl(null);
                        switch (WindowMode)
                        {
                            default:
                            case Mode.Editor:
                                UpdateDataRecordQueue();
                                break;
                            case Mode.Look:
                                UpdateDataLook();
                                break;
                            case Mode.Build:
                                UpdateDataBuild();
                                break;
                        }

                        TempTable[nameof(WindowMode)] = WindowMode;
                    }
                }
            }
        }
    }
}