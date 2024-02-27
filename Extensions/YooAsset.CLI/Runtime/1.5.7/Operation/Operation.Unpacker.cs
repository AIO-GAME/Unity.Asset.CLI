// #if SUPPORT_YOOASSET
// using YooAsset;
//
// namespace AIO.UEngine.YooAsset
// {
//     partial class YAssetProxy
//     {
//         /// <summary>
//         /// 创建内置资源解压器
//         /// </summary>
//         /// <param name="tag">资源标签</param>
//         /// <param name="unpackingMaxNumber">同时解压的最大文件数</param>
//         /// <param name="failedTryAgain">解压失败的重试次数</param>
//         public ResourceUnpackerOperation UnPackerPatchDefault(
//             string tag,
//             int unpackingMaxNumber,
//             int failedTryAgain)
//         {
//             return DefaultPackage.CreateResourceUnPacker(tag, unpackingMaxNumber, failedTryAgain);
//         }
//
//         /// <summary>
//         /// 创建内置资源解压器
//         /// </summary>
//         /// <param name="tags">资源标签列表</param>
//         /// <param name="unpackingMaxNumber">同时解压的最大文件数</param>
//         /// <param name="failedTryAgain">解压失败的重试次数</param>
//         public ResourceUnpackerOperation UnPackerPatchDefault(
//             string[] tags,
//             int unpackingMaxNumber,
//             int failedTryAgain)
//         {
//             return DefaultPackage.CreateResourceUnPacker(tags, unpackingMaxNumber, failedTryAgain);
//         }
//
//         /// <summary>
//         /// 创建内置资源解压器
//         /// </summary>
//         /// <param name="unpackingMaxNumber">同时解压的最大文件数</param>
//         /// <param name="failedTryAgain">解压失败的重试次数</param>
//         public ResourceUnpackerOperation CreateResourceUnPackerDefault(
//             int unpackingMaxNumber,
//             int failedTryAgain)
//         {
//             return DefaultPackage.CreateResourceUnPacker(unpackingMaxNumber, failedTryAgain);
//         }
//
//         /// <summary>
//         /// 创建内置资源解压器
//         /// </summary>
//         /// <param name="packageName">包名</param>
//         /// <param name="tag">资源标签</param>
//         /// <param name="unpackingMaxNumber">同时解压的最大文件数</param>
//         /// <param name="failedTryAgain">解压失败的重试次数</param>
//         public ResourceUnpackerOperation UnPackerPatch(
//             string packageName,
//             string tag,
//             int unpackingMaxNumber,
//             int failedTryAgain)
//         {
//             return Dic.TryGetValue(packageName, out var asset)
//                 ? asset.CreateResourceUnPacker(tag, unpackingMaxNumber, failedTryAgain)
//                 : null;
//         }
//
//         /// <summary>
//         /// 创建内置资源解压器
//         /// </summary>
//         /// <param name="packageName">包名</param>
//         /// <param name="tags">资源标签列表</param>
//         /// <param name="unpackingMaxNumber">同时解压的最大文件数</param>
//         /// <param name="failedTryAgain">解压失败的重试次数</param>
//         public ResourceUnpackerOperation UnPackerPatch(
//             string packageName,
//             string[] tags,
//             int unpackingMaxNumber,
//             int failedTryAgain)
//         {
//             return Dic.TryGetValue(packageName, out var asset)
//                 ? asset.CreateResourceUnPacker(tags, unpackingMaxNumber, failedTryAgain)
//                 : null;
//         }
//
//         /// <summary>
//         /// 创建内置资源解压器
//         /// </summary>
//         /// <param name="packageName">包名</param>
//         /// <param name="unpackingMaxNumber">同时解压的最大文件数</param>
//         /// <param name="failedTryAgain">解压失败的重试次数</param>
//         public ResourceUnpackerOperation UnPackerPatch(
//             string packageName,
//             int unpackingMaxNumber,
//             int failedTryAgain)
//         {
//             return Dic.TryGetValue(packageName, out var asset)
//                 ? asset.CreateResourceUnPacker(unpackingMaxNumber, failedTryAgain)
//                 : null;
//         }
//     }
// }
// #endif