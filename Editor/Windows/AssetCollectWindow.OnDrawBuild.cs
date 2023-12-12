/*|============|*|
|*|Author:     |*| Star fire
|*|Date:       |*| 2023-12-01
|*|E-Mail:     |*| xinansky99@foxmail.com
|*|============|*/

using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace AIO.UEditor
{
    public partial class AssetCollectWindow
    {
        partial void OnDrawHeaderBuildMode()
        {
            EditorGUILayout.Separator();
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

            CurrentPackageIndex = BuildConfig.PackageName == null
                ? 0
                : Array.IndexOf(LookModeDisplayPackages, BuildConfig.PackageName);

            if (BuildConfig.BuildTarget == 0 ||
                BuildConfig.BuildTarget == BuildTarget.NoTarget
               )
            {
                BuildConfig.BuildTarget = EditorUserBuildSettings.activeBuildTarget;
            }
        }

        private void OnDrawBuildBuild()
        {
            if (LookModeDisplayPackages is null || LookModeDisplayPackages.Length == 0)
            {
                EditorGUILayout.Separator();
                return;
            }

            using (GELayout.VHorizontal())
            {
                GELayout.Label("输出路径", GTOption.Width(true));
                if (GUILayout.Button("选择", GP_Width_50))
                    BuildConfig.BuildOutputPath =
                        EditorUtility.OpenFolderPanel("请选择导出路径", BuildConfig.BuildOutputPath, "");

                if (string.IsNullOrEmpty(BuildConfig.BuildOutputPath) ||
                    Directory.GetParent(BuildConfig.BuildOutputPath) == null) return;

                if (GUILayout.Button("打开", GP_Width_50))
                {
                    PrPlatform.Open.Path(BuildConfig.BuildOutputPath).Async();
                    return;
                }

#if SUPPORT_YOOASSET
                if (GUILayout.Button("生成配置", GP_Width_75))
                {
                    MenuItem_YooAssets.CreateConfig(BuildConfig.BuildOutputPath);
                    return;
                }
#endif
            }

            using (GELayout.VHorizontal())
            {
                GELayout.HelpBox(BuildConfig.BuildOutputPath);
            }

            using (GELayout.VHorizontal())
            {
                EditorGUILayout.LabelField("构建版本", GP_Width_100);
                BuildConfig.BuildVersion = GELayout.Field(BuildConfig.BuildVersion);
                if (GUILayout.Button(GC_REFRESH, GP_Width_20))
                {
                    BuildConfig.BuildVersion = DateTime.Now.ToString("yyyy-MM-dd-HHmmss");
                }
            }

            using (GELayout.VHorizontal())
            {
                BuildConfig.FirstPack = GELayout.ToggleLeft("首包", BuildConfig.FirstPack, GTOption.Width(50));
                BuildConfig.ValidateBuild =
                    GELayout.ToggleLeft("验证构建结果", BuildConfig.ValidateBuild, GP_Width_100);

                GELayout.Separator();
#if SUPPORT_YOOASSET
                if (GUILayout.Button("构建 Yoo", GP_Width_75))
                {
                    var BuildCommand = new YooAssetBuildCommand
                    {
                        PackageVersion = BuildConfig.BuildVersion,
                        BuildPackage = BuildConfig.PackageName,
                        EncyptionClassName = "",
                        ActiveTarget = BuildConfig.BuildTarget,
                        BuildPipeline = BuildConfig.BuildPipeline,
                        OutputRoot = BuildConfig.BuildOutputPath,
                    };
                    YooAssetBuild.ArtBuild(BuildCommand);
                    BuildConfig.BuildVersion = DateTime.Now.ToString("yyyy-MM-dd-HHmmss");
                }
#else
                if (GUILayout.Button("构建", GP_Width_50))
                {
                    EditorUtility.DisplayDialog("提示", "请先导入 YooAsset Or Other TrdTools", "确定");
                }
#endif
            }

            using (GELayout.VHorizontal())
            {
                EditorGUILayout.LabelField("构建平台", GP_Width_100);
                BuildConfig.BuildTarget = GELayout.Popup(BuildConfig.BuildTarget, GEStyle.PreDropDown);
                if (GUILayout.Button(GC_REFRESH, GP_Width_20))
                {
                    BuildConfig.BuildTarget = EditorUserBuildSettings.activeBuildTarget;
                }
            }

            using (GELayout.VHorizontal())
            {
                EditorGUILayout.LabelField("构建包名", GP_Width_100);
                CurrentPackageIndex = EditorGUILayout.Popup(CurrentPackageIndex, LookModeDisplayPackages,
                    GEStyle.PreDropDown);
                if (GUI.changed)
                {
                    if (Data.Packages.Length <= CurrentPackageIndex || CurrentPackageIndex < 0)
                    {
                        CurrentPackageIndex = 0;
                    }

                    BuildConfig.PackageName = Data.Packages[CurrentPackageIndex].Name;
                }
            }

            using (GELayout.VHorizontal())
            {
                EditorGUILayout.LabelField("构建管线", GP_Width_100);
                BuildConfig.BuildPipeline = GELayout.Popup(BuildConfig.BuildPipeline, GEStyle.PreDropDown);
            }

            using (GELayout.VHorizontal())
            {
                EditorGUILayout.LabelField("构建模式", GP_Width_100);
                BuildConfig.BuildMode = GELayout.Popup(BuildConfig.BuildMode, GEStyle.PreDropDown);
            }

            // BuildConfig.EncyptionClassName = GELayout.Popup("加密模式", YooAssetEncryptionsIndex, YooAssetEncryptionsName);

            // BuildConfig.CompressedModeName = GELayout.Popup("压缩模式", BuildConfig.CompressOption);

            // BuildConfig.OutputNameStyle = GELayout.Popup("文件名称样式", BuildConfig.OutputNameStyle);

            // BuildConfig.CopyBuildinFileOption = GELayout.Popup("首包资源文件的拷贝方式", BuildConfig.CopyBuildinFileOption);

            if (Tags != null && Tags.Length > 0)
            {
                using (GELayout.VHorizontal())
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

        private void OnDrawBuildFTP()
        {
            using (GELayout.VHorizontal())
            {
                EditorGUILayout.LabelField("地址", GP_Width_100);
                BuildConfig.FTPServerIP = GELayout.FieldDelayed(BuildConfig.FTPServerIP);
                if (string.IsNullOrEmpty(BuildConfig.FTPServerIP)) return;

                if (GUILayout.Button("校验", GP_Width_50))
                {
                    GUI.FocusControl(null);
                    BuildFTPValidate();
                }

                if (GUILayout.Button("上传", GP_Width_50))
                {
                    GUI.FocusControl(null);
                    BuildFTPUpload();
                }
            }

            using (GELayout.VHorizontal())
            {
                EditorGUILayout.LabelField("端口", GP_Width_100);
                BuildConfig.FTPServerPort = GELayout.FieldDelayed(BuildConfig.FTPServerPort);
            }

            using (GELayout.VHorizontal())
            {
                EditorGUILayout.LabelField("用户名", GP_Width_100);
                BuildConfig.FTPUser = GELayout.FieldDelayed(BuildConfig.FTPUser);
            }

            using (GELayout.VHorizontal())
            {
                EditorGUILayout.LabelField("密码", GP_Width_100);
                BuildConfig.FTPPassword = GELayout.FieldDelayed(BuildConfig.FTPPassword);
            }

            using (GELayout.VHorizontal())
            {
                EditorGUILayout.LabelField("远程路径", GP_Width_100);
                BuildConfig.FTPRemotePath = GELayout.FieldDelayed(BuildConfig.FTPRemotePath);
            }

            using (GELayout.VHorizontal())
            {
                EditorGUILayout.LabelField("本地路径", GP_Width_100);
                BuildConfig.FTPLocalPath = GELayout.FieldDelayed(BuildConfig.FTPLocalPath);

                if (GUILayout.Button("选择", GP_Width_50))
                {
                    GUI.FocusControl(null);
                    BuildConfig.FTPLocalPath =
                        EditorUtility.OpenFolderPanel("请选择上传路径", BuildConfig.FTPLocalPath, "");
                    return;
                }

                if (Directory.Exists(BuildConfig.FTPLocalPath))
                {
                    if (GUILayout.Button("移动", GP_Width_50))
                    {
                        GUI.FocusControl(null);
                        var source = BuildConfig.BuildOutputPath.Trim('/', '\\');
                        var target = BuildConfig.FTPLocalPath.Trim('/', '\\');
                        if (AHelper.IO.ExistsFolder(target))
                            AHelper.IO.DeleteFolder(target, SearchOption.AllDirectories, true);
                        PrPlatform.Folder.Copy(target, source).Async();
                    }

                    if (GUILayout.Button("链接", GP_Width_50))
                    {
                        GUI.FocusControl(null);
                        var source = BuildConfig.BuildOutputPath.Trim('/', '\\');
                        var target = BuildConfig.FTPLocalPath.Trim('/', '\\');
                        IExecutor executor = null;
                        if (AHelper.IO.ExistsFolder(target))
                            executor = PrPlatform.Folder.Del(target);
                        var symbolic = executor is null
                            ? PrPlatform.Folder.Symbolic(target, source)
                            : executor.Link(PrPlatform.Folder.Symbolic(target, source));
                        symbolic.Async();
                    }

                    if (GUILayout.Button("打开", GP_Width_50))
                    {
                        GUI.FocusControl(null);
                        PrPlatform.Open.Path(BuildConfig.FTPLocalPath).Async();
                    }
                }
            }
        }

        private async void BuildFTPValidate()
        {
            var uri = string.Concat(BuildConfig.FTPServerIP, ":", BuildConfig.FTPServerPort, "\\",
                BuildConfig.FTPRemotePath).Trim('\\', '/');
            var handle = await AHelper.Net.FTP.CheckAsync(uri, BuildConfig.FTPUser, BuildConfig.FTPPassword);
            EditorUtility.DisplayDialog("提示", handle ? "连接成功" : "连接失败", "确定");
        }

        private async void BuildFTPUpload()
        {
            var serverIP = string.Concat(BuildConfig.FTPServerIP, ":", BuildConfig.FTPServerPort).Trim('\\', '/');
            using (var handle = AHandle.FTP.Create(serverIP, BuildConfig.FTPUser, BuildConfig.FTPPassword,
                       BuildConfig.FTPRemotePath))
            {
                await handle.InitAsync();
                var args = new AProgressEvent
                {
                    OnProgress = progress =>
                    {
                        if (EditorUtility.DisplayCancelableProgressBar("Upload FTP", progress.ToString(),
                                progress.Progress / 100f))
                        {
                        }
                    },
                    OnError = error =>
                    {
                        Debug.LogException(error);
                        EditorUtility.ClearProgressBar();
                    },
                    OnComplete = () =>
                    {
                        EditorUtility.ClearProgressBar();
                        EditorUtility.DisplayDialog("Upload FTP", "Upload FTP Complete", "OK");
                    }
                };
                await handle.UploadDirAsync(BuildConfig.BuildOutputPath, args);
            }
        }

        partial void OnDrawBuildMode()
        {
            FoldoutBuildSetting = GELayout.VFoldoutHeader(OnDrawBuildBuild, "构建设置", FoldoutBuildSetting);
            FoldoutUploadFTP = GELayout.VFoldoutHeader(OnDrawBuildFTP, "FTP", FoldoutUploadFTP);
        }
    }
}