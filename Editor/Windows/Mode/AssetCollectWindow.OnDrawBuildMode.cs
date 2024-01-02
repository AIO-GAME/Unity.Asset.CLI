/*|============|*|
|*|Author:     |*| Star fire
|*|Date:       |*| 2023-12-01
|*|E-Mail:     |*| xinansky99@foxmail.com
|*|============|*/

using System;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using YooAsset.Editor;

namespace AIO.UEditor
{
    public partial class AssetCollectWindow
    {
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

        private void OnDrawBuildBuild()
        {
            if (LookModeDisplayPackages is null || LookModeDisplayPackages.Length == 0) return;
            using (GELayout.Vertical(GEStyle.Badge))
            {
                using (GELayout.VHorizontal())
                {
                    GELayout.Label("输出路径", GTOption.Width(98));

                    if (string.IsNullOrEmpty(BuildConfig.BuildOutputPath) ||
                        Directory.GetParent(BuildConfig.BuildOutputPath) == null)
                    {
                        GELayout.Separator();
                        if (GUILayout.Button("选择目录", GEStyle.toolbarbutton, GP_Width_50))
                            BuildConfig.BuildOutputPath =
                                EditorUtility.OpenFolderPanel("请选择导出路径", BuildConfig.BuildOutputPath, "");
                        return;
                    }

                    BuildConfig.FirstPack = GELayout.ToggleLeft("首包", BuildConfig.FirstPack, GP_Width_50);
                    BuildConfig.ValidateBuild = GELayout.ToggleLeft("验证构建结果", BuildConfig.ValidateBuild, GP_Width_100);

                    GELayout.Separator();

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

                    if (GUILayout.Button("构建资源", GEStyle.toolbarbutton, GP_Width_75))
                    {
                        var currentScene = SceneManager.GetSceneAt(0);
                        if (!string.IsNullOrEmpty(currentScene.path))
                        {
                            var scene = SceneManager.GetSceneByPath(currentScene.path);
                            if (scene.isDirty) // 获取当前场景的修改状态
                            {
                                if (EditorUtility.DisplayDialog("提示", "当前场景未保存,是否保存?", "保存", "取消"))
                                {
                                    EditorSceneManager.SaveScene(scene);
                                }
                            }
                        }


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
                }

                using (GELayout.VHorizontal(GEStyle.ToolbarBottom))
                {
                    GELayout.Label(BuildConfig.BuildOutputPath, GEStyle.CenteredLabel);
                }

                using (GELayout.VHorizontal(GEStyle.ToolbarBottom))
                {
                    EditorGUILayout.LabelField("构建版本", GP_Width_100, GP_Height_20);
                    BuildConfig.BuildVersion = GELayout.Field(BuildConfig.BuildVersion, GP_Height_20);
                    if (GUILayout.Button(GC_REFRESH, GEStyle.toolbarbutton, GP_Width_20, GP_Height_20))
                    {
                        BuildConfig.BuildVersion = DateTime.Now.ToString("yyyy-MM-dd-HHmmss");
                    }
                }

                using (GELayout.VHorizontal(GEStyle.ToolbarBottom))
                {
                    EditorGUILayout.LabelField("构建平台", GP_Width_100);
                    BuildConfig.BuildTarget = GELayout.Popup(BuildConfig.BuildTarget, GEStyle.PreDropDown);
                    if (GUILayout.Button(GC_REFRESH, GEStyle.toolbarbutton, GP_Width_20, GP_Height_20))
                    {
                        BuildConfig.BuildTarget = EditorUserBuildSettings.activeBuildTarget;
                    }
                }

                using (GELayout.VHorizontal(GEStyle.ToolbarBottom))
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

                using (GELayout.VHorizontal(GEStyle.ToolbarBottom))
                {
                    EditorGUILayout.LabelField("构建管线", GP_Width_100);
                    BuildConfig.BuildPipeline = GELayout.Popup(BuildConfig.BuildPipeline, GEStyle.PreDropDown);
                }

                using (GELayout.VHorizontal(GEStyle.ToolbarBottom))
                {
                    EditorGUILayout.LabelField("构建模式", GP_Width_100);
                    BuildConfig.BuildMode = GELayout.Popup(BuildConfig.BuildMode, GEStyle.PreDropDown);
                }

                if (Tags != null && Tags.Length > 0)
                {
                    using (GELayout.VHorizontal(GEStyle.ToolbarBottom))
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

        private StringBuilder _builder = new StringBuilder();

        private string GetFTPItemDes(ASBuildConfig.FTPConfig config, int i)
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
                if (config.DirTreeFiled.IsValidity())
                {
                    _builder.Append(config.DirTreeFiled.GetFullPath()).Append(" -> ");
                }

                _builder.Append(config.Server).Append(':')
                    .Append(config.Port);
                if (string.IsNullOrEmpty(config.RemotePath))
                {
                    _builder.Append(']');
                }
                else
                {
                    _builder.Append('/').Append(config.RemotePath).Append(']');
                }
            }

            if (_builder.Length == 0) _builder.Append($"NO.{i}");
            return _builder.ToString().Replace('\\', '/');
        }

        private string GetGCItemDes(ASBuildConfig.GCloudConfig config, int i)
        {
            _builder.Clear();
            if (!config.Folded && config.isUploading) _builder.Append($"[上传中 ... ]");
            if (!string.IsNullOrEmpty(config.Name))
                _builder.Append(config.Name);

            if (!string.IsNullOrEmpty(config.Description))
                _builder.Append('(').Append(config.Description).Append(')');

            if (!string.IsNullOrEmpty(config.BUCKET_NAME))
            {
                _builder.Append('[');
                if (config.DirTreeFiled.IsValidity())
                {
                    _builder.Append(config.DirTreeFiled.GetFullPath()).Append(" -> ");
                }

                _builder.Append(config.BUCKET_NAME);
                if (string.IsNullOrEmpty(config.Description))
                {
                    _builder.Append(']');
                }
                else
                {
                    _builder.Append('/').Append(config.Description).Append(']');
                }
            }

            if (_builder.Length == 0) _builder.Append($"NO.{i}");
            return _builder.ToString().Replace('\\', '/');
        }

        private void OnDrawBuildFTP()
        {
            if (BuildConfig.FTPConfigs is null || BuildConfig.FTPConfigs.Length == 0) return;
            using (var scope = new EditorGUILayout.ScrollViewScope(OnDrawConfigFTPScroll))
            {
                OnDrawConfigFTPScroll = scope.scrollPosition;


                for (var i = BuildConfig.FTPConfigs.Length - 1; i >= 0; i--)
                {
                    using (GELayout.Vertical(GEStyle.INThumbnailShadow))
                    {
                        using (GELayout.VHorizontal(GEStyle.Toolbar))
                        {
                            if (GELayout.Button(BuildConfig.FTPConfigs[i].Folded ? GC_FOLDOUT : GC_FOLDOUT_ON,
                                    GEStyle.TEtoolbarbutton, GP_Width_30))
                            {
                                BuildConfig.FTPConfigs[i].Folded = !BuildConfig.FTPConfigs[i].Folded;
                                GUI.FocusControl(null);
                            }

                            GELayout.Label(GetFTPItemDes(BuildConfig.FTPConfigs[i], i), GEStyle.HeaderLabel,
                                GP_Width_EXPAND);

                            if (!string.IsNullOrEmpty(BuildConfig.FTPConfigs[i].Server) &&
                                !string.IsNullOrEmpty(BuildConfig.FTPConfigs[i].User) &&
                                !string.IsNullOrEmpty(BuildConfig.FTPConfigs[i].Pass))
                            {
                                if (GUILayout.Button("校验", GEStyle.toolbarbutton, GP_Width_50))
                                {
                                    GUI.FocusControl(null);
                                    BuildFTPValidate(BuildConfig.FTPConfigs[i]);
                                }

                                if (!string.IsNullOrEmpty(BuildConfig.FTPConfigs[i].DirTreeFiled.DirPath))
                                {
                                    if (GUILayout.Button("复制", GEStyle.toolbarbutton, GP_Width_50))
                                    {
                                        GUI.FocusControl(null);
                                        var source = BuildConfig.BuildOutputPath.Trim('/', '\\');
                                        var target = Path.Combine(
                                            BuildConfig.FTPConfigs[i].DirTreeFiled.DirPath
                                                .Trim('/', '\\'), Path.GetFileName(source));
                                        if (EditorUtility.DisplayDialog("提示", $"{source}\n复制\n{target}", "确定",
                                                "取消"))
                                        {
                                            if (AHelper.IO.ExistsFolder(target))
                                                AHelper.IO.DeleteFolder(target, SearchOption.AllDirectories, true);
                                            PrPlatform.Folder.Copy(target, source).Async();
                                            return;
                                        }
                                    }

                                    if (GUILayout.Button("链接", GEStyle.toolbarbutton, GP_Width_50))
                                    {
                                        GUI.FocusControl(null);
                                        var source = BuildConfig.BuildOutputPath.Trim('/', '\\');
                                        var target = Path.Combine(
                                            BuildConfig.FTPConfigs[i].DirTreeFiled.DirPath.Trim('/', '\\'),
                                            Path.GetFileName(source));
                                        if (EditorUtility.DisplayDialog("提示", $"{source}\n链接\n{target}", "确定",
                                                "取消"))
                                        {
                                            IExecutor executor = null;
                                            if (AHelper.IO.ExistsFolder(target))
                                                executor = PrPlatform.Folder.Del(target);
                                            var symbolic = executor is null
                                                ? PrPlatform.Folder.Symbolic(target, source)
                                                : executor.Link(PrPlatform.Folder.Symbolic(target, source));
                                            symbolic.Async();
                                            return;
                                        }
                                    }
                                }

                                if (File.Exists(AssetSystem.SequenceRecordQueue.LOCAL_PATH))
                                {
                                    if (GUILayout.Button("更新首包配置", GEStyle.toolbarbutton, GP_Width_100))
                                    {
                                        GUI.FocusControl(null);
                                        UpdateUploadFirstPack(BuildConfig.FTPConfigs[i]);
                                    }
                                }

                                if (GUILayout.Button(GC_REFRESH, GEStyle.toolbarbutton, GP_Width_20))
                                {
                                    GUI.FocusControl(null);
                                    BuildConfig.FTPConfigs[i].DirTreeFiled.UpdateOption();
                                    return;
                                }
                            }

                            if (GUILayout.Button(GC_DEL, GEStyle.toolbarbutton, GP_Width_20))
                            {
                                GUI.FocusControl(null);
                                if (EditorUtility.DisplayDialog("提示", "确定删除?", "确定", "取消"))
                                {
                                    BuildConfig.FTPConfigs = BuildConfig.FTPConfigs.RemoveAt(i);
                                    return;
                                }
                            }
                        }

                        OnDrawBuildFTP(BuildConfig.FTPConfigs[i]);
                    }
                }
            }
        }

        private void OnDrawBuildGCloud()
        {
            if (BuildConfig.GCloudConfigs is null || BuildConfig.GCloudConfigs.Length == 0) return;
            using (var scope = new EditorGUILayout.ScrollViewScope(OnDrawConfigGCScroll))
            {
                OnDrawConfigGCScroll = scope.scrollPosition;
                for (var i = BuildConfig.GCloudConfigs.Length - 1; i >= 0; i--)
                {
                    using (GELayout.Vertical(GEStyle.INThumbnailShadow))
                    {
                        using (GELayout.VHorizontal(GEStyle.Toolbar))
                        {
                            if (GELayout.Button(BuildConfig.GCloudConfigs[i].Folded ? GC_FOLDOUT : GC_FOLDOUT_ON,
                                    GEStyle.TEtoolbarbutton, GP_Width_30))
                            {
                                BuildConfig.GCloudConfigs[i].Folded = !BuildConfig.GCloudConfigs[i].Folded;
                                GUI.FocusControl(null);
                            }

                            GELayout.Label(GetGCItemDes(BuildConfig.GCloudConfigs[i], i),
                                GEStyle.HeaderLabel, GP_Width_EXPAND);

                            if (BuildConfig.GCloudConfigs[i].isUploading) GUI.enabled = false;

                            if (!string.IsNullOrEmpty(BuildConfig.GCloudConfigs[i].BUCKET_NAME))
                            {
                                if (!string.IsNullOrEmpty(BuildConfig.GCloudConfigs[i].DirTreeFiled.DirPath))
                                {
                                    if (File.Exists(AssetSystem.SequenceRecordQueue.LOCAL_PATH))
                                    {
                                        if (GUILayout.Button("更新首包配置", GEStyle.toolbarbutton, GP_Width_100))
                                        {
                                            GUI.FocusControl(null);
                                            UpdateUploadFirstPack(BuildConfig.GCloudConfigs[i]);
                                        }
                                    }
                                }

                                if (GUILayout.Button(GC_REFRESH, GEStyle.toolbarbutton, GP_Width_20))
                                {
                                    GUI.FocusControl(null);
                                    BuildConfig.GCloudConfigs[i].DirTreeFiled.UpdateOption();
                                    return;
                                }
                            }

                            if (GUILayout.Button(GC_DEL, GEStyle.toolbarbutton, GP_Width_20))
                            {
                                GUI.FocusControl(null);
                                if (EditorUtility.DisplayDialog("提示", "确定删除?", "确定", "取消"))
                                {
                                    BuildConfig.GCloudConfigs = BuildConfig.GCloudConfigs.RemoveAt(i);
                                    return;
                                }
                            }

                            if (BuildConfig.GCloudConfigs[i].isUploading) GUI.enabled = true;
                        }

                        OnDrawBuildGC(BuildConfig.GCloudConfigs[i]);
                    }
                }
            }
        }

        private static async void UpdateUploadFirstPack(ASBuildConfig.FTPConfig config)
        {
            await config.UploadFirstPack(AssetSystem.SequenceRecordQueue.LOCAL_PATH);
            EditorUtility.DisplayDialog("提示", "上传成功", "确定");
        }

        private static async void UpdateUploadFirstPack(ASBuildConfig.GCloudConfig config)
        {
            await config.UploadFirstPack(AssetSystem.SequenceRecordQueue.LOCAL_PATH);
        }

        private static async void CreateFTP(ASBuildConfig.FTPConfig config)
        {
            var handle = AHandle.FTP.Create(config.Server, config.Port,
                config.User, config.Pass, config.RemotePath);
            var status = await handle.InitAsync();
            EditorUtility.DisplayDialog("提示", status
                ? $"创建成功 {handle.URI}"
                : $"创建失败 {handle.URI}", "确定");
        }

        private void OnDrawBuildGC(ASBuildConfig.GCloudConfig config)
        {
            if (!config.Folded) return;
            if (config.isUploading) GUI.enabled = false;

            using (GELayout.VHorizontal(GEStyle.ToolbarBottom))
            {
                EditorGUILayout.LabelField("名称:描述", GP_Width_100);
                config.Name = GELayout.FieldDelayed(config.Name);
                config.Description = GELayout.FieldDelayed(config.Description);
            }

            using (GELayout.VHorizontal(GEStyle.ToolbarBottom))
            {
                EditorGUILayout.LabelField("储存桶:元数据", GP_Width_100);
                config.BUCKET_NAME = GELayout.FieldDelayed(config.BUCKET_NAME);
                config.MetaData = GELayout.FieldDelayed(config.MetaData);
            }

            using (GELayout.VHorizontal(GEStyle.ToolbarBottom))
            {
                EditorGUILayout.LabelField("本地路径", GP_Width_100);
                config.DirTreeFiled.OnDraw();

                if (config.isUploading) GUILayout.Label("上传中", GEStyle.toolbarbutton, GP_Width_50);
                else if (GUILayout.Button("上传", GEStyle.toolbarbutton, GP_Width_50))
                {
                    GUI.FocusControl(null);
                    config.Upload();
                }
            }

            if (config.isUploading) GUI.enabled = true;
        }

        private void OnDrawBuildFTP(ASBuildConfig.FTPConfig config)
        {
            if (!config.Folded) return;

            if (config.isUploading) GUI.enabled = false;

            using (GELayout.VHorizontal(GEStyle.ToolbarBottom))
            {
                EditorGUILayout.LabelField("名称:描述", GP_Width_100);
                config.Name = GELayout.FieldDelayed(config.Name);
                config.Description = GELayout.FieldDelayed(config.Description);
            }

            using (GELayout.VHorizontal(GEStyle.ToolbarBottom))
            {
                EditorGUILayout.LabelField("地址:端口", GP_Width_100);
                config.Server = GELayout.FieldDelayed(config.Server);
                config.Port = GELayout.FieldDelayed(config.Port, GP_Width_50);
            }

            using (GELayout.VHorizontal(GEStyle.ToolbarBottom))
            {
                EditorGUILayout.LabelField("账户:密码", GP_Width_100);
                config.User = GELayout.FieldDelayed(config.User);
                config.Pass = GELayout.FieldDelayed(config.Pass);
            }

            using (GELayout.VHorizontal(GEStyle.ToolbarBottom))
            {
                EditorGUILayout.LabelField("远程路径", GP_Width_100);
                config.RemotePath = GELayout.FieldDelayed(config.RemotePath);
                if (!string.IsNullOrEmpty(config.RemotePath))
                {
                    if (GUILayout.Button("创建", GEStyle.toolbarbutton, GP_Width_50))
                    {
                        GUI.FocusControl(null);
                        CreateFTP(config);
                        return;
                    }
                }
            }

            using (GELayout.VHorizontal(GEStyle.ToolbarBottom))
            {
                if (config.isUploading)
                {
                    EditorGUILayout.LabelField($"上传进度... {config.UploadProgress}",
                        GEStyle.CenteredLabel, GP_Width_EXPAND);

                    if (GUILayout.Button("取消", GEStyle.toolbarbutton, GP_Width_50))
                    {
                        GUI.FocusControl(null);
                        config.UploadOperation?.Cancel();
                    }
                }
                else
                {
                    EditorGUILayout.LabelField("本地路径", GP_Width_100);
                    config.DirTreeFiled.OnDraw();
                    if (!string.IsNullOrEmpty(config.Server) &&
                        !string.IsNullOrEmpty(config.User) &&
                        !string.IsNullOrEmpty(config.Pass) &&
                        !string.IsNullOrEmpty(config.DirTreeFiled.DirPath)
                       )
                    {
                        if (GUILayout.Button("上传", GEStyle.toolbarbutton, GP_Width_50))
                        {
                            GUI.FocusControl(null);
                            config.Upload();
                        }
                    }
                }
            }

            if (config.isUploading) GUI.enabled = true;
        }

        private static async void BuildFTPValidate(ASBuildConfig.FTPConfig config)
        {
            var handle = await config.Validate();
            EditorUtility.DisplayDialog("提示", handle ? "连接成功" : "连接失败", "确定");
        }

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
            //
            // FoldoutNoticeDingDing = GELayout.VFoldoutHeaderGroupWithHelp(OnDrawBuildNoticeDingDing,
            //     "钉钉通知", FoldoutNoticeDingDing);
        }
    }
}