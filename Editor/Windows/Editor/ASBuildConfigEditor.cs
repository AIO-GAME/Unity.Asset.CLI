﻿using AIO.UEngine;
using UnityEditor;
using UnityEngine;

namespace AIO.UEditor
{
    /// <summary>
    /// ASBuildConfig
    /// </summary>
    [CustomEditor(typeof(ASBuildConfig))]
    public class ASBuildConfigEditor : AFInspector<ASBuildConfig>
    {
        protected override void OnGUI()
        {
            using (new EditorGUILayout.HorizontalScope(GEStyle.Toolbar))
            {
                EditorGUILayout.LabelField(string.Empty, GUILayout.Width(125));
                if (GUILayout.Button("保存", GEStyle.toolbarbuttonRight))
                {
                    EditorUtility.SetDirty(Target);
                    AssetDatabase.SaveAssets();
                }

                if (GUILayout.Button("打开", GEStyle.toolbarbuttonRight))
                {
                    AssetCollectWindow.WindowMode = AssetCollectWindow.Mode.Build;
                    EditorApplication.ExecuteMenuItem("AIO/Window/Asset");
                }

                if (GUILayout.Button("构建", GEStyle.toolbarbutton))
                {
                    AssetProxyEditor.BuildArt(Target);
                }
            }

            using (new EditorGUILayout.HorizontalScope(GEStyle.Toolbar))
            {
                EditorGUILayout.LabelField("构建平台", GUILayout.Width(125));
                Target.BuildTarget =
                    (BuildTarget)EditorGUILayout.EnumPopup(Target.BuildTarget, GEStyle.TEToolbarDropDown);
            }

            using (new EditorGUILayout.HorizontalScope(GEStyle.Toolbar))
            {
                EditorGUILayout.LabelField("打包管线", GUILayout.Width(125));
                Target.BuildPipeline =
                    (EBuildPipeline)EditorGUILayout.EnumPopup(Target.BuildPipeline, GEStyle.TEToolbarDropDown);
            }

            using (new EditorGUILayout.HorizontalScope(GEStyle.Toolbar))
            {
                EditorGUILayout.LabelField("构建模式", GUILayout.Width(125));
                Target.BuildMode = (EBuildMode)EditorGUILayout.EnumPopup(Target.BuildMode, GEStyle.TEToolbarDropDown);
            }

            using (new EditorGUILayout.HorizontalScope(GEStyle.Toolbar))
            {
                EditorGUILayout.LabelField("压缩模式", GUILayout.Width(125));
                Target.CompressedMode =
                    (ECompressMode)EditorGUILayout.EnumPopup(Target.CompressedMode, GEStyle.TEToolbarDropDown);
            }

            using (new EditorGUILayout.HorizontalScope(GEStyle.Toolbar))
            {
                EditorGUILayout.LabelField("版本号", GUILayout.Width(125));
                using (new EditorGUILayout.HorizontalScope(GEStyle.toolbarbutton))
                    Target.BuildVersion = EditorGUILayout.TextField(Target.BuildVersion);
            }

            using (new EditorGUILayout.HorizontalScope(GEStyle.Toolbar))
            {
                EditorGUILayout.LabelField("构建资源包名称", GUILayout.Width(125));
                using (new EditorGUILayout.HorizontalScope(GEStyle.toolbarbutton))
                    Target.PackageName = EditorGUILayout.TextField(Target.PackageName);
            }

            using (new EditorGUILayout.HorizontalScope(GEStyle.Toolbar))
            {
                EditorGUILayout.LabelField("加密模式", GUILayout.Width(125));
                using (new EditorGUILayout.HorizontalScope(GEStyle.toolbarbutton))
                    Target.EncyptionClassName = EditorGUILayout.TextField(Target.EncyptionClassName);
            }

            using (new EditorGUILayout.HorizontalScope(GEStyle.Toolbar))
            {
                EditorGUILayout.LabelField("首包标签集合", GUILayout.Width(125));
                using (new EditorGUILayout.HorizontalScope(GEStyle.toolbarbutton))
                    Target.FirstPackTag = EditorGUILayout.TextField(Target.FirstPackTag);
            }

            using (new EditorGUILayout.HorizontalScope(GEStyle.Toolbar))
            {
                EditorGUILayout.LabelField("构建结果输出路径", GUILayout.Width(125));
                using (new EditorGUILayout.HorizontalScope(GEStyle.toolbarbutton))
                    Target.BuildOutputPath = EditorGUILayout.TextField(Target.BuildOutputPath);
            }

            using (new EditorGUILayout.HorizontalScope(GEStyle.Toolbar))
            {
                EditorGUILayout.LabelField("验证构建结果", GUILayout.Width(125));
                if (GUILayout.Button(Target.ValidateBuild ? "已启用" : "已禁用", GEStyle.toolbarbutton))
                    Target.ValidateBuild = !Target.ValidateBuild;
            }

            using (new EditorGUILayout.HorizontalScope(GEStyle.Toolbar))
            {
                EditorGUILayout.LabelField("资源包合并至Latest", GUILayout.Width(125));
                if (GUILayout.Button(Target.MergeToLatest ? "已启用" : "已禁用", GEStyle.toolbarbutton))
                    Target.MergeToLatest = !Target.MergeToLatest;
            }

            using (new EditorGUILayout.HorizontalScope(GEStyle.Toolbar))
            {
                EditorGUILayout.LabelField("自动清理缓存数量", GUILayout.Width(125));
                using (new EditorGUILayout.HorizontalScope(GEStyle.toolbarbutton))
                    Target.AutoCleanCacheNum = EditorGUILayout.IntSlider(Target.AutoCleanCacheNum, 3, 10);
            }
        }
    }
}