using AIO.UEngine;
using UnityEditor;

namespace AIO.UEditor
{
    /// <summary>
    ///     AssetBuildCommand
    /// </summary>
    public class AssetBuildCommand : ArgumentCustom
    {
        private const string PREFIX = "b@";

        /// <summary>
        ///     构建管线
        /// </summary>
        public const string C_BUILD_PIPELINE = PREFIX + nameof(BuildPipeline);

        /// <summary>
        ///     目标平台
        /// </summary>
        public const string C_ACTIVE_TARGET = PREFIX + nameof(ActiveTarget);

        /// <summary>
        ///     加密类名称
        /// </summary>
        public const string C_ENCYPTION_CLASS_NAME = PREFIX + nameof(EncyptionClassName);

        /// <summary>
        ///     构建模式
        /// </summary>
        public const string C_BUILD_MODE = PREFIX + nameof(BuildMode);

        /// <summary>
        ///     构建的包裹名称
        /// </summary>
        public const string C_PACKAGE = PREFIX + nameof(BuildPackage);

        /// <summary>
        ///     压缩方式
        /// </summary>
        public const string C_COMPRESS_OPTION = PREFIX + nameof(CompressOption);

        /// <summary>
        ///     输出文件名称样式
        /// </summary>
        public const string C_OUTPUT_NAME_STYLE = PREFIX + nameof(OutputNameStyle);

        /// <summary>
        ///     首包资源文件的拷贝方式
        /// </summary>
        public const string C_COPY_BUILD_IN_FILE_OPTION = PREFIX + nameof(CopyBuildInFileOption);

        /// <summary>
        ///     首包资源文件的标签集合
        /// </summary>
        public const string C_COPY_BUILD_IN_FILE_TAGS = PREFIX + nameof(CopyBuildInFileTags);

        /// <summary>
        ///     文件输出根目录
        /// </summary>
        public const string C_OUTPUT_ROOT = PREFIX + nameof(OutputRoot);

        /// <summary>
        ///     验证构建结果
        /// </summary>
        public const string C_VERIFY_BUILDING_RESULT = PREFIX + nameof(VerifyBuildingResult);

        /// <summary>
        ///     构建版本
        /// </summary>
        public const string C_PACKAGE_VERSION = PREFIX + nameof(PackageVersion);

        /// <summary>
        ///     合并对比至Latest
        /// </summary>
        public const string C_MERGE_TO_LATEST = PREFIX + nameof(MergeToLatest);

        /// <summary>
        ///    导出至StreamingAssets
        /// </summary>
        public const string C_EXPORT_TO_STREAMING_ASSETS = PREFIX + nameof(ExportToStreamingAssets);
        
        [Argument(C_ACTIVE_TARGET, EArgLabel.Enum)]
        public BuildTarget ActiveTarget = BuildTarget.Android;

        [Argument(C_BUILD_MODE, EArgLabel.Enum)]
        public EBuildMode BuildMode = EBuildMode.IncrementalBuild;

        [Argument(C_PACKAGE, EArgLabel.String)]
        public string BuildPackage = string.Empty;

        [Argument(C_BUILD_PIPELINE, EArgLabel.Enum)]
        public EBuildPipeline BuildPipeline = EBuildPipeline.BuiltinBuildPipeline;

        [Argument(C_COMPRESS_OPTION, EArgLabel.Enum)]
        public ECompressMode CompressOption = ECompressMode.LZ4;

        [Argument(C_COPY_BUILD_IN_FILE_OPTION, EArgLabel.Enum)]
        public ECopyBuildInFileOption CopyBuildInFileOption = ECopyBuildInFileOption.ClearAndCopyAll;

        [Argument(C_COPY_BUILD_IN_FILE_TAGS, EArgLabel.String)]
        public string CopyBuildInFileTags = string.Empty;

        [Argument(C_ENCYPTION_CLASS_NAME, EArgLabel.String)]
        public string EncyptionClassName = string.Empty;

        [Argument(C_MERGE_TO_LATEST, EArgLabel.Bool)]
        public bool MergeToLatest = false;

        [Argument(C_EXPORT_TO_STREAMING_ASSETS, EArgLabel.Bool)]
        public bool ExportToStreamingAssets = false;
        
        [Argument(C_OUTPUT_NAME_STYLE, EArgLabel.Enum)]
        public EOutputNameStyle OutputNameStyle = EOutputNameStyle.BundleName_HashName;

        [Argument(C_OUTPUT_ROOT, EArgLabel.String)]
        public string OutputRoot = string.Empty;

        [Argument(C_PACKAGE_VERSION, EArgLabel.String)]
        public string PackageVersion = string.Empty;

        [Argument(C_VERIFY_BUILDING_RESULT, EArgLabel.Bool)]
        public bool VerifyBuildingResult = true;
    }
}