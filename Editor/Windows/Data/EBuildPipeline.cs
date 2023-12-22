/*|============|*|
|*|Author:     |*| Star fire
|*|Date:       |*| 2023-12-22
|*|E-Mail:     |*| xinansky99@foxmail.com
|*|============|*/

using UnityEngine;

namespace AIO.UEditor
{
    public enum EBuildPipeline
    {
        /// <summary>
        /// 内置打包管线
        /// </summary>
        [InspectorName("内置打包管线")] BuiltinBuildPipeline,

#if UNITY_2018_1_OR_NEWER

        /*
            01  Setup - 平台环境初始化
            SwitchToBuildPlatform
            RebuildSpriteAtlasCache

            02  玩家 Scripts - 工程源代码编译
            BuildPlayerScripts、PostScriptsCallback

            03  Dependency
            CalculateSceneDependencyData
            CalculateCustomDependencyData (UNITY_2019_3_OR_NEWER)
            CalculateAssetDependencyData
            StripUnusedSpriteSources
            PostDependencyCallback

            04  Packing
            GenerateBundlePacking
            GenerateBundleCommands
            GenerateSubAssetPathMaps
            GenerateBundleMaps
            PostPackingCallback

            05  Writing

            WriteSerializedFiles
            ArchiveAndCompressBundles
            AppendBundleHash
            GenerateLinkXml
            PostWritingCallback

            06  Generate manifest files
         */

        /// <summary>
        /// 自定义打包管线
        /// </summary>
        [InspectorName("自定义打包管线(需安装)")] ScriptableBuildPipeline,
#endif
    }
}