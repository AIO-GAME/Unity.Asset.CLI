// /*|✩ - - - - - |||
// |||✩ Author:   ||| -> XINAN
// |||✩ Date:     ||| -> 2023-08-08
// |||✩ Document: ||| ->
// |||✩ - - - - - |*/
//
// #if SUPPORT_UNITASK
// using ATask = Cysharp.Threading.Tasks.UniTask;
// #else
// using ATask = System.Threading.Tasks.Task;
// #endif
// #if SUPPORT_YOOASSET
// using System.Collections.Generic;
// using System.Threading.Tasks;
// using UnityEngine;
// using YooAsset;
//
// namespace AIO.UEngine
// {
//     public partial class YAssetSystem
//     {
//         public partial class HUHandler
//         {
//             private Dictionary<string, UpdatePackageManifestOperation> ManifestOperations;
//
//             /// <summary>
//             /// 向网络端请求并更新补丁清单 异步
//             /// </summary>
//             public async ATask UpdatePackageManifestAsync(bool autoSaveVersion = true, int timeout = 60)
//             {
//                 AssetSystem.InvokeEvent(EASEventType.UpdatePackageManifest, string.Empty);
//                 if (Packages.Count <= 0) return;
//                 foreach (var asset in Packages.Values)
//                 {
//                     var version = asset.Config.Version;
//                     Print.LogFormat("向网络端请求并更新补丁清单 -> [{0} -> {1}] ", asset.Config.Name, version);
//                     var opManifest = asset.UpdatePackageManifestAsync(version, autoSaveVersion, timeout);
//                     ManifestOperations.Add(asset.Config.Name, opManifest);
//                     await opManifest.Task;
//                     switch (opManifest.Status)
//                     {
//                         case EOperationStatus.Succeed:
//                             break;
//                         default:
//                             Print.ErrorFormat("[{0} -> {1} : {2}] -> {3}", asset.Config.Name, version, opManifest.Status, opManifest.Error);
//                             Packages.Remove(asset.Config.Name);
//                             return;
//                     }
//                 }
//             }
//         }
//     }
// }
// #endif
