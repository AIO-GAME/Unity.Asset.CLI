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
        public static ResourceUnpackerOperation UnpackerPatchDefault(string tag, int unpackingMaxNumber, int failedTryAgain)
        {
            return DefaultPackage.CreateResourceUnpacker(tag, unpackingMaxNumber, failedTryAgain);
        }

        /// <summary>
        /// 创建内置资源解压器
        /// </summary>
        /// <param name="tags">资源标签列表</param>
        /// <param name="unpackingMaxNumber">同时解压的最大文件数</param>
        /// <param name="failedTryAgain">解压失败的重试次数</param>
        public static ResourceUnpackerOperation UnpackerPatchDefault(string[] tags, int unpackingMaxNumber, int failedTryAgain)
        {
            return DefaultPackage.CreateResourceUnpacker(tags, unpackingMaxNumber, failedTryAgain);
        }

        /// <summary>
        /// 创建内置资源解压器
        /// </summary>
        /// <param name="unpackingMaxNumber">同时解压的最大文件数</param>
        /// <param name="failedTryAgain">解压失败的重试次数</param>
        public static ResourceUnpackerOperation CreateResourceUnpackerDefault(int unpackingMaxNumber, int failedTryAgain)
        {
            return DefaultPackage.CreateResourceUnpacker(unpackingMaxNumber, failedTryAgain);
        }

        /// <summary>
        /// 创建内置资源解压器
        /// </summary>
        /// <param name="package">包名</param>
        /// <param name="tag">资源标签</param>
        /// <param name="unpackingMaxNumber">同时解压的最大文件数</param>
        /// <param name="failedTryAgain">解压失败的重试次数</param>
        public static ResourceUnpackerOperation UnpackerPatch(string package, string tag, int unpackingMaxNumber, int failedTryAgain)
        {
            if (!Dic.TryGetValue(package, out var asset)) return null;
            return asset.CreateResourceUnpacker(tag, unpackingMaxNumber, failedTryAgain);
        }

        /// <summary>
        /// 创建内置资源解压器
        /// </summary>
        /// <param name="package">包名</param>
        /// <param name="tags">资源标签列表</param>
        /// <param name="unpackingMaxNumber">同时解压的最大文件数</param>
        /// <param name="failedTryAgain">解压失败的重试次数</param>
        public static ResourceUnpackerOperation UnpackerPatch(string package, string[] tags, int unpackingMaxNumber, int failedTryAgain)
        {
            if (!Dic.TryGetValue(package, out var asset)) return null;
            return asset.CreateResourceUnpacker(tags, unpackingMaxNumber, failedTryAgain);
        }

        /// <summary>
        /// 创建内置资源解压器
        /// </summary>
        /// <param name="package">包名</param>
        /// <param name="unpackingMaxNumber">同时解压的最大文件数</param>
        /// <param name="failedTryAgain">解压失败的重试次数</param>
        public static ResourceUnpackerOperation UnpackerPatch(string package, int unpackingMaxNumber, int failedTryAgain)
        {
            if (!Dic.TryGetValue(package, out var asset)) return null;
            return asset.CreateResourceUnpacker(unpackingMaxNumber, failedTryAgain);
        }
    }
}
#endif