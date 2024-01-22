/*|============|*|
|*|Author:     |*| star fire
|*|Date:       |*| 2024-01-22
|*|E-Mail:     |*| xinansky99@gmail.com
|*|============|*/

using AIO.UEngine;
using UnityEditor;
using UnityEngine;

namespace AIO.UEditor
{
    /// <summary>
    /// ASBuildConfig
    /// </summary>
    [CustomEditor(typeof(ASBuildConfig))]
    public class ASBuildConfigEditor : NILInspector<ASBuildConfig>
    {
        protected override void OnGUI()
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.LabelField("构建平台", GUILayout.Width(120));
                Target.BuildTarget = (BuildTarget)EditorGUILayout.EnumPopup(Target.BuildTarget);
            }

            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.LabelField("打包管线", GUILayout.Width(120));
                Target.BuildPipeline = (EBuildPipeline)EditorGUILayout.EnumPopup(Target.BuildPipeline);
            }

            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.LabelField("构建模式", GUILayout.Width(120));
                Target.BuildMode = (EBuildMode)EditorGUILayout.EnumPopup(Target.BuildMode);
            }

            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.LabelField("压缩模式", GUILayout.Width(120));
                Target.CompressedMode = (ECompressMode)EditorGUILayout.EnumPopup(Target.CompressedMode);
            }

            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.LabelField("版本号", GUILayout.Width(120));
                Target.BuildVersion = EditorGUILayout.TextField(Target.BuildVersion);
            }

            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.LabelField("构建资源包名称", GUILayout.Width(120));
                Target.PackageName = EditorGUILayout.TextField(Target.PackageName);
            }

            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.LabelField("加密模式", GUILayout.Width(120));
                Target.EncyptionClassName = EditorGUILayout.TextField(Target.EncyptionClassName);
            }

            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.LabelField("首包标签集合", GUILayout.Width(120));
                Target.FirstPackTag = EditorGUILayout.TextField(Target.FirstPackTag);
            }

            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.LabelField("构建结果输出路径", GUILayout.Width(120));
                Target.BuildOutputPath = EditorGUILayout.TextField(Target.BuildOutputPath);
            }

            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.LabelField("验证构建结果", GUILayout.Width(120));
                Target.ValidateBuild = EditorGUILayout.Toggle(Target.ValidateBuild);
            }

            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.LabelField("资源包合并至Latest", GUILayout.Width(120));
                Target.MergeToLatest = EditorGUILayout.Toggle(Target.MergeToLatest);
            }

            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.LabelField("自动清理缓存数量", GUILayout.Width(120));
                Target.AutoCleanCacheNum = EditorGUILayout.IntSlider(Target.AutoCleanCacheNum, 3, 10);
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
                    AssetCollectWindow.WindowMode = AssetCollectWindow.Mode.Build;
                    EditorApplication.ExecuteMenuItem("AIO/Window/Asset");
                }

                if (GUILayout.Button("构建"))
                {
                    AssetProxyEditor.BuildArt(Target);
                }
            }
        }
    }
}