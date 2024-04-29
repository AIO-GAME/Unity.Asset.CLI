#if SUPPORT_YOOASSET
using System;
using System.Collections;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using UnityEngine;
using YooAsset;

namespace AIO.UEngine.YooAsset
{
    public partial class Proxy
    {
        private T HandleGet<T>(in string location)
        where T : OperationHandleBase
        {
            return ReferenceOPHandle.TryGetValue(location, out var operation)
                ? (T)operation
                : null;
        }


        private void HandleAdd<T>(in string location, T operation)
        where T : OperationHandleBase
        {
            if (operation is null) return;
            if (ReferenceOPHandle.ContainsKey(location))
            {
                ReleaseOperationHandle(ReferenceOPHandle[location]);
                ReferenceOPHandle[location] = null;
                AssetSystem.LogFormat(string.Intern("Free Asset Handle Release : {0}"), location);
            }

            ReferenceOPHandle[location] = operation;
        }

        public override void HandleFree(string location)
        {
            if (ReferenceOPHandle.TryGetValue(location, out var operation))
            {
                ReleaseOperationHandle(operation);
                ReferenceOPHandle.Remove(location);
                AssetSystem.LogFormat(string.Intern("Free Asset Handle Release : {0}"), location);
            }
        }

        /// <summary>
        ///     资源回收（卸载引用计数为零的资源）
        /// </summary>
        /// <param name="packageName">指定包名</param>
        /// <param name="isForce">是否强制回收</param>
        public void UnloadUnusedAssets(string packageName, bool isForce = false)
        {
            if (Dic.TryGetValue(packageName, out var value))
            {
                if (isForce)
                    value.Package.ForceUnloadAllAssets();
                else
                    Runner.StartCoroutine(UnloadUnusedAssetsCo(_ =>
                    {
                        value.Package.UnloadUnusedAssets();
                        AssetSystem.LogFormat(string.Intern("Free Asset Handle Release : {0}"), packageName);
                    }));
            }
        }

        public override void UnloadUnusedAssets(bool isForce = false)
        {
            foreach (var key in ReferenceOPHandle.Keys.ToArray())
            {
                var temp = ReferenceOPHandle[key];
                if (temp.IsValid) continue;
                var status = temp.Status;
                if (status == EOperationStatus.Succeed || status == EOperationStatus.Processing) continue;
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

        private static void ReleaseOperationHandle(OperationHandleBase operation)
        {
            if (operation.IsValid)
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

            operation = null; // 释放引用
        }


    }
}
#endif