// /*|✩ - - - - - |||
// |||✩ Author:   ||| -> XINAN
// |||✩ Date:     ||| -> 2023-08-08
// |||✩ Document: ||| ->
// |||✩ - - - - - |*/
//
//
// #if SUPPORT_UNITASK
// using Cysharp.Threading.Tasks;
// using ATask = Cysharp.Threading.Tasks.UniTask;
// #else
// using System.Threading.Tasks;
// using ATask = System.Threading.Tasks.Task;
// #endif
// #if SUPPORT_YOOASSET
// using System;
// using System.Collections.Generic;
// using System.Linq;
// using System.Threading.Tasks;
// using UnityEngine;
// using YooAsset;
//
// namespace AIO.UEngine
// {
//     public partial class YAssetSystem
//     {
// //         public partial class HUHandler
// //         {
// //             private Dictionary<string, DownloaderOperation> DownloaderOperationss;
// //
// //             /// <summary>
// //             /// 总下载数量
// //             /// </summary>
// //             public int TotalDownloadCount { get; private set; }
// //
// //             /// <summary>
// //             /// 总下载大小
// //             /// </summary>
// //             public long TotalDownloadBytes { get; private set; }
// //
// //             /// <summary>
// //             /// 当前已经完成的下载总数量
// //             /// </summary>
// //             public int CurrentDownloadCount
// //             {
// //                 get { return DownloadCountList.Sum(item => item.Value); }
// //             }
// //
// //             /// <summary>
// //             /// 当前已经完成的下载总大小
// //             /// </summary>
// //             public long CurrentDownloadBytes
// //             {
// //                 get { return DownloadBytesList.Sum(item => item.Value); }
// //             }
// //
// //             /// <summary>
// //             /// 创建补丁下载器 异步
// //             /// </summary>
// //             /// <param name="downloadingMaxNumber"> 同时下载的最大数量 </param>
// //             /// <param name="failedTryAgain"> 失败后重试次数 </param>
// //             /// <param name="timeout"> 超时时间 </param>
// //             public void CreatePatchDownloader(
// //                 int downloadingMaxNumber = 50,
// //                 int failedTryAgain = 2,
// //                 int timeout = 60)
// //             {
// //                 if (Packages.Count <= 0) return;
// //                 DownloadBytesList.Clear();
// //                 foreach (var name in ManifestOperations.Keys)
// //                 {
// //                     var asset = Packages[name];
// //                     if (EnableSidePlayWithDownload) continue;
// //
// //                     var version = asset.Config.Version;
// //                     var operation = asset.CreateResourceDownloader(downloadingMaxNumber, failedTryAgain, timeout);
// //                     if (operation.TotalDownloadCount <= 0)
// //                     {
// //                         Print.LogFormat("[{0} : {1}] 无需下载更新当前资源包", asset.Config, version);
// //                         Packages.Remove(name);
// //                         continue;
// //                     }
// //
// //                     TotalDownloadCount += operation.TotalDownloadCount;
// //                     TotalDownloadBytes += operation.TotalDownloadBytes;
// //
// //                     DownloadCountList.Add(name, operation.CurrentDownloadCount);
// //                     DownloadBytesList.Add(name, operation.CurrentDownloadBytes);
// //
// //                     Print.LogFormat("创建补丁下载器，准备下载更新当前资源版本所有的资源包文件 [{0} -> {1} ] 文件数量 : {2} , 包体大小 : {3}",
// //                         asset.Config, version,
// //                         operation.TotalDownloadCount, operation.TotalDownloadBytes);
// //                     DownloaderOperationss.Add(name, operation);
// //                 }
// //             }
// //
// //             private Dictionary<string, int> DownloadCountList;
// //
// //             private Dictionary<string, long> DownloadBytesList;
// //
// //             public async ATask Download()
// //             {
// //                 if (Packages.Count <= 0) return;
// //                 AssetSystem.InvokeEvent(EASEventType.BeginDownload, string.Empty);
// //                 var tasks = new List<ATask>();
// //                 foreach (var operation in DownloaderOperationss)
// //                 {
// //                     var key = operation.Key;
// //
// //                     void OnUpdateProgress(int totalDownloadCount, int currentDownloadCount, long totalDownloadBytes, long currentDownloadBytes)
// //                     {
// //                         DownloadCountList[key] = currentDownloadCount;
// //                         DownloadBytesList[key] = currentDownloadBytes;
// //                         OnDownloadProgress?.Invoke(DownloadCountList.Values.Sum(), TotalDownloadCount, DownloadBytesList.Values.Sum(), TotalDownloadBytes);
// //                     }
// //
// //                     void OnUpdateDownloadOver(bool isSucceed)
// //                     {
// //                         if (isSucceed) AssetSystem.InvokeEvent(EASEventType.DownlandPackageSuccess, key);
// //                         else AssetSystem.InvokeEvent(EASEventType.DownlandPackageFailure, DownloaderOperationss[key].Error);
// //                     }
// //
// //                     operation.Value.OnDownloadOverCallback = OnUpdateDownloadOver;
// //                     operation.Value.OnDownloadProgressCallback = OnUpdateProgress;
// //                     operation.Value.OnDownloadErrorCallback = (filename, error) =>
// //                     {
// //                         var concat = string.Concat(filename, ":", error);
// //                         AssetSystem.InvokeEvent(EASEventType.DownlandFileFailure, concat);
// //                     };
// //
// //                     operation.Value.BeginDownload();
// // #if SUPPORT_UNITASK
// //                     tasks.Add(operation.Value.Task.AsUniTask());
// // #else
// //                     tasks.Add(operation.Value.Task);
// // #endif
// //                 }
// // #if UNITY_WEBGL
// //             foreach (var task in tasks) await task;
// // #else
// //                 await ATask.WhenAll(tasks);
// // #endif
// //                 AssetSystem.InvokeEvent(EASEventType.HotUpdateDownloadFinish, string.Empty);
// //             }
// //         }
//     }
// }
// #endif
