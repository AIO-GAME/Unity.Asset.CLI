using UnityEditor;
using UnityEngine;

namespace AIO.UEditor
{
    /// <summary>
    /// AssetCollectRoot
    /// </summary>
    [CustomEditor(typeof(AssetCollectRoot))]
    public class AssetCollectRootEditor : AFInspector<AssetCollectRoot>
    {
        protected override void OnGUI()
        {
            using (new EditorGUILayout.HorizontalScope(GEStyle.Toolbar))
            {
                EditorGUILayout.LabelField(string.Empty, GTOption.Width(100));
                if (GUILayout.Button("保存", GEStyle.toolbarbuttonRight))
                {
                    GUI.FocusControl(null);
                    Target.Save();
#if UNITY_2021_1_OR_NEWER
                    AssetDatabase.SaveAssetIfDirty(Target);
#else
                    AssetDatabase.SaveAssets();
#endif
                    if (EditorUtility.DisplayDialog("保存", "保存成功", "确定"))
                    {
                        AssetDatabase.Refresh();
                    }
                }

                if (GUILayout.Button("打开", GEStyle.toolbarbutton))
                {
                    AssetCollectWindow.WindowMode = AssetCollectWindow.Mode.Editor;
                    EditorApplication.ExecuteMenuItem("AIO/Window/Asset");
                }
            }

            using (new EditorGUILayout.HorizontalScope(GEStyle.Toolbar))
            {
                EditorGUILayout.LabelField("开启可寻址路径", GTOption.Width(100));
                if (GUILayout.Button(Target.EnableAddressable ? "已启用" : "已禁用", GEStyle.toolbarbutton))
                    Target.EnableAddressable = !Target.EnableAddressable;
            }

            using (new EditorGUILayout.HorizontalScope(GEStyle.Toolbar))
            {
                EditorGUILayout.LabelField("包含资源GUID", GTOption.Width(100));
                if (GUILayout.Button(Target.IncludeAssetGUID ? "已启用" : "已禁用", GEStyle.toolbarbutton))
                    Target.IncludeAssetGUID = !Target.IncludeAssetGUID;
            }

            using (new EditorGUILayout.HorizontalScope(GEStyle.Toolbar))
            {
                EditorGUILayout.LabelField("唯一资源包名", GTOption.Width(100));
                if (GUILayout.Button(Target.UniqueBundleName ? "已启用" : "已禁用", GEStyle.toolbarbutton))
                    Target.UniqueBundleName = !Target.UniqueBundleName;
            }

            using (new EditorGUILayout.HorizontalScope(GEStyle.Toolbar))
            {
                EditorGUILayout.LabelField("首包资源规则", GTOption.Width(100));
                Target.SequenceRecordPackRule =
                    GELayout.Popup(Target.SequenceRecordPackRule, GEStyle.TEToolbarDropDown);
            }
        }
    }
}