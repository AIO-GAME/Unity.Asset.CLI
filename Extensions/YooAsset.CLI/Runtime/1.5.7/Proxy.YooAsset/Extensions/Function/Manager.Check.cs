/*|✩ - - - - - |||
|||✩ Author:   ||| -> xi nan
|||✩ Date:     ||| -> 2023-08-11
|||✩ Document: ||| ->
|||✩ - - - - - |*/

#if SUPPORT_UNITASK
#define UNITASK
#endif

#if SUPPORT_YOOASSET
#define YOOASSET
#endif

#if UNITASK
using ATask = Cysharp.Threading.Tasks.UniTask;
using ATaskObject = Cysharp.Threading.Tasks.UniTask<UnityEngine.Object>;
using ATaskGameObject = Cysharp.Threading.Tasks.UniTask<UnityEngine.GameObject>;
using ATaskObjectArray = Cysharp.Threading.Tasks.UniTask<UnityEngine.Object[]>;
using ATaskScene = Cysharp.Threading.Tasks.UniTask<UnityEngine.SceneManagement.Scene>;
using ATaskString = Cysharp.Threading.Tasks.UniTask<string>;
using ATaskByteArray = Cysharp.Threading.Tasks.UniTask<System.Byte[]>;
using ATaskBoolean = Cysharp.Threading.Tasks.UniTask<System.Boolean>;
#else
using ATask = System.Threading.Tasks.Task;
using ATaskObject = System.Threading.Tasks.Task<UnityEngine.Object>;
using ATaskGameObject = System.Threading.Tasks.Task<UnityEngine.GameObject>;
using ATaskObjectArray = System.Threading.Tasks.Task<UnityEngine.Object[]>;
using ATaskScene = System.Threading.Tasks.Task<UnityEngine.SceneManagement.Scene>;
using ATaskString = System.Threading.Tasks.Task<string>;
using ATaskByteArray = System.Threading.Tasks.Task<System.Byte[]>;
using ATaskBoolean = System.Threading.Tasks.Task<System.Boolean>;
#endif
#if YOOASSET
using UnityEngine;
using System;
using System.Collections;
using YooAsset;

namespace AIO.UEngine.YooAsset
{
    internal partial class YAssetSystem
    {
        private static async ATaskBoolean LoadCheckOPTask(OperationHandleBase operation)
        {
            if (!operation.IsValid)
            {
                Debug.LogError(operation.LastError);
                return false;
            }

            await operation.Task;
            if (operation.Status != EOperationStatus.Succeed)
            {
                Debug.LogError(operation.LastError);
                return false;
            }

            return true;
        }

        private static bool LoadCheckOPSync(OperationHandleBase operation)
        {
            if (!operation.IsValid)
            {
                Debug.LogErrorFormat("操作句柄失效 -> {0}", operation.LastError);
                return false;
            }

            if (operation.Status == EOperationStatus.Failed)
            {
                Debug.LogErrorFormat("资源加载失败 -> {0}", operation.LastError);
                return false;
            }

            return true;
        }

        private static IEnumerator LoadCheckOPCO(OperationHandleBase operation, Action<bool> cb)
        {
            if (!operation.IsValid)
            {
                AssetSystem.LogError(operation.LastError);
                cb?.Invoke(false);
                yield break;
            }

            yield return operation;
            if (operation.Status != EOperationStatus.Succeed)
            {
                AssetSystem.LogError(operation.LastError);
                cb?.Invoke(false);
                yield break;
            }

            cb?.Invoke(true);
        }
    }
}
#endif