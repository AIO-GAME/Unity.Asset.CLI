﻿/*|============|*|
|*|Author:     |*| Star fire
|*|Date:       |*| 2023-12-01
|*|E-Mail:     |*| xinansky99@foxmail.com
|*|============|*/

using System;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;
#if SUPPORT_YOOASSET
using YooAsset.Editor;
#endif

namespace AIO.UEditor
{
    public partial class AssetCollectWindow
    {
        private StringBuilder _builder = new StringBuilder();

        /// <summary>
        /// 更新数据 资源构建模式
        /// </summary>
        private void UpdateDataBuildMode()
        {
            LookModeDisplayPackages = Data.Packages.Select(x => x.Name).ToArray();
            BuildConfig.BuildVersion = DateTime.Now.ToString("yyyy-MM-dd-HHmmss");
            Tags = Data.GetTags();
            CurrentTagIndex = 0;
            foreach (var tag in Tags)
            {
                if (BuildConfig.FirstPackTag.Contains(tag))
                {
                    CurrentTagIndex |= 1 << Array.IndexOf(Tags, tag);
                }
            }

            Data.CurrentPackageIndex = BuildConfig.PackageName == null
                ? 0
                : Array.IndexOf(LookModeDisplayPackages, BuildConfig.PackageName);

            if (BuildConfig.BuildTarget == 0 ||
                BuildConfig.BuildTarget == BuildTarget.NoTarget
               ) BuildConfig.BuildTarget = EditorUserBuildSettings.activeBuildTarget;
        }

        /// <summary>
        /// 绘制 资源构建模式 导航栏
        /// </summary>
        partial void OnDrawHeaderBuildMode()
        {
            EditorGUILayout.Separator();
            if (GUILayout.Button(GC_Select_ASConfig, GEStyle.TEtoolbarbutton, GP_Width_30, GP_Height_20))
            {
                GUI.FocusControl(null);
                Selection.activeObject = ASBuildConfig.GetOrCreate();
            }

            if (GUILayout.Button(GC_SAVE, GEStyle.TEtoolbarbutton, GP_Width_30, GP_Height_20))
            {
                GUI.FocusControl(null);
                BuildConfig.Save();
                EditorUtility.DisplayDialog("保存", "保存成功", "确定");
            }
        }

        /// <summary>
        /// 绘制 打包配置模式
        /// </summary>
        partial void OnDrawBuildMode()
        {
            FoldoutBuildSetting = GELayout.VFoldoutHeaderGroupWithHelp(OnDrawBuildBuild,
                "构建设置", FoldoutBuildSetting);

            FoldoutUploadFTP = GELayout.VFoldoutHeaderGroupWithHelp(
                OnDrawBuildFTP,
                "FTP",
                FoldoutUploadFTP,
                () => { BuildConfig.AddOrNewFTP(); }, 0, null, new GUIContent("✚"));

            FoldoutUploadGCloud = GELayout.VFoldoutHeaderGroupWithHelp(
                OnDrawBuildGCloud,
                "Google Cloud",
                FoldoutUploadGCloud,
                () => { BuildConfig.AddOrNewGCloud(); }, 0, null, new GUIContent("✚"));

            // FoldoutNoticeDingDing = GELayout.VFoldoutHeaderGroupWithHelp(OnDrawBuildNoticeDingDing,
            //     "钉钉通知", FoldoutNoticeDingDing);
        }

        /// <summary>
        /// 绘制 资源构建模式配置
        /// </summary>
        private void OnDrawBuildBuild()
        {
            if (LookModeDisplayPackages is null || LookModeDisplayPackages.Length == 0) return;
            using (new EditorGUILayout.VerticalScope(GEStyle.Badge))
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    EditorGUILayout.LabelField("输出路径", GTOption.Width(98));

                    if (string.IsNullOrEmpty(BuildConfig.BuildOutputPath) ||
                        Directory.GetParent(BuildConfig.BuildOutputPath) == null)
                    {
                        EditorGUILayout.Separator();
                        if (GUILayout.Button("选择目录", GEStyle.toolbarbutton, GP_Width_50))
                            BuildConfig.BuildOutputPath =
                                EditorUtility.OpenFolderPanel("请选择导出路径", BuildConfig.BuildOutputPath, "");
                        return;
                    }

                    BuildConfig.FirstPack = GELayout.ToggleLeft("首包", BuildConfig.FirstPack, GP_Width_50);
                    BuildConfig.ValidateBuild = GELayout.ToggleLeft("验证构建结果", BuildConfig.ValidateBuild, GP_Width_100);

                    EditorGUILayout.Separator();

                    if (GUILayout.Button("选择目录", GEStyle.toolbarbutton, GP_Width_75))
                        BuildConfig.BuildOutputPath =
                            EditorUtility.OpenFolderPanel("请选择导出路径", BuildConfig.BuildOutputPath, "");

                    if (GUILayout.Button("打开目录", GEStyle.toolbarbutton, GP_Width_75))
                    {
                        PrPlatform.Open.Path(BuildConfig.BuildOutputPath).Async();
                        return;
                    }

                    if (GUILayout.Button("清空缓存", GEStyle.toolbarbutton, GP_Width_75))
                    {
                        var sandbox = Path.Combine(EHelper.Path.Project, "Bundles");
                        if (Directory.Exists(sandbox))
                            AHelper.IO.DeleteFolder(sandbox, SearchOption.AllDirectories, true);
                        return;
                    }

#if SUPPORT_YOOASSET
                    if (GUILayout.Button("生成配置", GEStyle.toolbarbutton, GP_Width_75))
                    {
                        MenuItem_YooAssets.CreateConfig(BuildConfig.BuildOutputPath);
                        return;
                    }
#endif
                    if (Application.isPlaying) GUI.enabled = false;
                    if (GUILayout.Button("构建资源", GEStyle.toolbarbutton, GP_Width_75))
                    {
                        SaveScene();
#if SUPPORT_YOOASSET
                        ConvertYooAsset.Convert(Data);
                        var BuildCommand = new YooAssetBuildCommand
                        {
                            PackageVersion = BuildConfig.BuildVersion,
                            BuildPackage = BuildConfig.PackageName,
                            EncyptionClassName = "",
                            ActiveTarget = BuildConfig.BuildTarget,
                            BuildPipeline = BuildConfig.BuildPipeline,
                            OutputRoot = BuildConfig.BuildOutputPath,
                            BuildMode = BuildConfig.BuildMode,
                            CopyBuildinFileOption = ECopyBuildinFileOption.None,
                            CopyBuildinFileTags = string.Empty
                        };
                        YooAssetBuild.ArtBuild(BuildCommand);
                        MenuItem_YooAssets.CreateConfig(BuildConfig.BuildOutputPath);
                        BuildConfig.BuildVersion = DateTime.Now.ToString("yyyy-MM-dd-HHmmss");
#else
                        if (EditorUtility.DisplayDialogComplex("提示", "当前没有导入资源实现工具", "导入 YooAsset", "导入其他", "取消") == 0)
                        {
#if !SUPPORT_YOOASSET
                            Install.YooAssetRunAsync();
#endif
                        }
#endif
                        return;
                    }

                    if (Application.isPlaying) GUI.enabled = true;
                }

                using (new EditorGUILayout.HorizontalScope(GEStyle.ToolbarBottom))
                {
                    EditorGUILayout.LabelField(BuildConfig.BuildOutputPath, GEStyle.CenteredLabel);
                }

                using (new EditorGUILayout.HorizontalScope(GEStyle.ToolbarBottom))
                {
                    EditorGUILayout.LabelField("构建版本", GP_Width_100, GP_Height_20);
                    BuildConfig.BuildVersion = GELayout.Field(BuildConfig.BuildVersion, GP_Height_20);
                    if (GUILayout.Button(GC_REFRESH, GEStyle.toolbarbutton, GP_Width_20, GP_Height_20))
                    {
                        BuildConfig.BuildVersion = DateTime.Now.ToString("yyyy-MM-dd-HHmmss");
                    }
                }

                using (new EditorGUILayout.HorizontalScope(GEStyle.ToolbarBottom))
                {
                    EditorGUILayout.LabelField("构建平台", GP_Width_100);
                    BuildConfig.BuildTarget = GELayout.Popup(BuildConfig.BuildTarget, GEStyle.PreDropDown);
                    if (GUILayout.Button(GC_REFRESH, GEStyle.toolbarbutton, GP_Width_20, GP_Height_20))
                    {
                        BuildConfig.BuildTarget = EditorUserBuildSettings.activeBuildTarget;
                    }
                }

                using (new EditorGUILayout.HorizontalScope(GEStyle.ToolbarBottom))
                {
                    EditorGUILayout.LabelField("构建包名", GP_Width_100);
                    Data.CurrentPackageIndex = EditorGUILayout.Popup(Data.CurrentPackageIndex, LookModeDisplayPackages,
                        GEStyle.PreDropDown);
                    if (GUI.changed)
                    {
                        if (Data.Packages.Length <= Data.CurrentPackageIndex || Data.CurrentPackageIndex < 0)
                        {
                            Data.CurrentPackageIndex = 0;
                        }

                        BuildConfig.PackageName = Data.Packages[Data.CurrentPackageIndex].Name;
                    }
                }

                using (new EditorGUILayout.HorizontalScope(GEStyle.ToolbarBottom))
                {
                    EditorGUILayout.LabelField("构建管线", GP_Width_100);
                    BuildConfig.BuildPipeline = GELayout.Popup(BuildConfig.BuildPipeline, GEStyle.PreDropDown);
                }

                using (new EditorGUILayout.HorizontalScope(GEStyle.ToolbarBottom))
                {
                    EditorGUILayout.LabelField("构建模式", GP_Width_100);
                    BuildConfig.BuildMode = GELayout.Popup(BuildConfig.BuildMode, GEStyle.PreDropDown);
                }

                if (Tags != null && Tags.Length > 0)
                {
                    using (new EditorGUILayout.HorizontalScope(GEStyle.ToolbarBottom))
                    {
                        EditorGUILayout.LabelField("首包标签", GP_Width_100);
                        CurrentTagIndex = EditorGUILayout.MaskField(CurrentTagIndex, Tags, GEStyle.PreDropDown);
                    }

                    if (GUI.changed)
                    {
                        BuildConfig.FirstPackTag = string.Empty;
                        for (var i = 0; i < Tags.Length; i++)
                        {
                            if ((CurrentTagIndex & (1 << i)) != 0)
                            {
                                BuildConfig.FirstPackTag += string.Concat(Tags[i], ";");
                            }
                        }
                    }

                    if (CurrentTagIndex != 0) GELayout.HelpBox(BuildConfig.FirstPackTag);
                }
            }
        }

        private void OnDrawBuildNoticeDingDing()
        {
            // 钉钉 WebHook
            // 钉钉 Secret(请选择加签方式 内容过滤可能导致消息丢失)
            // 钉钉 通知事件类型列表
        }
    }
}