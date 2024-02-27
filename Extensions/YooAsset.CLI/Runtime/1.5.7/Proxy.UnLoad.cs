/*|✩ - - - - - |||
|||✩ Author:   ||| -> xi nan
|||✩ Date:     ||| -> 2023-08-22
|||✩ Document: ||| ->
|||✩ - - - - - |*/

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
            if (ReferenceOPHandle.ContainsKey(location))
            {
                ReleaseInternal?.Invoke(ReferenceOPHandle[location], null);
                ReferenceOPHandle.Remove(location);
            }

            ReferenceOPHandle[location] = operation;
        }

        public override void HandleFree(string location)
        {
            if (ReferenceOPHandle.TryGetValue(location, out var operation))
            {
                ReleaseInternal?.Invoke(operation, null);
                ReferenceOPHandle.Remove(location);
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
                    Resources.UnloadUnusedAssets();
                    value.Package.UnloadUnusedAssets();
                }
            }
        }

        public override void UnloadUnusedAssets(bool isForce = false)
        {
            if (isForce)
            {
                foreach (var value in Dic.Values)
                    value.Package.ForceUnloadAllAssets();
            }
            else
            {
                Resources.UnloadUnusedAssets();
                foreach (var value in Dic.Values)
                    value.Package.UnloadUnusedAssets();
            }
        }

        public override async Task UnloadSceneTask(string location)
        {
            if (ReferenceOPHandle.TryGetValue(location, out var value))
            {
                if (value is SceneOperationHandle handle)
                    await handle.UnloadAsync().Task;
                ReleaseInternal?.Invoke(value, null);
                ReferenceOPHandle.Remove(location);
            }
        }

        public override IEnumerator UnloadSceneCO(string location, Action cb)
        {
            if (ReferenceOPHandle.TryGetValue(location, out var value))
            {
                if (value is SceneOperationHandle handle)
                    yield return handle.UnloadAsync();
                ReleaseInternal?.Invoke(value, null);
                ReferenceOPHandle.Remove(location);
            }
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
            catch (Exception)
            {
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