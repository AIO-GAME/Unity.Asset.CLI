﻿/*|============|*|
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

            TempBuilder.Clear();
            if (Data.IsPackageValid())
            {
                if (!ViewGroupList.IsShow &&
                    GUILayout.Button("⇘ Group", GEStyle.TEtoolbarbutton, GP_Width_75, GP_Height_20))
                {
                    GUI.FocusControl(null);
                    ViewGroupList.IsShow = true;
                }

                TempBuilder.Append(Data.CurrentPackage.Name);
                if (!string.IsNullOrEmpty(Data.CurrentPackage.Description))
                {
                    TempBuilder.Append('(');
                    TempBuilder.Append(Data.CurrentPackage.Description);
                    TempBuilder.Append(')');
                }
            }
            else
            {
                ViewGroupList.IsShow = false;
            }

            if (Data.IsGroupValid())
            {
                TempBuilder.Append(" / ");
                TempBuilder.Append(Data.CurrentGroup.Name);
                if (!string.IsNullOrEmpty(Data.CurrentGroup.Description))
                {
                    TempBuilder.Append('(');
                    TempBuilder.Append(Data.CurrentGroup.Description);
                    TempBuilder.Append(')');
                }
            }

            GUILayout.Label(TempBuilder.ToString(), GEStyle.toolbarbuttonRight, GP_Height_20, GP_Width_EXPAND);

            if (GUILayout.Button(GC_ToConvert, GEStyle.TEtoolbarbutton, GP_Width_30, GP_Height_20))
            {
                GUI.FocusControl(null);
                try
                {
                    AssetProxyEditor.ConvertConfig(Data, true);
                    EditorUtility.DisplayDialog("转换", "转换 YooAsset 成功", "确定");
                }
                catch (Exception e)
                {
                    EditorUtility.DisplayDialog("转换", "转换 YooAsset 失败\n" + e.Message, "确定");
                }
            }

            if (GUILayout.Button(GC_FOLDOUT, GEStyle.TEtoolbarbutton, GP_Width_30, GP_Height_20))
            {
                Data.FoldoutOff();
            }

            if (GUILayout.Button(GC_Select_ASConfig, GEStyle.TEtoolbarbutton, GP_Width_30, GP_Height_20))
            {
                GUI.FocusControl(null);
                Selection.activeObject = AssetCollectRoot.GetOrCreate();
            }

            if (GUILayout.Button(GC_SAVE, GEStyle.TEtoolbarbutton, GP_Width_30, GP_Height_20))
            {
                GUI.FocusControl(null);
                Data.Save();
                if (EditorUtility.DisplayDialog("保存", "保存成功", "确定"))
                {
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
#if UNITY_2020_1_OR_NEWER
                    AssetDatabase.SaveAssetIfDirty(Data);
#else
                    AssetDatabase.SaveAssets();
#endif
#endif
                }
            }
        }

        partial void OnDrawHeaderBuildMode();
    }
}