// /*|✩ - - - - - |||
// |||✩ Author:   ||| -> XINAN
// |||✩ Date:     ||| -> 2023-08-09
// |||✩ Document: ||| ->
// |||✩ - - - - - |*/
//
// #if SUPPORT_YOOASSET
// using System;
// using System.Collections.Generic;
// using YooAsset;
// namespace AIO.UEngine
// {
// public partial class YAssetSystem
// {
//     /// <summary>
//     /// 资源更新处理器
//     /// </summary>
//     public partial class HUHandler : IDisposable
//     {
//         /// <summary>
//         /// 下载进度事件
//         /// [1: 当前下载数量 ]
//         /// [2: 总下载数量 ]
//         /// [3: 当前下载大小 ]
//         /// [4: 总下载大小 ]
//         /// </summary>
//         public event Action<int, int, long, long> OnDownloadProgress;
//
//         private Dictionary<string, YAssetPackage> Packages;
//
//         private HUHandler()
//         {
//             Packages = new Dictionary<string, YAssetPackage>();
//             VersionOperations = new Dictionary<string, UpdatePackageVersionOperation>();
//             ManifestOperations = new Dictionary<string, UpdatePackageManifestOperation>();
//             DownloaderOperationss = new Dictionary<string, DownloaderOperation>();
//
//             DownloadBytesList = new Dictionary<string, long>();
//             DownloadCountList = new Dictionary<string, int>();
//         }
//
//         internal HUHandler(IDictionary<string, YAssetPackage> packages) : this()
//         {
//             foreach (var package in packages) Packages.Add(package.Key, package.Value);
//         }
//
//         internal HUHandler(ICollection<YAssetPackage> packages) : this()
//         {
//             foreach (var package in packages) Packages.Add(package.PackageName, package);
//         }
//
//         internal HUHandler(YAssetPackage package) : this()
//         {
//             Packages.Add(package.PackageName, package);
//         }
//
//         public void Dispose()
//         {
//             OnDownloadProgress = null;
//
//             Packages.Clear();
//
//             VersionOperations.Clear();
//             ManifestOperations.Clear();
//             DownloaderOperationss.Clear();
//             DownloadCountList.Clear();
//             DownloadBytesList.Clear();
//         }
//     }
// }}
// #endif
