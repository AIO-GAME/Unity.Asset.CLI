#if SUPPORT_YOOASSET
using System;
using System.Collections;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using YooAsset;

namespace AIO.UEngine.YooAsset
{
    public partial class Proxy
    {
        private T HandleGet<T>(string location) where T : OperationHandleBase
        {
            return ReferenceOPHandle.TryGetValue(location, out var operation)
                ? (T)operation
                : null;
        }

        private void HandleAdd<T>(string location, T operation) where T : OperationHandleBase
        {
            if (string.IsNullOrEmpty(location)) return;
            if (operation is null) return;
            if (ReferenceOPHandle.TryGetValue(location, out var value))
            {
                ReferenceOPHandle.Remove(location);
                ReleaseOperationHandle(value);
                AssetSystem.Log("Free Asset Handle Release : {0}", location);
            }

            ReferenceOPHandle[location] = operation;
        }

        public override void HandleFree(string location)
        {
            if (ReferenceOPHandle.TryGetValue(location, out var operation))
            {
                ReferenceOPHandle.Remove(location);
                ReleaseOperationHandle(operation);
                AssetSystem.Log("Free Asset Handle Release : {0}", location);
            }
        }

        /// <summary>
        /// 资源回收（卸载引用计数为零的资源）
        /// </summary>
        /// <param name="packageName">指定包名</param>
        /// <param name="isForce">是否强制回收</param>
        public void UnloadUnusedAssets(string packageName, bool isForce = false)
        {
            if (Dic.TryGetValue(packageName, out var value))
            {
                if (isForce)
                {
                    value.Package.ForceUnloadAllAssets();
                }
                else
                {
                    Runner.StartCoroutine(UnloadUnusedAssetsCo(_ =>
                    {
                        value.Package.UnloadUnusedAssets();
                        AssetSystem.Log("Free Package Handle Release : {0}", packageName);
                    }));
                }
            }
        }

        public override void UnloadUnusedAssets(bool isForce = false)
        {
            foreach (var key in ReferenceOPHandle.Keys.ToArray())
            {
                if (ReferenceOPHandle[key].IsValid) continue;
                if (ReferenceOPHandle[key].Status != EOperationStatus.Failed) continue;
                ReferenceOPHandle.Remove(key);
            }

            if (isForce)
            {
                ReferenceOPHandle.Clear();
                foreach (var value in Dic.Values)
                    value.Package.ForceUnloadAllAssets();
            }
            else
            {
                Runner.StartCoroutine(UnloadUnusedAssetsCo(_ =>
                {
                    foreach (var value in Dic.Values)
                        value.Package.UnloadUnusedAssets();
                }));
            }
        }

        private static IEnumerator UnloadUnusedAssetsCo(Action<AsyncOperation> completed)
        {
            var operation = Resources.UnloadUnusedAssets();
            operation.completed += completed;
            yield return operation;
        }

        public override async Task UnloadSceneTask(string location)
        {
            if (ReferenceOPHandle.TryGetValue(location, out var operation))
            {
                ReferenceOPHandle.Remove(location);
                if (operation is SceneOperationHandle handle)
                    await handle.UnloadAsync().Task;
                Runner.StartCoroutine(UnloadUnusedAssetsCo(_ =>
                {
                    ReleaseOperationHandle(operation);
                    AssetSystem.Log("Free Scene Handle Release : {0}", location);
                }));
            }
        }

        public override IEnumerator UnloadSceneCO(string location, Action cb)
        {
            if (ReferenceOPHandle.TryGetValue(location, out var operation))
            {
                ReferenceOPHandle.Remove(location);
                if (operation is SceneOperationHandle handle)
                    yield return handle.UnloadAsync();
                yield return Resources.UnloadUnusedAssets();

                ReleaseOperationHandle(operation);
                AssetSystem.Log("Free Scene Handle Release : {0}", location);
            }
        }

        private static void ReleaseOperationHandle(OperationHandleBase operation)
        {
            if (!operation.IsValid) return;
            switch (operation)
            {
                case AllAssetsOperationHandle handle:
                    handle.Dispose();
                    return;
                case RawFileOperationHandle handle:
                    handle.Dispose();
                    return;
                case SubAssetsOperationHandle handle:
                    handle.Dispose();
                    return;
                case AssetOperationHandle handle:
                    handle.Dispose();
                    return;
                case SceneOperationHandle handle:
                    if (!handle.IsMainScene()) handle.UnloadAsync();
                    return;
            }

            operation = null;
        }

        public override IEnumerator ClearUnusedCacheCO(Action<bool> cb)
        {
            var enumerable = Dic.Values.Select(package => package.ClearUnusedCacheFilesAsync());
            foreach (var operation in enumerable) yield return operation;
            cb?.Invoke(true);
        }

        public override async Task<bool> ClearUnusedCacheTask()
        {
            try
            {
                var enumerable = Dic.Values.Select(package => package.ClearUnusedCacheFilesAsync().Task);
#if UNITY_WEBGL
                foreach (var task in enumerable) await task;
#else
                await Task.WhenAll(enumerable);
#endif
            }
            catch (Exception e)
            {
                AssetSystem.LogException(e);
                return false;
            }

            return true;
        }

        public override IEnumerator ClearAllCacheCO(Action<bool> cb)
        {
            var enumerable = Dic.Values.Select(package => package.ClearAllCacheFilesAsync());
            foreach (var operation in enumerable) yield return operation;
            cb?.Invoke(true);
        }

        public override async Task<bool> ClearAllCacheTask()
        {
            try
            {
                var enumerable = Dic.Values.Select(package => package.ClearAllCacheFilesAsync().Task);
#if UNITY_WEBGL
                foreach (var task in enumerable) await task;
#else
                await Task.WhenAll(enumerable);
#endif
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }
    }
}
#endif