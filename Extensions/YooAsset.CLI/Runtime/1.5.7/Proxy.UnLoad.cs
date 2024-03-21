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
        [ProfilerScope]
        private T HandleGet<T>(in string location) where T : OperationHandleBase
        {
            return ReferenceOPHandle.TryGetValue(location, out var operation)
                ? (T)operation
                : null;
        }

        [ProfilerScope]
        private void HandleAdd<T>(in string location, T operation) where T : OperationHandleBase
        {
            if (operation is null) return;
            if (ReferenceOPHandle.ContainsKey(location))
            {
                ReleaseOperationHandle(ReferenceOPHandle[location]);
                ReferenceOPHandle[location] = null;
                AssetSystem.Log(string.Intern("Free Asset Handle Release : {0}"), location);
            }

            ReferenceOPHandle[location] = operation;
        }

        [ProfilerScope]
        public override void HandleFree(string location)
        {
            if (ReferenceOPHandle.TryGetValue(location, out var operation))
            {
                ReleaseOperationHandle(operation);
                ReferenceOPHandle.Remove(location);
                AssetSystem.Log(string.Intern("Free Asset Handle Release : {0}"), location);
            }
        }

        /// <summary>
        /// 资源回收（卸载引用计数为零的资源）
        /// </summary>
        /// <param name="packageName">指定包名</param>
        /// <param name="isForce">是否强制回收</param>
        [ProfilerScope]
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
                        AssetSystem.Log(string.Intern("Free Asset Handle Release : {0}"), packageName);
                    }));
                }
            }
        }

        [ProfilerScope]
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

        [ProfilerScope]
        private static IEnumerator UnloadUnusedAssetsCo(Action<AsyncOperation> completed)
        {
            var operation = Resources.UnloadUnusedAssets();
            operation.completed += completed;
            yield return operation;
        }

        [ProfilerScope]
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

        [ProfilerScope]
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

        [ProfilerScope]
        private static void ReleaseOperationHandle(OperationHandleBase operation)
        {
            if (operation.IsValid)
            {
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

            operation = null; // 释放引用
        }

        [ProfilerScope]
        public override IEnumerator ClearUnusedCacheCO(Action<bool> cb)
        {
            var enumerable = Dic.Values.Select(package => package.ClearUnusedCacheFilesAsync());
            foreach (var operation in enumerable) yield return operation;
            cb?.Invoke(true);
        }

        [ProfilerScope]
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

        [ProfilerScope]
        public override IEnumerator ClearAllCacheCO(Action<bool> cb)
        {
            var enumerable = Dic.Values.Select(package => package.ClearAllCacheFilesAsync());
            foreach (var operation in enumerable) yield return operation;
            cb?.Invoke(true);
        }

        [ProfilerScope]
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