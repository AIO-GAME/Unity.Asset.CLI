﻿// using System.Collections.Generic;
// using UnityEngine;
// using YooAsset;
//
// public class LoadAssetsByTagOperation<TObject> : GameAsyncOperation
// where TObject : Object
// {
//     private readonly string                     _tag;
//     private          List<AssetOperationHandle> _handles;
//     private          ESteps                     _steps = ESteps.None;
//
//     public LoadAssetsByTagOperation(string tag) { _tag = tag; }
//
//     /// <summary>
//     ///     资源对象集合
//     /// </summary>
//     public List<TObject> AssetObjects { private set; get; }
//
//     protected override void OnStart() { _steps = ESteps.LoadAssets; }
//
//     protected override void OnUpdate()
//     {
//         if (_steps == ESteps.None || _steps == ESteps.Done)
//             return;
//
//         if (_steps == ESteps.LoadAssets)
//         {
//             var assetInfos = YooAssets.GetAssetInfos(_tag);
//             _handles = new List<AssetOperationHandle>(assetInfos.Length);
//             foreach (var assetInfo in assetInfos)
//             {
//                 var handle = YooAssets.LoadAssetAsync(assetInfo);
//                 _handles.Add(handle);
//             }
//
//             _steps = ESteps.CheckResult;
//         }
//
//         if (_steps == ESteps.CheckResult)
//         {
//             var index = 0;
//             foreach (var handle in _handles)
//             {
//                 if (handle.IsDone == false)
//                 {
//                     Progress = (float)index / _handles.Count;
//                     return;
//                 }
//
//                 index++;
//             }
//
//             AssetObjects = new List<TObject>(_handles.Count);
//             foreach (var handle in _handles)
//                 if (handle.Status == EOperationStatus.Succeed)
//                 {
//                     var assetObject = handle.AssetObject as TObject;
//                     if (assetObject != null)
//                     {
//                         AssetObjects.Add(assetObject);
//                     }
//                     else
//                     {
//                         var error = $"资源类型转换失败：{handle.AssetObject.name}";
//                         Debug.LogError($"{error}");
//                         AssetObjects.Clear();
//                         SetFinish(false, error);
//                         return;
//                     }
//                 }
//                 else
//                 {
//                     Debug.LogError($"{handle.LastError}");
//                     AssetObjects.Clear();
//                     SetFinish(false, handle.LastError);
//                     return;
//                 }
//
//             SetFinish(true);
//         }
//     }
//
//     private void SetFinish(bool succeed, string error = "")
//     {
//         Error  = error;
//         Status = succeed ? EOperationStatus.Succeed : EOperationStatus.Failed;
//         _steps = ESteps.Done;
//     }
//
//     /// <summary>
//     ///     释放资源句柄
//     /// </summary>
//     public void ReleaseHandle()
//     {
//         foreach (var handle in _handles) handle.Release();
//
//         _handles.Clear();
//     }
//
//     #region Nested type: ESteps
//
//     private enum ESteps
//     {
//         None,
//         LoadAssets,
//         CheckResult,
//         Done
//     }
//
//     #endregion
// }