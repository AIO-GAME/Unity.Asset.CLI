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
                try
                {
                    GUI.FocusControl(null);
                    AssetProxyEditor.ConvertConfig(Data, false);
                    EditorUtility.DisplayDialog("转换", "转换 YooAsset 成功", "确定");
                }
                catch (Exception)
                {
                    // ignored
                }
            }

            if (GUILayout.Button(GC_SORT, GEStyle.TEtoolbarbutton, GP_Width_30, GP_Height_20))
            {
                Data.Sort();
            }

            if (GUILayout.Button(GC_MERGE, GEStyle.TEtoolbarbutton, GP_Width_30, GP_Height_20))
            {
                if (EditorUtility.DisplayDialog("合并", "确定合并当前资源包的所有组和收集器?", "确定", "取消"))
                {
                    Data.MergeCollector(Data.CurrentPackage.Name);
                }
            }

            if (GUILayout.Button(GC_REFRESH, GEStyle.TEtoolbarbutton, GP_Width_30, GP_Height_20))
            {
                Data.Refresh();
            }

            if (GUILayout.Button(GC_Select_ASConfig, GEStyle.TEtoolbarbutton, GP_Width_30, GP_Height_20))
            {
                GUI.FocusControl(null);
                Selection.activeObject = AssetCollectRoot.GetOrCreate();
            }

            if (GUILayout.Button(GC_SAVE, GEStyle.TEtoolbarbutton, GP_Width_30, GP_Height_20))
            {
                try
                {
                    GUI.FocusControl(null);
                    Data.Save();
#if UNITY_2021_1_OR_NEWER
                    AssetDatabase.SaveAssetIfDirty(Data);
#else
                    AssetDatabase.SaveAssets();
#endif
                    if (EditorUtility.DisplayDialog("保存", "保存成功", "确定"))
                    {
                        AssetDatabase.Refresh();
                    }
                }
                catch (Exception)
                {
                    // ignored
                }
            }
        }

        partial void OnDrawHeaderBuildMode();
    }
}