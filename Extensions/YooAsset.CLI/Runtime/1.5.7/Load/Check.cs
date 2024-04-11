#if SUPPORT_YOOASSET_157

using System;
using System.Collections;
using System.Threading.Tasks;
using YooAsset;

namespace AIO.UEngine.YooAsset
{
    /// <summary>
    /// Check
    /// </summary>
    internal static class Check
    {
        public static bool CheckSync(this OperationHandleBase operation)
        {
            if (!operation.IsValid)
            {
                AssetSystem.LogErrorFormat("操作句柄失效 -> {0}", operation.LastError);
                return false;
            }

            if (operation.Status == EOperationStatus.Failed)
            {
                AssetSystem.LogErrorFormat("资源加载失败 -> {0}", operation.LastError);
                return false;
            }

            return true;
        }

        public static async Task<bool> CheckTask(this OperationHandleBase operation)
        {
            if (!operation.IsValid)
            {
                AssetSystem.LogError(operation.LastError);
                return false;
            }

            await operation.Task;
            if (operation.Status != EOperationStatus.Succeed)
            {
                AssetSystem.LogError(operation.LastError);
                return false;
            }

            return true;
        }

        public static IEnumerator CheckCoroutine(this OperationHandleBase operation, Action<bool> completed)
        {
            if (!operation.IsValid)
            {
                AssetSystem.LogError(operation.LastError);
                completed?.Invoke(false);
                yield break;
            }

            yield return operation;
            if (operation.Status != EOperationStatus.Succeed)
            {
                AssetSystem.LogError(operation.LastError);
                completed?.Invoke(false);
                yield break;
            }

            completed?.Invoke(operation.Status == EOperationStatus.Succeed);
        }
    }
}
#endif