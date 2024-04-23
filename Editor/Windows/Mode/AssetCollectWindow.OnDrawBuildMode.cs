using System;
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

            LookModeDisplayPackages = Data.Packages.Select(x => x.Name).ToArray();
            BuildConfig.BuildVersion = DateTime.Now.ToString("yyyy-MM-dd-HHmmss");
            Tags = Data.GetTags();
            CurrentTagIndex = 0;
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
            rect.x = rect.width - 25;
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
                EditorGUI.LabelField(rect, $"磁盘剩余空间:{Disk.AvailableFreeSpace.ToConverseStringFileSize()}",
                    GEStyle.HeaderLabel);
            }

            rect.x += rect.width;
            rect.width = 150;
            EditorGUI.LabelField(rect, $"资源包数量:{Data.Packages.Length}", GEStyle.HeaderLabel);
        }

        #region Build

        /// <summary>
        ///     绘制 打包配置模式
        /// </summary>
        partial void OnDrawBuildMode(Rect rect)
        {
            var width = rect.width;

            #region 1

            rect.x = 0;
            rect.y = 0;
            rect.height = 20;

            EditorGUI.DrawRect(rect, TreeViewBasics.ColorLine);
            FoldoutBuildSetting = EditorGUI.Foldout(rect, FoldoutBuildSetting, "构建设置", true);
            rect.y += rect.height;
            if (FoldoutBuildSetting) OnDrawBuildBuild(rect, out rect);

            #endregion

            #region 2

            rect.height = 20;
            EditorGUI.DrawRect(rect, TreeViewBasics.ColorLine);

            rect.width -= 20;
            FoldoutUploadFTP = EditorGUI.Foldout(rect, FoldoutUploadFTP, "FTP", true);

            rect.x += rect.width;
            rect.width = 20;
            if (GUI.Button(rect, EditorGUIUtility.TrTempContent("✚"))) BuildConfig.AddOrNewFTP();

            rect.width = width;
            rect.y += rect.height;
            rect.x = 0;
            if (FoldoutUploadFTP) OnDrawBuildFTP(rect, out rect);

            #endregion

            #region 2

            rect.height = 20;
            EditorGUI.DrawRect(rect, TreeViewBasics.ColorLine);

            rect.width -= 20;
            FoldoutUploadGCloud = EditorGUI.Foldout(rect, FoldoutUploadGCloud, "Google Cloud", true);

            rect.x += rect.width;
            rect.width = 20;
            if (GUI.Button(rect, EditorGUIUtility.TrTempContent("✚"))) BuildConfig.AddOrNewGCloud();

            rect.width = width;
            rect.y += rect.height;
            rect.x = 0;
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

            cell.y += cell.height;
            cell.x = 20;
            cell.width = 90;
            EditorGUI.LabelField(cell, "输出路径", GEStyle.HeaderLabel);

            cell.x += cell.width;
            cell.width = rect.width - cell.x - 300;
            EditorGUI.LabelField(cell, BuildConfig.BuildOutputPath, GEStyle.HeaderLabel);

            using (new EditorGUI.DisabledScope(Application.isPlaying))
            {
                cell.x += cell.width;
                cell.width = 100;
                if (GUI.Button(cell, "构建指定资源", GEStyle.toolbarbutton))
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

            #endregion

            #region 3

            cell.y += cell.height;
            cell.x = 20;
            cell.width = 90;
            EditorGUI.LabelField(cell, "构建选项", GEStyle.HeaderLabel);

            cell.x += cell.width;
            cell.width = 120;
            BuildConfig.MergeToLatest = EditorGUI.ToggleLeft(cell, "生成Latest版本", BuildConfig.MergeToLatest);

            cell.x += cell.width;
            cell.width = 120;
            BuildConfig.ValidateBuild = EditorGUI.ToggleLeft(cell, "验证构建结果", BuildConfig.ValidateBuild);

            #endregion

            #region 4

            cell.y += cell.height;
            cell.x = 20;
            cell.width = 90;
            EditorGUI.LabelField(cell, "构建版本", GEStyle.HeaderLabel);

            cell.x += cell.width;
            cell.width = rect.width - cell.x - 20;
            BuildConfig.BuildVersion =
                EditorGUI.DelayedTextField(cell, BuildConfig.BuildVersion, GEStyle.ToolbarBoldLabel);

            cell.x += cell.width;
            cell.width = 20;
            if (GUI.Button(cell, GC_REFRESH, GEStyle.toolbarbutton))
                BuildConfig.BuildVersion = DateTime.Now.ToString("yyyy-MM-dd-HHmmss");

            #endregion

            #region 5

            cell.y += cell.height;
            cell.x = 20;
            cell.width = 90;
            EditorGUI.LabelField(cell, "构建平台", GEStyle.HeaderLabel);

            cell.x += cell.width;
            cell.width = rect.width - cell.x - 20;
            BuildConfig.BuildTarget =
                (BuildTarget)EditorGUI.EnumPopup(cell, BuildConfig.BuildTarget, GEStyle.PreDropDown);

            cell.x += cell.width;
            cell.width = 20;
            if (GUI.Button(cell, GC_REFRESH, GEStyle.toolbarbutton))
                BuildConfig.BuildTarget = EditorUserBuildSettings.activeBuildTarget;

            #endregion

            #region 6

            cell.y += cell.height;
            cell.x = 20;
            cell.width = 90;
            EditorGUI.LabelField(cell, "构建包名", GEStyle.HeaderLabel);

            cell.x += cell.width;
            cell.width = rect.width - cell.x;
            if (BuildConfig.BuildFirstPackage)
                EditorGUI.LabelField(cell, AssetSystem.TagsRecord, GEStyle.PreDropDown);
            else
                Data.CurrentPackageIndex = EditorGUI.Popup(cell, Data.CurrentPackageIndex, LookModeDisplayPackages,
                    GEStyle.PreDropDown);

            if (GUI.changed)
            {
                if (Data.Packages.Length <= Data.CurrentPackageIndex || Data.CurrentPackageIndex < 0)
                    Data.CurrentPackageIndex = 0;
                BuildConfig.PackageName = Data.Packages[Data.CurrentPackageIndex].Name;
            }

            #endregion

            #region 7

            cell.y += cell.height;
            cell.x = 20;
            cell.width = 90;
            EditorGUI.LabelField(cell, "构建管线", GEStyle.HeaderLabel);

            cell.x += cell.width;
            cell.width = rect.width - cell.x;
            BuildConfig.BuildPipeline =
                (EBuildPipeline)EditorGUI.EnumPopup(cell, BuildConfig.BuildPipeline, GEStyle.PreDropDown);

            #endregion

            #region 8

            cell.y += cell.height;
            cell.x = 20;
            cell.width = 90;
            EditorGUI.LabelField(cell, "压缩模式", GEStyle.HeaderLabel);

            cell.x += cell.width;
            cell.width = rect.width - cell.x;
            BuildConfig.CompressedMode =
                (ECompressMode)EditorGUI.EnumPopup(cell, BuildConfig.CompressedMode, GEStyle.PreDropDown);

            #endregion

            #region 9

            cell.y += cell.height;
            cell.x = 20;
            cell.width = 90;
            EditorGUI.LabelField(cell, "构建模式", GEStyle.HeaderLabel);

            cell.x += cell.width;
            cell.width = rect.width - cell.x;
            BuildConfig.BuildMode = (EBuildMode)EditorGUI.EnumPopup(cell, BuildConfig.BuildMode, GEStyle.PreDropDown);

            #endregion

            #region 10

            if (!(Tags is null) && Tags.Length > 0)
            {
                if (Tags.Length > 31) // 位运算最大支持31位 2^31 显示警告
                {
                    cell.y += cell.height;
                    cell.x = 20;
                    cell.width = 90;
                    EditorGUI.LabelField(cell, "首包标签", GEStyle.HeaderLabel);

                    cell.x += cell.width;
                    cell.width = rect.width - cell.x;
                    EditorGUI.HelpBox(cell, "首包标签数量超过31个, 请减少标签数量", MessageType.Warning);
                }
                else
                {
                    cell.y += cell.height;
                    cell.x = 20;
                    cell.width = 90;
                    EditorGUI.LabelField(cell, "首包标签", GEStyle.HeaderLabel);

                    cell.x += cell.width;
                    cell.width = rect.width - cell.x;
                    CurrentTagIndex = EditorGUI.MaskField(cell, CurrentTagIndex, Tags, GEStyle.PreDropDown);

                    if (GUI.changed)
                    {
                        BuildConfig.FirstPackTag =
                            string.Join(";", Tags.Where((t, i) => (CurrentTagIndex & (1 << i)) != 0));
                    }

                    #region 10

                    if (CurrentTagIndex != 0)
                    {
                        cell.y += cell.height;
                        cell.x = 20 + 90;
                        cell.width = rect.width - cell.x;
                        EditorGUI.HelpBox(cell, BuildConfig.FirstPackTag, MessageType.None);
                    }

                    #endregion
                }
            }

            #endregion

            #region 11

            cell.y += cell.height;
            cell.x = 20;
            cell.width = 90;
            EditorGUI.LabelField(cell, "缓存清理数量", GEStyle.HeaderLabel);

            cell.x += cell.width;
            cell.width = rect.width - cell.x;
            BuildConfig.AutoCleanCacheNumber = EditorGUI.IntSlider(cell, BuildConfig.AutoCleanCacheNumber, 1, 20);

            #endregion

            outRect = new Rect(rect.x, cell.y + cell.height, rect.width, cell.height);
        }

        #endregion

        #region FTP

        private void OnDrawBuildFTP(in Rect rect, out Rect outRect)
        {
            if (BuildConfig.FTPConfigs is null || BuildConfig.FTPConfigs.Length == 0)
            {
                outRect = rect;
                return;
            }

            var cell = new Rect(rect.x, rect.y, rect.width, 300);
            var view = new Rect(rect.x, rect.y, rect.width - 16, 20 * BuildConfig.FTPConfigs.Length);
            using (var scope = new GUI.ScrollViewScope(cell, OnDrawConfigFTPScroll, view))
            {
                OnDrawConfigFTPScroll = scope.scrollPosition;
                if (view.height > cell.height) cell.width -= 16;
                var rectItem = new Rect(rect.x, rect.y, rect.width - 16, 20);
                for (var i = BuildConfig.FTPConfigs.Length - 1; i >= 0; i--)
                {
                    rectItem.width = 20;
                    rectItem.x = cell.width - rectItem.width;

                    if (GUI.Button(rectItem, GC_DEL, GEStyle.toolbarbutton))
                    {
                        GUI.FocusControl(null);
                        if (EditorUtility.DisplayDialog("提示", "确定删除?", "确定", "取消"))
                        {
                            BuildConfig.FTPConfigs = BuildConfig.FTPConfigs.RemoveAt(i).Exclude();
                        }

                        continue;
                    }

                    if (!string.IsNullOrEmpty(BuildConfig.FTPConfigs[i].Server)
                        && !string.IsNullOrEmpty(BuildConfig.FTPConfigs[i].User)
                        && !string.IsNullOrEmpty(BuildConfig.FTPConfigs[i].Pass))
                    {
                        rectItem.width = 20;
                        rectItem.x -= rectItem.width;
                        if (GUI.Button(rectItem, GC_REFRESH, GEStyle.toolbarbutton))
                        {
                            GUI.FocusControl(null);
                            BuildConfig.FTPConfigs[i].DirTreeFiled.UpdateOption();
                        }

                        if (File.Exists(AssetSystem.SequenceRecordQueue.LOCAL_PATH))
                        {
                            rectItem.width = 100;
                            rectItem.x -= rectItem.width;
                            if (GUI.Button(rectItem, "更新首包配置", GEStyle.toolbarbutton))
                            {
                                UpdateUploadFirstPack(BuildConfig.FTPConfigs[i]);
                                GUI.FocusControl(null);
                            }
                        }

                        rectItem.width = 50;
                        rectItem.x -= rectItem.width;
                        if (GUI.Button(rectItem, "校验", GEStyle.toolbarbutton))
                        {
                            GUI.FocusControl(null);
                            ValidateFTP(BuildConfig.FTPConfigs[i]);
                        }
                    }

                    rectItem.width = rectItem.x - 30;
                    rectItem.x -= rectItem.width;
                    GUI.Label(rectItem, GetFtpItemDes(BuildConfig.FTPConfigs[i], i), GEStyle.HeaderLabel);

                    rectItem.width = 30;
                    rectItem.x -= rectItem.width;
                    if (GUI.Button(rectItem, BuildConfig.FTPConfigs[i].Folded ? GC_FOLDOUT : GC_FOLDOUT_ON,
                            GEStyle.TEtoolbarbutton))
                    {
                        BuildConfig.FTPConfigs[i].Folded = !BuildConfig.FTPConfigs[i].Folded;
                        GUI.FocusControl(null);
                    }

                    rectItem.y += rectItem.height;
                    if (!BuildConfig.FTPConfigs[i].Folded) continue;
                    rectItem.x = cell.x;
                    rectItem.width = cell.width;
                    OnDrawBuildFTP(BuildConfig.FTPConfigs[i], rectItem, out rectItem);
                }
            }

            outRect = new Rect(rect.x, cell.y + cell.height, rect.width, cell.height);
        }

        private string GetFtpItemDes(ASBuildConfig.FTPConfig config, int i)
        {
            _builder.Clear();
            if (!config.Folded && config.isUploading) _builder.Append($"[上传中:{config.UploadProgress.Progress}%]");
            if (!string.IsNullOrEmpty(config.Name))
                _builder.Append(config.Name);

            if (!string.IsNullOrEmpty(config.Description))
                _builder.Append('(').Append(config.Description).Append(')');

            if (!string.IsNullOrEmpty(config.Server))
            {
                _builder.Append('[');
                if (config.DirTreeFiled.IsValidity()) _builder.Append(config.DirTreeFiled.GetFullPath()).Append(" -> ");

                _builder.Append(config.Server).Append(':').Append(config.Port);
                if (string.IsNullOrEmpty(config.RemotePath))
                    _builder.Append(']');
                else
                    _builder.Append('/').Append(config.RemotePath).Append(']');
            }

            if (_builder.Length == 0) _builder.Append($"NO.{i}");
            return _builder.ToString().Replace('\\', '/');
        }

        private static void OnDrawBuildFTP(ASBuildConfig.FTPConfig config, in Rect rect, out Rect outRect)
        {
            using (new EditorGUI.DisabledScope(config.isUploading))
            {
                #region 名称:描述

                var cell = new Rect(rect.x + 5, rect.y, 100, 20);
                EditorGUI.LabelField(cell, "名称:描述", GEStyle.HeaderLabel);

                cell.x += cell.width;
                cell.width = (rect.width - cell.x) / 2;
                config.Name = EditorGUI.DelayedTextField(cell, config.Name);

                cell.x += cell.width;
                config.Description = EditorGUI.DelayedTextField(cell, config.Description);

                #endregion

                #region 地址:端口

                cell.y += cell.height;
                cell.x = rect.x + 5;
                cell.width = 100;
                EditorGUI.LabelField(cell, "地址:端口", GEStyle.HeaderLabel);

                cell.x += cell.width;
                cell.width = rect.width - cell.x - 50;
                config.Server = EditorGUI.DelayedTextField(cell, config.Server);

                cell.x += cell.width;
                cell.width = 50;
                config.Port = EditorGUI.DelayedIntField(cell, config.Port);

                #endregion

                #region 账户:密码

                cell.y += cell.height;
                cell.x = rect.x + 5;
                cell.width = 100;
                EditorGUI.LabelField(cell, "账户:密码", GEStyle.HeaderLabel);

                cell.x += cell.width;
                cell.width = (rect.width - cell.x) / 2;
                config.User = EditorGUI.DelayedTextField(cell, config.User);

                cell.x += cell.width;
                config.Pass = EditorGUI.DelayedTextField(cell, config.Pass);

                #endregion

                #region 远程路径

                cell.y += cell.height;
                cell.x = rect.x + 5;
                cell.width = 100;
                EditorGUI.LabelField(cell, "远程路径", GEStyle.HeaderLabel);

                cell.x += cell.width;
                cell.width = rect.width - cell.x - 50;
                config.RemotePath = EditorGUI.DelayedTextField(cell, config.RemotePath);

                cell.x += cell.width;
                cell.width = 50;
                if (GUI.Button(cell, "创建", GEStyle.toolbarbutton))
                {
                    GUI.FocusControl(null);
                    if (string.IsNullOrEmpty(config.RemotePath))
                        EditorUtility.DisplayDialog("提示", "远程路径不能为空", "确定");
                    else
                        CreateFTP(config);
                }

                #endregion

                #region 地址:端口

                cell.y += cell.height;
                if (config.isUploading)
                {
                    cell.x = rect.x + 5;
                    cell.width = rect.width - cell.x - 50;
                    EditorGUI.LabelField(cell, $"上传进度... {config.UploadProgress}", GEStyle.CenteredLabel);

                    cell.x += cell.width;
                    cell.width = 50;
                    if (GUI.Button(cell, "取消", GEStyle.toolbarbutton))
                    {
                        GUI.FocusControl(null);
                        config.UploadOperation?.Cancel();
                    }
                }
                else
                {
                    cell.width = 0;
                    if (!string.IsNullOrEmpty(config.Server) &&
                        !string.IsNullOrEmpty(config.User) &&
                        !string.IsNullOrEmpty(config.Pass) &&
                        !string.IsNullOrEmpty(config.DirTreeFiled.DirPath)
                       )
                    {
                        cell.x = rect.width - 50;
                        cell.width = 50;
                        if (GUI.Button(cell, "上传", GEStyle.toolbarbutton))
                        {
                            GUI.FocusControl(null);
                            config.Upload();
                        }
                    }

                    cell.x = rect.x + 5 + 100;
                    cell.width = rect.width - cell.width - cell.x;
                    config.DirTreeFiled.OnDraw(cell);

                    cell.width = 100;
                    cell.x -= cell.width;
                    EditorGUI.LabelField(cell, "本地路径", GEStyle.HeaderLabel);
                }

                #endregion

                outRect = new Rect(rect.x, cell.y + cell.height, rect.width, rect.height);
            }
        }

        #endregion

        #region Google Cloud

        private string GetGCItemDes(ASBuildConfig.GCloudConfig config, int i)
        {
            _builder.Clear();
            if (!config.Folded && config.isUploading) _builder.Append("[上传中 ... ]");
            if (!string.IsNullOrEmpty(config.Name))
                _builder.Append(config.Name);

            if (!string.IsNullOrEmpty(config.Description))
                _builder.Append('(').Append(config.Description).Append(')');

            if (!string.IsNullOrEmpty(config.BUCKET_NAME))
            {
                _builder.Append('[');
                if (config.DirTreeFiled.IsValidity()) _builder.Append(config.DirTreeFiled.GetFullPath()).Append(" -> ");

                _builder.Append(config.BUCKET_NAME);
                if (!string.IsNullOrEmpty(config.Description)) _builder.Append('/').Append(config.Description);
                _builder.Append(']');
            }

            if (_builder.Length == 0) _builder.Append($"NO.{i}");
            return _builder.ToString().Replace('\\', '/');
        }

        private void OnDrawBuildGCloud(in Rect rect, out Rect outRect)
        {
            if (BuildConfig.GCloudConfigs is null || BuildConfig.GCloudConfigs.Length == 0)
            {
                outRect = rect;
                return;
            }

            var cell = new Rect(rect.x, rect.y, rect.width, 300);
            var view = new Rect(rect.x, rect.y, rect.width - 16, 20 * BuildConfig.FTPConfigs.Length);
            using (var scope = new GUI.ScrollViewScope(cell, OnDrawConfigGCScroll, view))
            {
                OnDrawConfigGCScroll = scope.scrollPosition;
                if (view.height > cell.height + cell.y) cell.width -= 16;
                var rectItem = new Rect(rect.x, rect.y, rect.width - 16, 20);
                for (var i = BuildConfig.GCloudConfigs.Length - 1; i >= 0; i--)
                {
                    rectItem.width = 20;
                    rectItem.x = cell.width - rectItem.width;

                    if (GUI.Button(rectItem, GC_DEL, GEStyle.toolbarbutton))
                    {
                        GUI.FocusControl(null);
                        if (EditorUtility.DisplayDialog("提示", "确定删除?", "确定", "取消"))
                        {
                            BuildConfig.GCloudConfigs = BuildConfig.GCloudConfigs.RemoveAt(i).Exclude();
                        }
                    }

                    using (new EditorGUI.DisabledScope(BuildConfig.GCloudConfigs[i].isUploading))
                    {
                        if (!string.IsNullOrEmpty(BuildConfig.GCloudConfigs[i].BUCKET_NAME))
                        {
                            rectItem.width = 20;
                            rectItem.x -= rectItem.width;
                            if (GUI.Button(rectItem, GC_REFRESH, GEStyle.toolbarbutton))
                            {
                                GUI.FocusControl(null);
                                BuildConfig.GCloudConfigs[i].DirTreeFiled.UpdateOption();
                            }

                            if (!string.IsNullOrEmpty(BuildConfig.GCloudConfigs[i].DirTreeFiled.DirPath)
                                && File.Exists(AssetSystem.SequenceRecordQueue.LOCAL_PATH))
                            {
                                rectItem.width = 100;
                                rectItem.x -= rectItem.width;
                                if (GUI.Button(rectItem, "更新首包配置", GEStyle.toolbarbutton))
                                {
                                    GUI.FocusControl(null);
                                    UpdateUploadFirstPack(BuildConfig.GCloudConfigs[i]);
                                }
                            }
                        }
                    }

                    rectItem.width = rectItem.x - 30;
                    rectItem.x -= rectItem.width;
                    GUI.Label(rectItem, GetGCItemDes(BuildConfig.GCloudConfigs[i], i), GEStyle.HeaderLabel);

                    rectItem.width = 30;
                    rectItem.x -= rectItem.width;
                    if (GUI.Button(rectItem, BuildConfig.GCloudConfigs[i].Folded ? GC_FOLDOUT : GC_FOLDOUT_ON,
                            GEStyle.TEtoolbarbutton))
                    {
                        BuildConfig.GCloudConfigs[i].Folded = !BuildConfig.GCloudConfigs[i].Folded;
                        GUI.FocusControl(null);
                    }

                    rectItem.y += rectItem.height;
                    if (!BuildConfig.GCloudConfigs[i].Folded) continue;
                    rectItem.x = cell.x;
                    rectItem.width = cell.width;
                    OnDrawBuildGC(BuildConfig.GCloudConfigs[i], rectItem, out rectItem);
                }
            }

            outRect = new Rect(rect.x, cell.y + cell.height, rect.width, cell.height);
        }

        private void OnDrawBuildGC(ASBuildConfig.GCloudConfig config, in Rect rect, out Rect outRect)
        {
            using (new EditorGUI.DisabledScope(config.isUploading))
            {
                #region 名称:描述

                var cell = new Rect(rect.x + 5, rect.y, 100, 20);
                EditorGUI.LabelField(cell, "名称:描述", GEStyle.HeaderLabel);

                cell.x += cell.width;
                cell.width = (rect.width - cell.x) / 2;
                config.Name = EditorGUI.DelayedTextField(cell, config.Name);

                cell.x += cell.width;
                config.Description = EditorGUI.DelayedTextField(cell, config.Description);

                #endregion

                #region 环境 : gcloud local

                cell.y += cell.height;
                cell.x = rect.x + 5;
                cell.width = 100;
                EditorGUI.LabelField(cell, "环境 : gcloud local", GEStyle.HeaderLabel);

                cell.x += cell.width;
                cell.width = rect.width - cell.x;
                config.GCLOUD_PATH = EditorGUI.DelayedTextField(cell, config.GCLOUD_PATH);

                #endregion

                #region 环境 gsutil local

                cell.y += cell.height;
                cell.x = rect.x + 5;
                cell.width = 100;
                EditorGUI.LabelField(cell, "环境 : gcloud local", GEStyle.HeaderLabel);

                cell.x += cell.width;
                cell.width = rect.width - cell.x;
                config.GSUTIL_PATH = EditorGUI.DelayedTextField(cell, config.GSUTIL_PATH);

                #endregion

                #region 储存桶

                cell.y += cell.height;
                cell.x = rect.x + 5;
                cell.width = 100;
                EditorGUI.LabelField(cell, "储存桶", GEStyle.HeaderLabel);

                cell.x += cell.width;
                cell.width = rect.width - cell.x;
                config.BUCKET_NAME = EditorGUI.DelayedTextField(cell, config.BUCKET_NAME);

                #endregion


                #region 元数据:[键:值]

                cell.y += cell.height;
                cell.x = rect.x + 5;
                cell.width = 100;
                EditorGUI.LabelField(cell, "元数据:[键:值]", GEStyle.HeaderLabel);

                cell.x += cell.width;
                cell.width = (rect.width - cell.x) / 2;
                config.MetaDataKey = EditorGUI.DelayedTextField(cell, config.MetaDataKey);

                cell.x += cell.width;
                config.MetaDataValue = EditorGUI.DelayedTextField(cell, config.MetaDataValue);

                #endregion

                #region 本地路径

                using (GELayout.VHorizontal(GEStyle.ToolbarBottom))
                {
                    cell.y += cell.height;
                    cell.x = rect.x + 5;
                    cell.width = 100;
                    EditorGUI.LabelField(cell, "本地路径", GEStyle.HeaderLabel);

                    cell.x += cell.width;
                    cell.width = rect.width - cell.x - 50;
                    config.DirTreeFiled.OnDraw(cell);

                    cell.x += cell.width;
                    cell.width = 50;
                    if (config.isUploading)
                    {
                        EditorGUI.LabelField(cell, "上传中", GEStyle.toolbarbutton);
                    }
                    else if (GUI.Button(cell, "上传", GEStyle.toolbarbutton))
                    {
                        GUI.FocusControl(null);
                        _ = config.Upload();
                    }
                }

                #endregion

                outRect = new Rect(rect.x, cell.y + cell.height, rect.width, rect.height);
            }
        }

        #endregion
    }
}