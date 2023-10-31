#if SUPPORT_YOOASSET

using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
using YooAsset;
using YooAsset.Editor;

namespace AIO.UEditor
{
    /// <summary>
    ///  Yoo Asset
    /// </summary>
    public static class YooAssetBuild
    {
        public static void ArtBuild()
        {
            var cmd = Argument.ResolverCustomCur<UnityArgsCommand>();
            var args = cmd.executeMethod;
            var buildArgs = Argument.ResolverCustom<YooAssetBuildCommand>(args);

            if (string.IsNullOrEmpty(buildArgs.BuildPackage)) throw new ArgumentNullException($"构建包: 名称不能为 NULL");
            if (string.IsNullOrEmpty(buildArgs.PackageVersion)) throw new ArgumentNullException($"构建包: 版本不能为 NULL");

            if (string.IsNullOrEmpty(buildArgs.OutputRoot))
                buildArgs.OutputRoot = AssetBundleBuilderHelper.GetDefaultBuildOutputRoot();

            if (Enum.IsDefined(typeof(BuildTarget), buildArgs.ActiveTarget) ||
                buildArgs.ActiveTarget == BuildTarget.NoTarget)
                buildArgs.ActiveTarget = EditorUserBuildSettings.activeBuildTarget;

            if (Enum.IsDefined(typeof(EOutputNameStyle), buildArgs.OutputNameStyle))
                buildArgs.OutputNameStyle = EOutputNameStyle.BundleName_HashName;

            if (Enum.IsDefined(typeof(ECopyBuildinFileOption), buildArgs.CopyBuildinFileOption))
                buildArgs.CopyBuildinFileOption = ECopyBuildinFileOption.None;

            if (Enum.IsDefined(typeof(ECompressOption), buildArgs.CompressOption))
                buildArgs.CompressOption = ECompressOption.LZ4;

            if (Enum.IsDefined(typeof(EBuildPipeline), buildArgs.BuildPipeline))
                buildArgs.BuildPipeline =
#if UNITY_2021_3_OR_NEW
                    EBuildPipeline.ScriptableBuildPipeline;
#else
                    EBuildPipeline.BuiltinBuildPipeline;
#endif

            if (Enum.IsDefined(typeof(EBuildMode), buildArgs.BuildMode))
                buildArgs.BuildMode = EBuildMode.ForceRebuild;


            ArtBuild(buildArgs);
        }

        /// <summary>
        /// 修改配置
        /// </summary>
        public static void ChangeSetting()
        {
        }

        /// <summary>
        /// 构建版本相关
        /// </summary>
        /// <returns></returns>
        private static string GetBuildPackageVersion()
        {
            var totalMinutes = DateTime.Now.Hour * 60 + DateTime.Now.Minute;
            return DateTime.Now.ToString("yyyy-MM-dd") + "-" + totalMinutes;
        }

        private static IEncryptionServices CreateEncryptionServicesInstance(string EncryptionClassName)
        {
            if (string.IsNullOrEmpty(EncryptionClassName)) return null;
            return (from item in EditorTools.GetAssignableTypes(typeof(IEncryptionServices))
                where item.FullName == EncryptionClassName
                select (IEncryptionServices)Activator.CreateInstance(item)).FirstOrDefault();
        }

        public static void ArtBuild(YooAssetBuildCommand command)
        {
            Debug.Log(AHelper.Json.Serialize(command));

            var buildParameters = new BuildParameters();
            buildParameters.BuildTarget = command.ActiveTarget;
            buildParameters.BuildPipeline = command.BuildPipeline;
            buildParameters.BuildMode = command.BuildMode;
            buildParameters.PackageName = command.BuildPackage;
            buildParameters.CompressOption = command.CompressOption;
            buildParameters.OutputNameStyle = command.OutputNameStyle;
            buildParameters.SharedPackRule = new ZeroRedundancySharedPackRule();
            buildParameters.CopyBuildinFileOption = command.CopyBuildinFileOption;
            buildParameters.CopyBuildinFileTags = command.CopyBuildinFileTags;
            buildParameters.VerifyBuildingResult = command.VerifyBuildingResult;
            buildParameters.PackageVersion = command.PackageVersion;
            buildParameters.BuildOutputRoot = command.OutputRoot;
            buildParameters.StreamingAssetsRoot = Application.streamingAssetsPath;
            buildParameters.DisableWriteTypeTree = false;

            if (string.IsNullOrEmpty(command.EncyptionClassName))
                buildParameters.EncryptionServices = CreateEncryptionServicesInstance(command.EncyptionClassName);

            if (command.BuildPipeline == EBuildPipeline.ScriptableBuildPipeline)
            {
                buildParameters.SBPParameters = new BuildParameters.SBPBuildParameters();
                buildParameters.SBPParameters.WriteLinkXML = true;
            }

            var builder = new AssetBundleBuilder();
            var buildResult = builder.Run(buildParameters);
            if (buildResult.Success) EditorUtility.RevealInFinder(buildResult.OutputPackageDirectory);
            MenuItem_YooAssets.CreateConfig();
        }
    }
}
#endif