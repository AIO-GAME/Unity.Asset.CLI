/*|============|*|
|*|Author:     |*| USER
|*|Date:       |*| 2024-01-22
|*|E-Mail:     |*| xinansky99@gmail.com
|*|============|*/

using UnityEditor;
using UnityEngine;

namespace AIO.UEditor
{
    /// <summary>
    /// AssetCollectRoot
    /// </summary>
    [CustomEditor(typeof(AssetCollectRoot))]
    public class AssetCollectRootEditor : NILInspector<AssetCollectRoot>
    {
        protected override void OnGUI()
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                Target.EnableAddressable = EditorGUILayout.ToggleLeft("开启可寻址路径", Target.EnableAddressable);
            }

            using (new EditorGUILayout.HorizontalScope())
            {
                Target.IncludeAssetGUID = EditorGUILayout.ToggleLeft("包含资源GUID", Target.IncludeAssetGUID);
            }

            using (new EditorGUILayout.HorizontalScope())
            {
                Target.UniqueBundleName = EditorGUILayout.ToggleLeft("唯一资源包名", Target.UniqueBundleName);
            }

            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("保存"))
                {
                    EditorUtility.SetDirty(Target);
                    AssetDatabase.SaveAssets();
                }

                if (GUILayout.Button("打开"))
                {
                    AssetCollectWindow.WindowMode = AssetCollectWindow.Mode.Editor;
                    EditorApplication.ExecuteMenuItem("AIO/Window/Asset");
                }
            }
        }
    }
}