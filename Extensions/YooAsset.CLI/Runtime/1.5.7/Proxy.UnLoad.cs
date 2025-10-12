#if SUPPORT_YOOASSET
using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.Scripting;
using YooAsset;

namespace AIO.UEngine.YooAsset
{
    partial class Proxy
    {
        [Preserve]
        private T HandleGet<T>(in string location)
        where T : OperationHandleBase
        {
            return ReferenceOPHandle.TryGetValue(location, out var operation)
                ? (T)operation
                : null;
        }

        [Preserve]
        private void HandleAdd<T>(in string location, T operation)
        where T : OperationHandleBase
        {
            if (operation is null) return;
            if (ReferenceOPHandle.ContainsKey(location))
            {
                ReleaseOperationHandle(ReferenceOPHandle[location]);
                ReferenceOPHandle[location] = null;
                       AssetSystem.LOG.I(string.Intern("Free Asset Handle Release : {0}"), location);
            }

            ReferenceOPHandle[location] = operation;
        }

        [Preserve]
        public override void HandleFree(string location)
        {
            if (ReferenceOPHandle.TryGetValue(location, out var operation))
            {
                ReleaseOperationHandle(operation);
                ReferenceOPHandle.Remove(location);
                       AssetSystem.LOG.I(string.Intern("Free Asset Handle Release : {0}"), location);
            }
        }

        /// <summary>
        /// 资源回收（卸载引用计数为零的资源）
        /// </summary>
        /// <param name="packageName">指定包名</param>
        /// <param name="isForce">是否强制回收</param>
        [Preserve]
        public void UnloadUnusedAssets(string packageName, bool isForce = false)
        {
            if (!Dic.TryGetValue(packageName, out var value)) return;
            if (isForce) value.Package.ForceUnloadAllAssets();
            else
                Runner.StartCoroutine(UnloadUnusedAssetsCo, (Action<AsyncOperation>)delegate
                {
                    value.Package.UnloadUnusedAssets();
                           AssetSystem.LOG.I(string.Intern("Free Asset Handle Release : {0}"), packageName);
                });
        }

        [Preserve]
        public override void UnloadUnusedAssets(bool isForce = false)
        {
            if (isForce)
            {
                ReferenceOPHandle.Clear();
                Dic.Values.ToList().ForEach(value => value.Package.ForceUnloadAllAssets());
            }
            else
            {
                ReferenceOPHandle.Where(pair => !pair.Value.IsValid).
                                  Where(pair => pair.Value.Status != EOperationStatus.Succeed).
                                  Where(pair => pair.Value.Status != EOperationStatus.Processing).
                                  Select(pair => pair.Key).
                                  ToList().
                                  ForEach(item => ReferenceOPHandle.Remove(item));
                Runner.StartCoroutine(UnloadUnusedAssetsCo(OnCompleted));
            }
        }

        [Preserve]
        private void OnCompleted(AsyncOperation operation)
        {
            foreach (var package in Dic.Values) package.Package.UnloadUnusedAssets();
        }

        [Preserve]
        private static IEnumerator UnloadUnusedAssetsCo(Action<AsyncOperation> completed)
        {
            var operation = Resources.UnloadUnusedAssets();
            operation.completed += completed;
            yield return operation;
        }

        [Preserve]
        private static void ReleaseOperationHandle(OperationHandleBase operation)
        {
            if (!operation.IsValid) return;
            switch (operation)
            {
                case AllAssetsOperationHandle handle:
                    handle.Dispose();
                    break;
                case RawFileOperationHandle handle:
                    handle.Dispose();
                    break;
                case SubAssetsOperationHandle handle:
                    handle.Dispose();
                    break;
                case AssetOperationHandle handle:
                    handle.Dispose();
                    break;
                case SceneOperationHandle handle:
                    if (!handle.IsMainScene()) handle.UnloadAsync();
                    break;
            }
        }
    }
}
#endif