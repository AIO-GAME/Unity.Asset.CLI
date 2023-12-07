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
        private string[] Tags;
        private int CurrentTagIndex;

        partial void OnDrawHeaderBuild()
        {
            EditorGUILayout.Separator();
        }

        private void UpdateDataBuild()
        {
            LookModdePackages = Data.Packages.Select(x => x.Name).ToArray();
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
                : Array.IndexOf(LookModdePackages, BuildConfig.PackageName);
        }

        private void OnDrawBuildBuild()
        {
            if (LookModdePackages is null || LookModdePackages.Length == 0)
            {
                EditorGUILayout.Separator();
                return;
            }

            using (GELayout.VHorizontal())
            {
                GELayout.Label("输出路径", GTOption.Width(true));
                if (GELayout.Button("选择", 50))
                    BuildConfig.BuildOutputPath =
                        EditorUtility.OpenFolderPanel("请选择导出路径", BuildConfig.BuildOutputPath, "");

                if (string.IsNullOrEmpty(BuildConfig.BuildOutputPath) ||
                    Directory.GetParent(BuildConfig.BuildOutputPath) == null) return;

                if (GELayout.Button("打开", 50))
                {
                    PrPlatform.Open.Path(BuildConfig.BuildOutputPath).Async();
                    return;
                }
            }

            using (GELayout.VHorizontal())
            {
                GELayout.HelpBox(BuildConfig.BuildOutputPath);
            }

            using (GELayout.VHorizontal())
            {
                EditorGUILayout.LabelField("构建版本", GTOption.Width(100));
                BuildConfig.BuildVersion = GELayout.Field(BuildConfig.BuildVersion);
                if (GELayout.Button(GC_REFRESH, 20))
                {
                    BuildConfig.BuildVersion = DateTime.Now.ToString("yyyy-MM-dd-HHmmss");
                }
            }

            using (GELayout.VHorizontal())
            {
                BuildConfig.FirstPack = GELayout.ToggleLeft("首包", BuildConfig.FirstPack, GTOption.Width(50));
                BuildConfig.ValidateBuild =
                    GELayout.ToggleLeft("验证构建结果", BuildConfig.ValidateBuild, GTOption.Width(100));

#if SUPPORT_YOOASSET
                GELayout.Separator();
                if (GELayout.Button("构建 Yoo", 75))
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
#endif
            }

            using (GELayout.VHorizontal())
            {
                EditorGUILayout.LabelField("构建平台", GTOption.Width(100));
                BuildConfig.BuildTarget = GELayout.Popup(BuildConfig.BuildTarget, GEStyle.PreDropDown);
                if (GELayout.Button(GC_REFRESH, 20))
                {
                    BuildConfig.BuildTarget = EditorUserBuildSettings.activeBuildTarget;
                }
            }

            using (GELayout.VHorizontal())
            {
                EditorGUILayout.LabelField("构建包名", GTOption.Width(100));
                CurrentPackageIndex = EditorGUILayout.Popup(CurrentPackageIndex, LookModdePackages,
                    GEStyle.PreDropDown);
                if (GUI.changed)
                {
                    BuildConfig.PackageName = Data.Packages[CurrentPackageIndex].Name;
                }
            }

            using (GELayout.VHorizontal())
            {
                EditorGUILayout.LabelField("构建管线", GTOption.Width(100));
                BuildConfig.BuildPipeline = GELayout.Popup(BuildConfig.BuildPipeline, GEStyle.PreDropDown);
            }

            using (GELayout.VHorizontal())
            {
                EditorGUILayout.LabelField("构建模式", GTOption.Width(100));
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
                    EditorGUILayout.LabelField("首包标签", GTOption.Width(100));
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

        private bool FoldoutBuildSetting = true;
        private bool FoldoutUploadFTP = true;

        private void OnDrawBuildFTP()
        {
            using (GELayout.VHorizontal())
            {
                EditorGUILayout.LabelField("地址", GTOption.Width(100));
                BuildConfig.FTPServerIP = GELayout.Field(BuildConfig.FTPServerIP);
                if (GELayout.Button("校验", 50))
                {
                    GUI.FocusControl(null);
                    BuildFTPValidate();
                }

                if (GELayout.Button("上传", 50))
                {
                    GUI.FocusControl(null);
                    BuildFTPUpload();
                }
            }

            using (GELayout.VHorizontal())
            {
                EditorGUILayout.LabelField("端口", GTOption.Width(100));
                BuildConfig.FTPServerPort = GELayout.Field(BuildConfig.FTPServerPort);
            }

            using (GELayout.VHorizontal())
            {
                EditorGUILayout.LabelField("用户名", GTOption.Width(100));
                BuildConfig.FTPUser = GELayout.Field(BuildConfig.FTPUser);
            }

            using (GELayout.VHorizontal())
            {
                EditorGUILayout.LabelField("密码", GTOption.Width(100));
                BuildConfig.FTPPassword = GELayout.Field(BuildConfig.FTPPassword);
            }

            using (GELayout.VHorizontal())
            {
                EditorGUILayout.LabelField("远程路径", GTOption.Width(100));
                BuildConfig.FTPRemotePath = GELayout.Field(BuildConfig.FTPRemotePath);
            }

            using (GELayout.VHorizontal())
            {
                EditorGUILayout.LabelField("本地路径", GTOption.Width(100));
                BuildConfig.FTPLocalPath = GELayout.Field(BuildConfig.FTPLocalPath);

                if (GELayout.Button("选择", 50))
                {
                    GUI.FocusControl(null);
                    BuildConfig.FTPLocalPath =
                        EditorUtility.OpenFolderPanel("请选择上传路径", BuildConfig.FTPLocalPath, "");
                    return;
                }

                if (Directory.Exists(BuildConfig.FTPLocalPath))
                {
                    if (GELayout.Button("移动", 50))
                    {
                        GUI.FocusControl(null);
                        var source = BuildConfig.BuildOutputPath.Trim('/', '\\');
                        var target = BuildConfig.FTPLocalPath.Trim('/', '\\');
                        if (AHelper.IO.ExistsFolder(target))
                            AHelper.IO.DeleteFolder(target, SearchOption.AllDirectories, true);
                        PrPlatform.Folder.Copy(target, source).Async();
                    }

                    if (GELayout.Button("链接", 50))
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

                    if (GELayout.Button("打开", 50))
                    {
                        GUI.FocusControl(null);
                        PrPlatform.Open.Path(BuildConfig.FTPLocalPath).Async();
                        return;
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

        partial void OnDrawBuild()
        {
            FoldoutBuildSetting = GELayout.VFoldoutHeader(OnDrawBuildBuild, "构建设置", FoldoutBuildSetting);
            FoldoutUploadFTP = GELayout.VFoldoutHeader(OnDrawBuildFTP, "FTP", FoldoutUploadFTP);
        }
    }
}