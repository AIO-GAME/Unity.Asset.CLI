#if SUPPORT_YOOASSET
using YooAsset;

namespace AIO.UEngine.YooAsset
{
    internal partial class YAssetSystem
    {
        /// <summary>
        /// 创建内置资源解压器
        /// </summary>
        /// <param name="tag">资源标签</param>
        /// <param name="unpackingMaxNumber">同时解压的最大文件数</param>
        /// <param name="failedTryAgain">解压失败的重试次数</param>
        public static ResourceUnpackerOperation UnPackerPatchDefault(string tag, int unpackingMaxNumber,
            int failedTryAgain)
        {
            return DefaultPackage.CreateResourceUnPacker(tag, unpackingMaxNumber, failedTryAgain);
        }

        /// <summary>
        /// 创建内置资源解压器
        /// </summary>
        /// <param name="tags">资源标签列表</param>
        /// <param name="unpackingMaxNumber">同时解压的最大文件数</param>
        /// <param name="failedTryAgain">解压失败的重试次数</param>
        public static ResourceUnpackerOperation UnPackerPatchDefault(string[] tags, int unpackingMaxNumber,
            int failedTryAgain)
        {
            return DefaultPackage.CreateResourceUnPacker(tags, unpackingMaxNumber, failedTryAgain);
        }

        /// <summary>
        /// 创建内置资源解压器
        /// </summary>
        /// <param name="unpackingMaxNumber">同时解压的最大文件数</param>
        /// <param name="failedTryAgain">解压失败的重试次数</param>
        public static ResourceUnpackerOperation CreateResourceUnPackerDefault(int unpackingMaxNumber,
            int failedTryAgain)
        {
            return DefaultPackage.CreateResourceUnPacker(unpackingMaxNumber, failedTryAgain);
        }

        /// <summary>
        /// 创建内置资源解压器
        /// </summary>
        /// <param name="package">包名</param>
        /// <param name="tag">资源标签</param>
        /// <param name="unpackingMaxNumber">同时解压的最大文件数</param>
        /// <param name="failedTryAgain">解压失败的重试次数</param>
        public static ResourceUnpackerOperation UnPackerPatch(string package, string tag, int unpackingMaxNumber,
            int failedTryAgain)
        {
            if (!Dic.TryGetValue(package, out var asset)) return null;
            return asset.CreateResourceUnPacker(tag, unpackingMaxNumber, failedTryAgain);
        }

        /// <summary>
        /// 创建内置资源解压器
        /// </summary>
        /// <param name="package">包名</param>
        /// <param name="tags">资源标签列表</param>
        /// <param name="unpackingMaxNumber">同时解压的最大文件数</param>
        /// <param name="failedTryAgain">解压失败的重试次数</param>
        public static ResourceUnpackerOperation UnPackerPatch(string package, string[] tags, int unpackingMaxNumber,
            int failedTryAgain)
        {
            if (!Dic.TryGetValue(package, out var asset)) return null;
            return asset.CreateResourceUnPacker(tags, unpackingMaxNumber, failedTryAgain);
        }

        /// <summary>
        /// 创建内置资源解压器
        /// </summary>
        /// <param name="package">包名</param>
        /// <param name="unpackingMaxNumber">同时解压的最大文件数</param>
        /// <param name="failedTryAgain">解压失败的重试次数</param>
        public static ResourceUnpackerOperation UnPackerPatch(string package, int unpackingMaxNumber,
            int failedTryAgain)
        {
            if (!Dic.TryGetValue(package, out var asset)) return null;
            return asset.CreateResourceUnPacker(unpackingMaxNumber, failedTryAgain);
        }
    }
}
#endif