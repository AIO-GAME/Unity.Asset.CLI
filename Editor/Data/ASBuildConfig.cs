using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using AIO.UEngine;
using UnityEditor;
using UnityEngine;

namespace AIO.UEditor
{
    [Serializable]
    [Description("资源打包配置")]
    [HelpURL("https://github.com/AIO-GAME/Unity.Asset.CLI/blob/main/.github/API_USAGE/ToolWindow.md#-%E6%89%93%E5%8C%85%E5%B7%A5%E5%85%B7-")]
    public partial class ASBuildConfig : ScriptableObject
    {
        private static ASBuildConfig _instance;

        /// <summary>
        ///     自动清理缓存数量
        /// </summary>
        [InspectorName("自动清理缓存数量")]
        public int AutoCleanCacheNumber = 5;

        /// <summary>
        ///     打包管线
        /// </summary>
        [InspectorName("打包管线")]
        public EBuildPipeline BuildPipeline;

        /// <summary>
        ///     构建模式
        /// </summary>
        [InspectorName("构建模式")]
        public EBuildMode BuildMode = EBuildMode.IncrementalBuild;

        /// <summary>
        ///     构建版本号
        /// </summary>
        [InspectorName("版本号")]
        public string BuildVersion;

        /// <summary>
        ///     资源包名称
        /// </summary>
        [InspectorName("构建资源包名称")]
        public string PackageName;

        /// <summary>
        ///     加密模式
        /// </summary>
        [InspectorName("加密模式")]
        public string EncyptionClassName;

        /// <summary>
        ///     压缩模式
        /// </summary>
        [InspectorName("压缩模式")]
        public ECompressMode CompressedMode;

        /// <summary>
        ///     首包标签集合
        /// </summary>
        [InspectorName("首包标签集合")]
        public string FirstPackTag;

        /// <summary>
        ///     验证构建结果
        /// </summary>
        [InspectorName("验证构建结果")]
        public bool ValidateBuild;

        /// <summary>
        ///     构建结果输出路径
        /// </summary>
        [InspectorName("构建结果输出路径")]
        public string BuildOutputPath;

        /// <summary>
        ///     构建平台
        /// </summary>
        [InspectorName("构建平台")]
        public BuildTarget BuildTarget;

        /// <summary>
        ///     将资源包合并对比至Latest 文件夹
        /// </summary>
        [InspectorName("将资源包合并对比至Latest")]
        public bool MergeToLatest;

        /// <summary>
        ///     构建首包资源
        /// </summary>
        [InspectorName("构建首包资源")]
        public bool BuildFirstPackage;

        /// <summary>
        ///     FTP上传配置
        /// </summary>
        [InspectorName("FTP上传配置")]
        public FTPConfig[] FTPConfigs;

        /// <summary>
        ///     GC上传配置
        /// </summary>
        [SerializeField]
        [InspectorName("GC上传配置")]
        public GCloudConfig[] GCloudConfigs;

        /// <summary>
        ///     获取本地资源包地址
        /// </summary>
        public static ASBuildConfig GetOrCreate()
        {
            if (_instance is null)
            {
                var objects = EHelper.IO.GetScriptableObjects<ASBuildConfig>();
                if (objects != null && objects.Length > 0)
                    foreach (var asset in objects)
                    {
                        if (asset is null) continue;
                        if (string.IsNullOrEmpty(asset.BuildOutputPath))
                            asset.BuildOutputPath = Path.Combine(EHelper.Path.Project, "Bundles");

                        _instance = asset;
                        break;
                    }

                if (_instance is null)
                {
                    _instance                 = CreateInstance<ASBuildConfig>();
                    _instance.BuildOutputPath = Path.Combine(EHelper.Path.Project, "Bundles");
                    AssetDatabase.CreateAsset(_instance, "Assets/Editor/ASBuildConfig.asset");
                    AssetDatabase.SaveAssets();
                }
            }

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

        public static implicit operator AssetBuildCommand(ASBuildConfig config)
        {
            var command = new AssetBuildCommand
            {
                BuildMode             = config.BuildMode,
                BuildPipeline         = config.BuildPipeline,
                ActiveTarget          = config.BuildTarget,
                PackageVersion        = config.BuildVersion,
                CompressOption        = config.CompressedMode,
                BuildPackage          = config.PackageName,
                EncyptionClassName    = config.EncyptionClassName,
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