using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using AIO.UEngine;
using UnityEditor;
using UnityEngine;

namespace AIO.UEditor
{
    public partial class AssetCollectWindow
    {
        private readonly StringBuilder _builder = new StringBuilder();

        /// <summary>
        ///     更新数据 资源构建模式
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

            LookModeDisplayPackages  = Data.Packages.Select(x => x.Name).ToArray();
            BuildConfig.BuildVersion = DateTime.Now.ToString("yyyy-MM-dd-HHmmss");
            Tags                     = Data.GetTags();
            CurrentTagIndex          = 0;
            foreach (var tag in Tags)
                if (BuildConfig.FirstPackTag.Contains(tag))
                    CurrentTagIndex |= 1 << Array.IndexOf(Tags, tag);

            Data.CurrentPackageIndex = BuildConfig.PackageName == null
                ? 0
                : Array.IndexOf(LookModeDisplayPackages, BuildConfig.PackageName);

            if (BuildConfig.BuildTarget == 0 ||
                BuildConfig.BuildTarget == BuildTarget.NoTarget
               ) BuildConfig.BuildTarget = EditorUserBuildSettings.activeBuildTarget;
        }

        /// <summary>
        ///     绘制 资源构建模式 导航栏
        /// </summary>
        partial void OnDrawHeaderBuildMode(Rect rect)
        {
            rect.x     = rect.width - 25;
            rect.width = 25;
            if (GUI.Button(rect, GC_SAVE, GEStyle.TEtoolbarbutton))
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
                    if (EditorUtility.DisplayDialog("保存", "保存成功", "确定")) AssetDatabase.Refresh();
                }
                catch (Exception)
                {
                    // ignored
                }
            }

            rect.x -= rect.width;
            if (GUI.Button(rect, GC_Select_ASConfig, GEStyle.TEtoolbarbutton))
            {
                GUI.FocusControl(null);
                Selection.activeObject = ASBuildConfig.GetOrCreate();
            }

#if SUPPORT_YOOASSET
            rect.x -= rect.width;
            if (GUI.Button(rect, GC_REPORT, GEStyle.TEtoolbarbutton))
            {
                GUI.FocusControl(null);
                EditorApplication.ExecuteMenuItem("YooAsset/AssetBundle Reporter");
            }
#endif

            rect.x = 20;
            if (Disk != null)
            {
                rect.width = 150;
                EditorGUI.LabelField(rect, $"磁盘剩余空间:{Disk.AvailableFreeSpace.ToConverseStringFileSize()}", GEStyle.HeaderLabel);
            }

            rect.x     += rect.width;
            rect.width =  150;
            EditorGUI.LabelField(rect, $"资源包数量:{Data.Packages.Length}", GEStyle.HeaderLabel);
        }

        /// <summary>
        ///     绘制 打包配置模式
        /// </summary>
        partial void OnDrawBuildMode(Rect rect)
        {
            var width = rect.width;

            #region 1

            rect.x      = 0;
            rect.y      = 0;
            rect.height = 20;

            EditorGUI.DrawRect(rect, TreeViewBasics.ColorLine);
            FoldoutBuildSetting =  EditorGUI.Foldout(rect, FoldoutBuildSetting, "构建设置", true);
            rect.y              += rect.height;
            if (FoldoutBuildSetting) OnDrawBuildBuild(rect, out rect);

            #endregion

            #region 2

            rect.height = 20;
            EditorGUI.DrawRect(rect, TreeViewBasics.ColorLine);

            rect.width       -= 20;
            FoldoutUploadFTP =  EditorGUI.Foldout(rect, FoldoutUploadFTP, "FTP", true);

            rect.x     += rect.width;
            rect.width =  20;
            if (GUI.Button(rect, EditorGUIUtility.TrTempContent("✚"))) BuildConfig.AddOrNewFTP();

            rect.width =  width;
            rect.y     += rect.height;
            rect.x     =  0;
            if (FoldoutUploadFTP) OnDrawBuildFTP(rect, out rect);

            #endregion

            #region 2

            rect.height = 20;
            EditorGUI.DrawRect(rect, TreeViewBasics.ColorLine);

            rect.width          -= 20;
            FoldoutUploadGCloud =  EditorGUI.Foldout(rect, FoldoutUploadGCloud, "Google Cloud", true);

            rect.x     += rect.width;
            rect.width =  20;
            if (GUI.Button(rect, EditorGUIUtility.TrTempContent("✚"))) BuildConfig.AddOrNewGCloud();

            rect.width =  width;
            rect.y     += rect.height;
            rect.x     =  0;
            if (FoldoutUploadGCloud) OnDrawBuildGCloud(rect, out rect);

            #endregion
        }

        /// <summary>
        ///     绘制 资源构建模式配置
        /// </summary>
        private void OnDrawBuildBuild(in Rect rect, out Rect outRect)
        {
            if (LookModeDisplayPackages is null || LookModeDisplayPackages.Length == 0)
            {
                outRect = rect;
                return;
            }

            #region 1

            var cell = new Rect(rect.width - 75, rect.y, 75, rect.height);
            if (GUI.Button(cell, "生成配置", GEStyle.toolbarbutton))
            {
                AssetProxyEditor.CreateConfig(BuildConfig.BuildOutputPath, BuildConfig.MergeToLatest, true);
            }

            cell.x -= cell.width;
            if (GUI.Button(cell, "清空缓存", GEStyle.toolbarbutton))
            {
                var sandbox = Path.Combine(BuildConfig.BuildOutputPath);
                if (Directory.Exists(sandbox)
                 && EditorUtility.DisplayDialog(
                        "清空缓存", "确定清空缓存?\n-----------------------\n清空缓存代表着\n之后每个资源包\n第一次构建必定是强制构建模式",
                        "确定", "取消")
                   ) AHelper.IO.DeleteDir(sandbox, SearchOption.AllDirectories, true);
            }

            cell.x -= cell.width;
            if (GUI.Button(cell, "打开目录", GEStyle.toolbarbutton))
            {
                GUI.FocusControl(null);
                if (!Directory.Exists(BuildConfig.BuildOutputPath))
                    Directory.CreateDirectory(BuildConfig.BuildOutputPath);
                PrPlatform.Open.Path(BuildConfig.BuildOutputPath).Async();
            }

            cell.x -= cell.width;
            if (GUI.Button(cell, "选择目录", GEStyle.toolbarbutton))
            {
                GUI.FocusControl(null);
                BuildConfig.BuildOutputPath = EditorUtility.OpenFolderPanel("请选择导出路径", BuildConfig.BuildOutputPath, "");
            }

            #endregion

            #region 2

            cell.y     += cell.height;
            cell.x     =  20;
            cell.width =  90;
            EditorGUI.LabelField(cell, "输出路径", GEStyle.HeaderLabel);

            cell.x     += cell.width;
            cell.width =  rect.width - cell.x - 300;
            EditorGUI.LabelField(cell, BuildConfig.BuildOutputPath, GEStyle.HeaderLabel);

            using (new EditorGUI.DisabledScope(Application.isPlaying))
            {
                cell.x     += cell.width;
                cell.width =  100;
                if (GUI.Button(cell, "构建指定资源", GEStyle.toolbarbutton))
                {
                    if (EditorUtility.DisplayDialog($"构建指定{BuildConfig.PackageName}资源包", $"构建{BuildConfig.PackageName}资源包", "确定", "取消"))
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

                cell.x += cell.width;
                if (GUI.Button(cell, "构建首包资源", GEStyle.toolbarbutton))
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

                cell.x += cell.width;
                if (GUI.Button(cell, "构建全部资源包", GEStyle.toolbarbutton))
                {
                    if (EditorUtility.DisplayDialog("构建全部资源包", $"构建\n {string.Join(",", Data.GetNames().Append(AssetSystem.TagsRecord))} 资源包?", "确定", "取消"))
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
            }

            #endregion

            #region 3

            cell.y     += cell.height;
            cell.x     =  20;
            cell.width =  90;
            EditorGUI.LabelField(cell, "构建选项", GEStyle.HeaderLabel);

            cell.x                    += cell.width;
            cell.width                =  120;
            BuildConfig.MergeToLatest =  EditorGUI.ToggleLeft(cell, "生成Latest版本", BuildConfig.MergeToLatest);

            cell.x                    += cell.width;
            cell.width                =  120;
            BuildConfig.ValidateBuild =  EditorGUI.ToggleLeft(cell, "验证构建结果", BuildConfig.ValidateBuild);

            #endregion

            #region 4

            cell.y     += cell.height;
            cell.x     =  20;
            cell.width =  90;
            EditorGUI.LabelField(cell, "构建版本", GEStyle.HeaderLabel);

            cell.x                   += cell.width;
            cell.width               =  rect.width - cell.x - 20;
            BuildConfig.BuildVersion =  EditorGUI.DelayedTextField(cell, BuildConfig.BuildVersion, GEStyle.ToolbarBoldLabel);

            cell.x     += cell.width;
            cell.width =  20;
            if (GUI.Button(cell, GC_REFRESH, GEStyle.toolbarbutton)) BuildConfig.BuildVersion = DateTime.Now.ToString("yyyy-MM-dd-HHmmss");

            #endregion

            #region 5

            cell.y     += cell.height;
            cell.x     =  20;
            cell.width =  90;
            EditorGUI.LabelField(cell, "构建平台", GEStyle.HeaderLabel);

            cell.x                  += cell.width;
            cell.width              =  rect.width - cell.x - 20;
            BuildConfig.BuildTarget =  (BuildTarget)EditorGUI.EnumPopup(cell, BuildConfig.BuildTarget, GEStyle.PreDropDown);

            cell.x     += cell.width;
            cell.width =  20;
            if (GUI.Button(cell, GC_REFRESH, GEStyle.toolbarbutton)) BuildConfig.BuildTarget = EditorUserBuildSettings.activeBuildTarget;

            #endregion

            #region 6

            cell.y     += cell.height;
            cell.x     =  20;
            cell.width =  90;
            EditorGUI.LabelField(cell, "构建包名", GEStyle.HeaderLabel);

            cell.x     += cell.width;
            cell.width =  rect.width - cell.x;
            if (BuildConfig.BuildFirstPackage)
                EditorGUI.LabelField(cell, AssetSystem.TagsRecord, GEStyle.PreDropDown);
            else
                Data.CurrentPackageIndex = EditorGUI.Popup(cell, Data.CurrentPackageIndex, LookModeDisplayPackages, GEStyle.PreDropDown);

            if (GUI.changed)
            {
                if (Data.Packages.Length <= Data.CurrentPackageIndex || Data.CurrentPackageIndex < 0)
                    Data.CurrentPackageIndex = 0;
                BuildConfig.PackageName = Data.Packages[Data.CurrentPackageIndex].Name;
            }

            #endregion

            #region 7

            cell.y     += cell.height;
            cell.x     =  20;
            cell.width =  90;
            EditorGUI.LabelField(cell, "构建管线", GEStyle.HeaderLabel);

            cell.x                    += cell.width;
            cell.width                =  rect.width - cell.x;
            BuildConfig.BuildPipeline =  (EBuildPipeline)EditorGUI.EnumPopup(cell, BuildConfig.BuildPipeline, GEStyle.PreDropDown);

            #endregion

            #region 8

            cell.y     += cell.height;
            cell.x     =  20;
            cell.width =  90;
            EditorGUI.LabelField(cell, "压缩模式", GEStyle.HeaderLabel);

            cell.x                     += cell.width;
            cell.width                 =  rect.width - cell.x;
            BuildConfig.CompressedMode =  (ECompressMode)EditorGUI.EnumPopup(cell, BuildConfig.CompressedMode, GEStyle.PreDropDown);

            #endregion

            #region 9

            cell.y     += cell.height;
            cell.x     =  20;
            cell.width =  90;
            EditorGUI.LabelField(cell, "构建模式", GEStyle.HeaderLabel);

            cell.x                += cell.width;
            cell.width            =  rect.width - cell.x;
            BuildConfig.BuildMode =  (EBuildMode)EditorGUI.EnumPopup(cell, BuildConfig.BuildMode, GEStyle.PreDropDown);

            #endregion

            #region 10

            if (!(Tags is null) && Tags.Length > 0)
            {
                if (Tags.Length > 31) // 位运算最大支持31位 2^31 显示警告
                {
                    cell.y     += cell.height;
                    cell.x     =  20;
                    cell.width =  90;
                    EditorGUI.LabelField(cell, "首包标签", GEStyle.HeaderLabel);

                    cell.x     += cell.width;
                    cell.width =  rect.width - cell.x;
                    EditorGUI.HelpBox(cell, "首包标签数量超过31个, 请减少标签数量", MessageType.Warning);
                }
                else
                {
                    cell.y     += cell.height;
                    cell.x     =  20;
                    cell.width =  90;
                    EditorGUI.LabelField(cell, "首包标签", GEStyle.HeaderLabel);

                    cell.x          += cell.width;
                    cell.width      =  rect.width - cell.x;
                    CurrentTagIndex =  EditorGUI.MaskField(cell, CurrentTagIndex, Tags, GEStyle.PreDropDown);

                    if (GUI.changed)
                    {
                        BuildConfig.FirstPackTag = string.Join(";", Tags.Where((t, i) => (CurrentTagIndex & (1 << i)) != 0));
                    }

                    #region 10

                    if (CurrentTagIndex != 0)
                    {
                        cell.y     += cell.height;
                        cell.x     =  20 + 90;
                        cell.width =  rect.width - cell.x;
                        EditorGUI.HelpBox(cell, BuildConfig.FirstPackTag, MessageType.None);
                    }

                    #endregion
                }
            }

            #endregion

            #region 11

            cell.y     += cell.height;
            cell.x     =  20;
            cell.width =  90;
            EditorGUI.LabelField(cell, "缓存清理数量", GEStyle.HeaderLabel);

            cell.x                           += cell.width;
            cell.width                       =  rect.width - cell.x;
            BuildConfig.AutoCleanCacheNumber =  EditorGUI.IntSlider(cell, BuildConfig.AutoCleanCacheNumber, 1, 20);

            #endregion

            outRect = new Rect(rect.x, cell.y + cell.height, rect.width, cell.height);
        }
    }
}