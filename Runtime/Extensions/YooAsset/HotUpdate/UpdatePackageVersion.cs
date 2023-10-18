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
//
// #if SUPPORT_YOOASSET
// using System;
// using System.Collections.Generic;
// using System.Threading.Tasks;
// using UnityEngine;
// using YooAsset;
// namespace AIO.UEngine
// {
//
// public partial class YAssetSystem
// {
//     public partial class HUHandler
//     {
//         private Dictionary<string, UpdatePackageVersionOperation> VersionOperations;
//
//         /// <summary>
//         /// 异步向网络端请求最新的资源版本
//         /// </summary>
//         /// <param name="appendTimeTicks">附加时间磋</param>
//         /// <param name="timeout">超时时间</param>
//         /// <returns>
//         /// Ture: 有更新
//         /// False: 无更新
//         /// </returns>
//         public async ATask UpdatePackageVersionAsync(bool appendTimeTicks = true, int timeout = 60)
//         {
//             AssetSystem.InvokeEvent(EASEventType.UpdatePackageVersion, string.Empty);
//             if (Packages.Count <= 0) return;
//             var tasks = new List<ATask>();
//             foreach (var asset in Packages.Values)
//             {
//                 Print.LogFormat("向网络端请求最新的资源版本 -> [{0} -> Local : {1}]", asset.PackageName, asset.Config.Version);
//                 if (asset.Mode == EPlayMode.HostPlayMode)
//                 {
//                     var opVersion = asset.UpdatePackageVersionAsync(appendTimeTicks, timeout);
//                     VersionOperations.Add(asset.Config.Name, opVersion);
// #if SUPPORT_UNITASK
//                     tasks.Add(opVersion.ToUniTask());
// #else
//                     tasks.Add(opVersion.Task);
// #endif
//                 }
//             }
//
// #if UNITY_WEBGL
//             if (tasks.Count > 0) { foreach (var task in tasks) await task; }
// #else
//             if (tasks.Count > 0) await ATask.WhenAll(tasks.ToArray());
// #endif
//             foreach (var opVersion in VersionOperations)
//             {
//                 var package = Packages[opVersion.Key];
//                 switch (opVersion.Value.Status)
//                 {
//                     case EOperationStatus.Succeed: // 本地版本与网络版本不一致
//                         var version = package.Config.Version;
//                         if (version != opVersion.Value.PackageVersion)
//                             package.Config.Version = opVersion.Value.PackageVersion;
//                         break;
//                     default:
//                         // 如果获取远端资源版本失败，说明当前网络无连接。
//                         // 在正常开始游戏之前，需要验证本地清单内容的完整性。
//                         var packageVersion = package.GetPackageVersion();
//                         var operation = package.PreDownloadContentAsync(packageVersion);
//                         await operation.Task;
//                         if (operation.Status != EOperationStatus.Succeed)
//                         {
//                             Console.WriteLine($"请检查本地网络，有新的游戏内容需要更新！-> {opVersion.Key}");
//                             break;
//                         }
//
//                         Packages.Remove(opVersion.Key);
//                         break;
//                 }
//             }
//         }
//
//         /// <summary>
//         /// 异步向网络端请求最新的资源版本
//         /// </summary>
//         /// <param name="appendTimeTicks">附加时间磋</param>
//         /// <param name="timeout">超时时间</param>
//         /// <param name="finish">
//         /// 回调
//         /// Ture: 有更新
//         /// False: 无更新
//         /// </param>
//         public async void UpdatePackageVersion(bool appendTimeTicks = true, int timeout = 60, Action finish = null)
//         {
//             if (Packages.Count <= 0) return;
//             await UpdatePackageVersionAsync(appendTimeTicks, timeout);
//             finish?.Invoke();
//         }
//     }
// }}
// #endif
