/*|============|*|
|*|Author:     |*| Star fire
|*|Date:       |*| 2023-12-06
|*|E-Mail:     |*| xinansky99@foxmail.com
|*|============|*/

using System.IO;
using AIO.UEngine;
using UnityEditor;
using UnityEngine;

namespace AIO.UEditor
{
    [HelpURL(
        "https://github.com/AIO-GAME/Unity.Asset.CLI/blob/main/.github/API_USAGE/ToolWindow.md#-%E6%89%93%E5%8C%85%E5%B7%A5%E5%85%B7-")]
    public partial class ASBuildConfig : ScriptableObject
    {
        /// <summary>
        /// 自动清理缓存数量
        /// </summary>
        [InspectorName("自动清理缓存数量")] public int AutoCleanCacheNum = 5;

        /// <summary>
        /// 打包管线
        /// </summary>
        [InspectorName("打包管线")] public EBuildPipeline BuildPipeline;

        /// <summary>
        /// 构建模式
        /// </summary>
        [InspectorName("构建模式")] public EBuildMode BuildMode = EBuildMode.IncrementalBuild;

        /// <summary>
        /// 构建版本号
        /// </summary>
        [InspectorName("版本号")] public string BuildVersion;

        /// <summary>
        /// 资源包名称
        /// </summary>
        [InspectorName("构建资源包名称")] public string PackageName;

        /// <summary>
        /// 加密模式
        /// </summary>
        [InspectorName("加密模式")] public string EncyptionClassName;

        /// <summary>
        /// 压缩模式
        /// </summary>
        [InspectorName("压缩模式")] public ECompressMode CompressedMode;

        /// <summary>
        /// 首包标签集合
        /// </summary>
        [InspectorName("首包标签集合")] public string FirstPackTag;

        /// <summary>
        /// 验证构建结果
        /// </summary>
        [InspectorName("验证构建结果")] public bool ValidateBuild;

        /// <summary>
        /// 构建结果输出路径
        /// </summary>
        [InspectorName("构建结果输出路径")] public string BuildOutputPath;

        /// <summary>
        /// 构建平台
        /// </summary>
        [InspectorName("构建平台")] public BuildTarget BuildTarget;

        /// <summary>
        /// 将资源包合并对比至Latest 文件夹
        /// </summary>
        [InspectorName("将资源包合并对比至Latest")] public bool MergeToLatest;

        private static ASBuildConfig _instance;

        /// <summary>
        /// 获取本地资源包地址
        /// </summary>
        public static ASBuildConfig GetOrCreate()
        {
            if (_instance != null) return _instance;
            var objects = EHelper.IO.GetScriptableObjects<ASBuildConfig>();
            if (objects != null && objects.Length > 0)
            {
                foreach (var asset in objects)
                {
                    if (asset is null) continue;
                    if (string.IsNullOrEmpty(asset.BuildOutputPath))
                    {
                        asset.BuildOutputPath = Path.Combine(EHelper.Path.Project, "Bundles");
                    }

                    _instance = asset;
                    break;
                }
            }

            if (_instance is null)
            {
                _instance = CreateInstance<ASBuildConfig>();
                _instance.BuildOutputPath = Path.Combine(EHelper.Path.Project, "Bundles");
                AssetDatabase.CreateAsset(_instance, "Assets/Editor/ASBuildConfig.asset");
                AssetDatabase.SaveAssets();
            }

            return _instance;
        }

        public void Save()
        {
            if (Equals(null)) return;
            EditorUtility.SetDirty(this);
#if UNITY_2021_1_OR_NEWER
            AssetDatabase.SaveAssetIfDirty(this);
#endif
        }
    }
}