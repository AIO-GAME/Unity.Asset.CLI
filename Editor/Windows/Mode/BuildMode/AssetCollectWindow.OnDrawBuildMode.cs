/*|============|*|
|*|Author:     |*| Star fire
|*|Date:       |*| 2023-12-01
|*|E-Mail:     |*| xinansky99@foxmail.com
|*|============|*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

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
            // 获取当前文件磁盘剩余空间
            try
            {
                Disk = new DriveInfo(EHelper.Path.Project);
            }
            catch (Exception)
            {
                // ignored
            }

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
            if (Disk != null)
            {
                EditorGUILayout.LabelField($"     磁盘剩余空间:{Disk.AvailableFreeSpace.ToConverseStringFileSize()}",
                    GEStyle.HeaderLabel, GP_Width_160);
            }

            EditorGUILayout.LabelField($"资源包数量:{Data.Packages.Length}", GEStyle.HeaderLabel, GP_Width_150);
            EditorGUILayout.Separator();
#if SUPPORT_YOOASSET
            if (GUILayout.Button(GC_REPORT, GEStyle.TEtoolbarbutton, GP_Width_30, GP_Height_20))
            {
                GUI.FocusControl(null);
                EditorApplication.ExecuteMenuItem("YooAsset/AssetBundle Reporter");
            }
#endif

            if (GUILayout.Button(GC_Select_ASConfig, GEStyle.TEtoolbarbutton, GP_Width_30, GP_Height_20))
            {
                GUI.FocusControl(null);
                Selection.activeObject = ASBuildConfig.GetOrCreate();
            }

            if (GUILayout.Button(GC_SAVE, GEStyle.TEtoolbarbutton, GP_Width_30, GP_Height_20))
            {
                try
                {
                    GUI.FocusControl(null);
                    BuildConfig.Save();
#if UNITY_2021_1_OR_NEWER
                    AssetDatabase.SaveAssetIfDirty(BuildConfig);
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
                    }

                    BuildConfig.ValidateBuild = GELayout.ToggleLeft("验证构建结果", BuildConfig.ValidateBuild, GP_Width_100);
                    BuildConfig.MergeToLatest =
                        GELayout.ToggleLeft("生成Latest版本", BuildConfig.MergeToLatest, GP_Width_100);
                    // BuildConfig.BuildFirstPackage =
                    //     GELayout.ToggleLeft("构建首包资源", BuildConfig.BuildFirstPackage, GP_Width_100);

                    EditorGUILayout.Separator();

                    if (GUILayout.Button("选择目录", GEStyle.toolbarbutton, GP_Width_75))
                    {
                        GUI.FocusControl(null);
                        BuildConfig.BuildOutputPath = EditorUtility.OpenFolderPanel(
                            "请选择导出路径", BuildConfig.BuildOutputPath, "");
                    }

                    if (GUILayout.Button("打开目录", GEStyle.toolbarbutton, GP_Width_75))
                    {
                        GUI.FocusControl(null);
                        if (!Directory.Exists(BuildConfig.BuildOutputPath))
                            Directory.CreateDirectory(BuildConfig.BuildOutputPath);
                        PrPlatform.Open.Path(BuildConfig.BuildOutputPath).Async();
                    }

                    if (GUILayout.Button("清空缓存", GEStyle.toolbarbutton, GP_Width_75))
                    {
                        var sandbox = Path.Combine(BuildConfig.BuildOutputPath);
                        if (Directory.Exists(sandbox))
                        {
                            if (EditorUtility.DisplayDialog("清空缓存",
                                    $"确定清空缓存?\n-----------------------\n清空缓存代表着\n之后每个资源包\n第一次构建必定是强制构建模式", "确定", "取消"))
                            {
                                AHelper.IO.DeleteDir(sandbox, SearchOption.AllDirectories, true);
                            }
                        }
                    }

                    if (GUILayout.Button("生成配置", GEStyle.toolbarbutton, GP_Width_75))
                    {
                        AssetProxyEditor.CreateConfig(BuildConfig.BuildOutputPath, BuildConfig.MergeToLatest, true);
                    }
                }

                using (new EditorGUILayout.HorizontalScope(GEStyle.ToolbarBottom))
                {
                    EditorGUILayout.LabelField(BuildConfig.BuildOutputPath, GEStyle.CenteredLabel);

                    if (Application.isPlaying) GUI.enabled = false;
                    if (GUILayout.Button("构建指定资源", GEStyle.toolbarbutton, GP_Width_100))
                    {
                        if (EditorUtility.DisplayDialog($"构建指定{BuildConfig.PackageName}资源包",
                                $"构建{BuildConfig.PackageName}资源包", "确定", "取消"))
                        {
                            try
                            {
                                AssetProxyEditor.ConvertConfig(Data, false);
                                BuildConfig.BuildFirstPackage = false;
                                AssetProxyEditor.BuildArt(BuildConfig, true);
                                BuildConfig.BuildVersion = DateTime.Now.ToString("yyyy-MM-dd-HHmmss");
                            }
                            catch (Exception e)
                            {
                                BuildConfig.BuildVersion = DateTime.Now.ToString("yyyy-MM-dd-HHmmss");
                                Debug.LogError(e);
                            }
                        }
                    }

                    if (GUILayout.Button("构建首包资源", GEStyle.toolbarbutton, GP_Width_100))
                    {
                        if (EditorUtility.DisplayDialog("构建首包资源包", "构建序列纪录资源包", "确定", "取消"))
                        {
                            try
                            {
                                AssetProxyEditor.ConvertConfig(Data, false);
                                BuildConfig.BuildFirstPackage = true;
                                AssetProxyEditor.BuildArt(BuildConfig, true);
                                BuildConfig.BuildVersion = DateTime.Now.ToString("yyyy-MM-dd-HHmmss");
                            }
                            catch (Exception e)
                            {
                                BuildConfig.BuildVersion = DateTime.Now.ToString("yyyy-MM-dd-HHmmss");
                                Debug.LogError(e);
                            }
                        }
                    }

                    if (GUILayout.Button("构建全部资源包", GEStyle.toolbarbutton, GP_Width_100))
                    {
                        var temp = new List<string>(Data.GetPackageNames());
                        temp.Add(AssetSystem.TagsRecord);
                        if (EditorUtility.DisplayDialog("构建全部资源包", $"构建\n {string.Join(",", temp.ToArray())} 资源包?",
                                "确定", "取消"))
                        {
                            try
                            {
                                AssetProxyEditor.ConvertConfig(Data, false);
                                BuildConfig.BuildFirstPackage = true;
                                AssetProxyEditor.BuildArtAll(BuildConfig, true);
                                BuildConfig.BuildVersion = DateTime.Now.ToString("yyyy-MM-dd-HHmmss");
                            }
                            catch (Exception e)
                            {
                                BuildConfig.BuildVersion = DateTime.Now.ToString("yyyy-MM-dd-HHmmss");
                                Debug.LogError(e);
                            }
                        }
                    }

                    if (Application.isPlaying) GUI.enabled = true;
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
                    if (BuildConfig.BuildFirstPackage)
                        GUILayout.Label(AssetSystem.TagsRecord, GEStyle.PreDropDown);
                    else
                        Data.CurrentPackageIndex = EditorGUILayout.Popup(
                            Data.CurrentPackageIndex, LookModeDisplayPackages, GEStyle.PreDropDown);

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
                    EditorGUILayout.LabelField("压缩模式", GP_Width_100);
                    BuildConfig.CompressedMode = GELayout.Popup(BuildConfig.CompressedMode,
                        GEStyle.PreDropDown);
                }

                using (new EditorGUILayout.HorizontalScope(GEStyle.ToolbarBottom))
                {
                    EditorGUILayout.LabelField("构建模式", GP_Width_100);
                    BuildConfig.BuildMode = GELayout.Popup(BuildConfig.BuildMode, GEStyle.PreDropDown);
                }

                if (!(Tags is null) && Tags.Length > 0)
                {
                    if (Tags.Length > 31)
                    {
                        // 位运算最大支持31位 2^31 显示警告
                        using (new EditorGUILayout.HorizontalScope(GEStyle.ToolbarBottom))
                        {
                            EditorGUILayout.LabelField("首包标签", GP_Width_100);
                            EditorGUILayout.HelpBox("首包标签数量超过31个, 请减少标签数量", MessageType.Warning);
                        }
                    }
                    else
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

                using (new EditorGUILayout.HorizontalScope(GEStyle.ToolbarBottom))
                {
                    EditorGUILayout.LabelField("缓存清理数量", GP_Width_100);
                    BuildConfig.AutoCleanCacheNum = GELayout.Slider(BuildConfig.AutoCleanCacheNum, 1, 20);
                }
            }
        }
    }
}