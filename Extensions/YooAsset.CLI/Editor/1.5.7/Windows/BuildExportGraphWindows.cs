/*|✩ - - - - - |||
|||✩ Author:   ||| -> xi nan
|||✩ Date:     ||| -> 2023-08-16
|||✩ Document: ||| ->
|||✩ - - - - - |*/

#if SUPPORT_YOOASSET

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using YooAsset;
using YooAsset.Editor;

namespace AIO.UEditor.CLI
{
    public class YooAssetGraphicRect : GraphicBisection
    {
        private AssetBuildCommand Commond;

        /// <summary>
        /// YooAsset的资源包列表
        /// </summary>
        private string[] YooAssetPackages;

        private int YooAssetPackagesIndex;

        /// <summary>
        /// YooAsset的加密列表
        /// </summary>
        private List<Type> YooAssetEncryptions;

        private string[] YooAssetEncryptionsName;

        private int YooAssetEncryptionsIndex;

        /// <summary>
        /// 平台路径
        /// </summary>
        private string BuildInFilePlatform;

        /// <summary>
        /// 工程路径
        /// </summary>
        private Dictionary<BuildTarget, YooAssetUnityArgs> EngineeringPathPath;

        public BuildTarget BuildTarget;

        public YooAssetGraphicRect()
        {
            YooAssetPackageTarget = new List<string>();
            YooAssetPackageTargetIndex = new Dictionary<string, int>();
            YooAssetPackageVersionTarget = new Dictionary<string, List<string>>();

            ServerIP = EHelper.Prefs.LoadString<YooAssetGraphicRect>(nameof(ServerIP));
            User = EHelper.Prefs.LoadString<YooAssetGraphicRect>(nameof(User));
            Password = EHelper.Prefs.LoadString<YooAssetGraphicRect>(nameof(Password));
            LocalStoragePath = EHelper.Prefs.LoadString<YooAssetGraphicRect>(nameof(LocalStoragePath));
            EngineeringPathPath = EHelper.Prefs.LoadJson(typeof(YooAssetGraphicRect).FullName,
                new Dictionary<BuildTarget, YooAssetUnityArgs>());

            var content = EHelper.Prefs.LoadJson(nameof(YooAssetBuildCommand),
                new AssetBuildCommand
                {
                    CopyBuildInFileOption = ECopyBuildInFileOption.None,
                    OutputNameStyle = EOutputNameStyle.HashName
                });
            Commond = content;
#if UNITY_2021_1_OR_NEWER
            Commond.BuildPipeline = EBuildPipeline.ScriptableBuildPipeline;
#else
            Commond.BuildPipeline = EBuildPipeline.BuiltinBuildPipeline;
#endif

            YooAssetPackages = AssetBundleCollectorSettingData.Setting.Packages
                .Select(package => package.PackageName).ToArray();
            YooAssetEncryptions = EditorTools.GetAssignableTypes(typeof(IEncryptionServices));
            YooAssetEncryptions.Insert(0, null);
            YooAssetEncryptionsName = new string[YooAssetEncryptions.Count];
            for (var i = 0; i < YooAssetEncryptions.Count; i++)
            {
                YooAssetEncryptionsName[i] =
                    YooAssetEncryptions[i] != null ? YooAssetEncryptions[i].FullName : "None";
            }

            if (string.IsNullOrEmpty(Commond.OutputRoot))
                Commond.OutputRoot = AssetBundleBuilderHelper.GetDefaultBuildOutputRoot();
            Commond.PackageVersion = DateTime.Now.ToString("yyyy-MM-dd-HHmmss");

            BuildTarget = EditorUserBuildSettings.activeBuildTarget;
            UpdateTarget();
        }

        protected bool FoldoutYooAsset;

        protected void OnDrawYooAsset()
        {
            using (GELayout.VHorizontal())
            {
                GELayout.Label("输出路径", GTOption.Width(true));
                if (GELayout.Button("Select", GTOption.Width(50)))
                    Commond.OutputRoot = EditorUtility.OpenFolderPanel("请选择导出路径", Commond.OutputRoot, "");

                if (GELayout.Button("Open", GTOption.Width(50)))
                {
                    PrPlatform.Open.Path(Commond.OutputRoot).Async();
                    return;
                }
            }

            using (GELayout.VHorizontal())
            {
                Commond.OutputRoot = GELayout.Field(Commond.OutputRoot);
            }

            if (string.IsNullOrEmpty(Commond.OutputRoot)) return;

            Commond.BuildPipeline = GELayout.Popup("构建管线", Commond.BuildPipeline);

            Commond.BuildMode = GELayout.Popup("构建模式", Commond.BuildMode);

            using (GELayout.VHorizontal())
            {
                Commond.PackageVersion = GELayout.Field("构建版本", Commond.PackageVersion);
                if (GELayout.Button("刷新", GTOption.Width(50)))
                {
                    Commond.PackageVersion = DateTime.Now.ToString("yyyy-MM-dd-HHmmss");
                }
            }

            YooAssetPackagesIndex = GELayout.Popup("构建包名", YooAssetPackagesIndex, YooAssetPackages);

            YooAssetEncryptionsIndex = GELayout.Popup("加密模式", YooAssetEncryptionsIndex, YooAssetEncryptionsName);

            Commond.CompressOption = GELayout.Popup("压缩模式", Commond.CompressOption);

            Commond.OutputNameStyle = GELayout.Popup("文件名称样式", Commond.OutputNameStyle);

            Commond.CopyBuildInFileOption = GELayout.Popup("首包资源文件的拷贝方式", Commond.CopyBuildInFileOption);

            Commond.CopyBuildInFileTags = GELayout.Field("首包资源文件的标签集合", Commond.CopyBuildInFileTags);

            Commond.VerifyBuildingResult = GELayout.ToggleLeft("验证构建结果", Commond.VerifyBuildingResult);

            GELayout.Space();
        }

        protected bool FoldoutBuildInFile;

        protected void OnDrawBuildInFile()
        {
            using (GELayout.VHorizontal())
            {
                GELayout.Label("平台路径", GTOption.Width(true));
                if (GELayout.Button("Select", GTOption.Width(50)))
                    BuildInFilePlatform = EditorUtility.OpenFolderPanel("请选择平台路径", BuildInFilePlatform, "");

                if (Directory.Exists(BuildInFilePlatform))
                {
                    if (GELayout.Button("Open", GTOption.Width(50)))
                    {
                        PrPlatform.Open.Path(BuildInFilePlatform).Async();
                        return;
                    }
                }
            }

            using (GELayout.VHorizontal())
            {
                BuildInFilePlatform = GELayout.Field(BuildInFilePlatform);
            }

            if (!EngineeringPathPath.ContainsKey(BuildTarget))
            {
                EngineeringPathPath.Add(BuildTarget, new YooAssetUnityArgs(BuildTarget));
            }

            using (GELayout.VHorizontal())
            {
                GELayout.Label("项目导出工程路径", GTOption.Width(true));

                EngineeringPathPath[BuildTarget].VersionIndex = GELayout.Popup(
                    EngineeringPathPath[BuildTarget].VersionIndex,
                    YooAssetUnityArgs.Versions, GTOption.Width(50));

                if (GELayout.Button("Select", GTOption.Width(50)))
                {
                    EngineeringPathPath[BuildTarget].OutputRoot = EditorUtility.OpenFolderPanel("项目导出工程路径",
                        EngineeringPathPath[BuildTarget].OutputRoot, "");
                    return;
                }

                if (GELayout.Button("Open", GTOption.Width(50)))
                {
                    PrPlatform.Open.Path(EngineeringPathPath[BuildTarget].OutputRoot).Async();
                    return;
                }
            }

            using (GELayout.VHorizontal())
            {
                EngineeringPathPath[BuildTarget].OutputRoot = GELayout.Field(
                    EngineeringPathPath[BuildTarget].OutputRoot);
            }
        }

        protected override void OnDrawLeft(Rect rect)
        {
            FoldoutYooAsset = GELayout.VFoldoutHeaderGroupWithHelp(OnDrawYooAsset, "YooAsset", FoldoutYooAsset);

            FoldoutBuildInFile =
                GELayout.VFoldoutHeaderGroupWithHelp(OnDrawBuildInFile, "DrawBuildInFile", FoldoutBuildInFile);
        }

        private List<string> YooAssetPackageNames;
        private Dictionary<string, List<string>> YooAssetPackageVersionTarget;
        private List<string> YooAssetPackageTarget;
        private Dictionary<string, int> YooAssetPackageTargetIndex;
        private int YooAssetPackageIndex;

        public void UpdateTarget()
        {
            YooAssetPackageVersionTarget.Clear();
            YooAssetPackageTarget.Clear();
            YooAssetPackageTargetIndex.Clear();

            if (!EngineeringPathPath.ContainsKey(BuildTarget))
                EngineeringPathPath.Add(BuildTarget, new YooAssetUnityArgs(BuildTarget));
            BuildInFilePlatform = Path.Combine(Commond.OutputRoot, BuildTarget.ToString())
                .Replace('/', Path.DirectorySeparatorChar);
            YooAssetPackageNames = AHelper.IO.GetDirsName(BuildInFilePlatform).ToList();
            Commond.PackageVersion = DateTime.Now.ToString("yyyy-MM-dd-HHmmss");

            EHelper.Prefs.SaveJson(typeof(YooAssetGraphicRect).FullName, EngineeringPathPath);
            EHelper.Prefs.SaveJson(nameof(YooAssetBuildCommand), Commond);
        }

        protected override void OnDrawRight(Rect rect)
        {
            OnDrawRight(rect.width / 2, rect.width / 4);
        }

        public void OnDrawRight(float widthHalf, float widthQuarter)
        {
            GELayout.Vertical(Command0, GEStyle.INThumbnailShadow);
            if (AHelper.IO.ExistsDir(Commond.OutputRoot))
            {
                GELayout.Vertical(Command1, GEStyle.INThumbnailShadow);
                GELayout.Vertical(Command2, GEStyle.INThumbnailShadow);
                GELayout.Vertical(Command3, GEStyle.INThumbnailShadow);
            }
        }

        private void Command0()
        {
            GELayout.Label("YooAsset 命令合集", GEStyle.DDHeaderStyle, GTOption.Height(25));
            using (GELayout.VHorizontal())
            {
                if (GELayout.Button("构建", 50, 25))
                {
                    Commond.BuildPackage = YooAssetPackages[YooAssetPackagesIndex];
                    Commond.EncyptionClassName = YooAssetEncryptions[YooAssetEncryptionsIndex]?.FullName;
                    Commond.ActiveTarget = BuildTarget;
                    YooAssetBuild.ArtBuild(Commond);
                    Commond.PackageVersion = DateTime.Now.ToString("yyyy-MM-dd-HHmmss");
                }

                if (AHelper.IO.ExistsDir(BuildInFilePlatform))
                {
                    if (AHelper.IO.ExistsDir(EngineeringPathPath[BuildTarget].OutputRoot) &&
                        YooAssetPackageTarget.Count == 0)
                    {
                        if (GELayout.Button("组合资源包\n目标工程", 100, 25))
                        {
                            var dic = YooAssetPackageTarget.ToDictionary(
                                target => target,
                                target => YooAssetPackageVersionTarget[target][YooAssetPackageTargetIndex[target]]);
                            EngineeringPathPath[BuildTarget].BuiltUp(BuildTarget, BuildInFilePlatform, dic);
                            Commond.PackageVersion = DateTime.Now.ToString("yyyy-MM-dd-HHmmss");
                        }
                    }
                }
            }

            GELayout.Space();
        }

        private async void Upload()
        {
            using (var handle = AHandle.FTP.Create(ServerIP, User, Password, "Bundles"))
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
                    OnComplete = (e) =>
                    {
                        EditorUtility.ClearProgressBar();
                        EditorUtility.DisplayDialog("Upload FTP", "Upload FTP Complete", "OK");
                    }
                };
                await handle.UploadDirAsync(Commond.OutputRoot, args);
            }
        }

        private void Command1()
        {
            using (GELayout.VHorizontal())
            {
                GELayout.Label("Upload FTP", GEStyle.DDHeaderStyle, GTOption.Height(25));
                if (GELayout.Button("Upload", GTOption.Width(50), GTOption.Height(25)))
                {
                    Upload();
                }
            }

            using (GELayout.Vertical())
            {
                ServerIP = GELayout.Field("Host", ServerIP);
                User = GELayout.Field("User", User);
                Password = GELayout.Field("Password", Password);
            }

            GELayout.Space();
        }

        private void Command2()
        {
            using (GELayout.VHorizontal())
            {
                GELayout.Label("Local Storage", GEStyle.DDHeaderStyle, GTOption.Height(25));
                if (GELayout.Button("Move", GTOption.Width(50), GTOption.Height(25)))
                {
                    var source = Commond.OutputRoot.Trim('/', '\\');
                    var target = LocalStoragePath.Trim('/', '\\');
                    if (AHelper.IO.ExistsDir(target))
                        AHelper.IO.DeleteDir(target, SearchOption.AllDirectories, true);
                    PrPlatform.Folder.Copy(target, source).Async();
                }

                if (GELayout.Button("Link", GTOption.Width(50), GTOption.Height(25)))
                {
                    var source = Commond.OutputRoot.Trim('/', '\\');
                    var target = LocalStoragePath.Trim('/', '\\');
                    IExecutor executor = null;
                    if (AHelper.IO.ExistsDir(target))
                        executor = PrPlatform.Folder.Del(target);
                    var symbolic = executor is null
                        ? PrPlatform.Folder.Symbolic(target, source)
                        : executor.Link(PrPlatform.Folder.Symbolic(target, source));
                    symbolic.Async();
                }

                if (GELayout.Button("Select", GTOption.Width(50), GTOption.Height(25)))
                {
                    LocalStoragePath =
                        EditorUtility.OpenFolderPanel("Please select the path", LocalStoragePath, "");
                }

                if (GELayout.Button("Open", GTOption.Width(50), GTOption.Height(25)))
                {
                    PrPlatform.Open.Path(LocalStoragePath).Async();
                }
            }

            using (GELayout.Vertical())
            {
                LocalStoragePath = GELayout.Field(LocalStoragePath);
            }

            GELayout.Space();
        }

        private void Command3()
        {
            if (!AHelper.IO.ExistsDir(BuildInFilePlatform)) return;
            using (GELayout.VHorizontal())
            {
                GELayout.Label("组合资源 StreamingAssets", GEStyle.DDHeaderStyle, GTOption.Height(25));

                if (GELayout.Button("Pack", GTOption.Width(50), GTOption.Height(25)))
                {
                    var dic = YooAssetPackageTarget.ToDictionary(
                        target => target,
                        target => YooAssetPackageVersionTarget[target][YooAssetPackageTargetIndex[target]]);
                    EngineeringPathPath[BuildTarget]
                        .BuiltUpToStreamingAssets(BuildTarget, BuildInFilePlatform, dic);
                    Commond.PackageVersion = DateTime.Now.ToString("yyyy-MM-dd-HHmmss");
                    return;
                }
            }

            if (YooAssetPackageNames.Count > 0)
            {
                using (GELayout.VHorizontal())
                {
                    YooAssetPackageIndex = GELayout.Popup("资源包名", YooAssetPackageIndex, YooAssetPackageNames);
                    if (GELayout.Button("Add", GTOption.Width(50)))
                    {
                        if (!YooAssetPackageVersionTarget.ContainsKey(YooAssetPackageNames[YooAssetPackageIndex]))
                        {
                            var package = Path.Combine(BuildInFilePlatform,
                                YooAssetPackageNames[YooAssetPackageIndex]);
                            var list = AHelper.IO.GetDirsName(package)
                                .Where(file => !file.StartsWith("OutputCache")).ToList();
                            YooAssetPackageVersionTarget.Add(YooAssetPackageNames[YooAssetPackageIndex], list);
                        }

                        if (!YooAssetPackageTarget.Contains(YooAssetPackageNames[YooAssetPackageIndex]))
                        {
                            YooAssetPackageTargetIndex.Add(YooAssetPackageNames[YooAssetPackageIndex], 0);

                            YooAssetPackageTarget.Add(YooAssetPackageNames[YooAssetPackageIndex]);
                            YooAssetPackageIndex = 0;
                            YooAssetPackageNames.RemoveAt(YooAssetPackageIndex);
                        }
                    }
                }
            }

            if (YooAssetPackageTarget.Count > 0)
            {
                using (GELayout.Vertical())
                {
                    for (var i = YooAssetPackageTarget.Count - 1; i >= 0; --i)
                    {
                        var j = YooAssetPackageTarget.Count - i - 1;
                        using (GELayout.VHorizontal())
                        {
                            if (GELayout.Button(YooAssetPackageTarget[j], GTOption.Width(150)))
                            {
                                PrPlatform.Open
                                    .Path(Path.Combine(BuildInFilePlatform, YooAssetPackageTarget[j]))
                                    .Async();
                                return;
                            }

                            YooAssetPackageTargetIndex[YooAssetPackageTarget[j]] =
                                GELayout.Popup(YooAssetPackageTargetIndex[YooAssetPackageTarget[j]],
                                    YooAssetPackageVersionTarget[YooAssetPackageTarget[j]]);

                            if (GELayout.Button("Del", GTOption.Width(50)))
                            {
                                YooAssetPackageNames.Add(YooAssetPackageTarget[j]);
                                YooAssetPackageTargetIndex.Remove(YooAssetPackageTarget[j]);
                                YooAssetPackageTarget.RemoveAt(j);
                                return;
                            }
                        }
                    }
                }
            }

            GELayout.Space();
        }

        private string ServerIP;
        private string User;
        private string Password;
        private string LocalStoragePath;

        public override void SaveData()
        {
            EHelper.Prefs.SaveString<YooAssetGraphicRect>(nameof(ServerIP), ServerIP);
            EHelper.Prefs.SaveString<YooAssetGraphicRect>(nameof(User), User);
            EHelper.Prefs.SaveString<YooAssetGraphicRect>(nameof(Password), Password);
            EHelper.Prefs.SaveString<YooAssetGraphicRect>(nameof(LocalStoragePath), LocalStoragePath);
            EHelper.Prefs.SaveJson(typeof(YooAssetGraphicRect).FullName, EngineeringPathPath);
            EHelper.Prefs.SaveJson(nameof(YooAssetBuildCommand), Commond);
        }
    }
}


#endif