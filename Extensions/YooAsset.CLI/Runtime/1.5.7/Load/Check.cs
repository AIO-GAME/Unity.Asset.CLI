#if SUPPORT_YOOASSET_157

using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine.Scripting;
using YooAsset;

namespace AIO.UEngine.YooAsset
{
    /// <summary>
    /// Check
    /// </summary>
    internal static class Check
    {
        [Preserve]
        public static bool CheckSync(this OperationHandleBase operation)
        {
            if (!operation.IsValid)
            {
                AssetSystem.LOG.E("操作句柄失效 -> {0}", operation.LastError);
                return false;
            }

            if (operation.Status == EOperationStatus.Failed)
            {
                AssetSystem.LOG.E("资源加载失败 -> {0}", operation.LastError);
                return false;
            }

            return true;
        }

        [Preserve]
        public static async Task<bool> CheckTask(this OperationHandleBase operation)
        {
            if (!operation.IsValid)
            {
                AssetSystem.LOG.E(operation.LastError);
                return false;
            }

            await operation.Task;
            if (operation.Status != EOperationStatus.Succeed)
            {
                AssetSystem.LOG.E(operation.LastError);
                return false;
            }

            return true;
        }

        [Preserve]
        public static IEnumerator CheckCoroutine(this OperationHandleBase operation, Action<bool> completed)
        {
            if (!operation.IsValid)
            {
                AssetSystem.LOG.E(operation.LastError);
                completed?.Invoke(false);
                yield break;
            }

            yield return operation;
            if (operation.Status != EOperationStatus.Succeed)
            {
                AssetSystem.LOG.E(operation.LastError);
                completed?.Invoke(false);
                yield break;
            }

            completed?.Invoke(operation.Status == EOperationStatus.Succeed);
        }
    }
}
#endif