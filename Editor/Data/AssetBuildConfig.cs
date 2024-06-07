using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using AIO.UEngine;
using UnityEditor;
using UnityEngine;

namespace AIO.UEditor
{
    [Serializable]
    [Description("资源打包配置")]
    [HelpURL("https://github.com/AIO-GAME/Unity.Asset.CLI/blob/main/.github/API_USAGE/ToolWindow.md#-%E6%89%93%E5%8C%85%E5%B7%A5%E5%85%B7-")]
    public partial class AssetBuildConfig : ScriptableObject
    {
        private static AssetBuildConfig _instance;

        [InspectorName("自动清理缓存数量")]
        public int AutoCleanCacheNumber = 5;

        [InspectorName("打包管线")]
        public EBuildPipeline BuildPipeline;

        [InspectorName("构建模式")]
        public EBuildMode BuildMode = EBuildMode.IncrementalBuild;

        [InspectorName("版本号")]
        public string BuildVersion;

        [InspectorName("资源包名称")]
        public string PackageName;

        [InspectorName("加密模式")]
        public string EncryptionClassName;

        [InspectorName("压缩模式")]
        public ECompressMode CompressedMode;

        [InspectorName("首包标签集合")]
        public string FirstPackTag;

        [InspectorName("验证构建结果")]
        public bool ValidateBuild;

        [InspectorName("导至StreamingAssets")]
        public bool ExportToStreamingAssets;

        [InspectorName("构建结果输出路径")]
        public string BuildOutputPath;

        [InspectorName("构建平台")]
        public BuildTarget BuildTarget;

        [InspectorName("将资源包合并对比至Latest")]
        public bool MergeToLatest;

        [InspectorName("构建首包资源")]
        public bool BuildFirstPackage;

        [InspectorName("FTP上传配置")]
        public FTPConfig[] FTPConfigs;

        [InspectorName("GC上传配置"), SerializeField]
        public GCloudConfig[] GCloudConfigs;

        /// <summary>
        ///     获取本地资源包地址
        /// </summary>
        public static AssetBuildConfig GetOrCreate()
        {
            if (_instance != null) return _instance;

            var objects = EHelper.IO.GetScriptableObjects<AssetBuildConfig>();
            if (objects != null && objects.Length > 0)
                foreach (var asset in objects.Where(asset => asset))
                {
                    if (string.IsNullOrEmpty(asset.BuildOutputPath))
                        asset.BuildOutputPath = Path.Combine(EHelper.Path.Project, "Bundles");

                    _instance = asset;
                    break;
                }

            if (_instance) return _instance;

            _instance                 = CreateInstance<AssetBuildConfig>();
            _instance.BuildOutputPath = Path.Combine(EHelper.Path.Project, "Bundles");
            AssetDatabase.CreateAsset(_instance, "Assets/Editor/ASBuildConfig.asset");
            AssetDatabase.SaveAssets();

            return _instance;
        }

        public void Save()
        {
            if (Equals(null))
            {
                Debug.LogWarning("ASBuildConfig is null so not save");
                return;
            }

            EditorUtility.SetDirty(this);
        }

        public static implicit operator AssetBuildCommand(AssetBuildConfig config)
        {
            var command = new AssetBuildCommand
            {
                BuildMode             = config.BuildMode,
                BuildPipeline         = config.BuildPipeline,
                ActiveTarget          = config.BuildTarget,
                PackageVersion        = config.BuildVersion,
                CompressOption        = config.CompressedMode,
                BuildPackage          = config.PackageName,
                EncyptionClassName    = config.EncryptionClassName,
                CopyBuildInFileOption = ECopyBuildInFileOption.None,
                CopyBuildInFileTags   = config.FirstPackTag,
                MergeToLatest         = config.MergeToLatest,
                VerifyBuildingResult  = config.ValidateBuild,
                OutputRoot            = config.BuildOutputPath,
                OutputNameStyle       = EOutputNameStyle.BundleName_HashName
            };
            return command;
        }
    }
}