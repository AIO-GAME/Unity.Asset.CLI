using System;
using System.IO;
using System.Linq;
using AIO.UEngine;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace AIO.UEditor
{
    public class TreeViewItemBuildSetting : TreeViewItem, ITVItemDraw
    {
        private AssetBuildConfig BuildConfig;
        private AssetCollectRoot Data;
        private string[]         PackageNames;
        private string[]         Tags;
        private int              CurrentTagIndex;

        public TreeViewItemBuildSetting(int id, string displayName, AssetBuildConfig config) : base(id, 1, displayName)
        {
            if (!config) return;
            BuildConfig  = config;
            Data         = AssetCollectRoot.GetOrCreate();
            PackageNames = Data.GetNames();
            Tags         = Data.Tags;
            if (Tags is null || Tags.Length == 0)
            {
                CurrentTagIndex = 0;
                return;
            }

            CurrentTagIndex = Tags is null ? 0 : Tags.Select((t, i) => BuildConfig.FirstPackTag?.Contains(t) ?? false ? 1 << i : 0).Aggregate((a, b) => a | b);
        }

        public bool  AllowChangeExpandedState             => false;
        public bool  AllowRename                          => false;
        public float GetHeight()                          => 20;
        public bool  MatchSearch(string search)           => false;
        public Rect  GetRenameRect(Rect rowRect, int row) => rowRect;

        public void OnDraw(Rect cell, int col, ref RowGUIArgs args)
        {
            switch (col)
            {
                case 0:
                    cell.x     += 10;
                    cell.width -= 10;
                    TreeView.DefaultGUI.BoldLabel(cell, displayName, args.selected, args.focused);

                    cell.x     += cell.width - 1;
                    cell.width =  1;
                    EditorGUI.DrawRect(cell, TreeViewBasics.ColorLine);
                    break;
                case 1:
                    DrawContent(cell, ref args);
                    break;
            }
        }

        private void DrawContent(Rect rect, ref RowGUIArgs args)
        {
            var cell = new Rect(rect.x, rect.y, rect.width, rect.height);
            switch (args.row)
            {
                case 0:
                {
                    cell.width = 100;
                    cell.x     = rect.width - cell.width + rect.x;
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
                        EditorUtility.RevealInFinder(BuildConfig.BuildOutputPath);
                    }

                    cell.x -= cell.width;
                    if (GUI.Button(cell, "选择目录", GEStyle.toolbarbutton))
                    {
                        GUI.FocusControl(null);
                        BuildConfig.BuildOutputPath = EditorUtility.OpenFolderPanel("请选择导出路径", BuildConfig.BuildOutputPath, "");
                    }

                    break;
                }

                case 1:
                {
                    cell.width = rect.width - cell.x - 300 + rect.x;
                    if (!string.IsNullOrEmpty(BuildConfig.BuildOutputPath))
                    {
                        if (GUI.Button(cell, BuildConfig.BuildOutputPath, GEStyle.HeaderLabel))
                        {
                            GUI.FocusControl(null);
                            if (!Directory.Exists(BuildConfig.BuildOutputPath))
                                Directory.CreateDirectory(BuildConfig.BuildOutputPath);
                            EditorUtility.RevealInFinder(BuildConfig.BuildOutputPath);
                        }
                    }

                    using (new EditorGUI.DisabledScope(Application.isPlaying))
                    {
                        cell.x     += cell.width;
                        cell.width =  100;
                        if (GUI.Button(cell, "构建指定资源", GEStyle.toolbarbutton))
                        {
                            if (EditorUtility.DisplayDialog($"构建指定{BuildConfig.PackageName}资源包",
                                                            $"构建{BuildConfig.PackageName}资源包",
                                                            "确定", "取消"))
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
                                    AssetProxyEditor.ConvertConfig(AssetCollectRoot.GetOrCreate(), false);
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
                            if (EditorUtility.DisplayDialog("构建全部资源包",
                                                            $"构建\n {string.Join(",", Data.GetNames().Append(AssetSystem.TagsRecord))} 资源包?", "确定",
                                                            "取消"))
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

                    break;
                }

                case 2:
                {
                    if (GUI.Button(rect, BuildConfig.MergeToLatest ? "已启用" : "已禁用", GEStyle.toolbarbutton))
                        BuildConfig.MergeToLatest = !BuildConfig.MergeToLatest;

                    break;
                }

                case 3:
                {
                    if (GUI.Button(rect, BuildConfig.ValidateBuild ? "已启用" : "已禁用", GEStyle.toolbarbutton))
                        BuildConfig.ValidateBuild = !BuildConfig.ValidateBuild;

                    break;
                }

                case 4:
                {
                    cell.width               = rect.width - 20;
                    BuildConfig.BuildVersion = EditorGUI.DelayedTextField(cell, BuildConfig.BuildVersion, GEStyle.ToolbarBoldLabel);

                    cell.x     += cell.width;
                    cell.width =  20;
                    if (GUI.Button(cell, EditorGUIUtility.IconContent("Refresh").SetTooltips("刷新"), GEStyle.toolbarbutton))
                        BuildConfig.BuildVersion = DateTime.Now.ToString("yyyy-MM-dd-HHmmss");
                    break;
                }

                case 5:
                {
                    cell.width              = rect.width - 20;
                    BuildConfig.BuildTarget = (BuildTarget)EditorGUI.EnumPopup(cell, BuildConfig.BuildTarget, GEStyle.PreDropDown);

                    cell.x     += cell.width;
                    cell.width =  20;
                    if (GUI.Button(cell, EditorGUIUtility.IconContent("Refresh").SetTooltips("刷新"), GEStyle.toolbarbutton))
                        BuildConfig.BuildTarget = EditorUserBuildSettings.activeBuildTarget;
                    break;
                }

                case 6:
                    cell.width = rect.width;
                    if (BuildConfig.BuildFirstPackage)
                        EditorGUI.LabelField(cell, AssetSystem.TagsRecord, GEStyle.PreDropDown);
                    else
                        Data.CurrentPackageIndex = EditorGUI.Popup(cell, Data.CurrentPackageIndex, PackageNames, GEStyle.PreDropDown);

                    if (GUI.changed)
                    {
                        if (Data.Packages.Length <= Data.CurrentPackageIndex || Data.CurrentPackageIndex < 0)
                            Data.CurrentPackageIndex = 0;
                        BuildConfig.PackageName = Data.Packages[Data.CurrentPackageIndex].Name;
                    }

                    break;

                case 7:
                    cell.width                = rect.width;
                    BuildConfig.BuildPipeline = (EBuildPipeline)EditorGUI.EnumPopup(cell, BuildConfig.BuildPipeline, GEStyle.PreDropDown);
                    break;

                case 8:
                    cell.width                 = rect.width;
                    BuildConfig.CompressedMode = (ECompressMode)EditorGUI.EnumPopup(cell, BuildConfig.CompressedMode, GEStyle.PreDropDown);
                    break;

                case 9:
                    cell.width            = rect.width;
                    BuildConfig.BuildMode = (EBuildMode)EditorGUI.EnumPopup(cell, BuildConfig.BuildMode, GEStyle.PreDropDown);
                    break;

                case 10:
                    if (!(Tags is null) && Tags.Length > 0)
                    {
                        if (Tags.Length > 31) // 位运算最大支持31位 2^31 显示警告
                        {
                            cell.width = rect.width;
                            EditorGUI.HelpBox(cell, "首包标签数量超过31个, 请减少标签数量", MessageType.Warning);
                        }
                        else
                        {
                            cell.width      = CurrentTagIndex == 0 ? rect.width : 200;
                            CurrentTagIndex = EditorGUI.MaskField(cell, CurrentTagIndex, Tags, GEStyle.PreDropDown);
                            if (GUI.changed) BuildConfig.FirstPackTag = string.Join(";", Tags.Where((t, i) => (CurrentTagIndex & (1 << i)) != 0));

                            if (CurrentTagIndex != 0)
                            {
                                cell.x     += 200;
                                cell.width =  rect.width - 200;
                                EditorGUI.HelpBox(cell, BuildConfig.FirstPackTag, MessageType.None);
                            }
                        }
                    }

                    break;

                case 11:
                    cell.width                       = rect.width;
                    BuildConfig.AutoCleanCacheNumber = EditorGUI.IntSlider(cell, BuildConfig.AutoCleanCacheNumber, 1, 20);
                    break;
            }
        }
    }
}